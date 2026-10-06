# 素材请求：iruka-npc-v3

- 状态：requested
- 提出者及提交号：Claude Code（`iruka-npc-v1`、`v2` 反馈：采样后脸糊、头发里有蓝色杂点、全身零散杂色，和卡卡西相比明显粗糙）
- 资产类型：角色姿态（城镇 NPC 动作组）
- **换思路：以已接入的卡卡西为底稿逐帧改**。底稿：`art/deliveries/kakashi-npc-v4/` 的最终源图（与游戏里 `ShinobiPrototype/Content/NPCs/Kakashi.png` 同一套，必须作为图像参考）。两人都穿绿色中忍马甲、深蓝忍装，身体、马甲、裤子、鞋、动作**直接沿用卡卡西的像素**，只改：
  1. 头发：卡卡西的白色竖发 → **黑色头发，脑后扎一个向上翘的马尾**（2～3 美术像素的一束，和头分开、轮廓清楚）；
  2. 脸：去掉卡卡西的面罩和遮眼护额 → **露出整张脸**：两只眼睛（深色 1 像素）、两阶肤色、**鼻梁上一道横向的疤**（比肤色深一阶）；
  3. 护额：正戴在额头上（不遮眼），蓝布 + 亮色额片。
  4. 配色只能来自卡卡西底稿的调色板 + 黑发、肤色、疤这几种颜色；不许有零散单点。
- 帧序与 `art/requests/iruka-npc-v1.md` 相同（`Iruka_Idle_Walk.png`：Idle + Walk 6；`Iruka_Idle_Jump_Sit_Throw.png`：Idle + Jump + Sit + Throw 3；`Iruka_Talk.png`：Idle + Talk 2）。卡卡西没有的姿势（Sit 读卷宗、Talk）参照 `art/deliveries/iruka-npc-v2/source/` 的姿势、用卡卡西的画法补出。格式同达兹纳：身高 23 美术像素，每美术像素 8×8 屏幕像素，每格 224×200，Alpha 只有 0/255。
- 交付：源图 `source/`；与卡卡西、达兹纳并排的对比预览（4 倍和 1 倍）。
- 交付后接入提交号：待填
