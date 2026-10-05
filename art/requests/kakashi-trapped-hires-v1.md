# 素材请求：kakashi-trapped-hires-v1

- 状态：requested
- 提出者及提交号：Claude Code（夜间 NPC 外貌任务：卡卡西已换成高清规格，湖边水牢里被困的两帧 `ShinobiPrototype/Content/NPCs/Kakashi_Trapped_0/1.png` 还是旧画风，要跟上）
- **基准：已通过的高清卡卡西** `art/deliveries/kakashi-hires-full-v1/Kakashi_00_Idle.png` 和 `art/deliveries/kakashi-hires-full-v2/Kakashi_07_Jump.png`（必须作为图像参考）：同一个头（银色刺猬头、斜戴护额、半睁的右眼、面罩）、同样的配色（不超过 15 色）和画法，1 像素 = 1 屏幕像素。旧的两帧 `Kakashi_Trapped_0.png`、`Kakashi_Trapped_1.png` 只参考姿势。
- 内容：被困在水球里、悬在水中挣扎的卡卡西，两帧循环：
  - `Kakashi_Trapped_0.png`：身体微蜷、一只手撑向前方（推水球壁），另一只手护在胸前，腿半屈，头发和护额布条像在水里一样向上飘；
  - `Kakashi_Trapped_1.png`：同一姿势的第二帧，手和腿换个位置，头发、布条飘动变化。眼神要能看出在用力（右眼睁大一点）。
  - **不画水球**（游戏代码另外画水球），只画人。
- 规格：每帧 80×80，人物居中（中心约在 (40,40)），透明背景，Alpha 只有 0/255；`python3 scripts/pixel_noise.py` 孤立像素低于 4%（贴输出）。
- 交付：两个 PNG 放根下；`preview.png`（两帧 1 倍、4 倍，旁边放高清卡卡西站立图对比）。
- 交付后接入提交号：待填
