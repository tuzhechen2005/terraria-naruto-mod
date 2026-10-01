# mod-icon-v5 delivery

- status: delivered
- request ID: `mod-icon-v5`
- backend: built-in `image_gen__imagegen`, using the imagegen skill through the art bridge.
- intent: edit the existing v4 cover's wordmark to match the user-selected name **Terruto**.

## Files

| Path | Dimensions | Mode / alpha |
| --- | ---: | --- |
| `source/edited-icon.png` | 1254×1254 | RGB, opaque |
| `icon.png` | 80×80 | RGB, opaque |
| `icon_small.png` | 30×30 | RGB, opaque |
| `readme-icon.png` | 640×640 | RGB, opaque |
| `preview.png` | 1060×1000 | RGB, opaque |

## Edit brief supplied to the image worker

> Use case: text-localization. Edit target: the existing v4 high-resolution pixel-art cover. Replace only the bottom orange-gold stone wordmark NARUTO with TERRUTO, spelled T E R R U T O, seven uppercase letters. Fit the whole word in the existing title area using the same heavy pixel lettering, gold highlights, orange shading, dark dimensional outlines and stone base. Keep Naruto, Kakashi, their faces, clothing, poses and positions, the hand touching the Rasengan, blue energy, orange chakra, Hidden Leaf Village, Hokage Rock, trees, buildings, frame and corner ornaments. Keep the square composition and opaque illustrated background. No other text or leftover letters from the original wordmark.

The full request is in `art/requests/mod-icon-v5.md`. The generated PNG was inspected and copied into `source/edited-icon.png`; the wordmark itself was edited by the built-in image tool, not drawn with a script or font.

## Sampling and visual checks

- Full cover: Lanczos sampling to 80×80, then 64-colour median-cut quantization without dithering.
- Small icon: the same normalized square character crop as v4 (x=163–1129, y=25–991 on a 1254×1254 source), sampled to 30×30 and 48 colours without dithering. The title is outside this crop.
- README: sampled to 160×160 and 128 colours without dithering, then enlarged 4× with nearest-neighbour sampling.
- Verified PNG decoding, dimensions, RGB mode and palette limits.
- Inspected the source and size preview: the wordmark reads **TERRUTO** with both Rs and all seven letters visible in the README and 80×80 versions. Naruto, Kakashi, the Rasengan and village retain the existing composition. The full wordmark fits within the stone frame.
- Checked the 80×80 and 30×30 versions on white and GitHub-dark backgrounds.

## Workflow note

The art worker completed the built-in image edit, then stopped making progress during size export. The developer stopped that worker and completed sampling and preview layout from its generated image. No replacement image was generated and no script redrew the wordmark. The bridge's local state records this developer-completed delivery explicitly.

## Integration and remaining checks

The user explicitly authorized the name and corresponding wordmark change. The developer copied the game-size files to the mod root and the README file to `docs/images/mod-icon.png`. Both GitHub Markdown/browser previews passed: the heading is Terruto, the new cover displays at 320×320, and there is no horizontal overflow. `scripts/verify-mac.sh` ran with a separate save directory; all 13 rule suites and mod packaging passed, with zero build errors. Existing warnings remain. In-game display after reloading has not been checked.
