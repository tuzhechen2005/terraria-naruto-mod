#!/usr/bin/env python3
"""Build a town-NPC sheet drawn at 1x (one screen pixel per art pixel) from per-frame PNGs.

Each frame is a PNG facing right (skills/terraria-npc-pixel-art: 80x80, body centre x=40, feet on row 75). This flips
the frames to face left like vanilla town NPCs, stacks them vertically in the order given (NpcSheet order for town NPCs:
Idle, Walk x6, Jump, Sit, Throw x3; a frame may be repeated to fill a slot) and cuts a 30x26 head icon from the idle frame.

Vanilla draws a town NPC's frame bottom 4 px below its hitbox and its own sheets leave two empty rows under the feet,
so the feet sink 2 px into the ground. Every frame is shifted by the same amount so the idle frame's feet land on the
third row from the bottom, matching that.

Usage: python3 scripts/build_hires_npc_sheet.py [--head X,Y] OUT_SHEET OUT_HEAD FRAME0.png [FRAME1.png ...]
--head gives the icon's top-left corner in the finished (left-facing) idle cell, for heads whose bounding box is
dominated by hair (Iruka's ponytail); without it the icon is centred on the top of the idle frame.
"""
import sys
from PIL import Image

args = sys.argv[1:]
head_at = None
if args[0] == "--head":
    head_at = tuple(int(v) for v in args[1].split(","))
    args = args[2:]
out_sheet, out_head, *frames = args
cells = [Image.open(f).convert("RGBA") for f in frames]
w, h = cells[0].size
drop = (h - 3) - (cells[0].getbbox()[3] - 1)
sheet = Image.new("RGBA", (w, h * len(cells)))
for i, c in enumerate(cells):
    if c.size != (w, h):
        sys.exit(f"{frames[i]} is {c.size}, expected {(w, h)}")
    bb = c.getbbox()
    if bb[1] + drop < 0 or bb[3] + drop > h:
        sys.exit(f"{frames[i]} would be clipped by the {drop:+d} px shift")
    cell = Image.new("RGBA", (w, h))
    cell.paste(c, (0, drop))
    sheet.paste(cell.transpose(Image.FLIP_LEFT_RIGHT), (0, i * h))
sheet.save(out_sheet)
idle = sheet.crop((0, 0, w, h))
if head_at is None:
    x0, y0, x1, y1 = idle.getbbox()
    head_at = ((x0 + x1) // 2 - 15, y0)
idle.crop((head_at[0], head_at[1], head_at[0] + 30, head_at[1] + 26)).save(out_head)
print(out_sheet, sheet.size, f"shift {drop:+d}")
