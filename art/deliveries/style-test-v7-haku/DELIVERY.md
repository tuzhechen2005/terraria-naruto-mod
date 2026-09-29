# Delivery: style-test-v7-haku

- status: delivered
- request ID: style-test-v7-haku
- delivered files:
  - `source/Haku_style.png` — 168 × 304 px, RGBA; binary alpha (0 or 255). Exact 8 × 8 blocks per art pixel.
  - `Haku_style_final.png` — 21 × 38 px, RGBA; binary alpha (0 or 255). One screen pixel per art pixel for import; display at 2× for the requested game scale.
  - `Haku_mask_comparison_4x.png` — 72 × 40 px, RGBA; fully opaque comparison of the v6 mask (left) and v7 mask (right), each cropped mask region enlarged 4× with nearest-neighbor scaling.
  - `source/imagegen_mask_concept.png` — 932 × 1688 px, RGBA; contains partial alpha. Built-in image generation output retained as a mask-design concept, not a game-ready sprite.
- prompt summary: Edited the v6 Haku sprite using its image as the edit target and `art/reference/Haku_s_shinobi_attire.png` solely as a mask design reference. Requested a full white face mask with a dark rim, bright red forehead and lower-face marks, and a narrow dark eye slit while retaining pose, clothes, palette, and right-facing silhouette. The built-in image generation result guided the final 21 × 38 pixel mask revision. The final sprite retains every v6 pixel outside the face region.
- checks performed: Visually inspected the generated concept, final sprite, 8× source, and mask comparison. Verified final/source dimensions, binary alpha, exact nearest-neighbor 8× blocks, unchanged alpha silhouette, and that all 31 changed art pixels are within face coordinates x=13–17 and y=4–11. Checked mask contrast and red-mark readability at the final 21 × 38 size and right-facing direction.
- remaining game-side checks: Import into tModLoader and verify 2× display scale, mask readability during play, facing direction, and animation/frame integration beside the current Zabuza sprite.
