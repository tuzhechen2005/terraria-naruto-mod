# 素材请求：seal-jutsu-v1

- 状态：requested
- 提出者及提交号：Claude Code（规格 `specs/装备与忍术系统.spec.md`：结印系统的第一批三张卷轴）
- 资产类型：物品图标 3 个 + 招式特效 2 组
- **物品图标**（原版物品风格，2×2 屏幕像素为一美术像素，约 28×28～32×32，深色外描边，3～4 阶硬色阶，透明背景，Alpha 只有 0/255；参考原版卷轴类物品和本模组 `ShinobiPrototype/Content/Items/MissionScroll.png`）：
  - `ScrollClone.png`：分身术卷轴——米黄卷轴，封口处一个浅蓝的人形剪影成双；
  - `ScrollFireball.png`：火遁·豪火球之术卷轴——深红卷轴，封口处一团火焰纹；
  - `ScrollChidori.png`：千鸟卷轴——深蓝卷轴，封口处白蓝色闪电纹。
- **特效**（全部朝右，代码负责旋转和翻转；比物品更大、多帧、颜色鲜亮，会用发光叠加画；精度向灾厄同期武器的弹幕看齐；2×2 美术像素，透明背景，Alpha 只有 0/255）：
  - `FxGreatFireball_0`～`_3`（各约 64×64）：豪火球——一个翻滚的大火球，橙黄核心、红色外焰、暗红烟边，4 帧循环；
  - `FxFireBurst_0`～`_3`（各约 96×96）：火球炸开——由小到大再散开；
  - `FxChidori_0`～`_3`（各约 48×48）：千鸟——手掌位置一团噼啪作响的蓝白雷光，向四周放射细小电弧，4 帧循环；
  - `FxChidoriTrail_0`～`_2`（各约 96×24）：冲刺时身后拖出的一道雷光，3 帧。
- 画风：本模组已有特效 `ShinobiPrototype/Content/Projectiles/FxWind_0.png`、`FxSandWave_0.png` 的质量（必须作为图像参考）。
- 交付：全部放根下，源图 `source/`；`preview.png`：1 倍和 3 倍，深浅背景各一。
- 交付后接入提交号：待填
