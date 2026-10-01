status: delivered
request ID: wave-epilogue-walk-v2

# Delivery

## Files and image properties

### Zabuza

- `art/deliveries/wave-epilogue-walk-v2/Zabuza_Rise_0.png` — 288×128, RGBA, alpha 0/255.
- `art/deliveries/wave-epilogue-walk-v2/Zabuza_Rise_1.png` — 288×128, RGBA, alpha 0/255.
- `art/deliveries/wave-epilogue-walk-v2/Zabuza_Rise_2.png` — 288×128, RGBA, alpha 0/255.
- `art/deliveries/wave-epilogue-walk-v2/Zabuza_Stagger_0.png` — 288×128, RGBA, alpha 0/255.
- `art/deliveries/wave-epilogue-walk-v2/Zabuza_Stagger_1.png` — 288×128, RGBA, alpha 0/255.
- `art/deliveries/wave-epilogue-walk-v2/Zabuza_Stagger_2.png` — 288×128, RGBA, alpha 0/255.
- `art/deliveries/wave-epilogue-walk-v2/Zabuza_Stagger_3.png` — 288×128, RGBA, alpha 0/255.
- `art/deliveries/wave-epilogue-walk-v2/Zabuza_Collapse_0.png` — 288×128, RGBA, alpha 0/255.
- `art/deliveries/wave-epilogue-walk-v2/Zabuza_Collapse_1.png` — 288×128, RGBA, alpha 0/255.
- `art/deliveries/wave-epilogue-walk-v2/Zabuza_Collapse_2.png` — 288×128, RGBA, alpha 0/255.

### Haku

- `art/deliveries/wave-epilogue-walk-v2/Haku_Rise_0.png` — 144×96, RGBA, alpha 0/255.
- `art/deliveries/wave-epilogue-walk-v2/Haku_Rise_1.png` — 144×96, RGBA, alpha 0/255.
- `art/deliveries/wave-epilogue-walk-v2/Haku_Rise_2.png` — 144×96, RGBA, alpha 0/255.
- `art/deliveries/wave-epilogue-walk-v2/Haku_Stagger_0.png` — 144×96, RGBA, alpha 0/255.
- `art/deliveries/wave-epilogue-walk-v2/Haku_Stagger_1.png` — 144×96, RGBA, alpha 0/255.
- `art/deliveries/wave-epilogue-walk-v2/Haku_Stagger_2.png` — 144×96, RGBA, alpha 0/255.
- `art/deliveries/wave-epilogue-walk-v2/Haku_Stagger_3.png` — 144×96, RGBA, alpha 0/255.
- `art/deliveries/wave-epilogue-walk-v2/Haku_Collapse_0.png` — 144×96, RGBA, alpha 0/255.
- `art/deliveries/wave-epilogue-walk-v2/Haku_Collapse_1.png` — 144×96, RGBA, alpha 0/255.
- `art/deliveries/wave-epilogue-walk-v2/Haku_Collapse_2.png` — 144×96, RGBA, alpha 0/255.

- `art/deliveries/wave-epilogue-walk-v2/Zabuza_source.png` — 1983×793, RGBA with graded alpha.
- `art/deliveries/wave-epilogue-walk-v2/Haku_source.png` — 1983×793, RGBA with graded alpha.
- `art/deliveries/wave-epilogue-walk-v2/preview.png` — 3168×576, opaque RGB.
- `art/deliveries/wave-epilogue-walk-v2/alignment.png` — 3168×500, opaque RGB.

## Prompt summary

Generated Zabuza and Haku right-facing pixel animation pose sheets with the named combat frames as visual references and the v1 preview as pose reference. Zabuza is unarmed, injured, forward-leaning, with both arms hanging; Haku is unmasked, injured, forward-leaning, with a hand at the chest. The generated sheets were cut into Rise (3), Stagger (4), and Collapse (3) frames per character, reduced to the game art-pixel grid, hard-quantized to colors found in the named combat reference sprites, and aligned on the requested canvases.

## Checks performed

- Visually inspected both generated source sheets and the final combat-frame comparison on dark and light backgrounds.
- Checked all 20 game frames for exact dimensions (Zabuza 288×128; Haku 144×96), right-facing silhouette, and readability at native size.
- Programmatically verified each frame has only alpha values 0 and 255, identical colors in each global 2×2 block, and opaque colors drawn only from its named combat reference sprites.
- Programmatically verified the last occupied foot row is y=123 for Zabuza and y=91 for Haku; inspected centerline and baseline in `alignment.png`, including all-frame overlays.

## Remaining game-side checks

- Import the 20 frames into the mod animation and inspect the Rise → Stagger loop → Collapse transition in Terraria at the intended camera zoom.
- Confirm feet stay on the encounter ground and inspect silhouette readability against actual arena backgrounds and effects.
- No game build or in-game playback was run by the art worker.
