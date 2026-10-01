status: delivered
request ID: orochimaru-moves-v1

## Delivered files

All paths below are relative to this delivery directory.

| Files | Dimensions | Alpha |
| --- | --- | --- |
| `Orochimaru_Idle_0.png`–`Orochimaru_Idle_3.png` | 112×88 each | 0/255 only |
| `Orochimaru_Walk_0.png`–`Orochimaru_Walk_3.png` | 112×88 each | 0/255 only |
| `Orochimaru_Disguise_0.png` | 112×88 | 0/255 only |
| `Orochimaru_DisguiseWalk_0.png`–`Orochimaru_DisguiseWalk_3.png` | 112×88 each | 0/255 only |
| `Orochimaru_Reveal_0.png`–`Orochimaru_Reveal_2.png` | 112×88 each | 0/255 only |
| `Orochimaru_Emerge_0.png`–`Orochimaru_Emerge_2.png` | 112×88 each | 0/255 only |
| `Orochimaru_Sink_0.png`–`Orochimaru_Sink_2.png` | 112×88 each | 0/255 only |
| `Orochimaru_Hurt_0.png` | 112×88 | 0/255 only |
| `source/idle_walk_generated.png` | 2172×724 | RGBA, intermediate alpha |
| `source/disguise_reveal_generated.png` | 2172×724 | RGBA, intermediate alpha |
| `source/emerge_sink_hurt_generated.png` | 2200×715 | RGBA, intermediate alpha |
| `preview.png` | 1100×4410 | Opaque RGB |

`source/assemble.py` records the sampling, palette matching, pixel cleanup, and preview layout. The 23 game-size frames are in the delivery root. `Orochimaru_Idle_0.png` and `Orochimaru_Disguise_0.png` are byte-identical to the approved v3 sprites; `Orochimaru_Emerge_2.png` uses that approved idle pose as the fully emerged endpoint.

## Prompt summary

Used the built-in `image_gen__imagegen` tool with the approved v3 Orochimaru sprites as character references and the named Tazuna comparison as the pixel-art style reference. Generated three transparent pose sheets: true-form idle and slow walk; disguised walk and face reveal; emergence, snake-swarm exit, and hurt. Prompts required right-facing poses, consistent proportions and baseline, pale face and gold snake eye in true form, green grass-ninja disguise, ivory clothing and purple rope in true form, hard pixel clusters, and no text or background.

## Checks performed

- Visually inspected the three generated source sheets and the 1×/4× action-row preview against the approved v3 pose.
- Sampled every pose onto a 2×2 screen-pixel grid, matched colors to the approved v3 sprites, adjusted the idle hair and shirt edge one art pixel at a time, and removed isolated pixels from the hurt and final sinking frames.
- Verified all 23 frame files decode at 112×88, have only alpha 0 or 255, contain no mismatched 2×2 blocks, and have their lowest opaque row at y=83. Full-height frames remain centered around x=56; emergence and sinking heights change with the action.
- Verified the approved idle and disguise anchor frames are byte-identical to v3. Checked the face, gold eye, purple rope, action sequence, right-facing silhouette, and small-scale readability in the preview.

## Remaining game-side checks

- Import the frames in tModLoader and check draw offsets, facing, collision-box alignment, and animation cadence during the encounter.
- Check readability against forest backgrounds and whether reveal, emergence, and snake-swarm timing matches the implemented encounter. No game build or in-game check was performed by this art worker.
