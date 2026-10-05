status: delivered
request ID: kakashi-hires-full-v2

## Delivered files

- `Kakashi_01_Walk.png` through `Kakashi_06_Walk.png`: six walk frames
- `Kakashi_07_Jump.png`: tucked-leg jump
- `Kakashi_08_Sit.png`: seated reading pose
- `Kakashi_09_Throw.png` through `Kakashi_11_Throw.png`: windup, release, recovery
- `Kakashi_generated_pose_source.png`: selected original image generated with the built-in image tool, retained as a pose reference source
- `sheet_preview.png`: twelve frames including the unchanged v1 idle frame, at 1× and 3×
- `walk.gif`: six walking frames at 4×
- `heads_8x.png`: twelve head crops at 8×

All paths are under `art/deliveries/kakashi-hires-full-v2/`.

## Dimensions and alpha

- Each game frame: 80×80 RGBA, fully transparent or fully opaque pixels, no partial alpha.
- Generated source: 2079×756 RGBA, transparent background with partial-alpha edge pixels; reference only, not for direct game import.
- Preview: 2880×320 RGBA; head comparison: 3072×192 RGBA; GIF: 320×320, six frames with transparency.

## Prompt summary

Built-in `image_gen__imagegen` produced a transparent eleven-pose Kakashi pixel-art reference sheet from the v1 Kakashi idle frame and the named Iruka walk, jump, sit, and throw references. The final 80×80 frames were pixel-edited from the v1 Kakashi sprites using that generated pose reference: reciprocal free-arm motion in the walk, connected navy legs with wraps and sandals in the jump, two bent seated legs and an orange book, and shoulder-rooted throwing arm poses with a kunai.

## Checks performed

- Inspected the generated source and the 1×/3× frame preview.
- Confirmed all eleven frames are 80×80 with binary alpha and foot pixels ending at y=76.
- Compared the head crop (x=24–55, y=15–38) byte-for-byte with v1 idle: identical in all eleven frames.
- Ran `scripts/pixel_noise.py` on the eleven game frames: isolated pixels 2.2%–3.8%, all below 4%. The generated source is reference art and is excluded from the game-frame pixel-noise threshold.
- Verified the walking GIF has six frames.

## Remaining game-side checks

- Import the eleven frames into the mod, then confirm animation order, pivot/alignment, and readability at native scale in Terraria. Game-side checks were not run by this art worker.
