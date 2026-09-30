# neji-style-v1 交付

- status: delivered
- request ID: `neji-style-v1`
- 生成方式：内置 `image_gen__imagegen`，四个姿势各一次生成；从 `$CODEX_HOME/generated_images` 复制原始 PNG 到本目录 `source/`。

## 文件

| 文件 | 尺寸 | Alpha |
| --- | --- | --- |
| `Neji_Idle.png` | 112×88 | 二值透明；可见像素全为 255 |
| `Neji_Palm.png` | 112×88 | 二值透明；可见像素全为 255 |
| `Neji_Rotation.png` | 112×88 | 二值透明；可见像素全为 255 |
| `Neji_SixtyFour.png` | 112×88 | 二值透明；可见像素全为 255 |
| `preview.png` | 516×244 | 不透明；明暗底色 1 倍预览 |
| `scale-preview-haku-gaara.png` | 352×122 | 不透明；与白、我爱罗 v1 同屏比例预览 |
| `source/Neji_Idle_generated.png` | 1415×1112 | 含透明与局部半透明边缘的原始生成图 |
| `source/Neji_Palm_generated.png` | 1415×1112 | 含透明与局部半透明边缘的原始生成图 |
| `source/Neji_Rotation_generated.png` | 1415×1112 | 含透明与局部半透明边缘的原始生成图 |
| `source/Neji_SixtyFour_generated.png` | 1415×1112 | 含透明与局部半透明边缘的原始生成图 |

## 提示词摘要

以 `gaara-style-v1/Gaara_Idle.png`、`haku-base-v4/Haku_Idle_0.png` 为画风和尺寸参考；生成中忍考试时期的日向宁次。每帧保留长棕发、护额、浅紫白眼、米白高领开衩上衣、深棕短裤、右臂及右大腿绷带、忍具袋与凉鞋。分别表现柔拳站姿、带淡蓝查克拉的掌击、回天圆形查克拉环、八卦六十四掌蹲姿及脚下阵圈。要求面朝右、透明背景、硬边限色像素画。

## 已检查

- 目视检查四个原始生成图和最终 1 倍明暗背景预览；确认姿势、朝向、主要装束及白眼特征。
- 四张游戏 PNG 均为 112×88；透明像素 Alpha 0，可见像素 Alpha 255；各约 18–20 种可见色。
- 最终 PNG 每个 2×2 屏幕像素块颜色相同；人物脚部最下方位于 y=83（接触线 y=84）。旋转及阵圈效果也留在画布内。
- 与白、我爱罗 v1 同屏核对角色比例。未执行模组构建或游戏内验收。

## 待游戏侧检查

- 接入 Boss 帧并在游戏中核对绘制原点、朝向、实际光照与 1 倍画面可读性。
- 检查回天及八卦阵圈是否与碰撞框、攻击时序和地面高度协调；如需动态动画，当前四张仅为关键姿势样张。
