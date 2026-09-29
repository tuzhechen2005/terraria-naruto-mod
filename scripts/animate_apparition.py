#!/usr/bin/env python3
"""Make a looping smoke animation from one good apparition frame.

Rows sway sideways with a travelling wave (stronger toward the top, where the smoke
rises), and the brightest pixels (eyes, fang tips) flicker. Keeps the source frame's
quality in every frame instead of relying on separately generated frames.

  python3 scripts/animate_apparition.py master.png out_dir --prefix Zabuza_Aura --frames 6
"""

import argparse
import math
from pathlib import Path

import numpy as np
from PIL import Image


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("master")
    parser.add_argument("out_dir")
    parser.add_argument("--prefix", default="Zabuza_Aura")
    parser.add_argument("--frames", type=int, default=6)
    parser.add_argument("--amplitude", type=float, default=2.0, help="max sway in art pixels")
    parser.add_argument("--burst-prefix", default="Zabuza_AuraBurst")
    parser.add_argument("--scale", type=int, default=2, help="screen pixels per art pixel")
    args = parser.parse_args()
    src = np.array(Image.open(args.master).convert("RGBA"))
    h, w = src.shape[:2]
    # Center the gap between the two lower chakra columns (where the body stands).
    lower = src[int(h * 0.75):, :, 3] > 0
    cols = lower.any(axis=0)
    xs = np.where(cols)[0]
    gap = [x for x in range(xs[0], xs[-1]) if not cols[x]]
    middle = (gap[0] + gap[-1]) / 2 if gap else (xs[0] + xs[-1]) / 2
    shift = int(round(w / 2 - 0.5 - middle))
    src = np.roll(src, shift, axis=1)
    if shift > 0:
        src[:, :shift] = 0
    elif shift < 0:
        src[:, shift:] = 0
    # Trim empty rows below so the column base sits on the canvas bottom (the feet).
    rows = np.where((src[..., 3] > 0).any(axis=1))[0]
    src = np.roll(src, h - 1 - rows[-1], axis=0)
    lum = src[..., :3].astype(float).mean(axis=2)
    bright = (src[..., 3] > 0) & (lum > np.percentile(lum[src[..., 3] > 0], 96))
    out = Path(args.out_dir)
    out.mkdir(parents=True, exist_ok=True)
    for f in range(args.frames):
        phase = 2 * math.pi * f / args.frames
        frame = np.zeros_like(src)
        for y in range(h):
            weight = 1.0 - 0.7 * (y / h)  # top sways most
            shift = round(args.amplitude * weight * math.sin(phase + y * 0.35))
            frame[y] = np.roll(src[y], shift, axis=0)
            if shift > 0:
                frame[y, :shift] = 0
            elif shift < 0:
                frame[y, shift:] = 0
        # Eyes and fang tips pulse between normal and extra-bright.
        glow = 0.5 + 0.5 * math.sin(phase * 2)
        mask = np.zeros((h, w), bool)
        for y in range(h):
            weight = 1.0 - 0.7 * (y / h)
            shift = round(args.amplitude * weight * math.sin(phase + y * 0.35))
            mask[y] = np.roll(bright[y], shift)
        boosted = frame[..., :3].astype(float)
        boosted[mask] = boosted[mask] + (255 - boosted[mask]) * 0.45 * glow
        frame[..., :3] = boosted.clip(0, 255).astype(np.uint8)
        save(frame, out / f"{args.prefix}_{f}.png", args.scale)
    # Emergence: chakra rises from the feet, then the face fills in.
    for k, reveal in enumerate((0.35, 0.7, 1.0)):
        frame = src.copy()
        cut = int(h * (1 - reveal))
        frame[:cut] = 0
        save(frame, out / f"{args.burst_prefix}_{k}.png", args.scale)
    print(f"{args.frames} loop + 3 burst frames -> {out} (shifted {shift} px to center)")


def save(frame, path, scale):
    image = Image.fromarray(frame)
    image.resize((image.width * scale, image.height * scale), Image.NEAREST).save(path)


if __name__ == "__main__":
    main()
