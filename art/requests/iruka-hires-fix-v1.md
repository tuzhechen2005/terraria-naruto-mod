# 素材请求：iruka-hires-fix-v1

- 状态：通过（2026-10-05，已接入）
- 提出者及提交号：Claude Code（夜间 NPC 外貌任务，逐像素验收 `art/deliveries/iruka-hires-full-v1/`）
- 验收结果：12 帧里 11 帧通过（头部一致、走路摆臂、跳跃、坐着看卷宗都很好）。**只有 `Iruka_10_Throw.png` 不合格**：一根深蓝色的横棍从胸口穿过身体，看不出是手臂。
- 要做：只重画 `Iruka_10_Throw.png`（甩出的瞬间）：右臂（靠前的手臂）**从右肩伸出**，向右前方完全伸直，深蓝袖子 + 肤色的手，手刚松开，一支苦无在手前方 2～4 像素处飞出（灰色刀身、深色握柄）；左臂向后自然摆开；身体略前倾，重心在前脚。头部与 `Iruka_09_Throw.png`、`Iruka_11_Throw.png` 完全相同（像素级），配色、尺寸、脚底 y=76 相同。参照 `art/deliveries/iruka-hires-full-v1/` 的 09 和 11 两帧（必须作为图像参考），10 要在它们之间自然过渡。
- 规格：80×80，面朝右，Alpha 只有 0/255，`python3 scripts/pixel_noise.py` 孤立像素低于 4%。
- 交付：`Iruka_10_Throw.png` 放根下；`throw_preview.png`：09、10、11 三帧并排 1 倍和 4 倍。
- 交付后接入提交号：待填
