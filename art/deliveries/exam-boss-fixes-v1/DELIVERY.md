# Delivery: exam-boss-fixes-v1

- status: delivered
- request ID: `exam-boss-fixes-v1`
- delivered frame files (delivery root):
  - `Gaara_Walk_0.png` through `Gaara_Walk_3.png`
  - `Gaara_Cracked_Idle_0.png` through `Gaara_Cracked_Idle_3.png`
  - `Gaara_Beast_Arm_0.png` through `Gaara_Beast_Arm_2.png`
  - `Dosu_Idle_0.png` through `Dosu_Idle_3.png`
  - `Dosu_Walk_0.png` through `Dosu_Walk_3.png`
  - `Dosu_Hurt_0.png`
- review files: `preview.png`, `alignment-overlay.png`
- generated source images: `source/Gaara_Walk_source.png`, `source/Gaara_Cracked_source.png`, `source/Gaara_Beast_Arm_source.png`, `source/Dosu_Idle_source.png`, `source/Dosu_Walk_source.png`, `source/Dosu_Hurt_source.png`
- frame preparation script: `source/build_frames.py`

## Dimensions and alpha

- All 17 standard frames are **112×88 RGBA**; all 3 transformed arm frames are **176×104 RGBA**. There are 20 frame PNGs total.
- Every frame has transparency with alpha values **0 or 255 only**. The lowest occupied pixel is at **y=83** for standard frames and **y=99** for transformed frames.
- `preview.png`: **750×4554 RGB**. `alignment-overlay.png`: **880×740 RGB**.
- Generated source sheets retain their original RGBA dimensions and partial alpha; only the game-size frame PNGs have binary alpha.

## Prompt summary

Used the built-in image generation tool with the named Gaara and Dosu base delivery frames as visual references. Requested six pixel-art animation source images: folded-arm alternating Gaara walk; angry Gaara with visibly fractured sand armor and changing falling grains; three Shukaku-arm windup/sweep/recovery poses with the arm itself moving; hunched Dosu idle with the perforated arm guard hanging at his side; compact cloak brisk Dosu walk; and Dosu recoiling behind a raised guard. Cropped and resized the generated poses to the requested canvases. Reversed only the lower-leg region of `Gaara_Walk_2.png` to make the opposite stride visibly alternate.

## Checks performed

- Opened the generated source images and inspected the six selected sheets at original size.
- Viewed the completed `preview.png` on dark and light backgrounds alongside correctly named original Gaara Idle/Beast Idle and Dosu Drill references. Each row's label comes directly from the file shown.
- Viewed `alignment-overlay.png`, which overlays each new frame on its named original reference and marks the center line and foot line.
- Checked each frame's dimensions, RGBA mode, occupied bounds, bottom pixel row, and binary alpha by script. The walk, cracked, beast-arm, idle, and hurt silhouettes are distinct at game scale.

## Remaining game-side checks

- Import the 20 frames and inspect animation timing, player-facing direction, visual scale, and frame transitions in tModLoader.
- Verify attack hitboxes and any horizontal mirroring convention against the new arm and guard silhouettes.
- No game build or in-game test was run by this art worker.
