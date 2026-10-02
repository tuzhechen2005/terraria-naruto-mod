status: delivered
request ID: dosu-set-v3

## Delivered files

- `Dosu_Idle_0.png`–`Dosu_Idle_5.png` (6)
- `Dosu_Walk_0.png`–`Dosu_Walk_7.png` (8)
- `Dosu_Hurt_0.png` (1)
- `Dosu_DrillWindupIn_0.png`, `Dosu_DrillWindup_0.png`–`Dosu_DrillWindup_1.png`, `Dosu_Drill_0.png`–`Dosu_Drill_1.png` (5)
- `Dosu_WaveIn_0.png`, `Dosu_Wave_0.png`–`Dosu_Wave_2.png` (4)
- `Dosu_SlamIn_0.png`, `Dosu_Slam_0.png`–`Dosu_Slam_2.png` (4)
- `Dosu_LeapIn_0.png`, `Dosu_Leap_0.png`–`Dosu_Leap_2.png` (4)
- `Dosu_Head_Boss.png` (1)
- `source/Dosu_Idle_0_generated.png` (built-in image generation output)
- `preview.png`, `preview_4x.png`, `Dosu_Idle_overlay.png`, `Dosu_Walk_overlay.png`
- `build_delivery.py` (reproducible assembly and pixel cleanup)

All paths above are relative to `art/deliveries/dosu-set-v3/`. There are 33 game assets. Character frames are 112×88 RGBA PNG; the map head is 30×30 RGBA PNG. Every game asset uses only alpha 0 or 255. Character frames were verified to contain identical pixels in every 2×2 screen-pixel block. The generated source is 1415×1112 RGBA and retains its original alpha.

## Prompt and process

The built-in image generation edit used `art/deliveries/dosu-base-v3/Dosu_Idle_0.png` as the character base, the existing `Dosu_Idle_0.png` as the cylindrical gauntlet reference, and `tazuna-npc-v1/source/Tazuna_Idle_Walk.png` as the style reference. The prompt called for a closed dark outline, deeper fur shadows, a visible tuft of dark hair, bandages revealing one eye, a cylindrical gauntlet with a neat row of dark holes, crisp hard-value pixel clusters, and a transparent background. This generated master was reduced to the specified pixel grid. The existing Dosu game frames supplied action-pose silhouettes; their colors, contours and isolated flecks were cleaned on the same grid. Four lead-in poses and the added idle/walk frames were assembled from those inputs.

## Checks performed

- Inspected the generated master and the final idle, walk, wave, drill, slam and leap examples at enlarged scale.
- Reviewed `preview.png` against the named base, existing idle frame and Tazuna reference; exported `preview_4x.png` for detailed review.
- Programmatically checked all 33 game PNGs: dimensions, binary alpha and 2×2 pixel blocks. Checked grounded frame feet against the y=83 baseline; the airborne leap frame is intentionally higher.
- Exported six-frame idle and eight-frame walk overlays to inspect registration.

## Remaining game-side checks

- Integrate the sprites into `ShinobiPrototype`, build and reload the mod, and inspect animation timing, origin alignment, direction flipping, attack reach, and 1.5× in-game readability.
- Review art consistency and technique transitions in motion. This delivery has not been game-tested.
