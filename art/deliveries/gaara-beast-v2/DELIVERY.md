# gaara-beast-v2 交付

- status: delivered
- request ID: `gaara-beast-v2`

## 交付文件

- 游戏帧：`Gaara_Beast_Idle_0.png`～`Gaara_Beast_Idle_3.png`、`Gaara_Beast_ArmIn_0.png`、`Gaara_Beast_Arm_0.png`～`Gaara_Beast_Arm_2.png`、`Gaara_Beast_BulletIn_0.png`、`Gaara_Beast_Bullet_0.png`～`Gaara_Beast_Bullet_2.png`。共 12 张，均在本目录根下；每张 176×104 RGBA，透明背景，Alpha 仅 0/255。
- 源图：`source/generated_idle.png`（1632×964）、`source/generated_windup.png`（1520×1035）、`source/generated_sweep.png`（1774×887）、`source/generated_breath.png`（1632×964）。均为 RGBA 透明源图，生成边缘含半透明像素。
- `preview.png`：2880×2280 RGB；12 张游戏帧、新底稿和一阶段站立帧的 1 倍、4 倍并排对照。

## 提示词概要

使用内置 `image_gen__imagegen`，以 `gaara-base-v2/Gaara_Idle_0.png` 锁定人身的红发、绿松石色眼睛、额头红记、红褐长衣、白色斜带和双节葫芦；以现有 `Gaara_Beast_Idle_0.png`、`Gaara_Beast_Arm_0.png`、`Gaara_Beast_Arm_1.png`、`Gaara_Beast_Bullet_0.png` 分别参考守鹤化造型及动作；以 `Tazuna.png` 为唯一画风参考。生成待机、后蓄力、横扫和张口蓄气四张透明源图，再从这些图制作连续帧。要求面朝右、沙质巨臂和尾巴、蓝色咒纹、深色连贯描边、左上受光、硬色阶、无弹体。

## 已检查

- 对照指定参考图及预览图检查人物朝右，红发、葫芦、巨臂黑爪、沙尾和蓝色咒纹在游戏尺寸下可辨认。
- 12 张帧均为 176×104；人体约在 x=64，脚底最低像素均在 y=99；无图像越界。
- 每张帧的 Alpha 仅 0/255，所有 2×2 屏幕像素块内颜色完全一致；不透明像素各自形成一个四向连通主体，没有孤立像素。
- 12 张帧均经同一 64 色调色板处理，并检查各组相邻帧存在动作变化。游戏帧只有硬色块；源图保留生成时的抗锯齿边缘。

## 尚需游戏侧检查

- 接入后在 Terraria 内以 1.5 倍显示检查与一、二阶段的身高和色调衔接、绘制锚点、碰撞盒及左右翻转。
- 实机播放待机、巨臂横扫和沙尘吐息动画，确认节奏和状态切换；本交付未运行模组构建或游戏内验收。
