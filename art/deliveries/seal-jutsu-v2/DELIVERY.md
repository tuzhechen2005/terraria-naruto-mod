# seal-jutsu-v2 delivery

- status: delivered
- request ID: `seal-jutsu-v2`
- generation: built-in `image_gen__imagegen`, using the two v1 sprites named in the request as style references.

## Delivered files

All paths below are relative to this directory. Every game-size frame is an RGBA PNG with alpha values **0 or 255 only** and art pixels aligned to **2×2 screen pixels**.

| Files | Count | Dimensions each | Alpha |
| --- | ---: | ---: | --- |
| `FxHugeFireball_0.png`–`FxHugeFireball_3.png` | 4 | 208×208 | binary transparency |
| `FxHugeExplosion_0.png`–`FxHugeExplosion_5.png` | 6 | 288×288 | binary transparency |
| `FxGroundFire_0.png`–`FxGroundFire_3.png` | 4 | 64×48 | binary transparency |
| `FxChidoriCharge_0.png`–`FxChidoriCharge_3.png` | 4 | 112×112 | binary transparency |
| `FxLightningTrail_0.png`–`FxLightningTrail_3.png` | 4 | 64×32 | binary transparency |
| `FxChidoriImpact_0.png`–`FxChidoriImpact_4.png` | 5 | 192×192 | binary transparency |
| `preview.png` | 1 | 1744×2104 | opaque RGB; each group at native size on dark and light backgrounds |
| `source/FxHugeFireball_sheet.png` | 1 | 2172×724 | RGBA, partial alpha |
| `source/FxHugeExplosion_sheet.png` | 1 | 1536×1024 | RGBA, partial alpha |
| `source/FxGroundFire_sheet.png` | 1 | 2172×724 | RGBA, partial alpha |
| `source/FxChidoriCharge_sheet.png` | 1 | 2172×724 | RGBA, partial alpha |
| `source/FxLightningTrail_sheet.png` | 1 | 2172×724 | RGBA, partial alpha |
| `source/FxChidoriImpact_sheet.png` | 1 | 2172×724 | RGBA, partial alpha |

## Prompt summary

Generated six animation sheets in the v1 fire and Chidori palettes: a right-moving, four-frame white-hot fireball; a six-stage flash, fire ring, pillars, smoke and ember explosion; looping ground flames; four radial Chidori charge frames; a horizontal, repeatable lightning trail; and a five-stage radial Chidori impact that breaks into sparks. Requested transparent backgrounds, jagged pixel clusters, and hard color bands. Cropped and palette-reduced each sheet into its specified game-size frames.

## Checks performed

- Inspected both named v1 reference sprites and all six generated sheets.
- Inspected `preview.png` at native size on dark and light backgrounds for silhouette, rightward fireball direction, effect sequence, and small-scale readability.
- Verified all 27 frames exist, have exact requested dimensions, contain distinct frame data, use only alpha 0/255, and consist of uniform 2×2 pixel blocks.
- Verified the left and right pixel columns match exactly in every `FxGroundFire` and `FxLightningTrail` frame so repeated tiles join.

## Remaining game-side checks

- Integrate the frames into the mod, build it, and inspect all animations in Terraria at intended timing and scale, including additive glow, rotations, fireball growth, ground placement, and trail tiling during movement.
- Adjust timing, placement, or source selection if a frame appears clipped or too bright in the live scene. No game-side build or in-game acceptance was run by this art worker.
