# Shinobi Prototype：迁移到 Mac 开发交接

更新于 2026-09-28。本文用于把当前 Windows 上的 tModLoader 模组项目交给 Mac 继续开发；它不表示已经在 Mac 上构建或试玩通过。

## 需要带走什么

- 复制本仓库**整个项目文件夹**，至少包含 `ShinobiPrototype/`、`art/`、`specs/`、`tests/` 和根目录文档。`ShinobiPrototype/` 是模组源码；单独复制游戏安装的 `.tmod` 不足以继续开发。
- `bin/`、`obj/`、`.vs/` 和 `.tmod` 属于可再生成的产物，不必迁移。当前 Git 文件均尚未提交，不能只在 Mac 上 `git clone` 并期望拿到现有内容；首次迁移以复制项目目录为准。
- 如需保留角色与世界，**先退出游戏**，再从 Windows 的 `C:\Users\徐吉良\Documents\My Games\Terraria\tModLoader\` 另行复制 `Players/`、`Worlds/`。保留 `.plr`/`.tplr` 和 `.wld`/`.twld` 及其备份；模组角色、世界数据可能存于对应的 `t` 文件。
- 迁移前可分别为项目文件夹及存档文件夹留一份不修改的备份。不要把存档直接覆盖到 Mac 已有的同名角色或世界上。

## Mac 首次启动

1. 安装与 Windows 端相同分支/版本的 Terraria、tModLoader，并安装适合 Mac 芯片的 .NET 8 SDK。先启动 tModLoader 一次，再退出。
2. 在 Mac 的 tModLoader 菜单中通过 **Workshop → Develop Mods → Open Sources** 找到实际使用的 `ModSources` 目录。不同安装方式可能改变路径，不要照抄 Windows 路径。
3. 把复制来的 `ShinobiPrototype/` 直接放进 `ModSources/`，使目录结构为 `ModSources/ShinobiPrototype/build.txt`、`ModSources/ShinobiPrototype/Content/...`。
4. **先修正 `ShinobiPrototype/ShinobiPrototype.csproj`**：它目前只有一条写死的 `<Import Project="D:\Steam\steamapps\common\tModLoader\tMLMod.targets" />`，在 Mac 上必然找不到。优先参照 Mac 端 tModLoader“创建模组”生成的项目文件，改用 Mac 实际生成的 `ModSources/tModLoader.targets` 或对应的本机 tModLoader targets；不要沿用 Windows 绝对路径。若 Mac 的 `ModSources` 尚无 targets，可先在游戏里创建一个临时示例模组以取得当前版本的模板，再对照调整本项目。不要删除源码或覆盖本项目素材。
5. 在 **Workshop → Develop Mods** 中选择本模组的 **Build + Reload**。首次成功后，确认模组列表显示 `Shinobi Prototype`、版本 `0.3.3`，并进入一个**测试世界**检查是否能加载贴图和召唤首领。命令行 `dotnet build` 可辅助查错，但不能代替游戏内加载验证。
6. 如需接续旧存档，先在 Mac 启动游戏一次以生成自身存档目录，再通过游戏或 Finder 找到 tModLoader 实际使用的数据目录。备份 Mac 原有 `Players/`、`Worlds/` 后，合并从 Windows 带来的文件；不要只复制 Terraria 原版的存档目录。首次打开旧世界前保留原始备份。

官方参考：[tModLoader 基础开发指南](https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide)、[Mac/Linux 开发说明](https://github.com/tModLoader/tModLoader/wiki/Developing-on-Mac-or-Linux)、[.NET 8 SDK 下载](https://dotnet.microsoft.com/download/dotnet/8.0)。

## 项目现状与开发入口

- 最新战斗设计以 `specs/M9_波之国双首领战.spec.md` 为准：再不斩半血转鬼人后召唤白；两人独立血量；任一方先死，另一方强化；需要同一场击败两人才算通关。白的冰镜空间、短暂双人隐身、四方向低密度冰锥，以及原版首领血条也列在 M9 规格中。
- `README.md` 的部分描述仍属于更早的“白和再不斩分别挑战”版本，迁移后不应据此判断当前 M9 玩法。M9 规格记录“代码和素材已实现、自动测试通过；实机验收待复核”，这是此前的项目记录，**不是 Mac 端验收结果**。
- 主要实现位于 `ShinobiPrototype/Content/NPCs/ZabuzaBoss.cs`、`HakuBoss.cs`、对应弹幕和 `ShinobiPrototype/Common/WaveDuoRules.cs`。对应规则测试为 `tests/WaveDuoRules/WaveDuoRules.Tests.csproj`。在项目根目录可运行 `dotnet run --project tests/WaveDuoRules/WaveDuoRules.Tests.csproj`；其他规则测试见 `tests/`。
- Windows 的 `Sync-ModSources.ps1` 根据 Windows 用户目录复制源码，Mac 上不应直接照用。最简单的 Mac 工作方式是**直接编辑 `ModSources/ShinobiPrototype/` 中的源码**；若同时保留仓库的另一份副本，必须明确哪份是唯一源码，避免改了仓库却构建旧副本。
- `art/Prepare-*.ps1` 和 `tests/Verify-*.ps1` 也是 PowerShell 脚本。它们不是游戏内 Build + Reload 的前置条件；在 Mac 上要运行这些脚本时需另装 PowerShell，并检查脚本里的路径及工具依赖。

## Mac 端最小验收清单

- [ ] `Build + Reload` 成功，模组列表版本正确，无缺失资源或大小写错误。Mac 若使用区分大小写的文件系统，贴图路径的字母大小写需与文件名完全一致。
- [ ] 新测试角色、世界可进入；`/m0 god on` 可用于单人测试，结束后用 `/m0 god off` 关闭。
- [ ] 再不斩半血转场后白加入，冰镜空间能正常结束；两种击杀顺序、玩家死亡重开均按 M9 规格结算。
- [ ] 使用游戏选择的**原版首领血条**，两个首领头像和地图头像正常；白的千本、冰锥、冰镜在明暗背景下可辨认。
- [ ] 旧角色与世界如已迁移，进入前后的角色物品、任务进度和世界状态一致。
- [ ] 联机、弹幕平衡和完整流程尚需实机复核；仅构建成功不能视为这些项目通过。

若加载失败，先保存完整错误日志和 tModLoader 版本、Mac 芯片型号、`dotnet --info` 输出。`MissingResourceException` 优先检查 `Content/...` 路径、素材是否完整及大小写；targets 找不到则先检查 `.csproj` 的 Windows 绝对路径是否已替换。
