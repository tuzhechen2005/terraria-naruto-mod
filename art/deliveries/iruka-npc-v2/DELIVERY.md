status: delivered
request ID: iruka-npc-v2

# Delivered files

Game-size PNGs (RGBA, transparent, alpha strictly 0 or 255; 28×25 art pixels per 224×200 frame):

- `source/Iruka_Idle_Walk.png` — 1568×200; Idle 1 + Walk 6.
- `source/Iruka_Idle_Jump_Sit_Throw.png` — 1344×200; Idle 1 + Jump 1 + Sit 1 + Throw 3.
- `source/Iruka_Talk.png` — 672×200; Idle 1 + Talk 2.

Built-in imagegen edit outputs retained as transparent RGBA reference sources:

- `source/generated_Iruka_Idle_Walk.png` — 2172×724; alpha channel present, including partial alpha.
- `source/generated_Iruka_Idle_Jump_Sit_Throw.png` — 2172×724; alpha channel present, including partial alpha.
- `source/generated_Iruka_Talk.png` — 2170×725; alpha channel present, including partial alpha.

Comparison previews of the first Idle frame, beside Tazuna and Kakashi:

- `comparison_1x.png` — game-size preview, 720×248.
- `comparison_4x.png` — fourfold game-size preview, 2880×992.

# Prompt summary

Edited each of the three v1 generated Iruka sheets with the built-in image tool. The prompt preserved the existing pose sequence, direction, proportions, outfit, and warm pixel-art palette while targeting clearer eyes, a horizontal nose-bridge scar, a separate upward ponytail, a blue forehead band with a bright metal plate, hard-edged color blocks, and transparent background. The game-size sheets retain the v1 frame geometry and were pixel-corrected at 28×25 art-pixel scale for face, scar, ponytail notch/tie, and plate, then enlarged with nearest-neighbor scaling.

# Checks performed

- Visually inspected the original and generated sheets, all three game-size output sheets, and both comparison previews.
- Verified 7/6/3 frames, 224×200 pixels per frame, all poses facing right, and no frame art crossing a cell boundary.
- Verified exact 8×8 pixel-block alignment and binary alpha (0/255) on every game-size sheet.
- Checked the eye, two-tone face, scar, ponytail, blue forehead band, bright plate, and clothing readability in the Idle comparison at 1× and 4×.

# Remaining game-side checks

- Import the three game-size PNGs into tModLoader and confirm the frame order, animation timing, origin, and direction in play.
- Check face, scar, and ponytail visibility at the actual in-game zoom and against representative backgrounds; adjust the 28×25 art-pixel cells if the engine's rendering obscures a feature.
