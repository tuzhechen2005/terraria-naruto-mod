import Foundation

struct AppChoice {
    let name: String
    let bundleID: String
    let defaultSelected: Bool
}

let appChoices = [
    AppChoice(name: "Google Chrome", bundleID: "com.google.Chrome", defaultSelected: true),
    AppChoice(name: "Codex", bundleID: "com.openai.codex", defaultSelected: false),
    AppChoice(name: "Visual Studio Code / Claude", bundleID: "com.microsoft.VSCode", defaultSelected: false)
]

struct ProcessRow {
    let uid: UInt32
    let pid: Int32
    let executable: String
    let arguments: String

    var isGameClient: Bool {
        let name = URL(fileURLWithPath: executable).lastPathComponent.lowercased()
        guard name == "dotnet" || name == "tmodloader" else { return false }
        if name == "dotnet" && arguments.range(
            of: #"(?:^|\s)(?:"[^"\n]*/|[^\s]*/)?tModLoader\.dll"?(?:\s|$)"#,
            options: [.regularExpression, .caseInsensitive]) == nil { return false }
        // Steam's auxiliary client can outlive the game window.
        if arguments.range(of: #"(?:^|\s)-terrariasteamclient"#, options: [.regularExpression, .caseInsensitive]) != nil { return false }
        return arguments.range(of: #"(?:^|\s)-(?:server|build|buildmod)(?:\s|$|=)"#,
                               options: [.regularExpression, .caseInsensitive]) == nil
    }
}

func parseProcessColumns(_ text: String) -> [Int32: (UInt32, String)] {
    var rows: [Int32: (UInt32, String)] = [:]
    for line in text.split(separator: "\n") {
        let parts = line.split(maxSplits: 2, omittingEmptySubsequences: true, whereSeparator: { $0.isWhitespace })
        if parts.count == 3, let uid = UInt32(parts[0]), let pid = Int32(parts[1]) {
            rows[pid] = (uid, String(parts[2]))
        }
    }
    return rows
}

func gamePIDs(executables: String, arguments: String, uid: UInt32) -> Set<Int32> {
    let commands = parseProcessColumns(executables)
    let args = parseProcessColumns(arguments)
    return Set(commands.compactMap { pid, row in
        guard row.0 == uid, let arg = args[pid], arg.0 == uid else { return nil }
        return ProcessRow(uid: uid, pid: pid, executable: row.1, arguments: arg.1).isGameClient ? pid : nil
    })
}

enum WatchAction: Equatable { case wait, playing, restore, startupTimeout }

enum SessionMode {
    case launchGame, pauseOnly
    func shouldLaunchGame(existingGames: Set<Int32>) -> Bool {
        self == .launchGame && existingGames.isEmpty
    }
}

struct GameWatch {
    var sawGame = false
    var absentSince: Date?
    let started: Date
    var startupTimeout: TimeInterval? = 180
    let exitGrace: TimeInterval = 12

    mutating func observe(_ pids: Set<Int32>?, now: Date) -> WatchAction {
        // Failed process scans are unknown, never evidence that the game exited.
        guard let pids = pids else { return .wait }
        if !pids.isEmpty {
            sawGame = true
            absentSince = nil
            return .playing
        }
        if !sawGame {
            return startupTimeout.map { now.timeIntervalSince(started) >= $0 } == true ? .startupTimeout : .wait
        }
        if absentSince == nil { absentSince = now }
        return now.timeIntervalSince(absentSince!) >= exitGrace ? .restore : .wait
    }
}

struct ProcessIdentity: Codable, Equatable {
    let uid: UInt32
    let pid: Int32
    let executable: String
    let started: String
}

struct LiveProcess {
    let identity: ProcessIdentity
    let parent: Int32
    let state: String
    var isStopped: Bool { state.contains("T") }
}

func parseLiveProcesses(_ text: String) -> [Int32: LiveProcess] {
    var result: [Int32: LiveProcess] = [:]
    for line in text.split(separator: "\n") {
        let columns = line.split(maxSplits: 9, omittingEmptySubsequences: true, whereSeparator: { $0.isWhitespace })
        guard columns.count == 10, let uid = UInt32(columns[0]), let pid = Int32(columns[1]),
              let parent = Int32(columns[2]) else { continue }
        let identity = ProcessIdentity(uid: uid, pid: pid, executable: String(columns[9]),
                                       started: columns[3...7].joined(separator: " "))
        result[pid] = LiveProcess(identity: identity, parent: parent, state: String(columns[8]))
    }
    return result
}

func pauseTargets(snapshot: [Int32: LiveProcess], roots: Set<Int32>, bundlePaths: [String],
                  uid: UInt32, protected: Set<Int32>) -> [LiveProcess] {
    var included = roots
    for process in snapshot.values where process.identity.uid == uid {
        if bundlePaths.contains(where: { process.identity.executable.hasPrefix($0 + "/") }) {
            included.insert(process.identity.pid)
        }
    }
    var changed = true
    while changed {
        changed = false
        for process in snapshot.values where process.identity.uid == uid {
            if included.contains(process.parent) && included.insert(process.identity.pid).inserted { changed = true }
        }
    }
    // Preserve the utility, its watcher and all its ancestors, even when started from a terminal in Codex.
    var excluded = protected
    for pid in protected {
        var current = pid
        var visited = Set<Int32>()
        while let row = snapshot[current], visited.insert(current).inserted {
            excluded.insert(current)
            current = row.parent
        }
    }
    func depth(_ process: LiveProcess) -> Int {
        var count = 0
        var current = process.identity.pid
        var visited = Set<Int32>()
        while let row = snapshot[current], included.contains(row.parent), visited.insert(current).inserted {
            count += 1; current = row.parent
        }
        return count
    }
    return snapshot.values.filter {
        included.contains($0.identity.pid) && !excluded.contains($0.identity.pid)
            && $0.identity.uid == uid && !$0.isStopped
    }.sorted { depth($0) == depth($1) ? $0.identity.pid < $1.identity.pid : depth($0) < depth($1) }
}

struct PauseRecord: Codable, Equatable {
    let bundleID: String
    let identity: ProcessIdentity
}

struct Session: Codable {
    var records: [PauseRecord]
    let created: Date
    var inputSwitch: InputSwitch? = nil
}

struct InputSwitch: Codable, Equatable {
    let originalID: String
    let englishID: String
}

struct KeyboardSource {
    let id: String
    let languages: [String]
    let isKeyboardLayout: Bool
    let selectable: Bool
}

func englishSourceID(_ sources: [KeyboardSource]) -> String? {
    let usable = sources.filter { $0.isKeyboardLayout && $0.selectable && $0.languages.contains("en") }
    for id in ["com.apple.keylayout.ABC", "com.apple.keylayout.US"] {
        if usable.contains(where: { $0.id == id }) { return id }
    }
    return usable.first?.id
}

struct SessionStore {
    let directory: URL
    var file: URL { directory.appendingPathComponent("session.json") }

    func load() throws -> Session? {
        guard FileManager.default.fileExists(atPath: file.path) else { return nil }
        let result = try JSONDecoder().decode(Session.self, from: Data(contentsOf: file))
        // Recovery only accepts records from the explicit application allowlist.
        let allowed = Set(appChoices.map(\.bundleID))
        guard result.records.allSatisfy({ allowed.contains($0.bundleID) }) else {
            throw NSError(domain: "GameMode", code: 1,
                          userInfo: [NSLocalizedDescriptionKey: "恢复记录包含不支持的应用。"])
        }
        return result
    }

    func save(_ session: Session?) throws {
        try FileManager.default.createDirectory(at: directory, withIntermediateDirectories: true,
                                               attributes: [.posixPermissions: 0o700])
        if let session = session {
            let data = try JSONEncoder().encode(session)
            try data.write(to: file, options: .atomic)
            try FileManager.default.setAttributes([.posixPermissions: 0o600], ofItemAtPath: file.path)
        } else if FileManager.default.fileExists(atPath: file.path) {
            try FileManager.default.removeItem(at: file)
        }
    }
}
