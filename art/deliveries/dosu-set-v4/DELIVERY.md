# Dosu set v4 delivery

- status: delivered
- request ID: dosu-set-v4
- output directory: `art/deliveries/dosu-set-v4/`

## Delivered game asset paths

- `art/deliveries/dosu-set-v4/Dosu_DrillWindupIn_0.png`
- `art/deliveries/dosu-set-v4/Dosu_DrillWindup_0.png`
- `art/deliveries/dosu-set-v4/Dosu_DrillWindup_1.png`
- `art/deliveries/dosu-set-v4/Dosu_Drill_0.png`
- `art/deliveries/dosu-set-v4/Dosu_Drill_1.png`
- `art/deliveries/dosu-set-v4/Dosu_Head_Boss.png`
- `art/deliveries/dosu-set-v4/Dosu_Hurt_0.png`
- `art/deliveries/dosu-set-v4/Dosu_Idle_0.png`
- `art/deliveries/dosu-set-v4/Dosu_Idle_1.png`
- `art/deliveries/dosu-set-v4/Dosu_Idle_2.png`
- `art/deliveries/dosu-set-v4/Dosu_Idle_3.png`
- `art/deliveries/dosu-set-v4/Dosu_Idle_4.png`
- `art/deliveries/dosu-set-v4/Dosu_Idle_5.png`
- `art/deliveries/dosu-set-v4/Dosu_LeapIn_0.png`
- `art/deliveries/dosu-set-v4/Dosu_Leap_0.png`
- `art/deliveries/dosu-set-v4/Dosu_Leap_1.png`
- `art/deliveries/dosu-set-v4/Dosu_Leap_2.png`
- `art/deliveries/dosu-set-v4/Dosu_SlamIn_0.png`
- `art/deliveries/dosu-set-v4/Dosu_Slam_0.png`
- `art/deliveries/dosu-set-v4/Dosu_Slam_1.png`
- `art/deliveries/dosu-set-v4/Dosu_Slam_2.png`
- `art/deliveries/dosu-set-v4/Dosu_Walk_0.png`
- `art/deliveries/dosu-set-v4/Dosu_Walk_1.png`
- `art/deliveries/dosu-set-v4/Dosu_Walk_2.png`
- `art/deliveries/dosu-set-v4/Dosu_Walk_3.png`
- `art/deliveries/dosu-set-v4/Dosu_Walk_4.png`
- `art/deliveries/dosu-set-v4/Dosu_Walk_5.png`
- `art/deliveries/dosu-set-v4/Dosu_Walk_6.png`
- `art/deliveries/dosu-set-v4/Dosu_Walk_7.png`
- `art/deliveries/dosu-set-v4/Dosu_WaveIn_0.png`
- `art/deliveries/dosu-set-v4/Dosu_Wave_0.png`
- `art/deliveries/dosu-set-v4/Dosu_Wave_1.png`
- `art/deliveries/dosu-set-v4/Dosu_Wave_2.png`

## Source and review paths

- `art/deliveries/dosu-set-v4/source/Dosu_Idle_0_generated.png` — built-in image generation concept with all requested visual references supplied as inputs.
- `art/deliveries/dosu-set-v4/source/Dosu_Idle_0_final_4x.png` — exact nearest-neighbor enlargement of the selected final idle frame.
- `art/deliveries/dosu-set-v4/source/build_frames.py` — reproducible pixel-grid frame construction from the named base.
- `art/deliveries/dosu-set-v4/source/base_4x.png` and `source/old_idle_4x.png` — inspection enlargements of the named base and old idle reference.
- `art/deliveries/dosu-set-v4/preview.png` — all delivered frames plus the base, all existing 112×88 Dosu frames, and one Tazuna frame at 1× and 4×.
- `art/deliveries/dosu-set-v4/preview_1x.png`, `preview_4x.png`, `preview_index.txt` — separate contact sheets and ordered index.
- `art/deliveries/dosu-set-v4/idle_overlay.png` and `walk_overlay.png` — registered cycle overlays.

## Dimensions and alpha

- 32 character frames: 112×88 RGBA PNG; transparent background; alpha values only 0 and 255.
- `Dosu_Head_Boss.png`: 30×30 RGBA PNG; transparent background; alpha values only 0 and 255.
- Each character frame uses all 27 opaque colors sampled from `art/deliveries/dosu-base-v3/Dosu_Idle_0.png` on an exact 2×2 screen-pixel grid.
- Generated concept source: 1414×1112 RGBA PNG with soft alpha. It is a source reference, not a game frame.
- Final idle 4× source enlargement: 448×352 RGBA PNG with binary alpha. The previews have opaque dark backgrounds; the two overlays use alpha 0/130.

## Prompt summary

Built-in `image_gen__imagegen` generated a transparent, facing-right Dosu concept using the named Dosu base, original armor, Gaara consistency example, and Tazuna style references. The prompt specified a hunched fur-cloaked character, one visible eye, a dark hair tuft, a cylindrical four-port metal arm guard, near-black closed silhouette, hard color ramps, and no blur or stray pixels. The selected game frames were then constructed from the actual 27-color 2×2-pixel base so the entire set retains one character design. The arm guard shape and shading were revised from the generated concept; the finished game PNGs are pixel-clean derivatives of the base.

## Checks performed

- Visually inspected the named base, original idle/poses, Gaara consistency example, Tazuna style reference, generated concept, final idle enlargement, 1× and 4× contact sheets, head icon, and cycle overlays.
- Verified all 33 requested PNG paths exist; all 32 character frames are 112×88; the head icon is 30×30.
- Decoded every game PNG and verified alpha is 0/255 only. Verified every 112×88 frame is uniform within each 2×2 cell and uses 27 base colors (above the required 22).
- Checked no character frame has detached four-connected opaque components. Checked idle and walk frames retain the foot baseline at y=83. Reviewed facing direction and action silhouettes at game size.
- Inspected the preview with original frames and the Tazuna reference. No game run or mod build was performed by this art worker.

## Remaining game-side checks

- Integrate the 33 game PNGs into the mod, then run the Mac build verification.
- In Terraria at 1.5×, verify idle/walk timing, no visible stance-to-walk guard jump, action readability, hitbox alignment, left/right facing behavior, and the minimap head at its actual display size.
- Confirm attack frame ordering and in-game offsets against the current boss animation code.
