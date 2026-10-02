# Orochimaru base v5 delivery

- status: delivered
- request ID: `orochimaru-base-v5`

## Files

- `Orochimaru_Idle_0.png` — final game frame, 112×88 RGBA; alpha values 0/255 only.
- `source/generated_Orochimaru_face_v5.png` — selected built-in image generation source, 1415×1112 RGBA; partial alpha present.
- `preview.png` — v5, v4, current game frame, and Tazuna comparison at 1× and 4×; 1880×560 RGB.
- `face_10x.png` — final face detail at 10×; 280×280 RGBA with alpha values 0/255 only.

## Prompt summary

Built-in image generation used the v4 sprite as the edit target, the current in-game Orochimaru frame as an identity reference, and Tazuna as the pixel style reference. The requested change was confined to a narrower, shaded face, fine upward smile, pointed jaw, and clean three-shade hair while preserving the right-facing pose, gold snake eyes, purple eye makeup, costume, and transparent background. The selected generated source informed pixel-level refinement on the exact v4 grid.

## Checks performed

- Visually inspected v4, the current frame, Tazuna, generated output, final 1×/4× comparison, and final 10× face detail.
- Final frame is 112×88 with a 2×2 screen-pixel art grid, transparent background, and only binary alpha.
- Changed pixels are confined to head region x=66–85, y=14–37; body, pose, and costume remain from v4.
- Final opaque bounds are x=30–85, y=12–83; feet still end at y=83.
- Inspected the face outline, smile, hair locks, and silhouette at game scale and enlarged scale; no isolated single screen pixels were introduced.

## Remaining game-side checks

- Integrate the frame into `ShinobiPrototype/Content/NPCs/` and inspect in Terraria at 1.5×, especially smile readability and alignment with other Orochimaru frames. Game-side integration and testing were not performed by the art worker.
