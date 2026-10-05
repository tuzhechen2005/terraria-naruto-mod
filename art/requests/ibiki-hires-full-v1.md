# 素材请求：ibiki-hires-full-v1

- 状态：requested
- 提出者及提交号：Claude Code（夜间 NPC 外貌任务；伊比喜高清站立图已通过逐像素验收）
- **唯一基准：`art/deliveries/npc-hires-accepted/Ibiki_Idle.png`**（必须作为图像参考）。伊比喜站在考场里不走动，只需要 2 帧：
  - `Ibiki_00_Folded.png`：双臂交叉抱在胸前（黑手套），挺直站立，严厉地盯着前方——这是他平时的样子；
  - `Ibiki_01_Point.png`：有人和他说话时，一只手臂向前伸出、食指指向前方（训话），另一只手仍插在风衣里或垂下，嘴微张。
- 头部（头巾、两道疤、眼神）两帧像素级一致，只有嘴在 01 微张；风衣、配色与基准一致；规格：80×80，面朝右，脚底 y=76，Alpha 只有 0/255，`python3 scripts/pixel_noise.py` 孤立像素低于 4%。
- 交付：2 个 PNG 放根下；`preview.png`（2 帧 1 倍和 4 倍，旁边放基准站立图）。
- 交付后接入提交号：待填
