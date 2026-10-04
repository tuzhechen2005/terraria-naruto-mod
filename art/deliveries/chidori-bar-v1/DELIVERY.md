status: delivered
request ID: chidori-bar-v1

## Delivered files

| File | Dimensions | Alpha |
| --- | --- | --- |
| `ChidoriBarFrame.png` | 76×16 | RGBA, only 0/255 |
| `ChidoriBarFill.png` | 64×6 | RGBA, only 255; fully opaque within its rectangular fill area |
| `ChidoriBarSpark_0.png` | 12×12 | RGBA, only 0/255 |
| `ChidoriBarSpark_1.png` | 12×12 | RGBA, only 0/255 |
| `ChidoriBarSpark_2.png` | 12×12 | RGBA, only 0/255 |
| `preview.png` | 1030×560 | RGBA, opaque preview backgrounds |
| `source/generated-chidori-sheet.png` | 2172×724 | RGBA, graded alpha in original generated source; not for direct game use |

All paths are relative to `art/deliveries/chidori-bar-v1/`.

## Placement

Draw the frame first. Draw the visible portion of the fill at frame-local **(6, 5)**; the full fill occupies x=6–69 and y=5–10 inclusive. Reveal the fill from left to right by cropping its right edge. Draw the active spark after the fill, centered over its leading edge; the preview uses spark top-left x=`progress` and y=2 for progress 1–64. Omit the spark when progress is zero. Cycle spark frames 0–2.

## Prompt summary

Generated a transparent pixel-art source sheet with a compact dark-indigo lightning bar, a blue-to-white hard-step charge strip, and three white-core/blue-outline spark poses. The design used `ShinobiPrototype/Assets/UI/SealSlotBack.png` and `ShinobiPrototype/Content/Projectiles/FxChidoriCharge_0.png` as visual references. The generated source was reduced and cleaned for exact game dimensions and binary-alpha sprites.

## Checks performed

- Confirmed all five game sprite dimensions and binary alpha values programmatically.
- Confirmed the 64×6 channel location and full opaque fill region.
- Inspected `preview.png` with empty, half, and full states at 1× and 4× on dark and light backgrounds.
- Checked left-to-right fill progression, distinct spark poses, silhouette, and white-core visibility at game scale.

## Remaining game-side checks

- Confirm overhead position, draw order, and world-camera zoom in Terraria.
- Confirm the spark tracks the leading edge through the full charge animation without clipping or covering nearby UI.
- Check readability over actual bright and dark terrain during motion.
