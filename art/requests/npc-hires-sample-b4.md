# 素材请求：npc-hires-sample-b4

- 状态：疾风通过；红豆见 b5
- 提出者及提交号：Claude Code（夜间 NPC 外貌任务，逐像素验收 `npc-hires-sample-b3`：三代、伊比喜通过）。这一版只做红豆和疾风。
- **画风基准：已通过的七张** `art/deliveries/npc-hires-accepted/`（必须作为图像参考），规格同前：1 像素 = 1 屏幕像素，80×80，脚底 y=76，身高 60～62，大头比例，四分之三侧身朝右，每人不超过 20 色，`python3 scripts/pixel_noise.py` 孤立像素低于 3%（贴输出）。
- 从各自 b3 版出发修改（`art/deliveries/npc-hires-sample-b3/Anko_Idle.png`、`Hayate_Idle.png`，必须作为图像参考）：
  1. **御手洗红豆**：b3 的头发从额前垂下盖住了半张脸，**眼睛完全看不见**。改：前额只留两三缕短刘海，**整张脸露出来**（脸 15 宽），护额在额头上清楚可见；眼睛要大而亮（上眼线 + 眼白 + 紫褐瞳孔 + 高光），略眯，**一侧嘴角明显上扬的挑衅笑**；马尾保留 b3 那束翘起的形状。
  2. **月光疾风**：b3 的脸、黑眼圈都对了，保留。问题：**马甲画成了深蓝色**——改成和伊鲁卡、卡卡西一样的**绿色中忍马甲**（用 `art/deliveries/npc-hires-accepted/Iruka_Idle.png` 马甲的绿色调色板），里面深蓝忍装；背上斜背的刀柄要露出肩后。
- 交付：`Anko_Idle.png`、`Hayate_Idle.png` 放根下，源图 `source/`；`preview.png`（两人与已通过的七人并排，1 倍、3 倍）；`faces_8x.png`（头部放大 8 倍带网格和行号）。
- 交付后接入提交号：待填
