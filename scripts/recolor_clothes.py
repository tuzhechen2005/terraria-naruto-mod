#!/usr/bin/env python3
"""Darken olive/green dark cloth pixels to near-black, keeping shade steps.

Usage: python3 scripts/recolor_clothes.py frame.png [frame.png ...]
Only dark, somewhat saturated yellow-green pixels change; skin, steel, eyes and
outlines are left alone.
"""

import colorsys
import sys

import numpy as np
from PIL import Image


def recolor(path: str) -> None:
    data = np.array(Image.open(path).convert("RGBA"))
    rgb = data[..., :3].astype(float) / 255
    changed = 0
    for y, x in zip(*np.nonzero(data[..., 3])):
        r, g, b = rgb[y, x]
        h, s, v = colorsys.rgb_to_hsv(r, g, b)
        if 40 / 360 <= h <= 120 / 360 and s > 0.08 and v < 0.5:
            level = v * 0.72
            data[y, x, :3] = [round(level * 255), round(level * 255), round(min(1, level * 1.06) * 255)]
            changed += 1
    Image.fromarray(data).save(path)
    print(f"{path}: {changed} px")


for arg in sys.argv[1:]:
    recolor(arg)
