# seal-scrolls-v3

- status: delivered
- request ID: `seal-scrolls-v3`
- 图像工具：内置 `image_gen__imagegen`；分身与千鸟以各自 v2 源图编辑，豪火球沿用 v2 已符合要求的源图。

## 交付文件

| 路径 | 尺寸 | Alpha |
| --- | --- | --- |
| `ScrollClone.png` | 32×32 | RGBA，仅 0/255 |
| `ScrollFireball.png` | 32×32 | RGBA，仅 0/255 |
| `ScrollChidori.png` | 32×32 | RGBA，仅 0/255 |
| `source/ScrollClone.png` | 1254×1254 | RGBA，透明背景，边缘含半透明像素 |
| `source/ScrollFireball.png` | 1254×1254 | RGBA，透明背景，边缘含半透明像素 |
| `source/ScrollChidori.png` | 1254×1254 | RGBA，透明背景，边缘含半透明像素 |
| `preview.png` | 400×795 | RGB，不透明合成预览 |

## 提示词摘要

保留 v2 半展开的斜放卷轴、木轴头、深色外描边、米白纸面与透明背景。分身改为米黄卷边和两个人形的深浅蓝灰实心图案；千鸟保留靛蓝卷边，将纸面碎纹改成单一道粗折线闪电，使用白、浅蓝、深蓝色阶；豪火球的朱红包边与清楚的火纹沿用 v2。

## 已检查

- 目视检查两张生成源图、三张 32×32 游戏图及 `preview.png`；卷轴方向一致，图案在 1 倍和 3 倍预览下可辨，三种卷边颜色可区分。
- 三张游戏图均为 32×32 RGBA；Alpha 只有 0/255；可见轮廓均位于 x=1–30、y=3–28。缩图使用硬边采样，没有半透明边缘。
- `preview.png` 分别把图放在项目 `SealSlotBack.png` 与模拟蓝色物品格上，展示 1 倍及 3 倍效果。
- 未运行模组构建或游戏内验收；本次仅交付美术文件。

## 剩余游戏侧检查

- 开发助手接入三张游戏图后，在真实背包、印位和不同 UI 缩放下检查图案辨识度及颜色对比。蓝色物品格是模拟底图，应以游戏实际界面为准。
