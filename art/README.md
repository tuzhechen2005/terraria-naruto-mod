# M1 美术素材

本轮使用内置 imagegen 生成透明背景的原创像素风源图，保存在 `source/`；`Prepare-Icons.ps1` 将源图按透明像素裁切、缩放到 tModLoader 使用的 PNG 尺寸。生成后的图标和角色图在 `ShinobiPrototype/Content/`，是项目资产，不依赖本机图片缓存。

生成提示词要点（英文）：

- `profile-scroll.png`：Terraria-style 2D inventory icon；靛蓝布包裹的忍者训练卷轴、五元素结印；透明背景、无文字，32×32 可读。
- `training-kunai.png`：Terraria-style 2D inventory icon；黑铁三角苦无、缠绳握柄和环状尾部；透明背景、无文字，32×32 可读。
- `ninjutsu-manual.png`：Terraria-style 2D inventory icon；深蓝装帧的忍术研习手册、五色查克拉标记；透明背景、无文字，32×32 可读。
- `haku.png`：侧视人形冰遁忍者，白色面具、黑色长发、靛蓝服装；全身单帧、透明背景、无场景。
- `zabuza.png`：侧视人形叛忍剑士，面部包扎、深色衣装、背负阔刃剑；全身单帧、透明背景、无场景。
- `mission-scroll.png`：红绳与封蜡的旧任务卷轴，复用于两种 Boss 挑战卷轴。
- `mist-insignia.png`：蓝色波浪嵌饰的雾隐身份标记。
- `mist-scout.png`：蓝灰色装束、蒙面的雾隐侦察兵，全身单帧。
- `wave-medal.png`：蓝色桥与浪花纹章的铜质功绩牌。
- `chakra-technique.png`：掌心悬浮的蓝色查克拉球，复用于基础遁术图标。
- `exam-admission.png`：红色公章的中忍考试报名书，复用于预选赛挑战书。
- `heaven-scroll.png`：天蓝系绳与金色太阳标记的天之卷轴。
- `earth-scroll.png`：苔绿系绳与铜色山纹的地之卷轴。
- `chunin-headband.png`：靛蓝布条与钢板的中忍护额。
- `forest-candidate.png`：绿色轻甲的丛林试炼考生，全身单帧。
- `arena-rival.png`：背负小型沙葫芦的原创砂隐考生，无攻击特效的全身单帧。

角色当前是静态单帧，战斗招式仍由粒子和攻击判定表达；后续需要补移动、蓄力和受击动画。源图是创作材料，不代表像素级最终定稿。
# Claude 与 Codex 之间的素材请求及交付格式见 [AGENT_HANDOFF.md](AGENT_HANDOFF.md)。
