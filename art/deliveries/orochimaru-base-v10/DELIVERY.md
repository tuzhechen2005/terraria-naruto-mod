status: delivered
request ID: orochimaru-base-v10

## Delivered files

| File | Dimensions | Alpha | Purpose |
| --- | --- | --- | --- |
| `Orochimaru_Idle_0.png` | 224×136 | RGBA; alpha only 0/255 | Final game-size sprite |
| `preview.png` | 1392×840 | RGB; no alpha | v9/v10 side by side at 1× and 3×, plus face crops at 8× |
| `imagegen_face_reference.png` | 1609×977 | RGBA; antialiased alpha | Original built-in image generation output, retained as a visual reference; not game-ready |

## Prompt summary

The built-in image tool edited the v9 sprite reference toward brighter gold eyes, darker and wider purple eye shadow, and a thin sly smile. Its output was inspected and copied from `$CODEX_HOME/generated_images`. Because that output changed the canvas size and used antialiased pixels, the final 224×136 sprite applies the face details to an exact copy of v9 at game-pixel resolution.

## Checks performed

- Inspected the v9 input and generated output visually; inspected the final 1×, 3×, and 8× comparison preview.
- Confirmed final sprite is 224×136 RGBA, with alpha values only 0 and 255.
- Confirmed the v9 alpha channel is identical in v10. The only 13 changed pixels are within x=116–121 and y=37–43; all pixels outside that face area are identical to v9.
- Confirmed nontransparent bounds remain x=90–133 and y=28–131, preserving center alignment and foot baseline.

## Remaining game-side checks

- Load the final sprite in tModLoader and judge eye and mouth readability at native gameplay scale and against in-game backgrounds.
- Check alignment and consistency when the remaining animation frames are derived from this base.
