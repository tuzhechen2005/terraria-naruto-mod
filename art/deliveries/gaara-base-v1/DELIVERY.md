# gaara-base-v1 delivery

- status: delivered
- request ID: `gaara-base-v1`
- delivered files: 31 individual `Gaara_<Action>_<Index>.png` frames in this directory; `preview.png`; `alignment-overlay.png`; source images and reproducible `build_frames.py` in `source/`.
- frame counts: `Idle` 4, `Walk` 4, `Cast` 3, `Shield` 2, `Wave` 3, `Hurt` 1, `Cracked_Idle` 4, `Beast_Idle` 4, `Beast_Arm` 3, `Beast_Bullet` 3.
- dimensions: human frames 112 × 88 px; beast frames 176 × 104 px. `preview.png` is 1568 × 1400 px; `alignment-overlay.png` is 1872 × 130 px.
- alpha status: all 31 final frames are RGBA with transparent backgrounds; every pixel alpha is either 0 or 255. Character pixels are fully opaque.

## Prompt summary

Used built-in image generation with the approved `gaara-style-v2` sprites as visual references. Generated a forward sand-wave action source and a partially transformed air-bullet source, keeping the red hair, black outfit, white cloth, gourd and tan Shukaku features. Requested transparent backgrounds, crisp limited-color pixel art and a yellow right eye with a black four-point-star pupil. The final frames use the approved v2 sprites as their consistent character base and animate the specified attacks and effects; the two generated images are retained under `source/` as action references.

## Checks performed

- Inspected approved v2 references and both generated source images visually.
- Inspected `preview.png` at game pixel scale against both light and dark backgrounds and inspected the frame overlay.
- Verified all 31 final frame counts, canvas dimensions, binary alpha and bottommost opaque pixel: y=83 for human frames (foot line y=84 exclusive), y=99 for beast frames (foot line y=100 exclusive).
- Checked the golden beast eye and black star pupil in the enlarged reference and final frame.

## Remaining game-side checks

- Integrate the individual PNGs into the Boss animation state machine and confirm frame timing and loop transitions in tModLoader.
- Check facing, attack hitboxes, sand effects, and visual readability against Terraria terrain and lighting during an actual fight.
- Game build and in-game acceptance were not run by this art worker.
