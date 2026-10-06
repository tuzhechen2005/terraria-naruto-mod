status: delivered
request ID: npc-hires-sample-v4a

## Delivered files

- `Iruka_Idle.png` — 80×80 RGBA game sprite; alpha values 0/255.
- `Kakashi_Idle.png` — 80×80 RGBA game sprite; alpha values 0/255.
- `source/Iruka_Generated.png` — 1254×1254 RGBA imagegen source; transparent background with partial alpha at softened edges.
- `source/Kakashi_Generated.png` — 1254×1254 RGBA imagegen source; transparent background with partial alpha at softened edges.
- `preview.png` — 810×380 opaque RGB review image: v3b Tazuna plus v4a Iruka and Kakashi at 1× and 3×.
- `faces_8x.png` — 680×255 opaque RGB review image: Iruka and Kakashi heads at 8× with grid and source sprite row numbers 16–38.

## Prompt summary and sprite translation

Used the built-in image generation tool in transparent edit mode, with the respective v3b 80×80 sprites as image references. The Iruka prompt preserved the ponytail, protector, vest, scroll, pose and palette while separating the bridge scar from a small raised-corner smile. The Kakashi prompt preserved the rearward silver spikes, body and palette while shifting the headband over the rear eye, enlarging the visible half-lidded eye's skin area and defining the mask edge. The selected generated files are copied unchanged to `source/`. The game-size sprites were translated by precise pixel edits to the v3b sprites, retaining their established palette and silhouette.

## Checks performed

- Inspected both generated sources and the 1×, 3× and 8× review images visually.
- Both game sprites are 80×80, with foot bottoms at y=76; both face right in three-quarter view.
- Both game sprites use only fully transparent or fully opaque pixels. Iruka has 16 opaque RGB colours; Kakashi has 15, each under the 20-colour limit.
- `python scripts/pixel_noise.py`: Iruka 1217 opaque pixels, 17 isolated (1.4%); Kakashi 1258 opaque pixels, 40 isolated (3.2%). Both below 4%.
- The source images are concept sources and have partial alpha; only the two 80×80 game sprites are intended for direct game use.

## Remaining game-side checks

- Integrate the two game sprites in the mod and verify their in-game scale, direction, face readability, and animation alignment in tModLoader. No game build or in-game check was run by the art worker.
