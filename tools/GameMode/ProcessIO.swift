import Foundation
import Darwin

func ps(_ columns: String) throws -> String {
    let task = Process()
    task.executableURL = URL(fileURLWithPath: "/bin/ps")
    task.arguments = ["-ww", "-axo", columns]
    let pipe = Pipe()
    task.standardOutput = pipe
    task.standardError = FileHandle.nullDevice
    try task.run()
    let data = pipe.fileHandleForReading.readDataToEndOfFile()
    task.waitUntilExit()
    guard task.terminationStatus == 0, let text = String(data: data, encoding: .utf8) else {
        throw NSError(domain: "GameMode", code: 2, userInfo: [NSLocalizedDescriptionKey: "无法读取进程状态。"])
    }
    return text
}

func liveProcesses() throws -> [Int32: LiveProcess] {
    let result = parseLiveProcesses(try ps("uid=,pid=,ppid=,lstart=,state=,comm="))
    guard result[getpid()] != nil else {
        throw NSError(domain: "GameMode", code: 3, userInfo: [NSLocalizedDescriptionKey: "进程快照不完整，无法核对身份。"])
    }
    return result
}

func scanGame() throws -> Set<Int32> {
    try gamePIDs(executables: ps("uid=,pid=,comm="), arguments: ps("uid=,pid=,args="), uid: getuid())
}

func signalVerified(_ identity: ProcessIdentity, signal: Int32) throws {
    guard identity.uid == getuid(), identity.pid > 1 else { throw POSIXError(.EPERM) }
    guard let current = try liveProcesses()[identity.pid] else { return }
    // PID reuse must not resume or pause a different process.
    guard current.identity == identity else { return }
    if kill(identity.pid, signal) != 0 && errno != ESRCH { throw POSIXError(POSIXErrorCode(rawValue: errno) ?? .EIO) }
}

func resumeSession(_ session: Session, store: SessionStore) -> [String] {
    var remaining: [PauseRecord] = []
    var errors: [String] = []
    for record in session.records.reversed() {
        do { try signalVerified(record.identity, signal: SIGCONT) }
        catch { remaining.append(record); errors.append("PID \(record.identity.pid)：\(error.localizedDescription)") }
    }
    do { try store.save(remaining.isEmpty ? nil : Session(records: remaining.reversed(), created: session.created)) }
    catch { errors.append(error.localizedDescription) }
    return errors
}

func watchParent(_ parent: Int32, store: SessionStore, interval: TimeInterval = 2) {
    let original = try? liveProcesses()[parent]?.identity
    while true {
        Thread.sleep(forTimeInterval: interval)
        guard let snapshot = try? liveProcesses() else { continue }
        if original == nil || snapshot[parent]?.identity != original {
            // Do not race a newly opened window for ownership of the recovery journal.
            let lock = open(store.directory.appendingPathComponent("session.lock").path,
                            O_CREAT | O_RDWR | O_CLOEXEC, 0o600)
            guard lock >= 0 else { return }
            defer { close(lock) }
            guard flock(lock, LOCK_EX | LOCK_NB) == 0 else { return }
            if let session = try? store.load() {
                let errors = resumeSession(session, store: store)
                if !errors.isEmpty { fputs(errors.joined(separator: "\n") + "\n", stderr) }
            }
            return
        }
    }
}
