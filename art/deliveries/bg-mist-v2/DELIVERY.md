# bg-mist-v2 delivery

- status: delivered
- request ID: `bg-mist-v2`
- generation: built-in `image_gen__imagegen`; selected outputs copied from `$CODEX_HOME/generated_images` and prepared at game dimensions.

## Delivered files

| File | Dimensions | Alpha |
| --- | --- | --- |
| `art/deliveries/bg-mist-v2/far.png` | 1024 × 435 | RGBA; transparent sky, opaque bottom |
| `art/deliveries/bg-mist-v2/mid.png` | 1024 × 435 | RGBA; transparent sky, opaque bottom |
| `art/deliveries/bg-mist-v2/close.png` | 1024 × 533 | RGBA; transparent sky, opaque bottom |
| `art/deliveries/bg-mist-v2/preview_stack.png` | 2048 × 533 | RGB; day sky (120, 170, 235) |
| `art/deliveries/bg-mist-v2/preview_stack_night.png` | 2048 × 533 | RGB; night-color preview |

## Prompt summary and reference placement

- `kirigakure.png`: Far layer has tall cylindrical buildings with rows of small windows and rooftop greenery. The broad, purple-roofed Mizukage building and circular emblem sit near the center, above the bridge line in the combined preview.
- `land_of_water.png`: Far layer uses mist-veiled gray-blue islands and distant rock formations behind the towers.
- `land_of_waves.png`: Mid layer places a small cluster of simple tile-roof stilt houses on the right-hand shore.
- `naruto_bridge.png`: Mid layer places a very long, low, straight bridge with regularly spaced piers across the sea; it is not an arch bridge.
- Terraria `Background_7.png`, `Background_8.png`, `Background_9.png`, and `Background_55.png`: Used for layered atmospheric depth, flat stepped colors, crisp silhouettes, and restrained pixel-art texture. Close layer adds shore rocks, reeds, a short wood dock, and a fishing boat low in the frame.

All three prompts requested true transparent sky, seamless horizontal repetition, a limited cool palette, and no lettering other than the possible Mizukage emblem. The generated sources were scaled to target dimensions, their alpha was reduced to hard transparent/opaque edges, and the outer edge regions were matched for tiling.

## Checks performed

- Opened all four named Naruto reference images and four named Terraria background references before generation.
- Inspected all three generated source images and both final two-tile stack previews visually. The tower cluster, central building, flat bridge, piers, shore huts, boat, dock, rocks, and reeds remain readable in the combined preview.
- Checked exact requested layer dimensions. Each layer contains only alpha values 0 and 255; its top row is transparent and its bottom row is opaque.
- Compared the first and last pixel columns of every layer: zero mismatched pixels. Preview files place two copies side by side with bottom-aligned layers.
- The night preview uses a dark sky and dimmed layer colors for visibility review; it is not a separate game asset.

## Remaining game-side checks

- Integrate the three layers in tModLoader and verify actual parallax offsets, horizon placement, bridge/building visibility, and scrolling seams at several zoom levels and day/night lighting states.
- Confirm the game's draw order and any runtime tinting preserve transparent sky and the intended relative layer saturation.
- 2026-09-30 Claude 接入前修正：mid.png 最左、最右两列的竖线（平铺时成为接缝）去掉；右侧小岛在 x≈880 被竖直截断，削成斜坡到桥面。原图保留为 mid_original.png。
