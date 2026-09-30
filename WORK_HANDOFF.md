# 当前工作交接

本文件供 Claude Code 与 Codex 在同一项目目录切换时恢复任务状态。以下为 **2026-09-30 Claude Code 完成 M2 中忍考试篇（占位美术）后** 的快照。接手时还要看用户最新消息、实际文件和 Git 状态；长期规则见 `AGENTS.md`，Mac 环境见 `DEVELOPMENT_MAC.md`。

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

## 当前状态（2026-09-30）

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
- **未做**：流派核心物品（写轮眼试管、八门、白眼）——等流派第一阶设计确认；蛇鳞材料；真正的 Boss 美术与三代、雨隐、考生美术（需 Codex，每批 2～4 个）；M3。

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

**待用户决定**：流派第一阶的被动与奥义（`specs/流派系统.spec.md` 第三节，决定后才做核心物品）；专家专属饰品；补充道具（手里剑、起爆符、木叶护额、鬼之兄弟独有掉落）；大师模式遗物；是否推送 GitHub。

**下一步**：用户 Build + Reload 后按 `tests/M2_中忍考试.acceptance.md` 实机验收，并确认地区背景中景/近景是否出现（修复在 c0a96a7、c2202d8，用户上次测试时游戏里仍是 01:06 的旧包；若仍不显示，查 `client.log` 是否有 “registered as 0 x 0”）；之后分批请求 M2 美术替换占位图；再 grill M3 木叶崩溃。

**工具**：`scripts/build_npc_sheet.py`、`scripts/build_head_equip.py`、`tools/BridgePreview` + `scripts/render_bridge_preview.py`、`scripts/art_bridge.py`（Codex 美术）。反编译原版用 `ilspycmd 8.2`（需 `DOTNET_ROLL_FORWARD=Major`），装在旧会话 scratchpad，不在仓库。

## 工作树与后台

- 分支 `main`，工作树干净，领先 origin/main 约 51 个提交未推送（推送需用户明确要求，推送前确认没有音乐文件进入提交）。
- 无运行中的美术请求。`.art-bridge/`、`art/reference/` 为本地忽略目录。

## 验证

- 任务链与湖边初遇：`./scripts/verify-mac.sh` 规则测试全部通过（含新 StoryRules 18 项），C# 编译 0 错误；游戏开着导致打包 TML003，需游戏内 Build + Reload 后实机测 T17–T28。
- 更早的代码改动（尾声、白台词）只跑过 `./scripts/verify-mac.sh` 的规则测试与编译；游戏开着时命令行打包会 TML003，需游戏内 Build + Reload。
- 图标只复制了文件；用户随后成功上传，说明游戏内构建已包含它。

## 下次交接填写项

更新时保留有用事实、替换过期任务信息：正在做什么；已经完成（文件、提交）；接下来做什么（第一步、阻碍、待用户决定）；工作树（分支、未提交文件、后台任务 ID）；验证（执行过的命令与结果，未执行的游戏内检查）。
