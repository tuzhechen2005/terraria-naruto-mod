# mod-icon-v4 delivery

- status: delivered
- request ID: `mod-icon-v4`
- generated with: built-in `image_gen__imagegen` through the imagegen skill

## Files

| Path | Dimensions | Alpha |
| --- | ---: | --- |
| `source/generated-icon.png` | 1254×1254 | None; opaque RGB |
| `icon.png` | 80×80 | None; opaque RGB |
| `icon_small.png` | 30×30 | None; opaque RGB |
| `readme-icon.png` | 640×640 | None; opaque RGB |
| `preview.png` | 1060×1000 | None; opaque RGB |

## Prompt summary

Original, richly layered square pixel-art game cover for Naruto Part I. Young Naruto fills the center-left foreground, with his hand physically connected to a bright blue Rasengan; Kakashi stands behind him at right. The background has Hidden Leaf rooftops, trees, and Hokage Rock. Hard-edged orange and blue chakra, a narrow textured dark border, and a custom orange-gold `NARUTO` pixel wordmark complete the composition. The prompt required grouped pixel shading, clear faces and silhouettes, an opaque illustrated background, and no other text, photography, 3D rendering, or watermark. The four named images in the request were used as visual references only; no reference pixels were copied into the result.

## Sampling and checks

- Copied the selected built-in generated PNG to `source/generated-icon.png`.
- Resampled the whole cover with Lanczos to 80×80, then quantized without dithering to **64 colors** for `icon.png`.
- Cropped the main character/action region from the same source (source rectangle x=163–1129, y=25–991), resampled to 30×30, and quantized without dithering to **48 colors** for `icon_small.png`. The small icon intentionally excludes the wordmark.
- Resampled the full cover to 160×160, quantized to 128 colors without dithering, then enlarged with nearest-neighbor scaling to 640×640 for `readme-icon.png`.
- Opened and visually checked the source and `preview.png`: Naruto's eyes, hair, forehead protector, whisker marks and orange jacket remain distinguishable; Kakashi's hair, mask and vest remain separate; Naruto's hand visibly meets the blue Rasengan. The `NARUTO` letters remain legible at 80×80. At 30×30, the character silhouettes and orange/blue contrast remain visible, though fine facial details are necessarily reduced.
- Checked white and GitHub-dark backgrounds in the preview. Checked PNG dimensions, RGB mode and color counts by script. No transparency is present or required because the artwork has a complete opaque background.

## Integration (2026-10-01)

- The user approved replacement after reviewing the preview. The developer copied the 80×80 and 30×30 PNGs to the mod root, and the 640×640 README asset to `docs/images/mod-icon.png`, without altering the delivered artwork.
- Both READMEs display the cover at 320×320. GitHub Markdown rendering and browser previews passed; the cover loads at the intended size with no horizontal overflow.
- The developer ran `scripts/verify-mac.sh` with a separate tModLoader save directory: all 13 rule suites and mod packaging passed, with zero build errors. Existing compiler warnings and the image-conversion fallback log remain.
- Actual icon display after an in-game reload has not been checked. The art worker itself did not run the build or game acceptance.
