# Delivery: npc-hires-sample-b5

- status: delivered
- Request ID: `npc-hires-sample-b5`
- Delivered files:
  - `Anko_Idle.png` — 80 × 80 RGBA game sprite; binary transparency (alpha 0 or 255).
  - `source/Anko_source.png` — 1254 × 1254 RGBA generated source; transparent background with partial-alpha edges.
  - `preview.png` — 720 × 360 opaque RGBA comparison of Anko, Haku, and Hayate at 1× and 3×.
  - `faces_8x.png` — 532 × 276 RGBA Anko and Haku face comparison at 8×, with grid and row numbers.

## Prompt summary

Built-in `image_gen__imagegen` generated a transparent Anko source using the b4 Anko sprite as the edit target, Haku's approved face as the key face reference, and a temporary contact sheet of all nine approved sprites as the style reference. The prompt preserved the purple-black ponytail, forehead protector, tan coat, mesh shirt, orange skirt, leg guards, chibi proportions, and right-facing stance, while specifying open highlighted eyes, a clean cheek, a short upturned smirk, and a softer pointed jaw. The 80 × 80 game sprite was finished on the b4 pixel grid using the generated source and approved references for visual guidance.

## Checks performed

- Inspected the generated source, native-size game sprite, 1×/3× preview, and 8× face sheet.
- Game sprite has a 61-pixel-tall opaque silhouette, from y=16 through y=76, with feet on y=76; it faces right.
- Game sprite uses 15 colors and binary alpha. `python3 scripts/pixel_noise.py` reported 20 isolated pixels among 962 opaque pixels (2.1%), below the requested 3% limit.
- Confirmed open eyes, no brown cheek blotch, and a raised right mouth corner in the enlarged face view.

## Remaining game-side checks

- Import `Anko_Idle.png` into the mod and inspect it in Terraria beside Haku and Hayate under actual in-game lighting and scale.
- Verify any sprite origin, hitbox, animation, and integration requirements in the game build. No game-side build or in-game acceptance was run by the art worker.
