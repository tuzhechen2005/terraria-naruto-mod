# Orochimaru set v5b delivery

- status: delivered
- request ID: `orochimaru-set-v5b`
- Generator: built-in `image_gen__imagegen`; no API key or fallback CLI.

## Delivered files

Game frames (18 PNGs, each **112×88 RGBA**, alpha values **0/255**):

- `Orochimaru_HandsIn_0.png`, `Orochimaru_Hands_0.png`, `Orochimaru_Hands_1.png`, `Orochimaru_Hands_2.png`
- `Orochimaru_DashIn_0.png`, `Orochimaru_Dash_0.png`, `Orochimaru_Dash_1.png`
- `Orochimaru_WindIn_0.png`, `Orochimaru_Wind_0.png`, `Orochimaru_Wind_1.png`
- `Orochimaru_NeckIn_0.png`, `Orochimaru_Neck_0.png`
- `Orochimaru_SealIn_0.png`, `Orochimaru_Seal_0.png`, `Orochimaru_Seal_1.png`
- `Orochimaru_SummonIn_0.png`, `Orochimaru_Summon_0.png`, `Orochimaru_Summon_1.png`

Generated source sheets (transparent RGBA with soft alpha, retained before game-size cleanup):

- `source/Orochimaru_Hands_generated.png` — 2172×724
- `source/Orochimaru_Dash_generated.png` — 2172×724
- `source/Orochimaru_Wind_generated.png` — 2172×724
- `source/Orochimaru_Neck_generated.png` — 2001×786
- `source/Orochimaru_Seal_generated.png` — 2172×724
- `source/Orochimaru_Summon_generated.png` — 2172×724

`preview.png` — 2736×3052 RGB, showing every frame beside the v5 base and Tazuna at 1× and 4×.

## Prompt summary

Used the v5 idle frame as the character-design reference, Tazuna as the hard-shaded pixel-art style reference, and the named older Orochimaru frames as pose references. Generated six action sheets for sleeves, ground dash, wind breath, extending neck, five-finger seal, and ground summon. Prompts specified right-facing poses, a narrow chin outline and sly mouth, transparent background, no effects or snakes, and consistent costume and colors. Cropped the selected generated sheets into frames, reduced them to a 2×2 screen-pixel art grid, mapped colors to the v5 base palette, removed tiny isolated components, and set binary alpha.

## Checks performed

- Visually inspected all six generated sheets and the final comparison preview.
- Confirmed 18 requested game-frame filenames, dimensions of 112×88, and alpha values limited to 0 and 255.
- Confirmed the lowest occupied pixel is row 83 in every frame and the poses face right.
- Confirmed `Neck_0` has no head above its collar; checked hands, dash, wind, seal, and summon silhouettes at 1× and 4× in the preview.
- Confirmed the six original generated sheets and preview are present with the dimensions listed above.

## Remaining game-side checks

- Import the frames and inspect animation timing, pivot alignment, and legibility at the game's 1.5× scale.
- Test the `Neck_0` collar join with NeckHead/NeckSegment and confirm the seal fingertip flame placement.
- Review facial pixels in game, especially the requested tiny mouth and chin contour, and adjust any frame that needs a pixel-level touch-up during integration.
