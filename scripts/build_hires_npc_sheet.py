#!/usr/bin/env python3
"""Build a high-resolution town-NPC sheet (one screen pixel per art pixel, shown at 1x) from per-frame PNGs.

Each frame is an 80x80 PNG facing right with the feet on row 76 (art/requests/npc-hires-sample-*.md). This flips
the frames to face left like vanilla town NPCs, stacks them vertically in NpcSheet order (Idle, Walk x6, Jump, Sit,
Throw x3) and cuts a 30x26 head icon from the idle frame.

Usage: python3 scripts/build_hires_npc_sheet.py OUT_SHEET OUT_HEAD FRAME0.png ... FRAME11.png
"""
import sys
from PIL import Image

out_sheet, out_head, *frames = sys.argv[1:]
if len(frames) != 12:
    sys.exit(f"need 12 frames, got {len(frames)}")
cells = [Image.open(f).convert("RGBA") for f in frames]
size = cells[0].size
sheet = Image.new("RGBA", (size[0], size[1] * len(cells)))
for i, c in enumerate(cells):
    if c.size != size:
        sys.exit(f"{frames[i]} is {c.size}, expected {size}")
    sheet.paste(c.transpose(Image.FLIP_LEFT_RIGHT), (0, i * size[1]))
sheet.save(out_sheet)
idle = cells[0].transpose(Image.FLIP_LEFT_RIGHT)
x0, y0, x1, y1 = idle.getbbox()
cx = (x0 + x1) // 2
idle.crop((cx - 15, y0, cx + 15, y0 + 26)).save(out_head)
print(out_sheet, sheet.size)
