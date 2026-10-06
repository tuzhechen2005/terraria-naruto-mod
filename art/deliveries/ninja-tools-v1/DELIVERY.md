status: delivered
request ID: ninja-tools-v1

# 交付文件

交付根目录：`art/deliveries/ninja-tools-v1/`

| 文件 | 尺寸 | Alpha |
| --- | --- | --- |
| `Shuriken.png` | 28×28 | 仅 0/255 |
| `ShurikenThrown_0.png`、`ShurikenThrown_1.png` | 各 18×18 | 仅 0/255 |
| `ShadowShuriken_0.png`、`ShadowShuriken_1.png` | 各 56×56 | 仅 0/255 |
| `PaperBomb.png` | 20×28 | 仅 0/255 |
| `PaperBombStuck_0.png`、`PaperBombStuck_1.png` | 各 14×20 | 仅 0/255 |
| `StealthBarFrame.png` | 60×10 | 仅 0/255 |
| `StealthBarFill.png` | 52×4 | 全部 255 |
| `preview.png` | 1100×2160 | RGB，不透明 |
| `source/Shuriken-source.png` | 1254×1254 | 含透明和部分透明像素 |
| `source/ShadowShuriken-source.png` | 1254×1254 | 含透明和部分透明像素 |
| `source/PaperBomb-source.png` | 1060×1484 | 含透明和部分透明像素 |
| `source/StealthBarFrame-source.png` | 2172×724 | 含透明和部分透明像素 |

# 图像与制作

使用内置 `image_gen__imagegen` 生成四张透明源图，提示词分别要求：钢灰四角手里剑和中心圆孔；深钢色四片折叠长刃的风魔手里剑；米黄纸条、朱红符文及下端系绳；与现有蓝色雷切条成套的深紫灰潜伏条。以项目的 `TrainingKunai.png`、`ChidoriBarFrame.png`、`StealthBuff.png` 为对应图像风格参考。游戏尺寸图从源图提取轮廓、配色并逐像素修整；旋转帧和闪烁帧在同一套像素风格下制作。`FxChidoriCharge_0.png` 已检查，用于确认特效的高对比亮部风格，但本次素材未加入额外发光。

请求列出的 `ShinobiPrototype/Content/Projectiles/KunaiThrown.png` 在仓库中不存在；未将它视为已检查的图像参考。

潜伏条叠放坐标：框左上角为 `(0,0)` 时，填充左上角放在 `(4,3)`。半满显示填充图左侧 26×4 像素，满值显示完整 52×4 像素。框尺寸 60×10，端头分别位于左右两端。

# 已检查

- 查看了全部四张生成源图与最终 `preview.png`；预览逐项给出 1 倍、3 倍及深浅背景，潜伏条给出空、半、满三态。
- 逐项核对 10 张游戏用 PNG 的尺寸、0/255 Alpha 和每个 2×2 像素块的一致性。
- 核对两组旋转帧及起爆符明灭帧互不相同；目视检查小尺寸轮廓、方向、对比度和符文可读性。

# 待游戏侧检查

- 接入后在 Terraria/tModLoader 中确认背包图标、飞行旋转方向、贴附位置与实际动画帧率。
- 确认潜伏条在 HUD 缩放与不同背景下的叠放位置、裁切宽度和满值亮度。
- 本次仅制作美术交付，未运行模组构建或游戏内验收。
