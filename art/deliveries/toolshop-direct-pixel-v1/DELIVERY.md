# toolshop-direct-pixel-v1

- status: delivered
- request ID: `toolshop-direct-pixel-v1`
- generation: built-in `image_gen__imagegen`; no API key or fallback CLI

## Delivered files

All paths below are relative to this directory.

| Path | Dimensions | Alpha | Purpose |
| --- | ---: | --- | --- |
| `ToolShopkeeper_source.png` | 1122×1402 | RGBA, partial alpha | Selected original generated source |
| `ToolShopkeeper_Idle_Right.png` | 80×80 | RGBA, 0/255 | Right-facing game-size stand candidate |
| `ToolShopkeeper_Idle_Left.png` | 80×80 | RGBA, 0/255 | Exact horizontal mirror |
| `ToolShopkeeper_Idle_6x.png` | 480×480 | RGBA, 0/255 | Whole frame, nearest-neighbor 6× inspection |
| `ToolShopkeeper_head_6x.png` | 180×162 | RGBA, 0/255 | Head crop, nearest-neighbor 6× inspection |
| `comparison_1x.png` | 300×208 | RGBA, fully opaque | Iruka, Kakashi, and shopkeeper at native scale on light/dark backgrounds |
| `comparison_3x.png` | 900×624 | RGBA, fully opaque | Same comparison at nearest-neighbor 3× |
| `ToolShopkeeper_Idle_manifest.json` | — | — | Deterministic export parameters and measured bounds |
| `PROMPT.md` | — | — | Full generation prompt |
| `preview.html` | — | — | Local visual preview |

## Prompt summary

Generate one right-facing, low-resolution pixel-art standing sprite for Tenten's middle-aged tool-merchant father. Use approved Iruka only for broad warm color blocks and clear silhouette; use the older Tool Shop art only for identity. Keep the round bun, short beard, burgundy frog-button jacket, dark plain apron, waist pouch, dark trousers, shin wraps, and cloth shoes. Give him his own middle-aged eye shape, no mouth line, no hat, no dense apron tools, and a transparent background. Full prompt is in `PROMPT.md`.

## Checks performed

- Inspected the generated source, true 80×80 right-facing sprite, 6× whole frame and head crop, and 1×/3× light/dark comparisons.
- Exported with the skill's `scripts/export_sprite.cjs`: 62-pixel body height, 80×80 canvas, baseline 76, pixel 1, sample phase (0.5, 0.5), alpha threshold 128, facing right.
- Measured visible bounds `x=26..53`, `y=14..75`: center x=40, height 62, foot bottom y=75, no canvas-edge clipping.
- Verified both game-size frames use only alpha 0 or 255 and every transparent pixel has RGB (0, 0, 0); left is an exact horizontal mirror of right.
- At native scale, bun, beard, burgundy jacket, dark apron, and wrapped legs remain distinct. Face crop shows the eye, skin, and beard as separate shapes. Compared beside approved Iruka and current Kakashi stand frames on light and dark backgrounds.

## Remaining game-side checks

- Have the owner review this **standing candidate** at native scale before extending animation.
- If approved, integrate the new sheet and adjust frame/scale handling, then inspect in tModLoader across actual backgrounds and motion. No game build or in-game visual check was performed for this art-only delivery.
