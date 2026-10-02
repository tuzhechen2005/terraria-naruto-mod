# Orochimaru set v11b delivery

status: delivered
request ID: orochimaru-set-v11b

## Delivered files

- 18 game frames in the delivery root:
  - `art/deliveries/orochimaru-set-v11b/Orochimaru_DashIn_0.png`
  - `art/deliveries/orochimaru-set-v11b/Orochimaru_Dash_0.png`
  - `art/deliveries/orochimaru-set-v11b/Orochimaru_Dash_1.png`
  - `art/deliveries/orochimaru-set-v11b/Orochimaru_HandsIn_0.png`
  - `art/deliveries/orochimaru-set-v11b/Orochimaru_Hands_0.png`
  - `art/deliveries/orochimaru-set-v11b/Orochimaru_Hands_1.png`
  - `art/deliveries/orochimaru-set-v11b/Orochimaru_Hands_2.png`
  - `art/deliveries/orochimaru-set-v11b/Orochimaru_NeckIn_0.png`
  - `art/deliveries/orochimaru-set-v11b/Orochimaru_Neck_0.png`
  - `art/deliveries/orochimaru-set-v11b/Orochimaru_SealIn_0.png`
  - `art/deliveries/orochimaru-set-v11b/Orochimaru_Seal_0.png`
  - `art/deliveries/orochimaru-set-v11b/Orochimaru_Seal_1.png`
  - `art/deliveries/orochimaru-set-v11b/Orochimaru_SummonIn_0.png`
  - `art/deliveries/orochimaru-set-v11b/Orochimaru_Summon_0.png`
  - `art/deliveries/orochimaru-set-v11b/Orochimaru_Summon_1.png`
  - `art/deliveries/orochimaru-set-v11b/Orochimaru_WindIn_0.png`
  - `art/deliveries/orochimaru-set-v11b/Orochimaru_Wind_0.png`
  - `art/deliveries/orochimaru-set-v11b/Orochimaru_Wind_1.png`
- `art/deliveries/orochimaru-set-v11b/preview.png` (1× and 3× comparison with chosen base)
- `art/deliveries/orochimaru-set-v11b/preview_1x.png`
- `art/deliveries/orochimaru-set-v11b/preview_3x.png`
- `art/deliveries/orochimaru-set-v11b/head_comparison_4x.png`
- Six generated source images and `build_frames.py` in `art/deliveries/orochimaru-set-v11b/source/`.

## Dimensions and alpha

- Each game frame: 224×136 RGBA; alpha values are exclusively 0 and 255; lowest opaque row is y=131.
- Source sheets: Hands, Dash, Wind, Seal, Summon are 2172×724 RGBA; Neck is 1774×887 RGBA.
- `preview.png`: 2904×3310 RGB; `head_comparison_4x.png`: 5700×240 RGB.

## Prompt summary

The built-in image generator used the chosen v11 idle sprite as character reference and the named v5c pose sheets for action only. Six transparent pose sheets cover sleeve thrust, low snake dash, wind inhalation and exhalation, planted neck extension, open-palmed seal, and crouched ground summon. Generated poses were sampled to game scale, mapped to the v11 palette, and the selected v11 face pixels were composited into each headed frame. `Neck_0` has no head or neck for the separate assembly.

## Checks performed

- Inspected the v11 base, v5c pose preview, Tazuna style reference, all six generated source sheets, the 1× game-scale preview, the 3× preview, and 4× head comparison.
- Checked all 18 files exist at 224×136, have binary transparency, stay inside canvas, and touch ground row 131.
- Checked facing direction, readable action silhouettes, frame order, head visibility except `Neck_0`, open seal palm, and removal of detached pose-sheet fragments.
- Confirmed all frame colors are drawn from the v11 base palette.

## Remaining game-side checks

- Load each frame in tModLoader and inspect animation timing, attachment point of NeckHead/NeckSegment, sprite origin, hitboxes, and alignment with snake, wind, flame, and summon effects.
- Confirm readability at normal game zoom against varied backgrounds.

No game build or in-game verification was run by this art worker.
