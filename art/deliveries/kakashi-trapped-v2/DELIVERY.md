# kakashi-trapped-v2 delivery

- status: delivered
- request ID: `kakashi-trapped-v2`
- Delivered game assets:
  - `Kakashi_Trapped_0.png` — 56×56 RGBA, transparent background, alpha values 0/255 only.
  - `Kakashi_Trapped_1.png` — 56×56 RGBA, transparent background, alpha values 0/255 only.
- Source: `source/Kakashi_Trapped_generated.png` — 1774×887 RGBA, transparent background with antialiased alpha in the source image.
- Previews: `Kakashi_Trapped_preview_1x.png` (200×72 RGB) and `Kakashi_Trapped_preview_4x.png` (704×240 RGB). Each shows the v4 idle Kakashi beside both trapped frames.

## Prompt summary

Generated two right-facing, suspended Kakashi poses with silver hair, headband, face mask, green flak vest and dark clothing, using `kakashi-npc-v4` and `tazuna-npc-v1` as style references. He curls his legs, protects his chest with one arm and raises the other toward an implied water wall. Reduced the generated art to the v4 palette and 23-art-pixel body height. The delivered second game frame has a one-art-pixel hair-tip shift and a one-art-pixel raised-hand extension.

## Checks performed

- Visually inspected the generated source and the final 1× and 4× previews against the v4 idle sprite.
- Confirmed both game sprites are centered, face right, and occupy a 32×46-pixel visible bounding box within each 56×56 canvas.
- Confirmed binary alpha, exact 2×2 pixel blocks, and one connected visible silhouette per game frame; no isolated opaque pixels.
- Confirmed the two game frames differ at four art-pixel cells, limited to the hair tip and raised hand.

## Remaining game-side checks

- Load both frames behind the water-ball texture in `WaterPrison.DrawTrappedKakashi` and verify draw alignment, readability through the water, and animation timing in game.
