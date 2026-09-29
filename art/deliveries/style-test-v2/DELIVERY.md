# Delivery: style-test-v2

- Status: delivered
- Request ID: `style-test-v2`

## Delivered files

- `source/Zabuza_style.png` — 328 × 336 px; RGBA, transparent background; character pixels alpha 255, background alpha 0.
- `Zabuza_style_1x.png` — 82 × 84 px; RGBA, transparent background; character pixels alpha 255, background alpha 0.
- `source/Haku_style.png` — 220 × 288 px; RGBA, transparent background; character pixels alpha 255, background alpha 0.
- `Haku_style_1x.png` — 55 × 72 px; RGBA, transparent background; character pixels alpha 255, background alpha 0.

The `source/` images are exact 4× nearest-neighbor enlargements of the corresponding game-size images, so every game pixel occupies a 4 × 4 block.

## Prompt summary

Generated two full-body, right-facing Naruto character idle sprites against transparency. Zabuza has his olive sleeveless outfit, bandage mask, Mist forehead protector, striped guards, sandals, and large shoulder-carried sword with circular hole. Haku has the white mask with red markings, dark hair with teal hairpin, teal and cream kimono, olive-brown hakama and sash, geta, and visible senbon. Both use sturdy 5–5.5-head proportions, angular silhouettes, bold outlines, and hard cel shading with pixel clusters intended for Terraria scale.

## Checks performed

- Inspected only the references named by the request: style-test-v1 sprites and in-game comparison, existing Terraria idle sprites, and the three named character reference images.
- Visually inspected both generated candidates and selected the versions with the clearest silhouettes and signature props.
- Cropped transparent margins, reduced with nearest-neighbor to 84 px (Zabuza) and 72 px (Haku) character height, then enlarged 4× with nearest-neighbor.
- Checked the final 1× and 4× images visually for right-facing direction, whole-character silhouette, sword hole, mask markings, senbon, and readability at game scale.
- Checked PNG dimensions and alpha: only alpha 0 and 255 remain; all visible pixels are fully opaque. The 4× files align exactly to the 1× pixel grid.

## Remaining game-side checks

- Import each 1× PNG into Terraria/tModLoader and compare side by side with style-test-v1 and the existing idle sprites in-game.
- Confirm sprite positioning, hitbox alignment, and readability against the game's actual background and animation/rendering pipeline.
