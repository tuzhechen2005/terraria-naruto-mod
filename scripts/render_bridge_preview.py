#!/usr/bin/env python3
"""Render tools/BridgePreview output to a PNG so the bridge design can be judged without the game.

Usage: python3 scripts/render_bridge_preview.py in.jsonl out.png
Flat colours per material; slopes drawn as triangles; objects as simple silhouettes. Not the in-game look,
only the shapes, proportions and composition.
"""
import json
import sys
from PIL import Image, ImageDraw

T = 8  # pixels per tile
COL = {
    "Brick": (128, 128, 136), "Slab": (170, 170, 176), "Stone": (100, 100, 108), "Sand": (214, 196, 128),
    "Dirt": (140, 98, 64), "Grass": (60, 170, 70), "Wood": (160, 110, 60), "DynastyWood": (190, 150, 100),
    "Shingle": (60, 90, 170), "Beam": (130, 90, 50), "Platform": (180, 130, 70), "Rope": (220, 200, 150),
    "RedBeam": (200, 50, 40), "RedWood": (210, 60, 45),
}
WALL = {"StoneWall": (90, 90, 96), "Railing": (110, 110, 118), "ShojiWall": (230, 225, 205),
        "Glass": (160, 210, 230), "WoodWall": (120, 85, 55)}

rows = [json.loads(l) for l in open(sys.argv[1])]
meta = rows[0]
cells = [r for r in rows if r["kind"] == "cell"]
o0 = meta["landmost"] - 2
o1 = meta["reach"]
ys = [c["y"] for c in cells] + [meta["waterY"] + 25]
y0, y1 = min(ys) - 3, max(ys) + 1
W, H = (o1 - o0 + 1) * T, (y1 - y0 + 1) * T
img = Image.new("RGB", (W, H), (150, 190, 225))
d = ImageDraw.Draw(img)

def box(o, y):
    x = (o - o0) * T
    yy = (y - y0) * T
    return x, yy, x + T - 1, yy + T - 1

water = meta["waterY"]
d.rectangle((0, (water - y0) * T, W, H), fill=(50, 110, 170))
for r in rows:
    if r["kind"] == "terrain":
        x0, _, x1, _ = box(r["o"], 0)
        d.rectangle((x0, (r["ground"] - y0) * T, x1, H), fill=(214, 196, 128) if r["o"] <= 0 else (180, 160, 110))
for r in rows:
    if r["kind"] == "wall":
        d.rectangle(box(r["o"], r["y"]), fill=WALL[r["b"]])
for c in cells:
    x0, y0_, x1, y1_ = box(c["o"], c["y"])
    col = COL[c["part"]]
    s = c["shape"]
    if c["part"] in ("Beam", "RedBeam"):
        d.rectangle((x0 + 2, y0_, x1 - 2, y1_), fill=col)
    elif c["part"] == "Rope":
        d.line((x0 + T // 2, y0_, x0 + T // 2, y1_), fill=col, width=2)
    elif c["part"] == "Platform":
        d.rectangle((x0, y0_, x1, y0_ + 2), fill=col)
    elif s == "Full":
        d.rectangle((x0, y0_, x1, y1_), fill=col)
    elif s == "Half":
        d.rectangle((x0, y0_ + T // 2, x1, y1_), fill=col)
    elif s == "RisesSeaward":  # sea is to the right in the preview
        d.polygon([(x0, y1_), (x1, y0_), (x1, y1_)], fill=col)
    elif s == "RisesLandward":
        d.polygon([(x0, y0_), (x1, y1_), (x0, y1_)], fill=col)
    elif s == "CeilingRisesSeaward":
        d.polygon([(x0, y0_), (x1, y0_), (x0, y1_)], fill=col)
    elif s == "CeilingRisesLandward":
        d.polygon([(x0, y0_), (x1, y0_), (x1, y1_)], fill=col)
for r in rows:
    if r["kind"] != "fixture":
        continue
    x0, y0_, x1, y1_ = box(r["o"], r["y"])
    f = r["f"]
    if f == "LampPost":
        d.rectangle((x0 + 3, (r["y"] - 5 - y0) * T, x0 + 4, y1_), fill=(40, 40, 40))
        d.ellipse((x0 + 1, (r["y"] - 6 - y0) * T, x1 - 1, (r["y"] - 5 - y0) * T + 2), fill=(255, 230, 120))
    elif f == "Palm":
        d.rectangle((x0 + 3, (r["y"] - 8 - y0) * T, x0 + 5, y0_), fill=(150, 110, 70))
        d.ellipse((x0 - 12, (r["y"] - 10 - y0) * T, x1 + 12, (r["y"] - 7 - y0) * T), fill=(50, 150, 60))
    elif f in ("IslandSign", "BridgeSign"):
        d.rectangle((x0, y0_ - T, x0 + 2 * T - 1, y1_), fill=(170, 120, 60))
    elif f == "Chest":
        d.rectangle((x0, y0_ - T, x0 + 2 * T - 1, y1_), fill=(150, 100, 40))
    elif f == "Door":
        d.rectangle((x0 + 1, y0_ - 2 * T, x1 - 1, y1_), fill=(110, 70, 40))
    elif f == "Table":
        d.rectangle((x0 - T, y0_ - T, x1 + T, y0_ - T + 2), fill=(150, 100, 60))
    elif f == "Chair":
        d.rectangle((x0 + 1, y0_ - T, x1 - 1, y1_), fill=(150, 100, 60))
    elif f == "Lantern":
        d.rectangle((x0 + 1, y0_, x1 - 1, y1_ + T), fill=(255, 200, 90))
    elif f == "Shell":
        d.ellipse((x0 + 1, y0_ + 3, x1 - 1, y1_), fill=(240, 220, 210))
d.line(((0 - o0) * T, 0, (0 - o0) * T, 6), fill=(255, 0, 0), width=2)  # shoreline marker
img = img.resize((W * 2, H * 2), Image.NEAREST)
img.save(sys.argv[2])
print(sys.argv[2], img.size)
