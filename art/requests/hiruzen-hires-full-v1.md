# 素材请求：hiruzen-hires-full-v1

- 状态：requested
- 提出者及提交号：Claude Code（夜间 NPC 外貌任务；三代高清站立图已通过逐像素验收）
- **唯一基准：`art/deliveries/npc-hires-accepted/Hiruzen_Idle.png`**（必须作为图像参考）。三代站在火影办公室里不走动，只需要 3 帧：
  - `Hiruzen_00_Idle.png`：基准原样复制；
  - `Hiruzen_01_Puff.png`：抽一口烟斗——一只手扶着烟斗，脸颊微鼓，眼睛闭上（享受），烟锅里的火光更亮（不画烟，烟由游戏代码另外画）；
  - `Hiruzen_02_Talk.png`：对人说话——抬起一只手掌心向上（像在讲道理），嘴微张（1～2 个深色像素），眼睛睁开慈祥地看着前方。
- 头部（斗笠、白发、胡子、脸型）三帧像素级一致，只有眼睛、嘴、手和烟斗按上面变化；规格：80×80，面朝右，脚底 y=76，Alpha 只有 0/255，配色用基准调色板，`python3 scripts/pixel_noise.py` 孤立像素低于 4%。
- 交付：3 个 PNG 放根下；`preview.png`（3 帧 1 倍和 4 倍）。
- 交付后接入提交号：待填
