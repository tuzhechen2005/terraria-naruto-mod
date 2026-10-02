status: delivered
request ID: gaara-tween-v1

## Delivered files

- `Gaara_Walk_0.png` through `Gaara_Walk_7.png` — 8 walking frames.
- `Gaara_Idle_0.png` through `Gaara_Idle_5.png` — 6 breathing / drifting sand frames.
- `Gaara_CastIn_0.png`, `Gaara_WaveIn_0.png`, `Gaara_ShieldIn_0.png` — 3 action lead-in frames.
- `preview.png` — each action on one row, starting with the existing idle reference.
- `preview_idle_walk_idle.png`, `preview_idle_cast_idle.png`, `preview_idle_wave_idle.png`, `preview_idle_shield_idle.png` — sequence strips. Existing attack frames appear only in the previews as comparison.
- `source/generated_Gaara_tween_concept.png` — built-in imagegen conceptual source sheet, copied from `$CODEX_HOME/generated_images`.
- `source/Gaara_Idle_0_pixel_reference.png` — enlarged pixel reference used for visual inspection.

## Dimensions and alpha

- All 17 game frames: 112 × 88 PNG, RGBA, alpha values only 0 and 255. Every frame has head starting at y=20 and feet ending at y=83, faces right, and uses only colors found in the existing `Gaara_Idle_0.png`.
- Generated conceptual source: 1983 × 793 PNG, RGBA with partial alpha. It is **not** a game-ready sprite.
- The enlarged pixel reference and previews use opaque display backgrounds.

## Prompt summary

Built-in `image_gen__imagegen` generated a transparent conceptual sheet of ordinary-form Gaara with eight folded-arm walk poses, six quiet idle poses, and three action lead-ins. The existing Gaara idle sprite supplied the identity; Tazuna's delivered comparison image supplied the pixel-art style reference. The final game frames were pixel-refined from the existing idle sprite to retain his exact face, colors, size, and direction. The conceptual sheet was not sliced into the game frames because its semi-transparent edges and altered proportions did not meet the request.

## Checks performed

- Visually inspected the existing idle and attack references, the generated conceptual source, the pixel reference, and the preview strips.
- Verified all 17 requested names exist and every game frame is 112 × 88, alpha 0/255, and restricted to the existing idle palette.
- Verified every frame shares the same top and foot baseline; walked the sequence next to the original idle sprite in previews.
- Measured adjacent-frame changes across the walk and idle loops. Walk transitions change 162–201 pixels; idle transitions change 14–33 pixels, including the loop seam.

## Remaining game-side checks

- Integrate the frames into the NPC animation logic and inspect timing, direction, attack lead-ins, sand particle overlap, and apparent foot sliding in Terraria at native scale. No game runtime or mod build was run by this art worker.
