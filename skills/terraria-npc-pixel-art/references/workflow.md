# 生成与最终尺寸导出

## 新角色提示词骨架

使用内置图像生成工具；下面各字段按角色替换，不能把示例特征复制给所有 NPC。

```text
Asset: one low-resolution pixel-art town NPC idle sprite for a Terraria Naruto mod.
Character: [身份、年龄、服装与需要保留的轮廓特征].
Reference 1: approved Iruka enlarged game frame, STYLE ONLY: warm colors,
substantial broad color masses, clear silhouette, readable face at tiny size.
Reference 2: [角色参考], IDENTITY ONLY, not resolution or rendering style.
Full body, [朝向], relaxed idle stance, transparent background.
Design deliberately as a tiny sprite with about 24–32 coarse rows of visual
color blocks, not an intricate illustration to shrink later. Preserve bold
hair/hat/mask silhouette and broad clothing panels. Few large shadows,
warm upper-left light. No fabric micro-texture, tiny pocket details or dithering.
For an exposed face: preserve THIS character's own eye size, shape, aspect
ratio and expression from the identity reference. Do not copy Iruka's eye
dimensions or impose a minimum eye height. Keep the eyes readable at the
final screen size using appropriate contrast and spacing; omit the mouth line.
Leave skin space
between the eye and [角色确有的特征]. Do not add Iruka's scar to this character.
No text, scenery, ground shadow or extra accessories.
```

“24–32 粗色块行”是生成设计目标，不是机械验收项。最后输出是 62 像素高、保留较大色块及必要细节的纹理。不要为了达到提示词中的行数强行缩成一套统一网格。每种体型都应在正常尺寸下看是否好看。

## 局部修改提示词

```text
Precise local edit of the attached APPROVED sprite. Change only [明确变化].
Keep the face width, expression, pose, silhouette, body, clothing, palette,
position, canvas and all other features unchanged. Preserve transparency.
Keep no mouth line. Do not redraw the whole character or add accessories.
```

对伊鲁卡，本轮用户让眼睛竖向占两格，实际输出为 4 屏幕像素高；眼睛下方一行肤色，疤略下移。模型只加高了部分眼睛，最后把模型生成的**局部眼睛区域**按垂直方向适配到该角色定稿所需高度。这个数字仅记录伊鲁卡案例，**不能复用到其他角色**。用户明确要求每个人保留不同的眼睛大小；其他人先看各自的角色参考，不预设统一眼型或尺寸。局部合成前先核对源图尺寸与坐标；位置变化的生图结果不能直接套旧坐标。

## 通用导出工具

运行 `node scripts/export_sprite.cjs --help` 查看参数。需要 Node.js 与 `pngjs`；Codex 桌面可调用 workspace dependencies 工具定位运行时，将返回的包目录设置为 `NODE_PATH`。不要硬编码当前机器的运行时缓存路径到 skill。

示例路径均相对于本 skill 根目录，输出目录由当前任务选择：

```sh
node scripts/export_sprite.cjs --input assets/approved-iruka-source.png --out /absolute/task/output --prefix Iruka --canvas 64x80 --height 62 --baseline 76 --pixel 1 --phase-x 0.8 --phase-y 0.5
```

这个示例可以逐像素复现 `assets/approved-iruka-right.png`。相位 0.8 是伊鲁卡实测值，别的角色先用默认 0.5，看眼睛是否被漏采样后再调整。

- `--height` 是导出身体高度，不是整张画布高；`--baseline` 是脚底下一行（76 表示最低脚像素 y=75）。
- `--pixel 1` 导出到最终屏幕纹理尺寸，仍可有大色块；`--pixel 2` 强制每格 2×2，可能损伤五官，只在该角色实际适合时使用。
- `--bbox x0,y0,x1,y1` 可指定源图包围盒，坐标上界不包含；不指定时按 alpha 阈值 128 找包围盒。
- 动画用站立帧确定比例，再给后续帧使用同一 `--scale`。例如伊鲁卡比例为 62/1372；不同姿势的高度应变化，不能每帧都重新设置 `--height 62`。
- 采样会硬化透明，删除晕边；默认保留源色，不限色、不抖动，不因色数高自动打回。
- 输出 Right/Left 的命名由 `--facing` 声明源图方向；工具不识别角色朝向，必须目视确认。
- 同名结果已存在时默认停止；本次任务确实要更新这些导出文件时使用 `--overwrite`。输出绝不能指向原始输入文件。

工具输出实际帧、另一朝向、整数 6 倍检查图及 manifest。用实际小帧生成比较页面，CSS `image-rendering: pixelated`，1 倍行不要以高分辨率源图代替。

## 本案例事实与边界

- 2026-10-05 用户认可的是最后的无嘴线、加高眼睛版本，附在 assets 中。
- 已被否决的 23 格旧格式把眼睛漏采样；强制 31 格把眼睛拉成白柱。成品保留原设计色块，按 62 像素高度一次导出，并局部改善脸部读法。
- 模型输出有晕边、混合块大小与同色块内近似色，最终帧有数百个 RGB 值；这些数量不决定是否好看。
- PNG 尺寸、alpha、人物高/脚底、镜像、眼睛高度与局部修改范围已检查；完整动画、模组接入与实机尚未完成。不要把本样张的认可扩展成所有 NPC 已通过。
