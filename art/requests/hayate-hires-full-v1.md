# 素材请求：hayate-hires-full-v1

- 状态：requested
- 提出者及提交号：Claude Code（夜间 NPC 外貌任务；疾风高清站立图已通过逐像素验收）
- **唯一基准：`art/deliveries/npc-hires-accepted/Hayate_Idle.png`**（必须作为图像参考）。疾风站在中央塔大厅里不走动，只需要 3 帧：
  - `Hayate_00_Idle.png`：基准原样复制；
  - `Hayate_01_Pose.png`：**咳嗽**——一只手握拳挡在嘴前，肩膀微缩、上身略前倾，眼睛闭上（每隔一会儿出现一次）；
  - `Hayate_02_Talk.png`：对考生说话——一只手抬到胸前、掌心朝上，嘴微张（1 个深色像素），眼神疲惫但认真。
- 头部（护额头巾、发型、黑眼圈、脸型）三帧一致，只有眼睛、嘴、手臂按上面变化；绿色中忍马甲、背上的刀与基准一致；规格：80×80，面朝右，脚底 y=76，Alpha 只有 0/255，配色用基准调色板，`python3 scripts/pixel_noise.py` 孤立像素低于 4%。
- 交付：3 个 PNG 放根下；`preview.png`（3 帧 1 倍和 4 倍）。
- 交付后接入提交号：待填
