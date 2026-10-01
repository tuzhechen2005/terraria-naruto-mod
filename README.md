<p align="center">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="docs/images/banner-dark.png">
    <img src="docs/images/banner-light.png" alt="Shinobi Prototype · 泰拉瑞亚火影忍者模组" width="640">
  </picture>
</p>

<p align="center">
  <a href="https://github.com/tuzhechen2005/terraria-naruto-mod/actions/workflows/tests.yml"><img src="https://github.com/tuzhechen2005/terraria-naruto-mod/actions/workflows/tests.yml/badge.svg" alt="CI"></a>
  <a href="https://github.com/tuzhechen2005/terraria-naruto-mod/stargazers"><img src="https://img.shields.io/github/stars/tuzhechen2005/terraria-naruto-mod?style=flat&logo=github&label=stars&color=f08c28" alt="GitHub stars"></a>
  <img src="https://img.shields.io/badge/version-0.4.0-e05a2b" alt="version 0.4.0">
  <img src="https://img.shields.io/badge/tModLoader-1.4.4-4e7d32" alt="tModLoader 1.4.4">
  <a href="LICENSE"><img src="https://img.shields.io/badge/license-MIT-3a6ea5" alt="MIT license"></a>
</p>

<p align="center">一个《火影忍者》主题的 tModLoader 模组，还在开发中。</p>

---

## 这是什么

我想在泰拉瑞亚里玩一遍火影第一部的剧情，又不想丢掉原版的流程，所以做了这个模组。原版的 Boss 都还在，火影的剧情插在它们中间：打完克苏鲁之眼前后去波之国，打完世界吞噬者或克苏鲁之脑之后参加中忍考试。

目前做到中忍考试。

波之国这一段已经能完整玩下来。卡卡西带你去海边找造桥的达兹纳，路上被鬼之兄弟伏击，到湖边救被水牢困住的卡卡西，最后在没修完的桥上打再不斩和白。两人都倒下以后有一段尾声。

中忍考试的代码基本写完了，我正在一段一段地实际玩、一段一段地改，有些人物还在用占位图。新世界会在出生点生成木叶村，考试的几关是：
- 伊比喜的笔试；
- 死亡森林抢卷轴；
- 中央塔里和音忍多斯的预选赛；
- 城墙外会场里和我爱罗的正式赛。

另外还有查克拉、替身术（挨打前一刻按 F），以及写轮眼、八门、白眼几种流派核心。

## 怎么玩

需要 tModLoader（Terraria 1.4.4）。一定要**新建世界**：木叶村、海边的桥、考试场地都是生成世界时建的，旧世界里没有。目前只在单人模式下测过。

不知道下一步干什么，可以翻忍者手册，或者去问卡卡西。

想跳过已经玩过的部分，可以用这些测试指令（单人模式）：

| 指令 | 作用 |
| --- | --- |
| `/m0 god on` / `off` | 无敌 |
| `/m0 story 1`～`5` | 跳到波之国任务的某一步 |
| `/m0 epilogue zabuza` | 直接看再不斩和白的尾声 |
| `/m0 exam ForestGate` | 进度设到刚考完笔试 |
| `/m0 exam gate` / `tower` / `stadium` | 传送到考试场地 |
| `/m0 exam rebuild` | 在当前世界重建死亡森林的场地 |

完整列表直接输入 `/m0` 查看。

## 自己编译

把 `ShinobiPrototype/` 放进（或链接到）tModLoader 的 `ModSources/`，然后在游戏里「Workshop → Develop Mods」点 Build + Reload。

我是在 Mac 上开发的，环境配置记在 [DEVELOPMENT_MAC.md](DEVELOPMENT_MAC.md) 里。`./scripts/verify-mac.sh` 会跑完所有测试再打包；游戏开着的时候打包会失败，这是正常的，这时就在游戏里 Build + Reload。

刷怪条件、Boss 选招、场地布局这类规则都写成了不依赖泰拉瑞亚的代码，测试在 `tests/` 里，每次推送 GitHub Actions 都会跑一遍。

## 目录

```
ShinobiPrototype/   模组源码和贴图
specs/              每一篇的设计文档
tests/              规则测试和游戏内验收清单
art/                美术的请求和交付记录
scripts/            构建、世界生成检查、处理像素图的脚本
docs/archive/       早期的旧文档
```

想参与的话看 [CONTRIBUTING.md](CONTRIBUTING.md)。

## 版权

代码和原创美术用 [MIT](LICENSE) 许可。火影的角色、名字和设定属于岸本齐史和集英社；泰拉瑞亚属于 Re-Logic。这是非官方的同人作品。

仓库里没有放任何动画原声、配音或官方图片。

---

**English:** A work-in-progress Naruto mod for Terraria (tModLoader 1.4.4). It keeps every vanilla boss and slots Part I of the story in between: the Land of Waves arc around the Eye of Cthulhu, and the Chunin Exams after the Eater of Worlds / Brain of Cthulhu, in a Hidden Leaf village generated at spawn. Start a new world to play. Code and original art are MIT; Naruto belongs to its owners.
