#!/usr/bin/env python3
"""Count speckle in pixel art: opaque pixels with no 4-neighbour of a similar colour (an isolated dot of its own
colour), and the number of distinct colours. Used to accept character art (art/AGENT_HANDOFF.md).

Usage: python3 scripts/pixel_noise.py IMAGE [IMAGE ...] [--tolerance 24]
"""
import argparse
from PIL import Image

p = argparse.ArgumentParser()
p.add_argument("images", nargs="+")
p.add_argument("--tolerance", type=int, default=24)
a = p.parse_args()
for path in a.images:
    im = Image.open(path).convert("RGBA")
    w, h = im.size
    px = im.load()
    opaque = isolated = 0
    colours = set()
    for y in range(h):
        for x in range(w):
            r, g, b, al = px[x, y]
            if al == 0:
                continue
            opaque += 1
            colours.add((r, g, b))
            alone = True
            for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):
                nx, ny = x + dx, y + dy
                if 0 <= nx < w and 0 <= ny < h:
                    r2, g2, b2, a2 = px[nx, ny]
                    if a2 and abs(r - r2) + abs(g - g2) + abs(b - b2) <= a.tolerance:
                        alone = False
                        break
            isolated += alone
    print(f"{path}: {opaque} px, {len(colours)} colours, {isolated} isolated ({100 * isolated / max(1, opaque):.1f}%)")
