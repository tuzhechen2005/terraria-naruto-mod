status: delivered
request ID: haku-hires-full-v1

## Delivered files

- `HakuForest_00_Idle.png` — 80×80 RGBA; exact approved idle copy; alpha 0/255.
- `HakuForest_01_Walk.png` through `HakuForest_06_Walk.png` — six 80×80 RGBA walk frames; alpha 0/255.
- `HakuForest_07_Crouch.png`, `HakuForest_08_Crouch.png` — two 80×80 RGBA herb-gathering frames; alpha 0/255.
- `HakuForest_09_Talk.png` — 80×80 RGBA talk frame; alpha 0/255.
- `sheet_preview.png` — 2400×320 RGBA; ten frames in one row at 1× above ten frames in one row at 3×; alpha 0/255.
- `walk.gif` — 80×80, six frames, 125 ms per frame, looping; transparent index.
- `heads_8x.png` — 3040×232 RGBA; ten 38×29 head crops enlarged 8×; alpha 0/255.
- `Haku_generated_pose_source.png` — 2172×724 RGBA generated pose source; transparent background with partial alpha at antialiased edges.

All paths above are relative to `art/deliveries/haku-hires-full-v1/`.

## Prompt summary

Built-in image generation used the approved `HakuForest_Idle.png` for Haku's appearance and palette, and the accepted Iruka full animation sheet for motion style. The prompt requested nine separate right-facing pixel-art poses: six quiet walk phases, two herb-gathering crouches, and a gentle talk pose, on a transparent background. The generated source was pixel-scaled to game frames, mapped to the approved 20-color idle palette, and combined with the approved head pixels.

## Checks performed

- Visually inspected the generated source, 10-frame preview, a walk frame, and a crouch frame at 8×.
- Confirmed the idle PNG is byte-equivalent in decoded pixels to the approved baseline.
- Confirmed all ten frames are 80×80, have only alpha 0/255, use only baseline colors, have the approved head region (rows 16–43) pixel-exact, and end at foot row y=76.
- Ran `scripts/pixel_noise.py` on all ten frames using Python 3.14 with Pillow: idle 3.2%; walk frames 1.5–1.7%; crouch frames 1.3%; talk frame 1.6%. All are below 4%.
- Confirmed the GIF has six frames and the preview and head sheet have the stated dimensions.

## Remaining game-side checks

- Import into tModLoader and inspect leg motion, hair movement, herb/basket readability, frame timing, and foot alignment at actual game zoom.
- The talk frame keeps the approved facial pixels exactly, so the intended smile must be judged from the body gesture in game; a changed mouth would conflict with the request's pixel-exact head requirement.
