# Ibiki NPC v1 delivery

- status: delivered
- request ID: `ibiki-npc-v1`

## Delivered files

| Path | Dimensions | Alpha |
| --- | ---: | --- |
| `source/Ibiki_Idle_Walk.png` | 1568 × 200; seven 224 × 200 cells | Transparent; only 0 and 255 |
| `source/Ibiki_Idle_Jump_Sit_Throw.png` | 1344 × 200; six 224 × 200 cells | Transparent; only 0 and 255 |
| `source/Ibiki_Talk.png` | 672 × 200; three 224 × 200 cells | Transparent; only 0 and 255 |
| `source/generated_Ibiki_walk.png` | 1945 × 808 | Transparent generated source; antialiased edges |
| `source/generated_Ibiki_action.png` | 2172 × 724 | Transparent generated source; antialiased edges |
| `source/generated_Ibiki_talk.png` | 1881 × 836 | Transparent generated source; antialiased edges |
| `Guide_Kakashi_Tazuna_Hiruzen_Ibiki_comparison.png` | 1200 × 560 | Opaque preview on dark and light backgrounds |

## Pose order

- `Ibiki_Idle_Walk.png`: Idle with hands behind back, Walk 1–6.
- `Ibiki_Idle_Jump_Sit_Throw.png`: same Idle, Jump, Sit, Throw preparation, Throw release, Throw recovery.
- `Ibiki_Talk.png`: same Idle, arms crossed, raised arm pointing forward to announce.

## Prompt summary

The built-in `image_gen__imagegen` tool generated three transparent pose strips of Morino Ibiki, all facing right. The prompts specified his tall broad build, black headscarf with a silver Leaf forehead plate, stern face with two pale scars, long black double-breasted coat, gray shirt, black trousers and ninja shoes. They requested blocky Terraria-style pixel art, distinct animation poses, and no background or text. The selected sources were converted to 28 × 25 art-pixel cells and enlarged to exact 8 × 8 screen-pixel blocks. Dark exterior pixels were lightened slightly for readability, and pale face pixels preserve the scars at the final scale.

## Checks performed

- Visually inspected all three generated originals, the final pose sheets, and the comparison preview beside the Guide, Kakashi, Tazuna, and Hiruzen.
- Confirmed 7, 6, and 3 poses in the requested order, facing right; the Idle cell is byte-identical across all three sheets.
- Confirmed final sheet dimensions, binary alpha, and uniform color within every 8 × 8 block. Standing feet share the bottom line; Jump is raised and Sit is shorter.
- Checked the headscarf, silver forehead plate, coat silhouette, scar pixels, and talk gesture at game scale on dark and light backgrounds. Ibiki is taller than Kakashi in the comparison.

## Remaining game-side checks

- Import the strips into tModLoader and verify draw offset, facing direction, collision-box fit, and frame selection.
- Play the walk, throw, and dialogue animations in game to check cadence and gesture readability.
- Check the two face scars and dark coat under the classroom's actual game lighting.
