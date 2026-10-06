status: delivered
request ID: haku-hires-fix-v1

## Delivered files

| File | Dimensions | Alpha |
| --- | --- | --- |
| `HakuForest_07_Crouch.png` | 80×80 | Binary, 0/255 |
| `HakuForest_08_Crouch.png` | 80×80 | Binary, 0/255 |
| `crouch_preview.png` | 960×400 | Binary, 0/255 |
| `HakuForest_07_Crouch_source.png` | 1254×1254 | Transparent with intermediate alpha |
| `HakuForest_08_Crouch_source.png` | 1254×1254 | Transparent with intermediate alpha |

All paths are under `art/deliveries/haku-hires-fix-v1/`. The two source PNGs are the selected built-in image-generation outputs. The preview shows idle, 07, and 08 in that order at 1× above and 4× below.

## Prompt summary

Used Haku idle, walk 03, and talk 09 as character and palette references, plus Iruka sit 08 as the crouch anatomy reference. Generated separate low squats facing right: 07 reaches toward a small herb while the other hand steadies the basket; 08 moves the herb into the basket. Requested a transparent background, bent knees, lowered hips, hair off the ground, and no loose elements. Converted the selected sources into 80×80 hard-pixel sprites; copied the idle head's opaque pixels 16 pixels downward and matched the existing idle palette with small herb and basket accents.

## Checks performed

- Visually inspected both source images and both final sprites at game scale; both read as crouching and face right.
- Final opaque bounds: 07 `(18, 32, 60, 77)`; 08 `(21, 32, 59, 77)`. Both are 45 pixels high, with the bottom opaque row at `y=76`.
- Verified both final PNGs have only alpha values 0 and 255 and exactly one 4-connected opaque component each.
- Verified every opaque pixel in the copied idle head region retains its original RGBA value after the 16-pixel downward shift.
- Ran `python3 scripts/pixel_noise.py`: 07 has 17/1153 isolated pixels (1.5%); 08 has 18/1126 (1.6%). Both are below 4%.
- Visually checked the hair ends above the ground, the basket sits by the body, and the 07 herb is a three-pixel green tip connected to the hand.

## Remaining game-side checks

- Import the two 80×80 PNGs into the mod and check their alignment and transitions with idle, walk, and talk in-game.
- Check the crouch sequence against the actual background and Terraria's display scale.
