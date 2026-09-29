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

## 未完成与下一步（2026-09-29 用户外出期间 Claude 自主推进后的状态）

**M1 进度**（规格 `specs/M1_波之国下忍篇.spec.md`，实施阶段 1–6）：

- 阶段 1 查克拉/兵粮丸/结晶/替身术：完成；替身术用户已确认可用。
- 阶段 2 卡卡西/忍者手册/模组设置：完成（卡卡西 v2 形象、1.1 倍、补发手册与施工图、替身练习八连投）。
- 阶段 3 Boss 奖励：保底四选一武器（+约 20% 第二把）、专家宝藏袋、面具 1/7、每角色首胜给结晶与功绩牌、四把武器**初版数值待用户试玩确认**（见规格表格）、苦无改远程、手册卷宗/本章奖励/忍道页、Boss Checklist。**未做**：专家专属饰品（待讨论）、大师模式遗物；奖杯正在做（`wave-trophies-v1`）。
- 阶段 4 大桥/海雾/预告/达兹纳：完成。大桥 `BridgeDesign.Version` = 4（`/m0 bridge` 显示本世界版本），**旧世界需新建**才能看到新桥与整平的小屋。海雾经多轮试验定为“复用再不斩二阶段雾的全屏平涂 + 较淡小圆雾粒，颜色随天空亮度（夜里不发亮），画在前景水面之上”，默认浓度 100%；再不斩战斗转场雾与白的冰闪也并入同一层。雾中身影：剪影+隐名、每角色每世界一次预告、反复出没、千本警告。达兹纳有自己的形象（`tazuna-npc-v1`）。
- 阶段 5 鬼之兄弟：完成（夹击、锁链横扫、一死另一暴走；v2 形象）。独有掉落待讨论。
- 阶段 6 手册剪影与美术：忍道/本章奖励页已接入图标。
- 森林里的少年（不戴面具的白）：完成，Boss 战白会对见过的玩家回忆。
- Boss 战：再不斩冲刺按克眼重做（预判、全向 60°、穿地形、冲过头、1/2/3 连冲）；卡住或离太远时“瞬身术”（淡出、在玩家身后空地出现）。规格 `specs/M10_...` 末尾有记录。**待用户实机确认**手感。

**测试指令**（M0 规格有说明）：`/m0 items`、`/m0 god`、`/m0 time`、`/m0 bridge`、`/m0 preview`、`/m0 sighting`、`/m0 senbon`、`/m0 mist`、`/m0 brothers`、`/m0 forest`。

**待用户决定**：专家专属饰品；补充道具（手里剑、起爆符、木叶护额、鬼之兄弟独有掉落）；四把武器与鬼之兄弟数值；大师模式遗物是否要做。

**工具**：`scripts/build_npc_sheet.py`（NPC/敌人帧表）、`scripts/build_head_equip.py`（头部装备 20 帧）、`tools/BridgePreview` + `scripts/render_bridge_preview.py`（离线看大桥）、`scripts/art_bridge.py`（Codex 美术）。反编译原版代码：scratchpad 里装过 `ilspycmd 8.2`（需 `DOTNET_ROLL_FORWARD=Major`），不在仓库内。

**未实机**：以上所有 2026-09-29 下午之后的改动都只做了规则测试与编译，未进游戏验证。命令行打包常因游戏开着失败（TML003），需在游戏内 Build + Reload。

## 工作树与后台

- 分支 `main`（大量 M1 提交尚未推送到 GitHub）；`preview/style-c` 已合并（可删除）。
- 无运行中的美术请求（`m1-chakra-items-v1`、`kakashi-npc-v1`、`kakashi-npc-v2` 已交付）；Codex 在 2026-09-29 触发用量上限（14:01 后恢复）；`.art-bridge/`、`art/reference/`（含原版贴图与用户截图）为本地忽略目录，不跨机器。

## 下次交接填写项

更新时保留有用事实、替换过期任务信息：正在做什么；已经完成（文件、提交）；接下来做什么（第一步、阻碍、待用户决定）；工作树（分支、未提交文件、后台任务 ID）；验证（执行过的命令与结果，未执行的游戏内检查）。
