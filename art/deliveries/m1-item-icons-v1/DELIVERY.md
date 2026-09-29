# m1-item-icons-v1 交付

- status: delivered
- request ID: `m1-item-icons-v1`

## 文件

| 用途 | 路径 | 尺寸 | Alpha |
| --- | --- | --- | --- |
| 游戏图标：忍者手册 | `art/deliveries/m1-item-icons-v1/NinjaHandbook.png` | 28×30 | RGBA；仅 0/255 |
| 游戏图标：达兹纳的施工图 | `art/deliveries/m1-item-icons-v1/BridgeBlueprint.png` | 32×28 | RGBA；仅 0/255 |
| ImageGen 源图：忍者手册 | `art/deliveries/m1-item-icons-v1/source/NinjaHandbook.png` | 1254×1254 | RGBA；含边缘半透明像素 |
| ImageGen 源图：施工图 | `art/deliveries/m1-item-icons-v1/source/BridgeBlueprint.png` | 1659×948 | RGBA；含边缘半透明像素 |

## 提示词摘要

使用内置 `image_gen__imagegen` 分别生成透明背景的像素画源图：深绿封面、右侧四眼线装和木叶标志竖签的合本；蓝色半展开施工图、白色拱桥侧视线图和右端麻绳。两者均要求饱和硬色阶、同色相深色描边、块状像素和无写实光影。游戏尺寸图根据源图缩制；施工图在 16×14 美术像素网格上简化桥拱与卷边，以保证缩小后可读。

## 已检查

- 查看了请求指定的两张角色画风图和两张现有物品图，以及生成源图与最终图。
- 核对了两张最终图的尺寸、透明范围、仅 0/255 的 alpha，及严格的 2×2 像素块。
- 在游戏尺寸下查看轮廓：手册保持书本轮廓与右侧装订；施工图保留蓝纸、白色桥拱和右端卷边麻绳。

## 尚需游戏侧检查

- 接入物品贴图后，在物品栏、掉落物和实际游戏缩放下确认辨识度、颜色及方向；本次未运行模组或游戏内验收。
