status: delivered
request ID: kakashi-trapped-hires-v2

## Delivered files

- `Kakashi_Trapped_0.png` — 80×80 RGBA, alpha values 0/255 only.
- `Kakashi_Trapped_1.png` — 80×80 RGBA, alpha values 0/255 only.
- `Kakashi_Trapped_Generated_Source.png` — 1254×1254 RGBA built-in image-generation source, with partial alpha.
- `preview.png` — 1050×525 opaque comparison of both frames at 1× and 4× beside accepted idle art.
- `head_check_8x.png` — 900×300 opaque comparison of the accepted head and both new heads at 8×.

All paths are relative to this delivery directory.

## Prompt summary

Used the built-in image-generation tool with the v1 trapped pose and accepted `Kakashi_00_Idle.png` as visual references: curled body, forward pushing glove with separated fingers, half-bent legs, lifted headband cloth, transparent background, and accepted palette. Used that result as an action reference. The game-size frames were pixel-composited from the v1 pose and exact accepted idle-head pixels to preserve the already-approved face, eye, mask, and forehead plate.

## Checks performed

- Visually inspected both game-size sprites, the generated source, `preview.png`, and `head_check_8x.png`; pushing hand, floating pose, left-to-right facing, and 1× readability checked.
- Verified both sprites are 80×80, centered within transparent margins (occupied bounds: frame 0 `(15,10)–(68,70)`, frame 1 `(14,10)–(68,71)`), and contain only fully transparent or fully opaque pixels.
- Verified every opaque game-sprite color comes from the accepted idle palette. Green is confined to the vest. The saturated orange emblem colors are confined to the shoulder; the face retains its accepted skin-shading pixel.
- Verified all opaque source-head pixels are unchanged in both frames, and the head pixels match across frames. Hand, leg, and headband-cloth positions vary slightly.
- `python3 scripts/pixel_noise.py`: frame 0 — `1379 px, 14 colours, 42 isolated (3.0%)`; frame 1 — `1426 px, 14 colours, 46 isolated (3.2%)`. Both are below 4%.

## Remaining game-side checks

- Import both frames into the water-prison animation and confirm in-game scale, draw offset, facing, animation cadence, and appearance behind the water effect. No game build or in-game acceptance was performed by the art worker.
