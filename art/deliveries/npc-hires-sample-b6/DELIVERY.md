status: delivered
request ID: npc-hires-sample-b6

## Delivered files

- `Anko_Idle.png` — 80×80 RGBA game sprite; transparent background (alpha 0/255); visible bounds x=27–52, y=16–76.
- `source/Anko_generated.png` — 1222×1287 RGBA image generated with the built-in `image_gen__imagegen` tool; transparent background with graded alpha. This is the high-resolution concept source. The game sprite was pixel assembled from the accepted Haku face and b5 Anko body to preserve the requested pixel layout.
- `preview.png` — 720×342 RGBA transparent comparison of Anko, Haku, and Hayate at 1× and 3×.
- `faces_8x.png` — 488×226 RGBA opaque comparison of Anko and Haku heads at 8× with grid and source y-row numbers.

## Prompt summary

Create a transparent, right-facing, three-quarter-view pixel-art Anko with Haku's feminine face shape and open highlighted eyes, purple-brown pupils, a raised brow and sly smile. Use b5 Anko's purple-black upturned ponytail, Konoha forehead protector, tan coat, fishnet shirt, orange skirt, leg guards, and sandals. Keep bangs clear of the eyes and avoid cheek marks.

## Checks performed

- Inspected the generated source and all final PNGs visually, including native-size and enlarged comparisons.
- Confirmed 80×80 game image, feet at y=76, 61-pixel visible height, right-facing silhouette, clean face, open eyes, and transparent background.
- Game sprite uses 15 RGB colours (limit 20).
- Ran `scripts/pixel_noise.py` with a temporary compatible RGBA PNG reader because Pillow is unavailable in this worker environment. Output: `art/deliveries/npc-hires-sample-b6/Anko_Idle.png: 961 px, 15 colours, 21 isolated (2.2%)` (limit below 3%). The temporary reader was removed after the check.
- Confirmed alpha status and dimensions of all delivered images; preview has transparency, face study has an opaque backing for its grid.

## Remaining game-side checks

- Import the 80×80 sprite into the mod and inspect it in Terraria at actual NPC scale, including its visual alignment beside the accepted Haku and Hayate sprites.
- Confirm the expression and outfit remain readable against in-game backgrounds and during animation or lighting changes.
