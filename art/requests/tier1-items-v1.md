# 素材请求：tier1-items-v1

- 状态：requested
- 提出者及提交号：Claude Code（`specs/装备与忍术系统.spec.md` 第 1 档；用户 2026-10-04 定制作顺序：第 1 档先做完整一环）
- 资产类型：物品图标 + 挥舞/弹幕 + 合成站贴图
- 参照（必须作为图像参考）：本模组的苦无 `ShinobiPrototype/Content/Items/TrainingKunai.png`、手里剑 `ShinobiPrototype/Content/Items/NinjaTools/Shuriken.png`、斩首大刀 `ShinobiPrototype/Content/Items/Weapons/Kubikiribocho.png`（同一套武器画法）；火焰特效参照 `ShinobiPrototype/Content/Projectiles/` 里豪火球的 `FxGreatFireball_0.png`。
- 规格：原版物品与弹幕风格，2×2 屏幕像素为一美术像素，深色外描边，3～4 阶硬色阶，左上光源，透明背景，Alpha 只有 0/255。
- 文件：
  1. `TrainingNinjato.png`（修行忍刀，约 40×40，斜放，刀柄在左下）：直刃短忍刀，黑色缠绳刀柄、方形护手，刀身钢灰带一道高光；朴素的训练用刀。
  2. `NinjatoSlashArc_0`～`_2`（第三刀的刀光，约 64×64，3 帧）：一道白中带淡蓝的弧形刀光，由亮到散。
  3. `DemonChainGauntlet.png`（鬼之兄弟的锁链手甲，约 36×36）：一只带三根尖爪的金属手甲（参照鬼之兄弟的造型），后面拖着一截粗铁链。
  4. `GauntletChainLink.png`（约 10×12）：锁链的一节，可沿直线重复拼接；`GauntletClawHead.png`（约 22×22）：飞出去的爪头。
  5. `PhoenixFlowerScroll.png`（火遁·凤仙火之术的物品图标，约 28×28）：一卷打开的红色卷轴，上面一个火焰符纹（本模组的忍术物品都画成卷轴或结印，不画法杖）。
  6. `PhoenixFlowerFire_0`～`_3`（凤仙火的小火球，约 22×22，4 帧）：小而亮的橙红火球，带一点拖尾；同一时刻会有 4～5 个一起飞。
  7. `ToolWorkbench.png`（忍具台，**3×2 格的放置物块：48×32 屏幕像素，按原版家具贴图格式每格 16×16、格间 2 像素间隔，即 52×34 的贴图**）：木制工作台，台面摆着苦无、手里剑、一卷图纸和磨刀石，台下挂着工具。另交 `ToolWorkbenchItem.png`（物品图标，约 32×24）。
- 交付：全部放根下，源图 `source/`；`preview.png`：每样 1 倍和 3 倍，深浅背景各一；刀光和火球按帧排开。
- 需要避免：写实或高分辨率插画缩小；半透明杂边；凤仙火画成法杖；忍具台不是 3×2 格。
- 交付后接入提交号：待填
