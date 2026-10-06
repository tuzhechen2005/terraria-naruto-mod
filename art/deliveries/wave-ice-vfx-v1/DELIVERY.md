status: delivered
request ID: wave-ice-vfx-v1

## Files

- `source/wave-ice-vfx-sheet.png`: built-in imagegen source, 1536 × 1024 RGBA.
- `sprites/IceMirror.png`: 128 × 192 RGBA; center anchor (64, 96).
- `sprites/MirrorCracks.png`: 128 × 192 RGBA; center anchor (64, 96), aligns inside IceMirror.
- `sprites/MirrorShards.png`: 128 × 128 RGBA; center anchor (64, 64), decorative aftermath.
- `sprites/NeedleGlow.png`: 96 × 32 RGBA; center anchor (48, 16), points right, visual emphasis on the existing damaging needle only.
- `sprites/FrostCloud.png`: 128 × 128 RGBA; center anchor (64, 64), decorative mist.
- `sprites/MirrorHalo.png`: 128 × 192 RGBA; center anchor (64, 96), aligns with IceMirror; intended for active mirror only.
- `preview/light-1x.png`, `preview/dark-1x.png`: 480 × 448 RGB sheets at actual sprite pixel size on light/dark backgrounds.
- `preview/mirror-loop.apng`: 256 × 256 RGBA, nine frames at 100 ms each; illustrative form → light → crack → scatter loop.
- `PROMPT.txt`: complete generation prompt and transparent-background setting.
- `export.py`: reproducible crop, resize, zero-hidden-RGB, preview, loop and manifest export from the retained source.
- `manifest.json`: source crop windows, detected alpha bounds, export positions, anchors, facing, visual role and suggested visibility.

## Checks performed

- Inspected the generated source and both 1× previews. All six source items were distinct and did not touch adjacent crops.
- Checked the six PNGs at exact requested dimensions. Every sprite has fully transparent and partially transparent pixels, nonempty art, and empty outer edge rows/columns.
- Checked zero-alpha pixels: all RGB channels are zero after export. Soft translucent edges remain; no NPC binary-alpha or limited-palette process was applied.
- Checked IceMirror, MirrorHalo and FrostCloud centers are open alpha; mirror, crack and halo share the 128 × 192 center anchor (64, 96).
- Checked NeedleGlow points right and remains a narrow trail; shards and cloud are visual decoration, not new attacks.

## Remaining game-side checks

- Test mirror/crack/halo registration and the active mirror highlight in the actual tModLoader scene; inspect on platforms and bright/dark biomes.
- Align NeedleGlow tip with the existing projectile's damage area and verify its prewarning and direction under horizontal flip.
- Tune layer opacity, duration, particles and off-screen lifetime against the encounter and measure performance. This delivery has not been tested in game; sprites and preview timing do not add hitboxes or abilities.
