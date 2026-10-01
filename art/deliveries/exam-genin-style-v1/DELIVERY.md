# exam-genin-style-v1 交付

- status: delivered
- request ID: `exam-genin-style-v1`
- 生成方式：内置 `image_gen__imagegen`；原始 PNG 从 `$CODEX_HOME/generated_images` 按文件路径复制到本目录 `source/`。

## 交付文件

以下路径均相对于 `art/deliveries/exam-genin-style-v1/`。

| 文件 | 尺寸 | Alpha |
| --- | --- | --- |
| `RainGenin_Idle.png` | 112×88 | 背景透明；可见像素不透明，Alpha 仅 0/255 |
| `RainGenin_Throw.png` | 112×88 | 背景透明；可见像素不透明，Alpha 仅 0/255 |
| `Candidate_Idle.png` | 112×88 | 背景透明；可见像素不透明，Alpha 仅 0/255 |
| `Candidate_Throw.png` | 112×88 | 背景透明；可见像素不透明，Alpha 仅 0/255 |
| `source/RainGenin_Idle_generated.png` | 1144×1375 | RGBA；原始生成图含局部半透明边缘 |
| `source/RainGenin_Throw_generated.png` | 1407×1118 | RGBA；原始生成图含局部半透明边缘 |
| `source/Candidate_Idle_generated.png` | 1415×1112 | RGBA；原始生成图含局部半透明边缘 |
| `source/Candidate_Throw_generated.png` | 1415×1112 | RGBA；原始生成图含局部半透明边缘 |
| `preview.png` | 520×226 | 不透明 RGB；四个姿势的 1 倍明暗背景预览 |
| `scale-preview-neji-gaara.png` | 520×114 | 不透明 RGB；与宁次、我爱罗同屏的 1 倍比例预览 |

## 提示词摘要

分别生成雨隐下忍撑伞站姿、收伞投出三根千本的姿势，以及通用考生持苦无戒备、投出一把苦无的姿势。指定全部面朝右、透明背景、深色外轮廓、左上光源及限色像素画。雨隐下忍包含深灰兜帽长外衣、带管呼吸面具、四竖线雨隐护额、淡灰纸伞和腰间千本袋；通用考生包含深绿色马甲、简化刻痕护额、深色裤、绑腿和苦无。原始生成图经缩放、限色和二值透明处理成为 2×2 屏幕像素网格的游戏图。

## 已检查

- 目视检查四张原始生成图、1 倍明暗预览和与宁次、我爱罗同屏的比例预览；检查姿势、朝向、纸伞开合、投掷物及角色轮廓。
- 四张游戏 PNG 均为 112×88，人体中线约在 x=56；可见像素的最低行均为 y=83。
- 每个 2×2 屏幕像素块颜色一致；四张游戏图分别有 18、17、18、17 种可见色；Alpha 均仅为 0 或 255。
- 仅在本交付目录下创建文件；未运行模组构建或游戏内验收。

## 待游戏侧检查

- 导入 tModLoader 后核对绘制原点、朝向、地面接触、光照，以及与宁次和我爱罗同屏时的 1 倍可读性。
- 核对雨隐护额四竖线、面具软管、伞骨和千本在实际游戏背景中的辨识度；这些细节在 1 倍尺寸下较细。
- 核对三根千本与苦无作为投掷物时的特效分层、出手时序和碰撞位置；当前交付为关键姿势样张。
