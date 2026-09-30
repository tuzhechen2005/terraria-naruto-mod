# wave-corpses-v1

- status: delivered
- request ID: `wave-corpses-v1`
- delivered files:
  - `source/Zabuza_Lying.png` — 384 × 112 RGBA (48 × 14 art pixels); head right, reaching arm right; transparent background.
  - `source/Haku_Lying.png` — 352 × 96 RGBA (44 × 12 art pixels); head right, broken mask fragments beside head; transparent background.

## Prompt summary

Built-in image generation used the named Zabuza and Haku project sprites as pixel-style references and the named anime images as character references. Both sprites show a low, horizontal defeated pose on transparent canvas. Zabuza has an exposed face, wrapped bare torso, kunai in his back, and no sword. Haku has a visible peaceful face, loose hair, teal clothing, folded hands, and broken mask pieces. No blood was requested or added. Generated images were reduced to the requested logical sprite sizes and enlarged by exact 8× nearest-neighbor scaling.

## Checks performed

- Visually inspected the generated images and final game-size PNGs for pose, head direction, character details, and readability at sprite scale.
- Verified exact canvas dimensions, RGBA output, alpha values of only 0 and 255, uniform 8 × 8 color blocks, and opaque pixels touching the bottom row.
- No source code, builds, or in-game checks were run for this art-only delivery.

## Remaining game-side checks

- Integrate the sprites, then confirm draw origin, bridge/ground contact, facing, visual scale beside the active Boss frames, and legibility during the ending cutscene in tModLoader.
