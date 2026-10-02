# Dosu 补帧交付

- status: delivered
- request ID: `dosu-tween-v1`

## 文件

- 游戏帧：根目录 `Dosu_Idle_0.png`～`Dosu_Idle_5.png`、`Dosu_Walk_0.png`～`Dosu_Walk_7.png`、`Dosu_DrillIn_0.png`、`Dosu_WaveIn_0.png`、`Dosu_SlamIn_0.png`、`Dosu_LeapIn_0.png`（共 18 张）。
- 逐行对照：`preview.png`。每行先放现有站立帧，再放对应新帧；招式行末附原招式首帧。
- 连播条：`playback-idle-walk-idle.png`、`playback-idle-drill-idle.png`、`playback-idle-wave-idle.png`、`playback-idle-slam-idle.png`、`playback-idle-leap-idle.png`。
- 生成源图：`source/Dosu_motion_concept.png`。此图只作动作参考，不应直接作为游戏帧接入。

## 尺寸与透明度

- 所有 18 张游戏帧：112×88 PNG，RGBA；Alpha 仅有 0 和 255，脚底边界均为 `y=84`，朝右。
- `preview.png`：3124×1786，RGB。
- 走路连播条：1120×88，RGBA；其余 4 条：448×88，RGBA。
- 生成源图：1536×1024，RGBA，含部分透明像素和非游戏背景色；仅供参考。

## 提示词与制作

内置 `image_gen__imagegen` 参照现有多斯站立、走路帧和达兹纳画风对照图，生成透明底的走路、呼吸及四种招式过渡姿势源图。游戏帧从项目现有多斯 PNG 制作：待机与四张过渡帧取自站立帧并作轻微像素位移；走路保留现有四张关键姿势，并为每张增加一张小幅位移帧。生成源图没有直接缩小为游戏帧，以保留现有角色的脸、颜色和细节。

## 已检查

- 检查了生成源图，以及所有动作与站立帧并排的 `preview.png`；在放大和游戏原尺寸下查看轮廓、朝向与可辨识度。
- 用脚本核对 18 张游戏帧的数量、尺寸、Alpha、脚底 `y=84`，并确认所有不透明 RGB 值均可在本请求所指的现有多斯帧中找到。
- 对照连播条检查待机、走路和四种招式的顺序。第一次插值版本出现面部模糊，已用保留原像素的位移帧替换。

## 仍需游戏端检查

- 接入后按实际播放速度检验走路循环及站立切换时是否仍有跳动，并决定是否进一步手绘关键姿势。
- 四张招式过渡帧目前是保留站立形象的轻微预备动作；需在游戏中检验是否足够清晰，特别是手臂动作与现有招式首帧的衔接。
- 走路关键帧来自现有 `Dosu_Walk_0`～`_3`，因此未逐帧从站立底稿重新绘制。若严格执行该制作要求，仍需人工逐像素重绘走路关键帧。
- 未运行模组构建或游戏内验收；本次只交付美术文件。
