# Delivery: orochimaru-moves-v11c

- status: delivered
- request ID: `orochimaru-moves-v11c`
- Image generation: built-in `image_gen__imagegen`; generated source copied to `source/generated_components.png`.

## Delivered files

All paths below are relative to this directory. Every game sprite is RGBA PNG with alpha restricted to 0 or 255.

| Files | Dimensions | Alpha |
| --- | --- | --- |
| `Orochimaru_Dash_0.png` through `Orochimaru_Dash_3.png` | 224×136 each | binary |
| `Orochimaru_Hands_0.png` through `Orochimaru_Hands_2.png` | 224×136 each | binary |
| `Orochimaru_HandsOut_0.png`, `Orochimaru_WindOut_0.png`, `Orochimaru_SealOut_0.png`, `Orochimaru_SummonOut_0.png`, `Orochimaru_NeckOut_0.png`, `Orochimaru_DashOut_0.png` | 224×136 each | binary |
| `Orochimaru_Neck_0.png` | 224×136 | binary |
| `Orochimaru_NeckHead.png` | 56×44 | binary |
| `Orochimaru_NeckSegment.png` | 12×12 | binary |
| `FxSnakeHand_Head.png` | 22×14 | binary |
| `FxSnakeHand_Segment.png` | 8×8 | binary |
| `preview.png`, `preview_1x.png` | 1120×320 each | opaque |
| `preview_3x.png` | 3360×960 | opaque |
| `source/generated_components.png` | 1774×887 | RGBA; soft source alpha |

`source/build_delivery.py` is the reproducible pixel assembly source. It uses only the request's named reference sprite sets plus the generated source image. The generated source was prompted for a transparent, right-facing pixel-art Orochimaru head, dark purple-gray snake head, pale neck tube, and dark snake body tube, matching the supplied base palette and Tazuna rendering. The game-size face pixels in `Orochimaru_NeckHead.png` were taken from the supplied Orochimaru base sprite.

## Connection points

- `Orochimaru_Hands_2.png`: upper sleeve opening center **(151, 66)**; lower sleeve opening center **(151, 78)**, measured from the full 224×136 frame's top-left pixel.
- `Orochimaru_Neck_0.png`: neck-root tip center **(133, 49)** in the 224×136 body frame.
- `Orochimaru_NeckHead.png`: incoming neck center **(0, 22)** on the left edge of the 56×44 head frame.

## Checks performed

- Inspected game-size sprites and 1×/3× previews visually for transparency, right-facing direction, readable silhouettes, two separate sleeve openings, and horizontal/30° neck assembly.
- Verified all 18 requested game sprites exist with exact requested dimensions and binary alpha.
- Verified all dash frames occupy top row y=82 and lowest row y=131.
- Verified left and right edge pixels match for both segment sprites, allowing seamless horizontal repetition.

## Remaining game-side checks

- Load the sprites in tModLoader and check the dash cycle at gameplay speed, the 40-frame snake-hand hold, and transition timing into standing.
- Check neck segment overlap and attachment during the game's curved rotation/interpolation, plus projectile spawn placement at both sleeve centers.
- Check visual scale and palette against the live boss under in-game lighting. No game-side check was run by the art worker.
