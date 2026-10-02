# orochimaru-fx-seal-v1 交付

- status: delivered
- request ID: `orochimaru-fx-seal-v1`

## 交付文件

| 路径（相对本目录） | 尺寸 | Alpha |
| --- | ---: | --- |
| `FxSealFlame_0.png` | 24×20 | RGBA，仅 0/255 |
| `FxSealFlame_1.png` | 24×20 | RGBA，仅 0/255 |
| `FxSealFlame_2.png` | 24×20 | RGBA，仅 0/255 |
| `FxSealGlyph.png` | 48×48 | RGBA，仅 0/255 |
| `FxSummonCircle.png` | 128×32 | RGBA，仅 0/255 |
| `preview.png` | 1120×640 | RGB，不适用；深浅背景、1 倍与 4 倍展示 |
| `source/Flame_source.png` | 2172×724 | RGBA，含半透明原始边缘 |
| `source/SealGlyph_source.png` | 1254×1254 | RGBA，含半透明原始边缘 |
| `source/SummonCircle_source.png` | 2172×724 | RGBA，含半透明原始边缘 |

## 提示词摘要与处理

使用内置 `image_gen__imagegen` 生成透明背景源图：三帧五指紫焰（近白焰心）、中心螺旋与五处符文块的封印圈，以及低角度、黑紫线条和放射纹的椭圆通灵圈。要求硬边像素、深描边、有限紫色阶和少量金色。将源图采样到逻辑像素网格、限制调色板并清理 Alpha。封印圈依据源图的螺旋和五符文构图逐像素重绘，以保证 48×48 下可辨认。

## 已检查

- 逐张目视检查源图、游戏尺寸图与 `preview.png`，核对五火苗、螺旋、五符文、椭圆方向和放射线。
- 程序检查五张游戏图的尺寸、Alpha 值仅为 0/255、各 2×2 屏幕像素块颜色一致。
- 在深色与浅色背景下检查 1 倍及最近邻 4 倍预览。

## 待游戏侧检查

- 接入后确认五火苗相对指尖的位置及三帧循环速度。
- 确认封印圈在玩家身上的大小、短暂亮起与淡出效果，以及查克拉停止恢复的时序。
- 确认通灵圈在地面透视、位置、缩放和万蛇召唤前的显示时长。
- 尚未进行模组构建或游戏内验收。
