status: delivered
request ID: npc-hires-sample-v2a

# 交付文件

| 文件 | 尺寸 | Alpha |
| --- | --- | --- |
| `Tazuna_Idle.png` | 80×80 | 仅 0/255，透明背景 |
| `Iruka_Idle.png` | 80×80 | 仅 0/255，透明背景 |
| `Kakashi_Idle.png` | 80×80 | 仅 0/255，透明背景 |
| `preview.png` | 1000×465 | 不透明，1 倍和 3 倍并排比较，含向导 |
| `faces_8x.png` | 928×292 | 不透明，头部 8 倍放大并叠加 1 像素网格 |
| `source/Tazuna_Generated.png` | 1254×1254 | 透明背景，含半透明边缘 |
| `source/Iruka_Generated.png` | 1241×1268 | 透明背景，含半透明边缘 |
| `source/Kakashi_Generated.png` | 1254×1254 | 透明背景，含半透明边缘 |

以上路径均相对于 `art/deliveries/npc-hires-sample-v2a/`。源图由内置 `image_gen__imagegen` 生成，并从其生成目录按文件路径复制到 `source/`。

# 尺寸与视觉要求

| 角色 | 人物身高 | 头高 | 脸部横向绘制范围 | 像素色数 |
| --- | ---: | ---: | ---: | ---: |
| 达兹纳 | 62 px（y=15–76） | 24 px | 约 15 px | 15 |
| 伊鲁卡 | 62 px（y=15–76） | 24 px | 约 15 px | 12 |
| 卡卡西 | 62 px（y=15–76） | 24 px | 约 16 px（含面罩遮挡区） | 11 |

三人均面朝右。达兹纳保留白发、圆框眼镜、白胡子、毛巾与腰间酒瓶；伊鲁卡保留高马尾、正戴护额、鼻梁横疤、笑容、绿色马甲和卷宗；卡卡西保留斜护额、银白刺发、半睁眼、面罩、绿色马甲与橙色小书。源图缩到游戏尺寸后，重新整理头部比例、色块和关键五官，未直接把未经修整的缩图作为成品。

Prompt summary: Generate one transparent, right-facing, full-body pixel-art idle sprite per character, with a warm Terraria-like palette, connected dark outline, flat material shades, oversized readable face, and each character's specified clothing and signature features. Then prepare exact 80×80 game sprites and inspect at 1×, 3×, and face 8× scale.

# 已执行检查

- 逐张查看源图、游戏尺寸预览与 8 倍脸部网格；确认人物朝向、发型、服装与手持物。
- 检查游戏 PNG 均为 80×80，非透明范围顶端 y=15、脚底 y=76，Alpha 只有 0 与 255。
- 检查每人颜色数不超过 24，并运行 `python3 scripts/pixel_noise.py`：

```text
art/deliveries/npc-hires-sample-v2a/Tazuna_Idle.png: 1325 px, 15 colours, 25 isolated (1.9%)
art/deliveries/npc-hires-sample-v2a/Iruka_Idle.png: 1017 px, 12 colours, 25 isolated (2.5%)
art/deliveries/npc-hires-sample-v2a/Kakashi_Idle.png: 1228 px, 11 colours, 24 isolated (2.0%)
```

# 仍需游戏侧检查

- 接入 tModLoader 后在游戏内以 1 倍实际显示，确认三人的脸、疤、眼镜与面罩在不同背景和照明下仍清楚。
- 核对游戏中的站立帧原点、碰撞框、脚底对齐及与原版城镇 NPC 的视觉比例。
- 这次只交付 Idle 样张；全套动作需在样张验收后制作。
