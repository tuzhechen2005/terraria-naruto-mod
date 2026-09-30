status: delivered
request ID: bg-konoha-v2

# Delivered files

| File | Dimensions | Alpha |
| --- | ---: | --- |
| `far.png` | 1024×435 | RGBA; transparent sky, opaque bottom; binary alpha |
| `mid.png` | 1024×435 | RGBA; transparent above roofs, opaque bottom; binary alpha |
| `close.png` | 1024×533 | RGBA; transparent above foliage, opaque bottom quarter; binary alpha |
| `far_source.png` | 2125×740 | RGBA; generated source |
| `mid_source.png` | 1774×887 | RGBA; generated source |
| `close_source.png` | 1774×887 | RGBA; generated source |
| `preview_stack.png` | 2048×533 | Opaque RGB day preview on `(120,170,235)` sky; two horizontal repeats |
| `preview_stack_night.png` | 2048×533 | Opaque RGB night-color preview; two horizontal repeats |

# Prompt summary and reference mapping

Built-in `image_gen__imagegen` produced three transparent Terraria-style pixel-art layers. The game-size files use nearest-neighbor scaling, hard binary alpha, and exact matching first/last pixel columns. The original generated PNGs are preserved as `_source` files.

- `art/reference/villages/hokage_rock.png`: `far.png` centers one ochre Hokage cliff. Four carved portraits run left to right: long straight hair, short spiky hair with cheek marks, elderly face under a broad hat, young short spiky hair. A forest and small buildings line the crest; blue-green hills meet the cliff at both sides. The four faces remain visible in both stacked previews.
- `art/reference/villages/konoha.png`: `mid.png` has dense small village houses under varied red, orange, blue, green, and beige roofs, plus cylindrical roof tanks. The village panorama sits below the carved faces in the stack.
- `art/reference/villages/hokage_tower.png`: the largest midground building is center-right, formed from linked round red-orange volumes with flat circular roofs. The white circular plaque on its upper facade bears the red `火` character, readable at game scale. The plaque is on the upper facade rather than the roof surface.
- `art/reference/terraria/bg/Background_7.png`, `Background_8.png`, `Background_9.png`, and `Background_55.png`: all three layers use transparent upper silhouettes, restricted color ramps, flat shapes, and crisp pixel edges. The close layer uses edge trees and a grass/earth band, leaving the central tower exposed.

# Checks performed

- Visually inspected all seven named reference images, all three generated sources, and the day/night stacked previews.
- Confirmed game dimensions, RGBA alpha with only values 0 and 255, opaque bottom rows, and matching left/right edge columns for every game layer.
- Checked the four face silhouettes and the `火` sign in the 2048×533 two-repeat previews; neither is covered by close foliage. Day and darkened night previews are included.
- No code, request file, unrelated art, or Git state was modified; no commit was made.

# Remaining game-side checks

- Integrate the three layers into the tModLoader background style and check actual parallax offsets, tiling, draw order, resolution scaling, and day/night lighting in game. The generated night preview is only a color simulation, not an in-game acceptance test.
