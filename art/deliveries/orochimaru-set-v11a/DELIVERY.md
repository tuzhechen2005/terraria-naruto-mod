status: delivered
request ID: orochimaru-set-v11a

# 交付文件

- `art/deliveries/orochimaru-set-v11a/Orochimaru_Idle_0.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Idle_1.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Idle_2.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Idle_3.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Idle_4.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Idle_5.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Walk_0.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Walk_1.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Walk_2.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Walk_3.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Walk_4.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Walk_5.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Walk_6.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Walk_7.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Hurt_0.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Reveal_0.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Reveal_1.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Reveal_2.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Emerge_0.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Emerge_1.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Emerge_2.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Sink_0.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Sink_1.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Sink_2.png` — 224×136, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_NeckHead.png` — 72×60, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_NeckSegment.png` — 24×24, RGBA, Alpha 仅 0/255
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Head_Boss.png` — 30×30, RGBA, Alpha 仅 0/255

## 源图与检查图

- `art/deliveries/orochimaru-set-v11a/source/Orochimaru_generated_pose_sheet.png` — 2043×770，带透明通道的图像生成姿势参考；不直接供游戏使用。
- `art/deliveries/orochimaru-set-v11a/source/base_detail_6x.png` — v11 底稿局部的 6 倍检查图。
- `art/deliveries/orochimaru-set-v11a/source/build_frames.py` — 从 v11 底稿制作游戏尺寸帧及预览的可复现脚本。
- `art/deliveries/orochimaru-set-v11a/preview.png` — 2832×4068，所有交付帧与 v11 底稿并排，包含 1 倍和 3 倍视图。
- `art/deliveries/orochimaru-set-v11a/Orochimaru_Idle_overlay.png`、`art/deliveries/orochimaru-set-v11a/Orochimaru_Walk_overlay.png` — 224×136 半透明叠图，仅供检查。

## 提示词摘要

使用内置 `image_gen__imagegen`，以 v11 底稿为身份和画风参考，生成透明背景的像素风动作姿势条：轻微呼吸、阴冷滑步、后仰受击、舔唇现身；保持黑色长发、浅色衣服、紫色绳结和金色眼睛。最终游戏帧以底稿原始像素为基础制作，姿势条只作动作参考。

## 已检查

- 逐项查看生成姿势图和最终预览，检查人物轮廓、朝右方向、脸眼可见性和 1 倍游戏尺寸可读性。
- `Idle_0` 与 v11 底稿文件 SHA-256 完全一致；其余帧沿用底稿头脸像素并进行平移。
- 27 张游戏 PNG 均为所需尺寸，Alpha 只含 0 和 255；人物帧 224×136，伸颈头 72×60，伸颈节 24×24，头像 30×30。
- 6 张呼吸帧和 8 张走路帧顶端均为 y=28、脚底均为 y=131；已输出叠图检查高度。
- 伸颈节左右端 24 行像素完全一致，可水平无缝重复。

## 尚需游戏内检查

- 在 tModLoader 中检查全部帧的实际播放速度、滑步幅度和动作转换。
- 检查出场/遁地动画与地面坐标、碰撞箱的对齐。
- 检查伸颈头与脖子节的游戏内拼接及地图头像显示。
