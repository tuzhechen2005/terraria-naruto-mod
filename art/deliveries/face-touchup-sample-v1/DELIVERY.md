# Face touchup sample v1

- status: delivered
- request ID: `face-touchup-sample-v1`

## Delivered files

| File | Dimensions | Alpha |
| --- | ---: | --- |
| `Tazuna.png` | 56 × 672 | RGBA, transparent/opaque pixels |
| `ToolShopkeeper.png` | 56 × 672 | RGBA, transparent/opaque pixels |
| `diff.png` | 112 × 672 | RGBA, transparent/opaque pixels |
| `preview.png` | 672 × 640 | RGB, opaque |
| `generated_face_reference.png` | 1983 × 793 | RGBA, graded transparency |

`generated_face_reference.png` is the selected built-in ImageGen source study. The two 56 × 672 PNGs are the game-size delivery. `diff.png` places Tazuna on the left and ToolShopkeeper on the right; magenta marks every changed source pixel. `preview.png` compares the original and edited standing frames with the vanilla Guide at 1× and 4× for both characters.

## Prompt summary

Transparent pixel-art study of both existing NPC faces using their sprite sheets and the three named vanilla Terraria references. Emphasis: separate eye white and pupil, short dark brow, single-pixel nose projection and mouth, limited skin tones, and preserved hair and beard. The final sheets apply small pixel edits to the original files, using a 2 × 2 source-pixel block as one art-grid pixel.

## Checks performed

- Inspected the original sprite sheets and all three named vanilla references, the generated study, final sheets, difference map, and 1×/4× preview.
- Confirmed both game sprites remain 56 × 672 with twelve 56 × 56 frames and binary alpha.
- Confirmed changed source pixels per frame: Tazuna frames 1–12: **28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28**; ToolShopkeeper frames 1–12: **28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28**.
- Checked the changed-pixel mask against the small brow, eye, adjacent skin, nose, and mouth blocks in each frame. Tazuna's alpha is unchanged. ToolShopkeeper adds eight opaque source pixels total: a 2 × 2 nose-tip block in each of frames 2 and 4. Other alpha pixels are unchanged.
- Checked nearest-neighbor readability and the original facing direction in the preview and all-frame inspection.

## Remaining game-side checks

- Import the two sprite sheets into the mod and inspect all twelve frames at actual in-game scale beside the Guide, including the game's horizontal flip when the NPC faces right.
- Verify that the nose pixels and beard boundaries remain legible during animation. Build and in-game acceptance were not run by the art worker.
