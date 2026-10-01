# 参与贡献

欢迎试玩、提问题和提建议。这是一个个人开发的粉丝模组，节奏是"实机试玩 → 反馈 → 逐条打磨"。

## 反馈问题

在 [Issues](https://github.com/tuzhechen2005/terraria-naruto-mod/issues) 里写清楚：

- 在哪一段（波之国、笔试、死亡森林、预选赛……）、做了什么、看到了什么；
- 截图，或 tModLoader 日志 `client.log` 里相关的报错；
- 是不是新建的世界（木叶、大桥和考试场地都只在新世界里生成）。

## 改代码

- 源码在 `ShinobiPrototype/`。能写成纯逻辑的规则（刷怪条件、Boss 选招、场地布局、数值）放在 `ShinobiPrototype/Common/*Rules.cs` 或 `*Design.cs`，不依赖 Terraria，并在 `tests/` 里补测试。
- 设计先写进 `specs/`（各篇章的规格），实机验收项写进 `tests/*.acceptance.md`。
- 提交前跑一遍规则测试：`for p in tests/*/*.Tests.csproj; do dotnet run --project "$p"; done`，再在游戏里 Build + Reload 看实际效果。构建通过不等于游戏内验收通过。

## 美术

- 所有角色都以达兹纳（`art/deliveries/tazuna-npc-v1/`）的泰拉瑞亚原生像素风为准：深色外描边、每种材质 3～4 阶硬色阶、明亮饱和、没有零散噪点。
- 请求写在 `art/requests/`，交付放在 `art/deliveries/`，流程见 `art/AGENT_HANDOFF.md`。

## 版权

不要提交动画原声、配音、官方图片或其他受版权保护的素材。参考图只放在本地的 `art/reference/`（已被 `.gitignore` 排除）。
