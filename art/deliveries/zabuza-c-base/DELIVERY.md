# zabuza-c-base delivery

- status: delivered
- request ID: `zabuza-c-base`
- tool: built-in `image_gen__imagegen`; no API key or fallback CLI used

## Delivered source images

All paths are relative to this delivery directory. Each PNG has transparent background with alpha values limited to 0 and 255; every visible color and alpha block is exactly 8×8 screen pixels.

| File | Dimensions | Left-to-right pose order |
| --- | ---: | --- |
| `source/Zabuza_Idle.png` | 1712×600 | idle size reference; breathing 1, 2, 3, 4 |
| `source/Zabuza_Run.png` | 2784×448 | idle size reference; running cycle 1–6 |
| `source/Zabuza_Windup.png` | 1672×600 | idle size reference; both-hand sword windup 1–3 |
| `source/Zabuza_Slash.png` | 1680×632 | idle size reference; slash start, fully extended right at waist height, recovery |
| `source/Zabuza_Seal.png` | 1360×584 | idle size reference; planted sword and hand seals 1–3 |
| `source/Zabuza_Leap.png` | 1312×560 | idle size reference; takeoff, descent |
| `source/Zabuza_Dash.png` | 1736×608 | idle size reference; low charge, forward collision lunge |

## Prompt summary

The approved C-style Zabuza sprite was used as the visual standard, with the two requested Zabuza images as character and sword references. Prompts specified right-facing full-body Terraria-style pixel sprites, dark blue-gray sleeveless clothing, white face wraps, striped guards, sandals, a broad cleaver with a circular hole, ordered animation poses, clear separation, and transparency. Generated sheets were normalized to an 8×8 screen-pixel art grid and opaque/clear alpha, targeting a body height near 42 art pixels. The final running frame was generated separately to keep its full silhouette clear of the sheet edge.

## Checks performed

- Visually inspected every generated sheet and the delivered sheets for pose order, silhouettes, sword direction, right-facing character, clear pose spacing, and readable details.
- Inspected a temporary preview at 2 screen pixels per art pixel, then removed that preview; only source images remain.
- Programmatically checked all seven PNGs: dimensions divisible by 8, each 8×8 block uniform in RGBA, alpha values exactly 0/255, and all outer canvas edges transparent.
- No game build or in-game test was run.

## Remaining game-side checks

- Extract the action frames, discard each leftmost idle size reference, and verify exact body scale, feet/hitbox alignment, and animation timing in Terraria.
- Review the final running frame's slightly larger sword silhouette alongside the other run frames; adjust during sprite extraction if needed.
- Verify boss phase 1/2 rendering, attack timing, and readability against live game backgrounds.
