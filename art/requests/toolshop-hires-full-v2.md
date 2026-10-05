# 素材请求：toolshop-hires-full-v2

- 状态：requested
- 提出者及提交号：Claude Code（夜间 NPC 外貌任务，逐像素验收 `art/deliveries/toolshop-hires-full-v1/`）
- **唯一基准：`art/deliveries/npc-hires-accepted/ToolShopkeeper_Idle.png`**（必须作为图像参考）。动作画法照已通过的伊鲁卡、达兹纳全套（`art/deliveries/iruka-hires-full-v1/`、`art/deliveries/tazuna-hires-full-v2/`，必须作为图像参考）。
- v1 的问题（必须全部解决）：
  1. **`08_Sit` 画了两个头**（原来的头下面又叠了一个头）——整帧重画：坐在凳子上（大腿水平、小腿垂直、脚在 y=76），低头用布擦一支苦无，**只有一个头**，和基准一样的头部像素整体下移。
  2. **`01_Walk` 变成正面朝前**——所有帧都是四分之三侧身朝右，和基准一致。
  3. **围裙每帧样式不同**（基准是腰部以下的一块深色帆布围裙，有前袋；v1 的走路、投掷帧变成带背带的大围兜，且每帧不同）——每一帧的围裙必须和基准**完全同一个样式**：深色帆布、从腰带垂到膝盖、前袋里插两三支苦无柄；酒红色立领短褂的盘扣、腰间忍具包也每帧都在。
- 帧（12 张）：`ToolShopkeeper_00_Idle.png`（基准原样复制）；`_01_Walk`～`_06_Walk`（步行 6 帧循环，手臂反向摆）；`_07_Jump`；`_08_Sit`；`_09_Throw`～`_11_Throw`（蓄力、手臂向前伸直苦无飞出、收手；飞出的苦无可以不连着人物，除此之外不要有任何不连着人物的像素）。
- 规格：1 像素 = 1 屏幕像素，80×80，面朝右，脚底 y=76，Alpha 只有 0/255；头部所有帧像素级和基准一致；`python3 scripts/pixel_noise.py` 每帧孤立像素低于 4%（贴输出）；另交 `torso_check.png`（12 帧脖子到脚并排放大 4 倍，检查围裙和衣服一致）。
- 交付：12 个 PNG；`sheet_preview.png`；`walk.gif`；`heads_8x.png`；`torso_check.png`。
- 交付后接入提交号：待填
