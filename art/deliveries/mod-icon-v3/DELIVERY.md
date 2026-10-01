status: delivered
request ID: mod-icon-v3

## Delivered files

| File | Dimensions | Alpha |
| --- | --- | --- |
| `source/generated-icon.png` | 1254×1254 | None; opaque RGB |
| `icon.png` | 80×80 | None; opaque RGB |
| `icon_small.png` | 30×30 | None; opaque RGB |
| `readme-icon.png` | 320×320 | None; opaque RGB |
| `preview.png` | 960×760 | None; opaque RGB |

## Prompt summary

Generated with the built-in `image_gen__imagegen` tool: an original square Terraria pixel badge with one large, connected orange-gold spiral leaf glyph, dark outline, upper-left highlights, navy-black opaque backing, thin copper-gold pixel frame, and a few blocky sparks. Asked for strong readability at 30 pixels and excluded text, forehead protector, kunai, characters, scenery, blur, glow, and 3D rendering.

## Pixel processing and checks

- Copied the generated PNG from `$CODEX_HOME/generated_images` into `source/`.
- Independently sampled the source to 80×80 and 30×30 with Lanczos, then reduced each to a fixed palette without dithering (28 and 20 colors respectively). The small icon was sampled from the source, not smoothed from the 80×80 icon.
- Enlarged the 80×80 icon four times with nearest-neighbor sampling for the README.
- Inspected the generated source and `preview.png` visually at both original sizes and nearest-neighbor enlargements against white and GitHub dark backgrounds. The spiral, attached line, pointed upper-right leaf tip, dark silhouette, and frame remain readable; no stray text or broken outline was seen.
- Verified PNG dimensions and confirmed all files are opaque RGB without alpha.

## Remaining game-side checks

- Integrate `icon.png` and `icon_small.png` in tModLoader and inspect the mod list and Workshop presentation in game.
- Check `readme-icon.png` in both English and Chinese README layouts after integration.
