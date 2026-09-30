# bg-ame-v2 delivery

- status: delivered
- request ID: `bg-ame-v2`
- destination: `art/deliveries/bg-ame-v2/`

## Delivered files

| File | Dimensions | Alpha |
| --- | ---: | --- |
| `far.png` | 1024 × 435 | Binary alpha; transparent sky, opaque bottom |
| `mid.png` | 1024 × 435 | Binary alpha; transparent sky, opaque bottom |
| `close.png` | 1024 × 533 | Binary alpha; transparent sky, opaque bottom 25% |
| `preview_stack.png` | 2048 × 533 | Opaque daytime composite on RGB (120, 170, 235) |
| `preview_night.png` | 2048 × 533 | Opaque night-color composite |
| `source_far.png` | 1923 × 817 | Generated RGBA source |
| `source_mid.png` | 1923 × 817 | Generated RGBA source |
| `source_close.png` | 2158 × 729 | Generated RGBA source |

## Prompt and reference mapping

- `amegakure.png`: The forest of tall, pointed gray metal buildings becomes the muted `far.png` skyline. Smaller pipe-covered towers also frame the landmark in `mid.png`. The wet industrial mood informs the pipes, railings, and puddle highlights in `close.png`.
- `pain_tower.png`: The tallest central building in `mid.png` carries thick bundled vertical pipes, stacked platforms, and two large monster heads with open mouths and protruding reddish tongues on its upper sides.
- Terraria `Background_7.png` and `Background_8.png`: Used for receding cool colors and broad flat background shapes. `Background_9.png` and `Background_55.png`: Used for hard pixel silhouettes, sparse color steps, and transparent sky above an opaque base.
- The generation prompts requested isolated transparent Terraria-style pixel-art layers, a recognizable Rain Village skyline and Pain's Tower, low foreground pipes, limited cool palettes, and repeatable horizontal edges. The game supplies the sky and clouds behind these layers.

## Checks performed

- Inspected all six named reference images, all three generated sources, the final layers, and both two-tile stacked previews.
- Resized and reduced each game layer to a blocky 2 × 2 pixel scale, used a limited palette, and converted the game-layer alpha to hard 0/255 edges.
- Confirmed exact dimensions, transparent top rows, fully opaque bottom rows, and matching first/last pixel columns on every game layer (zero mismatched rows).
- `close.png` has no visible pixels above y = 320, so its content stays within the bottom 40% of its 533-pixel canvas. Its bottom 133 rows are opaque.
- Day and night previews each place far, mid, and close bottom-aligned, repeating the full stack twice across 2048 pixels. The landmark and distant spires remain visible above the foreground.

## Remaining game-side checks

- Integrate the three layers in the Rain Village background style and inspect parallax tiling in motion, including wrap seams at different scroll offsets.
- Verify daytime, nighttime, rain, zoom levels, and actual game scale in tModLoader. This delivery has not been tested in-game.
