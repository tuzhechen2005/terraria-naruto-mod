# 素材请求：haku-hires-full-v1

- 状态：requested
- 提出者及提交号：Claude Code（夜间 NPC 外貌任务；白（森林采药、不戴面具）高清站立图已通过逐像素验收）
- **唯一基准：`art/deliveries/npc-hires-accepted/HakuForest_Idle.png`**（必须作为图像参考）。
- 画风与规格：照已通过的伊鲁卡全套（`art/deliveries/iruka-hires-full-v1/`，必须作为图像参考：走路摆臂、跳跃收腿、坐姿、投掷的画法都照它）。1 像素 = 1 屏幕像素，每帧 80×80，面朝右，脚底 y=76（跳跃可离地），透明背景，Alpha 只有 0/255；**头部在所有帧里像素级保持和基准站立图一致**，只随身体上下移动 0～1 像素；配色就用基准图的调色板；`python3 scripts/pixel_noise.py` 每帧孤立像素低于 4%（贴输出）。手臂要从肩膀长出来、画出手；腿和身体连贯；不许出现方块腿、棍子手臂。
- 帧（10 张，放根下）：`HakuForest_00_Idle.png`（基准原样复制）；`_01_Walk`～`_06_Walk`（轻盈安静的步伐，长发随步伐轻轻摆动，6 帧循环）；`_07_Crouch`、`_08_Crouch`（蹲下采药两帧：伸手摘草、把草放进篮子；脚底仍在 y=76）；`_09_Talk`（站着侧过脸对玩家温和地笑，一只手轻按胸口）。
- 交付：10 个 PNG；`sheet_preview.png`（10 帧一行，1 倍和 3 倍）；`walk.gif`；`heads_8x.png`。
- 交付后接入提交号：待填
