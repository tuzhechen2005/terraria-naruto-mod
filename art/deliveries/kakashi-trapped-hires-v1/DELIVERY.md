status: delivered
request ID: kakashi-trapped-hires-v1

## Delivered files

- `Kakashi_Trapped_0.png` — 80×80 RGBA game frame; alpha only 0/255; 14 opaque RGB colors.
- `Kakashi_Trapped_1.png` — 80×80 RGBA game frame; alpha only 0/255; 14 opaque RGB colors.
- `Kakashi_Trapped_0_Source.png` — 1254×1254 RGBA original generated source; graduated alpha (256 values).
- `Kakashi_Trapped_1_Source.png` — 1254×1254 RGBA original generated source; graduated alpha (256 values).
- `preview.png` — 960×400 opaque RGBA comparison; top row at 1×, bottom row at 4× nearest-neighbor scale. The approved standing sprite appears in the third column.

## Prompt summary

Built-in `image_gen__imagegen` generated two transparent pixel-art poses using the approved high-resolution Kakashi idle and jump sprites for identity, costume, palette and style, and the old trapped sprites for pose guidance. Both frames show a strained, curled Kakashi pushing against an implied wall, with a raised eye, bent legs, and upward-drifting hair and headband cloth. The second frame changes the pushing hand, legs and hair for a short loop. No water sphere or backdrop was requested or drawn. The game frames were centered and mapped to the approved idle sprite's 15-color palette, with 14 colors used in each final frame.

## Checks performed

- Inspected both generated sources and the 1×/4× preview against the two approved standing/jumping references.
- Final frame dimensions: 80×80 each; character bounds: frame 0 `(19,10)–(60,70)`, frame 1 `(18,9)–(62,71)`.
- Verified final game frame alpha channels contain only 0 and 255; background is transparent.
- Verified pose direction, pushing-arm silhouette, central placement, and readability in the 1× preview.
- `python3 scripts/pixel_noise.py`:
  - `Kakashi_Trapped_0.png`: 1219 opaque pixels, 14 colors, 33 isolated pixels (2.7%).
  - `Kakashi_Trapped_1.png`: 1286 opaque pixels, 14 colors, 47 isolated pixels (3.7%).

## Remaining game-side checks

- Integrate the two 80×80 game frames, then inspect both over the separately rendered water sphere in Terraria.
- Check animation alignment, collision/display bounds, and facing behavior during the encounter. No game-side or build verification was performed by this art worker.
