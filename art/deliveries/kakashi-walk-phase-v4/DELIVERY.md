status: delivered
request ID: kakashi-walk-phase-v4

## Delivered files

- `source/generated_opposite_sheet.png` — 2172×724 RGBA source, with alpha.
- `frames/Kakashi_OppositeContact.png` — 80×80 RGBA, transparent.
- `frames/Kakashi_OppositePassing.png` — 80×80 RGBA, transparent.
- `frames/Kakashi_ReturnLift.png` — 80×80 RGBA, transparent.
- `opposite_check_3x.png` — 720×480 RGB preview on dark and light backgrounds.
- `opposite_check_6x.png` — 1440×960 RGB preview on dark and light backgrounds.
- `cycle_strip.png` — 480×80 RGBA six-frame strip.
- `cycle.gif` — 80×80, six-frame animated GIF with transparency.
- `PROMPT.md`, `export_parameters.json`, `export.py` — prompt and reproducible export settings.

## Prompt summary

Generated three whole-body, right-facing Kakashi poses from the named v3 and approved idle references. The near leg keeps its two white thigh bands, while the unbanded far leg takes the forward contact. The source has a transparent background. Full prompt: `PROMPT.md`.

## Checks performed

- Visually inspected the generated source, the 3× dark/light preview, the 1× cycle strip, and the first frame of the GIF. Silver hair, forehead protector, single visible eye, mask, vest, waist and hip connection remain readable at game size. Both feet point right.
- In v3 01, the white-banded near leg reaches forward; v3 02 and 04 bring it closer beneath the torso. In `OppositeContact`, that banded near leg is behind and the unbanded far leg contacts forward. In `OppositePassing`, the unbanded far leg carries the body while the bent banded leg passes behind/under the pelvis. In `ReturnLift`, the banded near leg comes forward again toward v3 01. This supplies the opposite-leg half of the cycle without mirroring the sprite.
- New frame bounds are `(21,14)-(58,76)`, `(26,14)-(53,76)`, `(23,14)-(57,76)` respectively. Each has 62-pixel visible height, bottommost pixel y=75, horizontal center x≈39–39.5, no frame-edge clipping, alpha values only 0/255, and RGB `(0,0,0)` wherever alpha is 0.
- The GIF has six frames in requested order: v3 01, 02, 04, then OppositeContact, OppositePassing, ReturnLift. Original v3 files were only read.

## Remaining game-side checks

Development assistant should inspect the six-frame loop in tModLoader at 1×, especially the v3 04 → OppositeContact and ReturnLift → v3 01 transitions, foot contact, waist volume, and any apparent size or position jitter. These frames have not been integrated or game-tested here.
