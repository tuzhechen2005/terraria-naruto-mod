status: delivered
request ID: orochimaru-bullets-v1

## Delivered files

| Path (relative to this directory) | Dimensions | Alpha |
| --- | --- | --- |
| `FxSnakeBullet_0.png` | 28×12 | 0/255 only |
| `FxSnakeBullet_1.png` | 28×12 | 0/255 only |
| `FxVenomGlob_0.png` | 16×16 | 0/255 only |
| `FxVenomGlob_1.png` | 16×16 | 0/255 only |
| `FxVenomPool_0.png` | 48×12 | 0/255 only |
| `FxVenomPool_1.png` | 48×12 | 0/255 only |
| `FxVenomPool_2.png` | 48×12 | 0/255 only |
| `FxKusanagi.png` | 96×12 | 0/255 only |
| `source/generated_orochimaru_bullets.png` | 1774×887 | 0–255 source alpha |
| `preview.png` | 1240×660 | opaque composite preview |

## Prompt summary

Generated a transparent sheet of two right-facing purple-black snakes with open mouths, two purple and poison-green venom globs, three low bubbling puddles, and a right-pointing silver Kusanagi blade with a dark grip and uniform middle. Requested crisp Terraria-style pixel forms and dark outlines, using the named Tazuna and projectile images as visual references. Sampled the source and refined the game-size pixels, especially snake mouths and puddle surfaces.

## Checks performed

- Inspected generated source and final sprites visually, including 1× and 4× previews on dark and light backgrounds.
- Verified all eight game PNGs have the requested dimensions, alpha values limited to 0 and 255, and identical pixels within each 2×2 block.
- Verified animation frames within each group differ.
- Checked rightward snake mouths and sword tip, low puddle silhouettes, and sword's straight repeatable middle at preview scale.

## Remaining game-side checks

- Import into tModLoader and verify render scale, projectile rotation/pivot, frame cycling, and draw order during the Orochimaru fight.
- Confirm the sword's code-side horizontal stretch leaves the grip and tip looking correct.
- Check contrast over actual fight backgrounds and adjust if needed.

No game build or in-game acceptance was performed by the art worker.
