# 素材请求：wave-boss-vanity-v1

- 状态：delivered，已接入（scripts/build_head_equip.py 按原版头部上下浮动生成 20 帧）
- 提出者及提交号：Claude Code，M1 阶段 3（Boss 面具时装，掉率约 1/7），基于 `0d4533e`
- 资产类型：物品图标 ×2 ＋ 头部装备外观 ×2
- 游戏用途：玩家可穿戴的时装头部装备。
- **画风（已定稿，必须严格一致）**：以 `art/deliveries/style-test-v3/source/Zabuza_style.png`（C 版再不斩）与 `art/deliveries/style-test-v7-haku/source/Haku_style.png`（白）为标准：泰拉瑞亚原生像素画，2×2 屏幕像素为一个美术像素，块状、有棱角、明亮饱和、3–4 阶硬色阶、同色相深色描边；与原版角色（`art/reference/terraria/*_x4.png`）放在一起不违和。
- 内容（2×2 屏幕像素为一个美术像素；透明背景；源图放 `source/`）：
  1. **再不斩护额**：雾隐护额（金属护额板上刻四道竖直波纹，横向系在额头、**向一侧歪戴**），外加遮住口鼻的白色**绷带面罩**。交付 `ZabuzaHeadband.png`（物品图标，约 28×24）和 `ZabuzaHeadband_Head.png`（戴在头上时的样子，单帧，40×56 画布，头部位置与原版玩家头部一致：头部大约占画布 x 12–30、y 4–24 的区域，面朝右）。
  2. **白的面具**：追杀部队面具——白底椭圆面具，只露出两个细长眼孔，面具上有**红色纹样**与雾隐标志。交付 `HakuMask.png`（物品图标，约 24×28）和 `HakuMask_Head.png`（戴在头上的样子，单帧，40×56 画布，位置同上，面朝右）。
- 原作参考：`art/reference/Zabuza_Momochi.png`、`art/reference/Haku_s_shinobi_attire.png`（文字与图冲突时以图为准）。
- 需要避免的问题：戴上后位置偏出头部、细节太细、写实光影、半透明杂边。
- 交付后接入提交号：待填
