status: delivered
request ID: kakashi-walk-repair-v3

## Files

- `source/generated_walk_sheet.png`: selected generated source, 2172×724 RGBA with transparency.
- `frames/`: 14 game-size 80×80 RGBA PNGs. Six `Kakashi_01_Walk.png` through `Kakashi_06_Walk.png` are newly exported; the other eight are byte-for-byte copies from v2.
- `walk.gif`: 320×240, six frames, 1× and 3× side by side.
- `before_after_walk.gif`: 480×240, six frames, old left and new right at 3×.
- `strip_1x_3x.png`: 3360×640 RGB, all 14 frames on light and dark backgrounds.
- `waist_8x.png`: 3840×304 RGB, six old/new pairs from shoulder/neck through thighs.
- `PROMPTS.md`: full prompt lineage; `export_delivery.py` and `manifest.json`: reproducible fixed-scale export and source coordinates.

## Prompt summary

Built-in image generation used the approved idle for Kakashi's face, outfit, proportions, direction, and pixel style. A six-pose walk sheet was regenerated to give all figures a shared scale and sole line. The six complete bodies were sampled at one fixed scale, centered on the vest, and anchored at sole row 75. No torso or leg rectangle was transferred between animation frames.

## Checks performed

- Inspected the generated source, 1×/3× contact sheet, and 8× waist comparison. Walk figures face right; mask, collar, vest, waist, and hips remain visually connected. Alternating near/far leg placement and modest arm swing are visible, with a continuous sole baseline.
- Source figures measure 567–569 pixels high before export. All six exported walk frames are 80×80, with visible bounds inside x=23–54, y=14–75. There is no edge clipping.
- Each exported walk frame has only alpha 0/255 and zero RGB in transparent pixels. All eight non-walk frame files match v2 byte for byte.
- Previews were generated from the exported 80×80 frames.

## Remaining game-side checks

The development assistant must install the six walk frames into the game texture, check the cycle at Terraria's actual walk cadence and in both facing directions, and confirm there is no perceived foot slide or loop jump in a live session. No game build or in-game inspection was performed by this art worker.
