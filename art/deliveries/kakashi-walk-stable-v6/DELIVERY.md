status: delivered
request ID: kakashi-walk-stable-v6

## Files

- `source_walk_sheet.png` — full generated source, 2172×724 RGBA with transparency.
- `frames/Kakashi_01_Walk.png` through `frames/Kakashi_06_Walk.png` — six new 80×80 RGBA walk frames, alpha only 0/255, transparent RGB zero.
- `frames/Kakashi_00_Idle.png`, `frames/Kakashi_07_Jump.png` through `frames/Kakashi_11_Throw.png`, and `frames/Kakashi_Trapped_0.png`/`Kakashi_Trapped_1.png` — eight 80×80 RGBA frames copied byte-for-byte from the approved v2 frames.
- `walk_strip.png` — transparent 480×80 six-frame strip.
- `walk_1x.gif` (80×80) and `walk_3x.gif` (240×240) — six-frame loops, 140 ms/frame, fixed dark background, common quantization palette, disposal 2, optimization disabled.
- `walk_lossless.apng` — transparent 80×80, six-frame lossless comparison loop.
- `before_after.png` — v5 above, new walk below; `preview_3x.png` — all six new poses at 3×.
- `export.py` and `prompt.txt` — reproducible export and full generation prompt.

## Construction and checks

- One built-in imagegen call generated all six poses on a single sheet. The script crops fixed source y=126..647, rescales each at the common 62/522 ratio, aligns the body near x=37, and places it at y=14..75. No frame is stretched to its own bounding box.
- The approved Idle head, neck and collar block at x=20..51, y=14..36 is reused at the identical location in all six frames, replacing the generated region. Pixel comparison confirms this block is identical across the six frames.
- Visual checks: reviewed the generated source, new poses side by side at 1× and 3×, and v5/new comparison. The six poses show alternate contacts and passing phases; body direction remains right-facing. No visible ghost head, detached neck, waist separation, or cropped feet in these previews.
- Mechanical checks: 14 frames total; all 80×80. All six walk frame boxes start at y=14 and end at y=75. New walk frames have only alpha 0/255 and zero RGB at alpha 0. Eight non-Walk PNG files are byte-identical to v2, including Idle. Adjacent walk frames have distinct pixel content. GIF and APNG each contain six frames.

## Remaining game-side checks

- Integrate the six walk PNGs into the game sheet, then inspect at actual 1× Terraria scale and walk speed, including the 6→1 transition, on light and dark backgrounds. Confirm left-facing mirroring and collision/foot alignment in game. No source code or game files were changed here.
