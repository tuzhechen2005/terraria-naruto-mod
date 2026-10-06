# 卡卡西走路整体修复 v3

状态：已交付；最终循环采用01/02/04并配合v4、v5，统一接入见 `art/deliveries/npc-direct-pixel-round1/DELIVERY.md`。用户 2026-10-05 实机反馈「走路很怪，上半身下半身分家了，重新优化」。当前开发助手为 Codex；本请求仅交付素材，开发助手负责接入。

先读 `skills/terraria-npc-pixel-art/SKILL.md`、`references/workflow.md`，遵循内置 imagegen。修改范围仅 `art/deliveries/kakashi-walk-repair-v3/`。

## 参考与问题

- 已认可身份与像素外观：`art/deliveries/kakashi-direct-pixel-anim-v2/frames/Kakashi_00_Idle.png`，80×80、可见高62、脚底y75，面朝右、马甲中线约x40。本交付目录 `approved_idle_8x.png` 是同帧机械放大。
- 当前坏循环：`art/deliveries/kakashi-direct-pixel-anim-v2/frames/Kakashi_01_Walk.png` 至 `Kakashi_06_Walk.png`；本交付目录 `before_walk_6x.png` 是对比。
- 旧导出与拼接原因：v1 `export_frames.py` 和 v2 `build_delivery.py`。v2 用另一帧 y38–53 的躯干覆盖目标，再保留目标 y54 以下的腿；未按骨盆对齐，导致上身和下身的轴线、裤腰与体积不一致。单个 alpha 连通不能证明动画合理。

## 必须完成

1. 用已认可站姿作为身份参考，内置 imagegen 重做**完整六帧协调走路**。自然慢步，两腿交替、髋部连接胸腹，肩臂轻摆，脚交替支撑，循环首尾连续；不要超大跑步跨腿或僵硬重复站姿。
2. 头的刺发、斜护额、单只懒眼、眼白/瞳孔、面罩保持已认可身份；若局部复用已认可头部，必须以真实肩颈位置对齐并连到领口，不能从相邻帧硬贴不同骨盆的整截上身。面罩、领口、腰、髋、左右大腿根在所有帧连续，马甲下摆和裤腰没有横向剪切或透明缝；肩宽与躯干体积稳定。
3. 不使用“矩形复制邻帧上身、留下不匹配的腿”的做法。生图重做完整协调姿态；机械导出可裁切、固定比例采样、锚点对齐、镜像，不凭 alpha 连通判断成功。
4. 统一80×80，面朝右，固定身体中心约x40，脚底y75；步态中身体上下起伏至多约1–2像素，保持四肢比例与已认可站姿，不每帧拉成62高。脚尖、头、胳膊不裁切。用统一原图尺度/导出比例，绝不可每帧强制同高。
5. 交付完整14帧 `frames/`，六走路帧命名不变，其余8帧（Idle、Jump、Sit、Throw三帧、Trapped两帧）从v2**字节原样复制**，不得顺便重画。
6. 交付 `walk.gif`（1倍与3倍可并排）、`before_after_walk.gif`、`strip_1x_3x.png`（明暗背景）、`waist_8x.png`（逐帧肩颈至大腿，含改前/后对比）、生成原图、完整提示词、可复现导出脚本/manifest、`DELIVERY.md`。
7. 最终机械检查80×80、0/255alpha、透明RGB归零、脚底/身体轴稳定、无裁切、其余8帧不变。目视看真实尺寸与动画：上身/骨盆连续、两腿相位正确、胳膊与腿对应、首尾无跳变。若不成立继续修，不要把临时试稿写成最终交付。

用户已授权修复现有走路并接入。此请求不改游戏文件、其他请求、Git或交接；无需用户转发。保留已认可Idle，不自动修改其他NPC。
