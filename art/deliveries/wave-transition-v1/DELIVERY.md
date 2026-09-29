# Delivery: wave-transition-v1

- status: delivered
- request ID: wave-transition-v1

## Delivered files

- `source/wave_transition_sprite_sheet.png` — 1536 × 1024 PNG (3 × 3 pose sheet)

## Image details

- RGBA with binary alpha (0 or 255); transparent background is preserved.
- Nine poses arranged left-to-right, top-to-bottom: two Zabuza kneel poses, three Zabuza rise/roar poses, then four Haku emergence poses.
- The original generated sheet included soft alpha around ice and sprite edges; alpha was thresholded at 128 to keep the background transparent and make surviving pixels opaque.

## Prompt summary

Generated an isolated 3 × 3 pixel-art sprite sheet following the named Zabuza and Haku game-frame references and IceMirror design. The sequence depicts Zabuza kneeling and roaring, followed by Haku emerging from an upright blue ice mirror. Anime reference images informed mood only. No purple aura was added.

## Checks performed

- Opened and visually inspected the named Zabuza and Haku idle frames, Zabuza source sheet, IceMirror frame, and the three named anime mood references.
- Inspected the generated sheet and the delivered PNG visually.
- Verified PNG dimensions, RGBA mode, binary alpha, and transparent background sample pixels.
- Confirmed the sheet contains nine separated poses in the requested order. The output is source art only; exact per-pose dimensions were not authored here.

## Remaining game-side checks

- Slice and align the nine poses to the requested 224 × 112 Zabuza and 112 × 88 Haku frames, including centerline and foot-baseline alignment.
- Compare at 1× over light and dark backgrounds and against the in-game base frames; verify silhouette, edge pixels, and kneel-to-roar continuity.
- Confirm the Haku emergence frost reads correctly after frame slicing and that Zabuza's purple aura is added by code as intended.
- Perform in-game review after integration.
