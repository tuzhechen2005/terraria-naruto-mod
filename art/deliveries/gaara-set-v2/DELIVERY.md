# gaara-set-v2 delivery

status: delivered
request ID: gaara-set-v2

## Delivered files

- `Gaara_CastIn_0.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Cast_0.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Cast_1.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Cast_2.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Cracked_Idle_0.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Cracked_Idle_1.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Cracked_Idle_2.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Cracked_Idle_3.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Head_Boss.png` — 30×30, RGBA, alpha 0/255
- `Gaara_Hurt_0.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Idle_0.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Idle_1.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Idle_2.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Idle_3.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Idle_4.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Idle_5.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Shield_0.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Shield_1.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Walk_0.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Walk_1.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Walk_2.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Walk_3.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Walk_4.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Walk_5.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Walk_6.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Walk_7.png` — 112×88, RGBA, alpha 0/255
- `Gaara_WaveIn_0.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Wave_0.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Wave_1.png` — 112×88, RGBA, alpha 0/255
- `Gaara_Wave_2.png` — 112×88, RGBA, alpha 0/255
- `source/generated_key_poses.png` — 1536×1024, RGBA, transparent source atlas from built-in imagegen (alpha 0–254)
- `source/old_pose_reference.png` — 2240×1170, RGBA, old poses as reference only
- `source/build_frames.py` — reproducible game-frame processing script
- `preview.png` — 1380×5190, RGB, all frames at 1× and 4× with approved base and Tazuna references
- `preview_1x.png` — 800×870, RGB
- `preview_4x.png` — 1380×4320, RGB
- `alignment_overlay.png` — 896×400, RGB, idle and walk overlays with foot line

## Prompt summary

Built-in imagegen used the approved `gaara-base-v2/Gaara_Idle_0.png` as the exact character reference, old `Gaara_*.png` poses as pose-only references, and the Tazuna comparison as the style reference. The request asked for nine right-facing key poses with red hair, turquoise eye, deep red-brown clothing, white diagonal sash, beige gourd, connected dark outline, crisp pixel shading, and transparent surroundings. The game frames were then built from these key poses and the approved base, quantized to the base palette, and cleaned to binary alpha and 2×2 pixel blocks.

## Checks performed

- Inspected the generated source, the approved base, the Tazuna style reference, the old-pose contact sheet, the 1×/4× preview and the alignment overlay.
- Opened and decoded every delivered PNG. All 29 body frames are 112×88, RGBA with only 0/255 alpha, 2×2 solid pixel blocks, and lowest opaque row y=83. The portrait is 30×30, RGBA with only 0/255 alpha.
- `Gaara_Idle_0.png` matches the approved base byte for byte. Six idle, eight walk and four cracked-idle frames are each distinct within their loop.
- Visual checks: right-facing silhouette, gourd and strap readable at 1×; cast, wave, shield and hurt poses distinguishable. The sand wall is absent from the character frames.

## Remaining game-side checks

- Integrate and run Build + Reload in tModLoader; verify sprite alignment against hitboxes, animation timing, and readability at 1.5× game scale. This was not run by the art worker.
- Review the walk cycle and sand motion in game, where cadence and effects are easier to judge than in static previews.
