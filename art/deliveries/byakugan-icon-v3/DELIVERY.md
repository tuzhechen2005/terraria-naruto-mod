# byakugan-icon-v3 delivery

- status: delivered
- request ID: `byakugan-icon-v3`
- delivered files:
  - `ByakuganCore.png` — 32×32 RGBA game icon; transparent background; alpha values are only 0 and 255.
  - `source/ByakuganCore-source.png` — 1254×1254 RGBA generated source; transparent background with some semitransparent edge pixels.
  - `preview.png` — 608×164 RGB preview; SharinganCore1 and ByakuganCore side by side at 4× on dark and light backgrounds.
- prompt summary: Isolated activated Byakugan eye with luminous pale lavender iris, thin pale pupil ring and radial rays, thick dark upper lid and lowered brow, a narrow strip of gray skin, and blue-violet veins. Transparent pixel-art composition without the outer purple ring, flames, vortex, or full face. Generated with the built-in `image_gen__imagegen` tool.
- checks performed: Opened the user reference and SharinganCore1; visually inspected the generated source and the 4× preview; checked the 32×32 silhouette, eye direction, and readability beside SharinganCore1 on dark and light backgrounds; confirmed the final icon is centered within bounds `(1, 3, 31, 29)` and has no semitransparent pixels.
- remaining game-side checks: Import `ByakuganCore.png` in the mod and confirm appearance in the Terraria inventory at 1× scale, including on the actual inventory background. No in-game check was run by the art worker.
