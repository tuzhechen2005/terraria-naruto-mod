# Giant Snake v1 delivery

- status: delivered
- request ID: `giant-snake-v1`

## Delivered files

| Path | Dimensions | Alpha |
| --- | ---: | --- |
| `GiantSnake_Head.png` | 112 × 80 | Transparent; alpha 0 or 255 only |
| `GiantSnake_Body.png` | 56 × 64 | Transparent; alpha 0 or 255 only |
| `GiantSnake_Tail.png` | 64 × 48 | Transparent; alpha 0 or 255 only |
| `preview.png` | 1530 × 1110 | Opaque preview |
| `source/generated_GiantSnake_Head.png` | 1484 × 1060 | Transparent generated original; antialiased alpha |
| `source/generated_GiantSnake_Body.png` | 1659 × 948 | Transparent generated original; antialiased alpha |
| `source/generated_GiantSnake_Tail.png` | 1448 × 1086 | Transparent generated original; antialiased alpha |

## Prompt summary

The built-in `image_gen__imagegen` tool produced three separate transparent pixel-art sources: a right-facing open-mouthed giant serpent head, a repeating purple-brown and beige body, and a tail with its tip to the left and connection on the right. Prompts specified dark scale markings, pale segmented belly scales, a gold vertical-pupil eye, white fangs, red forked tongue, thick dark outline, and hard upper-left-lit color steps based on the Tazuna delivery reference. The sources were sampled to an art-pixel grid and enlarged 2× with nearest-neighbor scaling. The final sprites use reduced hard-shaded palettes. The body seam columns and the head's neck edge were adjusted to align.

## Checks performed

- Inspected all three generated originals, all three final sprites, and `preview.png` visually.
- Confirmed exact requested final dimensions, binary alpha, and identical colors within every 2 × 2 screen-pixel block.
- Confirmed the body segment's left and right edge columns are pixel-identical, including transparency, for repeated tiling.
- Inspected a ten-body-segment wave assembly at game size and 2×, plus each isolated piece at 4×. The head points right; its gold eye, fangs, mouth, and forked tongue remain visible. The tail tip points left.

## Remaining game-side checks

- Import the three final sprites into tModLoader and check draw origins, rotation, per-segment overlap, and seam behavior across the full animation range. The preview covers one shallow wave only.
- Check the assembled snake's size, motion, and readability against the actual Death Forest arena and at different lighting levels.
- Confirm the head, body, and tail alignment with the implementation's chosen segment count, scaling, and collision geometry.
