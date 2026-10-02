status: delivered
request ID: orochimaru-fx-wind-v1

## Delivered files

- `FxWind_0.png` — 96×80 RGBA, binary alpha (0/255)
- `FxWind_1.png` — 96×80 RGBA, binary alpha (0/255)
- `FxWind_2.png` — 96×80 RGBA, binary alpha (0/255)
- `FxWind_3.png` — 96×80 RGBA, binary alpha (0/255)
- `source/FxWind_generated_sheet.png` — 2172×724 RGBA source image, variable alpha; copied byte-for-byte from the built-in image generator's output
- `preview.png` — 1616×856 RGB; all four frames shown at 4× and 1× on dark and light backgrounds

All paths above are relative to `art/deliveries/orochimaru-fx-wind-v1/`.

## Prompt summary

Built-in `image_gen__imagegen` with transparent output. Four horizontally arranged pixel-art frames of Orochimaru's rightward wind attack: a narrow mouth-side origin widening into curled white, pale cyan-green, and gray-green air ribbons, with green leaves and brown dirt clods. Requested hard color steps, dark outlines, upper-left lighting, and restrained purple-black accents. Used the named Tazuna, Orochimaru, and Zabuza projectile images as visual references.

## Checks performed

- Inspected the generated source and the final 1×/4× preview on both backgrounds.
- Sampled each frame to a 2×2 screen-pixel art grid, reduced colors to hard steps, cleaned isolated pixels, and adjusted frame 0's height for smoother looping.
- Verified all four game PNGs are 96×80, use only alpha 0/255, and have uniform 2×2 blocks.
- Verified the copied source matches the generated PNG byte-for-byte.
- Visually checked direction, narrow-to-wide silhouette, leaf and dirt visibility, and clarity at game scale.

## Remaining game-side checks

- Load the four frames in tModLoader and inspect animation continuity, draw origin, projectile direction, scale, forward motion, and visibility against actual terrain and boss attacks. No in-game check was performed by the art worker.
