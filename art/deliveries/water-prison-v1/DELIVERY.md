# water-prison-v1 delivery

- status: delivered
- request ID: `water-prison-v1`
- Generation: built-in `image_gen__imagegen`, using the requested Kakashi and Zabuza project references. Original generated PNGs were copied by filesystem path into `source/`. `make_frames.py` produces the game-size frames and previews from those source files.

## Delivered files

| File | Dimensions | Alpha |
| --- | --- | --- |
| `kakashi_trapped_0.png`, `kakashi_trapped_1.png` | 56×56 each | Transparent background; character alpha is binary |
| `zabuza_hold_0.png`, `zabuza_hold_1.png` | 288×128 each | Transparent background; character alpha is binary |
| `water_sphere_0.png`–`water_sphere_2.png` | 96×96 each | Transparent background; translucent sphere, center alpha 98/255 (38%) |
| `preview_composite.png` | 576×160 | Opaque, two 1× backdrop panels |
| `preview_composite_2x.png` | 1152×320 | Opaque, nearest-neighbor 2× preview |
| `source/kakashi_imagegen.png` | 1295×1214 | RGBA, transparent background |
| `source/zabuza_imagegen.png` | 1269×1240 | RGBA, transparent background |
| `source/water_sphere_imagegen.png` | 1254×1254 | RGBA, translucent source |

## Prompt summary

- Kakashi: match the existing Terraria NPC sprite; face left, float with curled legs, push an open palm against the prison; silver hair, headband, mask, green vest, navy clothes.
- Zabuza: match the existing sprite and style test; face right, stand steadily with an open outstretched palm, face bandages and executioner's sword on his back.
- Water sphere: clear pale freshwater blue, bright irregular rim and upper-left reflection, tiny bubbles, transparent exterior and translucent center.

## Checks performed

- Opened and visually inspected the three generated images, all game-size base frames, and the 2× composite preview.
- Verified PNG dimensions and alpha ranges with Pillow. Both character frame sets have hard binary alpha, with no semi-transparent fringe. Their transparent bounding boxes are Kakashi x=9–45, y=4–51 in frame 0 and Zabuza x=86–190, y=19–123 in both frames. Zabuza's feet end at y=123 (last occupied row), keeping the requested y=124 baseline.
- Verified Kakashi floats by one pixel between frames, Zabuza's cloth changes subtly without moving his feet or hand, and the three sphere frames differ in contour and rising highlights.
- Verified the water sphere is approximately 88 pixels wide, with 38% center opacity. Inspected the 1× and nearest-neighbor 2× composite on dark navy lake and pale sky colors. The sphere's left rim meets Zabuza's extended hand, and Kakashi remains readable inside it on both backdrops.

## Remaining game-side checks

- Import the seven game-size PNGs into the mod and verify layering, collision target positioning, frame cadence, and character baseline in a live Terraria scene. No game-side runtime test was performed by this art worker.
