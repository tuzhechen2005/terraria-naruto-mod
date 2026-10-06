# 素材请求：kakashi-hires-full-v1

- 状态：00 Idle 与头部通过，其余由 v2 取代
- 提出者及提交号：Claude Code（夜间 NPC 外貌任务；Kakashi 高清站立图已由 Claude 逐像素验收通过）
- 资产类型：城镇 NPC 全套动作（高清规格）
- **唯一基准：`art/deliveries/npc-hires-accepted/Kakashi_Idle.png`**（必须作为图像参考）。人物：旗木卡卡西：银色刺猬头向左后方斜、护额斜戴遮左眼、露出半睁慵懒的右眼、深蓝面罩；绿色中忍马甲、深蓝忍装、手套，手里一本橙色小书。每一帧都是同一个人：**头部（发型、护额、眼睛、眉毛、疤/面罩、嘴）在所有帧里像素级保持一致**，只随身体上下移动 0～1 像素；配色就用这张的调色板（不超过 20 色）；身材、轮廓画法不变。画风也参照已通过的达兹纳 `art/deliveries/npc-hires-accepted/Tazuna_Idle.png`。
- 规格：1 像素 = 1 屏幕像素，每帧 80×80，面朝右，脚底 y=76（跳跃帧脚可以离地），人体中线约 x=40，透明背景，Alpha 只有 0/255；`python3 scripts/pixel_noise.py` 每帧孤立像素低于 4%（交付说明贴输出）。
- 帧（12 张，放根下）：
  - `Kakashi_00_Idle.png`（就是基准图，原样复制）；
  - `Kakashi_01_Walk.png`～`Kakashi_06_Walk.png`：步行：懒散地一手拿书边走边看，另一手插兜或自然垂下；6 帧循环，两腿交替、手臂反向摆，首尾能接上；
  - `Kakashi_07_Jump.png`：跳起，膝盖收起；
  - `Kakashi_08_Sit.png`：坐着（膝盖弯 90 度，脚放在地上 y=76），低头看橙色小书；
  - `Kakashi_09_Throw.png`～`Kakashi_11_Throw.png`：扔苦无自卫：书收起或换手，举臂蓄力、甩出、收手。
- 交付：12 个 PNG；`sheet_preview.png`（12 帧一行，1 倍和 3 倍）；`walk.gif`（6 帧循环，每帧 100ms）；`heads_8x.png`（12 帧头部放大 8 倍并排，检查五官一致）。
- 需要避免：任何一帧的脸和基准不一样；杂点；身高变化。
- 交付后接入提交号：待填
