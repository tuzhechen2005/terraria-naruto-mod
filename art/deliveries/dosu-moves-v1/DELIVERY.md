# dosu-moves-v1 delivery

- status: delivered
- request ID: `dosu-moves-v1`
- generator: built-in `image_gen__imagegen`

## Delivered files

| Paths | Dimensions | Alpha |
| --- | --- | --- |
| `Dosu_Slam_0.png`, `Dosu_Slam_1.png`, `Dosu_Slam_2.png` | each 112×88 | RGBA; only 0 or 255 |
| `Dosu_Leap_0.png`, `Dosu_Leap_1.png`, `Dosu_Leap_2.png` | each 112×88 | RGBA; only 0 or 255 |
| `source/Dosu_Slam_0_generated.png`, `source/Dosu_Slam_1_generated.png`, `source/Dosu_Slam_2_generated.png` | each 1415×1112 | RGBA; transparent with partial-alpha source edges |
| `source/Dosu_Leap_0_generated.png`, `source/Dosu_Leap_1_generated.png`, `source/Dosu_Leap_2_generated.png` | each 1415×1112 | RGBA; transparent with partial-alpha source edges |
| `preview-1x.png` | 896×112 | RGB; opaque checkerboard preview at actual game-frame size |

## Prompt summary

Generated six right-facing Dosu poses with the approved idle and drill frames as visual references: raised-gauntlet slam windup, kneeling slam impact, half-crouched recovery, deep leap crouch, airborne diagonal gauntlet dive, and compressed landing impact. Prompts specified the single visible eye, head bandages, pale collar, tattered cloak and back cape, perforated right-arm sound gauntlet, muted gray-brown palette, pixel-art silhouette, and transparent background. Each source was reduced to the 2×2 game-pixel grid and mapped to colors used by existing Dosu frames.

## Checks performed

- Inspected each generated source and the 1× preview beside `Dosu_Idle_0.png` and `Dosu_Drill_0.png`.
- Verified all six game frames are 112×88, face right, use only opaque or fully transparent pixels, and have identical RGBA values within every 2×2 screen-pixel block.
- Verified the five grounded frames end at y=83; airborne `Dosu_Leap_1.png` ends at y=73.
- Verified the silhouette, bandaged head, gauntlet, and distinct windup, impact, recovery, crouch, dive, and landing poses remain readable at 1×.

## Remaining game-side checks

- Import the frames in tModLoader and check animation timing, pose transitions, facing, character pivot, hitbox alignment, impact frame timing, and gauntlet contact against the actual terrain.
