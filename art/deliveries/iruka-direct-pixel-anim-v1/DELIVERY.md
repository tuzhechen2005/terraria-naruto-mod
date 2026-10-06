status: delivered

# iruka-direct-pixel-anim-v1

## Delivered files

- `frames/Iruka_00_Idle.png` through `frames/Iruka_11_Throw.png`: twelve RGBA game frames, each 80×80, right-facing.
- `source/generated_walk_strip.png`, `source/generated_action_strip.png`: original built-in image generation outputs, each 2172×724 RGBA.
- `source/Iruka_01_Walk_pose.png` through `source/Iruka_11_Throw_pose.png`: eleven cropped high-resolution pose cells. Their heights are 724 px; widths are 362 px for walk and 434 or 435 px for actions.
- `source/approved_idle.png`, `source/approved_Iruka_B_eye_two_cells.png`: copies of the two approved references, 64×80 and 1024×1536 RGBA.
- `source/PROMPTS.md`: generation prompt summary.
- `export_frames.py`: repeatable crop, sample, anchor and head-compositing script.
- `strip_1x_3x.png`: 2880×640 RGB contact sheet, twelve frames in one row at 1× and 3× on light and dark backgrounds.
- `walk.gif`: 240×240, six-frame 3× loop at 100 ms per frame.
- `heads_6x.png`: 2088×156 RGBA head comparison.

## Frame provenance and alignment

| Frames | Body source | Foot row | Approved head top row |
| --- | --- | ---: | ---: |
| 00 Idle | Exact approved 64×80 frame, shifted +6 px horizontally | 75 | 14 |
| 01–06 Walk | Six generated walk cells, respectively | 75 | 16, 15, 16, 15, 16, 16 |
| 07 Jump | Generated action cell 1 | 69 | 15 |
| 08 Sit | Generated action cell 2 | 75 | 21 |
| 09–11 Throw | Generated action cells 3–5, respectively | 75 | 16, 16, 16 |

All eleven generated pose cells use the same 0.1 nearest-neighbor sample ratio. The idle frame is a pixel-exact copy of the previously approved game-size frame, translated six pixels so its vest is centered at x=40. Generated bodies are positioned using the green vest center and their final visible foot row. No pose is independently fit to 62 px high. The approved head patch is the 29×26 rectangle `x=17..45, y=14..39` from `source/approved_idle.png`, pasted at `x=23..51` and the head-top rows above. The script clears the generated head in `x=18..58` above the matching neck cutoff, preserving the raised windup fist. Detached generation specks are removed; a stray left-edge pixel in the recovery cell is removed.

## Checks performed

- Inspected both original generation outputs and the final 1×/3× light/dark contact sheet and 6× head strip.
- Confirmed twelve 80×80 frames, right-facing pose silhouettes, six distinct walk images, jump, seat and three throw phases.
- Confirmed binary alpha only (0/255) and zero RGB in transparent pixels in every final frame.
- Confirmed all ground frames end at y=75 and jump ends at y=69.
- Confirmed the approved idle frame is pixel-identical after its +6 px shift. Every final frame contains all 424 opaque pixels of the approved head patch unchanged at its recorded position, including the eye, scar and lack of mouth line.
- Checked body placement against x=40; torso color and arm swing change its apparent midpoint slightly between poses. No frame is clipped by the 80×80 canvas.

## Remaining game-side checks

Claude should assemble the 12-frame vertical NPC sheet and head texture, set `NPC.scale = 1`, then check in Terraria that walking has no visible horizontal jitter, feet meet the ground, the seated pose aligns with chairs, and the throw release lines up with the kunai projectile. Game integration and live gameplay were not performed by this art worker.

## Claude 修正：头和身体分离（用户 2026-10-05）

- 用户：“伊鲁卡的动作帧有几帧头和身体分离了”。原因：`export_frames.py` 只贴已认可帧 y=14..39 的头部，下巴下面两行（下颌阴影和脖子，站立帧 y=40..41）取自各生成姿势：走路帧缺脖子，跳和投掷帧还留着生成图自己的下巴，伸在脸下面。
- `fix_necks.py`：从 `frames_before_neck_fix/`（Codex 原交付）读入，清掉头下 6 行、x=36..55 里生成的肤色及其红褐描边（避开左侧手臂和投掷帧伸出的手），再把站立帧的脖子两行贴到头下；写回 `frames/`。站立帧不变。
- 检查：12 帧 alpha 0/255，地面帧脚底 y=75、跳 y=69 不变；6 倍目视每帧下巴都接在脖子和领口上，投掷第 10 帧的手保留。已重新拼 `Iruka.png`。`strip_1x_3x.png`、`walk.gif`、`heads_6x.png` 仍是修正前的预览。
