status: delivered
request ID: kakashi-hires-full-v1

# Delivered files

- `Kakashi_00_Idle.png` — 80×80 RGBA; byte-for-byte copy of the approved idle reference.
- `Kakashi_01_Walk.png` through `Kakashi_06_Walk.png` — six 80×80 RGBA walking frames.
- `Kakashi_07_Jump.png` — 80×80 RGBA jump frame.
- `Kakashi_08_Sit.png` — 80×80 RGBA seated frame.
- `Kakashi_09_Throw.png` through `Kakashi_11_Throw.png` — three 80×80 RGBA kunai throw frames.
- `sheet_preview.png` — 2880×320 RGBA, all 12 frames in one row at 1× above one row at 3×.
- `walk.gif` — 80×80, six frames, 100 ms per frame, looping.
- `heads_8x.png` — 3072×192 RGBA, 12 head crops at 8× in one row.
- `Kakashi_motion_reference.png` — 2172×724 RGBA image generated with the built-in `image_gen__imagegen` tool; pose reference, not a game sprite.

All delivered files are in `art/deliveries/kakashi-hires-full-v1/`.

# Prompt summary

Used the approved Kakashi idle sprite as the primary character and palette reference and the approved Tazuna idle sprite as the style reference. Requested six lazy book-reading walk poses, a tucked-knee jump, a seated book-reading pose, and three kunai throw stages, all facing right on a transparent background. The generated image served as a motion reference. Final game frames were composed at native pixel resolution from the approved Kakashi sprite so the head, palette, and 1-pixel edges stay exact.

# Checks performed

- Inspected `sheet_preview.png` at 1× and 3× and `heads_8x.png` at 8× for silhouette, direction, pose readability, and matching facial pixels.
- Verified every game frame is 80×80 RGBA with alpha values only 0 or 255, and uses 12–15 opaque colors (under the 20-color limit).
- Verified the 24×32 head region (`x=24…55`, `y=15…38`) matches the approved idle frame pixel-for-pixel in all 12 frames. The idle PNG is byte-for-byte identical to the reference.
- Verified feet reach `y=76` in the idle, walk, sit, and throw frames; the jump frame's feet are off the ground. Verified the GIF contains six frames and lasts 0.6 seconds.
- `scripts/pixel_noise.py` could not run because Pillow is absent and package installation was unavailable. Reproduced its four-neighbor, RGB tolerance 24 calculation directly on decoded RGBA pixels. Equivalent output:

```text
Kakashi_00_Idle.png: 1258 px, 15 colours, 40 isolated (3.2%)
Kakashi_01_Walk.png: 1258 px, 15 colours, 42 isolated (3.3%)
Kakashi_02_Walk.png: 1252 px, 15 colours, 48 isolated (3.8%)
Kakashi_03_Walk.png: 1245 px, 15 colours, 43 isolated (3.5%)
Kakashi_04_Walk.png: 1192 px, 15 colours, 42 isolated (3.5%)
Kakashi_05_Walk.png: 1206 px, 15 colours, 45 isolated (3.7%)
Kakashi_06_Walk.png: 1225 px, 15 colours, 43 isolated (3.5%)
Kakashi_07_Jump.png: 1269 px, 14 colours, 41 isolated (3.2%)
Kakashi_08_Sit.png: 1329 px, 15 colours, 41 isolated (3.1%)
Kakashi_09_Throw.png: 1246 px, 12 colours, 31 isolated (2.5%)
Kakashi_10_Throw.png: 1298 px, 12 colours, 33 isolated (2.5%)
Kakashi_11_Throw.png: 1250 px, 12 colours, 31 isolated (2.5%)
```

# Remaining game-side checks

- Import the 12 frame PNGs into the mod and inspect the walk cycle, seated pose, jump, and kunai timing in Terraria at native scale.
- Check NPC origin, collisions, draw offsets, and interactions with terrain; adjust game-side frame timing or offsets if needed.
- Run the project verification/build and record in-game acceptance. No source files or Git state were changed by this delivery.
