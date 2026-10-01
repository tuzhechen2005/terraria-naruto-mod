# Ibiki NPC v3 delivery

- status: delivered
- request ID: `ibiki-npc-v3`
- Method: built-in `image_gen__imagegen`, followed by cropping, palette cleanup, nearest-neighbor sampling to 28×25 art pixels, and 8× enlargement. No API key or fallback CLI was used.

## Game-size source strips

All paths below are relative to this delivery directory. Each cell is 224×200 PNG (28×25 art pixels); alpha is binary (0 or 255), with transparent backgrounds and exact 8×8 color blocks.

| File | Dimensions | Pose order |
| --- | --- | --- |
| `source/Ibiki_Idle_Walk.png` | 1568×200 | Idle (hands behind back), Walk 1–6 |
| `source/Ibiki_Idle_Jump_Sit_Throw.png` | 1344×200 | Idle, Jump, Sit, Throw 1–3 |
| `source/Ibiki_Talk.png` | 672×200 | Idle, Arms crossed, Point forward |

## Generated masters and previews

The selected image-tool PNGs were copied by filesystem path into `source/generated_idle.png` (2170×725), `source/generated_walk.png` (2170×725), `source/generated_action.png` (2171×724), and `source/generated_talk.png` (2170×725). These masters have transparent backgrounds with partial-alpha edge pixels; the three game-size strips above have binary alpha.

Tazuna comparison previews: `comparison_1x_light.png` and `comparison_1x_dark.png` (56×25 each), plus `comparison_4x_light.png` and `comparison_4x_dark.png` (224×100 each). Left is Tazuna; right is Ibiki. The previews have opaque backgrounds.

Build-script test outputs: `validation/Ibiki.png` (56×784, 14 frames) and `validation/Ibiki_Head.png` (26×26), both with binary alpha.

## Prompt summary

Used Ibiki v2 pose strips for coat lighting, physique and action layout, Ibiki v1 for the wrapped-head silhouette, and Tazuna for the pixel-art style. Required a hairless black headwrap, shiny silver forehead plate, two light scars, and a fully closed blue-grey double-breasted coat; preserved the right-facing NPC poses on transparent backgrounds. A separate walking pass supplied six distinct leg positions.

## Checks performed

- Visually inspected generated masters, all three sampled strips, the Tazuna side-by-side preview, and the assembled sheet.
- Confirmed 7/6/3 cells, 224×200 cells, exact 8×8 blocks, binary alpha, right-facing silhouettes, feet on the final art-pixel row, and no contact between adjacent cells.
- Confirmed the idle frame is 23 art pixels tall and 12 art pixels wide; every frame has a 3×2 silver plate and two separately placed light scar marks (vertical scaling makes some marks two pixels high).
- Ran `scripts/build_npc_sheet.py` successfully with all three strips; it produced 14 frames and a head icon in `validation/`.

## Remaining game-side checks

- Import the strips into the mod and view Ibiki beside Tazuna in the ninja school at actual game scale.
- Check animation transitions, pointing direction after the build script's horizontal flip, and the head icon in the game UI.
- No tModLoader build or in-game acceptance test was run by this art worker.
