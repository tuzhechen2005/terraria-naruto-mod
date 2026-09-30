# 素材请求：water-prison-v1

- 状态：integrated
- 提出者及提交号：Claude（M1 湖边初遇，规格见 `specs/M1_波之国下忍篇.spec.md`“湖边初遇（水牢术）”）
- 资产类型：角色姿态 + 场景物体（一组紧密相关的三件）
- 游戏用途与出现时机：原作“湖边初遇”：再不斩站在湖面上，一只手伸进水球维持水牢术，卡卡西被关在水球里。玩家攻击水球，打爆后卡卡西脱困。
- 角色或物体与交付文件（全部透明背景 PNG，放 `art/deliveries/water-prison-v1/`）：
  1. **卡卡西被困**：`kakashi_trapped_0.png`、`kakashi_trapped_1.png`，每帧 56×56，与现有城镇 NPC 表 `ShinobiPrototype/Content/NPCs/Kakashi.png`（12 帧竖排，每帧 56×56）同一比例、同一配色（银白冲天发、护额斜遮左眼、黑面罩、绿色马甲、深蓝衣裤）。姿态：悬浮在水中，身体微蜷，一只手掌贴在水球内壁上推，头发和衣角轻微向上飘；两帧之间只做轻微浮动（头发 / 手的 1–2 像素差异）。面朝左（与原表一致，游戏内会水平翻转）。人物居中，身体大约占画布 40×48。
  2. **再不斩维持水牢**：`zabuza_hold_0.png`、`zabuza_hold_1.png`，每帧 288×128，与现有再不斩帧（如 `ShinobiPrototype/Content/NPCs/Zabuza_Idle_0.png`）同一画布规格：身体中线 x=144，脚底 y=124，同一比例与配色（黑衣蓝灰色阶、绷带遮口、雾隐护额斜戴、斩首大刀背在背上）。姿态：站在水面上，**朝右的一只手臂水平伸直、手掌张开向前**（手要伸到画布右侧约 x=200 处，游戏里水球左缘会压在这只手上），另一只手放在身侧或结单手印；腿微分站稳。两帧只做呼吸起伏与衣摆微动。面朝右。
  3. **水球**：`water_sphere_0.png`～`water_sphere_2.png`，每帧 96×96，三帧循环。一个圆形水球，直径约 88 像素：半透明蓝色（中心 alpha 约 35–45%，好让里面的卡卡西看得见；边缘一圈更亮更不透明的水面轮廓），左上有高光，内部少量小气泡；三帧之间水面轮廓波动、气泡上浮。
- 必须保留的外形特征：卡卡西与再不斩与现有游戏精灵一致，一眼认出是同一角色；水球是清亮的淡水蓝（不是海水深蓝、不是紫色）。
- 动作、朝向与帧数：见上。
- 背景：透明。
- 现有风格参考文件：`ShinobiPrototype/Content/NPCs/Kakashi.png`、`ShinobiPrototype/Content/NPCs/Zabuza_Idle_0.png`、`ShinobiPrototype/Content/NPCs/Zabuza_Seal_0.png`、`art/deliveries/style-test-v3/`、`art/deliveries/style-test-v7-haku/`（画风标准：泰拉瑞亚原生风格，块状、有棱角、明亮饱和）。
- 原作造型参考：`art/reference/kakashi_v1_vs_guide.png`、`art/reference/Zabuza_full.png`、`art/reference/Zabuza_Momochi.png`。文字与图冲突时以图为准。原作场景：再不斩站在湖面，右手伸进一团水球里，卡卡西被困在水球中。
- 需要避免的问题：不要高分辨率插画直接缩小的糊图；不要抗锯齿半透明杂边（水球本身的半透明除外）；再不斩与卡卡西的脚底 / 画布位置要与现有帧对齐，否则游戏里会跳动；不要文字。
- 验收方式：游戏尺寸 1× 与 2× 预览；把水球叠在卡卡西被困帧上、再把水球左缘放在再不斩伸出的手上，合成一张预览图 `preview_composite.png`，在深蓝湖面背景和浅色天空背景下都看得清。
- 交付后接入提交号：待填
