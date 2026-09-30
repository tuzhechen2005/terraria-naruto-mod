# dosu-style-v1 delivery

- status: delivered
- request ID: `dosu-style-v1`
- generated with: built-in `image_gen__imagegen`; no API key or CLI

## Delivered files

| File | Size | Alpha |
| --- | --- | --- |
| `Dosu_Idle.png` | 112×88 | RGBA, transparent background; visible pixels alpha 255 |
| `Dosu_DrillWindup.png` | 112×88 | RGBA, transparent background; visible pixels alpha 255 |
| `Dosu_Drill.png` | 112×88 | RGBA, transparent background; visible pixels alpha 255 |
| `Dosu_Wave.png` | 112×88 | RGBA, transparent background; visible pixels alpha 255 |
| `preview.png` | 496×208 | RGB, opaque light and dark backgrounds, actual 1× sprites |
| `scale-preview-haku-gaara.png` | 352×112 | RGB, opaque background, actual 1× Haku/Gaara/Dosu sprites |
| `source/Dosu_Idle_generated.png` | 1254×1254 | RGBA, source alpha includes partial edge pixels |
| `source/Dosu_DrillWindup_generated.png` | 1254×1254 | RGBA, source alpha includes partial edge pixels |
| `source/Dosu_Drill_generated.png` | 1254×1254 | RGBA, source alpha includes partial edge pixels |
| `source/Dosu_Wave_generated.png` | 1254×1254 | RGBA, source alpha includes partial edge pixels |

## Prompt summary

Generated a hunched, right-facing Dosu with white bandages exposing one eye, short black hair, music-note forehead plate, mottled gray-beige cloak, snake-pattern scarf, straw hump, and a large perforated metal sound gauntlet. The four prompts specified idle, raised windup, forward drill strike, and horizontal sound-wave attack. The requested Terraria-style limited palette and hard pixel-art outlines were used. The wave source's filled white effect was simplified to thin pixel arcs in the game-size sprite.

## Checks performed

- Visually inspected generated source poses and both 1× previews against the named Haku and Gaara references.
- Checked each game sprite at 112×88 with the feet on the y=84–85 pixel block and consistent right-facing character direction.
- Checked all game sprites have binary alpha only (0 or 255), with no partially transparent visible character pixels.
- Checked every 2×2 screen-pixel block is identical and each final sprite uses 17–21 RGBA colors including transparency.
- Checked the large gauntlet silhouette and sound effect remain visible on light and dark preview backgrounds.

## Remaining game-side checks

- Import sprites into the mod, confirm Terraria rendering and animation timing in the Central Tower boss encounter.
- Check the smallest face/headband/scarf details against the actual game background and UI scale; adjust by hand if needed.
