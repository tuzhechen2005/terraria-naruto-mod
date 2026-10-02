# 素材请求：orochimaru-fx-eyes-v1

- 状态：requested
- 提出者及提交号：Claude Code（同上，大蛇丸攻击特效第 4 份）
- 资产类型：画面叠加（杀气）
- 用途：大蛇丸放出**杀气**时，玩家被定住约 2 秒：屏幕四周泛起暗红色（代码画），画面上方浮现**一双巨大的金色蛇眼**，替身术挣脱时散开。
- 文件：`FxKillingEyes.png`（约 192×48）：一双**金色竖瞳的蛇眼**，眼周有大蛇丸那样的**紫色眼影**和一点苍白的皮肤，左右对称、眼神阴冷；只画眼睛和眼周，外围透明。缩到游戏里会放大 2～3 倍显示，所以线条要粗、色块要大。
- **画风**：以达兹纳 `art/deliveries/tazuna-npc-v1/` 为准——深色外描边、每种颜色 3–4 阶硬色阶、左上受光、饱和、没有零散噪点、**不要柔和渐变和半透明**；2×2 屏幕像素为一美术像素，透明背景，Alpha 只有 0/255。配色和大蛇丸本人呼应：紫、紫黑、苍白、少量金色。参考大蛇丸 `ShinobiPrototype/Content/NPCs/Orochimaru_Idle_0.png`，以及再不斩特效的做法 `ShinobiPrototype/Content/Projectiles/ZabuzaWaterDragon.png`、`ZabuzaWaterWave.png`（整块贴图，不是粒子）。
- 流程：生成像素风 → 采样到网格 → **对照生成图逐像素修整**。
- 交付：文件放交付目录根下，源图放 `source/`；`preview.png`：1 倍和 4 倍，深色和浅色两种背景，动画帧横排。
- 交付后接入提交号：待填
