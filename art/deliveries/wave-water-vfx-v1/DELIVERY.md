status: delivered

# wave-water-vfx-v1

Delivered files:

| File | Dimensions | Alpha / purpose |
| --- | --- | --- |
| `source.png` | 1536×1024 | RGBA, six generated pieces on transparent sheet |
| `sprites/WaterDragonHead.png` | 192×128 | RGBA, right-facing danger silhouette |
| `sprites/WaterDragonBody.png` | 256×96 | RGBA, horizontal danger silhouette and softer foam |
| `sprites/WaterGather.png` | 192×192 | RGBA, decorative charge with open center |
| `sprites/WaterSlash.png` | 256×192 | RGBA, decorative right-opening blade trail |
| `sprites/WaterSplash.png` | 256×128 | RGBA, harmless impact aftermath |
| `sprites/MistPuff.png` | 128×128 | RGBA, decorative teleport warning |
| `preview.png` | 572×708 | RGB, all six at game size on dark and light backgrounds |
| `water_loop.gif` | 512×256, 12 frames | short repeating motion/alpha review from the same generated art |

`prompt.md` records the full built-in image prompt. `manifest.json` records source cells, crop rectangles, game canvas sizes, anchors, direction, and danger/decorative roles. `export.py` reproduces sprites, preview, GIF, and manifest from `source.png` with Pillow.

Prompt summary: one two-row, three-column transparent sheet of coherent animated blue-green water with cold-white highlights, soft glow, a water dragon head and body, charge swirl, right-opening slash, impact splash, and teleport mist. No characters, lettering, grid, or solid backdrop.

Checks performed: visually inspected source and dark/light 1× preview; all six silhouettes and right-facing head/slash are readable; inspected source partition cuts and confirmed no adjacent piece enters a sprite; checked all game PNG dimensions, RGBA alpha, actual semitransparent pixels, zero RGB at fully transparent pixels, and fully clear game-canvas borders; confirmed GIF has 12 frames. The source sheet's faint glow touches its outer edge, while every exported game PNG retains a clear border.

Remaining game-side checks: place sprites in the actual encounter at 1× over light/dark terrain and platforms; align dragon head/body joins and exact attack collision shape; verify slash follows the weapon and mirror direction on left-facing attacks; tune charge and teleport timing; compare actual frame time with effects enabled/disabled. These checks require current developer integration and were not run here.
