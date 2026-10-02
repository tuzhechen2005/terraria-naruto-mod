# Orochimaru base v9 delivery

- status: delivered
- request ID: `orochimaru-base-v9`

## Delivered files

| File | Dimensions | Alpha |
| --- | ---: | --- |
| `Orochimaru_Idle_0.png` | 224 × 136 | RGBA; only 0 and 255 |
| `preview.png` | 3456 × 850 | Opaque RGB comparison sheet |
| `source/Orochimaru_generated_reference.png` | 1609 × 977 | RGBA; includes partial alpha; visual reference only |

## Prompt summary

Built-in `image_gen__imagegen` edited with the A 104 px starting image as the layout reference, the approved source A as the character-detail reference, and Tazuna as the pixel-art style reference. The prompt preserved the right-facing stance, narrow pale face, gold eyes, violet eye shadow, long black hair, robe, purple rope, fingers, leg wraps and sandals, with a transparent background and hard pixel edges. The generated reference was inspected and copied from Codex's `generated_images` directory. The game frame was finished at native size using the approved A composition and source A detail, followed by pixel-level eye restoration and a 56-color quantization pass.

## Checks performed

- Opened and visually inspected the A starting image, source A, Tazuna style comparison, Zabuza idle frame, generated reference, final frame and preview.
- Verified the game frame is 224 × 136, has 62 RGBA colors total, and uses only alpha 0/255.
- Verified visible pixels occupy x=90–133 and y=28–131; the lowest foot pixel is on y=131 and the figure faces right.
- Inspected the figure at 1× and nearest-neighbor 3× beside A, source A at comparable height, and Zabuza at original 1× size. Inspected the face at 8×.
- The preview has labels and contains all four comparisons at 1× and 3× plus the face close-up.

## Remaining game-side checks

- Integrate the new frame into the mod, then verify its anchor, hitbox, draw scale, and appearance in Terraria at 1×. This delivery did not modify or build game code.
- Confirm later animation frames preserve this frame's proportions, palette and facial details.
