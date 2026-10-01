# Anko NPC v1 delivery

- status: delivered
- request ID: `anko-npc-v1`

## Delivered files

| Path | Dimensions | Alpha |
| --- | ---: | --- |
| `source/Anko_Idle_Walk.png` | 1792 × 256; seven 256 × 256 cells | Transparent; 0 or 255 only |
| `source/Anko_Idle_Jump_Sit_Throw.png` | 1536 × 256; six 256 × 256 cells | Transparent; 0 or 255 only |
| `source/Anko_Talk.png` | 768 × 256; three 256 × 256 cells | Transparent; 0 or 255 only |
| `source/generated_Anko_walk.png` | 2171 × 724 | Transparent generated original; partial alpha at edges |
| `source/generated_Anko_action.png` | 2172 × 724 | Transparent generated original; partial alpha at edges |
| `source/generated_Anko_talk.png` | 1746 × 901 | Transparent generated original; partial alpha at edges |
| `Anko_Tazuna_comparison.png` | 670 × 410 | Opaque preview |

## Pose order

- `Anko_Idle_Walk.png`: Idle with hand on hip; Walk 1–6.
- `Anko_Idle_Jump_Sit_Throw.png`: Idle; Jump; Sit and eat dango; kunai wind-up; kunai release; follow-through.
- `Anko_Talk.png`: Idle; lick a kunai held beside the face; raise a hand to explain the rules.

The first Idle cell is pixel-identical in all three final sheets.

## Prompt summary

Built-in `image_gen__imagegen` generated three transparent pose strips, using the delivered Tazuna source strip as a style reference and the first Anko strip as the character reference for the other two. The prompts specified Part I Anko's short spiky purple ponytail, forehead protector, open beige calf-length coat, charcoal mesh top, orange skirt, knee guards, sandals, and teasing expression. They requested right-facing poses, separated silhouettes, near-black outlines, warm hard shading, and the listed walk, dango, kunai, and talk actions. The generated art was sampled into 32 × 32 art-pixel cells, given a shared 72-color palette without dithering, and enlarged exactly 8 × 8 per art pixel.

## Checks performed

- Inspected all three generated originals, all three final sheets, and the Tazuna comparison preview visually.
- Confirmed 7, 6, and 3 poses in the stated order, facing right and confined to separate cells.
- Confirmed exact dimensions, binary alpha, and uniform color within every 8 × 8 block of each final source sheet.
- Confirmed the standing Idle and all six walk poses are 28 art pixels high, with feet on art-pixel row 29, leaving two transparent rows below. Jump is lifted; Sit is lower.
- Checked the 1× game-size view and 4× inspection view beside Tazuna at 1.36×. Beige, purple, and orange remain distinct at game scale. Fine mesh, facial expression, dango, and the small kunai are easier to read in the enlarged source art than in the game-size preview.

## Remaining game-side checks

- Import the sheets and verify frame alignment, draw offset, facing direction, and collision-box fit in tModLoader.
- Play walk, jump, sit, throw, and talk animations in game to check cadence and action timing.
- Check outfit and small prop readability against the Forest of Death entrance and examiner-hut backgrounds, in daylight and at night.
