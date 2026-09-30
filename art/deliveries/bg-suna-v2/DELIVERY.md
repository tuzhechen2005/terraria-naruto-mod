# bg-suna-v2 delivery

status: delivered
request ID: bg-suna-v2

## Delivered files

| File | Dimensions | Alpha |
| --- | ---: | --- |
| `far.png` | 1024×435 | RGBA; transparent sky, opaque bottom; binary alpha |
| `mid.png` | 1024×435 | RGBA; transparent sky, opaque bottom; binary alpha |
| `close.png` | 1024×533 | RGBA; transparent upper area, opaque bottom; binary alpha |
| `preview_stack.png` | 2048×533 | RGB; opaque day preview, two horizontal tiles |
| `preview_stack_night.png` | 2048×533 | RGB; opaque night color preview, two horizontal tiles |
| `far_source.png` | 1926×817 | RGBA; selected generated source |
| `mid_source.png` | 1923×817 | RGBA; selected generated source |
| `close_source.png` | 2172×724 | RGBA; selected generated source |

All paths are under `art/deliveries/bg-suna-v2/`.

## Prompt summary

Generated three separate transparent pixel-art parallax sources with the built-in image generation tool. The far layer emphasizes a broad ring of dusty ochre canyon walls and a narrow central cleft. The mid layer emphasizes dense cylindrical sand buildings and a large central Kazekage dome with a circular `風` emblem. The close layer contains only low dunes and eroded rocks. The palette and hard-edged silhouettes follow the named Terraria background references. Selected source images were copied from the built-in tool output, then resized with nearest-neighbor sampling to the required game dimensions; partial alpha fringes were removed.

## Reference elements and placement

- `sunagakure.png`: the enclosing, stepped sandstone cliffs occupy `far.png` on both sides; the narrow canyon entrance is the cleft near its center. The crowded village inside the ring occupies `mid.png`.
- `kazekage.png`: the round, tiered Kazekage building dominates the center of `mid.png`. Its front circular plaque contains `風`; the narrow cylindrical turret stands immediately to its right. Smaller round-roof cylindrical homes, window bands, platforms, and pipes flank it.
- `Background_7.png` and `Background_8.png`: the far layer uses broad low-saturation planes and a hard sky silhouette. `Background_9.png` and `Background_55.png`: the middle and close layers use clear transparent upper areas and a solid bottom suitable for parallax stacking.

## Checks performed

- Opened both Naruto references and all four named Terraria background references before generation.
- Visually inspected all three generated sources, the daylight two-tile stack, and the night two-tile stack.
- Verified exact game-size dimensions. All three layers have only alpha values 0 and 255, transparent top rows, and fully opaque bottom rows.
- Verified the outermost left and right pixel columns match in every row. In the stacked preview, the main `風` plaque stays visible above the low foreground.
- The close layer's highest visible pixel is at y=400 of 533, keeping all foreground shapes within the bottom 25% of the image.

## Remaining game-side checks

- Integrate the three game-size layers into the tModLoader background path and inspect actual parallax, tile joins, lighting, and scale in a desert biome at day and night. The night preview is a color simulation; game rendering has not been tested by this art worker.
