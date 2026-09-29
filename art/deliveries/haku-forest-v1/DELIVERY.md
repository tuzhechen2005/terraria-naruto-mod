# Haku forest NPC delivery

- status: delivered
- request ID: `haku-forest-v1`

## Delivered files

| Path | Dimensions | Alpha |
| --- | ---: | --- |
| `source/HakuForest_Idle_Walk.png` | 1568 × 200; 7 cells of 224 × 200 | Transparent; alpha 0 or 255 only |
| `source/HakuForest_Crouch_Talk.png` | 896 × 200; 4 cells of 224 × 200 | Transparent; alpha 0 or 255 only |
| `source/generated_HakuForest_Idle_Walk.png` | 2098 × 749 | Transparent generated source; includes partial alpha |
| `source/generated_HakuForest_Crouch_Talk.png` | 1881 × 836 | Transparent generated source; includes partial alpha |
| `Guide_HakuForest_comparison.png` | 460 × 260 | Opaque preview |

## Pose order

- `HakuForest_Idle_Walk.png`: Idle with herb basket; Walk 1–6, facing right.
- `HakuForest_Crouch_Talk.png`: same Idle; Crouch 1; Crouch 2 reaching toward the ground; standing Talk with a small smile, facing right.

## Prompt summary

Used the built-in `image_gen__imagegen` tool for two transparent action strips, following the named Haku face/hair, Zabuza and Haku style, Terraria Guide, and Kakashi layout references. The prompts specified loose black hair, no mask or headband, a pale pink short kimono, pale sash, sandals, a bamboo herb basket, right-facing poses, and chunky Terraria pixel art. The generated strips were cropped and scaled into 28 × 25 art-pixel cells at 8 × 8 screen pixels per art pixel; alpha was made binary in the game-size sheets.

## Checks performed

- Inspected both generated strips and both game-size sprite sheets visually.
- Confirmed all requested poses are present in separate cells, with a common foot baseline and right-facing silhouettes.
- Confirmed standing poses are 23 art pixels high; crouch poses are lower; first Idle cell is pixel-identical in both sheets.
- Confirmed final sheet dimensions, binary alpha, and uniform color in every 8 × 8 screen-pixel block.
- Compared Idle with `NPC_22_x4.png` in the included side-by-side preview for game-scale height and readability.

## Remaining game-side checks

- Import the sheets into the mod and verify sprite origin, direction, and visible size in Terraria.
- Play the six walking frames and the two crouch frames in sequence to assess motion and timing.
- Check dialogue pose, basket placement, and contrast against the intended forest background in game.
