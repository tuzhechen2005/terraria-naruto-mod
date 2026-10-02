status: delivered
request ID: dosu-base-v3

## Delivered files

- `Dosu_Idle_0.png` — final 112×88 RGBA game frame; Alpha values only 0 and 255; 27 opaque RGB colors.
- `source/Dosu_Idle_0_generated.png` — built-in imagegen source master, 1415×1112 RGBA; transparent background with 256 Alpha levels at antialiased edges.
- `preview.png` — 1882×532 opaque RGB comparison of v3, existing Dosu frame, v2, and Tazuna at 1× and 4×.

## Prompt summary

The built-in image generator received the existing Dosu frame as the required identity, pose, and silhouette reference; v2 as a negative reference for noise and broken outlines; and Tazuna as the required hard-edged pixel-art style reference. The prompt requested one right-facing, hunched Dosu with a bandaged face, Sound Village forehead plate, grey-brown shaggy cloak, large slotted sonic gauntlet, dark trousers, leg bindings, and sandals on a transparent background. The final game frame was rebuilt on a 56×44 art-pixel grid using the generated master for colors and shapes and the existing frame for the approved silhouette.

## Checks performed

- Inspected the generated master and final frame visually; compared the final against all three requested references in `preview.png`.
- Verified the final frame is 112×88, exactly 2×2 screen pixels per art pixel, right-facing, with one connected silhouette.
- Verified the nontransparent bounding box is x=28..75 and y=24..83, placing the lowest foot pixel on y=83.
- Verified Alpha contains only 0 and 255, 27 opaque colors, and all 97 exterior boundary art pixels use the same dark outline color.
- Checked the bandage, eye, gauntlet holes, fur masses, and stance at 1× and 4×.

## Remaining game-side checks

- Load the frame in tModLoader and inspect it at the intended 1.5× game scale against the surrounding NPC art and scene lighting.
- Check animation continuity after subsequent Dosu frames are derived from this base.
