# 素材请求：toolshop-hires-full-v3

- 状态：requested
- 提出者及提交号：Claude Code（夜间 NPC 外貌任务，逐像素验收 `art/deliveries/toolshop-hires-full-v2/`）
- v2 通过的帧（**不要改**）：`00_Idle`、`07_Jump`、`09_Throw`、`11_Throw`。围裙和衣服已经每帧一致，保持。
- 要重画的帧（以 v2 的同名帧和 `art/deliveries/npc-hires-accepted/ToolShopkeeper_Idle.png` 为基准，必须作为图像参考）：
  1. **`01_Walk`～`06_Walk`**：腿和身体保留 v2；问题是**往后摆的那只手是一个 5×4 的肉色方块**。改成和伊鲁卡走路（`art/deliveries/iruka-hires-full-v1/Iruka_02_Walk.png`）一样：酒红色袖子从肩膀连到手腕，手是 2×3 左右的握拳，有深色轮廓；前后手臂随腿反向摆动。
  2. **`08_Sit`**：v2 里他**站着**，凳子在旁边。改成**真的坐在凳子上**：凳子在他身下，臀部压在凳面上，大腿水平向前、小腿垂直向下、脚在 y=76；双手在身前用一块白布擦一支苦无；头部像素和基准一致、整体下移。参考 `art/deliveries/iruka-hires-full-v1/Iruka_08_Sit.png`。
  3. **`10_Throw`**：v2 里苦无飞出去了，但**看不到扔的那只手臂**，身后腰部还有一团棕色的东西。改成：右臂从右肩向前完全伸直（酒红袖子 + 手），手刚松开，苦无在手前方 2～4 像素处；删掉身后那团东西；照 `art/deliveries/iruka-hires-fix-v1/Iruka_10_Throw.png` 的画法。
- 规格：80×80，面朝右，脚底 y=76，Alpha 只有 0/255；头部像素级和基准一致；`python3 scripts/pixel_noise.py` 每帧孤立像素低于 4%；除飞出的苦无外不要有悬空像素。
- 交付：`ToolShopkeeper_01_Walk.png`～`_06_Walk.png`、`_08_Sit.png`、`_10_Throw.png` 放根下；`sheet_preview.png`（含 v2 通过帧的完整 12 帧，1 倍和 3 倍）；`walk.gif`。
- 交付后接入提交号：待填
