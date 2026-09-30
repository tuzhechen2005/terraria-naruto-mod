status: delivered
request ID: bg-iwa-v2

## Delivered files

| Path | Dimensions | Alpha |
| --- | ---: | --- |
| `art/deliveries/bg-iwa-v2/far.png` | 1024 × 435 | RGBA; transparent sky, binary 0/255 alpha, opaque bottom row |
| `art/deliveries/bg-iwa-v2/mid.png` | 1024 × 435 | RGBA; transparent sky, binary 0/255 alpha, opaque bottom row |
| `art/deliveries/bg-iwa-v2/close.png` | 1024 × 533 | RGBA; transparent upper area, binary 0/255 alpha, opaque bottom row |
| `art/deliveries/bg-iwa-v2/preview_stack.png` | 2048 × 533 | RGBA; fully opaque day preview on RGB (120, 170, 235) sky |
| `art/deliveries/bg-iwa-v2/preview_night.png` | 2048 × 533 | RGBA; fully opaque night-tinted preview |
| `art/deliveries/bg-iwa-v2/source_far.png` | 1983 × 793 | RGBA; original generated source with partial alpha |
| `art/deliveries/bg-iwa-v2/source_mid.png` | 2170 × 725 | RGBA; original generated source with partial alpha |
| `art/deliveries/bg-iwa-v2/source_close.png` | 2172 × 724 | RGBA; original generated source with partial alpha |

## Prompt summary and reference mapping

Generated three separate transparent Terraria-style parallax layers with the built-in image generation tool. Requested chunky pixels, flat limited gray-brown rock colors, atmospheric far depth, an unmistakable carved stone village, and low foreground rubble. Final game-sized images were downsampled to a 2 × 2 pixel grid, color-reduced, given hard alpha edges, and adjusted at both ends for exact horizontal edge continuity.

- `art/reference/villages/iwagakure.png`: The central tiered Tsuchikage tower is near the center of `mid.png`, surrounded by round stone dwellings built into rock columns. Suspended bridges cross gaps on both sides of the tower. The encircling high rock formations appear in `far.png` and behind the village in `mid.png`.
- `art/reference/villages/land_of_earth.png`: Its dense field of narrow pointed stone pillars and layered ridges defines `far.png`. Rough exposed earth and boulders carry into `close.png`.
- `Background_7.png` and `Background_8.png`: Used for layered, opaque-at-bottom mountain silhouettes and restrained distance colors.
- `Background_9.png` and `Background_55.png`: Used for transparent upper areas, solid lower terrain, readable repeat rhythm, and crisp foreground edges.

## Checks performed

- Opened and visually inspected all six named reference images and all three generated sources.
- Inspected the final day and night doubled-width, bottom-aligned stacked previews. The central tower and several bridges remain visible above the close layer; far peaks appear behind the city.
- Verified exact game dimensions and alpha channels. Final game PNGs contain only alpha 0 or 255; all bottom rows are fully opaque.
- Verified the first and last columns of each game PNG match pixel for pixel, including alpha, for horizontal tiling.
- Verified the close layer's highest opaque pixel begins at y = 326 of 533, leaving the top 61% transparent. Its ridge stays within the lower 40%.
- Visually inspected game-sized pixel silhouette and seam in the twice-tiled previews.

## Remaining game-side checks

- Load the three layers in tModLoader and inspect parallax scroll, tiling seams, placement against actual terrain, and visibility of the tower across display resolutions and zoom settings.
- Check day/night tint as applied by the game and confirm the game's own sky and sun/moon remain visible through transparent areas.
