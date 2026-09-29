# Delivery: zabuza-v7-base

- status: delivered
- request ID: `zabuza-v7-base`
- Generation: built-in `image_gen__imagegen`; generated PNGs copied by filesystem path from `$CODEX_HOME/generated_images`.

## Delivered files

| File | Dimensions | Alpha status | Pose order, left to right |
|---|---:|---|---|
| `source/Zabuza_Idle.png` | 2032×774 | RGBA; transparent background pixels present; partial alpha present (40.32% of canvas pixels are semitransparent) | Standard idle scale reference, then Idle frames 1–4 |
| `source/Zabuza_Windup_Slash_Seal.png` | 2172×724 | RGBA; transparent background pixels present; partial alpha present (32.44% of canvas pixels are semitransparent) | Standard idle scale reference; Windup 1–3; Slash 1–3; Seal 1–2 |
| `source/Zabuza_Run_Leap.png` | 2172×724 | RGBA; transparent background pixels present; partial alpha present (30.03% of canvas pixels are semitransparent) | Standard idle scale reference; Run 1–6; Leap 1–2 |
| `source/Zabuza_Dash.png` | 2172×724 | RGBA; transparent background pixels present; partial alpha present (30.83% of canvas pixels are semitransparent) | Standard idle scale reference; Dash 1–2 |

Alpha percentages were measured by decoding PNG scanlines. The generated art has substantial partial alpha, so the requirement that all character pixels be fully opaque is **not met**. Do not assume these are ready to use as final game frames without edge-alpha cleanup.

## Prompt summary

Generated four horizontal transparent character sheets from the named Zabuza style and canon references. Prompts specified a right-facing masked Zabuza in black clothing, his broad notched sword, a repeated idle reference pose at the far left, consistent per-sheet scale, the requested ordered poses, angular mature proportions, stepped dark outlines, and hard-edged limited cel shading. No text or scene elements were requested.

## Checks performed

- Opened and visually inspected all seven reference images named in the request.
- Visually inspected each generated sheet at full size: pose order and silhouette separation, right-facing orientation, black outfit readability, sword shape, feet and hands, and requested motion poses.
- Checked PNG format, dimensions, RGBA channel presence, transparency coverage, and partial-alpha coverage.
- No game-size frames were made, as requested.
- Limitation: although the prompts requested discrete pixel art and opaque character pixels, the generated output retains softened/partially transparent edges and smoother detail than the pixel-art samples. Pose consistency and precise game-scale readability still need review after pixelization.

## Remaining game-side checks

- Clean partial edge alpha and pixelize frames to the mod's intended 4× art-pixel scale; verify the silhouette and fully opaque character pixels.
- Compare the standard idle reference against the style-test sample after scaling to 84 px tall; confirm dark-clothing readability on a dark in-game background.
- Check per-frame alignment, animation timing, weapon clearance, hitbox fit, and appearance in Terraria/tModLoader.
