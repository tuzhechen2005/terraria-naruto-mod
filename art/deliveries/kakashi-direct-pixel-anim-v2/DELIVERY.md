status: delivered
request ID: kakashi-direct-pixel-anim-v2

# Kakashi direct pixel animation v2

## Delivered files

- `frames/`: complete set of 14 game-size 80×80 RGBA PNGs. The four changed files are `Kakashi_03_Walk.png`, `Kakashi_04_Walk.png`, `Kakashi_05_Walk.png`, and `Kakashi_11_Throw.png`. The other ten are byte-for-byte copies of v1.
- `walk.gif`: 240×240, six frames at 120 ms each, right-facing walk preview on a dark background.
- `strip_1x_3x.png`: 3360×640 RGB, all 14 frames at 1× and 3× on light and dark backgrounds.
- `fix_8x.png`: 5120×640 RGB, before/after pairs for the four changed frames, enlarged with nearest-neighbor sampling.
- `generated_corrections.png`: 2172×724 RGBA original output from the built-in `image_gen__imagegen` tool, copied from `$CODEX_HOME/generated_images`.
- `build_delivery.py`: repeatable pixel-level final-frame and preview assembly.

## Prompt and finalization

The image-generation prompt asked for the four right-facing Kakashi poses with a flat olive vest waist in Walk 03/04 and a natural rear sleeve and visible hand in Walk 05/Throw 11, preserving the existing palette, head, leg phases, throw gesture, scale, and transparent background. The generated sheet was inspected as a visual correction reference. Its native proportions and pixels did not match the approved 80×80 animation closely enough for direct downsampling. The game-size finals therefore use exact v1 pixels: adjacent approved walk torsos replace the distorted waist in 03/04, and an approved connected rear arm replaces the hook in 05/11. The target legs, feet, facing, and all other frames remain as in v1.

## Checks performed

- Inspected the generated sheet and before/after 8× comparison, plus the final 1×/3× strip on light and dark grounds. At game scale the four corrected silhouettes read as right-facing Kakashi; the repaired waist no longer protrudes as a separate green box, and the rear arm has a continuous sleeve and visible skin-tone hand.
- Verified exactly 14 final frame files; each is 80×80 RGBA with only fully transparent or fully opaque alpha, zero RGB in transparent pixels, and no pixels touching the canvas edge.
- Verified the four edited frames each form one connected 8-neighbor silhouette. All ground frames end at y=75; the jump and trapped frames retain their v1 placement.
- Verified the ten unchanged frame files match v1 byte-for-byte; only the four requested frames differ.
- Verified `walk.gif` has six 240×240 frames and both PNG comparison sheets have the listed dimensions.

## Remaining game-side checks

Assemble and preview the frames in Terraria/tModLoader at native scale. Check the walk cycle timing and alignment, Throw 11 in sequence with the kunai release, and the silhouette against actual game backgrounds. No in-game test or mod build was performed by this art worker.
