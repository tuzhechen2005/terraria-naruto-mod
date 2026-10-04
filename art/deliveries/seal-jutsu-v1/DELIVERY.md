# seal-jutsu-v1 交付

- status: delivered
- request ID: `seal-jutsu-v1`

## 交付文件

- 物品图标（32×32）：`ScrollClone.png`、`ScrollFireball.png`、`ScrollChidori.png`
- 豪火球（64×64，4 帧）：`FxGreatFireball_0.png`～`FxGreatFireball_3.png`
- 火球炸开（96×96，4 帧）：`FxFireBurst_0.png`～`FxFireBurst_3.png`
- 千鸟（48×48，4 帧）：`FxChidori_0.png`～`FxChidori_3.png`
- 千鸟冲刺残光（96×24，3 帧）：`FxChidoriTrail_0.png`～`FxChidoriTrail_2.png`
- `preview.png`（1325×5435）：逐张展示深色、浅色背景上的 1 倍与 3 倍图像。
- `source/ScrollClone.png`（1402×1122）、`source/ScrollFireball.png`（1254×1254）、`source/ScrollChidori.png`（1346×1168）、`source/FxGreatFireball.png`、`source/FxFireBurst.png`、`source/FxChidori.png`、`source/FxChidoriTrail.png`（后四张均为 2172×724）：内置图像工具生成的原始源图。

游戏尺寸 PNG 均为 RGBA，Alpha 仅有 0 和 255；源图保留生成器的部分透明度，供后续重制。所有文件均位于本交付目录。

## 提示词概要

参照项目的任务卷轴、风与砂特效图，生成原版风格的三张斜向卷轴，分别以双人浅蓝标记、火焰标记、蓝白闪电标记区分。四组特效均朝右，使用透明背景、硬边像素块与高对比配色：翻滚的红橙火球、逐步扩大的爆炸、蓝白千鸟电弧、向左拖曳的细长雷光。生成后切帧、缩放、限制颜色并处理为 2×2 像素块。

## 已检查

- 目视检查全部源图与游戏尺寸图；在深浅背景上检查 1 倍和 3 倍预览。
- 检查三种卷轴及四组特效的轮廓、颜色区分、朝向和逐帧变化；爆炸帧按实际图形边界分割，避免相邻帧混入。
- 用脚本检查 18 张游戏尺寸 PNG 的尺寸、非空轮廓、二值 Alpha 和每个 2×2 像素块的一致性。

## 仍需游戏侧检查

- 接入物品与弹幕代码后，在 tModLoader 中查看实际缩放、发光叠加、动画帧速、碰撞点和冲刺残光的位置；必要时调整绘制偏移。
- 构建与游戏内验收未由美术工位执行。
