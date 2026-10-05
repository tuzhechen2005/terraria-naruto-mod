# armor-genin-v3

- **status: delivered**
- **request ID:** `armor-genin-v3`

## Files

| File | Dimensions | Alpha |
|---|---:|---|
| `GeninCombatHead_Head.png` | 40×1120 | binary 0/255 |
| `GeninCombatBody_Body.png` | 360×224 | binary 0/255 |
| `GeninCombatLegs_Legs.png` | 40×1120 | binary 0/255 |
| `GeninCombatHead.png` | 28×28 | binary 0/255 |
| `GeninCombatBody.png` | 30×30 | binary 0/255 |
| `GeninCombatLegs.png` | 28×30 | binary 0/255 |
| `preview.png` | 920×480 | opaque comparison board |
| `source/GeninCombat_concept.png` | 1774×887 | RGBA with graded edge alpha; design source only |
| `source/build_sprites.py` | Python source | n/a |

All paths above are relative to `art/deliveries/armor-genin-v3/`.

## Prompt summary

Built-in `image_gen__imagegen` made a transparent pixel-art design sheet showing front, three-quarter, side and back views plus the three garment pieces. It specified a bright silver leaf-engraved forehead plate on a dark navy band, medium-bright blue short-sleeve tunic with white trim and mesh, brown diagonal strap and kunai, white forearm wraps, gray-beige trousers, right-thigh pouch, left-thigh wrap, white calf wraps and blue open-toe sandals. The final game sheets were drawn at 2× screen pixels from this design, using the named vanilla images for frame and atlas registration. The generated design image itself is preserved as the source; game PNGs are hard-edged and do not inherit its graded alpha.

## Checks performed

- Inspected the generated source, all six output PNGs and the 1×/3× preview visually.
- Verified exact sheet sizes, 20 nonempty head frames and 20 nonempty leg frames. Head y coordinates follow the vanilla head's two-pixel rise on frames 7–9 and 14–16; trouser position follows the leg template's elevated frames.
- Verified all six game PNGs use only alpha 0 or 255. The generated concept source has graded alpha and is not a game PNG.
- Counted opaque pixels **outside** reference masks: head **1,432** compared with `Armor_Head_22.png`; body **5,280** compared with `Armor_1.png`; legs **3,736** compared with `Armor_Legs_1.png`. The head comparison is to the named vanilla ninja head reference because no copper head mask was supplied. The body and leg counts show that those sheets were not merely recolored copper outlines.
- Preview composites back arm, legs, torso, original player face and hair, armor head and front arm in that order. It shows standing, two walk frames and a jump frame at 1× and 3×, same-frame ninja and copper comparisons, and three icons at 1× and 3×.
- Headband tails change with frame phase and lift in jump frames; tunic strap tails have two positions; trouser and foot silhouettes shift with step phase.

## Remaining game-side checks

- Import the six PNGs into the mod and verify Terraria's actual arm-part mapping, draw order, horizontal flip and animation frame selection in-game. The preview follows the requested layer order but cannot prove the game's renderer will use every atlas region identically.
- Check all hairstyles, skin colors, standing/walking/jumping and both facing directions for collisions or detached parts, then confirm that item icons remain readable in inventory UI.
- Build and perform game playtest; neither was run by this art worker.
