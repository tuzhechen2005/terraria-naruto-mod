#!/usr/bin/env python3
"""Convert fine 1x boss frames into Terraria-native 2x2-pixel sprites.

All frames of one character share a palette so animation stays consistent:
  python3 scripts/terraria_native.py <out_dir> frame.png [frame.png ...]
  python3 scripts/terraria_native.py --native <out_dir> art.png ...   (input already 1 px per art pixel)

Per frame: 2x2 blocks take their dominant palette color (saturated accents such as
mask markings are weighted up so they survive), stray pixels are removed, the edge of
the silhouette gets a darker same-hue outline, then each art pixel is drawn 2x2.
"""

import colorsys
import sys
from pathlib import Path

import numpy as np
from PIL import Image

COLORS = 28


def adjust(rgb):
    """Terraria-like color: lift near-black to blue-gray, add a little saturation."""
    r, g, b = (c / 255 for c in rgb)
    h, s, v = colorsys.rgb_to_hsv(r, g, b)
    if v < 0.28:
        # Dark cloth: keep its steps but move them into a readable blue-gray ramp.
        v = 0.16 + v * 0.9
        if s < 0.25:
            h, s = 0.63, max(s, 0.22)
    s = min(1.0, s * 1.18)
    v = min(1.0, v * 1.06)
    return tuple(round(c * 255) for c in colorsys.hsv_to_rgb(h, s, v))


def build_palette(frames):
    pixels = np.concatenate([f[f[..., 3] > 0][:, :3] for f in frames])
    image = Image.fromarray(pixels.reshape(-1, 1, 3).astype(np.uint8), "RGB")
    quant = image.quantize(COLORS, method=Image.MEDIANCUT, dither=Image.NONE)
    flat = quant.getpalette()[:COLORS * 3]
    return np.array(flat, dtype=float).reshape(-1, 3)


def nearest(colors, palette):
    d = ((colors[:, None, :] - palette[None, :, :]) ** 2).sum(axis=2)
    return d.argmin(axis=1)


def convert(frame, palette, saturation, native=False):
    if native:
        # Already at art resolution: treat every pixel as a 2x2 block of itself.
        frame = np.repeat(np.repeat(frame, 2, axis=0), 2, axis=1)
    h, w = frame.shape[:2]
    h2, w2 = h // 2, w // 2
    alpha = frame[..., 3] > 0
    index = np.full((h, w), -1)
    index[alpha] = nearest(frame[alpha][:, :3].astype(float), palette)
    art = np.full((h2, w2), -1)
    for y in range(h2):
        for x in range(w2):
            block = index[y * 2:y * 2 + 2, x * 2:x * 2 + 2].ravel()
            block = block[block >= 0]
            if len(block) < 2:
                continue
            values, counts = np.unique(block, return_counts=True)
            score = counts + 1.6 * saturation[values] * (saturation[values] > 0.45)
            art[y, x] = values[score.argmax()]
    # Despeckle: drop lone pixels, fill single-pixel holes.
    filled = art >= 0
    padded = np.pad(filled, 1)
    neighbors = padded[:-2, 1:-1].astype(int) + padded[2:, 1:-1] + padded[1:-1, :-2] + padded[1:-1, 2:]
    art[filled & (neighbors <= 1)] = -1
    holes = (~filled) & (neighbors >= 4)
    for y, x in zip(*np.nonzero(holes)):
        around = [art[yy, xx] for yy, xx in ((y - 1, x), (y + 1, x), (y, x - 1), (y, x + 1))
                  if 0 <= yy < h2 and 0 <= xx < w2 and art[yy, xx] >= 0]
        if around:
            art[y, x] = max(set(around), key=around.count)
    filled = art >= 0
    padded = np.pad(filled, 1)
    edge = filled & ~(padded[:-2, 1:-1] & padded[2:, 1:-1] & padded[1:-1, :-2] & padded[1:-1, 2:])
    out = np.zeros((h2, w2, 4), dtype=np.uint8)
    colors = np.array([adjust(c) for c in palette.astype(int)], dtype=float)
    for y, x in zip(*np.nonzero(filled)):
        rgb = colors[art[y, x]]
        if edge[y, x]:
            hh, ss, vv = colorsys.rgb_to_hsv(*(rgb / 255))
            rgb = np.array(colorsys.hsv_to_rgb(hh, min(1, ss * 1.1 + 0.1), vv * 0.42)) * 255
        out[y, x, :3] = rgb
        out[y, x, 3] = 255
    return Image.fromarray(out).resize((w2 * 2, h2 * 2), Image.NEAREST)


def main():
    args = sys.argv[1:]
    native = "--native" in args
    args = [a for a in args if a != "--native"]
    out = Path(args[0])
    out.mkdir(parents=True, exist_ok=True)
    paths = [Path(p) for p in args[1:]]
    frames = [np.array(Image.open(p).convert("RGBA")) for p in paths]
    palette = build_palette(frames)
    saturation = np.array([colorsys.rgb_to_hsv(*(c / 255))[1] for c in palette])
    for path, frame in zip(paths, frames):
        convert(frame, palette, saturation, native).save(out / path.name)
    print(f"{len(paths)} frames -> {out}")


if __name__ == "__main__":
    main()
