# 素材请求：orochimaru-set-v5b

- 状态：requested
- 提出者及提交号：Claude Code（用户 2026-10-02：大蛇丸全套从新底稿改出）
- 资产类型：Boss 全套之二——招式帧与引帧（`<动作>In_0` 是招式开始前 0.1 秒的过渡帧，从站立姿势到招式第一帧之间）
- **底稿（必须作为图像参考）**：`art/deliveries/orochimaru-base-v5/Orochimaru_Idle_0.png`。招式姿势参考旧帧 `ShinobiPrototype/Content/NPCs/Orochimaru_{Hands,Dash,Wind,Neck,Seal,Summon}_*.png`（只参考姿势）。
- **底稿上统一要做的一处修改（所有帧都按这个画）**：下巴处那个粗 L 形黑块去掉，下颌只留普通的深色外描边；嘴改成 1 美术像素高、约 3 美术像素长的细线，靠脸前侧，最右端向上翘 1 像素（阴笑）。其他地方不改。
- **画风唯一标准：达兹纳 `art/deliveries/tazuna-npc-v1/`**（游戏里 `ShinobiPrototype/Content/NPCs/Tazuna.png`）：深色外描边连贯、每种材质 3～4 阶硬色阶、左上受光、颜色饱和、没有零散噪点、**不要写实的细碎渐变**，也不要只有十几种颜色的粗平涂。
- **参照原图**：生成时必须把下面列出的图作为图像参考输入给图像生成工具，保留角色的造型、配色和辨识特征；生成后对照底稿逐像素修整（去掉孤立的单个像素、补齐断开的描边），**每一帧都要和底稿像同一个人**：身高、头身比、配色、描边粗细、色阶数量都照底稿。
- 尺寸格式：人物帧每张 112×88，2×2 屏幕像素为一美术像素，人体中线 x=56，脚底最低行 y=83，面朝右，透明背景，Alpha 只有 0/255。游戏里放大 1.5 倍。
- 文件：
  - `Orochimaru_HandsIn_0`、`Orochimaru_Hands_0`～`_2`：潜影蛇手——双袖向前甩出，袖口张开（蛇本身另画，不要画进来）；
  - `Orochimaru_DashIn_0`、`Orochimaru_Dash_0`～`_1`：贴地蛇行冲刺，身体压得很低、几乎平贴；
  - `Orochimaru_WindIn_0`、`Orochimaru_Wind_0`～`_1`：风遁·大突破——吸气后仰，再向前猛吐（风另画）；也用于吐毒和吐草薙剑；
  - `Orochimaru_NeckIn_0`、`Orochimaru_Neck_0`：伸颈——身体站定前倾，**脖子以上不画**（头和脖子由 NeckHead/NeckSegment 拼接，从领口处接出）；
  - `Orochimaru_SealIn_0`、`Orochimaru_Seal_0`～`_1`：五行封印——一手五指张开向前推出，指尖位置留出来放紫色火焰；
  - `Orochimaru_SummonIn_0`、`Orochimaru_Summon_0`～`_1`：通灵之术——咬破拇指后一手按向地面，半蹲。
- 交付：全部放根下，源图 `source/`；`preview.png` 所有帧与底稿、达兹纳并排，1 倍和 4 倍。
- 交付后接入提交号：待填
