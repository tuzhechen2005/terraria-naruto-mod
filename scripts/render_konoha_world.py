#!/usr/bin/env python3
"""Render a generated world's village (the KonohaDump output) with vanilla Terraria textures.

Usage: python3 scripts/render_konoha_world.py dump.jsonl out.png [--bg DIR] [--crop DX0 DX1]

Vanilla textures are extracted on demand from the local Terraria install into art/reference/terraria/tiles/
(gitignored; they stay local). Tiles are drawn from their real frames, walls from their wall frames, slopes and
half blocks masked, liquids tinted. Tree tops are drawn as simple canopies. Also prints vanilla's housing verdict
for every designed home (check / furniture / score).
"""
import json
import os
import struct
import subprocess
import sys
from PIL import Image, ImageDraw

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
TEX = os.path.join(ROOT, "art/reference/terraria/tiles")
HOME = os.path.expanduser("~")
IMAGES = HOME + "/Library/Application Support/Steam/steamapps/common/Terraria/Terraria.app/Contents/Resources/Content/Images"
TML = HOME + "/Library/Application Support/Steam/steamapps/common/tModLoader"


def extract(names):
    need = [n for n in names if not os.path.exists(os.path.join(TEX, n + ".png"))]
    if not need:
        return
    os.makedirs(TEX, exist_ok=True)
    tool = os.path.join(ROOT, "tools/XnbExtract/bin/Debug/net8.0/Xnbx.dll")
    if not os.path.exists(tool):
        subprocess.run([HOME + "/.dotnet-x64/dotnet", "build", os.path.join(ROOT, "tools/XnbExtract"), "-v", "q"], check=True)
    env = dict(os.environ, DYLD_LIBRARY_PATH=TML + "/Libraries/Native/OSX")
    files = [os.path.join(IMAGES, n + ".xnb") for n in need if os.path.exists(os.path.join(IMAGES, n + ".xnb"))]
    if files:
        subprocess.run([HOME + "/.dotnet-x64/dotnet", tool, TEX] + files, env=env, check=True,
                       stdout=subprocess.DEVNULL)
    for n in need:
        raw = os.path.join(TEX, n + ".rgba")
        if not os.path.exists(raw):
            continue
        b = open(raw, "rb").read()
        w, h, _ = struct.unpack("<iii", b[:12])
        im = Image.frombytes("RGBA", (w, h), b[12:12 + w * h * 4])
        px = im.load()
        for y in range(h):
            for x in range(w):
                r, g, bb, a = px[x, y]
                if 0 < a < 255:
                    px[x, y] = (min(255, r * 255 // a), min(255, g * 255 // a), min(255, bb * 255 // a), a)
        im.save(os.path.join(TEX, n + ".png"))
        os.remove(raw)


cache = {}


def tex(name):
    if name not in cache:
        p = os.path.join(TEX, name + ".png")
        cache[name] = Image.open(p).convert("RGBA") if os.path.exists(p) else None
    return cache[name]


def main():
    args = sys.argv[1:]
    src, out = args[0], args[1]
    bg = args[args.index("--bg") + 1] if "--bg" in args else None
    rows = [json.loads(l) for l in open(src)]
    meta = rows[0]
    tiles = [r for r in rows if "x" in r and "k" not in r]
    rooms = [r for r in rows if r.get("k") == "room"]
    x0, x1, y0, y1 = meta["x0"], meta["x1"], meta["y0"], meta["y1"]
    if "--crop" in args:
        i = args.index("--crop")
        x0, x1 = meta["cx"] + int(args[i + 1]), meta["cx"] + int(args[i + 2])
    extract(sorted({f"Tiles_{r['t']}" for r in tiles if "t" in r} | {f"Wall_{r['w']}" for r in tiles if "w" in r}))

    W, H = (x1 - x0 + 1) * 16, (y1 - y0 + 1) * 16
    img = Image.new("RGBA", (W, H), (0, 0, 0, 255))
    d = ImageDraw.Draw(img)
    for yy in range(H):  # sky gradient
        t = yy / H
        d.line((0, yy, W, yy), fill=(int(95 + 60 * t), int(150 + 50 * t), int(230 + 15 * t), 255))
    if bg:
        for layer in ("far", "mid", "close"):
            p = os.path.join(bg, layer + ".png")
            if os.path.exists(p):
                lay = Image.open(p).convert("RGBA")
                scale = 2
                lay = lay.resize((lay.width * scale, lay.height * scale), Image.NEAREST)
                base = (meta["gy"] - y0) * 16 + 40
                for ox in range(0, W, lay.width):
                    img.alpha_composite(lay, (ox, max(0, base - lay.height)))

    def at(x, y):
        return (x - x0) * 16, (y - y0) * 16

    for r in tiles:  # walls
        if "w" not in r or not (x0 <= r["x"] <= x1):
            continue
        t = tex(f"Wall_{r['w']}")
        if t is None:
            continue
        piece = t.crop((r["wfx"], r["wfy"], r["wfx"] + 32, r["wfy"] + 32))
        px, py = at(r["x"], r["y"])
        img.alpha_composite(piece, (px - 8, py - 8))
    shade = Image.new("RGBA", (W, H), (0, 0, 0, 0))
    for r in tiles:  # tiles
        if "t" not in r or not (x0 <= r["x"] <= x1):
            continue
        px, py = at(r["x"], r["y"])
        t = tex(f"Tiles_{r['t']}")
        if t is None:
            d.rectangle((px, py, px + 15, py + 15), fill=(255, 0, 255, 255))
            continue
        if r["t"] == 5 and r["fx"] >= 22 and r["fy"] >= 198:  # tree top: a round canopy
            d.ellipse((px - 40, py - 70, px + 56, py + 10), fill=(40, 130, 50, 255), outline=(25, 90, 35, 255))
            continue
        piece = t.crop((r["fx"], r["fy"], r["fx"] + 16, r["fy"] + 16))
        if r.get("hb"):
            piece = piece.crop((0, 0, 16, 8))
            img.alpha_composite(piece, (px, py + 8))
            continue
        sl = r.get("sl", 0)
        if sl:
            mask = Image.new("L", (16, 16), 0)
            md = ImageDraw.Draw(mask)
            tri = {1: [(0, 0), (0, 16), (16, 16)], 2: [(16, 0), (16, 16), (0, 16)],
                   3: [(0, 0), (16, 0), (0, 16)], 4: [(0, 0), (16, 0), (16, 16)]}[sl]
            md.polygon(tri, fill=255)
            clear = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
            piece = Image.composite(piece, clear, mask)
        img.alpha_composite(piece, (px, py))
    for r in tiles:  # liquids
        if "l" in r and x0 <= r["x"] <= x1:
            px, py = at(r["x"], r["y"])
            h = r["l"] * 16 // 255
            col = {0: (40, 90, 200, 150), 1: (230, 80, 20, 200), 2: (230, 180, 40, 180)}.get(r.get("lt", 0))
            ImageDraw.Draw(shade).rectangle((px, py + 16 - h, px + 15, py + 15), fill=col)
    img.alpha_composite(shade)
    sx = (meta["spawnX"] - x0) * 16
    if 0 <= sx < W:
        sy = (meta["spawnY"] - y0) * 16
        d.rectangle((sx, sy - 42, sx + 16, sy - 1), outline=(255, 0, 255, 255), width=2)
    img.convert("RGB").save(out)

    bad = [r for r in rooms if not (r["check"] and r["needs"] and r["score"] > 0)]
    print(f"{len(rooms)} homes, {len(rooms) - len(bad)} valid by vanilla housing rules; ground y={meta['gy']}, world {meta['w']}x{meta['h']}")
    for r in bad:
        print(f"  NOT VALID: {r['b']} x{r['x0'] - meta['cx']}..{r['x1'] - meta['cx']} y{r['top'] - meta['gy']}..{r['bottom'] - meta['gy']}"
              f" check={r['check']} needs={r['needs']} score={r['score']}")


main()
