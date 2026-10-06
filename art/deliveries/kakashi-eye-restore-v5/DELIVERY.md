status: delivered
request ID: kakashi-eye-restore-v5

## Delivered files

- `frames/Kakashi_00_Idle.png` through `frames/Kakashi_11_Throw.png`, plus `frames/Kakashi_Trapped_0.png` and `frames/Kakashi_Trapped_1.png`: complete 14-frame set. Each is 80×80 RGBA with binary alpha (0/255), transparent RGB zero. Only `Kakashi_01_Walk.png` through `Kakashi_06_Walk.png` changed; the other eight are byte-identical copies of the request's current-frame source.
- `approved_idle_source.png`: 80×80 RGBA, binary alpha, exact copy of the approved v2 Idle. The current Idle was checked pixel-identical to this source.
- `imagegen_eye_study.png`: native built-in imagegen PNG, 1254×1254 RGBA with smooth alpha. Source study only; **not a game frame** because it changed the pose and eye design.
- `eyes_after_8x.png`: 1296×168 opaque RGB comparison of Idle plus six repaired walk eyes.
- `eyes_before_after_8x.png`: 2584×168 opaque RGB, before seven frames followed by after seven frames.
- `walk_full_1x.png`: 624×96 opaque RGB; real-size Idle and walk contact sheet.
- `walk_full_3x.png`: 1744×256 opaque RGB; nearest-neighbor enlarged contact sheet.
- `walk_01_game_size.png`: 80×80 RGBA, binary alpha; single game-size sample.
- `walk_01_3x.png`: 240×240 RGBA, binary alpha; nearest-neighbor enlargement.
- `walk_cycle.gif`: 240×240 opaque, six-frame walking loop, 130 ms per frame.
- `compose_eyes.py`: reproducible exact local composite and validation script.
- `PROMPT.md`: prompt summary and source-use note.

## Composite and checks

The donor eye neighborhood is source rectangle `(36,25,47,31)`, with right/bottom exclusive. Each donor pixel is applied only where both donor and target are opaque. The walk-frame destination rectangles are:

| Frame | Offset | Destination rectangle | Changed pixels |
| --- | --- | --- | ---: |
| 01 | (+3,+2) | (39,27,50,33) | 55 |
| 02 | (+2,+2) | (38,27,49,33) | 51 |
| 03 | (+2,+2) | (38,27,49,33) | 55 |
| 04 | (+2,+0) | (38,25,49,31) | 56 |
| 05 | (+1,+0) | (37,25,48,31) | 56 |
| 06 | (+2,+0) | (38,25,49,31) | 56 |

Ran `python3 compose_eyes.py` successfully. It checks all 14 dimensions; binary alpha and transparent RGB zero for source/game frames; exact pixel identity for the eight untouched frames; every changed pixel inside its listed ROI; and unchanged alpha/silhouette across the six edited frames. Thus body, walking phase, feet, centerline, and facing direction are preserved. Inspected the generated image, eye comparisons, 1× and 3× full-frame sheets. The final game-size sample remains legible as one exposed eye, with no enlarged black eye block.

## Remaining game-side checks

The development assistant should review all six walk frames and the GIF, rebuild the Kakashi sheet, then inspect 1× walking in Terraria, including left-facing flip, motion at normal speed, and transition into Idle. No game build or in-game check was run by this art worker.
