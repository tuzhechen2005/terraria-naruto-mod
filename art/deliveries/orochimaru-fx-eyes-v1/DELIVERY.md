# Orochimaru killing eyes — delivery

- status: delivered
- request ID: `orochimaru-fx-eyes-v1`
- delivered files:
  - `FxKillingEyes.png` — game texture, 192×48 RGBA, alpha values only 0 and 255.
  - `source/FxKillingEyes_generated.png` — selected built-in imagegen source, 2172×724 RGBA, variable alpha.
  - `preview.png` — 1000×430 opaque RGB preview with the single static frame at 1× and 4× on dark and light backgrounds.
- Prompt summary: A horizontally balanced pair of cold golden snake eyes with vertical pupils, thick purple and near-black eye makeup, small pale skin areas, chunky pixel-art outlines, hard color steps, and a transparent background. No face, text, glow, particles, or scenery.
- Checks performed: Inspected the generated source and both project visual references. Reduced the chosen image to a 96×24 art grid, mapped it to a restricted hard-step palette, removed detached pixels, added transparent perimeter padding, and enlarged each art pixel to a 2×2 screen block. Visually checked 1× and 4× previews on dark and light backgrounds. Verified the game PNG is 192×48, has only alpha 0/255, has no inconsistent 2×2 blocks, and its nontransparent bounds are (6, 2)–(186, 46).
- Remaining game-side checks: Confirm overlay position and 2–3× scaling during the killing-intent stun in Terraria; confirm it remains legible against live scenes and disperses correctly when substitution breaks the stun. No game runtime check was performed by the art worker.
