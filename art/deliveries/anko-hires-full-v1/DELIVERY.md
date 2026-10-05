# Anko high-resolution three-frame delivery

- status: delivered
- request ID: `anko-hires-full-v1`

## Delivered files

| File | Dimensions | Alpha |
| --- | ---: | --- |
| `Anko_00_Idle.png` | 80 × 80 | 0/255 only; byte-for-byte copy of accepted reference |
| `Anko_01_Pose.png` | 80 × 80 | 0/255 only |
| `Anko_02_Talk.png` | 80 × 80 | 0/255 only |
| `preview.png` | 960 × 400 | 0/255 only; three frames at 1× above and 4× below |
| `Anko_01_Pose_source.png` | 1254 × 1254 | transparent with intermediate alpha; built-in image generation source |
| `Anko_02_Talk_source.png` | 1254 × 1254 | transparent with intermediate alpha; built-in image generation source |

All paths are relative to `art/deliveries/anko-hires-full-v1/`.

## Prompt summary

The built-in `image_gen__imagegen` tool used the accepted `Anko_Idle.png` as its sole visual reference. It generated transparent right-facing pixel-art concepts for (1) a hand-on-hip pose with a gray-bladed, dark-handled ring kunai beside the face and a stronger grin, and (2) a rules-announcement pose with a forward arm, raised index finger, and slightly open mouth. The 80 × 80 frames were finished against the accepted base pixels to keep the head, outfit, palette, and feet aligned.

## Checks performed

- Inspected both generated source images and the 1×/4× final preview visually, including action silhouette, direction, and game-size readability.
- Confirmed the idle frame matches the accepted reference byte for byte.
- Confirmed all three game frames are 80 × 80, face right, have feet at `y=76`, use only alpha 0/255, and use only the accepted 15-color palette.
- Compared head pixels with the accepted frame: the pose changes two mouth-area pixels; talk changes one; all other head pixels match.
- Reproduced the `scripts/pixel_noise.py` four-neighbor/tolerance-24 calculation: idle 2.08%, pose 2.90%, talk 2.37%; all below 4%. The Python script itself was unavailable because this worker's Python lacks Pillow.
- Checked opaque connected components: both new action frames form one connected silhouette. The accepted idle frame contains its original two-pixel separated detail at `(49,39)`–`(49,40)` and was copied unchanged as requested.

## Remaining game-side checks

- Load the three frames in Terraria/tModLoader and confirm rendering, frame switching, anchor position, and the pose/talk readability at the Forty-Fourth Training Ground entrance.
