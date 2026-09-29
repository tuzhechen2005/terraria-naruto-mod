# Delivery: demon-apparition-v1

- status: delivered
- request ID: `demon-apparition-v1`
- Delivered files:
  - `source/demon-apparition-generated.png` — selected generated source image, 1644×957 px, RGBA with transparency.
  - `demon-apparition-sheet.png` — game-size sprite sheet, 456×196 px, RGBA with transparency. Each frame cell is 88×96 art pixels; 4 px gutters separate cells. The first cell in each row is the solid cyan size reference.
- Frame order:
  - Top row, left to right after the reference cell: loop frames 1–4.
  - Bottom row, left to right after the reference cell: emergence frames 1–3; final cell is transparent and unused.
- Prompt summary: Original transparent pixel-art purple chakra demon apparition based on the requested visual references. Four subtly varied sustained demon-face frames and three forming/rising frames, with flame horns, narrow pale-pink eyes, a fanged mouth, torn cheek flames, and a tapering lower flame column. Used dark violet, purple, magenta, and pale pink; excluded red and characters.
- Checks performed: Opened and visually inspected both demon-mode references and the named C-version Zabuza style reference. Inspected the generated source and the game-size sheet. Confirmed the sheet dimensions, RGBA alpha channel, transparent artwork background, two cyan reference cells, seven ordered artwork cells, and nearest-neighbor resizing for crisp pixel edges. The sheet places the effects within consistent 88×96 cells with transparent gutters.
- Remaining game-side checks: Composite behind the C-version Zabuza sprite and verify layering, scale, alignment to the character, flame visibility around the character, and perceived threat at normal gameplay zoom. Check the four-frame loop transition and emergence timing in tModLoader; tune frame timing or poses if needed.
