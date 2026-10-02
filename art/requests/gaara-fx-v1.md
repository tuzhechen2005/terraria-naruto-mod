# 素材请求：gaara-fx-v1

- 状态：requested
- 提出者及提交号：Claude Code（用户 2026-10-02：我爱罗特效换成专门画的，像大蛇丸那样；设计已确认）
- 资产类型：招式特效（全部朝右，代码负责移动、拉伸、翻转）
- 文件：
  - `FxSandWall_0.png`～`_2.png`（各约 40×80）：**沙之守护**——我爱罗身前竖起的一道厚沙墙，3 帧沙粒流动循环（米黄、沙黄、褐色三阶，深褐描边）；
  - `FxSandShuriken_0.png`～`_1.png`（约 16×16）：**砂手里剑**——四角的沙质手里剑，2 帧旋转；
  - `FxQuicksandMark.png`（约 48×12，扁平贴地）：**流沙预警**——地面上一圈翻涌的沙纹；
  - `FxSandPillar_0.png`～`_2.png`（各约 40×96）：**流沙沙柱**——从地面猛地喷出的沙柱，3 帧（升起、顶端炸开、散落）；
  - `FxSandWave_0.png`～`_2.png`（各约 64×40）：**沙浪**——贴地向前翻滚的沙浪，3 帧循环。
- **画风**：以达兹纳 `art/deliveries/tazuna-npc-v1/` 为准，深色外描边、硬色阶、不要渐变和半透明；沙子配色和我爱罗的葫芦一致（参考 `ShinobiPrototype/Content/NPCs/Gaara_Idle_0.png`）。2×2 屏幕像素为一美术像素，透明背景，Alpha 只有 0/255。参考大蛇丸已接入的特效 `ShinobiPrototype/Content/Projectiles/FxWind_0.png` 的质量。
- 流程：生成 → 采样 → 逐像素修整。交付：文件放根下，源图 `source/`，`preview.png` 1 倍与 4 倍、深浅背景。
- 交付后接入提交号：待填
