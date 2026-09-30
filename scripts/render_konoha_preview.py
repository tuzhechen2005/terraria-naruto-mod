#!/usr/bin/env python3
"""Flat-colour layout preview of the Leaf village design (tests/KonohaDesign dump), to judge composition.

Usage: python3 scripts/render_konoha_preview.py in.jsonl out.png [x0 x1]
Not the in-game look (see scripts/render_konoha_world.py for the textured render of a generated world).
"""
import json
import sys
from PIL import Image, ImageDraw

T = 6
COL = {"Grass": (70, 170, 70), "Dirt": (140, 98, 64), "Slab": (170, 170, 176), "Brick": (128, 128, 136),
       "RedBrick": (180, 60, 45), "Stucco": (230, 214, 170), "Marble": (240, 240, 235), "DynastyWood": (190, 150, 100),
       "Wood": (160, 110, 60), "RedShingle": (200, 70, 50), "BlueShingle": (70, 100, 180), "Plating": (170, 190, 200),
       "Beam": (130, 90, 50), "Platform": (180, 130, 70), "LivingWood": (120, 80, 40)}
WALL = {"Planks": (150, 115, 80), "Stucco": (200, 185, 150), "Shoji": (235, 228, 205), "Marble": (205, 205, 200),
        "RedBrick": (140, 60, 50), "Wood": (120, 85, 55), "Brick": (90, 90, 96), "Palm": (200, 160, 110), "RedStucco": (190, 60, 50), "Fence": (130, 95, 60), "DoorLeaf": (110, 50, 40)}
FIX = {"Door": (110, 70, 30), "Table": (90, 60, 30), "Chair": (90, 60, 30), "Lantern": (255, 200, 60),
       "LampPost": (60, 60, 60), "Sign": (200, 170, 110), "Banner": (240, 240, 240), "Tree": (40, 120, 50),
       "Bookcase": (100, 70, 40), "Bed": (180, 60, 60)}
SIZE = {"Door": (1, 3), "Table": (3, 2), "Chair": (1, 2), "Lantern": (1, 2), "LampPost": (1, 6), "Sign": (2, 2),
        "Banner": (1, 3), "Tree": (3, 14), "Bookcase": (3, 4), "Bed": (4, 2)}

rows = [json.loads(l) for l in open(sys.argv[1])]
xs = [r["x"] for r in rows if "x" in r]
x0 = int(sys.argv[3]) if len(sys.argv) > 4 else min(xs) - 2
x1 = int(sys.argv[4]) if len(sys.argv) > 4 else max(xs) + 2
y0, y1 = -48, 6
img = Image.new("RGB", ((x1 - x0 + 1) * T, (y1 - y0 + 1) * T), (130, 180, 235))
d = ImageDraw.Draw(img)

def box(x, y, w=1, h=1):
    return ((x - x0) * T, (y - y0) * T, (x - x0 + w) * T - 1, (y - y0 + h) * T - 1)

for r in rows:
    if r["k"] == "w":
        d.rectangle(box(r["x"], r["y"]), fill=WALL[r["m"]])
for r in rows:
    if r["k"] != "c":
        continue
    x, y, c, s = r["x"], r["y"], COL[r["m"]], r["s"]
    a, b, e, f = box(x, y)
    if r["m"] == "Platform":
        d.rectangle((a, b, e, b + 1), fill=c)
    elif r["m"] == "Beam":
        d.rectangle((a + 1, b, e - 1, f), fill=c)
    elif s == "TopRisesEast":
        d.polygon([(a, f), (e, b), (e, f)], fill=c)
    elif s == "TopRisesWest":
        d.polygon([(a, b), (e, f), (a, f)], fill=c)
    else:
        d.rectangle((a, b, e, f), fill=c)
for r in rows:
    if r["k"] != "p":
        continue
    w, h = SIZE[r["m"]]
    x = r["x"] - (w // 2 if r["m"] in ("Table", "Tree", "Bookcase", "Bed") else 0)
    top = r["y"] if r["m"] in ("Lantern", "Banner") else r["y"] - h + 1
    d.rectangle(box(x, top, w, h), outline=FIX[r["m"]], fill=FIX[r["m"]] if r["m"] != "Tree" else None)
img.save(sys.argv[2])
print(img.size)
