# Hiruzen NPC v2 delivery

- status: delivered
- request ID: `hiruzen-npc-v2`
- Date: 2026-10-01

## Game-ready source strips

All source cells are 224×200 PNG (28×25 art pixels, nearest-neighbor enlarged 8×), face right, use binary transparency, and share the same foot baseline. The first cell of each strip is Idle.

| File | Dimensions | Pose order, left to right |
| --- | ---: | --- |
| `source/Hiruzen_Idle_Walk.png` | 1568×200, RGBA | Idle holding pipe; Walk 1–6 |
| `source/Hiruzen_Idle_Jump_Sit_Throw.png` | 1344×200, RGBA | Idle; Jump; Sit smoking pipe; Throw windup, release, follow-through |
| `source/Hiruzen_Talk.png` | 672×200, RGBA | Idle; exhale smoke; raise hand to speak |

## Other delivered files

| File | Dimensions and alpha | Purpose |
| --- | --- | --- |
| `source/generated_Hiruzen_Idle_Walk.png` | 2172×724 RGBA, varying alpha | Selected built-in image generation output before sampling |
| `source/generated_Hiruzen_Idle_Jump_Sit_Throw.png` | 2172×724 RGBA, varying alpha | Selected built-in image generation output before sampling |
| `source/generated_Hiruzen_Talk.png` | 2172×724 RGBA, varying alpha | Selected built-in image generation output before sampling |
| `Guide_Hiruzen_Tazuna_comparison.png` | 560×360 RGB, opaque | Tazuna and Hiruzen idle side by side at 1× and 4× on dark and light backgrounds |
| `Hiruzen_build_check.png` | 56×784 RGBA, transparent | 14-frame game-scale build check output |
| `Hiruzen_head_check.png` | 26×26 RGBA, transparent | Head crop produced by build check |

## Prompt summary

Used the built-in `image_gen__imagegen` tool with Tazuna's source strip as the style reference. Generated Hiruzen as an elderly, short Hokage in a warm-white broad conical hat and red-trimmed white robe, with a red 火 emblem, gray-white hair and goatee, dark inner collar, and long brown pipe. Requested Tazuna-like proportions, warm hard color steps, near-black outlines, right-facing distinct poses, and transparent backgrounds. Corrected an initial red-dominant hat generation to a white hat. Sampled the selected generated strips into 28×25 art-pixel cells, enlarged each pixel 8×, and reinforced the small red hat emblem at game scale.

## Checks performed

- Inspected all three selected generated strips, final strips, 1×/4× comparison, and game-scale build output visually.
- Checked all 16 source cells for exact 8×8 blocks, RGBA alpha values limited to 0 and 255, foot baseline, and a red emblem block of at least 2×2 art pixels.
- Ran `scripts/build_npc_sheet.py` using Walk 0–6, Jump/Sit/Throw 1–5, and Talk 1–2. It generated a 56×784 sheet with 14 frames and a 26×26 head crop without error.

## Remaining game-side checks

- Integrate the strips into the NPC build and inspect Hiruzen alongside Tazuna in the actual game at native zoom, including the talk smoke and action transitions.
- Confirm the final head icon and all 14 frames in the tModLoader UI and world. The build check is not an in-game acceptance test.
