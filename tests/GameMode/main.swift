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
var manualWatch = GameWatch(started: now, startupTimeout: nil)
check(manualWatch.observe([], now: now.addingTimeInterval(3600)) == .wait, "pause-only never times out or launches a game")
check(manualWatch.observe([42], now: now.addingTimeInterval(3601)) == .playing, "pause-only attaches to a later game")
_ = manualWatch.observe([], now: now.addingTimeInterval(3602))
check(manualWatch.observe([], now: now.addingTimeInterval(3614)) == .restore, "attached game exit restores pause-only session")
check(!SessionMode.pauseOnly.shouldLaunchGame(existingGames: []), "pause-only does not launch without game")
check(!SessionMode.pauseOnly.shouldLaunchGame(existingGames: [42]), "pause-only does not launch with game")
check(!SessionMode.launchGame.shouldLaunchGame(existingGames: [42]), "launch entry attaches without a second launch")
check(SessionMode.launchGame.shouldLaunchGame(existingGames: []), "launch entry starts an absent game")
let abc = KeyboardSource(id: "com.apple.keylayout.ABC", languages: ["en"], isKeyboardLayout: true, selectable: true)
let us = KeyboardSource(id: "com.apple.keylayout.US", languages: ["en"], isKeyboardLayout: true, selectable: true)
let chinese = KeyboardSource(id: "Chinese", languages: ["zh-Hans"], isKeyboardLayout: false, selectable: true)
let vietnamese = KeyboardSource(id: "Vietnamese", languages: ["vi"], isKeyboardLayout: true, selectable: true)
check(englishSourceID([chinese, us, abc]) == abc.id, "prefer enabled ABC")
check(englishSourceID([us, chinese]) == us.id, "US fallback")
check(englishSourceID([chinese, vietnamese]) == nil, "non-English Latin layouts are not English")
check(englishSourceID([KeyboardSource(id: abc.id, languages: ["en"], isKeyboardLayout: true, selectable: false)]) == nil, "nonselectable source excluded")
let row = ProcessRow(uid: 501, pid: 5, executable: "/Steam App/dotnet/dotnet", arguments: "/Steam App/dotnet/dotnet tModLoader.dll -steam")
check(row.isGameClient, "actual Steam dotnet command")
for arg in ["tModLoader.dll -server", "tModLoader.dll -build Foo", "tModLoader.dll -buildmod=Foo", "another.dll", "tModLoader.dll -terrariasteamclient316"] {
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
// Input tests use a fake source service; the real keyboard remains untouched.
var source = "Chinese"
var selectedSources: [String] = []
var failSelection = false
let fakeInput = InputSourceService(currentID: { source }, englishID: { abc.id }, select: { id in
    if failSelection { throw NSError(domain: "test", code: 1) }
    source = id; selectedSources.append(id)
})
let change = try fakeInput.prepare()
check(change == InputSwitch(originalID: "Chinese", englishID: abc.id) && selectedSources.isEmpty, "prepare records original without changing keyboard")
try fakeInput.select(change.englishID)
let inputSession = Session(records: [], created: now, inputSwitch: change)
try store.save(inputSession)
check(try store.load()?.inputSwitch == change, "input source recovery journal roundtrip")
check(resumeSession(inputSession, store: store, input: fakeInput).isEmpty && source == "Chinese", "restore original input after English selection")
source = "Japanese"
check(resumeSession(inputSession, store: store, input: fakeInput).isEmpty && source == "Japanese", "preserve a manually chosen input source")
source = abc.id; failSelection = true
check(!resumeSession(inputSession, store: store, input: fakeInput).isEmpty, "input restoration error reported")
check(try store.load()?.inputSwitch == change, "failed input restoration remains recoverable")
failSelection = false
check(resumeSession(try store.load()!, store: store, input: fakeInput).isEmpty, "input restoration retry succeeds")
let legacy = "{\"records\":[],\"created\":0}"
try Data(legacy.utf8).write(to: store.file)
check(try store.load()?.inputSwitch == nil, "legacy journal without input source is compatible")
try store.save(nil)
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
source = abc.id; failSelection = true
let mixedSession = Session(records: session.records, created: now, inputSwitch: change)
try store.save(mixedSession)
try signalVerified(identity, signal: SIGSTOP)
awaitCondition("dummy stopped for mixed recovery") { (try? liveProcesses()[identity.pid]?.isStopped) == true }
check(!resumeSession(mixedSession, store: store, input: fakeInput).isEmpty, "mixed recovery reports failed input restoration")
check((try liveProcesses()[identity.pid]?.isStopped) == false, "input failure does not block resuming apps")
check(try store.load()?.records.isEmpty == true && store.load()?.inputSwitch == change, "retry journal retains only failed input state")
failSelection = false
check(resumeSession(try store.load()!, store: store, input: fakeInput).isEmpty, "mixed recovery retry clears journal")
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
