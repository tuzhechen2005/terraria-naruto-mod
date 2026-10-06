# 当前工作交接

## 2026-10-06 Codex：PR合并前审查与修复

- 用户授权审查两个PR，合格则批准并合并。PR1已以merge commit合入main（6287c067740383be71cbbd7b56834152c7f9b3c8）；其CI通过，选定卡卡西v5/伊鲁卡游戏图与当前开发图字节一致。PR2已从PR1分支改为main，独立审查工作树合入最新main后检查，无冲突，Wiki发布文件保留。
- PR2旧头5aa94a3仍带已否定鬼影与已知绘制问题。补回13f2516的般若运行时恢复、146f450的Wave私有预乘缓存、0db2f38的两处MagicPixel源矩形修复；预乘算式直接留在缓存转换内，不引入后续SlashSweepRules/斩击逻辑。更新catalog/安装脚本的混合说明、交付与美术交接，素材未重画，不改判定/玩法。
- 文字检查0问题、14组规则及完整模组编译通过，0错误、2条旧KonohaDump警告；BuildMod=false，未打包或改变当前游戏包。16张软RGBA/透明RGB与预乘数值范围通过；diff检查通过。验证日志/tmp/shinobi-two-pr-verification.log，文字检查hjson依赖仅放/tmp/shinobi-two-pr-review-deps。
- 当前账号tuzhechen2005为PR作者，GitHub拒绝自我Approve；PR1已留下审查评论，按用户授权合并。PR2本修复提交推送后再核对最终CI、留言并合并，不能把旧提交CI当作新提交已通过。
- 后续斩击/水浪实验、新skill与CLAUDE入口仍在codex/vfx-animation-study，不在本次两PR范围。完整M11、GPU美术/联机/性能未验收，助手未启动游戏，无后台美术请求。


## 2026-10-05 Codex：两个PR与M11特效（当前）

- 用户要求卡卡西回到上一版；游戏和预览已恢复v5修眼后的v3/v4步态。v6只存档，不能重新接入。首份PR： https://github.com/tuzhechen2005/terraria-naruto-mod/pull/1 ，分支codex/npc-art-and-wave-boss-design，提交f7c0159，包含NPC交付/skill/M11定稿及此前本地未推送的72个提交；未合并。
- 当前分支codex/wave-boss-vfx，从f7c0159分出。只制作M11已有特效：三桥接请求wave-water-vfx-v1、wave-ice-vfx-v1、wave-demon-vfx-v1均delivered，无后台任务。
- 当前交付入口 `art/deliveries/wave-vfx-review-v1/DELIVERY.md`。16张RGBA已装入 `ShinobiPrototype/Assets/Vfx/Wave/`；源图/提示词/短循环在三份交付，原生明暗图/组合GIF/APNG/参数/hash在review目录。
- 渲染接口 `WaveVfx.cs`，缓存预加载扩展；客户端余波 `WaveVfxBursts.cs`上限64/24帧清理，短震8帧3px/压暗18帧20%；音效 `WaveVfxAudio.cs`仅原版SoundID。客户端装饰/镜头设置中英齐全。
- 现有刀光、千本亮尖/冰痕/消散、移动残像、镜框/亮镜剪影/裂纹/碎裂/暴走寒光、鬼影/压下、旋转刀迹/接刀环、水遁聚势/水花已接入；水龙释放/破阵/暴走有镜头节点。没有修改伤害、生命、判定、Boss体型、追击、奖励或地形。
- 完整水龙头身、锁线、瞬身落点、阵列及合击接口/素材已备，需M11同步战斗逻辑用实际路径/阶段调用。当前旧78×32水龙判定未硬套巨大龙身。不能把本轮特效当完整M11重设计实现。
- `install_wave_vfx.py`与`build_wave_vfx_preview.py`可复现检查/安装/预览；16张机械检查、三份导出重跑与游戏资源字节一致、引用/24帧动画检查通过。最终verify-mac通过：文本0、14组规则通过、构建0错误；两条旧KonohaDump和基线未知图片类型提示仍在，见review/verify-mac.log。游戏/联机/性能实测未运行，组合图不是实机截图。
- 第二PR已创建： https://github.com/tuzhechen2005/terraria-naruto-mod/pull/2 ，特效实现提交aa5abb9，基底为第一PR分支，让审查只包含特效增量；先合并PR1，再将PR2基底改为main进行合并。本轮只开PR、未合并。当前工作树提交后保持干净，未来工作不要混入Boss逻辑重设计。

本文件供 Claude Code 与 Codex 在同一项目目录切换时恢复任务状态。NPC 最新进展见下方 **卡卡西走路与眼部修复、新NPC第一批样张**；Boss 重设计已整理为 **M11 定稿文档，待实施**。后面保留此前开发快照。接手时还要看用户最新消息、实际文件和 Git 状态；长期规则见 `AGENTS.md`，Mac 环境见 `DEVELOPMENT_MAC.md`。

## 2026-10-05 Codex：卡卡西走路连续性再次修复（已接入，未实机）

**后续决定：用户要求保留刚刚上一版，即v5修眼后的v3/v4循环。已恢复游戏贴图和预览到v5。v6与stable_frames仅存档，不作为当前接入版本。用户要求先为现有NPC/skill/M11文档和全部本轮交付开PR，再另开分支只制作M11列出的双首领特效并开第二个PR。**

- 用户指出v5循环闪烁、未对齐。实际PNG存在两个半循环眼高y29/y27的2px跳变、头形与马甲版型变化；之前仅腰轴对齐不充分。v5不得继续称作视觉验收通过。
- `kakashi-walk-stable-v6` 已delivered，无后台任务。一次imagegen生成完整六姿势，固定复用认可Idle头/脖子/领口 `(20,14,52,37)`；原图、提示词、14帧和脚本在对应交付目录。
- **当前最终14帧**在 `art/deliveries/npc-direct-pixel-round1/kakashi_stable_frames/`。根 `stabilize_kakashi.py`应用共同身体色板，并复用认可Idle胸口内区 `(35,37,42,47)`，alpha/肩臂/腰髋/腿几何不变。PNG/GIF/APNG的头部与胸口内区逐像素固定，其他8帧byte-identical。
- 组表时传 `--final-frames art/deliveries/npc-direct-pixel-round1/kakashi_stable_frames`；不传仍生成历史v5。Kakashi_game.png与游戏Kakashi.png已替换六Walk；其他6行、头像、水牢、C#不变。最终表翻转/+2px位移逐行通过。
- 预览GIF公用色板，另有无损 `kakashi_walk.png` APNG；网页来源更新，JS语法通过，浏览器未实测。最终 `./scripts/verify-mac.sh` 通过（0错误、两条旧KonohaDump警告；打包仍有未知图片类型提示，未定位）；实机/无头未运行。下一步在游戏重载后检查左右行走、6→1循环与停止切Idle。
- Skill补充多锚点与全循环连续性检查；validator通过。其他NPC、Boss文档保持原样。

## 2026-10-05 Codex：卡卡西走路与眼部修复、新NPC第一批样张（已接入，未实机）

- 用户反馈卡卡西走路上下身分离，要求优化并用 `terraria-npc-pixel-art` 开始其他NPC。后续明确新眼睛的大黑块是**卡卡西**；已恢复其认可站姿的眼部，不给其他NPC统一眼型。
- 定位：游戏确实消费v2帧，80×960/1倍和帧顺序正确；v2将别帧躯干覆盖目标腿部，腰髋未匹配。v3整体重做走路，再由v4补对侧步态。统一腰轴后六帧为v3的01/02/04+v4的三姿势，只整体平移，保留内部身体连接。
- **当前最终素材入口**：`art/deliveries/npc-direct-pixel-round1/DELIVERY.md`、`Kakashi_game.png`、`kakashi_walk.gif`、`preview.html`。最终完整14帧在 `art/deliveries/kakashi-eye-restore-v5/frames/`；round1的 `kakashi_frames/` 仅为修眼前输入，不能误接入。
- 六帧眼部各改51–56像素，来自已认可v2 Idle局部，在各自小矩形内合成；眼部外、alpha、躯干、腿相位与脚底均不变。游戏 `Kakashi.png` 已更新，其他6行、头像、水牢两帧与C#代码未改。已核对最终表12行、左向翻转与整体脚底+2位移逐像素一致。
- **其他NPC已开始**：`hiruzen-direct-pixel-v1` 三代站姿（80×80，57高，细长眼/白胡须/烟斗），`toolshop-direct-pixel-v1` 忍具店老板站姿（80×80，62高，发髻/胡须/酒红上衣/围裙）；均为skill直接像素流程的候选，未接入或扩动画。并排图 `npc-direct-pixel-round1/new_npc_comparison_3x.png`。
- 五个请求 `kakashi-walk-repair-v3`、`kakashi-walk-phase-v4`、`kakashi-eye-restore-v5`、`hiruzen-direct-pixel-v1`、`toolshop-direct-pixel-v1` 均 delivered，**无后台美术任务在跑**。各交付有源图、提示词和导出/局部合成参数；不重复submit。
- Skill已补腰髋接续、本角色已认可眉眼逐帧对照，并修正伊鲁卡/卡卡西已接入事实；skill validator通过。原NPC画风入口更新；没有更改其他角色眼睛格数。
- 检查：PNG尺寸/脚底/透明RGB/二值alpha/镜像/无裁切、局部外不变、未改动作原样、GIF六帧、JS语法与静态引用通过。最终 `./scripts/verify-mac.sh` 通过（文本0、规则通过、完整构建0错误、两条KonohaDump提示）；日志有未定位的“Image loading failed: unknown image type”运行提示，见round1/verify-mac.log。**无头加载和游戏内实机均未运行**。内置浏览器禁止file协议，未做页面浏览器实测，未绕过限制。
- 分支main，基准HEAD `6333e31`。本轮未提交：Kakashi.png、skill、art/AGENT_HANDOFF.md的NPC状态更新、此交接段、五个新请求及交付目录、round1整合目录。此前M11与旧规格优先级的未提交文档保持原样，未混入源码或清理。
- 下一步：用户重载后看卡卡西左右走路/停止切Idle、眼睛、脚贴地和实际节奏；针对明确问题局部修。三代与店老板先看站姿，再按认可底稿扩动作；其余NPC尚未制作新样张。Boss实施继续读M11，不把本轮NPC修复视为Boss已开发。

## 2026-10-05 Codex：再不斩与白完整重设计定稿（仅文档）

- 用户通过 28 项讨论确认方向，最后明确「可以定稿，然后给出完整的设计文档」。交付 `specs/M11_波之国双首领战重设计.spec.md` 与 `tests/M11_波之国双首领战重设计.acceptance.md`。新版冲突处覆盖 M9/M10；AGENTS、旧规格顶部、总纲和美术交接入口已标注优先级。
- 保留用户满意的结构：再不斩半血后白助阵、独立生命、先死一人另一人暴走、按死亡序播放尾声。本体可见高再不斩约玩家 3 倍、白 2.5 倍；再不斩主动落脚/跳跃/突进/水雾瞬身，白自由飞行与镜阵交替，普通身体无接触伤害。
- 核心攻击为短刀术连段、锁定路线水龙、停留投针、可破镜阵及间歇水龙/千本合击。自然收招有短反击，破阵奖励更大；镜阵及奖励窗口再不斩不抢攻。基础跑跳为机动基准，正常配装熟悉后约 3–4 分钟。
- 用户明确选择动画式流体与柔光特效（Q26.C），主体同样可柔边/半透明；普通攻击已有明显演出、强招完整、暴走升级；重击轻震与少数强招短压暗。不能改回全硬边像素效果。
- 大桥仅作为起战点与基础落脚，玩家可自建平台并将战斗带往世界其他区域；不为每个 Boss 永久占用大型场景。尾声在最后击杀附近，远距离/障碍/高差时用短过渡整理安全演出位置，不传送玩家。
- 文档区分已确认方向、首版调试数值、当前代码基线（核查基准 `0917d0e`）。差距包含非致死伤害跨半血、白槽满缺席、离屏保活、真实双击杀账本、命中去重、远距离尾声与同材料自建脚手架保护；均尚未修复。
- 本轮只改文档，未改源码、美术或游戏参数；无新后台美术请求。`git diff --check` 通过；M11 主文档 454 行、验收清单 59 项，16 个本地链接存在、Q1–Q28 决策完整、代码围栏与末尾空白检查通过。规则测试、构建、无头加载、单人/联机实机均未运行。
- 分支 `main`，接手时 HEAD 已到 `7985748`（Claude 伊鲁卡补脖子）。保留其他工作中的 `art/requests/kakashi-direct-pixel-anim-v1.md` 与对应交付目录；其任务属于 NPC 工作，本轮未操作或重新提交。
- 下一步最小动作：实施者先读 M11 第 13、15 节与验收清单，从半血锁线、真实双击杀、生成异常和主攻互斥开始，继而确认实际尺寸关键姿势；按清单分别记录自动检查与游戏验收。文档定稿不代表改版已完成。

## 2026-10-05 Claude Code：伊鲁卡全套动作接入、卡卡西样张（未实机）

- 用户：再不斩/白的优化正在和 Codex 讨论，之后给 Claude 设计文档；先做伊鲁卡动作帧并接入、用 skill 画下一个 NPC、残留文件先提交。残留已提交（`d5e3b2a` 作废旧稿，`01f0f7d` skill/伊鲁卡单帧/复盘/调研）。
- **伊鲁卡已接入**：`iruka-direct-pixel-anim-v1`（Codex 后台桥接）交 12 帧 80×80，站立帧就是已认可帧原样，其余帧都贴了已认可头部像素（脸一致、无嘴线）；脚底 y=75，身体中线 x=40。`scripts/build_hires_npc_sheet.py` 改为整体下移让站立帧脚落在倒数第 3 行（原版画法：帧底在判定框下 4 像素、脚陷地 2 像素），新增 `--head X,Y`（伊鲁卡用 `--head 28,18`，否则头像被马尾占满）。`Iruka.png` 80×960、`Iruka_Head.png` 30×26，`Iruka.cs` 的 `NPC.scale = 1f`。
- **卡卡西样张**（`kakashi-direct-pixel-v1`）：用户“很不错 真不错 就是身高要统一一下”。Claude 用同一源图按 62 高、相位 0.8/0.5 重导出 `Kakashi_Idle_h62_*`（默认相位漏眼睛），对比图 `compare_h62.png`，待用户看；统一总高后他的头比伊鲁卡小。未接入，未出动作帧。
- 用户看后反馈几帧头和身体分离：Codex 只贴了下巴以上的头部，脖子取自生成图。已用 `art/deliveries/iruka-direct-pixel-anim-v1/fix_necks.py` 补脖子、清掉残留的生成下巴并重拼（原交付在 `frames_before_neck_fix/`）。
- 验证：`./scripts/verify-mac.sh` 通过（0 错误，旧的两条 KonohaDump 警告）。**未实机**：伊鲁卡在游戏里的大小、脚贴地、走路是否左右抖、坐椅子对位、投苦无出手位置、头像图标。无头服务器加载未运行（只换贴图与缩放）。
- **卡卡西已接入**：用户选 62 高版本（“第一个，眼白比较多的”）。`kakashi-direct-pixel-anim-v1` 出 14 帧（12 帧 + 水牢 2 帧，头部贴到面罩下缘和领口），Claude 验收打回 03/04 走路（马甲下摆鼓出方块）、05 走路与 11 投掷（背后深蓝钩子），`kakashi-direct-pixel-anim-v2` 重做这 4 帧、其余原样。`Kakashi.png`/`Kakashi_Head.png`（`--head 29,16`）/`Kakashi_Trapped_0/1.png` 已换；`NpcSheet.KakashiBody` 改为 `KakashiScale = 1f`，卡卡西、水牢、考官借用贴图都用它。10 投掷背后仍有一小截钩状色块未修。verify 通过；**未实机**。
- 下一步：用户进游戏看伊鲁卡和卡卡西（大小、贴地、走路、坐、投苦无、头像、水牢里的卡卡西）。再不斩/白等 Codex 设计文档。

## 2026-10-05 Codex：青元仙途 Boss 设计调研（讨论中）

此段为定稿前研究快照；其中“未定稿/继续确认/建议尺寸”的状态已由上方 M11 定稿段取代，研究范围与证据限制保留。

- 用户觉得 Boss 体量、地形限制及战斗演出不足，要求实际调研 Terraria《凡人修仙传》模组与忍者战斗的共通点。调研对象已定位为工坊 3778390811《青元仙途》；3796089290 是同名合集。
- 调研记录：`docs/reviews/2026-10-05_青元仙途Boss设计调研.md`。已阅读作者当前说明并在浏览器抽看玄骨、乱星海及真仙马良相关实机视频片段；没有完整观看系列、安装试玩或读取对方源码。能确认放大人形、空中交战、大片术式效果；不能确认所有 Boss 固定 3–4 倍或全程穿实心方块，效果归属也未全部确认。
- 对照本项目：白已默认自由飞行穿地形，再不斩冲刺已穿地形且有受阻瞬身；`M10_波之国双首领战优化.spec.md` 冲突处覆盖 M9。总纲旧方向仍是“人小招大”，本轮用户在重新讨论，未定稿。
- 建议先在再不斩/白一战比较本体比例、主动移动和标志术式，分别验证；报告里的 2.5 / 3 / 3.5 玩家身高与其他 Boss 方案均为建议，未获用户确认。未修改源码、美术或规格，构建/规则测试/实机均未运行。
- 下一步最小动作：继续与用户确认战斗方向；若授权做原型，先定一个 Boss 的尺寸对照与移动/大招试验，不自动替换全员。此前伊鲁卡 skill 与单帧工作保持，仍未接入。

## 2026-10-05 Codex：伊鲁卡直接像素样张与最终尺寸单帧（未接入）

- 用户要求阅读 docs/reviews/2026-10-05_NPC美术复盘/ 并讨论直接生成像素角色；随后要求实际尝试。交付 art/deliveries/iruka-direct-pixel-v1/ 的 A/B 样张，用户反馈「没事 我觉得现在挺好 已经超越dazina了」。最后生成的 B 为后续优先底稿，不再为了匹配达兹纳重画其外观；用户未单独点名 A/B。
- 用户随后关心游戏尺寸丢失细节，要求把 B 做成最终尺寸单帧。已产出 Iruka_Idle_final_Right.png / Left.png：64×80，人物高 62，脚底最后一行 y=75，建议后续接入 1 倍显示；保留 B 原图色块及细小脸部特征，不是严格 2×2 网格。23 格旧格式与严格 31 格方案损伤五官，保留为反例，不要接错。
- **用户最新决定：这个尺寸不用画嘴巴。** 最终单帧已去嘴线，保留眼睛与鼻梁疤；后续动作沿用无嘴线设计。只合成生图结果的口部区域，其他原图像素保持原样；已验证局部改动范围、alpha、身高/脚底及左右翻转。
- 用户随后要求眼睛竖向占两格。本轮按 4 屏幕像素高实现，眼睛宽度保持；疤略下移，眼睛与疤之间留一行肤色，继续无嘴线。最终 PNG 与 preview.html 已更新，before_eye 文件保留改前。验证眼睛 4 行、肤色间隔及局部变化范围通过；用户最新明确认可「现在的这个就是完美」，仍未接入游戏。
- 用户要求整理为可供其他 NPC 复用的 skill，随后明确补充「每个人的眼睛大小都不一样的」。已创建 skills/terraria-npc-pixel-art/（SKILL.md、画风/生成/导出说明、已认可原图和真实小帧、通用导出工具、Codex UI 元数据），安装到用户 Codex skills 目录的符号链接，Claude 可直接读取仓库版本；art/AGENT_HANDOFF.md 已加入入口和新旧规范优先级。复用的是画风与小尺寸流程，不是统一眼睛格数或复制伊鲁卡疤。
- 同目录 preview.html 已更新，final_frame_preview.png 显示真实导出帧的正常尺寸与 3 倍、明暗背景对比。导出、完整提示词、限制与检查记录见 DELIVERY.md / FRAME_MANIFEST.json / export_frame.cjs。清理生图尝试未采用，最终仍来自已认可的 B。
- PNG 尺寸、人物高度/脚底、0/255 alpha、透明区零 RGB、左右翻转检查通过，已目视检查页面；完整模组构建、无头加载与实机全部未运行。未替换游戏贴图、未改 NPC.scale、未扩展动画，不能把单帧结果称为已接入。
- 分支 main；本轮新增 art/requests/iruka-direct-pixel-v1.md 与对应交付目录，本交接段未提交。原有其他交付、请求和复盘未提交文件保持原样；iruka-npc-v5 状态 delivered，本轮直接生成，无新的后台桥接任务。
- Skill 验证：quick_validate.py 通过；通用导出工具可逐像素复现已认可伊鲁卡，镜像、共同动画比例、同名覆盖控制、裁切拒绝、透明源图拒绝和非法相位拒绝均通过。仅技能/工具检查，无新角色生图、模组构建或游戏实测。
- 下一步最小动作：按用户指定的下一个 NPC 调用 skill，保留该角色独有的眼型；或用户要求继续伊鲁卡时按已认可底稿制作真实动作帧并接入/实测。不要复用旧 23 格、1.36 倍管线，不因创建 skill 自动改全员。

## 2026-10-05 Claude Code：交接时的仓库状态（NPC 美术复盘之后）

- **复盘文档**：`docs/reviews/2026-10-05_NPC美术复盘/README.md` 加上 `images/`（16 张：新旧全员对比、头部特写、三人 12 帧对比、各轮样张）。内容是昨晚高清重画的原始需求、新旧对比、问题、修改、设计依据和后续方向。第 7 节开头补了一条说明：后来 Codex 的直接像素伊鲁卡（上一节）已取代文中的伊鲁卡 v5 和“不要用 1 像素网格”这条建议。未提交。
- **伊鲁卡 v5 作废**：`art/requests/iruka-npc-v5.md`（已提交）和 `art/deliveries/iruka-npc-v5/`（未提交）是 Claude 按旧 23 格规格申请的样张，桥接状态为 delivered，用户没有选它，已被 `iruka-direct-pixel-v1` 方案 B 取代。**不要接入。**
- **游戏里的 NPC 贴图**：九个友好 NPC 都是高清重画之前的版本（与 `fac1565` 逐字节相同，已核对）。伊鲁卡仍是 iruka-npc-v3，`NPC.scale = NpcSheet.ScaleFor(46)`。
- **工作区里未提交的内容（都不是 Claude 本轮写的代码）**：
  - Codex 改过的 `WORK_HANDOFF.md` 和 `art/AGENT_HANDOFF.md`；
  - `skills/`；
  - `art/requests/iruka-direct-pixel-v1.md` 和 `art/deliveries/iruka-direct-pixel-v1/`；
  - `docs/reviews/2026-10-05_青元仙途Boss设计调研.md` 和本复盘文件夹；
  - 历史残留：`art/deliveries/` 下的 `iruka-npc-v4`、`iruka-npc-v5`、`ninja-mobs-full-v2`（三者都已作废），`kakashi-npc-v3`（10-01 作废的半成品），`orochimaru-base-v8`、`orochimaru-style-v1`（10-02 的旧中间稿）；`art/requests/orochimaru-base-v8.md`、`orochimaru-set-v10a.md`、`orochimaru-set-v10b.md`；`art/npc_current_preview.png`。
  - 残留文件是否提交或删除，要问用户，不要自己清理。
- **测试**：复盘只生成了图片和文档，没有改源码，没有运行构建或实机。
- **没有后台美术任务在跑。**
- **下一步**：以上一节（伊鲁卡直接像素）和最上面的 Boss 调研为准，等用户指定。如果做伊鲁卡动作帧或接入游戏，按 `skills/terraria-npc-pixel-art/SKILL.md` 执行，接入后要实机看大小和脚是否贴地。

## 2026-10-05 夜 Claude Code：NPC 高清重画（用户看后否决，已全部恢复）

- **用户 2026-10-05：“太丑了，NPC 全都恢复到之前的版本”**。九个 NPC 的贴图和缩放代码已恢复到高清重画之前（达兹纳、忍具店老板恢复到改脸样张之前的原图；伊鲁卡是 iruka-npc-v3）。下面是当时的记录，高清规格不要再用。

- 用户要求：NPC 外貌要有辨识度，Claude 放大逐像素验收，不满意就一直让 Codex 重画。根因：旧规格的脸只有约 5×5 像素。改为高清规格（`art/AGENT_HANDOFF.md`“高清城镇 NPC 规格”）：1 像素 = 1 屏幕像素，80×80 帧，游戏里 `NpcSheet.HiresScale`（1 倍）。
- **九个友好 NPC 全部换成高清并接入**：达兹纳（含桥边）、伊鲁卡、卡卡西（含水牢被困两帧）、忍具店老板、三代、白（森林）、伊比喜、红豆、疾风。每张都经 8 倍逐像素验收；打回过的问题记录在各 `art/requests/*-hires-*.md`。全员图 `art/deliveries/npc-hires-accepted/all_npcs_3x.png`。
- 代码：`NpcSheet.HiresScale`；各 NPC 的 `NPC.scale` 改为 1；三代烟雾位置跟新烟斗；考官借用卡卡西贴图时也是 1 倍。拼图 `scripts/build_hires_npc_sheet.py`，等待/重试 `scripts/art_wait.py`，杂点统计 `scripts/pixel_noise.py`。
- 验证：`./scripts/verify-mac.sh` 通过；无头建世界加载无异常。**未实机**：在游戏里看大小、脚是否贴地、走路与坐下动画、头像图标、对话时的帧。
- 小兵（浪人、叛忍）按用户要求未动（代码里缩到 0.75、朝向已修，美术仍是旧的）。

## 2026-10-04 Claude Code：第 1 档一环（已实现，未实机）

- 制作顺序第 1 步完成（`specs/空档衔接与火影小兵.spec.md`、`specs/装备与忍术系统.spec.md`、`specs/推荐配装.spec.md`）。本地提交，**未推送**：bb8a5af 第 1 档装备、660fcfc 卡卡西三门修行 + 浪人/叛忍/鬼之兄弟回归、5e990a4 图标 v2、c27ea3e 手册首领页推荐配装、220572b 伊鲁卡任务板、以及本次盔甲提交。
- 伊鲁卡（`Content/NPCs/Iruka.cs`，iruka-npc-v3）：开局住进木叶。“接任务/任务”按钮接、交、领报酬（`MissionPlayer`、`MissionRules`）；四种 D 级（抓小虎 `LostTora`、送信、采集、夜间巡逻 3 浪人）不连续重复；“兑换”是任务令牌计价商店（`MissionTokenCurrency`）。与原规格不同：没做结晶碎片，直接 15 令牌换一颗结晶（已写进规格，需告诉用户）。
- 盔甲 `Content/Items/Tier1/Tier1Armor.cs`（armor-academy-v1、armor-genin-v1，套在原版忍者头/铜身/铜腿模板上）：学院训练服 5/5/4，套装多 1 木头、木头恢复快 15%；下忍战斗服 4/4/2，忍具潜伏值快 50%、潜伏加成 30%→40%。忍具台合成。
- 测试指令：`/m0 armor`（两套盔甲各一套）、`/m0 lesson …`、`/m0 ninja [ronin|rogue]`、`/m0 mission tora|letter|gather|patrol|done`、`/m0 dmg`。
- 验证：`./scripts/verify-mac.sh` 通过（含任务规则两条新测试），文本检查 0，无头建世界加载无异常。**未实机**：盔甲在玩家身上的走跑跳攻击姿势、头发遮挡，小虎逃跑手感，任务按钮文字，令牌商店价格显示。
- 下一步：用户实机试玩第 1 档；然后第 2 步（斩首大刀、千本、水龙弹、魔镜冰晶进第 2 档 + 雾隐追杀部队）。工作树里 `art/requests/orochimaru-*`、`art/deliveries/orochimaru-*` 不是本会话的，保持原样。

## 2026-10-05 Codex：修复进游戏后输入法变中文

- 用户反馈游戏外英文、切入游戏变中文。本次接手时客户端 PID 7890 正在运行，旧工具有勾选自动英文，但没有 `session.json`、暂停会话已结束；旧代码只有会话开启时监听游戏焦点，这是明确遗漏。另有一次成功切换就禁止后续检查的时序缺陷，无法纠正稍后恢复中文。
- 独立工具升级 1.2：保持工具运行并勾选自动英文即可生效，不需要暂停后台；暂停两个入口保持不变。空闲时也发现游戏，0.2 秒轻量轮询补足 SDL 前台通知，切入后约 0.25/0.8/1.5 秒复查英文。`GameInputFocus` 通过 PID、焦点代次、选项和检查有效期隔离旧回调，切出、快速切回、关闭选项时不误改其他程序；稳定游玩允许手动中文聊天。
- `--diagnose` 新增前台 PID、游戏激活状态，游戏的无 bundle `dotnet` 进程可被 `NSRunningApplication` 识别。源码仅 `tools/GameMode/`、测试 `tests/GameMode/main.swift`，说明见工具 README；没有修改游戏或系统输入法设置。
- 验证：`./scripts/test-game-mode.sh` 73 项通过，新增“外部英文→首轮检查成功→游戏稍后恢复中文→后续检查再切英文”模拟回归，及快速切出切回、关闭选项、过期回调、不持续覆盖中文聊天。`./scripts/build-game-mode.sh` 成功，已退出旧空闲工具并打开 1.2，原有三个后台选择和英文勾选保留。没有暂停、退出或重启游戏及用户后台应用；没有创建暂停会话。
- 实机状态：已请求用户切回现有游戏保持几秒；随后约 48 秒只读采样中游戏没有成为前台，因此尚无修复后的游戏内确认。旧输入法切换/恢复实测见下一节。未运行完整模组构建、未验证 FPS/Steam 启动/退出自动恢复。本次本地提交、不推送，其他助手美术目录保持原样。
- 下一步：用户切回 tModLoader 确认保持英文；若仍失败，读取前台 PID、游戏激活状态及当前输入法，区分未检测到焦点与游戏反复修改输入法，不通过修改系统全局自动切换选项绕过问题。

## 2026-10-04 Codex：游戏模式新增英文输入与仅暂停入口

- 用户要求进游戏自动切英文，并且已在游戏中时能只暂停后台、不再次启动游戏。独立 Mac 工具升级为 1.1；范围仅 `tools/GameMode/`、两个工具构建/测试脚本、`tests/GameMode/`，没有修改模组。
- 新窗口提供“暂停后台并启动游戏”“仅暂停后台（不启动游戏）”。仅暂停入口不调用 Steam 启动、无需 Steam；已有客户端自动接管，后续游戏退出约 12–15 秒恢复。没有游戏时保持暂停、允许手动恢复；后来检测到游戏也会接管，不适用 3 分钟启动超时。
- 自动英文默认勾选，使用系统 TIS API，优先已启用 ABC/美国/其他英文键盘；开始会话记录并切英文，每次游戏成为前台时应用英文，同一次前台游玩允许手动换中文聊天。结束时恢复原输入法，用户已手动换成其他输入法则保留。无额外权限、登录项或常驻服务；只在工具会话开启期间生效。新输入法状态随会话持久化并由异常恢复保护处理，兼容旧记录；输入法失败不会阻止应用恢复。
- 客户端检测排除 `-terrariasteamclient` 辅助进程，避免其残留阻止退出恢复。源码和使用边界详见 `tools/GameMode/README.md`。
- 验证：`./scripts/test-game-mode.sh` 57 项通过（含临时心跳进程实际 STOP/CONT 与异常恢复；输入法相关用模拟服务，不影响真实键盘）。`./scripts/build-game-mode.sh` 编译签名成功。原生窗口实测取消三应用勾选后执行仅暂停：游戏客户端为空，真实中文拼音 ITABC → 美国键盘 US → 手动恢复后 ITABC，恢复记录已清除；最后恢复用户原先三应用勾选，新窗口空闲，自动英文开启，没有暂停实际用户应用。
- 未实机：游戏窗口前台切换触发、Steam 启动、带游戏退出自动恢复、FPS 对比。本次未运行完整模组构建。分支 main，本次本地提交，不推送；其他助手新增盔甲/大蛇丸美术目录和请求保持原样。
- 下一步最小动作：用户已在游戏时点击仅暂停入口，切回游戏确认英文输入法；完全退出后确认后台与输入法恢复。剧情工作继续按 Claude 的第 1 档交接。

## 2026-10-04 Codex：暂停后台后内存补测（已采样，未验证恢复/FPS）

- 用户通知已暂停后台；会话记录 16:03:54 选择 Chrome/VS Code，16:05:40–16:06:12 四次采样确认分别 30/20 个主要进程停止，Codex 未暂停。Chrome 崩溃报告、VS Code 更新与崩溃报告进程仍运行；没有替用户改变暂停状态。
- 当前系统已用约 15 GiB、压缩器 5.61–5.78 GiB、交换空间 4.65 GiB、游戏 footprint 中位数 2.41 GiB；Chrome 5.22 GiB、VS Code 2.51 GiB，没有明显 footprint 下降。压力级别仍为 2，32 秒新增交换写出 0。
- 相较上一轮压缩器少、交换更多，但两轮隔了 17.19 分钟，中间新增交换写出累计 799.5 MiB；不能归因于暂停，也不能据游戏占用变化认定泄漏。补测已追加到 `docs/reviews/2026-10-04_游戏启动前后内存实测.md`，原始文件 `20261004-160612-paused.json` 在用户 memory-checks 目录。
- 已核对真实主要进程暂停；Codex 暂停、应用恢复、游戏退出自动恢复、FPS/卡顿改善仍未验证。下一步按用户反馈决定是否做固定场景、紧邻采样的暂停/恢复对照；本次无源码/设置改动、未运行模组构建。

## 2026-10-04 Codex：游戏启动前后内存实测（已采样，暂停补测见上）

- 用户依次通知未启动、正在启动、进入世界；已完成 3 次基准、3 次启动、4 次世界内采样。报告 `docs/reviews/2026-10-04_游戏启动前后内存实测.md`；本地原始 JSON 与只读采样器路径见报告。
- 游戏进入世界后 footprint 稳定约 2.23 GiB，系统压缩器从基准 5.58–5.85 GiB 增至 6.64–6.73 GiB；交换空间始终约 3.89 GiB，世界内 32 秒读入交换 0.25 MiB、写出 0。内存压力级别启动前后均为 2。后台原本已有压力，但不能仅据这些数据认定卡顿原因或泄漏。
- PID 45685 是主游戏，45787 是 `-terrariasteamclient` 辅助进程，两者计入游戏内存组；不是启动了两个游戏。加载日志仅启用 ShinobiPrototype，模组 RAM 统计 251.3 MB，与整个游戏 footprint 不同口径。
- 本轮后台全部保持运行，未暂停/退出任何应用，未修改游戏设置或源码。FPS、Boss 战、暂停工具效果与游戏退出自动恢复：**未测**。下一步如继续排查，需在实际卡顿时测帧时间与资源活动，或单独比较同场景暂停后台前后；Codex 被暂停会连带其采样子进程停止，需要先独立采样器。

## 2026-10-04 Codex：Mac 游戏模式（暂停/恢复，待游戏实测）

- 用户明确纠正：要暂停 Chrome、Codex 等后台应用，保留程序状态，不要退出重开。早期退出方案已替换；过程中没有退出或暂停实际用户应用。
- 独立工具源码 `tools/GameMode/`，构建 `scripts/build-game-mode.sh`，测试 `scripts/test-game-mode.sh` / `tests/GameMode/`，使用说明 `tools/GameMode/README.md`。根目录 `游戏模式.command` 双击打开；本机已生成忽略的 `tools/GameMode/bin/泰拉瑞亚游戏模式.app`，窗口保持空闲供用户使用。
- 勾选 Chrome、Codex、VS Code / Claude，确认后发送 SIGSTOP 暂停同用户进程与子进程；默认只勾选 Chrome，选择会记住。通过 Steam 启动 tModLoader，或接管已经运行的客户端；退出游戏约 12–15 秒后发送 SIGCONT 恢复同一进程。按钮可手动恢复；不终止、不重新启动选中应用。
- PID/启动时间/可执行文件/用户 ID 核对、请求前原子恢复记录、独立异常退出恢复保护、会话锁、启动超时恢复、客户端/服务器/构建识别、工具与游戏及祖先保护已实现。状态在用户 `Library/Application Support/TerrutoGameMode`，无登录项、管理员权限或系统设置改动。
- 边界：暂停降低后台 CPU 活动，**不立即释放既有 RAM、不保证 FPS 改善**；窗口暂时无法操作，下载与 AI 网络请求可能超时。先等 AI 工作完成。不要与其他暂停工具同时操作同一程序。macOS 原生游戏模式侧重 CPU/GPU 优先级，tModLoader 的系统原生全屏识别未验证。
- 验证：32 项工具检查通过，包含仅针对临时心跳测试进程的实际暂停/恢复、所有者异常退出后恢复、旧保护进程与新会话锁竞争；原生 .app 编译成功。只读诊断识别 Chrome/Codex/VS Code，未检测到游戏。原生窗口与开始提示已查看，确认后取消，未执行真实应用暂停。实际 Chrome/Codex 暂停、Steam 启动、游戏结束恢复、内存/FPS 对比：**未运行**。本次不改模组，完整模组构建未运行。
- 分支 main；其他助手同时提交了 `be10bcf` 档位数值与美术请求，本次只保存工具与本段交接，未推送、未接入美术。下一步：用户结束 AI 工作后勾选目标应用，开启游戏模式，实测恢复和卡顿差异；剧情开发仍按 Claude 规格继续。

## 2026-10-04 Codex：卡顿排查与首次显示优化（待实机复测）

- 用户反馈放忍术/Boss 战与首次进入地区/显示新素材时卡顿。基准源码 `8316584`；详细采样、实现和对比步骤见 `docs/reviews/2026-10-04_卡顿排查.md`。
- 16GB Mac 采样时已有约 5.6GB 压缩、4.2GB 交换、15GiB 磁盘可用，运行约 36 天；后台 Chrome、Codex、VS Code/Claude 等占用明显。游戏未运行，不能据此证明卡顿原因。旧日志热重载未释放警告仅是线索，不当作已确认的当前泄漏。
- 已移除背景首次绘制 `Texture2D.GetData` 与大临时数组；`BackgroundLayoutData.cs` 从当前六张近景烘焙地面行，生成工具 `scripts/export_background_layout.py`，背景测试校验图片哈希避免素材变更后定位过期。尺寸恢复分支使用异步加载，未就绪暂跳过该层。
- 新增 `ClientVisualAssets` 预载现有 NPC/弹幕/UI 贴图，`BossSprites` 与 `FxArt` 缓存资产和动画帧数组，卸载清空；服务器不加载图像。预载把部分内存占用提前到启动期，减少首次显示期间的资源请求，不代表总内存下降或 Boss 持续掉帧已经解决。
- 验证：背景规则通过，`./scripts/verify-mac.sh` 860 项规则检查、模组打包通过，0 错误；既有两条 `KonohaDump` 告警和 FNA 图像回退日志仍在。客户端实机帧时间/FPS、动画与背景显示、热重载缓存验收：**未运行**。游戏设置、后台应用和存档未修改。
- 工作范围仅上述绘制/资源代码、背景规则测试、生成工具与排查文档。期间另一助手在更新空档/装备规格和提交新美术请求，相关改动保持原样；未接入其美术、未重复提交后台请求。性能优化保存为本地提交，未推送。
- 下一步最小动作：保存后重启电脑，减少无关后台程序，完整退出后启动游戏加载新包，对比首次/第二次进地区与施术；仍卡则在实际游戏运行中采样内存压力、换页与游戏 CPU/FPS。后续背景素材变化先重生成定位记录；原剧情内容开发目标见下面 Claude 交接，不被本次性能排查取代。

## 2026-10-04 Claude Code：空档衔接、小兵与档位设计（方向已定，未开工）

- 用户要求重度思考三件事：两段剧情之间怎么衔接、世界的火影小兵、武器档位。方案已经逐项同意，记录在新规格 `specs/空档衔接与火影小兵.spec.md`，武器部分补进 `specs/装备与忍术系统.spec.md` 的“档位的通用设计”。
- 用户已定：卡卡西三门修行（忍犬通灵、豪火球、爬树→查克拉贴墙移动能力）；任务板由伊鲁卡接待，可以连续接；五行相克要做；小兵用一套忍者底稿换装；死亡森林巨兽要做；敌人做替身、结印、潜伏三种镜像机制；制作顺序 1→4（先做第 1 档完整一环）。
- 下一步：第 1 步（第 1 档完整一环）开工前，先与用户逐项定细节（任务列表、敌人数值、物品表、美术请求批次）。

## 2026-10-04 Claude Code：英文版（文字全部进本地化，未实机）

- 起因：评测 P0-2，代码里约 520 行写死的中文，英文玩家看到的是中文（而且英文字体没有汉字，会直接显示不出来）。用户要求“做英文版”。
- 已做：
  - 玩家能看到的文字全部移进 `Localization/` 两个文件的 `Text` 下，共 639 条，中英各一份。范围包括对白、任务目标、手册、笔试 31 题、HUD、提示、告示牌、世界生成进度、`/m0` 指令。代码用 `Common/Loc.cs` 的 `Loc.Get("区域.名字", 参数)` 读取；随机台词用 `Loc.Pick`。
  - 联机广播改为发送文本键（`Loc.Broadcast`、`Loc.SendTo`，即 `NetworkText.FromKey`），每个客户端按自己的语言显示。只有建桥失败的原因还是服务器语言（`BridgeBlueprintNet`）。
  - 英文译名沿用已有的本地化：Land of Waves、Mist Insignia、Forest of Death 等。卡卡西保持懒散调侃的语气。
  - 结印浮字改成十二支标识（`SealScroll.Signs` 是 Ram、Snake 等）：中文显示“未巳”，英文显示单词，按文字宽度排开。
  - 英文文本里去掉了 → 和 •：原版各语言文本里从没用过这两个符号，英文字体很可能没有。
  - 建筑名从中文改成英文标识（`KonohaBuildings.Academy` 等，考场是 ForestGate、CentralTower 等），显示时读 `Place.<id>`；告示牌在生成世界时按当时的语言写入。
  - 立志流派名改为键（`VowRules.SchoolKey`），方向词改为键（`StoryRules.Direction` 返回 `Dir.West`/`Dir.East`），练习评语改为键（`ChakraRules.PracticeVerdict`）；相关测试已同步。
  - 旧存档的庆祝队列里有中文 Boss 名，读取时转成新标识 `WaveDuo`。
  - 补了中文文件缺的 15 个键（弹幕名等，以前中文玩家死亡信息里会出现英文）。
  - 手册页面的文字放不下时自动缩小字号（英文比中文长，手册没有滚动条）。
  - 新增 `scripts/check_text.py`，已加入 `verify-mac.sh`：代码里出现写死的中文字符串、中英文件的键或占位符不一致、英文文本里有汉字，都会报错。规则已写进 `AGENTS.md`。README 两版注明支持中英文。
- 验证：`./scripts/verify-mac.sh` 通过（文字检查 0 个问题，840 项规则测试，打包成功）。英文后台服务器生成世界正常，生成进度显示英文，0 物件缺失。**未实机**：英文模式下的 HUD 宽度、手册缩小后的观感、结印浮字的英文排版、笔试面板，都需要把游戏语言切到 English 看一遍。
- 未处理：Wiki（Codex 维护，中文）；`wiki/` 和美术目录未动。
- 下一步：用户把游戏语言切到 English，走一遍开局（卡卡西、手册、达兹纳）、结印、笔试、考官对话，看有无显示不全或别扭的英文；把问题截图给我。

## 2026-10-04 Claude Code：敌方伤害标准与木头门槛（已实现，未实机）

- 起因：评测发现敌方弹幕实际伤害是代码值的 2 倍（原版命中玩家时 ×2）。与用户 grill 定了 18 个问题，规则和总表见 `specs/敌方伤害标准.spec.md`（用户已审总表）。
- 已做：
  - 所有敌方伤害数字改为“玩家实际吃到的值”，集中在 `Common/EnemyDamageRules.cs`，经 `Common/EnemyDamage.cs` 发出：弹幕除以 2；接触伤害乘原版模式倍率；直接 Hurt 同口径。考生改用固定常数，不再从 `NPC.damage` 推算。
  - 按基准生命（100/200/300/400）限伤：普通招不超过 15%，大招不超过 30%。大招预警至少 24 帧，暴怒和阶段加速不能压到 24 帧以下。快招拉长到 24 帧；宁次柔拳第一掌推迟到第 15 帧；考生近身苦无加 15 帧出招动作。
  - 一招只算一次伤害：再不斩冲刺去掉身体接触；大蛇丸突进只留收尾咬、伸颈只留落点咬；多斯落地只留下砸（带耳鸣）。持续判定每次施放只命中一次（`PlayerHits`），投刀去、回各一次。
  - 删除 `SoftenedDamage`、`OrochimaruWindupScale`；大蛇丸弱玩家 7 折补到毒池。
  - 木头：防御后伤害低于最大生命 10% 不消耗木头；分身和看破同样适用，控制招无视门槛。恢复时间 ×（1 + 挡下伤害 ÷ 最大生命，最多 2 倍），控制招按 1.3 倍；用掉的木头按顺序各自恢复。手册、卡卡西的说明已同步；写轮眼说明的“8 秒”改为实际的 16 秒。
  - 测试指令 `/m0 dmg [on|off]`、`/m0 logs off|on`；新测试 `tests/EnemyDamageRules`（已加入 `verify-mac.sh`）。
  - 评测报告里“专家模式不变是问题”一条已更正：原版弹幕本来就基本不随模式放大。
- 验证：`./scripts/verify-mac.sh` 通过（840 项规则测试，模组打包成功）；`konoha-worldgen-check.sh` 后台服务器加载、生成世界无异常。**未实机**。
- 下一步：用户用不开无敌的新角色，配合 `/m0 dmg`（必要时 `/m0 logs off`），在各基准生命下把鬼之兄弟、再不斩与白、考生、大蛇丸、多斯、宁次、我爱罗各打一遍，按手感微调 `EnemyDamageRules` 里的数字（不超过上限，测试会拦）。大蛇丸伸颈“路上擦到不算”需要实测确认手感。
- 工作树：本次改动已在本地提交（见 git log），未推送。另：本文件第一行标题前曾混入一串误触字符（`AZS   QAQAZ`），已恢复原标题。

## 2026-10-03 夜 Claude Code：玩家视角评测（用户睡前任务，只写文档，未改代码）

- 用户要求以玩家视角，对照原版与灾厄评价当前模组。报告：`docs/reviews/2026-10-03_玩家视角评测.md`，附图 `docs/reviews/2026-10-03_boss_pixel_density.png`（六个 Boss 按游戏内倍率并排）。
- 主要结论（均未改代码，等用户决定）：①Boss 弹幕实际伤害是代码值的 2 倍（反编译确认原版对敌方弹幕 `DamageVar(damage) * 2`；本模组直接传设计值），专家/大师模式弹幕伤害不变；②约 520 行写死的中文未进本地化，英文玩家会看到中文；③木头替身无伤害门槛、比灾厄的闪避（<5% 最大生命不触发、冷却 20–90 秒）慷慨很多；④两个“去变强”阶段与我爱罗之后模组几乎无内容，缺地区敌人、装备成长线、考试 Boss 掉落（多斯只掉钱）；⑤考试篇 Boss 有 3 种像素密度（1.5 倍与 1.15 倍非整数缩放）；⑥木叶占小世界 11% 宽、无防蔓延、只支持新世界。另有铃铛测试、一乐拉面、D 级任务板、查克拉走墙/水上行走等建议，以及优先级表。
- 外部：灾厄 2026-08-10 停止开发（维基百科）；原版 1.4.5 已发布，tModLoader 稳定版截至 2026-10-01 仍为 1.4.4。
- 验证：用 `./scripts/konoha-worldgen-check.sh` 生成种子 4242 小世界，木叶 48/48 房合格、四处地标 0 物件缺失（渲染图含原版贴图，未入库）。未在游戏客户端试玩；报告中标“需实机确认”的条目待用户实测。
- 下一步：用户读报告后挑选要做的项；若做第 1 项（弹幕伤害），先用不开无敌、不带木头的新角色实测每招伤害。

## 2026-10-03 Claude Code：装备与忍术系统（第一步完成）

**规格**：`specs/装备与忍术系统.spec.md`（与用户多轮 grill 定稿，本次已同步到实际实现）。它取代总纲里关于武器、替身术、忍术物品的旧描述。核心：保留原版四种伤害类型，但火影装备不分职业；三套人人共用的系统（木头替身、潜伏、结印）；每档 Boss 给整套战利品；强度对齐同期原版；分 7 档，第 1、2 档物品表已定，第 3～7 档只有框架。开发顺序：①系统 → ②第 1 档内容 → ③第 2 档内容（含再不斩四件武器等旧物品重做）。

**已完成（全部在 `main`，最后推送到 `780d8b1`，之后又有本地提交未推送）**
- **木头替身**（`SubstitutionPlayer`、`StealthPlayer`、`StealthBuff`、`ChakraHud`）：敌人的攻击自动消耗一根木头（开局 2 根，20 秒回一根，命中快 0.1 秒，写轮眼更快）；F 花两根木头 + 15 查克拉瞬移并潜伏 5 秒（下一击必暴 +30%，增益图标有专门的美术）。控制招先预警、能躲，躲不开有木头就自动脱身：大蛇丸杀气改成 1 秒红色扇形预警（`Orochimaru.UpdateStare/DrawStare`，规则 `ExamBossRules.InStare`），沙缚柩同理。卡卡西的替身修行仍练按键时机。
- **结印**（`SealPlayer`、`SealRules`、`SealSlotsUI`、`SealOverlay`、`Content/Items/Jutsu/SealScroll.cs`、`Content/Projectiles/SealJutsu.cs`）：Z/X/C 点按自动结 2/4/6 印并施展；自画的三格印位栏（背包右侧，卷轴随角色保存，开局有分身术）；HUD 常驻冷却。卷轴：分身术、豪火球（巨大、穿墙、撞大目标即爆）、千鸟（1.5 秒蓄雷 + 蓄力条 + 范围雷击，冲刺 40 格穿 3 格薄墙，撞首领 2.5 倍，留雷查克拉）。豪火球、千鸟暂用 `/m0 seals` 拿。
- **忍具**（`Content/Items/NinjaTools/`、`ToolStealthPlayer`、`NinjaToolProjectiles.cs`）：拿着忍具攒潜伏值（空手 12 秒，`FillSpeed` 留给忍具系装备），满了下一投是潜伏投掷。训练苦无 7、苦无 9、手里剑 8（反弹；潜伏投掷为影手里剑）、起爆符 30（贴附 1 秒后爆炸）；`/m0 tools`。强度按用户要求压到第 1 档（约每秒 27）。
- **忍具店**：`ToolShopkeeper`（天天的父亲，开局住进木叶），卖苦无、手里剑、起爆符、兵粮丸，波之国任务后加千本。
- **其他**：物品说明真正换行（hjson 无引号写法里的 `\n` 不是换行，49 条改成三引号多行并断长行）；千鸟音效来自用户的 `Sound_Track/千鸟音效.m4a`，转成 `ShinobiPrototype/Assets/LocalSounds/ChidoriDash.wav`（已加入 `.gitignore`，绝不提交；缺文件时用原版音效）。
- **大蛇丸**：改为 1:1 高精度（224×136 画布、游戏里 1.15 倍、约 120 像素高），底稿是用户选的 v6 源图 A 经 v9/v11（脸由 Claude 手改 4 个像素）；全套 `orochimaru-set-v11a/b` + `moves-v11c/d`（4 帧冲刺、蛇从袖口出、连续 S 形脖子、招式体型统一）。去掉了移动倾斜和出招压扁（细节图会闪）。
- **美术流程教训**（已写入 `art/AGENT_HANDOFF.md` 和记忆）：Boss 不要把源图缩到很小再"修"，大蛇丸起按 1:1；不要给五官定死尺寸；验收要和其他角色按游戏尺寸并排看整体，美观由用户定；招式帧要量身体比例（面积、胸宽），不只看脸。
- **加载事故**：两个 NPC 共用一个头像文件会让模组加载失败（已修，`8601421`）。编译检查查不出这类问题；新增 NPC、贴图后可用 `./scripts/konoha-worldgen-check.sh <目录> <种子> 1` 在后台服务器加载一遍（服务器不加载贴图，最终以游戏内为准）。

**下一步**
1. 用户在试玩系统手感（木头、潜伏投掷、结印三招、忍具店），等反馈再调数值。
2. 第 1 档内容（规格表）：修行忍刀、鬼之兄弟锁链手甲、火遁·凤仙火之术、忍犬通灵、豪火球卷轴的正式来源、四件饰品、两套盔甲（下忍战斗服让潜伏值 1.5 倍）、忍具图纸与配方。动手前按规格逐项确认，美术按“物品 2×2、特效大而发光、对齐灾厄精度”。
3. 穿插：宁次、再不斩、白的补帧请求（`art/requests/*-tween-v1.md` 已写未提交，提交前改成“从统一底稿出全套”）；第 2 档开始时天天加入忍具店。
4. Codex 的 Wiki 需按新代码重建（`wiki/check.py` → 核对 → `wiki/build.py`）。

**工作树**：`main`。未跟踪的历史残留不要接入：`art/deliveries/kakashi-npc-v3/`、`orochimaru-style-v1/`、`orochimaru-base-v8/`（中途停掉）及 `art/requests/orochimaru-base-v8.md`、`orochimaru-set-v10a/b.md`（v10 底稿作废后未发出）。`wiki/` 由 Codex 管理。没有在跑的后台美术任务。

**验证**：每次提交前 `./scripts/verify-mac.sh`，最后一次 738 项规则测试全过、C# 0 错误（游戏开着时打包报 TML003 属正常）；本地化文件用 `hjson` 解析校验；后台服务器加载与世界生成通过。游戏内：用户已确认木头、杀气预警、结印、豪火球、千鸟、印位栏可用并给过反馈；忍具数值调整（`2e8037e`）和忍具店老板的新形象尚待用户实机确认。

## 2026-10-03 Codex：README 与 Wiki 同步

- 用户要求更新 README 与 Wiki。已以 `main` 的游戏源码提交 `b2e3115` 为依据核对；本次仅修改文档、展示素材及其导出脚本，没有改动模组代码、规格或美术交付。
- 中英文 `README.md`、`README.zh-CN.md` 同步第二试必经大蛇丸、红豆领卷与疾风预选流程、我爱罗砂墙机制；替换旧的按时机 F 替身说明，写明两根木头、自动代挡、20 秒恢复、F 消耗两根和 15 查克拉进入 5 秒潜伏，以及写轮眼 16 秒恢复。
- 增加 Z/X/C 单击自动结印与三种卷轴说明。豪火球和千鸟明确列为 `/m0 seals` 开发试用，正常任务、探索、掉落和教学来源尚未接入；忍具商店、盔甲与武器重做仍是规划。豪火球已按最新代码写明命中 Boss/大型敌人爆炸；千鸟写明额外 1.5 秒蓄雷与薄墙穿越。
- `scripts/export_readme_assets.py` 从当前运行时贴图导出 24 张展示图：新增大蛇丸待机 GIF 与三卷轴，Boss 使用全部现有待机帧；NPC 使用运行时首帧高度，修复伊比喜裁切。文件在 `docs/images/readme/`。
- Wiki 新增 `clone-scroll`、`fireball-scroll`、`chidori-scroll`、`stealth`、`hand-seals` 五个词条；共 70 词条、79 页面（33 道具、8 机制）。同步替身、写轮眼、异常状态、入门和 Boss 对策，图鉴增加结印卷轴分组，保留用户要求的官方 Wiki / 灾厄参考风格。维护入口为 `wiki/content.py`、`wiki/build.py`、`wiki/check.py`、`wiki/README.md`。
- 已执行：展示素材导出、Wiki 构建、`python3 wiki/check.py`（79 页内部链接/资源/锚点通过，0 个过期依赖）、`git diff --check`；两版 README 各 33 个本地链接无缺失，24 张展示图与 49 张 Wiki PNG 解码通过。GitHub Markdown 渲染两版均保留 8 个表格与 32 个图片标签。
- 浏览器验证：13 个重点 Wiki 页面分别检查 1440px 与 390px 布局，两版 README 同样检查两种宽度；均无页面横向溢出或本地图片缺失。道具筛选“千鸟”仅保留对应卷轴，全文搜索命中新卷轴与结印机制，点击可正确跳转。截图 `wiki/previews/seal-scrolls-updated.jpg`。本地 Wiki 预览服务为 `http://127.0.0.1:8765/`；仍未公开托管。
- 本次模组规则测试、完整构建和游戏内验收：**未运行**，文档网页检查不代表实机验收通过。未提交新的美术请求；没有本次启动的后台美术任务。
- 发现规格滞后：`specs/装备与忍术系统.spec.md` 仍包含未实现、长按 C 与旧木头参数等描述。本次文档以现有代码和最新用户已决定的实现为准；未擅自重写该规格，后续玩法开发前应先同步。
- Git：工作在 `main`，README/Wiki 与导出素材提交为 `780d8b1`。用户反馈 GitHub README 未更新后，重新获取远程状态，确认 GitHub `main` 已包含此提交；GitHub API 返回的 README 文件哈希与本地一致，实际中文页面已显示木头、潜伏、Z/X/C 结印和三种卷轴。此前“未推送”是上轮完成时的快照，已由本次核查更新。接手前已有的 `art/deliveries/kakashi-npc-v3/`、`orochimaru-base-v8/`、`orochimaru-style-v1/` 及 `art/requests/orochimaru-base-v8.md`、`orochimaru-set-v10a.md`、`orochimaru-set-v10b.md` 保持未跟踪、未修改，不属于本次提交。
- 下一步最小动作：用户检查 README 与 Wiki；玩法改变后先运行 Wiki 检查、核对人工说明、再重建。公开托管按用户后续指示处理。

## 项目与规格入口

- 模组正式名称 **Terruto**（用户选定，融合 Terraria 与 Naruto）。根目录 `/Users/tuzhechen/Documents/ChatGPT/泰拉瑞亚火影模组`，唯一源码 `ShinobiPrototype/`。GitHub 公开仓库：https://github.com/tuzhechen2005/terraria-naruto-mod （分支 `main`）。
- 上位设计：`specs/总纲_主线与支线结构.spec.md`（原版 Boss 全保留、月总为阶段门槛、主线必打链、疾风传解锁、原版职业 + 流派核心饰品、独立查克拉、尾兽/通灵兽支线、写轮眼完整路线）。旧 M1/M2 设计已作废。
- M1 波之国：`specs/M1_波之国下忍篇.spec.md`（已实现，验收记录 `tests/M1_波之国下忍篇.acceptance.md`）。
- Boss 战：`specs/M9_波之国双首领战.spec.md` + `specs/M10_波之国双首领战优化.spec.md`（M10 已实现）。

## 用户已决定（本会话）

- 画风定稿：泰拉瑞亚原生风格（2×2 屏幕像素为一美术像素，块状有棱角，明亮饱和），标准为 `art/deliveries/style-test-v3/`（C 版再不斩）与 `art/deliveries/style-test-v7-haku/`（白）。再不斩黑衣（蓝灰色阶）。
- 鬼人化：紫色烟雾般若鬼影（参照火影手游，`art/reference/demon_mode_ingame_2.png`），非写实、非西方恶魔。暴走阶段（白先死）再不斩绷带脱落露脸。
- 美术一律交给 Codex，经 `scripts/art_bridge.py`；Codex worker 默认模型 `gpt-6-sol`、推理 medium（可用 `CODEX_MODEL`/`CODEX_EFFORT` 覆盖）。

## 早期已完成（M10 与画风，已推送）

- M10 双首领战优化：白生成兜底、半血锁血、白穿墙飞行、删水分身；三段转场（跪地/白出镜/怒吼）+ 屏幕浓雾；冰镜牢笼（8 面可破镜、边界推回）；再不斩暴走（投刀往返、口咬苦无突进、受伤 +25%）；白暴走（6 面重生镜、千杀水翔）；台词；Boss 随世界光照（30% 亮度下限）；紫色鬼影与紫色特效。
- 全部 Boss 素材（再不斩 65 帧、白 19 帧、鬼影 9 帧、冰镜/千本/飞刀）已按定稿画风接入，由脚本重建：`scripts/process_wave_c.sh`（角色与道具）、`scripts/animate_apparition.py`（鬼影循环与浮现，从 `demon-apparition-v3` 单帧生成）。
- 工具：`scripts/pixelize_frames.py`（拆分/缩放/对齐，含 `--auto`、`--centers`、`--scale`）、`scripts/clean_frames.py`、`scripts/recolor_clothes.py`、`scripts/terraria_native.py`、`scripts/terraria_scene_preview.py`、`tools/XnbExtract/`（解原版 XNB 贴图，仅本地对比）。
- 环境：游戏内 Build + Reload 所需 SDK 通过 `~/.dotnet/dotnet → /usr/local/share/dotnet/x64/dotnet` 链接解决（见 `DEVELOPMENT_MAC.md`）。Codex CLI 已移到 `/Applications/ChatGPT.app/Contents/Resources/codex-cli/bin/codex`，桥接脚本已适配。

## 当前状态（2026-10-01）

**2026-10-01 Codex：模组命名、公开 README、素材展示与新图标**

- 用户在头脑风暴后选定 **Terruto**，并要求把现有图标底部的 NARUTO 字标改成 TERRUTO。`build.txt` 的显示名称、中英文 README 标题与介绍、`description.txt` 的模组简介已同步，GitHub 仓库简介也已更新为 Terruto。
- 字标编辑 `mod-icon-v5` 已完成并接入，完整源图与交付记录位于 `art/deliveries/mod-icon-v5/`。内置 imagegen 编辑 v4 的源图，只替换底部字标，保留原构图。后台 worker 在源图生成后的尺寸导出阶段停滞，已停止；开发助手使用该编辑源图完成 80×80、30×30、640×640 和尺寸预览，桥接状态明确记录人工收尾后的 delivered，无运行进程。
- 改的是显示名称，源码目录、Mod 类名、命名空间、资源路径和本地化键仍是 `ShinobiPrototype`，与原有存档和配置所用的内部标识一致。
- Terruto 验证：四个面向玩家的文件均使用新名字；源图、README 与 80×80 图标的字标逐字核对为 T E R R U T O。PNG 解码、尺寸与色数通过，两版 GitHub Markdown 和浏览器首页预览通过。使用独立存档目录运行 `./scripts/verify-mac.sh`，13 组规则测试与完整打包通过、0 错误。既有编译告警和图像转换回退日志仍在；尚未进行游戏内重载验收。
- README 改为自然正文，默认英文 `README.md`，中文 `README.zh-CN.md`，顶部有语言切换入口；旧横幅改为可编辑标题，末尾英文摘要已删除。此前相关提交 `797ae10`、`2966df3` 已按用户“推送”的要求推到 `main`。
- `2827ab3` 已推送：补充波之国、中忍考试、剧情 NPC、流派、武器与任务道具的介绍。`docs/images/readme/` 的 20 张展示图均从当前运行时贴图导出：5 位 Boss 的四帧待机 GIF、4 位剧情 NPC、10 件道具，以及我爱罗三阶段对照。`scripts/export_readme_assets.py` 可在带 Pillow 的 Python 环境中重新生成；未修改游戏源码或原始贴图。
- 验证：中英文图片与文档链接通过，GIF 均为四帧透明循环；GitHub Markdown API 保留全部 20 张展示图和 7 个表格；浏览器预览两版全部图片加载、无横向溢出，并已检查排版。本次为文档展示改动，未重新构建或实机测试模组。
- `3108f8f` 曾接入 `mod-icon-v4`：少年鸣人、卡卡西、螺旋丸、木叶背景和 NARUTO 像素字标。本次由 v5 的 TERRUTO 字标版本替换，仍为 80×80 与 30×30 游戏图标、640×640 README 图片，首页显示为 320×320。v4 作为历史原件保留。
- 图标验证：三个目标 PNG 与交付原件一致，尺寸和解码通过；GitHub Markdown 渲染及两版浏览器首页预览通过。运行 `./scripts/verify-mac.sh` 并通过 `ExtraBuildModFlags` 指定独立存档目录，13 组规则测试及模组打包通过，0 错误；既有的编译告警与图像转换回退日志仍在。尚未在运行中的游戏里重新加载并验收图标。

**2026-10-01 深夜：大蛇丸改为第二试必经遭遇（规格第五节已重写；已提交，未推送，未实机）**

- 流程：入口小队 → 森林中段（`ExamSiteWorld.OrochimaruX`）孤身的草隐考生 `OrochimaruDisguise`（被打倒→`ShedSkin` 蛇蜕 + 本体从地里钻出 `Emerge`；走到跟前→撕脸 `Reveal` + 杀气）→ 塔前雨隐（需 `OrochimaruDone`）。结束：半血 / 撑满 90 秒（弱玩家 60 秒、伤害七成）/ 玩家倒下（`TargetGone`，"还不是时候"）。第一次必给写轮眼试管（`ChuninExamPlayer.CreditOrochimaru`，倒下则复活后给），蛇蜕重打 25%。卡卡西、红豆各一句反应（`Flags2`）。`/m0 orochimaru` 叫出伪装。
- 通灵巨蛇 `Projectiles/GiantSnake`：震动预警后贴地扭动冲过，按原作万蛇画（`giant-snake-v3` 已接入，头/身/尾沿波浪转角度）；小蛇用 `SnakeBody` 代码绘制扭动。
- 美术：大蛇丸全套已接入（`orochimaru-moves-v3` 全部从用户认可的 v3 站立图逐像素改出，走路只有腿动；招式 `orochimaru-moves-v2`；长颈的头和脖子节由 `Orochimaru.PostDraw` 沿曲线拼）。教训：同一角色的帧必须从同一张底稿改，否则游戏里会"两个形象交替"。
- 用户已定：MIT 许可证（版权方 tuzhechen2005）、README 改成口语；三位新 NPC（伊比喜 v4、红豆、疾风）按原尺寸 1 倍显示。

**2026-10-01 晚：第二试与预选赛改版（与用户 grill 定稿，见 `specs/M2_中忍考试篇.spec.md` 第十节；已提交并推送，未实机）**

- 已实现：场地 v2（大门 + 监考小屋 + 长铁丝网、59×56 中央塔 + 结印石像、空心巨树、警告牌（林中休息处用户嫌粗糙已删，rebuild 会清掉旧的），镜像 `ExamSite.Dir`，`/m0 exam rebuild`）；红豆发卷、疾风开始预选赛（`Content/NPCs/ExamProctors.cs`，`Packet.StartPrelims`）；固定节点（入口埋伏小队、塔前雨隐，`DeathForestSystem` 重写）；游荡小队只掉材料；考生苦无/手里剑/烟雾弹，雨隐千本雨/分身（`RainClone`）；多斯五招（`Dosu.cs`、`ExamBossRules.ChooseDosuMove`，`JutsuKind.GroundQuake/ResonanceRing/ImpactSlam`）；删 Boss 大字标题（`BossIntroSystem` 已删）、关大蛇丸自然触发；删雾隐侦察兵，标记改为鬼之兄弟 2 + 达兹纳 1。
- 世界生成验证：`scripts/konoha-worldgen-check.sh`（种子 12345 小世界）入口、中央塔、巨树均生成，0 物件缺失（脚本现在也渲染 HollowTree）。
- 美术：`dosu-moves-v1` 已接入。`ibiki-npc-v4`、`anko-npc-v1`、`hayate-npc-v1`（新规格：28 美术像素、256×256 格、游戏里 1 倍）已交付，**等用户看样张**；接入命令 `build_npc_sheet.py ... --cell 256x256 --frame 64 --base 62`（Idle_Walk:0-6、Idle_Jump_Sit_Throw:1-5、Talk:1,2 共 14 帧），接入后把 `ExamProctor`/`Ibiki` 的缩放改为 1、`NPC.height` 适配。红豆、疾风在美术接入前借用卡卡西贴图。
- 用户新规范：所有人物以达兹纳为画风标准；Codex 采样后要逐像素修整（写进 `art/AGENT_HANDOFF.md`）。
- 仓库整理：README 重写（面向公开访客，中英）；早期文档移到 `docs/archive/`。
- 待讨论：大蛇丸在第二试的出场方式；结印石像是否请 Codex 画成贴图（现在用大理石墙拼）。

**2026-10-01 Claude Code 接手后（本段）**

- 用户反馈与改动（已提交，未实机）：
  - 所有剧情 NPC 与伊比喜同高：`NpcSheet.StoryHeight`（50 像素 × 1.25）与 `ScaleFor(身体像素)`，卡卡西/达兹纳（桥头与城镇）/三代/森林少年白都按待机帧身高换算；水牢里被困的卡卡西同比例。
  - 尾声倒地：不再用旧的 `Zabuza_Lying`/`Haku_Lying`（与起身帧对不上），行走者起身前躺 `Rise_0`，其余时候躺 `Collapse_2`（`WaveCorpse`）；湖边再不斩中千本倒下也改播 `Collapse` 0→2（`WaterPrison.DrawLying`）。旧图只作缺图兜底。
  - 战斗台词：**只删我爱罗和大蛇丸的**（用户更正：再不斩、白、宁次、多斯的台词都保留，尾声/湖边/预告台词不是战斗台词）。我爱罗“沙之盾”格挡提示保留。
  - 接入 `byakugan-icon-v3`（`StyleCores/ByakuganCore.png`）与 `exam-genin-style-v1`（`RainGenin.png`、`ForestCanopyCandidate.png`，2 帧：站立/投掷，投掷后显示 20 帧；去掉染色；判定框高 56）。
  - 修复：卡卡西“回村/去忍者学校/练习替身术”与伊比喜“开始笔试”按钮在回调里关对话框，原版随后读 `npc[-1]` 报 IndexOutOfRange（client.log `Main.GUIChatDrawInner`）。改为 `NpcChatCloser.CloseNextTick()` 下一帧关。
- 再不斩尾声帧重新上色（用户：倒地帧太柔、色块太大，和 Boss 战形象不一样）：`scripts/sharpen_epilogue_frames.py` 把 Rise/Stagger/Collapse 共 10 帧按战斗帧调色板重上色（蓝灰衣服色阶 + 左上光、描边分肢体、两阶肤色、绑腿灰、头部面罩白），剪影与位置逐像素不变，衔接不受影响。输入为 `6d87a4a` 时的游戏帧（Stagger 已按躯干对齐，与交付不同）。白的尾声帧未处理。
- **2026-10-01 下午（用户实测反馈后，已提交，未实机）**：
  - 火影岩：`bg-konoha-far-v3` 按网上找的第一部参考（`art/reference/hokage_rock_web/`，本地忽略）重画四张脸，已接入 `KonohaFar.png`。我把交付图顶部透明天空裁掉、整体上提 64 像素（底部用森林镜像补齐），脸约在第 106–229 行（旧版 41–113 行）。**待实机确认是否被中景村子挡住**；若被挡，再去掉背后的青山继续上提，或请 Codex 按指定行数重排。
  - 砂隐中景接缝：`SunaMid.png` 截取 x=37–1014（宽 977）成无缝平铺。
  - 再不斩/白站在水面：`Common/LiquidSurface.cs` 按实际水量算水面（水牢、桥边预告、雾中剪影），去掉白的 24 像素抬高。
  - 考生改为真正的三人小队（`DeathForestSystem.UpdateSquads/SendSquad`，ai[3]=队号、ai[0]=队名），整队倒下才算并说明带的是什么卷；凑齐两卷后不再来小队、雨隐不再伏击、不再发卷。`/m0 squad` 叫一队。进度字段改为 `SquadsBeaten`（存档键 `examSquads`）。
  - 防卡：`Common/GroundSpot.cs` 找站立点；`Common/Systems/StuckRescue.cs`（GlobalNPC，本模组非飞行的敌对 NPC，除再不斩与水分身）出生在方块里或脚下悬空时挪到最近空地，战斗中嵌墙或 3 秒没进展（顶墙/看不到目标/超过 40 格）就瞬身到目标身边。
  - 伊比喜换成 `ibiki-npc-v3`（头巾护额、扣好的风衣），缩放按 46 像素身高。
  - 大蛇丸首次出现太突然：用户说先放放，之后专门讨论。
- **NPC 画风统一（用户 2026-10-01）**：现有剧情 NPC 风格割裂，统一成达兹纳（`tazuna-npc-v1`）那样——深色外描边、每种材质 3–4 阶硬色阶、饱和暖色、无噪点、23 美术像素高。已提交 `kakashi-npc-v4`、`ibiki-npc-v2`、`hiruzen-npc-v2`（同一段画风要求，流程须与达兹纳相同：生图后采样，不许脚本拼像素）。`kakashi-npc-v3` 作废（进程已停，状态文件可能仍显示 running；`art/deliveries/kakashi-npc-v3/` 是残留半成品，不接入）。交付后先给用户看并排预览，再用 `scripts/build_npc_sheet.py` 接入，`NpcSheet.ScaleFor` 的身体像素按新图更新；卡卡西的水牢帧 `Kakashi_Trapped_*` 与头像也要跟着换。森林少年白（`HakuForest`）风格接近达兹纳，暂不重画。
  - 交付结果（2026-10-01）：`kakashi-npc-v4`、`hiruzen-npc-v2` 画风对上达兹纳，用户同意，已接入（`build_npc_sheet.py`：卡卡西 Idle_Walk:0-6 + Idle_Jump_Sit_Throw:1-5，含新头像；三代再加 Talk:1,2，共 14 帧；三代缩放按 46 像素身高）。水牢被困帧换成 `kakashi-trapped-v2`。三代再缩到 0.92 倍（用户：稍大）。`ibiki-npc-v2` 画风对但造型错（无头巾护额、像长发、露胸、无疤），已提交 `ibiki-npc-v3` 只改造型。
- 后台美术：见上一条（`python3 scripts/art_bridge.py status <ID>`）。`exam-genin-full-v1` 已交付并接入（考生贴图 7 帧：站立、投掷、跑 4、跳）。
- 用户已定：白眼 v3、雨隐下忍与考生样张都通过。

**最近一次会话（2026-09-30 晚 ～ 10-01，全部已提交；GitHub 只推送到 `b411ffe`，之后的提交都还在本地，推送需用户明确要求）**

- **工作方式（用户定）**：可玩性第一；从 M1 之后的新内容开始逐段打磨，用户实机测试后逐条反馈；美术先出样张给用户拍板，风格确认后再做完整动作，每批 2～4 个，可并行；卡卡西台词要戏谑调侃（不要“AI 味”）。
- **大方向（已写进总纲“可玩性与引导”）**：提示像原版一样含蓄（氛围话 + 卡卡西当向导 + 手册写大概方向；`QuestTracker` 设置默认关）；每个 Boss 都有召唤物，剧情只负责第一次；自动出现的 Boss 先预告（5 秒）再大字标题（`BossIntroSystem`）再战后说明；对白 ≤3–4 句不锁操作；手册“首领”页 + Boss Checklist。
- **M2 第二轮与流派（规格已更新）**：见 `specs/M2_中忍考试篇.spec.md`、`specs/流派系统.spec.md`（第一阶已定：写轮眼一勾玉/八门/白眼/仙术；体术武器；第二核心槽在佩恩之后）。我爱罗完全尾兽化放进 M3 作为打不赢之战（鸣人与文太接手），可收服的守鹤在尾兽支线。
- **已实现（代码）**：多斯/我爱罗召唤物（音忍的对战牌、砂隐的葫芦）、蛇蜕可合成、大蛇丸五行封印；`StyleCorePlayer`（看破、开门、点穴、回天、三代传话立志）、核心 `SharinganCore1`/`EightGatesCore`/`ByakuganCore`、体术 `TaijutsuDamage` + 训练用绷带、小李的负重护腿；Boss 逐帧动画框架（`ExamBoss.Pose`/`SpritePrefix`，无图时退回占位）；Boss 血条头像 `<Name>_Head_Boss.png`；地标名牌与大地图图标（`LandmarkLabels.cs`）；木叶背景远景树林填充 + 中景上移；木叶布局 v2（每层隔墙都有门，测试走通整条街）；找地表从原版地形最高点往下扫（修好会场建在浮岛上、泥土通天墙）；笔试面板空引用修复。
- **M1 收尾打磨（本段正在进行）**：
  - 尾声：最后倒下的人先躺一会儿 → 站起 → 以固定慢速（0.6 像素/帧）边走边说台词 → 提前到达就站着等最后一句 → 倒下 → 才下雪与旁白；走 20 秒到不了就倒在半路（`WaveEpilogueRules`，测试在 `tests/StoryRules`）。
  - 卡卡西：尸体消失时瞬身到玩家脚下的地面（世界里没有就生成），玩家落地站到他身边后才打开对话框（显示推荐台词，台词“……嘛……要是怕了，现在后悔也还来得及。”）；从拿到推荐书到笔试合格，第二个按钮是“回村”（远处）或“去忍者学校”（村里）。
  - 回村庆祝：每次打赢 Boss（含原版）后回木叶，村民依次冒表情、头顶喊话（5 秒）+ 彩纸（`StoryPlayer` + `BossCelebrationNPC`）。
  - 笔试：找忍者学校一楼门口的森乃伊比喜（`Ibiki.cs`，贴图 `ibiki-npc-v1`，1.25 倍，轮廓已改深色）开始，推荐书只是凭证。三代火影用 `hiruzen-npc-v1` 贴图。
- **美术状态**：
  - 已接入：我爱罗（含修补）、多斯（含修补，帧上移 2 像素对齐）、宁次完整动作；写轮眼试管、八门卷轴、绷带、负重护腿图标；三代、伊比喜；尾声帧（v2，见下）。
  - `wave-epilogue-walk-v2`：**已接入**（用户 2026-10-01 同意）。以战斗帧为底重画，2×2 块、身高与脚底线同战斗帧；两人的 Stagger 四帧按躯干中心互相对齐（白原本左右偏差约 20 像素）。
  - 等用户拍板：`byakugan-icon-v3`（白眼，按用户参考图只留眼睛）、`exam-genin-style-v1`（雨隐下忍、考生样张，我认为可用）。`orochimaru-style-v1` 质量一般，**用户说大蛇丸先不管**。
- **用户待答**：再不斩/白战斗台词与音效——我建议台词做头顶文字（原作台词安全），音效用原版泰拉瑞亚声音搭配，不用动画原声；用户同意后先列表给他过目。
- **测试指令（新增）**：`/m0 epilogue zabuza|haku [距离]`（无需打 Boss 直接播尾声 + 卡卡西出场，会重置考试进度到等推荐）、`/m0 cheer [首领名]`（记一次庆祝，进木叶触发）、`/m0 exam [阶段|gate|tower|stadium|academy|hokage]`。木叶/场地地形改动只对新世界生效。
- **修了一个脚本问题**：`verify-mac.sh` 把 `--nologo` 传给了测试程序，导致根目录出现名为 `--nologo` 的文件且曾被提交；已删并修好（`b1f87ca`）。


**M1 全部完成**，用户已在游戏内验证到 0.4.0 版。要点：

- 查克拉/替身术（默认 F）、卡卡西（练习替身术、补发手册）、忍者手册、模组设置（海雾浓度默认 100%）。
- 大桥 `BridgeDesign.Version` = 4（旧世界需新建）；海雾（再不斩二阶段雾平涂 + 小雾粒，随天空亮度）；迷雾预告（每角色每世界一次）、雾中剪影反复出没、千本警告；达兹纳；鬼之兄弟；森林少年白。
- 再不斩挑战卷轴 = 雾隐标记 ×3 + 木材 ×10（工作台）。奖励：职业武器保底四选一 + 约 20% 第二把、面具 1/7、奖杯 1/10、专家宝藏袋、每角色首胜结晶与功绩牌。
- Boss AI：克眼式冲刺（按阶段加长加速、穿地形）、瞬身术、视线判定（玩家躲石头后不再白打远程）。
- **音乐与尾声**（`WaveMusic.cs`、`WaveEpilogueRules.cs`、`WaveEpilogueSystem.cs`、`WaveCorpse.cs`）：大桥范围 Glued State；召唤再不斩播放 SadnessAndSorrow 文件、二阶段与白 Strong and Strike（用户要求对调过入场与尾声曲）；两人都倒下后尸体躺地、台词（再不斩先死时白有 4 句）、下雪、播放 NeedToBeStrong 文件；有 Boss 在场时尾声音乐立刻停。用户确认过音乐顺序与雪可见，**尾声台词最终版未实机确认**。
- **原声音乐只在本地**：`ShinobiPrototype/Assets/Music/*.mp3` 与 `Sound_Track/` 已 gitignore，缺失时回落原版音乐。**绝不能提交到公开仓库。**
- **模组图标**：`ShinobiPrototype/icon.png`（80×80，Codex `mod-icon-v1`，提交 `a72a77c`）。
- **创意工坊**：用户已首次上传（仅自己可见，**包内含四首原声**，用户知情并决定暂不拆分），Steam 显示等待审核。若被拒，大概率与原声有关，方案：把音乐拆成本地附属模组，或发布构建时 `buildIgnore` 排除 `Assets\Music\*`。给朋友玩可直接发 `~/Library/Application Support/Terraria/tModLoader/Mods/ShinobiPrototype.tmod`。
- **给朋友的引导页**（大桥篇简版，私有 Artifact，需用户在 Share 菜单开权限）：https://claude.ai/artifact/4AJRcbKYYh9dQMApX8rutq 。源文件不在仓库，修改时先用 Artifact read 取回，再发布到同一 URL。

**2026-09-30 夜间（用户睡前授权，只做不需生图、不需拍板的代码与规划）**

- 修复：砂隐 / 妙木山 / 雾隐背景不显示——原版在 `BiomeMedium` 之前就选定沙漠、发光蘑菇、海洋背景，已把地区背景与木叶生物群系提到 `BiomeHigh`（反编译 `Main.GetPreferredBGStyleForPlayer` 确认）。
- 修复（用户实机反馈：只看到远景，木叶缺火影楼、砂隐缺村子）：`client.log` 有 `DivideByZeroException` 于 `SurfaceBackgroundStylesLoader.DrawCloseBackground`。tML 在工作线程登记背景尺寸时部分贴图尚未加载，宽高记为 0：中景画成 0×0、近景除零并中断整帧地表背景。`RegionBackground` 取贴图时补宽高（补时写日志 “registered as 0 x 0”）。另外 tML 的近景公式比原版低（少前景层 -150、相机基准不同），且我们的近景图地面线比原版森林近景（第 342 行）低，`BackgroundLayoutRules.CloseOffset` 在 `ChooseCloseTexture` 的 b 上补回，地面行运行时从贴图读取。测试套件 `tests/BackgroundRules`。仍待实机确认。
- 木叶装饰：挂画（原版 3×3 风景画样式 63/66/67/69/76/77/79/94）、学校武器架、门口陶盆草药、广场长椅、“阿”“吽”牌子；布局测试新增物件重叠检查（发现并修复学校书架与灯笼冲突）；导出钩子报告放置失败的物件，现 0 缺失。
- 规划草案已在 grill 会话（Q1–Q31）逐条定稿，见下方“M2 中忍考试篇”。

**M2 中忍考试篇（2026-09-30 定稿并实现；占位美术；未实机验收）**

- 规格：`specs/M2_中忍考试篇.spec.md`（定稿）、`specs/流派系统.spec.md`（第六节立志已定；第三节四流派第一阶的被动与奥义**仍待确认**，具体核心物品未做）。验收清单：`tests/M2_中忍考试.acceptance.md`（E01–E20，全部未执行）。
- 用户决定要点：M2 到我爱罗为止，木叶崩溃另立 M3；**只支持新世界**；死亡森林在原版丛林（入口 + 中央塔两地标）；正式赛会场在木叶城墙外；考试进度按玩家、Boss 击败按世界；笔试 9 题 + 第十题“接受”即合格，放弃则天亮后重考；不限时；雨隐三人组首次必伏击（保底卷轴）；考生小队 25%、第 5 队必掉；多斯固定为预选对手；我爱罗必打、宁次赛后可选切磋；门槛：大蛇丸 = 世吞/克脑或生命 ≥ 300，正式赛 = 骷髅王或生命 ≥ 400；流派混合制（核心可换 + 找三代立志一次，改投 10 金币），立志时提示仙术要 M3；四个本命章节（咒印 / 凯的修行 / 日向 / 妙木山）只定主题；三代是固定站在火影办公室的剧情 NPC，M3 牺牲后改找自来也、再后纲手；奥义键 V。
- 代码：
  - 规则（纯逻辑，测试 `tests/ExamRules`）：`ChuninExamRules`（阶段、门槛、笔试、卷轴保底）、`WrittenExamBank`（31 题，正确答案在首位，面板打乱）、`VowRules`、`ExamSiteDesign`（三地标布局）、`ExamBossRules`。
  - 进度：`ChuninExamPlayer`（存档 + `Packet.ExamSync` 同步给服务器，供刷怪判断）；`StoryWorld` 标记改为两个 BitsByte，新增 DownedDosu/DownedGaara/DownedNeji/OrochimaruMet，删除 DownedArenaRival。
  - 流程：卡卡西发推荐书（`ExamAdmissionScroll`，无配方）与宁次切磋书；`WrittenExamSystem`（学校内使用推荐书）；`ExamSiteWorld`/`ExamSiteBuilder`（由 `KonohaWorld` 生成步骤末尾调用）；`DeathForestSystem`（雨隐伏击）；`ExamBoutSystem`（塔内多斯、会场我爱罗、丛林大蛇丸）；`HiruzenSpawnSystem` + `Hiruzen`（原版老人贴图占位）。
  - Boss（占位贴图 = 考生精灵染色，招式用 `JutsuHitbox` 粒子表现）：`ExamBoss` 基类、`Dosu`、`Gaara`、`Neji`、`Orochimaru`（阈值退场，掉 `SnakeSkin` 可再召唤）、`SummonedSnake`；玩家状态 `JutsuStatusPlayer`（沙缚柩/杀气可被替身术打断，耳鸣左右反向 ≤2 秒，点穴降查克拉上限、满 3 层封替身术）。
  - 删除：原创砂隐考生 `ArenaRivalBoss`、预选赛挑战书及其弹幕、`ExamRules.cs`、地下潜伏考生。
  - 调试：`/m0 exam [阶段|gate|tower|stadium|academy|hokage]`。
- 世界生成已用后台服务器验证（小/中/大），三个地标 0 物件缺失；`scripts/konoha-worldgen-check.sh` 现同时渲染 `-Gate/-Tower/-Stadium` 三张图。
- **第二轮（同日，按“可玩性第一”）已实现**：卡卡西尾声后瞬身出场（戏谑口吻）交推荐书；雨隐三人组改为击败一队考生后在中央塔附近伏击；多斯（音忍的对战牌）、我爱罗（砂隐的葫芦）召唤物，蛇蜕可合成；大蛇丸五行封印；Boss 出场大字标题（`BossIntroSystem`）与预告；三代传话立志（第一次拿到核心时卡卡西转告）；提示只写大概方向，`QuestTracker` 设置（默认关）显示方向距离；手册“首领”页与 Boss Checklist 登记；流派第一阶核心 `SharinganCore1`/`EightGatesCore`/`ByakuganCore`（`StyleCorePlayer` 实现看破、开门、点穴与回天）；体术伤害类 `TaijutsuDamage` 与训练用绷带、小李的负重护腿（风压 + 右键冲刺踢）；我爱罗每人首杀必掉八门与护腿。验收 E21–E26。
- **美术（2026-09-30 第一批，等用户拍板风格）**：`hiruzen-npc-v1`（三代，像素偏花、“火”字不清）、`gaara-style-v1`（我爱罗四个关键姿势）、`m2-style-items-v1`（五个图标，质量好）均已交付，**尚未接入**；用户确认风格后再接入并继续后续批次（每批 2–4 个）。
- **未做**：二勾玉、三勾玉、仙术核心（来源在 M3 及以后）；第二核心槽的开放（佩恩之后，`StyleCorePlayer.SecondSlot` 已预留，第二个奥义键未做）；本命章节；M3。

**木叶村与地区背景（规格 `specs/M2a_木叶村与地区背景.spec.md`；用户出门前授权自主完成，已完成，待实机验收）**

- 木叶（仅新世界生成，阿吽大门在出生点居中，48 间房）：`Common/KonohaDesign.cs`（布局，测试 `tests/KonohaDesign`）、`KonohaBuilder`、`KonohaWorld`、`KonohaBiome`（全体原版 NPC 喜欢木叶 + 家在木叶内不计拥挤，钩 `On_ShopHelper.GetNearbyResidentNPCs`）、`KonohaDump`（仅开发用，环境变量触发）。小/中/大世界各生成验证，48/48 通过原版住房检查。
- 验证工具：`./scripts/konoha-worldgen-check.sh <目录> [种子] [大小]`（游戏开着也能用：模组打包到独立存档目录，后台服务器生成新世界并导出木叶）；`scripts/render_konoha_world.py dump.jsonl out.png [--bg 背景目录] [--crop 起 止]` 用原版贴图渲染并打印住房检查。zsh 不拆分变量，循环调用请用 bash。
- 地区背景（`Common/Systems/RegionBackgrounds.cs`，模组设置“火影地区背景”开关）：木叶=木叶区域、雾隐/波之国=大桥海雾范围、砂隐=沙漠、雪之国=雪原、草隐/泷隐=丛林、妙木山=发光蘑菇，均为地表。贴图 `ShinobiPrototype/Backgrounds/<Region>Far/Mid/Close.png`，全部为 v2（照原作参考图重画）。雨隐、岩隐只出图未接入。
- **未实机验证**：背景在游戏里的缩放、视差与地平线高度；地区切换时的淡入淡出；NPC 实际入住、幸福度报告与晶塔售卖；木叶中卡卡西与向导的位置。预览图里的背景比例是估计值。
- Codex 生图有时段额度（本次连续 8 个请求后 429，约 1 小时恢复）；以后一次最多交 2～4 个。

**本会话新增（2026-09-30，待实机验收）**：

- 讨论定稿：“雨隐”是口误（=雾隐，音乐维持大桥范围）；村庄方案写入总纲末“地区与村庄”：木叶以出生点为中心、出生处即大门，完整城镇、房间够所有 NPC 住，作为 M2 第一部分；其余村子挂原版生物群系 + 小地标 + 专属 BGM/怪物/背景；不用子世界。
- **任务链 + 湖边初遇**（规格：`specs/M1_波之国下忍篇.spec.md`“剧情补充”）：`Common/StoryRules.cs`（阶段与数值，测试 `tests/StoryRules`）、`StoryWorld` 新标记 MetTazuna/TazunaConfessed/LakeDone（新包 `TazunaTalk`）、`StoryPlayer.CurrentObjective` 按阶段、达兹纳初见/坦白、`LakeAmbushSystem`（找湖、触发、提示、雾、音乐）、`WaterPrison`（导演 NPC：开场/战斗/结尾/撤退，画再不斩、白、被困卡卡西、水球、千本）、`WaterClone`、卡卡西在事件中被定住并藏进水球、迷雾预告须在初遇后且台词改为“又是你”、挑战卷轴配方加条件。美术 `water-prison-v1` 已接入。测试指令 `/m0 lake`、`/m0 story <1-5>`。
- 之后的试玩修正（均已提交，未实机）：水牢 NPC 缺贴图导致模组加载失败（已修）；鬼之兄弟首次伏击必定发生（带标记进大桥海雾）；挑战卷轴只能在大桥 70 格内使用；召唤过再不斩后雾中剪影/低语/千本警告/预告不再出现（世界标记 ZabuzaFought）；尾声雪渐显渐隐；白生命 900；白出场即开冰镜牢笼（用户确认再不斩仍退到笼外，选 A）；再不斩冲刺加刀锋判定框 `ZabuzaDashHitbox`。
- 验收用例 T17–T28 在 `tests/M1_波之国下忍篇.acceptance.md`，全部未实机执行。再不斩与白的 Boss 战改为“克眼之后”难度，数值待试玩复核。

**测试指令**：`/m0 items`、`/m0 god`、`/m0 time`、`/m0 bridge`、`/m0 preview`、`/m0 sighting`、`/m0 senbon`、`/m0 mist`、`/m0 brothers`、`/m0 forest`、`/m0 lake`、`/m0 story <1-5>`。

**已定（2026-09-29）**：“雨隐村”是口误，指雾隐，音乐维持大桥范围；村庄方案见总纲末“地区与村庄”（木叶完整城镇，其余为生物群系 + 小地标 + 专属 BGM/怪物/背景）。

**待用户决定**：音效方案（台词已定：只删我爱罗、大蛇丸的）；大蛇丸重画（暂缓）；本命章节与 M3 细节（开发到时再讨论）；专家专属饰品；补充道具（手里剑、起爆符、木叶护额、鬼之兄弟独有掉落）；大师模式遗物；是否推送 GitHub。

**下一步**：①（已完成：`wave-epilogue-walk-v2` 已接入，提交 `6d87a4a`；需游戏内 Build + Reload 后用 `/m0 epilogue zabuza|haku` 实机看）；②按用户反馈继续打磨 M1 之后的流程（下一段是笔试之后：死亡森林）；③用户确认后做战斗台词与音效。旧记录：用户 Build + Reload 后按 `tests/M2_中忍考试.acceptance.md` 实机验收，并确认地区背景中景/近景是否出现（修复在 c0a96a7、c2202d8，用户上次测试时游戏里仍是 01:06 的旧包；若仍不显示，查 `client.log` 是否有 “registered as 0 x 0”）；之后分批请求 M2 美术替换占位图；再 grill M3 木叶崩溃。

**工具**：`scripts/build_npc_sheet.py`、`scripts/build_head_equip.py`、`tools/BridgePreview` + `scripts/render_bridge_preview.py`、`scripts/art_bridge.py`（Codex 美术）。反编译原版用 `ilspycmd 8.2`（需 `DOTNET_ROLL_FORWARD=Major`），装在旧会话 scratchpad，不在仓库。

## 工作树与后台

- 以最前面“2026-10-03 Claude Code：装备与忍术系统”一节的“工作树”为准。`.art-bridge/`、`art/reference/`、`Sound_Track/`、`ShinobiPrototype/Assets/Music/`、`ShinobiPrototype/Assets/LocalSounds/` 是本地忽略目录，原作音频与参考图绝不提交；推送前检查 `git diff --name-only origin/main..HEAD`。

## 验证

- 以最前面一节的“验证”为准。通用流程：`./scripts/verify-mac.sh`（规则测试 + 编译；游戏开着时打包 TML003 正常，需游戏内 Build + Reload）；改了本地化就用 `hjson` 解析一遍；新增 NPC 或贴图后用 `./scripts/konoha-worldgen-check.sh` 在后台服务器加载一遍。

## 下次交接填写项

更新时保留有用事实、替换过期任务信息：正在做什么；已经完成（文件、提交）；接下来做什么（第一步、阻碍、待用户决定）；工作树（分支、未提交文件、后台任务 ID）；验证（执行过的命令与结果，未执行的游戏内检查）。

## 2026-10-02 Codex：Terruto 中文 Wiki 第一版

- 用户确定按传统百科网站方案制作：中文优先、电脑与手机可用、图鉴与攻略分开、以现有游戏实现为准，带 Boss 推进路线。第一版已完成，未公开部署、未推送。
- 文件全部位于 `wiki/`：`content.py` 人工核对说明，`build.py` 属性与贴图导出/静态页面生成，`style.css`、`app.js` 网站交互，`check.py` 链接/锚点/图片/源码变化检查，`README.md` 维护方法，`site/` 可托管产物。
- 当前 65 词条、74 页面：30 道具、7 首领/战斗、7 人物、3 敌怪、5 地区、6 机制、7 指南。独立属性框、配方、相关词条、全文搜索、分类过滤、移动导航、Boss 点击路线均已实现。测试专用品和旧版转换物品不混入正常获取图鉴；未来 M3、仙术及尾兽只放开发路线。
- 已按当前代码纠正旧说明：红豆对话领卷；死亡森林入口小队→中段大蛇丸→塔前雨隐；游荡小队不发另一卷；多斯找疾风启动；正式赛门槛为骷髅王或 400 生命；宁次在正式赛阶段可切磋；我爱罗砂墙与喘息窗口。Wiki 未改变模组玩法或接入后台美术。
- 本地预览命令：`python3 -m http.server 8765 --bind 127.0.0.1 --directory wiki/site`；地址 `http://127.0.0.1:8765/`，已请求在 Codex 侧栏打开。后续若进程已关闭，重跑命令即可。
- 构建：使用工作区带 Pillow 的 Python 运行 `wiki/build.py` 成功。`python3 wiki/check.py`：74 页内部链接、图片路径与锚点通过，核对时 0 个依赖文件变化。Playwright 通过已安装 Chrome 检查 1440px 和 390px 全部 74 页（148 次访问）：无页面错误、无 HTTP 错误、无横向溢出、无失效图片；搜索跳转、回车搜索、分类过滤、空结果提示和移动导航通过。已目视查看桌面首页、道具页与手机首页截图。
- 未运行模组构建与游戏实测（本任务仅网站）；不把网页或源码核对写成游戏内验收。
- 同一工作区开发仍在继续：制作期间 `main` 新增我爱罗守卫相关提交，已有多斯/大蛇丸贴图与源码的其他会话改动，以及后台美术交付。Codex 本任务只写 `wiki/` 并追加本段，未暂存或提交其他会话文件。`site/source-manifest.json` 固定构建时依赖哈希；后续先运行 `check.py` 检查变化，核对相应说明后重新构建，避免百科落后于游戏。
- 下一步最小动作：用户查看首页与词条，提出外观/内容调整；需要让朋友访问时再配置公开静态托管。任何玩法修改后同步核对 `wiki/content.py` 并重建。
- 收尾核对期间检测到其他会话更新了大蛇丸源码哈希；已重新读取并重建，最后一次 `check.py` 结果再次为 74 页链接/图片/锚点通过、0 个依赖文件变化。素材随最后一次构建同步。

## 2026-10-02 夜间 Claude Code：三个考试 Boss 画风统一 + 我爱罗新机制（用户授权自主完成，未推送）

- 用户睡前定下的四项全部完成并提交到本地，**未推送**（推送须用户明说）。前后对比图在 `art/reviews/overnight-2026-10-02/`（`gaara_before_after.png`、`orochimaru_before_after.png`、`dosu_before_after.png`、`gaara_fx.png`）。
- **多斯黑色小人**：`37990b5`。成因是 PoseFirstFrame 的帧偏移套到了走路帧上（请求了不存在的 Walk_4/5），回落到旧占位贴图；现在缺帧一律画站立帧。
- **画风统一（全部以达兹纳为准，每个角色从一张底稿改出全套）**：
  - 我爱罗：底稿 `gaara-base-v2`，全套 `gaara-set-v2`（站立 6、走路 8、施术/沙浪引帧、守护、守护后喘气、碎甲 4）+ `gaara-beast-v2`（守鹤化，含引帧），`e8bc643`；
  - 大蛇丸：底稿 `orochimaru-base-v4`→`v5`（头约占身高四分之一、2×2 金色蛇眼、紫眼影、头发不挡脸），全套 `orochimaru-set-v5a`（站立、走路、受击、现身、钻出、遁地、伸颈头与颈节、头像）+ `orochimaru-set-v5c`（招式与引帧），`73e6765`。`v5b` 招式帧画成了侧脸看不到眼睛，自检退回，未接入。伸颈起点和五行封印指尖火焰已按新帧重新量过；
  - 多斯：底稿 `dosu-base-v2`（噪点，退回）→`v3`，全套 `dosu-set-v4`（每帧同一 27 色调色板，臂甲在站立和走路帧都垂在身侧），`73e6765`。`dosu-set-v3` 只剩 8～14 色、站走臂甲姿势不一致，退回；
  - 伪装考生 `Orochimaru_DisguiseWalk_*` 保持原样（草隐考生，不是大蛇丸本人的脸）。
  - 身高：再不斩约 92 屏幕像素，新我爱罗约 102，新大蛇丸约 108（比旧版高约 16%），多斯驼背约 90。
- **我爱罗新机制**（`fe2a6bf`、`505e4ab`、`6225862`；规则在 `ExamBossRules`，测试 +6）：
  - 沙之守护取代旧的“正面一律减伤”：每 8～10 秒，或正面累计受伤 ≥260 时提前，身前竖沙墙站定 3 秒，正面伤害 -90%、背后全额；之后喘气硬直 2 秒；守鹤化后不再使用；碎甲时不打断守护；
  - 弹幕：砂手里剑扇形（5 发，碎甲后 7 发）、流沙（玩家脚下及两侧标记，50 帧后喷沙柱，3 处/碎甲后 5 处，间隔 96 像素）、走路时甩沙弹（每 65 帧/碎甲后 40 帧）；沙缚柩不变；守鹤化保留巨臂与空气弹；
  - 特效美术 `gaara-fx-v1`（沙墙、砂手里剑、流沙标记、沙柱、沙浪）已接入；沙墙先升起再循环，沙浪先涨再卷。
- 公共改动：`ExamBoss.ArtIdleFrames/ArtWalkFrames`（按素材自动用 6/8 帧）和 `WithLeadIn`（招式前 6 帧用 `<动作>In_0`），我爱罗、多斯已用；大蛇丸仍用自己的同等逻辑。
- README 预览（多斯、我爱罗、我爱罗三形态、伊比喜）已刷新，`c37392c`。Codex 的 Wiki（`wiki/`）描述的是贴图更新前的状态，需按其 README 运行 `check.py` 后重建。
- **验证**：每次提交前 `./scripts/verify-mac.sh` 通过，717 项规则测试全过，0 警告 0 错误，打包成功。**未实机**：需要游戏内 Build + Reload 后用 `/m0 orochimaru`、塔内多斯、会场我爱罗看：沙墙位置和守护时机、流沙间隔是否好躲、甩沙弹频率、新帧在 1.5 倍下的观感、大蛇丸伸颈起点和封印火焰是否对准手。
- **下一步**：用户实机反馈；之后按队列做宁次、再不斩、白的补帧与统一（请求 `neji-tween-v1`、`zabuza-tween-v1`、`haku-tween-v1` 已写好未提交，提交前应改成“从统一底稿出全套”的写法），以及再不斩、白的动作平滑。
- 工作树：除 `wiki/`（Codex）与两个历史残留交付目录外无未提交改动；无后台美术任务在跑。

## 2026-10-02 Codex：Wiki 外观第二版（按用户要求参考官方 Wiki 与灾厄）

- 用户认为第一版外观不好，明确要求借鉴 Terraria 官方 Wiki 和灾厄 Wiki。已浏览两站实际首页、官方 Weapons 目录和 Night's Edge 词条，重做 `wiki/build.py` 页面模板与 `wiki/style.css`。
- 采用本模组木叶火影岩背景、顶部站点标识、深色土色正文、金色边框、青绿色链接、分组导航与页面标签。首页改成欢迎栏、模组/版本分栏和紧凑图标目录；道具页按武器、核心、饰品、材料、召唤物、任务、装饰七类生成表格，武器直接对照类型/伤害/使用时间。词条使用目录、正文与右侧属性框；移动端属性框移到正文上方。
- `wiki/app.js` 筛选改为同时隐藏没有结果的道具分组；CSS/JS 和导出贴图 URL 带内容哈希，避免旧资源缓存混入新版。重新构建同步了已接入的最新多斯、大蛇丸和我爱罗素材；未接入任何后台美术。
- 验证：74 页面内部链接、锚点、资产路径及源码新鲜度检查通过；PNG 解码通过。浏览器分别用 1440/390px 检查首页、道具、首领、人物、武器词条、双首领、推进路线和关于页，无整页横向溢出或已加载失效图片；写轮眼筛选仅剩一行/一组、空结果提示与搜索回车跳转通过。已目视检查两个宽度的页面；手机武器表格可在表格内横向滑动。
- 预览截图 `wiki/previews/home-desktop.jpg`；当前本地地址仍是 `http://127.0.0.1:8765/`。65 词条、74 页面保持，未公开部署、未推送；未修改游戏源码、未运行模组构建或实机验收。

## 2026-10-02 Codex：按最新开发状态更新 Wiki

- 用户要求“更新 wiki”。以当前 `main` / `250a606` 工作区核对；旧产物落后于大蛇丸源码和待机贴图。已重建 `wiki/site/`，同步已经接入的大蛇丸 v11 贴图；未接入未使用的美术交付，也未修改模组代码及其他会话的本地化改动。
- `wiki/content.py` 补全大蛇丸蛇群、毒液/毒池、蛇雨、草薙剑、巨蛇触发与二次杀气，补充我爱罗砂手里剑、流沙、砂弹、沙缚柩、守护破绽与应对建议。依照 NPC、弹幕及规则源码核对；对应弹幕文件加入依赖清单，后续修改可被 `check.py` 检出。
- `wiki/build.py` 搜索索引增加内容哈希，避免刷新后仍用旧关键词。65 词条 / 74 页面，沿用现有外观；本地预览仍为 `http://127.0.0.1:8765/`。
- 验证：带 Pillow 的工作区 Python 构建成功；`python3 wiki/check.py` 通过 74 页面链接、锚点和图片路径，0 依赖变化；PNG 全部解码通过。浏览器搜索“草薙剑”显示并跳转大蛇丸，两篇更新词条在 1440/390px 无页面横向溢出、无失效图片。已目视检查新贴图，预览保存 `wiki/previews/orochimaru-updated.jpg`。
- 未运行模组构建或游戏实测；本次仅更新网页。`wiki/` 仍未跟踪，交接文档追加更新；其他会话文件保持原样，未推送。
- 下一步最小动作：玩法或已接入素材变化后，先运行 `wiki/check.py`、核对人工说明，再重建。
