# dosu-base-v1 delivery

- status: delivered
- request ID: `dosu-base-v1`
- generator: built-in `image_gen__imagegen`; no API key or fallback CLI

## Delivered files

| Files | Dimensions | Alpha |
| --- | --- | --- |
| `Dosu_Idle_0.png`–`Dosu_Idle_3.png` | each 112×88 | RGBA; transparent background, visible pixels alpha 255 |
| `Dosu_Walk_0.png`–`Dosu_Walk_3.png` | each 112×88 | RGBA; transparent background, visible pixels alpha 255 |
| `Dosu_DrillWindup_0.png`–`Dosu_DrillWindup_1.png` | each 112×88 | RGBA; transparent background, visible pixels alpha 255 |
| `Dosu_Drill_0.png`–`Dosu_Drill_1.png` | each 112×88 | RGBA; transparent background, visible pixels alpha 255 |
| `Dosu_Wave_0.png`–`Dosu_Wave_2.png` | each 112×88 | RGBA; transparent background, visible pixels alpha 255 |
| `Dosu_Hurt_0.png` | 112×88 | RGBA; transparent background, visible pixels alpha 255 |
| `preview.png` | 584×1104 | RGB, opaque; each action on light and dark backgrounds at actual 1× size |
| `alignment-check.png` | 340×624 | RGB, opaque; each action's frames overlaid with x=56 and y=84 guides |
| `source/Dosu_WalkA_generated.png` | 1254×1254 | RGBA, transparent with partially transparent source edges |
| `source/Dosu_WalkB_generated.png` | 1254×1254 | RGBA, transparent with partially transparent source edges |
| `source/Dosu_Hurt_generated.png` | 1254×1254 | RGBA, transparent with partially transparent source edges |

## Prompt summary

Used the approved `dosu-style-v1` Dosu source as a visual reference for three generated source poses: two right-facing fast walking strides and one recoiling hurt pose. Prompts preserved his bandaged single-eye face, hunched silhouette, camouflage cloak, snake-pattern scarf, straw back cape, large perforated sound gauntlet, limited palette, hard pixel edges, and transparent background. Existing approved idle, windup, drill, and wave poses supplied the base silhouettes for those actions. In the game-size frames, the idle robe sways, windup arcs strengthen, the drill retracts, and wave arcs expand across successive frames.

## Checks performed

- Inspected the generated walk and hurt sources and both final 1× preview backgrounds.
- Checked all 16 game frames are exactly 112×88, right-facing, and aligned to the same bottom visible pixel rows y=84–85; the center alignment guide is x=56.
- Checked every game frame uses binary alpha only (0 or 255) and every 2×2 screen-pixel block has identical RGBA values.
- Checked individual game frames use 12–20 opaque colors; the generated source images retain partial alpha only in `source/`.
- Checked the head wrap, scarf, straw back cape, gauntlet, and sound arcs remain readable at 1× on light and dark backgrounds.

## Remaining game-side checks

- Import the frames and verify animation timing, orientation, pivot, hitboxes, and sound-effect placement in tModLoader.
- Inspect the loop transitions and leg motion during actual gameplay; adjust individual pixels or timing if the game camera makes them read poorly.
