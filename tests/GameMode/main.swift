import Foundation
import Darwin

if CommandLine.arguments.count > 2, CommandLine.arguments[1] == "--heartbeat" {
    let file = URL(fileURLWithPath: CommandLine.arguments[2])
    var count = 0
    while true {
        count += 1
        try String(count).write(to: file, atomically: true, encoding: .utf8)
        Thread.sleep(forTimeInterval: 0.03)
    }
}
if CommandLine.arguments.count > 3, CommandLine.arguments[1] == "--watchdog-test" {
    watchParent(Int32(CommandLine.arguments[2])!, store: SessionStore(directory: URL(fileURLWithPath: CommandLine.arguments[3])), interval: 0.1)
    exit(0)
}

var passed = 0
var failureCleanup: [() -> Void] = []
func check(_ condition: @autoclosure () throws -> Bool, _ label: String) {
    guard (try? condition()) == true else {
        for cleanup in failureCleanup.reversed() { cleanup() }
        fputs("FAIL: \(label)\n", stderr); exit(1)
    }
    passed += 1
}
let now = Date(timeIntervalSince1970: 1000)
var watch = GameWatch(started: now)
check(watch.observe([], now: now.addingTimeInterval(20)) == .wait, "no premature restore before startup")
check(watch.observe(nil, now: now.addingTimeInterval(200)) == .wait, "scan failure is unknown")
check(watch.observe([5], now: now.addingTimeInterval(30)) == .playing, "game detected")
check(watch.observe([], now: now.addingTimeInterval(35)) == .wait, "exit grace")
check(watch.observe([6], now: now.addingTimeInterval(38)) == .playing, "replacement process keeps session")
check(watch.observe([], now: now.addingTimeInterval(40)) == .wait, "grace reset")
check(watch.observe([], now: now.addingTimeInterval(52)) == .restore, "restore after exit")
var timeout = GameWatch(started: now)
check(timeout.observe([], now: now.addingTimeInterval(181)) == .startupTimeout, "failed launch timeout")
let row = ProcessRow(uid: 501, pid: 5, executable: "/Steam App/dotnet/dotnet", arguments: "/Steam App/dotnet/dotnet tModLoader.dll -steam")
check(row.isGameClient, "actual Steam dotnet command")
for arg in ["tModLoader.dll -server", "tModLoader.dll -build Foo", "tModLoader.dll -buildmod=Foo", "another.dll"] {
    check(!ProcessRow(uid: 501, pid: 5, executable: "/dotnet", arguments: arg).isGameClient, "exclude server/build/unrelated")
}
check(!ProcessRow(uid: 501, pid: 5, executable: "/usr/bin/python3", arguments: "tModLoader.dll").isGameClient, "exclude scanners")
check(gamePIDs(executables: "501 5 /dotnet\n502 6 /dotnet", arguments: "501 5 dotnet tModLoader.dll\n502 6 dotnet tModLoader.dll", uid: 501) == [5], "user isolation")
let sample = "501 10 1 Sun Oct 4 10:00:00 2026 S /Applications/Browser.app/Main\n501 11 10 Sun Oct 4 10:00:01 2026 S /browser/helper\n501 12 11 Sun Oct 4 10:00:02 2026 T /already-stopped\n501 13 1 Sun Oct 4 10:00:03 2026 S /Applications/Browser.app/Helper\n502 14 10 Sun Oct 4 10:00:04 2026 S /other-user\n501 15 10 Sun Oct 4 10:00:05 2026 S /GameMode\n501 16 1 Sun Oct 4 10:00:06 2026 S /Steam"
let snapshot = parseLiveProcesses(sample)
check(snapshot.count == 7 && snapshot[13]?.identity.started == "Sun Oct 4 10:00:03 2026", "stable identity parsing")
check(pauseTargets(snapshot: snapshot, roots: [10], bundlePaths: ["/Applications/Browser.app"], uid: 501, protected: []).map(\.identity.pid) == [10, 13, 11, 15], "roots, descendants and reparented bundle helper")
check(Set(pauseTargets(snapshot: snapshot, roots: [10], bundlePaths: [], uid: 501, protected: [15]).map(\.identity.pid)) == [11], "protect tool and ancestors; skip stopped and other users")

let directory = FileManager.default.temporaryDirectory.appendingPathComponent("terruto-mode-test-\(UUID().uuidString)")
try FileManager.default.createDirectory(at: directory, withIntermediateDirectories: true)
defer { try? FileManager.default.removeItem(at: directory) }
let store = SessionStore(directory: directory)
let beat = directory.appendingPathComponent("heartbeat")
let child = Process()
child.executableURL = URL(fileURLWithPath: CommandLine.arguments[0])
child.arguments = ["--heartbeat", beat.path]
try child.run()
failureCleanup.append { kill(child.processIdentifier, SIGCONT); child.terminate(); child.waitUntilExit() }
defer { kill(child.processIdentifier, SIGCONT); child.terminate(); child.waitUntilExit() }
func awaitCondition(_ label: String, condition: () -> Bool) {
    let deadline = Date().addingTimeInterval(5)
    while Date() < deadline {
        if condition() { check(true, label); return }
        Thread.sleep(forTimeInterval: 0.04)
    }
    check(false, label)
}
func count() -> Int { Int((try? String(contentsOf: beat, encoding: .utf8)) ?? "0") ?? 0 }
awaitCondition("dummy process started") { count() > 0 }
let identity = try liveProcesses()[child.processIdentifier]!.identity
let session = Session(records: [PauseRecord(bundleID: appChoices[0].bundleID, identity: identity)], created: now)
try store.save(session)
check(try store.load()!.records == session.records, "atomic journal roundtrip")
try signalVerified(identity, signal: SIGSTOP)
awaitCondition("STOP observed") { (try? liveProcesses()[identity.pid]?.isStopped) == true }
Thread.sleep(forTimeInterval: 0.1)
let stoppedCount = count()
Thread.sleep(forTimeInterval: 0.15)
check(count() == stoppedCount, "paused dummy heartbeat remains unchanged")
check(resumeSession(session, store: store).isEmpty, "resume succeeds")
awaitCondition("CONT resumes same process") { count() > stoppedCount }
check(!FileManager.default.fileExists(atPath: store.file.path), "journal cleared after resume")
let wrong = ProcessIdentity(uid: identity.uid, pid: identity.pid, executable: identity.executable, started: "different start time")
try signalVerified(wrong, signal: SIGSTOP)
check((try liveProcesses()[identity.pid]?.isStopped) == false, "PID identity mismatch never signals")

let parent = Process()
parent.executableURL = URL(fileURLWithPath: "/bin/sleep")
parent.arguments = ["30"]
try parent.run()
let protector = Process()
protector.executableURL = child.executableURL
protector.arguments = ["--watchdog-test", String(parent.processIdentifier), directory.path]
try protector.run()
defer { if parent.isRunning { parent.terminate() }; if protector.isRunning { protector.terminate() } }
Thread.sleep(forTimeInterval: 0.2)
try store.save(session)
try signalVerified(identity, signal: SIGSTOP)
Thread.sleep(forTimeInterval: 0.1)
let crashCount = count()
parent.terminate()
parent.waitUntilExit()
awaitCondition("watchdog resumes after owner exits") { count() > crashCount && !protector.isRunning }
check(!FileManager.default.fileExists(atPath: store.file.path), "watchdog clears recovery journal")

let newOwnerLock = open(store.directory.appendingPathComponent("session.lock").path, O_CREAT | O_RDWR | O_CLOEXEC, 0o600)
check(newOwnerLock >= 0 && flock(newOwnerLock, LOCK_EX | LOCK_NB) == 0, "new owner obtains lock")
let secondParent = Process()
secondParent.executableURL = parent.executableURL
secondParent.arguments = ["30"]
try secondParent.run()
let secondProtector = Process()
secondProtector.executableURL = child.executableURL
secondProtector.arguments = ["--watchdog-test", String(secondParent.processIdentifier), directory.path]
try secondProtector.run()
failureCleanup.append { if secondParent.isRunning { secondParent.terminate() }; if secondProtector.isRunning { secondProtector.terminate() }; close(newOwnerLock) }
Thread.sleep(forTimeInterval: 0.2)
try store.save(session)
try signalVerified(identity, signal: SIGSTOP)
secondParent.terminate()
secondParent.waitUntilExit()
awaitCondition("stale watcher exits when a new owner has the lock") { !secondProtector.isRunning }
let stillPaused = try liveProcesses()[identity.pid]?.isStopped == true
check(FileManager.default.fileExists(atPath: store.file.path) && stillPaused,
      "stale watcher cannot resume new owner session")
check(resumeSession(session, store: store).isEmpty, "new owner can restore")
close(newOwnerLock)
print("PASS: \(passed) GameMode checks (only disposable test processes were suspended)")
