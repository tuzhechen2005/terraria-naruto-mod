# 素材请求：armor-academy-v1

- 状态：requested
- 提出者及提交号：Claude Code（`specs/装备与忍术系统.spec.md` 第 1 档盔甲：学院训练服（Academy Training Gear，偏木头与防御）；用户 2026-10-04 定第二批做盔甲）
- 资产类型：盔甲（3 件物品图标 + 穿在玩家身上的 3 张装备贴图）
- 外形：忍者学校学生的修行服：头上一条**米白色布头巾**（额前打结，两条短带子垂在脑后）；上衣是**浅灰绿色的练功短上衣**，袖子挽到肘部，腰间扎一条深棕色布腰带，护臂用米白绷带缠着；下身是**深灰蓝练功裤**，膝盖下缠白色绑腿，草鞋。朴素、耐磨的训练装，色调偏浅偏暖。
- **装备贴图必须严格按原版格式**（本机模板，必须作为图像参考；只改画、不改排布和帧位置）：
  - 头：模板 `art/reference/terraria/armor/Armor_Head_22.png`（原版忍者头巾，40×1118）和 `Armor_Head_1.png`（40×1120）。交付 `AcademyTrainingHead_Head.png`，**40×1120**，20 帧纵向排列，每帧 40×56，每帧头部的位置与模板逐帧对齐（走路帧头部会上下移动，照模板）。
  - 身体：模板 `art/reference/terraria/armor/Armor_14.png`（原版忍者上衣）和 `Armor_1.png`，都是 **360×224** 的 1.4 拼合格式（躯干、前臂、后臂、肩膀分区）。交付 `AcademyTrainingBody_Body.png`，**360×224**，每个分区的像素位置与模板完全一致，只换成新衣服的配色和图案。
  - 腿：模板 `art/reference/terraria/armor/Armor_Legs_14.png`（原版忍者裤）和 `Armor_Legs_1.png`，交付 `AcademyTrainingLegs_Legs.png`，**40×1120**，20 帧，逐帧对齐模板。
- 物品图标：`AcademyTrainingHead.png`、`AcademyTrainingBody.png`、`AcademyTrainingLegs.png`（原版盔甲图标大小，约 22～30 像素见方，画成摆着的那件衣物/护具）。
- 画风：原版泰拉瑞亚盔甲的像素风，2×2 屏幕像素为一美术像素（和模板一样），深色外描边，3～4 阶硬色阶，左上光源，透明背景，Alpha 只有 0/255；配色与本模组人物一致（参考达兹纳 `art/deliveries/tazuna-npc-v1/`）。不许出现零散单点杂色。
- 交付：全部放根下，源图 `source/`；`preview.png`：把三张装备贴图按原版方式叠在一个原版玩家身上（站立、走路两帧）的预览，以及三个图标的 1 倍和 3 倍。
- 需要避免：装备贴图尺寸或分区位置和模板不一致（会在游戏里错位）；写实画法；半透明杂边。
- 交付后接入提交号：待填
