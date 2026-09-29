# Delivery: style-test-v5-haku

- status: delivered
- request ID: `style-test-v5-haku`
- delivered files:
  - `source/Haku_style.png` — 240 × 304 px (30 × 38 logical pixels at 8×); RGBA, transparent background; character pixels alpha 255, exterior alpha 0.
  - `Haku_style_final.png` — 30 × 38 px (1× logical game-size sprite); RGBA, transparent background; character pixels alpha 255, exterior alpha 0.
- prompt summary: Full-body, right-facing idle Haku sprite, using C-version Zabuza for proportions and pixel-art style, Haku references for costume and features; white hunter-nin mask with broad red markings, dark hair with two bangs, teal pin, teal kimono and ivory collar, dark sleeves, olive-brown hakama and belt end, wooden geta, and a senbon. Transparent background, limited palette, hard pixel edges, no anti-aliasing.
- checks performed: inspected all named references and generated image; confirmed 38-pixel logical height and 8×8 source blocks; confirmed source/final dimensions; confirmed alpha is binary (0 or 255) with no semitransparent character pixels; confirmed mask red marking is at least 2 logical pixels wide; inspected final sprite enlarged with nearest-neighbor for silhouette and feature readability.
- remaining game-side checks: load the sprite in Terraria/tModLoader and compare it side-by-side with C-version Zabuza at in-game scale; verify facing direction and readability in the intended game context.
