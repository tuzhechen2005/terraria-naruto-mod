# Mac 开发环境

项目唯一源码位于本工作区的 `ShinobiPrototype/`。Mac 的 tModLoader `ModSources/ShinobiPrototype` 是指向它的符号链接；不要复制一份源码后分别修改。`Downloads/泰拉瑞亚模组/` 是迁移原始包，保留作备份。

## 已准备

- Steam 版 tModLoader 位于 `~/Library/Application Support/Steam/steamapps/common/tModLoader`。
- .NET 8 x64 SDK 位于 `~/.dotnet-x64`，供 tModLoader 命令行打包；另有 Homebrew 的 ARM64 .NET 8 SDK。
- `ShinobiPrototype.csproj` 不再使用 Windows 的绝对路径，默认查找上述 Mac Steam 目录。若游戏装在另一处，可设置 `TML_DIR`，或给 MSBuild 传 `TmlTargetsPath`。
- `./scripts/verify-mac.sh` 运行六组规则测试，并生成完整 `.tmod`。脚本临时设置 tModLoader 所需的 macOS 原生库路径，不修改 Steam 安装文件。
- 当前生成的模组包位于 `~/Library/Application Support/Terraria/tModLoader/Mods/ShinobiPrototype.tmod`。

## 验证与限制

在项目根目录运行：

```sh
./scripts/verify-mac.sh
```

完整打包已在这台 Mac 上通过，结果为 0 个编译错误、0 个模组构建警告。六组规则测试均通过；`WaveDuoRules` 测试项目自身有两条常量模式警告。**尚未在 Mac 游戏内加载或试玩**，加载、图像、操作手感和联机仍须单独验收。当前玩法以 `specs/M9_波之国双首领战.spec.md` 为准；旧 `README.md` 和部分 M1/M2 清单仍描述分别挑战白与再不斩。

下载包里的 `.git` 缺少 `objects/` 和 `refs/`，不能作为 Git 仓库使用。本工作区使用新建的正常仓库，迁移前的提交历史无法从该包恢复。
