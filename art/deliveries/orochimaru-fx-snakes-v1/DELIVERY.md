status: delivered
request ID: orochimaru-fx-snakes-v1

## Delivered files

| File | Dimensions | Alpha |
| --- | ---: | --- |
| `FxSnakeHand_Head.png` | 24×16 | RGBA, 0/255 |
| `FxSnakeHand_Segment.png` | 10×10 | RGBA, 0/255 |
| `FxBite_0.png` | 48×48 | RGBA, 0/255 |
| `FxBite_1.png` | 48×48 | RGBA, 0/255 |
| `FxBite_2.png` | 48×48 | RGBA, 0/255 |
| `source/generated_snake_fx_sheet.png` | 2172×724 | RGBA source with intermediate alpha values |
| `preview.png` | 1080×520 | RGB preview on dark/light backgrounds |

All paths are relative to `art/deliveries/orochimaru-fx-snakes-v1/`.

## Prompt summary

Built-in `image_gen__imagegen` generated a transparent, five-part pixel-art source sheet: a right-facing open-mouth purple-black snake head with gold eye and ivory fangs; a repeating purple-black body section; closing white fang rows; a purple impact burst; and two fading purple-red bite marks. Requested dark outlines, hard color steps, upper-left light, and no gradients or stray particles. The selected source was copied from `$CODEX_HOME/generated_images` to `source/`.

## Checks performed

- Inspected the requested Tazuna, Orochimaru, and Zabuza reference images, generated source, and final preview.
- Sampled the generated art to the requested dimensions, repaired eye/fang pixels, rebuilt the body tile so its left and right cross-sections match, and removed stray soft-edge alpha.
- Checked all five final PNGs have only alpha 0 or 255 and each 2×2 pixel block is uniform.
- Checked the head faces right, the bite frames read in sequence, and all five sprites remain visible at 1× and 4× on dark and light backgrounds in `preview.png`.

## Remaining game-side checks

- Confirm the head attaches to the repeated body without a visible seam during extension, twisting, and retraction.
- Confirm all three bite frames align with actual hit points and remain readable against Terraria backgrounds at the game's display scale.
- Confirm sprite orientation, draw order, frame timing, and collision presentation in a live tModLoader build. No game-side build or playtest was run by this art worker.
