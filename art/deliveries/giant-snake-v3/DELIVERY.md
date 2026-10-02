# giant-snake-v3

- status: delivered
- request ID: giant-snake-v3

## Delivered files

| File | Dimensions | Alpha |
| --- | ---: | --- |
| `GiantSnake_Head.png` | 112×80 | RGBA, binary transparent/opaque |
| `GiantSnake_Body.png` | 56×64 | RGBA, binary transparent/opaque |
| `GiantSnake_Tail.png` | 64×48 | RGBA, binary transparent/opaque |
| `source/generated_GiantSnake_Head.png` | 1484×1060 | RGBA, partial alpha |
| `source/generated_GiantSnake_Body.png` | 1402×1122 | RGBA, partial alpha |
| `source/generated_GiantSnake_Tail.png` | 1448×1086 | RGBA, partial alpha |
| `preview.png` | 1600×1200 | RGB, opaque |

All paths are relative to `art/deliveries/giant-snake-v3/`.

## Prompt summary

Used the built-in `image_gen__imagegen` tool for three separate transparent pixel-art source images, with v2 parts as edit targets and anime Manda plus Tazuna sprite as visual references. Requested broad lavender head plates with dark seams and clean horns, a green vertical pupil, thick dark-brown body rings, a white side stripe, dark underside, continuous outlines, and hard color steps without noise or gradients. The final game-size sprites were sampled and manually cleaned on the pixel grid; the body ring and matching cut faces were reconstructed for repeatable assembly.

## Checks performed

- Inspected all three generated source images and final sprites visually.
- Confirmed game-size dimensions and right-facing head / left-pointing tail direction.
- Confirmed all game sprites have binary alpha, one connected visible silhouette each, and compact palettes (head 10, body 8, tail 11 RGBA colors including transparent).
- Inspected `preview.png` showing tail + 10 body sections + head at 1× and 2×, each part at 4×, v2 parts, and an anime reference thumbnail.
- Checked the eye, head plate seams, body stripe, ring thickness, dark underside, and tail stripe at game scale.

## Remaining game-side checks

- Load the three sprites in tModLoader and inspect rotation, overlap, attachment points, and animation against the actual NPC AI.
- Check readability against bright and dark Terraria backgrounds, especially the head-to-body and tail-to-body joins during fast movement.
