# Delivery: npc-hires-sample-v3b

- status: delivered
- request ID: `npc-hires-sample-v3b`

## Delivered files

| File | Dimensions | Alpha |
| --- | ---: | --- |
| `Tazuna_Idle.png` | 80×80 | RGBA, 0/255 only |
| `Iruka_Idle.png` | 80×80 | RGBA, 0/255 only |
| `Kakashi_Idle.png` | 80×80 | RGBA, 0/255 only |
| `source/Tazuna_Generated.png` | 1227×1282 | RGBA, transparent with partial edge alpha |
| `source/Iruka_Generated.png` | 1222×1287 | RGBA, transparent with partial edge alpha |
| `source/Kakashi_Generated.png` | 1230×1278 | RGBA, transparent with partial edge alpha |
| `preview.png` | 1000×460 | RGB comparison sheet |
| `faces_8x.png` | 800×290 | RGB, 8× nearest-neighbor faces with grid |

`make_sprites.py` is the reproducible pixel-art and preview assembly script. The high-resolution source images were generated with the built-in image tool, using the v2b generated images as references. The game-size sprites retain the v2b color-block style and were redrawn for a narrow, right-facing three-quarter silhouette.

## Prompt summary

- Tazuna: right-facing, elderly white-haired builder with small round glasses, visible eyes, white beard and moustache, towel, vest, and sake bottle.
- Iruka: right-facing smiling ninja with swept-back ponytail, clean chin, narrow bridge scar, forehead protector, green vest, and scroll.
- Kakashi: right-facing masked ninja with backward-swept silver hair, diagonally worn headband covering the rear eye, one exposed lazy eye, green vest, and orange book.
- Shared: full standing figure, transparent background, deliberate pixel-art color blocks, dark tinted outlines, hard shading, upper-left light, and no text or scenery.

## Checks performed

- Inspected the generated source images and the final `preview.png` and `faces_8x.png` visually.
- All game sprites are 80×80 with bounding boxes `Tazuna (25,16,56,77)`, `Iruka (22,16,55,77)`, `Kakashi (24,15,56,77)`; therefore their lowest opaque pixel is y=76 and their heights are 61, 61, and 62 pixels respectively.
- All game-sprite alpha values are exactly 0 or 255. All three have fewer than 20 colors.
- `python3 scripts/pixel_noise.py` output:

```text
Tazuna_Idle.png: 1381 px, 18 colours, 27 isolated (2.0%)
Iruka_Idle.png: 1217 px, 16 colours, 15 isolated (1.2%)
Kakashi_Idle.png: 1249 px, 14 colours, 33 isolated (2.6%)
```

- Compared the sprites with the vanilla Guide reference at the same character height in the preview sheet, at 1× and 3×.

## Remaining game-side checks

- Import the sprites into the mod and verify actual 1× readability, idle placement, walking direction, frame alignment, and whether each face remains legible on the game's backgrounds.
- Confirm that the game-side character proportions and held items read correctly during motion; this delivery contains still standing frames only.
