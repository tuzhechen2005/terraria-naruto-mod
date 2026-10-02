status: delivered
request ID: orochimaru-base-v4

## Delivered files

- `Orochimaru_Idle_0.png` — final game-size standing frame, 112×88 RGBA. Transparent background; alpha values are exclusively 0 and 255.
- `source/Orochimaru_generated.png` — selected built-in image generation source, 1415×1112 RGBA. It has partial alpha around the generated sprite edges; use the game-size file for integration.
- `preview.png` — 2064×524 RGBA comparison on an opaque slate background. Shows the new frame, current frame, flat reference, and Tazuna style frame at 1× and 4×.

## Prompt summary

Built-in `image_gen__imagegen` with the current Orochimaru frame, earlier flat frame, Tazuna comparison, and Tazuna sprite sheet supplied as image references. Requested one right-facing, full-body, transparent pixel-art Orochimaru with an exposed enlarged pale face, golden snake eyes, purple eye shadow, dark hair behind the face, small forehead protector, cream tunic, thick purple rope belt, dark trousers, sandals, and a hand raised near the chest. Tazuna's dark outlines and stepped color treatment guided the finish. The source was reduced to a logical 56×44 grid, retouched to expose the face and make both golden eye regions 2×2 logical pixels, then doubled to 112×88.

## Checks performed

- Visually inspected the generated source and final sprite, including the face enlarged and a 1×/4× side-by-side preview against all requested references.
- Confirmed final dimensions 112×88, opaque-pixel bounds x=30–83 and y=12–83, with the feet ending at y=83.
- Confirmed the final frame has only alpha 0/255 and every 2×2 screen-pixel block is uniform.
- Confirmed the sprite faces right, the face is unobscured, and the eyes remain visible at 1×.

## Remaining game-side checks

- Integrate as the Boss base frame and inspect in Terraria at the intended 1.5× game scale against the environment.
- Confirm hitbox alignment, sprite origin, and animation compatibility after the other frames are derived from this base.
