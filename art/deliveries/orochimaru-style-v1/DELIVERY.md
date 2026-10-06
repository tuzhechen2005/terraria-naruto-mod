# Orochimaru style v1 delivery

- status: delivered
- Request ID: `orochimaru-style-v1`
- Generation: built-in `image_gen__imagegen`; four separate transparent-background source images. No API key or fallback CLI.

## Delivered files

| File | Dimensions | Alpha |
| --- | ---: | --- |
| `Orochimaru_Disguise.png` | 112×88 | transparent; 0/255 only |
| `Orochimaru_Idle.png` | 112×88 | transparent; 0/255 only |
| `Orochimaru_SnakeHands.png` | 112×88 | transparent; 0/255 only |
| `Orochimaru_LongNeck.png` | 144×104 | transparent; 0/255 only |
| `source/Orochimaru_Disguise_generated.png` | 1278×1230 | transparent source, variable alpha |
| `source/Orochimaru_Idle_generated.png` | 1024×1536 | transparent source, variable alpha |
| `source/Orochimaru_SnakeHands_generated.png` | 1536×1024 | transparent source, variable alpha |
| `source/Orochimaru_LongNeck_generated.png` | 1536×1024 | transparent source, variable alpha |
| `preview.png` | 664×312 | opaque RGB |
| `scale-preview-neji-gaara.png` | 400×148 | opaque RGB |

## Prompt summary

Four full-body Orochimaru poses from the Part I Forest of Death encounter: concealed Kusa examinee, revealed idle, three snakes projecting from the right sleeve, and an S-curved extended neck with visible tongue. Prompts specified long black hair, pale skin, golden slit eyes and purple eye marks where visible, pale tunic, purple rope belt, dark trousers and sandals, facing right, and a limited-color Terraria pixel-art style. The source images were reduced to an approximately 16–20-color palette and doubled nearest-neighbor into game-size pixels.

## Checks performed

- Visually inspected all four generated sources, the 1× light/dark preview, and the Neji/Gaara/Orochimaru scale preview.
- Checked the final four sprite dimensions, binary alpha, palette size, margins, and feet alignment. Last opaque foot row is y=83 for the first three and y=99 for LongNeck. None touches a canvas edge.
- Checked that silhouettes face right and that LongNeck's torso stays near x=56 while the head extends right.
- The pose and snake strands remain visible at 1×; the snake heads and small facial details are subtle at this scale.

## Remaining game-side checks

- Load the PNGs in tModLoader and check their appearance, layering, hitbox alignment, and visibility over the Death Forest background during movement.
- Judge whether the three SnakeHands heads and eye markings read clearly at the actual zoom level; adjust art after in-game review if needed.
