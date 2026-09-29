# 当前工作交接

本文件供 Claude Code 与 Codex 在同一项目目录切换时恢复任务状态。以下为 **2026-09-29 Claude Code 会话结束（上下文将满）时** 的快照。接手时还要看用户最新消息、实际文件和 Git 状态；长期规则见 `AGENTS.md`，Mac 环境见 `DEVELOPMENT_MAC.md`。

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

- 用户表示 **Boss 战先放一放**。下一项任务等用户指定；可能方向：
  1. 实现新 M1（`specs/M1_波之国下忍篇.spec.md`）：删除身份卷轴/查克拉冲击/忍术研习手册/训练奖励；苦无改远程；替身术（热键、`ModPlayer.FreeDodge`）；查克拉三种恢复（自然、全职业命中每秒上限、兵粮丸）与查克拉结晶（地下生成、Boss 首杀一次）；统一掉落池（四职业武器各约 25%，时装约 1%）；魔法/召唤武器做成“术”。
  2. 重写 M2 中忍考试：删除原创砂隐考生（`ArenaRivalBoss`、`ArenaChallengeScroll`、`ArenaRushHitbox`、`ArenaSandBolt` 及规则/测试/本地化），改为宁次（可选）+ 我爱罗（必打）+ 死亡森林大蛇丸遭遇战（掉写轮眼一勾玉）。
- Boss 战遗留小项（待用户回来再说）：白的冰镜空间等实机反馈调参；`Zabuza_Seal`/白 `Move` 帧质量实机确认。

## 工作树与后台

- 分支 `main`，与 `origin/main` 同步；`preview/style-c` 已合并（可删除）。
- 无运行中的美术请求；`.art-bridge/`、`art/reference/`（含原版贴图与用户截图）为本地忽略目录，不跨机器。

## 下次交接填写项

更新时保留有用事实、替换过期任务信息：正在做什么；已经完成（文件、提交）；接下来做什么（第一步、阻碍、待用户决定）；工作树（分支、未提交文件、后台任务 ID）；验证（执行过的命令与结果，未执行的游戏内检查）。
