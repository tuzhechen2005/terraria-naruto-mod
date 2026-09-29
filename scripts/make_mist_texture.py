#!/usr/bin/env python3
"""Generate the seamless wisp texture for the Wave Country sea mist (SeaMistOverlay).

Thin horizontal strands of mist: noise that is long across and very short down, its rows bent by a slow wave so
each strand undulates, gathered into loose clusters with clear air between them. White with soft alpha, seamless
on both axes, high enough resolution to draw without magnification (so edges stay soft).
Usage: python3 scripts/make_mist_texture.py out.png
"""
import sys
import numpy as np
from PIL import Image

W, H = 2048, 512
rng = np.random.default_rng(11)
X, Y = np.meshgrid(np.arange(W, dtype=float), np.arange(H, dtype=float))


def noise(cells_x, cells_y, xs, ys):
    """Periodic value noise sampled at pixel coordinates (wrapping at W x H)."""
    grid = rng.random((cells_y, cells_x))
    fx = (xs % W) / W * cells_x
    fy = (ys % H) / H * cells_y
    x0 = np.floor(fx).astype(int) % cells_x
    y0 = np.floor(fy).astype(int) % cells_y
    tx = fx - np.floor(fx)
    ty = fy - np.floor(fy)
    tx = tx * tx * (3 - 2 * tx)
    ty = ty * ty * (3 - 2 * ty)
    x1 = (x0 + 1) % cells_x
    y1 = (y0 + 1) % cells_y
    top = grid[y0, x0] + (grid[y0, x1] - grid[y0, x0]) * tx
    bottom = grid[y1, x0] + (grid[y1, x1] - grid[y1, x0]) * tx
    return top + (bottom - top) * ty


def smoothstep(a, b, v):
    t = np.clip((v - a) / (b - a), 0, 1)
    return t * t * (3 - 2 * t)


# Slow vertical bending so strands undulate instead of running dead straight.
bend = (noise(5, 3, X, Y) - 0.5) * 60 + (noise(11, 5, X, Y) - 0.5) * 18
# Strands: few cells across, many down -> long thin horizontal streaks.
# (2026-09-29: strands a quarter as thick as the first strand version, which used 56 and 100 cells down.)
strands = 0.65 * noise(8, 224, X, Y + bend) + 0.35 * noise(18, 400, X, Y + bend * 1.3)
wisps = smoothstep(0.5, 0.86, strands)
# Loose clusters with clear air between them.
clusters = smoothstep(0.3, 0.8, noise(4, 3, X + 300, Y))
# Strands inside a faint haze where they gather, so the clumps read as mist rather than lines.
alpha = wisps * (0.1 + 0.9 * clusters) + 0.18 * clusters * clusters

# Soften along the strands (wrapping box blur across x).
for _ in range(2):
    k = 6
    padded = np.concatenate([alpha[:, -k:], alpha, alpha[:, :k]], axis=1)
    csum = np.cumsum(padded, axis=1)
    alpha = (csum[:, 2 * k:] - csum[:, :-2 * k]) / (2 * k)

# A touch of vertical softening so strand edges feather out.
alpha = (0.5 * np.roll(alpha, 1, axis=0) + 3 * alpha + 0.5 * np.roll(alpha, -1, axis=0)) / 4

rgba = np.zeros((H, W, 4), dtype=np.uint8)
rgba[..., :3] = 255
rgba[..., 3] = (np.clip(alpha, 0, 1) * 255).astype(np.uint8)
Image.fromarray(rgba).save(sys.argv[1])
print(sys.argv[1], "coverage", round(float(alpha.mean()), 3))
