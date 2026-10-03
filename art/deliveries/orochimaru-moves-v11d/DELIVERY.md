status: delivered
request ID: orochimaru-moves-v11d

## Delivered files

All paths are relative to this directory. The 11 runtime sprites are `224×136` PNG, facing right, with binary Alpha (`0` or `255`) and lowest opaque row `y=131`:

| File | Opaque pixels | Visible tunic chest width* |
|---|---:|---:|
| `Orochimaru_WindIn_0.png` | 2810 | 21 px |
| `Orochimaru_Wind_0.png` | 2632 | 22 px |
| `Orochimaru_Wind_1.png` | 2419 | 17 px |
| `Orochimaru_NeckIn_0.png` | 3006 | 28 px |
| `Orochimaru_Neck_0.png` | 2475 | 29 px |
| `Orochimaru_SealIn_0.png` | 2454 | 22 px |
| `Orochimaru_Seal_0.png` | 2506 | 22 px |
| `Orochimaru_Seal_1.png` | 2767 | 21 px |
| `Orochimaru_SummonIn_0.png` | 2648 | 19 px |
| `Orochimaru_Summon_0.png` | 2496 | 18 px |
| `Orochimaru_Summon_1.png` | 2302 | 20 px |

`preview.png` is a `2688×1224` PNG with binary Alpha, showing the 11 frames at 3× nearest-neighbor scale with the red `Idle_0` outline overlaid. Order is the table order, left to right in four columns; the final twelfth cell is empty.

`imagegen-pose-source.png` is the built-in image generation source, copied from `$CODEX_HOME/generated_images/01a1038d-cff4-7961-98ee-16d92a75eb27/exec-758c8937-7902-4c05-9773-53f6d91a5b6c.png`. It is `1536×1024` RGBA with mixed Alpha values (`0–254`). It is a **pose guide only**, not a runtime sprite: its soft edges/background and body scale did not meet the requested pixel rules. The 11 delivered runtime PNGs were pixel-finished using `Idle_0` as the exact-scale head and palette reference and the named prior frames as action references.

\*Chest width is the median horizontal span of visible light tunic pixels across the torso area, measured with the same method on every frame; bent poses and crossing arms can hide part of the chest. The same measure on `Idle_0` is 24 px. This is an audit measure, not a collision-box width.

## Prompt summary

Generate a transparent 11-pose side-view Orochimaru sheet from the supplied `Idle_0`: wind inhale/exhale, neck lean/headless root, seal wind-up/forward five-finger hand, and summoning thumb bite/ground crouch; retain the small face, slim proportions, cream clothing, purple rope, dark trousers, sandals, right-facing orientation and pixel-art character identity.

## Checks performed

- Visually inspected `Idle_0`, named prior action frames, generated pose source, `preview.png`, and game-size final frames.
- All 11 runtime PNGs decode at `224×136`; their Alpha values are only `0/255`, their colours are drawn from the `Idle_0` palette, and their lowest opaque row is `131`.
- Every frame has 2302–3006 opaque pixels, within `Idle_0`'s 2790 ±20% target (`2232–3348`).
- Checked right-facing silhouette and pose sequence at game scale. The head/face pixels on headed frames were copied from `Idle_0` at 1:1 scale; `Neck_0` has no head.
- `Neck_0` neck-root tip centre: **(128, 65)**, pointing upper-right. `Seal_1` forward fingertips centre: approximately **(142, 63)**.

## Remaining game-side checks

- Integrate the 11 sprites into the mod and check animation transitions, perceived size beside `Idle_0`, neck connection at (128, 65), and seal effect alignment around (142, 63) in tModLoader.
- No build, reload, or in-game acceptance check was run in this art-worker task.
