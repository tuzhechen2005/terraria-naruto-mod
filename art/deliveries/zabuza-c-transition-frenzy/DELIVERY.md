status: delivered
request ID: zabuza-c-transition-frenzy

# 交付文件

所有路径相对本交付目录；均为 RGBA PNG。背景 alpha 0，所有可见像素 alpha 255；所有色块在 8×8 屏幕像素格上。

| 文件 | 尺寸 | 左到右姿势顺序 |
| --- | --- | --- |
| `source/Zabuza_Kneel.png` | 1176×608 | 待机尺寸基准、Kneel 1、Kneel 2 |
| `source/Zabuza_Roar.png` | 1632×608 | 待机尺寸基准、Roar 1、Roar 2、Roar 3 |
| `source/Zabuza_FrenzyRoar.png` | 1488×608 | 待机尺寸基准、FrenzyRoar 1、FrenzyRoar 2、FrenzyRoar 3 |
| `source/Zabuza_Throw.png` | 1528×608 | 待机尺寸基准、Throw 1、Throw 2、Throw 3 |
| `source/Zabuza_Unarmed.png` | 2544×608 | 待机尺寸基准、Unarmed 1–6 |
| `source/Zabuza_Catch.png` | 1552×608 | 待机尺寸基准、Catch 1、Catch 2 |
| `source/ZabuzaThrownSword.png` | 352×128 | 独立水平大刀，刀身向右 |

# 提示词摘要

使用内置 `image_gen__imagegen`，以 C 版再不斩样稿为服装、配色、轮廓和像素画标准；参照指定原作图处理露脸、充血眼睛、尖牙、口咬苦无与斩首大刀。分别生成跪地、怒吼、暴走怒吼、投刀、六帧空手奔跑、接刀与水平飞刀。要求透明背景、面向右侧、同一比例、动作分离、泰拉瑞亚式块状硬色阶。随后选取生成结果，统一到待机基准、脚底线与 8×8 整数像素格。

# 已检查

- 逐张查看七张 PNG 的姿势顺序、轮廓、朝向、人物与武器的间距，以及缩小显示时的可读性。
- 逐像素验证所有 PNG 的 alpha 仅为 0 或 255；逐 8×8 块验证 RGBA 完全一致。
- 每张动作图最左侧使用同一张 344×336 的 C 版待机样稿作为尺寸基准，脚底统一在 y=544；姿势已留透明间隔。
- 飞刀图尺寸为 352×128，即 44×16 美术像素；刀柄在左，刀身朝右。

# 仍需游戏侧检查

- 由 Claude 接入、裁帧和设定碰撞盒后，在 tModLoader 中检查 2× 显示尺寸、六帧跑步循环、投刀与接刀时序、飞刀方向及与手部的对位。
- 游戏内确认暴走露脸、口中苦无和大刀在动态背景上的辨识度；本次未运行游戏验收。
