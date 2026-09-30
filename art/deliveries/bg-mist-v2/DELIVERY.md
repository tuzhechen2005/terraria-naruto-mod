# bg-mist-v2 交付状态

- status: blocked
- request ID: `bg-mist-v2`
- delivered file paths: 无；未生成 PNG。
- image dimensions and alpha status: 无成品可检查。
- prompt summary: 以雾隐村圆柱塔楼、屋顶绿植、中央水影楼与群岛浓雾制作远景；以波之国渔村、低矮平直的鸣人大桥制作中景；以礁石、芦苇、渔船和木桩码头制作近景。三层均要求透明天空、Terraria 像素画风和水平无缝平铺。
- checks performed: 已查看请求指定的四张《火影忍者》场景参考和四张 Terraria 背景参考；已尝试通过内置 `image_gen__imagegen` 生成远景。工具返回 HTTP 429 `usage_limit_reached`，提示约 3242 秒后重置，因此没有任何可供检查或交付的生成图片。
- remaining game-side checks: 三层 PNG 尚未生成；生成后须核对精确尺寸、透明天空、像素轮廓、左右接缝、三层叠放时的标志建筑可见性，制作日夜各一张双平铺预览，再由开发助手接入游戏检查。

## 参考图对应计划

- `kirigakure.png`：远景中央水影楼、圆柱塔楼、小方窗、绿色屋顶。
- `land_of_water.png`：远景岩岛剪影、灰蓝绿大气色和浓雾。
- `land_of_waves.png`：中景海岸渔村的密集简朴屋顶。
- `naruto_bridge.png`：中景海面上的长而平的低桥及连续桥墩。
