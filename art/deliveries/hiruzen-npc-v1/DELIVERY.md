# Hiruzen NPC v1 delivery

- status: delivered
- request ID: `hiruzen-npc-v1`

## Delivered files

| Path | Dimensions | Alpha |
| --- | ---: | --- |
| `source/Hiruzen_Idle_Walk.png` | 1568 × 200; seven 224 × 200 cells | Transparent; only 0 and 255 |
| `source/Hiruzen_Idle_Jump_Sit_Throw.png` | 1344 × 200; six 224 × 200 cells | Transparent; only 0 and 255 |
| `source/Hiruzen_Talk.png` | 672 × 200; three 224 × 200 cells | Transparent; only 0 and 255 |
| `source/generated_Hiruzen_walk.png` | 2126 × 740 | Transparent generated original; antialiased edges |
| `source/generated_Hiruzen_action.png` | 2172 × 724 | Transparent generated original; antialiased edges |
| `source/generated_Hiruzen_talk.png` | 1881 × 836 | Transparent generated original; antialiased edges |
| `Guide_Kakashi_Tazuna_Hiruzen_comparison.png` | 960 × 560 | Opaque dark/light 1× screen-pixel preview |

## Pose order

- `Hiruzen_Idle_Walk.png`: Idle holding pipe, Walk 1–6.
- `Hiruzen_Idle_Jump_Sit_Throw.png`: same Idle, Jump, Sit/smoke, Throw hand-seal preparation, Throw raised-hand release, Throw recovery.
- `Hiruzen_Talk.png`: same Idle, exhale smoke, raise hand to speak.

## Prompt summary

Built-in `image_gen__imagegen` generated three transparent pose strips. The prompts specified an elderly, short, right-facing Hiruzen with gray-white hair and goatee, wrinkles, white conical Hokage hat with a red Fire mark and trim, white broad-sleeved robe with red collar and triangular hem, dark inner collar, and brown pipe. They requested native Terraria-style blocky pixel art with hard shade steps, distinct poses, and no background. The selected strips were converted to 28 × 25 art-pixel cells and enlarged to exact 8 × 8 screen-pixel blocks. The final silhouettes were widened to read beside the Guide and Tazuna at game scale.

## Checks performed

- Inspected the generated originals, all three final sheets, and the comparison preview visually.
- Confirmed 7, 6, and 3 separated poses in the requested order, facing right. The Idle cell is byte-identical in all sheets.
- Confirmed exact dimensions, binary alpha, and uniform color within each 8 × 8 screen-pixel block. Standing frames end at the same ground line; jump is raised and sit is shorter.
- Checked the white/red silhouette, hat mark, pipe, speaking gesture, and release gesture at 1× screen scale on dark and light backgrounds beside the Guide, Kakashi, and Tazuna.

## Remaining game-side checks

- Import the sheets into tModLoader and confirm draw offset, facing direction, collision-box fit, and correct frame selection.
- Play the walk and throw sequences in game to check cadence and transitions.
- Check readability in the Hokage office under actual game lighting and backgrounds.
