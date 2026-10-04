# 素材请求：ninja-tools-v1

- 状态：requested
- 提出者及提交号：Claude Code（规格 `specs/装备与忍术系统.spec.md`：忍具与潜伏投掷，第一批）
- 资产类型：物品图标 + 弹幕 + 界面条
- 参照（必须作为图像参考）：本模组的苦无 `ShinobiPrototype/Content/Items/TrainingKunai.png` 和 `ShinobiPrototype/Content/Projectiles/KunaiThrown.png`（同一套忍具的画法）；特效质量参照 `ShinobiPrototype/Content/Projectiles/FxChidoriCharge_0.png`。
- 规格：原版物品与弹幕风格，2×2 屏幕像素为一美术像素，深色外描边，3～4 阶硬色阶，透明背景，Alpha 只有 0/255。
- 文件：
  1. `Shuriken.png`（物品图标，约 28×28）：四角手里剑，钢灰色，中间一个圆孔，刃口有高光；
  2. `ShurikenThrown_0`～`_1`（弹幕，约 18×18）：飞行中的手里剑，2 帧旋转；
  3. `ShadowShuriken_0`～`_1`（弹幕，约 56×56）：影手里剑——一枚大风魔手里剑（四片长刃、可折叠的那种），深钢色，2 帧旋转；
  4. `PaperBomb.png`（物品图标，约 20×28）：起爆符——米黄纸条，上面朱红色的“爆”字或符文，下端系着细绳；
  5. `PaperBombStuck_0`～`_1`（弹幕，约 14×20）：贴在目标上的起爆符，2 帧（符文一闪一灭，表示正在引爆）；
  6. `StealthBarFrame.png`（**60×10**）和 `StealthBarFill.png`（**52×4**）：玩家的“潜伏值”条——深紫灰底槽、细的深色描边，两端各一个小苦无尖的端头；填充从暗紫到亮紫白（3～4 阶分段），满时要醒目；填充在框内的位置交付说明写清楚。风格和 `ShinobiPrototype/Assets/UI/ChidoriBarFrame.png` 成套，但配色是潜伏的紫色（参考 `ShinobiPrototype/Content/Buffs/StealthBuff.png`）。
- 交付：全部放根下，源图 `source/`；`preview.png`：每样 1 倍和 3 倍，深浅背景各一；潜伏值条画空、一半、满三种状态。
- 交付后接入提交号：待填
