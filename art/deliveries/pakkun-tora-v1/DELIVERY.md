# Pakkun and Tora pixel sprites

- status: delivered
- request ID: `pakkun-tora-v1`

## Delivered files

| Path | Dimensions | Alpha |
| --- | ---: | --- |
| `frames/Pakkun_Idle.png` | 40 × 32 | RGBA, 0/255 only |
| `frames/Pakkun_Run_0.png` through `frames/Pakkun_Run_3.png` | Each 40 × 32 | RGBA, 0/255 only |
| `frames/Pakkun_Bite_0.png`, `frames/Pakkun_Bite_1.png` | Each 40 × 32 | RGBA, 0/255 only |
| `frames/Pakkun_Icon.png` | 28 × 28 | RGBA, 0/255 only |
| `frames/Tora_Idle.png` | 36 × 28 | RGBA, 0/255 only |
| `frames/Tora_Run_0.png` through `frames/Tora_Run_3.png` | Each 36 × 28 | RGBA, 0/255 only |
| `frames/Tora_Caught.png` | 36 × 28 | RGBA, 0/255 only |
| `source/generated_Pakkun_poses.png` | 2100 × 749 | RGBA; generated source has antialiased alpha |
| `source/generated_Tora_poses.png` | 2172 × 724 | RGBA; generated source has antialiased alpha |
| `source/generated_Pakkun_Icon.png` | 1254 × 1254 | RGBA; generated source has antialiased alpha |
| `source/build_frames.py` | Source processing script | — |
| `preview.png` | 1600 × 1040 | Opaque RGB |

## Frame order

- Pakkun: `Idle`, `Run_0` to `Run_3` (loop), `Bite_0` (open-mouth lunge), `Bite_1` (follow-through).
- Tora: `Idle`, `Run_0` to `Run_3` (loop), `Caught` (upright struggle).
- All animal frames face right. The bottom occupied row is the same within each animal's frame set.

## Prompt summary

Built-in `image_gen__imagegen` made three transparent pixel-art sources: a seven-pose tan pug strip, a six-pose brown tabby strip, and a Pakkun-over-summoning-scroll icon. The prompts specified Pakkun's blue vest, dark wrinkled face, and leaf forehead protector; Tora's red bow on the left ear and angry expression; dark outlines, hard shading, right-facing poses, and transparent backgrounds. The generated strips were sampled into game frames at one art pixel per 2 × 2 screen pixels. The open bite received a tooth and mouth-color pixel touch-up after reduction.

## Checks performed

- Visually inspected all three generated sources and the final 1×/3× preview against dark and light backgrounds, including a 20 × 40 screen-pixel player-size proxy.
- Confirmed 14 individual game PNGs with the requested dimensions and nonempty silhouettes.
- Confirmed every game PNG uses only alpha 0 or 255 and every 2 × 2 screen-pixel block is uniform.
- Confirmed Pakkun's vest and forehead plate, Tora's red bow, right-facing action silhouettes, and common bottom alignment at game size.

## Remaining game-side checks

- Import frames into tModLoader and verify draw origins, horizontal flip, collision boxes, animation cadence, and run/bite/caught timing.
- Inspect the sprites in motion against Terraria daytime and nighttime backgrounds. The preview uses a generic player-size proxy, so compare against the actual player sprite in game.
