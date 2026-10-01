# giant-snake-v2 delivery

- status: delivered
- request ID: `giant-snake-v2`
- method: built-in `image_gen__imagegen`, followed by nearest-neighbor pixel-grid sampling and manual eye/edge cleanup.

## Delivered files

| Path | Dimensions | Alpha |
| --- | ---: | --- |
| `GiantSnake_Head.png` | 112 × 80 | Transparent; 0/255 only |
| `GiantSnake_Body.png` | 56 × 64 | Transparent; 0/255 only |
| `GiantSnake_Tail.png` | 64 × 48 | Transparent; 0/255 only |
| `preview.png` | 1840 × 940 | Opaque inspection background |
| `source/generated_GiantSnake_Head.png` | 1484 × 1060 | Transparent; graded source alpha |
| `source/generated_GiantSnake_Body.png` | 1659 × 948 | Transparent; graded source alpha |
| `source/generated_GiantSnake_Tail.png` | 1448 × 1086 | Transparent; graded source alpha |

## Prompt summary

Three separate transparent pixel-art sources of Manda: a right-facing lavender armored head with backward horns, green slit eye and fangs; a repeatable horizontal body section with one dark transverse ring, white side line and dark underside; and a left-pointing tapered tail with two rings. Dark outlines, stepped color bands and upper-left light follow the named Tazuna style reference. No beige belly or spotted python pattern.

## Checks performed

- Opened all three named Manda references, the Tazuna style comparison and the v1 assembly preview.
- Inspected the generated outputs and the final 1×/2× assembly plus 4× individual parts beside a Manda reference thumbnail in `preview.png`.
- Verified sprite dimensions, transparent binary alpha and uniform 2×2 game-pixel blocks.
- Verified the body's left and right edge pixel columns are identical, including alpha, for repetition.
- Restored a readable green slit eye after palette reduction; checked head faces right and tail tip faces left.

## Remaining game-side checks

- Replace the three game sprites and inspect the actual sine-wave rotation, overlap and connection at runtime.
- Check readability and alignment under the game's lighting and animation at normal zoom. No in-game test was run by the art worker.
