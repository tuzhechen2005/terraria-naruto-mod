status: delivered
request ID: iruka-npc-v3

## Delivered files

- `source/Iruka_Idle_Walk.png` — 1568×200 RGBA; alpha 0/255; Idle + Walk 6.
- `source/Iruka_Idle_Jump_Sit_Throw.png` — 1344×200 RGBA; alpha 0/255; Idle + Jump + Sit (reading mission scroll) + Throw 3.
- `source/Iruka_Talk.png` — 672×200 RGBA; alpha 0/255; Idle + Talk 2 (mission scroll and raised hand).
- `Iruka_comparison_1x.png` — 164×81 RGB; opaque; Tazuna, Kakashi, Iruka at 1 art pixel per image pixel.
- `Iruka_comparison_4x.png` — 416×156 RGB; opaque; same characters at 4× nearest-neighbor scale.
- `reference/Iruka_imagegen_reference.png` — 2171×724 RGBA; semi-transparent alpha; original built-in image generation reference, **not** a game-ready sprite sheet.
- `build_sprites.py` — reproducible construction of the delivered sheets and comparison previews.

## Prompt summary

The built-in `image_gen__imagegen` tool used the Kakashi idle/walk sheet as the required body and pose image reference and the Iruka v2 talk sheet as a character and gesture reference. It requested black high-ponytail hair, a visible face with two eyes and horizontal nose scar, a forehead-level blue protector with bright plate, and the exact Kakashi outfit on a transparent pixel-art sheet. The generated reference was copied from `$CODEX_HOME/generated_images` by file path. Because that image had variable alpha and an irregular grid, the final sheets were built from Kakashi's exact 8×8 blocks, changing the head and the required scroll/talk details. Colors use the Kakashi palette plus four coordinated skin/scar colors.

## Checks performed

- Inspected the named Kakashi and Iruka v2 sheets and the Tazuna comparison reference.
- Visually inspected the generated reference and every final sheet, plus 1× and 4× comparisons.
- Verified all three final sheets have 224×200 cells, exact 8×8 color blocks, and only alpha 0 or 255.
- Verified every frame remains inside its cell, has the intended pose order, and faces right.
- Preserved Kakashi's vest, sleeves, pants, shoes and movement pixels outside the redrawn head, except for the small mission scroll and talk hand.

## Remaining game-side checks

- Import and verify the frame mapping, on-screen height, animation timing, and visual contrast in tModLoader.
- Confirm the ponytail, forehead plate and nose scar remain readable over representative in-game backgrounds at native display scale.
