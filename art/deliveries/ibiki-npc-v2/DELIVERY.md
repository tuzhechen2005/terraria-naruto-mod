# ibiki-npc-v2 delivery

- status: delivered
- request ID: `ibiki-npc-v2`
- Generation: built-in `image_gen__imagegen`; generated pose strips were sampled to a 28×25 art-pixel grid and enlarged with nearest-neighbor sampling to 224×200 per frame. No API key or fallback CLI was used.

## Delivered files

| Path | Dimensions | Alpha | Pose order |
| --- | ---: | --- | --- |
| `source/Ibiki_Idle_Walk.png` | 1568×200 | RGBA, binary transparency | Idle (hands behind back), Walk 1–6 |
| `source/Ibiki_Idle_Jump_Sit_Throw.png` | 1344×200 | RGBA, binary transparency | Idle, Jump, Sit, Throw windup, Throw release, Throw recovery |
| `source/Ibiki_Talk.png` | 672×200 | RGBA, binary transparency | Idle, arms folded, arm raised and pointing forward |
| `comparison_1x_dark.png` | 56×25 | Opaque RGB | Tazuna and Ibiki idle, native art pixels, dark background |
| `comparison_1x_light.png` | 56×25 | Opaque RGB | Tazuna and Ibiki idle, native art pixels, light background |
| `comparison_4x_dark.png` | 224×100 | Opaque RGB | Same comparison at 4×, dark background |
| `comparison_4x_light.png` | 224×100 | Opaque RGB | Same comparison at 4×, light background |

## Prompt summary

Generated Morino Ibiki in three transparent, right-facing pixel-art pose strips using `tazuna-npc-v1/source/Tazuna_Idle_Walk.png` as the style reference. The prompts specified a broad build, black headwrap and Leaf forehead plate, stern scarred face, long high-collar black coat with distinct blue-gray planes, gray inner shirt, black trousers and shoes, dark outlines, hard color clusters, and the requested pose orders. The first generated Ibiki strip was also used as a character consistency reference for the other strips.

## Checks performed

- Visually inspected the generated strips, final three source sheets, and the 4× light and dark comparisons.
- Confirmed source sheets have 7, 6, and 3 frames at exactly 224×200 per frame; each frame has a clear horizontal margin and a common foot baseline.
- Confirmed every source pixel is aligned to an 8×8 screen-pixel block and alpha values are only 0 or 255.
- Confirmed the exported poses face right and remain distinct at source size. The coat planes and arm gestures read in the comparison preview.

## Remaining game-side checks

- Run `scripts/build_npc_sheet.py`, integrate the generated sheets, and inspect the animations in Terraria beside Tazuna.
- Confirm the two facial scars and Leaf plate remain legible at native game scale; these fine details are the least certain after sampling to 28×25 art pixels.
