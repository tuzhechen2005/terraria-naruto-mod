# 素材请求：zabuza-c-frenzy-base

- 状态：requested
- 提出者及提交号：Claude Code，C 版画风全量重画第 3 批（共 4 批）
- 游戏用途：暴走阶段使用的基础动作。与第 1 批 `zabuza-c-base` 动作完全相同，唯一区别：**绷带已脱落、露出整张脸**（尖牙、充血的眼睛，可残留颈部碎绷带）。文件名在动作前加 Frenzy（如 `Zabuza_FrenzyRun.png`）。
- **画风（已定稿，必须严格一致）**：再不斩以 `art/deliveries/style-test-v3/source/Zabuza_style.png`（C 版）为标准；白以 `art/deliveries/style-test-v7-haku/source/Haku_style.png`（v7）为标准。两者同一画风：泰拉瑞亚原生像素画，2×2 屏幕像素为一个美术像素，块状、有棱角、明亮饱和、3–4 阶硬色阶、同色相深色描边，与原版角色（`art/reference/terraria/*_x4.png`）放在一起不违和。
- **尺寸**：再不斩身高 42 个美术像素，白 38 个美术像素。**按每美术像素 8×8 屏幕像素绘制**（再不斩约 336 高，白约 304 高），所有色块严格 8×8 对齐、无抗锯齿、无半透明，人物 alpha 全为 255，背景透明。
- **源图排版**：每个动作一张源图（可一张放 1–2 个动作），姿势从左到右一行排开、**互不接触**（武器也不要碰到相邻姿势），留足空隙；**每张源图最左边先画一个与标准样稿完全相同大小的待机站姿作为尺寸基准**（Claude 缩放后丢弃）。人物面朝右，各姿势与基准同一比例、脚底在同一水平线。
- **交付**：源图放 `source/`，文件名写明动作（如 `Zabuza_Run.png`）；DELIVERY.md 列出每张源图的姿势顺序。不需要交付其他尺寸，Claude 用脚本处理。
- 原作造型参考（本机，文字与图冲突时以图为准）：`art/reference/Zabuza_wields_kunai.png`（露脸主参考）、`art/reference/Zabuza_full.png`。只作参考，不得复制。
- 动作与帧数（按此顺序）：FrenzyIdle 4；FrenzyRun 6；FrenzyWindup 3；FrenzySlash 3；FrenzySeal 3；FrenzyLeap 2；FrenzyDash 2（动作含义同第 1 批）。
- 交付后接入提交号：待填
