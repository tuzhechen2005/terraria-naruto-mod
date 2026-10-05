import AppKit
import Darwin

let gameModeStore = SessionStore(directory: FileManager.default.homeDirectoryForCurrentUser
    .appendingPathComponent("Library/Application Support/TerrutoGameMode"))

if let index = CommandLine.arguments.firstIndex(of: "--watchdog") {
    // Independent helper: if the window process crashes, suspended apps still get resumed.
    // The watcher never suspends anything and exits once its parent is gone.
    guard CommandLine.arguments.count > index + 1, let parent = Int32(CommandLine.arguments[index + 1]), parent > 1 else { exit(1) }
    watchParent(parent, store: gameModeStore)
    exit(0)
}

if CommandLine.arguments.contains("--diagnose") {
    // Read-only diagnostic mode: no quitting, launching, preferences or session writes.
    for choice in appChoices {
        let apps = NSRunningApplication.runningApplications(withBundleIdentifier: choice.bundleID)
        print("\(choice.name): \(apps.isEmpty ? "未运行" : "运行中"), pid=\(apps.map { String($0.processIdentifier) }.joined(separator: ","))")
    }
    do { print("tModLoader 客户端：\(try scanGame().sorted())") }
    catch { fputs("\(error.localizedDescription)\n", stderr); exit(1) }
    do {
        print("当前输入法：\(try InputSourceService.system.currentID())")
        print("英文键盘：\(try InputSourceService.system.englishID())")
    } catch { print(error.localizedDescription) }
    exit(0)
}

final class GameModeApp: NSObject, NSApplicationDelegate, NSWindowDelegate {
    let store = gameModeStore
    var window: NSWindow!
    var checks: [NSButton] = []
    var status = NSTextField(labelWithString: "准备好再出发")
    var detail = NSTextField(wrappingLabelWithString: "")
    var startButton: NSButton!
    var pauseOnlyButton: NSButton!
    var englishCheck: NSButton!
    var restoreButton: NSButton!
    var session: Session?
    var phase = "idle"
    var watch: GameWatch?
    var timer: Timer?
    var lockFD: Int32 = -1
    var scanBusy = false
    var watchdog: Process?
    var operation = 0
    var gameIDs = Set<Int32>()
    var gameWasFrontmost = false
    var inputWarning: String?

    func applicationDidFinishLaunching(_ notification: Notification) {
        do {
            try FileManager.default.createDirectory(at: store.directory, withIntermediateDirectories: true,
                                                   attributes: [.posixPermissions: 0o700])
            lockFD = open(store.directory.appendingPathComponent("session.lock").path, O_CREAT | O_RDWR | O_CLOEXEC, 0o600)
            guard lockFD >= 0, flock(lockFD, LOCK_EX | LOCK_NB) == 0 else {
                showError("游戏模式已经在运行", "请使用 Dock 中已有的游戏模式窗口。")
                NSApp.terminate(nil)
                return
            }
            session = try store.load()
        } catch {
            showError("无法读取恢复记录", error.localizedDescription)
            NSApp.terminate(nil)
            return
        }
        do {
            let helper = Process()
            helper.executableURL = Bundle.main.executableURL
            helper.arguments = ["--watchdog", String(getpid())]
            helper.standardOutput = FileHandle.nullDevice
            helper.standardError = FileHandle.nullDevice
            try helper.run()
            watchdog = helper
        } catch {
            showError("无法启动恢复保护", error.localizedDescription)
            NSApp.terminate(nil)
            return
        }
        buildWindow()
        if session != nil {
            phase = "recovery"
            setStatus("上次暂停的应用尚待恢复", "点击“恢复运行”，处理完上次记录后即可重新开始。")
        }
        updateControls()
        timer = Timer.scheduledTimer(withTimeInterval: 3, repeats: true) { [weak self] _ in self?.tick() }
        NSWorkspace.shared.notificationCenter.addObserver(self, selector: #selector(workspaceActivated(_:)),
            name: NSWorkspace.didActivateApplicationNotification, object: nil)
        window.makeKeyAndOrderFront(nil)
        NSApp.activate(ignoringOtherApps: true)
    }

    func text(_ value: String, size: CGFloat = 13, bold: Bool = false) -> NSTextField {
        let field = NSTextField(wrappingLabelWithString: value)
        field.font = bold ? .boldSystemFont(ofSize: size) : .systemFont(ofSize: size)
        field.setContentCompressionResistancePriority(.required, for: .vertical)
        return field
    }

    func buildWindow() {
        window = NSWindow(contentRect: NSRect(x: 0, y: 0, width: 540, height: 550),
                          styleMask: [.titled, .closable, .miniaturizable], backing: .buffered, defer: false)
        window.title = "泰拉瑞亚游戏模式"
        window.isReleasedWhenClosed = false
        window.delegate = self
        window.center()
        let menu = NSMenu()
        let root = NSMenuItem()
        let submenu = NSMenu()
        submenu.addItem(withTitle: "退出游戏模式", action: #selector(NSApplication.terminate(_:)), keyEquivalent: "q")
        root.submenu = submenu
        menu.addItem(root)
        NSApp.mainMenu = menu

        let stack = NSStackView()
        stack.orientation = .vertical
        stack.alignment = .leading
        stack.spacing = 14
        stack.translatesAutoresizingMaskIntoConstraints = false
        window.contentView!.addSubview(stack)
        NSLayoutConstraint.activate([
            stack.leadingAnchor.constraint(equalTo: window.contentView!.leadingAnchor, constant: 28),
            stack.trailingAnchor.constraint(equalTo: window.contentView!.trailingAnchor, constant: -28),
            stack.topAnchor.constraint(equalTo: window.contentView!.topAnchor, constant: 26)
        ])
        stack.addArrangedSubview(text("让后台休息一下", size: 26, bold: true))
        stack.addArrangedSubview(text("保留窗口和标签页，暂停后台应用。\n退出 tModLoader 后，自动恢复运行。"))
        stack.addArrangedSubview(text("游戏期间暂停", size: 14, bold: true))
        for (index, choice) in appChoices.enumerated() {
            let check = NSButton(checkboxWithTitle: choice.name, target: self, action: #selector(selectionChanged))
            check.tag = index
            let preference = UserDefaults.standard.object(forKey: choice.bundleID) as? Bool
            check.state = (preference ?? choice.defaultSelected) ? .on : .off
            checks.append(check)
            stack.addArrangedSubview(check)
        }
        englishCheck = NSButton(checkboxWithTitle: "进入游戏时自动切换英文输入法", target: self, action: #selector(selectionChanged))
        englishCheck.state = (UserDefaults.standard.object(forKey: "automaticEnglish") as? Bool ?? true) ? .on : .off
        stack.addArrangedSubview(englishCheck)
        let warning = text("暂停期间应用无法操作，下载和联网任务可能超时。请先等 AI 任务完成。暂停减少 CPU 活动，但不会立即释放已占内存。", size: 12)
        warning.textColor = .secondaryLabelColor
        stack.addArrangedSubview(warning)
        warning.widthAnchor.constraint(equalTo: stack.widthAnchor).isActive = true
        let line = NSBox()
        line.boxType = .separator
        stack.addArrangedSubview(line)
        line.widthAnchor.constraint(equalTo: stack.widthAnchor).isActive = true
        status.font = .boldSystemFont(ofSize: 14)
        stack.addArrangedSubview(status)
        detail.font = .systemFont(ofSize: 12)
        detail.textColor = .secondaryLabelColor
        stack.addArrangedSubview(detail)
        detail.widthAnchor.constraint(equalTo: stack.widthAnchor).isActive = true
        detail.heightAnchor.constraint(greaterThanOrEqualToConstant: 36).isActive = true
        let buttons = NSStackView()
        buttons.spacing = 12
        startButton = NSButton(title: "暂停后台并启动游戏", target: self, action: #selector(start))
        startButton.bezelStyle = .rounded
        pauseOnlyButton = NSButton(title: "仅暂停后台（不启动游戏）", target: self, action: #selector(pauseOnly))
        pauseOnlyButton.bezelStyle = .rounded
        restoreButton = NSButton(title: "恢复运行", target: self, action: #selector(restoreClicked))
        restoreButton.bezelStyle = .rounded
        buttons.addArrangedSubview(startButton)
        buttons.addArrangedSubview(pauseOnlyButton)
        stack.addArrangedSubview(buttons)
        stack.addArrangedSubview(restoreButton)
        setStatus("准备好再出发", "已在游戏里？选择“仅暂停后台”。进入游戏模式后可最小化此窗口。")
    }

    @objc func selectionChanged() {
        for (index, check) in checks.enumerated() {
            UserDefaults.standard.set(check.state == .on, forKey: appChoices[index].bundleID)
        }
        UserDefaults.standard.set(englishCheck.state == .on, forKey: "automaticEnglish")
    }

    func setStatus(_ title: String, _ message: String) {
        status.stringValue = title
        detail.stringValue = message
    }

    func updateControls() {
        let idle = phase == "idle"
        for (index, check) in checks.enumerated() {
            check.isEnabled = idle
            let choice = appChoices[index]
            let running = !NSRunningApplication.runningApplications(withBundleIdentifier: choice.bundleID).isEmpty
            let paused = session?.records.contains(where: { $0.bundleID == choice.bundleID }) == true
            check.title = choice.name + (paused ? "  ·  暂停中" : running ? "  ·  运行中" : "  ·  未运行")
        }
        startButton.isEnabled = idle
        pauseOnlyButton.isEnabled = idle
        englishCheck.isEnabled = idle
        restoreButton.isEnabled = session != nil && phase != "restoring" && phase != "preparing"
    }

    func showError(_ title: String, _ message: String) {
        let alert = NSAlert()
        alert.messageText = title
        alert.informativeText = message
        alert.alertStyle = .warning
        alert.addButton(withTitle: "知道了")
        alert.runModal()
    }

    @objc func start() {
        begin(mode: .launchGame)
    }

    @objc func pauseOnly() {
        begin(mode: .pauseOnly)
    }

    func begin(mode: SessionMode) {
        guard phase == "idle" else { return }
        guard watchdog?.isRunning == true else {
            showError("恢复保护未运行", "请退出并重新打开游戏模式。")
            return
        }
        let selected = checks.enumerated().filter { $0.element.state == .on }.map { appChoices[$0.offset] }
        let alert = NSAlert()
        alert.messageText = mode == .pauseOnly ? "仅暂停后台？" : "暂停后台并启动游戏？"
        alert.informativeText = (selected.isEmpty ? "未选择后台应用。" : "暂停：\(selected.map(\.name).joined(separator: "、"))。")
            + (mode == .pauseOnly ? "\n不会启动 Steam 或游戏。已有游戏会自动接管；没有游戏时可手动恢复。" : "\n已有游戏会自动接管；否则通过 Steam 启动 tModLoader。")
            + "\n\n程序和窗口会保留，但暂停期间无法操作，联网任务可能超时。请等 AI 任务完成再开始。暂停不会立即释放已占内存。"
        alert.addButton(withTitle: mode == .pauseOnly ? "仅暂停后台" : "暂停并启动")
        alert.addButton(withTitle: "取消")
        guard alert.runModal() == .alertFirstButtonReturn else { return }
        let existingGames: Set<Int32>
        let change: InputSwitch?
        do {
            existingGames = try scanGame()
            change = englishCheck.state == .on ? try InputSourceService.system.prepare() : nil
        } catch { showError("无法准备游戏模式", error.localizedDescription); return }
        if mode.shouldLaunchGame(existingGames: existingGames) {
            guard NSWorkspace.shared.urlForApplication(toOpen: URL(string: "steam://rungameid/1281930")!) != nil else {
                showError("找不到 Steam", "请先安装并打开 Steam，确认已经安装 tModLoader。")
                return
            }
        }
        session = Session(records: [], created: Date(), inputSwitch: change)
        do { try store.save(session) }
        catch { session = nil; showError("无法保存恢复记录", error.localizedDescription); return }
        operation += 1
        inputWarning = nil
        gameWasFrontmost = false
        gameIDs = existingGames
        phase = "preparing"
        updateControls()
        setStatus("正在暂停后台", "窗口和标签页将保留，游戏结束后恢复运行。")
        let token = operation
        let apps = selected.map { choice in
            (choice, NSRunningApplication.runningApplications(withBundleIdentifier: choice.bundleID))
        }
        let protected: Set<Int32> = [getpid(), watchdog!.processIdentifier]
        // Roots first, then a second snapshot catches children forked during the first pass.
        // Everything already stopped before this session is skipped and never resumed by us.
        do {
            if let change = change { try InputSourceService.system.select(change.englishID) }
            for _ in 0..<2 {
                let snapshot = try liveProcesses()
                let games = try scanGame()
                for (choice, runningApps) in apps {
                    let roots = Set(runningApps.map(\.processIdentifier))
                    let paths = runningApps.compactMap { $0.bundleURL?.path }
                    let targets = pauseTargets(snapshot: snapshot, roots: roots, bundlePaths: paths,
                                               uid: getuid(), protected: protected.union(games))
                    for target in targets {
                        if session!.records.contains(where: { $0.identity == target.identity }) { continue }
                        // Journal before STOP: a crash between these operations remains recoverable.
                        session!.records.append(PauseRecord(bundleID: choice.bundleID, identity: target.identity))
                        try store.save(session)
                        try signalVerified(target.identity, signal: SIGSTOP)
                    }
                }
            }
            activateSession(token: token, mode: mode)
        } catch { restore(reason: "暂停未完成，已恢复已处理的进程：\(error.localizedDescription)") }
    }

    func activateSession(token: Int, mode: SessionMode) {
        guard token == operation else { return }
        phase = mode == .pauseOnly ? "waiting" : "starting"
        watch = GameWatch(started: Date(), startupTimeout: mode == .pauseOnly ? nil : 180)
        updateControls()
        setStatus(mode == .pauseOnly ? "后台已暂停 · 未启动游戏" : "正在启动 tModLoader",
                  mode == .pauseOnly ? "点击“恢复运行”即可结束。之后检测到游戏时会自动接管，退出游戏后恢复后台。" : "首次启动或 Steam 更新可能需要稍等。超过 3 分钟未检测到游戏，会恢复运行。")
        // A failed second scan must not accidentally request a second game launch.
        let pids: Set<Int32>
        do { pids = try scanGame() }
        catch { restore(reason: "无法读取游戏状态：\(error.localizedDescription)"); return }
        gameIDs = pids
        if !pids.isEmpty {
            _ = watch?.observe(pids, now: Date())
            phase = "playing"
            setStatus("游戏模式运行中", "已接管当前运行的游戏。完全退出 tModLoader 后自动恢复后台。")
            return
        }
        guard mode.shouldLaunchGame(existingGames: pids) else { tick(); return }
        // Never run the game as a child of Codex. Steam owns the game process.
        guard NSWorkspace.shared.open(URL(string: "steam://rungameid/1281930")!) else {
            restore(reason: "Steam 未接受启动请求。")
            return
        }
        tick()
    }

    func tick() {
        updateControls()
        guard (phase == "starting" || phase == "playing" || phase == "waiting"), !scanBusy else { return }
        scanBusy = true
        let token = operation
        DispatchQueue.global(qos: .utility).async { [weak self] in
            let result = try? scanGame()
            DispatchQueue.main.async {
                guard let self = self else { return }
                self.scanBusy = false
                guard token == self.operation, self.phase == "starting" || self.phase == "playing" || self.phase == "waiting" else { return }
                if let result = result { self.gameIDs = result; self.frontmostChanged() }
                let action = self.watch?.observe(result, now: Date()) ?? .wait
                switch action {
                case .playing:
                    self.phase = "playing"
                    self.setStatus("游戏模式运行中", self.inputWarning ?? "可以最小化此窗口。完全退出 tModLoader 后，约 12–15 秒自动恢复运行。")
                case .restore: self.restore(reason: "游戏已退出。")
                case .startupTimeout: self.restore(reason: "3 分钟内未检测到 tModLoader。Steam 若仍在更新，请稍后重试。")
                case .wait:
                    if result == nil { self.setStatus("暂时无法读取游戏状态", "保持游戏模式；你仍可点击“恢复运行”。") }
                }
            }
        }
    }

    @objc func workspaceActivated(_ notification: Notification) {
        // Let macOS finish restoring the focused app's remembered input source first.
        let token = operation
        DispatchQueue.main.asyncAfter(deadline: .now() + 0.25) { [weak self] in
            guard let self = self, token == self.operation else { return }
            self.frontmostChanged()
        }
    }

    func frontmostChanged() {
        guard phase == "playing" || phase == "starting" || phase == "waiting" else { return }
        let inGame = NSWorkspace.shared.frontmostApplication.map { gameIDs.contains($0.processIdentifier) } ?? false
        defer { gameWasFrontmost = inGame }
        guard inGame, !gameWasFrontmost, let change = session?.inputSwitch else { return }
        // Apply on entering the game, allowing Chinese chat input during the same visit.
        do { try InputSourceService.system.select(change.englishID); inputWarning = nil }
        catch {
            inputWarning = error.localizedDescription
            setStatus("后台已暂停 · 请手动切换英文", error.localizedDescription)
        }
    }

    @objc func restoreClicked() {
        if phase == "playing" || phase == "starting" {
            let alert = NSAlert()
            alert.messageText = "现在恢复运行？"
            alert.informativeText = "后台应用将继续运行。游戏会继续运行。"
            alert.addButton(withTitle: "恢复运行")
            alert.addButton(withTitle: "继续游戏模式")
            guard alert.runModal() == .alertFirstButtonReturn else { return }
        }
        restore(reason: "手动结束游戏模式。")
    }

    func restore(reason: String, onSuccess: (() -> Void)? = nil) {
        guard phase != "restoring", let current = session else { return }
        operation += 1
        phase = "restoring"
        watch = nil
        gameIDs = []
        gameWasFrontmost = false
        setStatus("正在恢复运行", reason)
        updateControls()
        DispatchQueue.global(qos: .utility).async { [weak self] in
            guard let self = self else { return }
            let errors = resumeSession(current, store: self.store)
            DispatchQueue.main.async {
                do {
                    self.session = try self.store.load()
                    self.phase = self.session == nil ? "idle" : "recovery"
                    self.setStatus(self.session == nil ? "后台应用已继续运行" : "部分状态尚待恢复",
                                   errors.isEmpty ? reason + " 可以再次开始游戏模式。" : errors.joined(separator: "\n"))
                    self.updateControls()
                    if self.session == nil { onSuccess?() }
                } catch {
                    self.phase = "recovery"
                    self.setStatus("恢复记录读取失败", error.localizedDescription)
                    self.updateControls()
                }
            }
        }
    }

    func windowShouldClose(_ sender: NSWindow) -> Bool {
        if session != nil { sender.miniaturize(nil); return false }
        return true
    }

    func applicationShouldTerminateAfterLastWindowClosed(_ sender: NSApplication) -> Bool { session == nil }

    func applicationShouldHandleReopen(_ sender: NSApplication, hasVisibleWindows flag: Bool) -> Bool {
        window?.deminiaturize(nil)
        window?.makeKeyAndOrderFront(nil)
        return true
    }

    func applicationShouldTerminate(_ sender: NSApplication) -> NSApplication.TerminateReply {
        guard window != nil else { return .terminateNow }
        guard session != nil else { return .terminateNow }
        guard phase != "preparing", phase != "restoring" else {
            showError("正在处理应用", "请等待当前步骤完成，再退出游戏模式。")
            return .terminateCancel
        }
        let alert = NSAlert()
        alert.messageText = "恢复运行后退出游戏模式？"
        alert.informativeText = "退出此工具后，将停止自动检测。游戏本身会继续运行。"
        alert.addButton(withTitle: "恢复并退出")
        alert.addButton(withTitle: "继续运行")
        if alert.runModal() == .alertFirstButtonReturn {
            restore(reason: "退出游戏模式。", onSuccess: { NSApp.terminate(nil) })
        }
        return .terminateCancel
    }
}

let application = NSApplication.shared
application.setActivationPolicy(.regular)
let delegate = GameModeApp()
application.delegate = delegate
application.run()
