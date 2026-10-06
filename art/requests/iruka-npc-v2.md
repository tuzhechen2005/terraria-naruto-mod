# 素材请求：iruka-npc-v2

- 状态：requested
- 提出者及提交号：Claude Code（`iruka-npc-v1` 用户与 Claude 的反馈：生成原图很好，但采样到游戏尺寸后脸只剩一块橙色，鼻梁疤和眼睛丢了，马尾糊成一团像戴了帽子）
- 资产类型：角色姿态（城镇 NPC 动作组），只修整采样，不重新设计
- **底稿**：`art/deliveries/iruka-npc-v1/source/generated_Iruka_*.png`（生成原图，造型、配色、姿势都以它为准）；格式、尺寸、帧序与 `art/requests/iruka-npc-v1.md` 完全相同（身高 23 美术像素，每美术像素 8×8 屏幕像素，每格 224×200，三张源图 `Iruka_Idle_Walk.png`、`Iruka_Idle_Jump_Sit_Throw.png`、`Iruka_Talk.png`）。
- **对照生成原图逐像素修整**，重点：
  1. 脸：眼睛（深色 1 像素）、肤色两阶、**鼻梁上一道横向的疤**（比肤色深一阶的一横）要读得出来；
  2. 马尾：从护额后上方翘起的一束，轮廓清楚，和头分开，不要糊成一顶帽子；
  3. 护额：蓝布 + 亮色额片；
  4. 清掉身体上零散的杂色单点（马甲、裤子上不该有随机的红点和亮点），色块干净。
- **画风标准仍是达兹纳** `art/deliveries/tazuna-npc-v1/`，并对照已接入的卡卡西 `art/deliveries/kakashi-npc-v4/` 的脸和马甲画法——伊鲁卡要和卡卡西同一精细度。
- 交付：三张源图放 `source/`；附与达兹纳、卡卡西并排的对比预览（4 倍和 1 倍各一）。
- 交付后接入提交号：待填
