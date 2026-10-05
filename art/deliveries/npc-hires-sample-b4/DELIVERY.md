status: delivered
request ID: npc-hires-sample-b4

## Delivered files

- `Anko_Idle.png` — 80×80 RGBA, binary alpha (transparent background).
- `Hayate_Idle.png` — 80×80 RGBA, binary alpha (transparent background).
- `source/Anko_source.png` — 1254×1254 RGBA, transparent background with soft edge alpha; built-in image generation source.
- `source/Hayate_source.png` — 1254×1254 RGBA, transparent background with soft edge alpha; built-in image generation source.
- `preview.png` — 2160×386 RGBA, opaque comparison of the seven accepted sprites and both b4 sprites at 1× and 3×.
- `faces_8x.png` — 600×328 RGBA, 8× face crops with one-pixel grid and row numbers.
- `_process.py` — reproducible 80×80 refinements and preview assembly.

## Prompt summary

Built-in `image_gen__imagegen` edited each b3 sprite using a contact sheet of all seven accepted sprites as style reference. Anko's prompt exposed her face, shortened the bangs, retained the upturned ponytail, and added the forehead protector, bright eyes, and one-sided smile. Hayate's prompt preserved his tired face and blue underclothes while adding an Iruka-palette green vest and diagonal sword grip. The selected generated PNGs were copied from `$CODEX_HOME/generated_images` into `source/`. The game-size sprites were refined pixel by pixel from the b3 80×80 sprites, informed by the generated sources.

## Checks performed

- Inspected both generated source PNGs, both 80×80 sprites, `preview.png`, and `faces_8x.png` visually.
- Both sprites face right in three-quarter view, use a large-head silhouette, occupy y=16–76 (61 pixels high), and put the feet on y=76.
- Anko's forehead plate, exposed face, eyes, smile, and ponytail are visible at 3×; Hayate's dark eye circles, green vest, blue underclothes, and shoulder sword grip are visible at 3×.
- `python3 scripts/pixel_noise.py`: Anko 963 opaque pixels, 15 colors, 15 isolated pixels (1.6%); Hayate 1314 opaque pixels, 20 colors, 25 isolated pixels (1.9%). Both are below 3%.
- Verified 80×80 images have only alpha 0 or 255. The two previews are for inspection, not game sprites.

## Remaining game-side checks

- Integrate the two idle sprites in the mod and inspect them in Terraria at native game scale against the accepted NPCs.
- Confirm the character names, facing, draw offset, and visual clarity in the actual lighting/backgrounds. No game build or in-game acceptance was run by the art worker.
