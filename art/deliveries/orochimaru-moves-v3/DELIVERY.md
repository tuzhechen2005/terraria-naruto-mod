# Orochimaru moves v3 delivery

- status: delivered
- request ID: `orochimaru-moves-v3`
- delivery root: `art/deliveries/orochimaru-moves-v3/`

## Delivered files

All game frames are **112 × 88 RGBA PNG**, with transparent backgrounds and alpha values limited to **0 or 255**:

- `Orochimaru_Idle_0.png`, `Orochimaru_Idle_1.png`, `Orochimaru_Idle_2.png`, `Orochimaru_Idle_3.png`
- `Orochimaru_Walk_0.png`, `Orochimaru_Walk_1.png`, `Orochimaru_Walk_2.png`, `Orochimaru_Walk_3.png`
- `Orochimaru_DisguiseWalk_0.png`, `Orochimaru_DisguiseWalk_1.png`, `Orochimaru_DisguiseWalk_2.png`, `Orochimaru_DisguiseWalk_3.png`
- `Orochimaru_Disguise_0.png` (extra compatibility frame matching the v1 filename set)
- `Orochimaru_Reveal_0.png`, `Orochimaru_Reveal_1.png`, `Orochimaru_Reveal_2.png`
- `Orochimaru_Emerge_0.png`, `Orochimaru_Emerge_1.png`, `Orochimaru_Emerge_2.png`
- `Orochimaru_Sink_0.png`, `Orochimaru_Sink_1.png`, `Orochimaru_Sink_2.png`
- `Orochimaru_Hurt_0.png`

Review files in the delivery root:

- `preview.png` — 1750 × 2030 RGB; every action row starts with the approved v3 idle comparison.
- `idle_walk_idle_strip.png` — 4032 × 264 RGB; enlarged Idle → Walk → Idle contact strip.
- `idle_walk_idle.gif` — 336 × 264 palette-indexed animation preview.

Source files:

- `source/Orochimaru_Idle_v3_reference.png` and `source/Orochimaru_Disguise_v3_reference.png` — 112 × 88 RGBA copies of the approved references, alpha 0/255.
- `source/walk_pose_generated.png` and `source/disguise_walk_pose_generated.png` — 2172 × 724 RGBA transparent pose sheets generated with the built-in image tool; source alpha includes intermediate values.
- `source/assemble.py` — reproducible frame assembly, palette correction, and preview export.

## Prompt summary

The built-in image generator was given each approved v3 sprite as an image reference and asked for four right-facing walk poses in the same Terraria pixel-art style, preserving face, hair, body width, clothing, and foot baseline. The disguise prompt specified long black hair, dark green candidate clothing, and black trousers. The generated sheets guided pose variation. Final game frames were assembled from the exact approved v3 master pixels and restricted to the supplied palette, plus four dark greens for the disguise.

## Checks performed

- Visually inspected the generated pose sheets and the final side-by-side preview at enlarged scale.
- Checked all 23 game frames: 112 × 88 RGBA, transparent, alpha only 0/255, solid 2 × 2 pixel blocks, and no colors outside the request palette plus four disguise greens.
- Confirmed `Idle_0` exactly matches the approved v3 idle sprite; all four normal walk frames and all four disguise walk frames are distinct.
- Checked right-facing direction, centered placement, and the standing/walking foot baseline in the preview. Full-height frames end at y = 83.
- Confirmed the GIF and strip open and show Idle → Walk → Idle.

## Remaining game-side checks

- Integrate the PNGs in the mod, then check idle/walk switching and disguise/reveal transitions at native game scale and actual animation timing.
- Check emerge/sink ground alignment, hurt timing, and any texture packing requirements in tModLoader.
- Build and in-game acceptance were not run by this art worker.
