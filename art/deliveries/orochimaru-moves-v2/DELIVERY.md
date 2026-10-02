status: delivered
request ID: orochimaru-moves-v2

## Delivered files

- `Orochimaru_Hands_0.png`, `Orochimaru_Hands_1.png`, `Orochimaru_Hands_2.png` — 112×88 each, transparent, alpha 0/255.
- `Orochimaru_Dash_0.png`, `Orochimaru_Dash_1.png` — 112×88 each, transparent, alpha 0/255.
- `Orochimaru_Wind_0.png`, `Orochimaru_Wind_1.png` — 112×88 each, transparent, alpha 0/255.
- `Orochimaru_Seal_0.png`, `Orochimaru_Seal_1.png` — 112×88 each, transparent, alpha 0/255.
- `Orochimaru_Summon_0.png`, `Orochimaru_Summon_1.png` — 112×88 each, transparent, alpha 0/255.
- `Orochimaru_Neck_0.png` — 112×88, transparent, alpha 0/255.
- `Orochimaru_NeckHead.png` — 48×40, transparent, alpha 0/255.
- `Orochimaru_NeckSegment.png` — 16×16, transparent, alpha 0/255; its left and right terminal columns match.
- `preview.png` — 1840×3110, opaque; each action's frames appear horizontally at 1× and 4× beside the accepted v3 Idle reference; detachable neck parts appear below.
- `source/Hands_generated.png` — 1954×805, transparent source with intermediate alpha.
- `source/Dash_generated.png` — 2172×724, transparent source with intermediate alpha.
- `source/Wind_generated.png` — 1774×887, transparent source with intermediate alpha.
- `source/Seal_generated.png` — 1536×1024, transparent source with intermediate alpha.
- `source/Summon_generated.png` — 1846×852, transparent source with intermediate alpha.
- `source/Neck_generated.png` — 1774×887, transparent source with intermediate alpha.
- `build_sprites.py` — reproducible source-to-game-size conversion and preview helper.

## Prompt summary

Used the built-in `image_gen__imagegen` tool with the accepted v3 Orochimaru images as design references. Generated six transparent pixel-art pose sheets: latent snake hands (extend, snakes emerge, retract), two low snake dashes, wind inhalation and exhalation, five-finger violet seal and palm strike, thumb bite/hand seal and ground summoning, and a headless long-neck body with separate biting head and repeatable neck segment. Prompts required right-facing poses, pale face, gold slit eye, purple makeup and waist rope, ivory tunic, dark trousers, and the approved pixel-art style.

## Checks performed

- Inspected the generated sheets and game-size preview against v3 Idle and the named Tazuna style reference.
- Sampled and adjusted poses on a 2×2 screen-pixel grid; repaired the gold eye/purple lid, summoning ground glyph, and seamless neck segment at art-pixel scale.
- Verified every game-size PNG's dimensions, alpha 0/255 only, and no mismatched 2×2 pixel blocks.
- Verified all twelve body frames face right and have lowest nontransparent pixel at y=83. Dash and crouched summoning poses are intentionally shorter; standing poses retain the v3 62-pixel height.
- Verified the 16×16 segment's left/right terminal columns are identical.

## Remaining game-side checks

- Integrate and inspect the poses at 1× in tModLoader during each move, particularly snake emission alignment, neck/head attachment and curvature, seal fingertips, and summoning glyph visibility.
- Confirm animation timing and any horizontal mirroring in the encounter. No build or in-game check was performed by this art worker.
