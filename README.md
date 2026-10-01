# Shinobi Prototype · 泰拉瑞亚火影模组

一个以《火影忍者》第一部为蓝本的 [tModLoader](https://github.com/tModLoader/tModLoader) 模组：保留原版泰拉瑞亚的全部 Boss 与流程，把忍者的主线剧情、Boss 战和村子嵌进去，让玩家边打原版边经历波之国任务和中忍考试。

> A Naruto-inspired content mod for Terraria (tModLoader). Fan project in active development — Chinese-first; an English summary is at the end.

**开发状态**：开发版 0.4.x。波之国篇已在游戏内跑通；中忍考试篇代码完成、正在逐段实机打磨，部分美术仍是占位图。

---

## 内容一览

### 波之国篇（克苏鲁之眼前后）

- **任务链**：卡卡西带队 → 去海边桥头找造桥工达兹纳 → 海雾里遭鬼之兄弟伏击 → 达兹纳坦白真相，任务升为 A 级 → 湖边遭遇再不斩的水牢术，从外面打破水牢救出卡卡西 → 断桥雾中的再不斩与白。
- **再不斩与白双首领战**：克眼式冲刺、雾隐阶段、冰镜牢笼、千杀水翔、白先倒下时再不斩暴走（绷带脱落、口咬苦无）。
- **尾声**：最后倒下的人起身走到对方身边，说完最后的话后倒下，雪落下来。
- 海边会生成一座没修完的石桥（桥头小屋、起重机、海雾），波之国的主线围绕它展开。

### 中忍考试篇（世界吞噬者 / 克苏鲁之脑之后）

- **木叶村**：新世界在出生点生成完整的木叶，阿吽大门、火影楼、忍者学校，共 48 间可入住的房子，原版 NPC 都能住进来。
- **第一试·笔试**：找主考官森乃伊比喜，答九道题，第十题是"接受还是放弃"。
- **第二试·死亡森林**：在丛林边的第四十四演习场入口找御手洗红豆领卷。从入口到森林深处的中央塔，一路有固定遭遇：
  - 入口外埋伏的考生小队；
  - 林中休息处和空心巨树；
  - 塔前空地的雨隐三人组（伞中千本雨、幻术分身）。
- **预选赛**：在中央塔大厅找月光疾风，对战音忍·多斯（响鸣穿连突、音波、地鸣、共鸣环、跳劈）。
- **正式赛**：木叶城墙外的会场，砂瀑之我爱罗（沙之盾、沙缚柩、守鹤半身化）；之后可找日向宁次切磋。
- **大蛇丸**：用蛇蜕在丛林召唤，打到一半他会遁走，可能留下写轮眼。

### 系统

- **查克拉与替身术**：独立的查克拉条；在挨打前一刻按替身术键（默认 F）留下一截木头闪开。
- **流派核心**：写轮眼、八门遁甲、白眼（仙术在后续篇章）。装备核心获得被动，按奥义键（默认 V）放流派奥义；找三代火影"立志"选定本命流派。
- **忍者手册**：任务、进度和首领信息；提示和原版一样含蓄，模组设置里可以打开"任务指引"显示方向与距离。
- **地区背景**：木叶（火影岩）、砂隐、雾隐、妙木山等地表背景。
- 装了 Boss Checklist 模组时，本模组的首领会出现在它的列表里。

---

## 游玩须知

- 需要 tModLoader（Steam 版，1.4.4）。
- **请使用新角色和新世界**：木叶、海边大桥和中忍考试场地都在世界生成时建好，旧世界里没有。
- 单人模式经过测试；多人模式的同步代码已写，但没有完整验证。

### 测试指令（单人）

主线不依赖这些指令，它们只用来跳过已经测过的部分：

| 指令 | 作用 |
| --- | --- |
| `/m0 god [on\|off]` | 测试无敌 |
| `/m0 story <1-5>` | 跳到波之国任务链的某一步 |
| `/m0 lake`、`/m0 preview` | 湖边水牢、断桥雾中预告 |
| `/m0 epilogue zabuza\|haku` | 直接播再不斩与白的尾声 |
| `/m0 exam <阶段>` | 设定中忍考试进度，如 `ForestGate`（刚考完笔试）、`ForestHunt`、`Prelims` |
| `/m0 exam gate\|tower\|camp\|tree\|rain\|stadium\|academy\|hokage` | 传送到考试场地 |
| `/m0 exam rebuild` | 在当前世界按最新设计重建死亡森林的场地 |
| `/m0 squad` | 叫来一队考生 |

---

## 从源码构建

源码在 `ShinobiPrototype/`。在 tModLoader 的 `ModSources/` 下建一个指向它的链接（或直接放进去），然后在游戏里「Workshop → Develop Mods」选 **Build + Reload**。

Mac 上的完整环境（.NET 8 x64 SDK、原生库路径）见 [DEVELOPMENT_MAC.md](DEVELOPMENT_MAC.md)。命令行一次跑完所有规则测试并打包：

```sh
./scripts/verify-mac.sh
```

游戏开着时命令行打包会报 TML003（模组文件被占用），这时只看测试和 C# 编译结果，打包交给游戏内的 Build + Reload。

规则层（刷怪条件、Boss 招式选择、场地布局等）写成不依赖 Terraria 的纯逻辑，测试在 `tests/` 下，每组都是独立的控制台程序。

---

## 仓库结构

| 路径 | 内容 |
| --- | --- |
| `ShinobiPrototype/` | 模组源码与贴图（`Common/` 规则与系统，`Content/` NPC、物品、弹幕） |
| `specs/` | 各篇章的设计规格；`总纲_主线与支线结构.spec.md` 是总体规划 |
| `tests/` | 规则测试与实机验收清单（`*.acceptance.md`） |
| `art/` | 美术请求（`requests/`）与交付（`deliveries/`），流程见 `art/AGENT_HANDOFF.md` |
| `scripts/` | 构建验证、世界生成检查、像素图处理脚本 |
| `tools/` | 开发用小工具（大桥预览、原版贴图解包） |
| `docs/archive/` | 早期设计稿与旧交接记录 |

### 美术

所有角色都以同一套泰拉瑞亚原生像素风为准（深色外描边、3～4 阶硬色阶、明亮饱和）。图由 AI 生成像素风姿势后，采样到像素网格并逐像素修整；每张图都先出样张，确认后才接进游戏。

---

## 版权说明

这是非商业的粉丝作品。《火影忍者》的角色、名称与设定归岸本齐史 / 集英社 / 东京电视台 / Studio Pierrot 所有。

仓库不包含任何动画原声、配音或官方图片。模组会在本地找原声文件（不随仓库分发），找不到时改用原版泰拉瑞亚音乐；美术参考图只保存在开发者本地。

---

## English summary

Shinobi Prototype weaves the first part of *Naruto* into vanilla Terraria without replacing it: every vanilla boss stays, and the story arcs unlock alongside them.

- **Wave Country** (around the Eye of Cthulhu): a mission chain ending in a two-boss fight against Zabuza and Haku on an unfinished bridge, with a scripted epilogue.
- **Chunin Exams** (after the Eater of Worlds / Brain of Cthulhu):
  - the Hidden Leaf generated around the spawn point;
  - a written test;
  - the Forest of Death with fixed encounters on the way to the central tower;
  - prelims against Dosu, finals against Gaara.
- **Systems**: chakra, the Substitution Jutsu, style cores (Sharingan, Eight Gates, Byakugan), a ninja handbook, and region backgrounds.

Start a new world. Build from `ShinobiPrototype/` with tModLoader's *Build + Reload*. Fan work; the repository contains no anime audio or official images (local soundtrack files are optional and fall back to vanilla music).
