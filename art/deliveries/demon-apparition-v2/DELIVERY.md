# Delivery: demon-apparition-v2

- status: delivered
- request ID: `demon-apparition-v2`
- Delivered files:
  - `source/demon-apparition-hannya-generated.png` — selected imagegen source, 1644×957 px, RGBA with transparency.
  - `demon-apparition-hannya-sheet.png` — game-size sprite sheet, 416×204 px, RGBA with binary transparency. Each frame cell is 80×100 px with 4 px transparent gutters.
- Frame order: Top row has one solid cyan reference rectangle followed by loop frames 1–4. Bottom row has one solid cyan reference rectangle followed by emergence frames 1–3. The final bottom cell is transparent and unused.
- Prompt summary: Used the v1 sheet for frame layout and purple chakra flame texture, and the two requested screenshots for mood and palette. Replaced the broad western demon face with a frontal, tall and narrow Japanese Hannya mask: long thin upcurving forehead horns, furrowed raised brows, slanted glowing eyes, wide upturned grin, upper and lower pointed teeth, and a tapered chin and lower flame column. Kept dark purple, violet, magenta, and pale pink-white; excluded red, characters, text, and background.
- Checks performed: Opened both named screenshots and the v1 source and game-size sheet. Inspected the generated source and final 1× sheet visually. Verified frontal direction, Hannya silhouette, visible eyes and teeth at game scale, 80×100 cells, two solid cyan reference cells, seven artwork cells, transparent gutters, empty unused cell, and alpha values of only 0 or 255 in the game-size sheet. Resized with nearest-neighbor sampling.
- Remaining game-side checks: Composite behind Zabuza in tModLoader and check scale, layering, vertical alignment, contrast against gameplay backgrounds, and the four-frame loop and three-frame emergence timing. Adjust placement or frame timing after in-game review.
