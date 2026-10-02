# 素材请求：orochimaru-fx-wind-v1

- 状态：requested
- 提出者及提交号：Claude Code（同上，大蛇丸攻击特效第 2 份）
- 资产类型：招式特效（风遁·大突破）
- 用途：大蛇丸张口吹出一股很宽的旋风，向前横扫，把玩家吹飞。代码把这组帧放大、向前推进，循环播放。
- 文件：`FxWind_0.png`～`FxWind_3.png`（各约 96×80，**朝右**吹）：几道**弯曲的白绿色气流**（白、淡青绿、灰绿三阶）卷成一股向右的旋风，里面夹着几片被卷起的**绿叶**和**土块**；4 帧之间气流向右翻卷，能首尾循环。左端（吹出的口）窄、右端宽。
- **画风**：以达兹纳 `art/deliveries/tazuna-npc-v1/` 为准——深色外描边、每种颜色 3–4 阶硬色阶、左上受光、饱和、没有零散噪点、**不要柔和渐变和半透明**；2×2 屏幕像素为一美术像素，透明背景，Alpha 只有 0/255。配色和大蛇丸本人呼应：紫、紫黑、苍白、少量金色。参考大蛇丸 `ShinobiPrototype/Content/NPCs/Orochimaru_Idle_0.png`，以及再不斩特效的做法 `ShinobiPrototype/Content/Projectiles/ZabuzaWaterDragon.png`、`ZabuzaWaterWave.png`（整块贴图，不是粒子）。
- 流程：生成像素风 → 采样到网格 → **对照生成图逐像素修整**。
- 交付：文件放交付目录根下，源图放 `source/`；`preview.png`：1 倍和 4 倍，深色和浅色两种背景，动画帧横排。
- 交付后接入提交号：待填
