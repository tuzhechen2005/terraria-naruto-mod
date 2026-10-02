# gaara-fx-v1 交付

- status: delivered
- request ID: gaara-fx-v1
- 制作方式：内置 `image_gen__imagegen` 生成五类源图；取样、硬色阶量化、2×2 像素化及逐像素修整后导出游戏尺寸 PNG。

## 交付文件

| 路径（相对本目录） | 尺寸 | Alpha |
|---|---:|---|
| `FxSandWall_0.png`、`FxSandWall_1.png`、`FxSandWall_2.png` | 每张 40×80 | 仅 0/255 |
| `FxSandShuriken_0.png`、`FxSandShuriken_1.png` | 每张 16×16 | 仅 0/255 |
| `FxQuicksandMark.png` | 48×12 | 仅 0/255 |
| `FxSandPillar_0.png`、`FxSandPillar_1.png`、`FxSandPillar_2.png` | 每张 40×96 | 仅 0/255 |
| `FxSandWave_0.png`、`FxSandWave_1.png`、`FxSandWave_2.png` | 每张 64×40 | 仅 0/255 |
| `source/SandWall_generated.png` | 1536×1024 | 有透明通道，含半透明像素 |
| `source/SandShuriken_generated.png` | 1774×887 | 有透明通道，含半透明像素 |
| `source/QuicksandMark_generated.png` | 2172×724 | 有透明通道，含半透明像素 |
| `source/SandPillar_generated.png` | 1536×1024 | 有透明通道，含半透明像素 |
| `source/SandWave_generated.png` | 1586×992 | 有透明通道，含半透明像素 |
| `preview.png` | 1860×1580 | RGB；深浅底色、1 倍和 4 倍预览 |
| `process.py` | — | 可重复取样与导出的处理脚本 |

## 提示词摘要

五类特效分别按竖直沙墙（3 帧）、四角砂手里剑（2 帧旋转）、贴地流沙环、喷发／炸开／散落沙柱（3 帧）和向右翻滚沙浪（3 帧）生成。均要求透明背景、深褐描边、我爱罗葫芦的暖沙配色、硬色阶和无渐变的像素画。游戏尺寸色板参考 `Gaara_Idle_0.png` 的葫芦与沙色。源图保留在 `source/`。

## 已检查

- 逐张核对 12 张游戏 PNG 的文件尺寸、RGBA、Alpha 仅 0/255、2×2 同色像素块；最多五个不透明颜色。
- 查看全部帧在深浅底色的 1 倍及 4 倍预览，核对右向沙浪、沙柱阶段、沙墙密度和旋转手里剑的轮廓。
- 流沙预警在 24×6 美术像素上逐像素修整为可辨识的扁平沙环。

## 仍需游戏侧检查

- 接入后的锚点、贴地位置、翻转及拉伸行为。
- 动画播放速度、碰撞盒与特效可读性；战斗场景中深浅背景对比。
- 模组构建和游戏内验收未运行；本次只交付素材，未改源码。
