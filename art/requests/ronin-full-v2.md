# 素材请求：ronin-full-v2

- 状态：requested
- 提出者及提交号：Claude Code（`ninja-mobs-full-v1` 反馈：浪人除样张外的帧满是零散橙色噪点，轮廓变成橙色，连重交的 Idle 都比样张糊；叛忍那一套很好，不用改）
- 资产类型：角色动作组（只做浪人）
- **底稿：`art/deliveries/ninja-mobs-style-v1/Ronin_Idle.png`、`Ronin_Slash_0.png`、`Ronin_Slash_1.png`**（用户认可的样张，必须作为图像参考，这三张直接沿用、不重画）。其余帧**从这张 Idle 逐帧改出来**：同一个人、同一调色板、同样干净的色块。
- **只许用样张 Idle 的调色板**（交付前检查：每帧的不透明颜色必须是样张 Idle 颜色的子集）。**轮廓一律是样张那样的深色**，不许出现橙色或浅色轮廓；身体内部不许有零散的单个像素点——每一块颜色至少 2×2 美术像素成团。
- 格式与 `art/deliveries/ninja-mobs-full-v1/` 相同：每格 112×88，2×2 屏幕像素为一美术像素，中线 x=56，脚底最低行 y=83，面朝右，Alpha 只有 0/255。
- 需要的帧：`Ronin_Walk_0`～`_3`（提刀小跑 4 帧，首尾循环；头和躯干基本不动，两腿交替，步伐粗重）、`Ronin_Jump`（跳起 1 帧）。另把三张样张原样复制一份到交付目录根下（`Ronin_Idle`、`Ronin_Slash_0`、`Ronin_Slash_1`）。
- 参照动作画法：`art/deliveries/ninja-mobs-full-v1/RogueGenin_Walk_*.png`（这套干净的跑步帧是质量基准）。
- 交付：源图 `source/`；预览 3 倍：新帧和样张 Idle、叛忍跑步帧并排。
- 交付后接入提交号：待填
