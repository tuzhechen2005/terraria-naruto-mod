# Kakashi NPC v2 delivery

- status: delivered
- request ID: `kakashi-npc-v2`

## Delivered files

| Path | Dimensions | Alpha |
| --- | ---: | --- |
| `source/Kakashi_Idle_Walk.png` | 1568 × 200; 7 cells of 224 × 200 | Transparent; alpha 0 or 255 only |
| `source/Kakashi_Idle_Jump_Sit_Throw.png` | 1344 × 200; 6 cells of 224 × 200 | Transparent; alpha 0 or 255 only |
| `Guide_Kakashi_comparison.png` | 460 × 260 | Opaque preview |
| `source/generated_Kakashi_Idle_Walk.png` | 1875 × 839 | Transparent generated source |
| `source/generated_Kakashi_Idle_Jump_Sit_Throw.png` | 1881 × 836 | Transparent generated source |

## Prompt summary

Built-in `image_gen__imagegen` generated two transparent Kakashi action strips. Prompts specified Terraria pixel art, Guide-like body width, silver hair, slanted forehead protector, navy mask and sleeves, green vest, white calf wraps, right-facing upright walk cycle, reading an orange book, and three kunai actions. The generated strips were resampled into the requested 8 × 8 screen-pixel art grid and 28 × 25 art-pixel cells.

## Checks performed

- Inspected the generated strips and final sprite sheets visually.
- Confirmed all 13 requested cells are present and nonoverlapping; the idle cell is identical in both sheets.
- Confirmed exact sheet dimensions, binary alpha, and uniform color within every 8 × 8 screen-pixel block.
- Compared the idle frame beside the provided Guide reference at displayed scale; inspected the walking poses and seated/book and throwing silhouettes.

## Remaining game-side checks

- Import the two sprite sheets and check baseline, facing direction, and in-game scale.
- Play the six walking frames in sequence to assess gait and loop smoothness in motion.
- Review jump, seated reading, and kunai timing in the game; adjust frames if animation or hitbox alignment requires it.
