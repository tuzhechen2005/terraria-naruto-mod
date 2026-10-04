# 当前工作交接

本文件供 Claude Code 与 Codex 在同一项目目录切换时恢复任务状态。最新工作见下方 **2026-10-03 Claude Code：装备与忍术系统（第一步完成）**；后面保留此前开发快照。接手时还要看用户最新消息、实际文件和 Git 状态；长期规则见 `AGENTS.md`，Mac 环境见 `DEVELOPMENT_MAC.md`。

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
