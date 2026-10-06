status: delivered
request ID: armor-academy-v1

## Delivered files

| File | Dimensions | Alpha |
|---|---:|---|
| `AcademyTrainingHead_Head.png` | 40×1120 | binary 0/255 |
| `AcademyTrainingBody_Body.png` | 360×224 | binary 0/255 |
| `AcademyTrainingLegs_Legs.png` | 40×1120 | binary 0/255 |
| `AcademyTrainingHead.png` | 32×32 | binary 0/255 |
| `AcademyTrainingBody.png` | 32×32 | binary 0/255 |
| `AcademyTrainingLegs.png` | 32×32 | binary 0/255 |
| `preview.png` | 552×210 | opaque review image |
| `source/AcademyTraining_generated.png` | 1536×1024 | transparent source with partial-alpha edge pixels |
| `source/build_assets.py` | source script | n/a |

## Prompt summary

Built-in image generation produced a front-facing pixel-art academy ninja in a cream tied cloth headband, warm gray-green short tunic with brown sash and forearm wraps, slate blue trousers, calf wraps, and straw sandals, plus separated icons. Requested Terraria-like hard shading, dark outlines, and a transparent background. The source was converted into icons; the equip sheets were recolored directly within the named vanilla template masks to preserve all frame and body-part positions.

## Checks performed

- Inspected the generated source, original named armor templates, and Tazuna style reference.
- Confirmed all three equipment sheet sizes and exact alpha-mask match to their corresponding templates: `Armor_Head_22` (padded by two transparent rows), `Armor_1`, and `Armor_Legs_1`; zero mask differences.
- Confirmed all six game PNGs use only alpha 0 or 255 and checked the assembled standing/walking direction and 1×/3× item icons in `preview.png`.
- The preview uses a neutral player face drawn under the template-aligned gear. It is a registration preview, not an in-game screenshot.

## Remaining game-side checks

- Import, build, and view on the actual Terraria player in stand, walk, jump, and attack poses; check arm layers, dye, hair/headband overlap, and alternate body shapes.
- Check item icons and outfit readability at the game's UI and character scale.
