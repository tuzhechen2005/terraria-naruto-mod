# M11 再不斩与白特效交付

状态：16张素材已制作并接入资源包，现有战斗装饰已接入；完整M11战斗逻辑仍未实现。用户要求只制作定稿文档已有特效，本轮没有新增招式、伤害、生命、判定、Boss体型或场地。

## 素材与短循环

| M11内容 | 资源 | 用途与状态 |
| --- | --- | --- |
| 5.2刀术水光、翻卷刀迹 | WaterSlash | 现有ZabuzaSlash装饰已接入；真实刀刃仍由角色/既有判定承担 |
| 5.3水龙聚势、龙头、龙身、崩散 | WaterGather、WaterDragonHead、WaterDragonBody、WaterSplash | 聚势/消散已接入现有阶段；完整龙头龙身绘制接口已备，等待M11危险路线/判定接入，不给旧78×32判定硬套巨大龙身 |
| 5.1水雾瞬身落点提示 | MistPuff | 素材与Mist绘制接口已备；M11确定落点的代码调用，当前未改瞬身寻点逻辑 |
| 6.1、6.2寒气、结晶、亮镜、裂纹、碎裂 | IceMirror、MirrorCracks、MirrorShards、FrostCloud、MirrorHalo | 现有冰镜成形、损伤裂纹、出手镜剪影与亮镜、暴走强化及破碎已接入；M11逐面调度由实际镜位/阶段提供 |
| 6.1千本亮针尖与冰痕、换位残像 | NeedleGlow、复用Haku本体帧 | 现有普通/镜阵/千杀针的亮尖冰痕、消散冰晶、移动残像已接入；针尖按纹理亮点(84,16)对齐真实弹幕端点 |
| 8.1紫色鬼影伸展、压下、烟焰 | DemonGhost、DemonPress | 现有鬼人状态后方绘制、斩击时过渡到压下姿态已接入；基线(128,300)，不伤人 |
| 8.1旋转刀迹与接刀冲击 | SwordSpinArc、ChakraImpact | 现有去/回程大刀旋转刀迹、接刀冲击环和音效已接入；余波不追加伤害 |
| 8.2低血悬浮针阵、针尖依次亮起、缺口 | NeedleGlow、NeedleArray绘制接口 | 当前PrismShard的实际悬浮预警已增强；完整阵列接口接收锁定位置/缺口，预览示范16逻辑位留2位，不新增第二套针阵实体 |
| 7.2先水龙、后千本两步合击 | 复用水/冰资源、LockedRoute | 组合预览与API已备；强招调度、锁线和安全落点属于M11逻辑，不在本PR改写 |
| 9.3短震、背景短压暗、分层音效 | WaveVfxBursts、WaveVfxAudio | 现有水龙释放、成功破阵、暴走启动已触发；重劈末击接口留给M11调用。已有白暴走强闪降为短促低强度冰蓝氛围 |

原图/提示词/裁切脚本分别保存在 `../wave-water-vfx-v1/`、`../wave-ice-vfx-v1/`、`../wave-demon-vfx-v1/`，均使用内置imagegen生成。没有分发外部动画音效或本地版权音乐，音效接口只用Terraria现有SoundID。

`*-dark-1x.png` 与 `*-light-1x.png` 是真正原始资源尺寸；`combined-loop.gif` 使用全循环公用色板，`combined-loop.png` 是24帧无损APNG。`water-composition.png` 与 `frenzy-composition.png` 为静态组合。

**组合预览是演示，不是游戏截图，也不是碰撞验收。** 人物用已有像素底稿缩放为126/105高，42px矩形仅是玩家身高标尺；游戏Boss体型未改变。预览中的平台、锁线、镜位、针阵仅解释定稿特效的配合，没有创建新玩法或场地。短循环是已生成位图的位移、旋转、缩放和透明度动画，不伪称逐帧AI生图。

## 接入接口

- 游戏资源：`ShinobiPrototype/Assets/Vfx/Wave/`，16张PNG。
- `WaveVfx.cs`提供Gather、LockedRoute、Dragon、Slash、Splash、Mist、Mirror、Silhouette、Shatter、Needle、NeedleArray、Ghost、SwordTrail、ChakraImpact。调用者提供同步阶段/锁定位置；渲染不选目标、不更新AI、不伤害玩家、不改世界。
- Dragon最多24段、NeedleArray最多16位；水龙使用实际alpha内容矩形去掉画布空白并让流段小幅重叠。调用者必须用实际伤害走廊供图，不把外围泡沫当新伤害。头/身裁切矩形由安装脚本校验，避免换源图后留下拼接空隙。
- 镜框、裂纹和halo共中心，绘制缩放分别配准；剪影复用实际白Idle帧。针尖/危险主体/路线/亮镜不受装饰强度0影响，水花、鬼影、刀迹、寒气可关闭。
- `ClientVisualAssets`在加载时异步预加载，并持有Asset句柄；逐帧取缓存，不重复读盘。
- `WaveVfxBursts`最多64个纯客户端余波，24更新帧内清理，离开世界/卸载归零；不占NPC/Projectile槽位。只减量装饰，预警直接绘制。
- 震屏8帧、最多3px；强招背景压暗18帧、上限20%，只在1200px附近触发，不叠加幅度、不锁镜头/输入。界面与普通前景玩家在后续绘制层；实际地形/血条可读性仍须实机检查。
- 现有客户端设置新增装饰强度、轻震、背景压暗，均有中英Localization。普针/轻斩不自动震屏。
- 音效按阶段事件调用，不能每个Draw重复播放；合击先后两步由未来同步状态机分别调用，单种声同时实例最多4。

## 复现与验证

在仓库根运行 `python3 scripts/install_wave_vfx.py`，校验三份交付后一次安装；再运行 `python3 scripts/build_wave_vfx_preview.py` 生成原生尺寸/组合/无损动画。`catalog.json`记录尺寸、RGBA、alpha边界、半透明像素、hash、锚点、帧数和危险/装饰角色。原图编辑必须走imagegen，不用脚本从零画VFX。

已检查：全部16张正确尺寸、RGBA、保留半透明、透明RGB为0、边缘留空；开放中心透明；游戏文件hash与交付一致；共同色板GIF/无损APNG24帧；绘制资源名称存在；文本检查0问题；没有添加新伤害/判定/世界修改。

最终 `./scripts/verify-mac.sh` 通过，文本0问题、14组规则测试通过、完整构建0错误；两条旧KonohaDump提示和已在基线存在的未定位未知图片类型打包提示仍在，见 `verify-mac.log`。三份导出脚本重跑后与游戏资源逐字节相同，资源名称和两个24帧动画检查通过。无头加载、单人/多人实机、实际平台/地形遮挡、左右镜像碰撞对应、特效开关帧时间/资源实测：**未运行**。未填写M11全改版通过，也不将组合预览当实机验收。
