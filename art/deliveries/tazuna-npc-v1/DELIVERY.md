# Tazuna NPC v1 delivery

- status: delivered
- request ID: `tazuna-npc-v1`

## Delivered files

| Path | Dimensions | Alpha |
| --- | ---: | --- |
| `source/Tazuna_Idle_Walk.png` | 1568 × 200; seven 224 × 200 cells | Transparent; alpha 0 or 255 only |
| `source/Tazuna_Idle_Jump_Sit_Throw.png` | 1344 × 200; six 224 × 200 cells | Transparent; alpha 0 or 255 only |
| `source/generated_Tazuna_Idle_Walk.png` | 2172 × 724 | Transparent generated original; antialiased edges |
| `source/generated_Tazuna_Idle_Jump_Sit_Throw.png` | 2172 × 724 | Transparent generated original; antialiased edges |
| `Guide_Tazuna_comparison.png` | 460 × 260 | Opaque preview |

## Pose order

- `Tazuna_Idle_Walk.png`: Idle, Walk 1–6. The walk frames alternate legs and arm swing.
- `Tazuna_Idle_Jump_Sit_Throw.png`: Idle, Jump, Sit/drink, Throw wind-up, Throw/release, Throw/recovery.

## Prompt summary

Built-in `image_gen__imagegen` created two transparent pose strips. The prompts specified an elderly, sturdy, slightly hunched Tazuna facing right: gray-white hair and short beard, round eyeglasses, white neck towel, cream shirt, worn ochre vest, dark loose trousers, sandals, and a waist sake gourd. They requested Terraria-style square pixel art with earthy hard-shaded colors, a six-frame walk, drinking while seated, and a three-frame carpenter-hammer throw. The generated strips were sampled into 28 × 25 art-pixel cells and enlarged at 8 × 8 screen pixels per art pixel.

## Checks performed

- Inspected both generated originals, final sprite sheets, and the Guide comparison preview visually.
- Confirmed 7 and 6 nonoverlapping poses in the requested order, all facing right; the Idle cell is byte-identical in both sheets.
- Confirmed exact sheet dimensions, 23-art-pixel standing height, common ground baseline for standing poses, binary alpha, and uniform color within every 8 × 8 screen-pixel block.
- Checked silhouette and clothing colors at game scale beside the provided Guide reference. The round glasses, towel, gourd, drinking pose, and hammer action remain visible.

## Remaining game-side checks

- Import the two sheets and confirm frame alignment, draw offset, facing direction, and collision-box fit in tModLoader.
- Play the six walking frames and three throwing frames in game to confirm cadence and hammer-release timing.
- Check readability against the bridge house and town backgrounds in daylight and at night.
