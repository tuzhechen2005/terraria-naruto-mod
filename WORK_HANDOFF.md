# 当前工作交接

本文件供 Claude Code 与 Codex 在同一项目目录切换时恢复任务状态。以下为 **2026-09-29 Claude Code 开始实现 M1 后** 的快照。接手时还要看用户最新消息、实际文件和 Git 状态；长期规则见 `AGENTS.md`，Mac 环境见 `DEVELOPMENT_MAC.md`。

## 项目与规格入口

- 根目录 `/Users/tuzhechen/Documents/ChatGPT/泰拉瑞亚火影模组`，唯一源码 `ShinobiPrototype/`。GitHub 公开仓库：https://github.com/tuzhechen2005/terraria-naruto-mod （分支 `main`）。
- 上位设计：`specs/总纲_主线与支线结构.spec.md`（原版 Boss 全保留、月总为阶段门槛、主线必打链、疾风传解锁、原版职业 + 流派核心饰品、独立查克拉、尾兽/通灵兽支线、写轮眼完整路线）。旧 M1/M2 设计已作废。
- M1 波之国：`specs/M1_波之国下忍篇.spec.md`（2026-09-29 重写，未实现）。
- Boss 战：`specs/M9_波之国双首领战.spec.md` + `specs/M10_波之国双首领战优化.spec.md`（M10 已实现）。

## 用户已决定（本会话）

- 画风定稿：泰拉瑞亚原生风格（2×2 屏幕像素为一美术像素，块状有棱角，明亮饱和），标准为 `art/deliveries/style-test-v3/`（C 版再不斩）与 `art/deliveries/style-test-v7-haku/`（白）。再不斩黑衣（蓝灰色阶）。
- 鬼人化：紫色烟雾般若鬼影（参照火影手游，`art/reference/demon_mode_ingame_2.png`），非写实、非西方恶魔。暴走阶段（白先死）再不斩绷带脱落露脸。
- 美术一律交给 Codex，经 `scripts/art_bridge.py`；Codex worker 默认模型 `gpt-6-sol`、推理 medium（可用 `CODEX_MODEL`/`CODEX_EFFORT` 覆盖）。

## 已完成（均已提交并推送，最新 `main` 含 merge `b05fbed` 及其后的交接提交）

- M10 双首领战优化：白生成兜底、半血锁血、白穿墙飞行、删水分身；三段转场（跪地/白出镜/怒吼）+ 屏幕浓雾；冰镜牢笼（8 面可破镜、边界推回）；再不斩暴走（投刀往返、口咬苦无突进、受伤 +25%）；白暴走（6 面重生镜、千杀水翔）；台词；Boss 随世界光照（30% 亮度下限）；紫色鬼影与紫色特效。
- 全部 Boss 素材（再不斩 65 帧、白 19 帧、鬼影 9 帧、冰镜/千本/飞刀）已按定稿画风接入，由脚本重建：`scripts/process_wave_c.sh`（角色与道具）、`scripts/animate_apparition.py`（鬼影循环与浮现，从 `demon-apparition-v3` 单帧生成）。
- 工具：`scripts/pixelize_frames.py`（拆分/缩放/对齐，含 `--auto`、`--centers`、`--scale`）、`scripts/clean_frames.py`、`scripts/recolor_clothes.py`、`scripts/terraria_native.py`、`scripts/terraria_scene_preview.py`、`tools/XnbExtract/`（解原版 XNB 贴图，仅本地对比）。
- 环境：游戏内 Build + Reload 所需 SDK 通过 `~/.dotnet/dotnet → /usr/local/share/dotnet/x64/dotnet` 链接解决（见 `DEVELOPMENT_MAC.md`）。Codex CLI 已移到 `/Applications/ChatGPT.app/Contents/Resources/codex-cli/bin/codex`，桥接脚本已适配。

## 验证状态

- `./scripts/verify-mac.sh`：6 组规则测试全部通过，打包 0 错误（最后一次在素材接入后）。命令行打包要求 tModLoader 完全退出（否则 TML003）。
- 用户已在游戏内确认 M10 第一版机制“没啥问题”。**最终画风版（当前 main）尚未实机查看**：动作衔接、鬼影大小/透明度（代码中 0.82，暴走 0.95）、夜间/地下亮度待确认。
- 伤害数值未实测（用户决定暂不测）。

## 未完成与下一步

- **当前任务：实现 M1**（`specs/M1_波之国下忍篇.spec.md`，2026-09-29 与用户讨论定稿，含“实施阶段”1–6）。
- 用户已决定（M1）：大桥结构与胜利后大桥完工要做；鬼之兄弟精英敌人要做；迷雾预告要做；替身术开局可用，用“实战提示 + 卡卡西练习 + 忍者手册”三层教学；手册“忍道”页展示四个流派终极剪影；卡卡西新世界直接在出生点生成，**其余火影 NPC 必须满足条件才入住**；达兹纳胜利前固定在桥头，胜利后满足条件入住。补充道具（手里剑、起爆符、木叶护额等）**待与用户讨论**，未定前不做。
- **阶段 1 已完成（代码）**：删除身份卷轴 / 查克拉冲击 / 研习手册 / 五遁 / 训练奖励及 `tests/TrainingRules`；`Common/ChakraRules.cs`（纯规则）+ `tests/ChakraRules`；查克拉上限随结晶（100→200）、全职业命中回复（每击 2、每秒上限 5）；兵粮丸（蘑菇 + 太阳花，回 40，10 秒冷却减益）；查克拉结晶物品与地下 2×2 物体（世界生成 + 旧世界首次更新时补放）；替身术（热键默认 F，0.4 秒待命，4 秒冷却，木头 + 瞬移 + 无敌，HUD 显示冷却，受击提示最多 3 次）。美术 `m1-chakra-items-v1` 已交付并接入。
- **阶段 1 待实机**：替身术手感与落点、F 键是否与原版冲突、结晶在地下的数量与分布、旧存档加载（T02–T04、T13）。
- 2026-09-29 参考灾厄后用户决定：Boss 掉落改保底（四选一 + 约 20% 再给一件）、专家宝藏袋、面具 1/7、奖杯、遗物；加 Boss Checklist 支持；加模组设置；手册加“卷宗”页；**不引入灾厄等其他模组的专属内容**；原创音乐以后考虑。用户实机确认替身术“可以按出来”。
- **阶段 2 已完成（代码）**：卡卡西城镇 NPC（`Content/NPCs/Kakashi.cs`，开场白带当前任务，按钮“指点”轮流 6 条提示、“练习替身术”投无伤害练习苦无；`KakashiSpawnSystem` 在出生点生成、死亡 2 分钟后回到出生点）；忍者手册（`NinjaHandbook` + `HandbookSystem` 面板：任务 / 忍术 / 查克拉三页，旧 `MissionScroll` 进背包自动转换）；模组设置 `ShinobiClientConfig`（查克拉条偏移、教学提示开关）。美术 `kakashi-npc-v1` 已接入。
- 用户实机反馈（阶段 2）：卡卡西太瘦、走路像倒着、练习只扔一支且时机固定。已修（`5578c37` 及其后）：`FindFrame` 补 `spriteDirection = direction`（倒着走的真正原因）；练习改为 `SubstitutionDrillPlayer`——卡卡西本人站定面向玩家、举苦无瞄准随机时长后投出，共 8 支；玩家需在 10–25 格内，替身冷却中不出手、已瞄准的苦无等冷却结束再放；苦无 2 倍大小加白光与拖影；练习中替身免费、冷却 0.8 秒，逐支判定成功 / 早了 / 晚了 / 走开；美术换成 `kakashi-npc-v2`（按向导比例加宽，Claude 提亮 25%）。
- **阶段 2 待实机**：卡卡西朝向与新造型、脚底是否贴地（帧高 56、脚底 y=54，必要时调 `DrawOffsetY`）、练习节奏与难度、手册面板、设置生效（T01、T05）。**最近两次命令行打包因游戏开着被占用（TML003）未产出新 `.tmod`，需在游戏内 Build + Reload 或关游戏后重跑 `verify-mac.sh`。**
- **下一步**：阶段 3——苦无改远程；保底掉落 / 宝藏袋 / 面具 / 奖杯 / 遗物 / 首杀奖励；四把 Boss 武器；水乱波；卷宗页；Boss Checklist。专家专属饰品与补充道具需先与用户讨论。

## 工作树与后台

- 分支 `main`（M1 提交尚未推送）；`preview/style-c` 已合并（可删除）。
- 无运行中的美术请求（`m1-chakra-items-v1`、`kakashi-npc-v1`、`kakashi-npc-v2` 已交付）；Codex 在 2026-09-29 触发用量上限（14:01 后恢复）；`.art-bridge/`、`art/reference/`（含原版贴图与用户截图）为本地忽略目录，不跨机器。

## 下次交接填写项

更新时保留有用事实、替换过期任务信息：正在做什么；已经完成（文件、提交）；接下来做什么（第一步、阻碍、待用户决定）；工作树（分支、未提交文件、后台任务 ID）；验证（执行过的命令与结果，未执行的游戏内检查）。
