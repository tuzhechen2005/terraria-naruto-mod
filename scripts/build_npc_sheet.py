#!/usr/bin/env python3
"""Build a town-NPC frame sheet from a Codex delivery in the kakashi-npc-v2 format.

Source strips hold 224x200 cells drawn at 8x8 screen pixels per art pixel, facing right, the first cell of each
strip being an Idle size reference. This shrinks each cell 4x to 2x2 blocks, drops specks (components smaller than
`--min-speck` pixels), flips it to face left like vanilla town NPCs, aligns all feet on one baseline in 56x56
frames stacked vertically, and cuts a head icon from the idle frame.

Usage:
  python3 scripts/build_npc_sheet.py OUT_SHEET OUT_HEAD STRIP:CELLS [STRIP:CELLS ...] [--brighten 1.0]
  e.g. ... Tazuna.png Tazuna_Head.png source/Tazuna_Idle_Walk.png:0,1,2,3,4,5,6 source/Tazuna_Idle_Jump_Sit_Throw.png:1,2,3,4,5
  Native-size sheets: add --cell 256x256 --frame 64 --base 62.
"""
import argparse
import numpy as np
from PIL import Image, ImageEnhance
from scipy import ndimage

p = argparse.ArgumentParser()
p.add_argument("sheet")
p.add_argument("head")
p.add_argument("strips", nargs="+")
p.add_argument("--brighten", type=float, default=1.0)
p.add_argument("--min-speck", type=int, default=6)
p.add_argument("--cell", default="224x200")
# Frame height and the feet's row in it: 56/54 for the 23-art-pixel town NPCs; 64/62 for the native-size ones
# (--cell 256x256, 28 art pixels tall, shown at 1x).
p.add_argument("--frame", type=int, default=56)
p.add_argument("--base", type=int, default=54)
a = p.parse_args()
cw, ch = (int(v) for v in a.cell.split("x"))
fw, fh, base = cw // 4, a.frame, a.base

cells = []
for spec in a.strips:
    path, idx = spec.rsplit(":", 1)
    strip = Image.open(path).convert("RGBA")
    for i in (int(v) for v in idx.split(",")):
        c = strip.crop((i * cw, 0, i * cw + cw, ch)).resize((cw // 4, ch // 4), Image.NEAREST)
        arr = np.array(c)
        solid = arr[:, :, 3] > 0
        labels, n = ndimage.label(solid, structure=np.ones((3, 3)))
        if n > 1:
            sizes = ndimage.sum(solid, labels, range(1, n + 1))
            for k, size in enumerate(sizes, start=1):
                if size < a.min_speck:
                    arr[labels == k] = 0
        c = Image.fromarray(arr)
        if a.brighten != 1.0:
            rgb = ImageEnhance.Color(ImageEnhance.Brightness(c.convert("RGB")).enhance(a.brighten)).enhance(1.1)
            out = rgb.convert("RGBA")
            out.putalpha(c.split()[3])
            c = out
        cells.append(c)

bottom = max(c.getbbox()[3] for c in cells)
sheet = Image.new("RGBA", (fw, fh * len(cells)), (0, 0, 0, 0))
for i, c in enumerate(cells):
    c = c.transpose(Image.FLIP_LEFT_RIGHT)
    sheet.paste(c, (0, i * fh + base - bottom), c)
sheet.save(a.sheet)

idle = cells[0].transpose(Image.FLIP_LEFT_RIGHT)
x0, y0, x1, y1 = idle.getbbox()
cx = (x0 + x1) // 2
idle.crop((cx - 13, y0, cx + 13, y0 + 26)).save(a.head)
print(a.sheet, sheet.size, "frames", len(cells))
