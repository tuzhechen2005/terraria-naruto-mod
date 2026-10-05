# 素材请求：rogue-genin-walk-v2

- 状态：requested
- 提出者及提交号：Claude Code（用户 2026-10-05：叛忍“攻击的时候和移动的时候形象不一样，移动的时候很胖，攻击的时候很瘦”；又看过 `ninja-mobs-full-v2` 说“还不如之前的”，所以**以之前的 v1 为准，只补这几帧**）
- 资产类型：角色动作帧（叛忍下忍 5 帧）
- 问题：`art/deliveries/ninja-mobs-full-v1/` 里 `RogueGenin_Walk_0～3` 和 `RogueGenin_Jump` 画成了另一个人（兜帽、下半张脸蒙面、身形壮），而 `RogueGenin_Idle`、`Throw`、`Slash_0/1` 是露脸、瘦削的。
- 要做：**只重画 `RogueGenin_Walk_0`～`_3`（持苦无压低身体小跑 4 帧，首尾循环）和 `RogueGenin_Jump`（跳起 1 帧）**，造型完全照 v1 的 `RogueGenin_Idle.png`（必须作为图像参考）：同样的头、同样露出的脸、同样带划痕的护额、同样瘦削的身材、同样的配色和画法。逐帧和 Idle 对比头部大小、身体宽度、颜色。
- **画法和 v1 相同**（生成 → 采样 → 对照生成图逐像素修整，不要逐像素从零手画整个人）；格式与 v1 相同：每格 112×88，2×2 屏幕像素为一美术像素，人体中线 x=56，脚底最低行 y=83，面朝右，透明背景，Alpha 只有 0/255。身高与 v1 的 Idle 一致（尺寸在代码里统一缩放）。
- 不要动 v1 的其他四帧。
- 交付：5 个 PNG 放根下，源图 `source/`；`preview.png`：v1 的 Idle、Throw、Slash_0、Slash_1 与新的 5 帧排成一行（1 倍和 3 倍），一眼看出是同一个人。
- 交付后接入提交号：待填
