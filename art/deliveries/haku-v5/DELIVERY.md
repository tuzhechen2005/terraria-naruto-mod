# Delivery: haku-v5

- **status:** blocked
- **request ID:** haku-v5
- **Generation:** completed with the built-in image generation tool; the generated PNG candidates are copied below. They are not acceptance-ready and should not be imported as final game assets.

## Files and pose order

- `source/Haku_Idle.png` — 1983 × 793 px; RGBA with transparent background, but character edges/regions contain extensive partial alpha. Poses left to right: standard standing size reference, Idle 1, Idle 2, Idle 3, Idle 4.
- `source/Haku_Throw_Dash.png` — 2172 × 724 px; RGBA with transparent background, but character edges/regions contain extensive partial alpha. Poses left to right: standard standing size reference, Throw 1, Throw 2, Throw 3, Throw release (5th pose), Dash 1, Dash 2, Dash 3.
- `source/Haku_Emerge.png` — 1983 × 793 px; RGBA with transparent background, but character edges/regions contain extensive partial alpha. Poses left to right: standard standing size reference, Emerge 1 (half-hidden behind mirror), Emerge 2, Emerge 3, Emerge 4 (fully emerged).
- `source/Haku_IceMirror_Senbon.png` — 2048 × 768 px; RGBA with transparent background and partial alpha in the image; objects left to right: intact mirror, cracked mirror, shattered mirror, right-facing senbon.

## Prompt summary

Generated a right-facing masked Haku with the specified teal and olive outfit, red mask markings, broader torso, and reference-matching angular cel-shaded appearance in source sheets for Idle, Throw/Dash, and Emerge. Generated the three ice-mirror states and a right-facing senbon in a separate object sheet. Prompts requested true transparency, coarse 4×4 pixel clusters, crisp hard edges, and opaque character pixels.

## Checks performed

- Opened and visually inspected only the project references named in the request: `style-test-v2/source/Zabuza_style.png`, `style-test-v2/source/Haku_style.png`, both named 1x samples, and the three `art/reference/` images.
- Visually inspected all selected generated PNGs for pose order, facing, mask markings, silhouette, gaps, and object readability.
- Confirmed PNG dimensions and RGBA encoding; decoded alpha values with a PNG filter-aware check. Fully transparent pixels exist, but the character sheets contain widespread partial-alpha pixels and far fewer fully opaque pixels than required (365, 409, and 351 opaque pixels in the complete Idle, Throw/Dash, and Emerge canvases respectively). The object sheet has no fully opaque pixels and also contains partial alpha.
- The generated rendering remains substantially smoother and more textured than the requested coarse, limited-color pixel art. The 4×4 pixel-grid and fully opaque character requirements therefore fail inspection; the source sheets are retained as candidates, not approved finals.

## Remaining game-side checks

- Regenerate or correct the sources to meet the hard pixel-grid style and make all Haku pixels fully opaque with a clean transparent background.
- Once compliant sources exist, downscale with `scripts/pixelize_frames.py` and inspect 1× silhouettes, direction, mask-mark readability, and consistency beside the approved Haku/Zabuza samples and Zabuza batch 1.
- Verify mirror translucency and readability and senbon silhouette at their requested final sizes (48×72 and 24×6), then perform in-game checks.
