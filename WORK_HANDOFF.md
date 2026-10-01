# 当前工作交接

本文件供 Claude Code 与 Codex 在同一项目目录切换时恢复任务状态。以下为 **2026-10-01 Claude Code 打磨 M1 收尾 → M2 开头途中** 的快照（用户要切新窗口）。接手时还要看用户最新消息、实际文件和 Git 状态；长期规则见 `AGENTS.md`，Mac 环境见 `DEVELOPMENT_MAC.md`。

## 项目与规格入口

- 根目录 `/Users/tuzhechen/Documents/ChatGPT/泰拉瑞亚火影模组`，唯一源码 `ShinobiPrototype/`。GitHub 公开仓库：https://github.com/tuzhechen2005/terraria-naruto-mod （分支 `main`）。
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

- 分支 `main`，领先 origin/main 23 个提交未推送（origin 在 `b411ffe`）；未跟踪：`art/deliveries/orochimaru-style-v1/`（用户说大蛇丸先不管）（推送需用户明确要求，推送前确认没有音乐文件进入提交）。
- 运行中的美术请求：无（`.art-bridge/bg-konoha-v1.json` 显示 running 是早已被 v2 取代的陈旧状态，忽略）。`.art-bridge/`、`art/reference/` 为本地忽略目录。

## 验证

- 任务链与湖边初遇：`./scripts/verify-mac.sh` 规则测试全部通过（含新 StoryRules 18 项），C# 编译 0 错误；游戏开着导致打包 TML003，需游戏内 Build + Reload 后实机测 T17–T28。
- 更早的代码改动（尾声、白台词）只跑过 `./scripts/verify-mac.sh` 的规则测试与编译；游戏开着时命令行打包会 TML003，需游戏内 Build + Reload。
- 图标只复制了文件；用户随后成功上传，说明游戏内构建已包含它。

## 下次交接填写项

更新时保留有用事实、替换过期任务信息：正在做什么；已经完成（文件、提交）；接下来做什么（第一步、阻碍、待用户决定）；工作树（分支、未提交文件、后台任务 ID）；验证（执行过的命令与结果，未执行的游戏内检查）。
