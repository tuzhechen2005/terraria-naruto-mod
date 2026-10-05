status: delivered
request ID: hayate-hires-full-v1

## Delivered files

| Path (relative to this directory) | Dimensions | Alpha |
| --- | ---: | --- |
| `Hayate_00_Idle.png` | 80×80 | 0/255 only; exact copy of accepted baseline |
| `Hayate_01_Pose.png` | 80×80 | 0/255 only |
| `Hayate_02_Talk.png` | 80×80 | 0/255 only |
| `preview.png` | 960×400 | opaque comparison sheet; 1× top row and 4× bottom row |
| `Hayate_01_Pose_source.png` | 1254×1254 | transparent with partial edge alpha; generated pose reference |
| `Hayate_02_Talk_source.png` | 1254×1254 | transparent with partial edge alpha; generated pose reference |

## Prompt summary

The built-in image generator used the accepted `Hayate_Idle.png` as the sole visual reference. It generated one restrained cough with a fist at the mouth, closed eyes, and a slight hunch, and one tired, serious talking pose with a chest-level palm-up gesture. Both prompts required the headband, hair, face, green vest, sword, right-facing direction, and transparency to stay consistent. The generated sources informed pixel-level 80×80 game frames based on the accepted baseline palette.

## Checks performed

- Inspected both generated sources and the three-frame preview at 1× and 4×.
- Confirmed all three game frames are 80×80, face right, have feet ending at y=76, use only baseline palette colors, and have alpha values only 0/255.
- Ran `python3 scripts/pixel_noise.py` on the three game frames: idle 1.9%, cough 2.1%, talk 2.8% isolated pixels (each below 4%).
- Confirmed idle frame is an exact file copy of the accepted baseline.

## Remaining game-side checks

- Integrate the three game frames and verify the cough/talk animation timing, anchor alignment, and readability in the central tower hall in-game. No game build or in-game check was performed by the art worker.
