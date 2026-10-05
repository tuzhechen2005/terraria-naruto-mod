status: delivered
request ID: armor-genin-v1

## Delivered files

| File | Dimensions | Alpha |
| --- | --- | --- |
| `GeninCombatHead.png` | 28×20 | 0/255 only |
| `GeninCombatBody.png` | 30×30 | 0/255 only |
| `GeninCombatLegs.png` | 28×30 | 0/255 only |
| `GeninCombatHead_Head.png` | 40×1120 | 0/255 only |
| `GeninCombatBody_Body.png` | 360×224 | 0/255 only |
| `GeninCombatLegs_Legs.png` | 40×1120 | 0/255 only |
| `preview.png` | 470×255 | opaque preview background |
| `source/genin_combat_concept.png` | 1774×887 | transparent, includes intermediate alpha |
| `source/build_genin.py` | source script | not an image |

## Prompt summary

Generated a transparent Terraria-style novice shinobi concept with a navy Leaf forehead protector and bright silver plate, fitted navy tunic with a diagonal leather strap and two kunai hilts, brown thigh pouch, navy trousers, white shin wraps, and blue sandals. Kept hard pixel shades, dark outlines, and a left-facing game sprite with isolated item views. The six game-size PNGs were built from that visual source and the named original armor templates.

## Checks performed

- Visually inspected the generated concept, the named reference sprites, all three game-size icons, and the standing and walking preview.
- Confirmed the six game-size images use only fully transparent or fully opaque pixels; no single-pixel isolated alpha components occur in them.
- Confirmed the head and body sheets have the requested dimensions and preserve every template alpha pixel without adding pixels outside the original masks. The 1118-pixel ninja head template was padded to the requested 1120-pixel 20-frame height.
- Confirmed all original leg-template pixels remain. Trousers, wraps, and the thigh pouch extend upward within each 40×56 frame; feet retain their original frame locations.
- Confirmed the preview shows two stacked 40×56 character frames at 3×, plus each icon at 1× and 3×.

## Remaining game-side checks

- Register the six game textures and inspect idle, running, jumping, and arm swing frames in tModLoader. In particular, confirm the added trouser pixels do not overlap body or skin layers and the body strap/kunai hilts read correctly at 1×.
- Check the actual player-layer draw order, especially the forehead plate and thigh pouch, with dyes and both facing directions.
