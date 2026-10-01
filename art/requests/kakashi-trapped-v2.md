# 素材请求：kakashi-trapped-v2

- 状态：requested
- 提出者及提交号：Claude Code，NPC 画风统一后续（卡卡西已换成 `kakashi-npc-v4`）
- 资产类型：角色姿态（2 帧）
- 游戏用途：湖边初遇时卡卡西被再不斩的水牢术困在水球里挣扎，画在水球贴图后面（`WaterPrison.DrawTrappedKakashi`，两帧交替）。旧的 `water-prison-v1` 被困帧是老卡卡西画风，已从游戏里撤下。
- **角色与画风**：与 `art/deliveries/kakashi-npc-v4/` 的卡卡西完全一致（同一套颜色、描边、体型、23 美术像素身高），画风标准是达兹纳 `art/deliveries/tazuna-npc-v1/`。
- 姿势：悬在水中、身体微微蜷起、一手抬起像要推开水壁、另一手护在身前，头发被水流带得稍微飘起；两帧之间只有手臂和头发有 1–2 美术像素的差别（缓慢挣扎）。
- 尺寸与格式：两张 `Kakashi_Trapped_0.png`、`Kakashi_Trapped_1.png`，每张 56×56，2×2 屏幕像素为一美术像素，人物居中，面朝右，背景透明，Alpha 只有 0/255。源图放 `source/`。
- 交付：DELIVERY.md；附一张与 `kakashi-npc-v4` 的 Idle 并排的 1 倍、4 倍预览。
- 需要避免：和 v4 卡卡西配色、体型不一致；半透明杂边；零散噪点。
- 交付后接入提交号：待填
