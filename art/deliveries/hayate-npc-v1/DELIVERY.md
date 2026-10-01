# Hayate NPC v1 delivery

- status: delivered
- request ID: `hayate-npc-v1`

## Delivered files

| Path | Dimensions | Alpha |
| --- | ---: | --- |
| `source/Hayate_Idle_Walk.png` | 1792 × 256; seven 256 × 256 cells | Transparent; alpha 0 or 255 only |
| `source/Hayate_Idle_Jump_Sit_Throw.png` | 1536 × 256; six 256 × 256 cells | Transparent; alpha 0 or 255 only |
| `source/Hayate_Talk.png` | 768 × 256; three 256 × 256 cells | Transparent; alpha 0 or 255 only |
| `source/generated_Hayate_walk.png` | 2172 × 724 | Transparent generated original; antialiased edges |
| `source/generated_Hayate_action.png` | 2172 × 724 | Transparent generated original; antialiased edges |
| `source/generated_Hayate_talk.png` | 1933 × 814 | Transparent generated original; antialiased edges |
| `Guide_Hayate_Tazuna_comparison.png` | 690 × 455 | Opaque preview |

## Pose order

- `Hayate_Idle_Walk.png`: Idle with hand over mouth, Walk 1–6.
- `Hayate_Idle_Jump_Sit_Throw.png`: identical Idle, Jump, Sit, katana draw, rightward slash, recovery/sheathing.
- `Hayate_Talk.png`: identical Idle, bent cough, raised-arm announcement.

## Prompt summary

Built-in `image_gen__imagegen` generated three transparent pose strips of Hayate Gekko facing right. The prompts specified a slim, pale, sickly ninja with dark eye circles, short brown hair, a cloth-wrapped Konoha forehead protector, green chunin vest with scroll pockets, dark blue clothing, and a katana hilt behind his right shoulder. The style requested hard-shaded pixel art, dark outlines, and upper-left highlights. Generated figures were sampled into 32 × 32 art-pixel cells, with 8 × 8 screen-pixel blocks. The standing Idle is 28 art pixels tall, with its feet two art pixels above the cell bottom. The comparison shows Hayate at game 1× and Tazuna at game 1.36×, plus a 4× inspection view.

## Checks performed

- Inspected the three generated strips, the final sprite sheets, and the comparison preview visually.
- Confirmed the 7, 6, and 3 frame counts, pose order, right-facing direction, and nonoverlapping cells.
- Confirmed the Idle cell is byte-identical across all three sheets, each cell is exactly 256 × 256, the standing baseline is shared, and the jump is raised above it.
- Programmatically confirmed binary alpha and uniform color within every 8 × 8 block in the three final sheets.
- Checked the silhouette and colors at game 1× and 4× beside Tazuna at game 1.36×.

## Remaining game-side checks

- Import the sheets and verify frame alignment, facing direction, collision-box fit, and dialogue pose selection in tModLoader.
- Play the six-frame walk and three-frame katana action in game to assess cadence and slash readability.
- Check the facial details and dark clothing against Central Tower lighting and backgrounds.
