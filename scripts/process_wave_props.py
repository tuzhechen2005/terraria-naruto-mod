#!/usr/bin/env python3
"""Ice mirror states, senbon and thrown blade from the v7 source sheets (keeps glass alpha)."""

import sys
from pathlib import Path

import numpy as np
from PIL import Image
from scipy import ndimage

out = Path(sys.argv[1] if len(sys.argv) > 1 else "props_out")
out.mkdir(parents=True, exist_ok=True)


def objects(path, merge):
    data = np.array(Image.open(path).convert("RGBA"))
    mask = ndimage.binary_dilation(data[..., 3] >= 64, iterations=merge)
    labels, _ = ndimage.label(mask)
    boxes = [b for b in ndimage.find_objects(labels) if b is not None]
    return data, sorted(boxes, key=lambda b: b[1].start)


def fit(crop, width, height):
    image = Image.fromarray(crop)
    image.thumbnail((width, height), Image.NEAREST)
    data = np.array(image)
    alpha = data[..., 3].astype(int)
    alpha = np.where(alpha < 64, 0, np.where(alpha > 220, 255, (alpha // 32) * 32 + 16))
    data[..., 3] = alpha
    canvas = np.zeros((height, width, 4), dtype=np.uint8)
    y, x = (height - data.shape[0]) // 2, (width - data.shape[1]) // 2
    canvas[y:y + data.shape[0], x:x + data.shape[1]] = data
    return Image.fromarray(canvas)


data, boxes = objects("art/deliveries/haku-v5/source/Haku_IceMirror_Senbon.png", 18)
mirrors, senbon = boxes[:3], boxes[3]
for i, box in enumerate(mirrors):
    fit(data[box], 48, 72).save(out / f"IceMirror_{i}.png")
fit(data[senbon], 32, 6).save(out / "HakuSenbon.png")
sword = np.array(Image.open(
    "art/deliveries/zabuza-v7-transition-frenzy/source/ZabuzaThrownSword.png").convert("RGBA"))
bbox = ndimage.find_objects((sword[..., 3] >= 64).astype(int))[0]
fit(sword[bbox], 88, 32).save(out / "ZabuzaThrownSword.png")
print("props ->", out)
