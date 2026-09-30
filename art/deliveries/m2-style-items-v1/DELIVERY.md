# m2-style-items-v1 交付

- status: delivered
- request ID: `m2-style-items-v1`
- 生成方式：内置 `image_gen__imagegen`；未使用 API 密钥或图像 CLI。

## 交付文件

| 成品（交付目录根下） | 尺寸 | Alpha | 源图（`source/` 下） | 源图尺寸 |
| --- | --- | --- | --- | --- |
| `SharinganCore1.png` | 32×32 | 透明；仅 0/255 | `SharinganCore1_source.png` | 1237×1271 |
| `EightGatesCore.png` | 32×32 | 透明；仅 0/255 | `EightGatesCore_source.png` | 1334×1179 |
| `ByakuganCore.png` | 32×32 | 透明；仅 0/255 | `ByakuganCore_source.png` | 1254×1254 |
| `TrainingWraps.png` | 32×32 | 透明；仅 0/255 | `TrainingWraps_source.png` | 1254×1254 |
| `LeeLegWeights.png` | 32×32 | 透明；仅 0/255 | `LeeLegWeights_source.png` | 1536×1024 |
| `preview.png` | 736×304 | 不透明 | — | — |

源图均为带透明通道的 PNG，含生成时产生的部分透明像素；游戏尺寸成品已清除这些像素。`preview.png` 为五图并排、深浅背景各一行、放大 4 倍的对照。

## 提示词摘要

以泰拉瑞亚物品栏像素图标为目标，逐个生成：软木塞玻璃试管内的淡绿液体和单勾玉红色写轮眼；绿色封皮卷轴及淡绿查克拉气焰；深蓝底板上的无瞳孔淡紫白眼和青筋；末端松开的白色绷带拳套；成对橙色腿部负重袋及灰色扣带。共同要求为透明背景、深色轮廓、硬色阶、居中紧凑、32 像素可读，无文字或额外物体。

## 已检查

- 查看了五张源图及 4 倍预览，检查物体方向、成对数量、眼睛差异和游戏尺寸轮廓。
- 五张成品均为 32×32 PNG，alpha 仅有 0 与 255，无半透明杂边；每张颜色数不超过 24。
- 在预览的深色和浅色背景上检查辨识度。

## 尚需游戏侧检查

- 接入对应物品后，在 tModLoader 实际物品栏中以 1 倍尺寸确认辨识度、图标方向和缩放效果。
- 本次仅交付美术文件，未运行构建或游戏内验收。
