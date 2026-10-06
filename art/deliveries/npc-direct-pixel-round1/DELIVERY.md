# 卡卡西走路修复与其他 NPC 第一批样张

状态：已交付。2026-10-05，基于 `6333e31`。用户要求修复卡卡西走路上下身分离，并用 NPC skill 开始其他角色；随后明确卡卡西的新眼睛像大黑块，要求恢复眼部。

## 用户最终选择：恢复v5

用户要求「还是刚刚那一版」。游戏Kakashi.png、Kakashi_game.png、预览与integration.json已恢复v5修眼版；v6/stable_frames及其脚本保留作实验存档，不用于当前接入。复现当前选择直接运行assemble_kakashi.py（不传final-frames）。构建不代表实机验收。

## 连续性修正实验：已撤回接入

用户指出v5闪烁与未对齐。前三/后三帧眼高差2px，头形与马甲版型跳变；只校腰线不足。下方v5内容为历史记录，视觉判断已被用户反馈否决。

- 新源图为 `../kakashi-walk-stable-v6/`：内置imagegen一次生成六姿势，固定复用认可Idle头/脖子/领口 `(20,14,52,37)`。
- **最终14帧在 `kakashi_stable_frames/`**。`stabilize_kakashi.py`对生成身体使用全循环共同32色色板，并复用认可Idle胸口内区 `(35,37,42,47)`。该区在腰髋上方，不涉及肩、手臂、骨盆、腿根和轮廓；alpha与生成身体几何不变。其余8帧byte-identical。参数见 `kakashi_stability.json`。
- PNG/GIF/APNG六帧的头部与胸口内区固定，脚底y75；游戏表翻转/+2px位移逐行核对通过。游戏Kakashi.png已接入新Walk，其他6行、头像、水牢、C#不变。
- `kakashi_walk.gif`采用公用色板；`kakashi_walk.png`为无损APNG。网页来源已更新，浏览器未实测。
- 复现时从仓库运行 `python3 art/deliveries/npc-direct-pixel-round1/stabilize_kakashi.py`，再运行 `python3 art/deliveries/npc-direct-pixel-round1/assemble_kakashi.py --final-frames art/deliveries/npc-direct-pixel-round1/kakashi_stable_frames`。默认不传final-frames仍生成旧v5，不能用于当前接入。
- 最终 `./scripts/verify-mac.sh` 通过（文本0、规则通过、构建0错误，两条旧KonohaDump警告；打包仍有未定位的未知图片类型提示，见verify-mac.log）。JS语法通过。实机和无头未运行。六个美术请求均已delivered，无后台任务。

## v5版本（用户选择保留，当前接入）

- `ShinobiPrototype/Content/NPCs/Kakashi.png`：80×960，12帧，1倍显示。仅01–06走路行更新；Idle、Jump、Sit、Throw三行与修改前相同。头像、水牢两帧与所有C#代码均未修改。
- **最终14帧来源**：`../kakashi-eye-restore-v5/frames/`。其他8帧原样，六走路帧恢复本角色已认可Idle的眉眼，不采用新的黑块眼。
- 六帧步态：v3 01、02、04 + v4 OppositeContact、OppositePassing、ReturnLift。只整体平移对齐腰轴，未把一个躯干拼到另一组腿上；白绑带腿前伸与后蹬可区分。
- `kakashi_frames/` 是修眼前的运动输入，供眼部合成脚本复现，**不用于最终游戏接入**。最终组表读取v5。
- `Kakashi_game.png` 是最终游戏表的交付副本。脚底统一由y75整体下移2像素，左右翻转后与实际游戏表逐行核对一致；头像检查图与原头像像素一致。

## 局部眼部恢复

内置imagegen局部研究输出未作为游戏帧：它改变了姿势和眼型。最终使用已认可卡卡西Idle `(36,25,47,31)` 内的像素，仅在目标也不透明时局部合成。

六帧分别修改55、51、55、56、56、56个像素，全部落在记录的眼部矩形内；alpha、身体、领口、腰髋、腿、脚底与其他8帧不变。具体矩形、源图、提示词与可复现脚本见 `../kakashi-eye-restore-v5/DELIVERY.md` 和 `compose_eyes.py`。这是卡卡西自身底稿复用，不统一其他NPC眼睛大小。

复现最终组表：先运行 `assemble_kakashi.py` 生成运动输入，再运行 `../kakashi-eye-restore-v5/compose_eyes.py` 恢复眼部，再运行 `assemble_kakashi.py` 重组最终表。脚本仅输出本交付目录，游戏安装需复制 `Kakashi_game.png`。

## 新 NPC 站姿候选

- 三代：`../hiruzen-direct-pixel-v1/`。80×80，可见高57，脚底y75，保留稍矮老人、细长眼、火影帽、白胡须与烟斗。
- 忍具店老板：`../toolshop-direct-pixel-v1/`。80×80，可见高62，脚底y75，保留单发髻、短胡须、酒红上衣与工作围裙。
- 两者均直接像素生图，经skill导出脚本确定性采样；原图、提示词、manifest、左右PNG和真实尺寸对比已保存。**均未接入或扩展动画，外观尚待用户评价。**

## 预览与实际检查

- `kakashi_walk.gif`：最终六帧，1倍与3倍；`kakashi_before_after.gif`：修改前/后3倍同步循环。
- `kakashi_walk_strip.png`：最终六帧1倍/3倍、明暗背景；`new_npc_comparison_3x.png`：伊鲁卡、卡卡西、三代、店老板。
- `preview.html`：本地预览，修眼后最终帧作为动画来源。内置浏览器工具禁止访问file协议，本轮没有浏览器页面实测；已核对静态文件引用与JS语法，并直接检查PNG。
- 最终游戏表12行与最终帧、翻转和脚底位移逐像素相符；14帧尺寸、透明RGB、0/255alpha、无裁切、非走路帧不变检查通过。新NPC的高度、脚底和左右镜像检查通过。
- `./scripts/verify-mac.sh`：最终版本通过，文本0问题、规则测试通过、完整构建0错误；模组编译仍有两条KonohaDump提示。日志 `verify-mac.log` 另含一次“Image loading failed: unknown image type”打包运行提示，来源尚未定位；本次PNG可被Pillow正确解码。构建通过不能代替游戏加载检查。
- NPC skill追加腰髋连接、交替腿和本角色眉眼对照要求，更新已接入事实；`quick_validate.py`通过。

无头加载、实际游戏走路/停止/左右朝向、光照和脚贴地：**未运行**。本轮五个桥接请求均已交付，无待执行美术任务。
