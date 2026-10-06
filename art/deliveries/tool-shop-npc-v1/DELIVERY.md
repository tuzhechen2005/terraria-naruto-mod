# tool-shop-npc-v1 delivery

- status: delivered
- request ID: `tool-shop-npc-v1`

## Delivered files

| Path | Dimensions | Alpha | Contents |
| --- | ---: | --- | --- |
| `source/ToolShop_Idle_Walk.png` | 1568×200 | RGBA; 0/255 only | 7 cells, 224×200 each: Idle, Walk 1–6 |
| `source/ToolShop_Idle_Jump_Sit_Throw.png` | 1344×200 | RGBA; 0/255 only | 6 cells, 224×200 each: Idle, Jump, Sit (polishing kunai), Throw 1 (wind-up), Throw 2 (release), Throw 3 (recovery) |
| `Tazuna_ToolShop_comparison.png` | 448×200 | RGBA; 0/255 only | Tazuna idle left, tool shop idle right, both at source scale |
| `source/generated_ToolShop_Idle_Walk.png` | 2171×724 | RGBA; partial alpha present | Original built-in image generation output, retained for rework; game-ready source is the 1568×200 file above |
| `source/generated_ToolShop_Idle_Jump_Sit_Throw.png` | 2172×724 | RGBA; partial alpha present | Original built-in image generation output, retained for rework; game-ready source is the 1344×200 file above |

## Prompt summary

Built-in `image_gen__imagegen`, with Tazuna's delivered source sheets as style references. A right-facing, friendly but shrewd middle-aged ninja tool shopkeeper with a single round black hair bun, short beard, burgundy Chinese frog-button jacket, dark apron holding kunai and a scroll, waist pouch, dark trousers, shin wraps, and cloth shoes. Requested seven idle/walk poses and six idle/jump/sit/throw poses on genuine transparency, with warm upper-left light, dark outlines, and hard pixel shading. The second generation also used the first generated sheet as a character reference.

The generated poses were cropped and resampled into the requested 28×25 art-pixel cells, then enlarged by nearest-neighbor to 224×200 per cell. Low-alpha fringe pixels were removed.

## Checks performed

- Visually inspected both generated sheets, both game-size sheets, and the Tazuna comparison preview.
- Confirmed right-facing character, bun, burgundy jacket, apron, readable walking variations, seated kunai cleaning, and three throw phases.
- Confirmed game-size dimensions, 23-art-pixel standing height, fixed grounded foot baseline, frame isolation, and game-size color blocks aligned to the 8×8 grid.
- Verified game-size sheets are RGBA with only fully transparent or fully opaque pixels; no antialiasing or semi-transparent edge pixels.

## Remaining game-side checks

- Import/compile the NPC animation sheet and verify frame mapping, walking loop, sit, jump, and kunai throw in tModLoader.
- Inspect in-game at native Terraria scale alongside Tazuna, including actual background contrast and motion readability. These game-side checks were not run by the art worker.
