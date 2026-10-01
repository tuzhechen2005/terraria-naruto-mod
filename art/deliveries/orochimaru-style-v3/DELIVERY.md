status: delivered
request ID: orochimaru-style-v3

## Delivered files

- `Orochimaru_Idle.png` — 112×88 PNG, transparent background; alpha values 0/255 only.
- `Orochimaru_Disguise.png` — 112×88 PNG, transparent background; alpha values 0/255 only.
- `source/Orochimaru_Idle_generated.png` — 1024×1536 generated PNG, alpha present with intermediate values.
- `source/Orochimaru_Disguise_generated.png` — 1166×1349 generated PNG, alpha present with intermediate values.
- `preview.png` — 3136×490 opaque comparison image. Left to right: v3 idle, v3 disguise, Tazuna ×1.36, Dosu, Gaara, v2 idle, v2 disguise; 1× above 4×.

## Prompt summary

Used the built-in `image_gen__imagegen` tool to revise each v2 source image in the style of the named Tazuna reference. Kept the existing right-facing poses and clothing palettes; requested visible human faces, gold snake eyes, smaller forehead bands, broader builds, stronger outlines and hard color planes. The idle form has pale skin, purple eye makeup, highlighted dark hair, bright ivory clothing and a purple rope. The disguise has ordinary warm skin, a gold eye, green clothing and dark trousers.

## Checks performed

- Inspected both generated sources and both game-size sprites, including the 1×/4× comparison.
- Sampled each generated pose onto a 2×2 screen-pixel grid and manually repaired the face, snake eye, eye makeup and mouth pixels.
- Confirmed both game-size PNGs decode as 112×88, use only alpha 0 or 255, and have zero mismatched 2×2 pixel blocks.
- Confirmed each nontransparent silhouette spans x=36–75 and y=22–83: centered on x≈56, bottom row y=83, about 62 screen pixels tall, facing right.
- Compared silhouette and readability with Tazuna, Dosu, Gaara and both v2 sprites in `preview.png`.

## Remaining game-side checks

- Integrate the two game-size PNGs, then verify rendering, scale and facing in tModLoader at 1× during the encounter. No game build or in-game check was performed by this art worker.
