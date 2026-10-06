status: delivered
request ID: hiruzen-direct-pixel-v1

# Hiruzen idle candidate

Single right-facing direct-pixel standing sample for user review. This delivery does not change game assets or code.

## Delivered files

| File | Dimensions | Alpha |
| --- | --- | --- |
| `Hiruzen_Source.png` | 1180×1333 | RGBA, mixed alpha; generated source |
| `Hiruzen_Idle_Right.png` | 80×80 | RGBA, 0/255 only; transparent RGB zero |
| `Hiruzen_Idle_Left.png` | 80×80 | RGBA, 0/255 only; transparent RGB zero |
| `Hiruzen_Idle_6x.png` | 480×480 | RGBA, enlarged inspection image |
| `Hiruzen_head_6x.png` | 228×168 | RGBA, enlarged head crop |
| `compare_1x.png` | 272×184 | RGB, dark/light comparison with Iruka and Kakashi |
| `compare_3x.png` | 816×552 | RGB, nearest-neighbor comparison |
| `preview.html` | HTML | Opens the comparisons at intended scale |
| `Hiruzen_Idle_manifest.json` | JSON | Export parameters and measured bounds |
| `PROMPT.md` | Markdown | Full prompt and reference roles |

## Prompt summary

Built-in image generation of one transparent, coarse-pixel Hiruzen sprite. The approved Iruka frame supplied style only; the Hiruzen portrait supplied identity; the prior Hiruzen sprites supplied clothing and pipe. Required a broad white and red Hokage hat, small narrow elderly eyes, short gray-white beard, white robe with red trim, dark inner collar, and long brown pipe. Full prompt is in `PROMPT.md`.

## Checks performed

- Inspected generated source, exported 1× frame, 6× frame, and dark/light comparisons at 1× and 3×.
- Exported visible figure bounding box: x=22–57, y=19–75, 36×57 pixels. Horizontal center is x=40; feet reach y=75; no alpha-128 source content touches the source edge.
- Final frames have only alpha 0 or 255, zero RGB in transparent pixels, and 80×80 canvas. Left frame is an exact horizontal mirror of the right frame.
- Right-facing profile, distinct broad hat, white robe, gray-white beard, pipe, and shorter height remain recognizable at 1×. Eye detail is small and partly shaded by the brim, consistent with the narrow-eyed elderly design.

## Remaining game-side checks

- User review of this standing candidate at normal size.
- After approval, develop animation frames and connect to the actual NPC texture and scale as a separate task.
- Check placement, direction, lighting, and readability in Terraria. No build or in-game verification was run for this art-only delivery.
