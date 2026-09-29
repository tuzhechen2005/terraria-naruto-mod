# Claude ↔ Codex 美术交接

本项目用本机共享工作区的请求文件与交付目录让 Claude Code 和 Codex CLI 直接交接美术。两边可以同时工作，但必须修改不同文件：Codex 只写对应的 `art/deliveries/<请求ID>/`，Claude 负责其他代码与最终接入。

## Claude 提请求

1. 复制 `art/requests/TEMPLATE.md` 为 `art/requests/<简短英文ID>.md`，填写用途、角色、外形、动作、尺寸、透明背景、参考素材和目标游戏文件。一个请求只处理一种资产或一组紧密相关的动作。
2. 执行 `python3 scripts/art_bridge.py submit <请求ID>`。桥接脚本会在后台启动新的 Codex CLI 会话；不需要用户转发消息，也不需要图像 API 密钥。
3. 继续处理不依赖图片的代码，定期执行 `python3 scripts/art_bridge.py status <请求ID>`。收到 `delivered` 后读取交付目录；收到 `failed` 后查看 `.art-bridge/<请求ID>.log`，按具体错误处理。

## Codex 交付图片

1. 阅读请求单及列出的现有素材，按项目像素风生成或编辑图片。源图与游戏用小图分开保存；不能把高分辨率插画直接当作游戏精灵。
2. 将源图、可用的 PNG、预览和 `DELIVERY.md` 放在 `art/deliveries/<请求ID>/`。`DELIVERY.md` 记录文件用途、尺寸、透明度、对应动作、生成提示词要点和已做的检查。
3. 检查透明边缘、像素轮廓、朝向、各帧脚底位置和缩到游戏尺寸后的辨识度。只写交付目录，不改代码或 Git 提交；桥接脚本在工作完成后写入状态文件。

## Claude 接入

1. 读取 `art/deliveries/<请求ID>/DELIVERY.md`，把游戏用 PNG 放入 `ShinobiPrototype/Content/` 的目标位置，并更新动画帧、判定和加载路径。
2. 运行 `./scripts/verify-mac.sh`，再在游戏内检查实际显示；构建通过不能代替实机美术验收。
3. 如需修改图片，创建带版本号的新请求 ID，并填写具体反馈与截图路径。完成后在请求单记录接入提交号。

桥接启动的是一个新的 Codex CLI 会话，不会唤醒当前 Codex 桌面聊天。Claude 必须主动调用 `submit`；仅写请求文件不会自动开始生图。图像生成可能需要几分钟，等待期间可继续其他开发任务。
