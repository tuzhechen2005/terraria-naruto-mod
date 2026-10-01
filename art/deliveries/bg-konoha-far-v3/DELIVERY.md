status: delivered
request ID: bg-konoha-far-v3

## Delivered files

| File | Dimensions | Alpha |
| --- | --- | --- |
| `art/deliveries/bg-konoha-far-v3/far_source.png` | 1923×817 | RGBA; generated source with partial transparency |
| `art/deliveries/bg-konoha-far-v3/far.png` | 1024×435 | RGBA; transparent sky, binary alpha (0/255) |
| `art/deliveries/bg-konoha-far-v3/preview_stack.png` | 2048×533 | RGB; blue sky (120,170,235) behind two horizontally tiled copies of far, mid, close, with all layer bottoms aligned |
| `art/deliveries/bg-konoha-far-v3/faces_comparison.png` | 3485×792 | RGB; 4× nearest-neighbor enlargement of the four faces beside the supplied close-up |

## Prompt summary

Built-in `image_gen__imagegen` generated a new wide Terraria-style pixel-art far layer using `four_hokage_faces_closeup.png`, `konoha_part1_rock_far.png`, and `Background_8.png` as visual references. The prompt specified one ochre rock wall, exactly four distinct stone-carved Hokage faces, transparent sky, forest on the rim, distant blue-green mountains at the sides, and irregular forest fading at the bottom. It excluded a hat on the third face, additional faces, modern buildings, flesh-colored portraits, and text. The source was resized with nearest-neighbor sampling to game size. The game image has hard alpha edges and matched left/right border pixels for horizontal tiling.

## Face reference checks

- First / Hashirama: checked against `four_hokage_faces_closeup.png` and `Hashirama_Senju.png`; broad calm face, rounded headband-like crown and long side hair.
- Second / Tobirama: checked against `four_hokage_faces_closeup.png` and `Tobirama_Senju.png`; narrower stern face, horizontal band and upright pointed hair.
- Third / Hiruzen: checked against `four_hokage_faces_closeup.png` and `Hiruzen_Sarutobi.png`; strong brow, swept-up hair, visible chin goatee, no hat.
- Fourth / Minato: checked against `four_hokage_faces_closeup.png` and `Minato_Namikaze.png`; young narrow face and radiating spiky hair with bangs.

## Checks performed

- Opened every image reference named in the request, including both Terraria backgrounds and the existing mid/close layers.
- Inspected the generated source, the 1024×435 game image, the face comparison, and the two-tile stacked preview visually.
- Confirmed the final game image has only alpha values 0 and 255, avoiding semi-transparent fringes.
- Confirmed first and last pixels match for all 435 rows; the repeated image shows no hard vertical edge.
- Confirmed four faces, stone-colored relief, forest rim, transparent sky, and no high-rise buildings.

## Remaining game-side checks

- **Important composition finding:** In the specified bottom-aligned `preview_stack.png`, the existing `KonohaMid.png` buildings cover most of the sculpted faces. Verify the actual parallax offsets and visibility in-game; a layer-position or composition change may be needed if the faces must remain fully visible during play.
- Load `far.png` in tModLoader and check daylight/night tint, camera movement, horizontal repeat, and scale. No build or in-game acceptance was run by this art worker.
