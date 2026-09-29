# Demon Brothers v1 delivery

- status: delivered
- request ID: `demon-brothers-v1`

## Delivered files

| Path | Dimensions | Alpha |
| --- | ---: | --- |
| `source/Gozu_Actions.png` | 2816 × 208; 11 cells of 256 × 208 | Transparent; alpha 0 or 255 only |
| `source/Meizu_Actions.png` | 2816 × 208; 11 cells of 256 × 208 | Transparent; alpha 0 or 255 only |
| `source/Chain_Segment.png` | 48 × 24; 6 × 3 art pixels | Transparent; alpha 0 or 255 only |
| `source/generated_Gozu_Actions.png` | 2172 × 724 | Transparent generated source; soft alpha present |
| `source/generated_Meizu_Actions.png` | 2079 × 756 | Transparent generated source; soft alpha present |
| `source/generated_Chain_Segment.png` | 1774 × 887 | Transparent generated source; soft alpha present |

## Pose order

For both action sheets, left to right: Idle (1), Run (6), Swipe wind-up (1), Swipe outward (1), Swipe recovery (1), Hurt (1). The leftmost Idle frame is the size reference. All frames face right and share a bottom baseline within their cells.

## Prompt summary

The built-in `image_gen__imagegen` tool generated separate transparent action strips for Gōzu and Meizu and one chain-link source. Prompts specified Terraria-style chunky pixel art, dark respirator masks, a single short forehead horn, ragged cloaks, oversized metal claw gauntlets with attachment rings, and distinct blue-gray versus green-gray palettes. The chain prompt specified a horizontal repeatable iron link. The selected source images were copied from `$CODEX_HOME/generated_images` into `source/`. The game-size sheets were cropped, reduced to 32 × 26 art-pixel cells, color-quantized, and expanded with 8 × 8 solid screen-pixel blocks. The 6 × 3 chain silhouette was simplified from the generated link for readable repetition at game scale.

## Checks performed

- Inspected the three generated source images and the game-size Idle frames and chain visually.
- Confirmed eleven nonempty frames per character, separate cells with clear outer columns, shared bottom baseline, and right-facing poses.
- Confirmed the final game-size images have exact requested dimensions, binary alpha, and uniform color within every 8 × 8 block.
- Confirmed the two cloak palettes are visually distinct and the chain's left/right connector pixels match.

## Remaining game-side checks

- Import the sheets and verify the 256 × 208 cell parsing, displayed scale, facing direction, and baseline in tModLoader.
- Play both six-frame run loops and three-frame swipes in game; adjust timing, hitboxes, and any poses that read poorly in motion.
- Check mask, horn, claw thickness, and chain connection against Terraria backgrounds at actual game zoom. These details are small and have not been verified in game.
