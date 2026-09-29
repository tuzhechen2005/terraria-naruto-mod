# haku-c 交付

- status: delivered
- request ID: haku-c
- 交付目录：`art/deliveries/haku-c/source/`

## 文件

| 文件 | 尺寸（屏幕像素） | 从左到右的内容 |
| --- | ---: | --- |
| `source/Haku_Idle.png` | 1112×368 | v7 待机尺寸基准；低空悬浮待机 1–4 |
| `source/Haku_Move.png` | 1752×368 | v7 待机尺寸基准；前倾平移 1–4 |
| `source/Haku_Throw.png` | 1480×368 | v7 待机尺寸基准；握千本、蓄力、出手、收势。第 3 帧带飞出的千本 |
| `source/Haku_Dash.png` | 1608×368 | v7 待机尺寸基准；低身起冲、水平突进、前伸收招 |
| `source/Haku_Emerge.png` | 1296×424 | v7 待机尺寸基准；冰镜中露半身、上身探出、大半身走出、完全走出。每帧冰镜在左后方 |
| `source/Haku_IceMirror_Whole.png` | 192×288 | 完整冰镜；24×36 美术像素 |
| `source/Haku_IceMirror_Cracked.png` | 192×288 | 开裂冰镜；24×36 美术像素 |
| `source/Haku_IceMirror_Shattered.png` | 192×288 | 碎裂冰镜；24×36 美术像素 |
| `source/Haku_Senbon.png` | 128×24 | 朝右千本；16×3 美术像素 |

全部文件为 RGBA PNG，背景 alpha=0，非透明像素 alpha=255，无半透明像素。每个美术像素为严格对齐的 8×8 屏幕像素块。动作图最左侧 168×304 像素基准图与 `art/deliveries/style-test-v7-haku/source/Haku_style.png` 逐像素相同；各动作姿势依次向右排开。

## 生成提示摘要

使用内置 imagegen，以 v7 白样稿和 C 版再不斩样稿为像素画风参考，生成白的待机、平移、掷千本、冲刺、冰镜现身动作；以原作冰镜画面作造型参考生成完整、开裂和碎裂冰镜，另生成朝右千本。保持白面具的饱和红纹、青绿上衣、橄榄棕袴裙与青绿发簪。将选定的生成 PNG 从 `$CODEX_HOME/generated_images` 复制到本交付目录，再清理零散边缘像素并转成 8×8 对齐的透明源图。

## 已检查

- 目视检查人物朝右、面具红纹、动作顺序、冰镜状态、千本方向及游戏尺度下的轮廓。
- 检查全部 PNG 的尺寸、透明背景、alpha 仅含 0/255、8×8 像素块完全一致。
- 检查五张动作图左侧基准与 v7 样稿逐像素一致，姿势之间留有透明间隔；出手帧的飞针与下一姿势分离。

## 尚需游戏端检查

- Claude 缩放、裁帧与接入后，在 tModLoader 实机确认动画时序、角色受击框、冰镜出场位置、千本速度与弹幕可读性。
- 本交付未运行模组构建或游戏内验收。
