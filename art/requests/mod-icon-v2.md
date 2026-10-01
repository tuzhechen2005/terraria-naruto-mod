# 素材请求：mod-icon-v2

- 状态：integrated（icon.png 原样；icon_small.png 由 Claude 在交付图上手画 7×7 木叶标志；横幅由 Claude 用徽记源图 + Avenir Next Bold / Hiragino Sans GB 重排，修正副标题间距）
- 提出者及提交号：Claude Code（基于 `127aeed`；用户 2026-10-01：模组图标重做，要和火影相关；GitHub 主页做成带 Logo 横幅的样式）
- 资产类型：图标 + README 横幅 Logo（一组，同一个徽记）
- 用途：
  1. `ShinobiPrototype/icon.png`：tModLoader 模组图标（模组列表、创意工坊页），80×80；
  2. `ShinobiPrototype/icon_small.png`：模组列表里的小图标，30×30；
  3. GitHub 仓库首页 README 顶部居中的横幅：徽记 + 文字标志，深色和浅色两版。
- **徽记内容（三者共用，一眼看出是火影）**：一条**木叶护额**——深蓝布带、银色金属护额片，护额片正中刻着**木叶标志**（螺旋加一个尖角的叶子形状，刻痕清楚），布带两端向后飘；护额后面**交叉两把苦无**（黑色刀身、刀柄缠布、尾部圆环），刀尖向上斜指两侧。整体是泰拉瑞亚的像素风：深色外描边、每种材质 3–4 阶硬色阶、左上受光、颜色饱和；可参考 `ShinobiPrototype/Content/NPCs/Tazuna.png` 的画法、`ShinobiPrototype/Content/Items/` 里的物品图标。不要照搬官方 Logo 或官方插画。
- **icon.png / icon_small.png**：方形、背景不透明：深夜蓝到墨绿的简单渐变或纯色底，加一圈细的橙色描边（火影的橙色，约 RGB 240,140,40）；徽记居中，占画面 80% 左右。80×80 按整倍数从源图缩小，硬边无抗锯齿；30×30 单独画，保证护额和木叶标志还认得出（可以省掉苦无）。
- **README 横幅**：`banner-dark.png`（给 GitHub 深色模式：透明背景，文字米白约 RGB 242,239,232）和 `banner-light.png`（浅色模式：透明背景，文字深灰约 RGB 30,30,36），尺寸 1280×400（显示时约缩到 640 宽，所以像素块要大、清楚）。左边是徽记（像素风，约 300×300），右边是文字标志：
  - 第一行大字 **Shinobi Prototype**（英文，粗、几何感，或像素字体都可以，要和像素徽记搭）；
  - 第二行小字 **泰拉瑞亚 · 火影忍者模组**。
  - **文字必须用代码按字体渲染（PIL 之类），不要用图像生成画字**，保证拼写准确；中文用系统里的中文字体（如 PingFang、Hiragino、Noto Sans CJK）。
  - 参考版式：左图右字、两者垂直居中、四周留白，像开源项目首页那种干净的标志横幅。
- 交付：`icon.png`、`icon_small.png`、`banner-dark.png`、`banner-light.png` 放交付目录根下，源图放 `source/`；附 `preview.png`：两张横幅分别放在 GitHub 深色（RGB 13,17,23）和浅色（白）背景上，以及两个图标的 1 倍、4 倍。
- 需要避免：文字拼错、文字糊；徽记缩到 80×80 和 30×30 后认不出；半透明杂边（横幅的透明背景边缘要干净）；照搬官方素材。
- 交付后接入提交号：待填
