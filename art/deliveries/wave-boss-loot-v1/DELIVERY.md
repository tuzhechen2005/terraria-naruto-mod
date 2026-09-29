status: delivered
request ID: wave-boss-loot-v1

# Delivered files

| Asset | Game PNG | Dimensions | Source PNG | Source dimensions |
|---|---|---:|---|---:|
| 斩首大刀 | `Kubikiribocho.png` | 56×56 | `source/Kubikiribocho_source.png` | 1254×1254 |
| 千本 | `Senbon.png` | 28×28 | `source/Senbon_source.png` | 1254×1254 |
| 水龙弹卷轴 | `WaterDragonScroll.png` | 32×32 | `source/WaterDragonScroll_source.png` | 1254×1254 |
| 魔镜冰晶卷轴 | `IceMirrorScroll.png` | 32×32 | `source/IceMirrorScroll_source.png` | 1254×1254 |
| 专家宝藏袋 | `WaveBossBag.png` | 32×34 | `source/WaveBossBag_source.png` | 1217×1293 |

`PREVIEW.png` shows every game PNG at native size and 2× size on dark and light backgrounds. All paths above are relative to this delivery directory. Every PNG has transparency. Game PNG alpha is strictly 0 or 255; the generated source PNGs retain their original alpha.

# Prompt summary

Generated five isolated Terraria-style pixel-art items against transparent backgrounds using the supplied Zabuza/Haku style references and the sword reference. The sword points up-right with both blade cutouts and a cloth-wrapped handle; senbon are silver needles tied with red cord; the blue and cyan jutsu emblems sit on partially opened scrolls; the treasure bag is mist blue-gray with four stacked mist marks. Game icons were reduced onto a 2×2 screen-pixel art grid. The bag emblem was clarified at game size so all four marks remain visible.

# Checks performed

- Visually inspected every generated source and game icon.
- Confirmed exact game dimensions, binary alpha, and uniform 2×2 pixel blocks for all five icons.
- Inspected native-size and 2× previews against both dark and light backgrounds in `PREVIEW.png`.
- Checked up-right sword direction, visible blade holes, distinct red-tied needles, scroll silhouettes and color separation, and four bag marks.

# Remaining game-side checks

- Integrate the five PNGs into the tModLoader item texture paths and inspect inventory, world-drop, and tooltip views in game.
- Verify Terraria texture filtering and UI scaling preserve the 2×2 pixel edges and that the bag marks remain readable during actual play.
