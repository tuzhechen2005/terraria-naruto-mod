# neji-base-v1 delivery

- status: delivered
- request ID: `neji-base-v1`
- generation: built-in `image_gen__imagegen` for Run, alternate Palm, and Hurt source poses; approved `neji-style-v1` game images used as visual and key-pose references. `source/build_frames.py` exports the aligned game frames from those sources.

## Delivered files

All 21 game frames below are **112×88 RGBA PNG** with binary alpha (transparent 0, visible pixels 255):

| Action | Files |
| --- | --- |
| Idle | `Neji_Idle_0.png`, `Neji_Idle_1.png`, `Neji_Idle_2.png`, `Neji_Idle_3.png` |
| Run | `Neji_Run_0.png`, `Neji_Run_1.png`, `Neji_Run_2.png`, `Neji_Run_3.png` |
| Palm | `Neji_Palm_0.png`, `Neji_Palm_1.png`, `Neji_Palm_2.png` |
| Rotation | `Neji_Rotation_0.png`, `Neji_Rotation_1.png`, `Neji_Rotation_2.png`, `Neji_Rotation_3.png` |
| SixtyFour_Windup | `Neji_SixtyFour_Windup_0.png`, `Neji_SixtyFour_Windup_1.png` |
| SixtyFour_Strike | `Neji_SixtyFour_Strike_0.png`, `Neji_SixtyFour_Strike_1.png`, `Neji_SixtyFour_Strike_2.png` |
| Hurt | `Neji_Hurt_0.png` |

- `preview.png`: 896×616 RGB, no alpha. Each action occupies one row with 1× light and dark background views.
- `alignment-check.png`: 784×88 RGB, no alpha. Frames of each action are stacked in one cell, with x=56 and y=84 guide lines.
- `source/Neji_Run_generated.png`, `source/Neji_PalmAlternate_generated.png`, `source/Neji_Hurt_generated.png`: each 1415×1112 RGBA; raw image-generation source with partial-alpha edge pixels.
- `source/Neji_Idle.png`, `source/Neji_Palm.png`, `source/Neji_Rotation.png`, `source/Neji_SixtyFour.png`: each 112×88 RGBA with binary alpha; copies of the approved key poses.
- `source/build_frames.py`: Pillow export script.

## Prompt summary

Generate low forward-leaning sprint, alternate left-palm strike, and recoil poses of the approved Neji design. Preserve long brown hair, forehead protector, pale Byakugan eyes, ivory tunic, dark shorts, right arm and right thigh bandages, pouch, and sandals. Match the approved hard-edged limited-palette Terraria boss sprite style on transparent backgrounds. The export script creates breathing, running, palm, rotation, windup, and rapid-strike sequences with opaque cyan effects, then enforces the final game palette, pixel grid, and anchor.

## Checks performed

- Inspected all approved key poses, three new generated sources, the final 1× light/dark preview, and the stacked alignment image.
- Confirmed exactly 21 files with the requested names and counts. All game frames are 112×88, have only alpha 0 or 255, use uniform 2×2 screen-pixel blocks, and have at most 20 visible colors.
- Confirmed every frame ends at y=83 (floor line y=84). The rotating chakra ring stays inside the canvas. The series keeps rightward attack orientation except for the explicit turning angles in Rotation.
- Source images retain their original partial alpha; only the game-size exports are binary alpha.

## Remaining game-side checks

- Wire frames into the boss animation and check actual draw origin, animation timing, loop transitions, hitbox positions, and right-facing rendering in tModLoader.
- Verify Byakugan veins, palms, hair, bandages, rotation ring, and Eight Trigrams ground pattern against the live game's backgrounds and lighting at 1× scale.
- No mod build or in-game acceptance test was run by this art worker.
