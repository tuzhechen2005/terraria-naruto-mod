# style-icons-v1 delivery

- status: delivered
- request ID: `style-icons-v1`

## Delivered files

| Game-size PNG | Dimensions | Alpha |
| --- | --- | --- |
| `source/Style_Sharingan.png` | 64 × 64 | Binary transparency (0 or 255) |
| `source/Style_EightGates.png` | 64 × 64 | Binary transparency (0 or 255) |
| `source/Style_Byakugan.png` | 64 × 64 | Binary transparency (0 or 255) |
| `source/Style_Sage.png` | 64 × 64 | Binary transparency (0 or 255) |

The selected imagegen originals are preserved as `source/imagegen/Style_Sharingan_generated.png`, `source/imagegen/Style_EightGates_generated.png`, `source/imagegen/Style_Byakugan_generated.png`, and `source/imagegen/Style_Sage_generated.png`. Each original is 1254 × 1254 RGBA and contains partial alpha. `qa/color-and-silhouette.png` is a 512 × 256 comparison preview.

## Prompt summary

Generated four Terraria-style pixel-art icons: a purple concentric Rinnegan eye in a horned Susanoo frame; a ninja in red Death Gate steam before a dragon-shaped chakra form; a pale cyan Tenseigan eye with a floral pupil and branching veins; and a golden chakra-clad ninja with nine flame-like tails. Requested transparent backgrounds, hard-edged pixels, dark same-hue outlines, and compact color ramps. The Sage image was edited with imagegen to add the two lower tails.

## Checks performed

- Inspected the named Zabuza, Haku, and Terraria sprite references and each generated image.
- Converted the selected originals to 64 × 64 using a 32 × 32 logical pixel grid, nearest-neighbor enlargement, a 16-color palette, and binary alpha.
- Confirmed each game-size PNG is RGBA, has only alpha values 0 and 255, and uses 2 × 2 screen-pixel blocks.
- Visually compared all four icons at game size and as solid-black silhouettes in `qa/color-and-silhouette.png`. Their horned eye, dragon/ninja, branching eye, and nine-tail fan outlines remain distinct.

## Remaining game-side checks

- Integrate the four `source/Style_*.png` files into the handbook UI and inspect at the actual UI scale and on light/dark backgrounds.
- Verify the locked-state black silhouette and unlocked colors in game. No game build or in-game acceptance test was run by the art worker.
