# Demon Brothers v2 delivery

- status: delivered
- request ID: `demon-brothers-v2`

## Delivered files

| Path | Dimensions | Alpha |
| --- | ---: | --- |
| `source/Gozu_Actions.png` | 2816 × 208; 11 cells of 256 × 208 | Transparent; alpha 0 or 255 only |
| `source/Meizu_Actions.png` | 2816 × 208; 11 cells of 256 × 208 | Transparent; alpha 0 or 255 only |
| `source/Chain_Segment.png` | 48 × 24 | Transparent; alpha 0 or 255 only |
| `source/generated_Gozu_Actions.png` | 2171 × 724 | Transparent generated source; includes intermediate alpha |
| `source/generated_Meizu_Actions.png` | 2171 × 724 | Transparent generated source; includes intermediate alpha |
| `source/generated_Chain_Segment.png` | 1774 × 887 | Transparent generated source; includes intermediate alpha |

## Pose order

Both action sheets use the same left-to-right order: Idle, Run 1–6, Swipe windup, Swipe extension, Swipe recovery, Hurt. Frame 0 is the idle size reference. All frames face right. Code should draw the connecting chain separately, using the gauntlet ring and `Chain_Segment.png`.

## Prompt summary

Built-in `image_gen__imagegen` generated separate transparent pixel-art action strips for gray-blue Gōzu and gray-green Meizu, using the named Zabuza, Haku, and Kakashi images as style references. Prompts required distinct cloaks, a horned Mist forehead plate, two-filter respirator, one three-claw metal gauntlet on opposite hands, six running poses, three swipe poses, and a hurt pose. A third built-in generation produced a repeatable iron chain section. Generated poses were isolated, reduced to the 32 × 26 art-pixel grid, brightened, and given explicit art-pixel highlights for the forehead plate, horn, and respirator before 8× enlargement.

## Checks performed

- Inspected all three generated images and the final action sheets and chain visually.
- Confirmed 11 nonoverlapping frames per character, identical 256 × 208 cell dimensions, right-facing direction, and a common foot baseline.
- Confirmed every final frame has at least two transparent art pixels along all four cell edges; removed isolated generated specks, including one detached pixel in Meizu Run 4.
- Confirmed final images have binary alpha and constant RGBA within every 8 × 8 screen-pixel block.
- Confirmed the chain segment is 6 × 3 art pixels and its left/right edges match for horizontal tiling.

The request simultaneously specifies 24 art-pixel visible height and two empty art pixels at both top and bottom of a 26-pixel-high cell. Those dimensions are incompatible. The final frames use 22 visible art pixels, preserving the requested two-pixel clearance and fixed cell size.

## Remaining game-side checks

- Import the sheets and inspect actual in-game scale and visibility in rain and fog, especially the horn, respirator filters, and three claws.
- Play each run and swipe sequence to verify timing, loop continuity, hitbox alignment, and chain-ring placement.
- Confirm the chain tiles cleanly between both moving gauntlets in game.
