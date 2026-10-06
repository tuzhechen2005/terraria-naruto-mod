status: delivered
request ID: hiruzen-hires-full-v1

## Delivered files

| File | Dimensions | Alpha |
| --- | --- | --- |
| `Hiruzen_00_Idle.png` | 80×80 | RGBA, only 0/255 |
| `Hiruzen_01_Puff.png` | 80×80 | RGBA, only 0/255 |
| `Hiruzen_02_Talk.png` | 80×80 | RGBA, only 0/255 |
| `preview.png` | 960×420 | Opaque RGB; 1× and 4× rows |
| `sources/Puff_imagegen.png` | 1254×1254 | RGBA, variable alpha; generated pose reference |
| `sources/Talk_imagegen.png` | 1254×1254 | RGBA, variable alpha; generated pose reference |

## Prompt summary

Used the accepted `Hiruzen_Idle.png` as the image reference for two built-in ImageGen edits: an eyes-closed pipe puff with a brighter ember and a raised-hand explanation pose with kind open eyes. Requested transparent, crisp pixel art without smoke or scenery. The generated images were large pose references. The three 80×80 game frames retain the accepted sprite's fixed pixels and palette; the action pixels were aligned to its grid.

## Checks performed

- Idle frame is byte-for-byte identical to the accepted reference.
- Inspected both generated images and the three-frame 1×/4× preview.
- All game frames are 80×80 with only fully transparent or fully opaque pixels. Feet end at y=76. All 19 opaque colors are from the reference palette.
- Isolated-pixel check using the request's 4-neighbor, color-distance-24 method: Idle 0.00%, Puff 0.31%, Talk 0.38%; each is below 4%.
- Puff changes 39 pixels and Talk changes 72 pixels from Idle. Both retain the hat, white hair, beard, body placement, and shoes. The Talk frame keeps the pipe in place.

## Remaining game-side checks

- Import the three frames into the mod and inspect animation at native game scale in the Hokage office, including hand/pipe overlap, expression readability, and timing. Game-side inspection has not been run by this art worker.
