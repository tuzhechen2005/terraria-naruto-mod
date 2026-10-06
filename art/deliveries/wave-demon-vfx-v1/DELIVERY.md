status: delivered

request ID: `wave-demon-vfx-v1`

## Files

- `source/DemonVfxSheet.png` — generated 2×2 source sheet, 1536×1024 RGBA, real transparency and partial alpha.
- `sprites/DemonGhost.png` — 256×320 RGBA, partial alpha, anchor (128, 300).
- `sprites/DemonPress.png` — 256×320 RGBA, partial alpha, anchor (128, 300).
- `sprites/SwordSpinArc.png` — 192×192 RGBA, partial alpha, center (96, 96).
- `sprites/ChakraImpact.png` — 192×192 RGBA, partial alpha, center (96, 96).
- `previews/at_1x_dark.png`, `previews/at_1x_light.png` — 608×590 RGB contact sheets at actual 1× sprite size.
- `previews/short_loop.gif` — 608×590 RGB, twelve 100 ms frames, illustrative pose/trail/ring cycle.
- `export.py` — reproducible crop, scale, alpha cleanup, and preview export.
- `manifest.json` — anchors, facing, suggested timing, and decorative-only roles.
- `PROMPT.md` — full built-in image generation prompt.

## Prompt summary

One built-in transparent 2×2 sheet: a consistent purple demon chakra face in upright and pressing poses, an open bright-leading sword arc, and a hollow chakra impact ring. No characters, text, terrain, or extra attacks.

## Checks performed

- Inspected the generated sheet and both poses visually. Same horns, brow mark, eyes, and jaw read across Ghost and Press; Press extends toward image right.
- Inspected the dark and light 1× previews: both ghost faces and silhouettes remain distinct; the sword trail is open and the ring center remains clear.
- Confirmed exact output sizes, RGBA mode, soft partial alpha, and fully transparent RGB=(0,0,0) in all sprites and the cleaned source.
- Confirmed all sprite alpha bounds have transparent outer margins. Ghost and Press share (128,300) as the visual lower anchor.
- `python3 export.py` completed successfully. No game build or in-game test was run.

## Remaining game-side checks

- Composite around the actual Zabuza sprite on both bright and dark backgrounds, including real platforms and player silhouettes; tune draw order and opacity so the ninja remains visible.
- Verify Ghost→Press change and left/right flip follow the heavy slash, the arc follows existing sword travel on outbound and return paths, and the impact ring aligns with catch and berserk-start events.
- Confirm decoration never adds hitboxes or damage and that alpha and glow survive the game's sprite renderer without fringing.
- Check game performance and short-loop timing at 1× in the real camera view.
