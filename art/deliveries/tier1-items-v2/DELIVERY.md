# Tier 1 item icons v2

- status: delivered
- request ID: `tier1-items-v2`
- delivered files: `TrainingNinjato.png`, `DemonChainGauntlet.png`, `RoughCloth.png`, `RogueHeadband.png`, `ToolBlueprint.png`, `MissionToken.png`, `KonohaHeadband.png`, `ToolPouch.png`, `PillPouch.png`, `TrainingPost.png`; matching generated source images in `source/`; `preview.png`.

| Item PNG | Game size | Source size |
| --- | ---: | ---: |
| `TrainingNinjato.png` | 40×40 | 1254×1254 |
| `DemonChainGauntlet.png` | 36×36 | 1254×1254 |
| `RoughCloth.png` | 24×20 | 1374×1145 |
| `RogueHeadband.png` | 28×16 | 1659×948 |
| `ToolBlueprint.png` | 24×24 | 1254×1254 |
| `MissionToken.png` | 20×24 | 1161×1355 |
| `KonohaHeadband.png` | 28×16 | 1659×948 |
| `ToolPouch.png` | 24×24 | 1254×1254 |
| `PillPouch.png` | 22×24 | 1226×1283 |
| `TrainingPost.png` | 22×32 | 1060×1484 |

All item and source images are RGBA with binary alpha (0/255). The game-size icons use 2×2 screen-pixel blocks. `preview.png` is 960×1222 RGB and shows each icon at 1× and 3× on light, dark, and inventory-blue backgrounds.

**Prompt summary:** Isolated Terraria-style pixel item icons with dark outlines, hard shade steps, upper-left highlights, and transparent backgrounds. The sword uses a wider bright steel blade, square guard, wrapped handle, and pommel ring; the gauntlet uses bright steel claws, red strap, and trailing chain. Materials and accessories follow the subjects and colors in the request.

**Checks performed:** Inspected all ten generated sources and the final preview. Confirmed icon dimensions, binary alpha for every source and game icon, 2×2 pixel blocks in each game icon, lower-left hilt to upper-right sword tip, separated bright gauntlet claws, and visibility on the inventory-blue preview. The headbands were enlarged within their requested canvases after the first scale check.

**Remaining game-side checks:** Import the ten icons, confirm inventory presentation at native UI scale against the actual game background, and review silhouette and color alongside adjacent vanilla and mod items. No game build or in-game acceptance check was run by the art worker.
