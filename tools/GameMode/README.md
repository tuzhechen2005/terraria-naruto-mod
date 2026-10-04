# Mac 游戏模式：暂停后台应用

双击项目根目录的 `游戏模式.command`（或 `bin/泰拉瑞亚游戏模式.app`）。在窗口中勾选 Chrome、Codex、Visual Studio Code / Claude，点击“暂停后台并开始游戏”。默认只勾选 Chrome；选择会记住。窗口打开时不会暂停任何程序。

工具保留进程、窗口、标签页，用 `SIGSTOP` 暂停选中应用及其同用户子进程，通过 Steam 启动 tModLoader。完全退出游戏后约 12–15 秒，用 `SIGCONT` 恢复原进程，不退出、不重开应用。也可以随时点“恢复运行”；这不会关闭游戏。游戏已运行时也可开启此工具。

## 能做到什么

- 暂停应用的 CPU 活动，阻止暂停的进程继续执行和分配内存。
- **不会立即释放已经占用的 RAM，也不保证帧率改善。** 已有内存是否被压缩、换出由 macOS 决定。内存不足导致的卡顿仍需实机比较。
- 暂停期间窗口无法操作，下载、音视频、联网请求、AI 任务可能超时。先等 Codex / Claude 当前任务完成；恢复执行不保证恢复已经断开的网络连接。
- 只处理窗口中明确选择的三个应用，不按“占用高低”自动暂停其他软件。Steam、游戏、此工具与恢复保护进程保持运行；若从某个终端直接启动工具，其祖先进程也会被保护。

Apple 的[原生游戏模式](https://support.apple.com/zh-cn/105118)侧重 CPU/GPU 优先级和减少后台任务占用。此 Mac 满足 Apple silicon / macOS 14+ 条件，但 tModLoader 是否支持 macOS 原生全屏并被系统识别，需要游戏运行时确认；游戏自己的全屏设置不等于系统原生全屏。

## 恢复保护

- 每个暂停请求前原子保存 PID、启动时间、可执行文件和用户 ID。恢复时再次核对，避免 PID 被复用后误操作其他进程。
- 游戏启动超过 3 分钟仍未检测到客户端，恢复后台。扫描失败不当作游戏退出；排除服务器与构建进程，处理游戏进程短暂重启。
- 窗口关闭按钮在游戏模式期间只最小化；退出工具会先恢复后台。
- 独立恢复保护进程在窗口进程异常退出后尝试恢复。二者都被结束时，重新打开工具，点击“恢复运行”。恢复失败保留记录以便重试。
- 原本已暂停的进程不归本次工具管理，不恢复它们。停止/恢复期间发生的外部进程状态变化仍存在竞态；不要与其他暂停工具同时操作同一应用。
- 状态保存在 `~/Library/Application Support/TerrutoGameMode/`，不提交进 Git；无登录项、常驻服务、管理员权限、内存清理或系统优先级修改。

## 开发与验证

```sh
./scripts/build-game-mode.sh
./scripts/test-game-mode.sh
'tools/GameMode/bin/泰拉瑞亚游戏模式.app/Contents/MacOS/GameMode' --diagnose
```

需要 macOS 和系统 Swift 编译器（Command Line Tools）。生成的 `.app` 与测试程序位于忽略的 `bin/`，源码位于本目录。只读 `--diagnose` 不暂停、恢复、启动任何应用，也不写入状态。

测试覆盖目标进程与子进程选择、预先暂停/其他用户/工具祖先排除、客户端检测、启动超时与退出宽限、恢复记录、PID 身份验证。使用测试程序自己的心跳进程验证实际暂停/恢复，以及所有者退出后的恢复保护。实际 Chrome/Codex 暂停、Steam 游戏启动、游戏中内存/FPS 对比仍需实机验收。

信号语义参考 Apple 的 [sigaction 手册](https://developer.apple.com/library/archive/documentation/System/Conceptual/ManPages_iPhoneOS/man2/sigaction.2.html)和 [kill 手册](https://developer.apple.com/library/archive/documentation/System/Conceptual/ManPages_iPhoneOS/man2/kill.2.html)。工具仅发送停止/继续信号，不发送终止信号。
