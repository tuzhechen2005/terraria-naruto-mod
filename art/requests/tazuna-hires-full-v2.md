# 素材请求：tazuna-hires-full-v2

- 状态：requested
- 提出者及提交号：Claude Code（夜间 NPC 外貌任务，逐像素验收 `art/deliveries/tazuna-hires-full-v1/`）
- **唯一基准：`art/deliveries/npc-hires-accepted/Tazuna_Idle.png`**（必须作为图像参考）。动作画法照已通过的伊鲁卡、卡卡西全套（`art/deliveries/iruka-hires-full-v1/`、`art/deliveries/kakashi-hires-full-v2/`，必须作为图像参考）。
- v1 的问题（必须全部解决）：
  1. **衣服在动作帧里变了**：基准站立图是**白衬衫 + 敞开的深色背心（左右两片深色前襟）+ 搭在脖子上的毛巾 + 腰带 + 灰绿裤子 + 白绑腿 + 棕色鞋子**，酒瓶挂在身侧。v1 的走路、跳跃、投掷帧里**背心没了**（只剩两条背带），**鞋子变成黑色**，酒瓶跑到身前——一走路就像换了衣服。v2 每一帧都必须穿和基准一模一样的衣服：深色背心两片前襟、毛巾、腰带、棕色鞋子、白绑腿，配色只用基准的调色板。
  2. **投掷帧头两侧有断开的短线**（像速度线/杂点）——删掉，画面里除了人物和飞出的酒瓶，不要有任何不连着人物的像素。
  3. **甩出的手臂是一根米色长棍**：手臂要从肩膀长出来，有袖子褶皱和肤色的手，像伊鲁卡 `Iruka_10_Throw.png`（`art/deliveries/iruka-hires-fix-v1/`）那样。
- 帧（12 张）：`Tazuna_00_Idle.png`（基准原样复制）；`_01_Walk`～`_06_Walk`（老人略蹒跚的步伐，手臂反向摆，酒瓶随身侧的手摆动，6 帧循环）；`_07_Jump`（收腿，和身体连贯）；`_08_Sit`（坐着举起酒瓶喝酒）；`_09_Throw`～`_11_Throw`（扔酒瓶自卫：举臂蓄力、手臂向前伸直酒瓶飞出、收手）。
- 规格：1 像素 = 1 屏幕像素，80×80，面朝右，脚底 y=76，Alpha 只有 0/255；头部所有帧像素级和基准一致；`python3 scripts/pixel_noise.py` 每帧孤立像素低于 4%（贴输出）；另交 `torso_check.png`：12 帧的身体（脖子到脚）并排放大 4 倍，用来检查衣服一致。
- 交付：12 个 PNG；`sheet_preview.png`；`walk.gif`；`heads_8x.png`；`torso_check.png`。
- 交付后接入提交号：待填
