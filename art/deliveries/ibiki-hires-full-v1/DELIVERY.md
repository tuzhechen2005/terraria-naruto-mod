# DELIVERY

- status: delivered
- request ID: `ibiki-hires-full-v1`
- Generated with: built-in `image_gen__imagegen` using the requested `Ibiki_Idle.png` as the visual reference. No API key or fallback CLI was used.

## Files

| File | Dimensions | Alpha |
| --- | --- | --- |
| `Ibiki_00_Folded.png` | 80×80 | Binary 0/255 |
| `Ibiki_01_Point.png` | 80×80 | Binary 0/255 |
| `Ibiki_00_Folded_source.png` | 1254×1254 | Transparent, with intermediate alpha values |
| `Ibiki_01_Point_source.png` | 1254×1254 | Transparent, with intermediate alpha values |
| `preview.png` | 960×470 | Opaque RGB |

All delivered files are under `art/deliveries/ibiki-hires-full-v1/`. The two `_source` files are the selected untouched image-generation outputs. The 80×80 sprites were reduced from them, mapped to the reference palette, and given the exact reference head pixels in both frames; only two mouth pixels differ in frame 01.

## Prompt summary

Full-body, right-facing, transparent pixel-art Ibiki matching the accepted idle reference: dark headband, scars, stern eyes, navy trench coat and boots. Frame 00 crosses black-gloved arms with a closed mouth. Frame 01 extends one black-gloved index finger to the right, keeps the other arm low, and opens the mouth slightly.

## Checks performed

- Visually inspected both generated sources and the final 1×/4× side-by-side preview against the accepted reference. Poses, direction, silhouettes and game-scale readability were checked.
- Both sprites are 80×80; nontransparent pixels end at `y=76` (inclusive). Alpha values are only 0 and 255.
- Head pixels in rows `y=16..35` are identical except the two mouth pixels at `(48,33)` and `(48,34)`.
- `python3 scripts/pixel_noise.py`: frame 00 has 5/1109 isolated pixels (0.5%); frame 01 has 8/1201 (0.7%). Both are below 4%.

## Remaining game-side checks

- Integrate the two sprites and verify their animation timing, draw offset, and alignment beside the existing idle frame in the game.
- Confirm the pointing hand and slightly open mouth remain readable at the game's actual zoom and background.
