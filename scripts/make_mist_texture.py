#!/usr/bin/env python3
"""Generate the seamless wispy mist texture for the Wave Country sea mist (SeaMistOverlay).

Periodic fractal noise, stretched horizontally so the mist reads as drifting banks rather than blobs, turned into
white with soft alpha. Seamless on both axes, so it can tile while it scrolls.
Usage: python3 scripts/make_mist_texture.py out.png
"""
import sys
import numpy as np
from PIL import Image

W, H = 1024, 256
rng = np.random.default_rng(7)


def periodic_noise(cells_x, cells_y):
    # Value noise on a wrapping lattice, smoothly interpolated, so the result tiles exactly.
    grid = rng.random((cells_y, cells_x))
    xs = np.linspace(0, cells_x, W, endpoint=False)
    ys = np.linspace(0, cells_y, H, endpoint=False)
    x0 = np.floor(xs).astype(int)
    y0 = np.floor(ys).astype(int)
    tx = xs - x0
    ty = ys - y0
    tx = tx * tx * (3 - 2 * tx)
    ty = ty * ty * (3 - 2 * ty)
    x1 = (x0 + 1) % cells_x
    y1 = (y0 + 1) % cells_y
    a = grid[np.ix_(y0, x0)]
    b = grid[np.ix_(y0, x1)]
    c = grid[np.ix_(y1, x0)]
    d = grid[np.ix_(y1, x1)]
    top = a + (b - a) * tx[None, :]
    bottom = c + (d - c) * tx[None, :]
    return top + (bottom - top) * ty[:, None]


noise = np.zeros((H, W))
amplitude = 1.0
total = 0.0
for octave in range(5):
    # Cells about three times wider than tall in texels: soft banks drawn out sideways.
    noise += amplitude * periodic_noise(5 * 2 ** octave, 4 * 2 ** octave)
    total += amplitude
    amplitude *= 0.5
noise /= total

# Drag the noise sideways (wrapping box blur along x) so banks trail off into wisps.
for _ in range(2):
    k = 8
    padded = np.concatenate([noise[:, -k:], noise, noise[:, :k]], axis=1)
    csum = np.cumsum(padded, axis=1)
    noise = (csum[:, 2 * k:] - csum[:, :-2 * k]) / (2 * k)
noise = (noise - noise.min()) / (noise.max() - noise.min())

# Soft threshold: clear gaps between banks, denser cores, feathered edges.
alpha = np.clip((noise - 0.3) / 0.5, 0, 1)
alpha = alpha * alpha * (3 - 2 * alpha)
rgba = np.zeros((H, W, 4), dtype=np.uint8)
rgba[..., :3] = 255
rgba[..., 3] = (alpha * 255).astype(np.uint8)
Image.fromarray(rgba).save(sys.argv[1])
print(sys.argv[1], "coverage", round(float(alpha.mean()), 3))
