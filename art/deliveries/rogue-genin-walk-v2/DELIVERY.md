# rogue-genin-walk-v2 delivery

- status: delivered
- request ID: `rogue-genin-walk-v2`

## Delivered files

- `RogueGenin_Walk_0.png` — 112×88 RGBA, alpha values 0/255 only
- `RogueGenin_Walk_1.png` — 112×88 RGBA, alpha values 0/255 only
- `RogueGenin_Walk_2.png` — 112×88 RGBA, alpha values 0/255 only
- `RogueGenin_Walk_3.png` — 112×88 RGBA, alpha values 0/255 only
- `RogueGenin_Jump.png` — 112×88 RGBA, alpha values 0/255 only
- `source/RogueGenin_WalkJump_generated_sheet.png` — 2172×724 RGBA, transparent with antialiased alpha
- `preview.png` — 3024×368 RGB; v1 Idle, Throw, Slash_0, Slash_1 followed by the five new frames, at 1× and 3× on a checkerboard

## Prompt summary

Used the built-in image generation tool with v1 Idle as the primary image reference and v1 Throw and Slash frames as supporting identity references. Requested five separated, right-facing pixel-art poses on a transparent sheet: four successive low kunai-running strides and one airborne jump. Specified the exposed face, scratched silver forehead protector, slim build, dark clothing, palette, and lack of hood or face mask. Sampled the generated poses to the game grid, aligned their heads and feet, and corrected the forehead-protector scratches at pixel level.

## Checks performed

- Visually inspected the source sheet and the 1×/3× preview against the four v1 reference frames. All five poses face right, retain a visible face and slim silhouette, and read as running or jumping at game scale.
- Confirmed each game frame is 112×88, has transparent background, and uses only alpha 0 or 255.
- Confirmed each game frame consists of uniform 2×2 pixel blocks, fits within its canvas, and ends at foot row y=83.
- Kept the four v1 reference frames untouched.

## Remaining game-side checks

- Replace the five corresponding game sprites and inspect the four-frame walk loop, jump transition, alignment, and visual continuity beside Idle and attack animations in Terraria at the actual display scale.
