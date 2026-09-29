#!/usr/bin/env python3
"""Clean delivered fixed-canvas frames: drop stray fragments, realign feet and centerline.

Usage: python3 scripts/clean_frames.py <in_dir> <out_dir> --glob 'Haku_*.png' \
           --center-x 56 --baseline 84 [--margin 6]
"""

import argparse
from pathlib import Path

import numpy as np
from PIL import Image
from scipy import ndimage


def clean(frame: np.ndarray, margin: int) -> np.ndarray:
    alpha = frame[..., 3] >= 128
    labels, count = ndimage.label(alpha)
    if count == 0:
        return frame
    sizes = ndimage.sum(alpha, labels, range(1, count + 1))
    main = labels == (int(np.argmax(sizes)) + 1)
    near = ndimage.binary_dilation(main, iterations=margin)
    keep = np.zeros_like(alpha)
    for index in range(1, count + 1):
        part = labels == index
        if (part & near).any():
            keep |= part
    out = frame.copy()
    out[~keep] = 0
    out[keep, 3] = 255
    return out


def realign(frame: np.ndarray, center_x: int, baseline: int) -> np.ndarray:
    alpha = frame[..., 3] > 0
    rows = np.where(alpha.any(axis=1))[0]
    lower = alpha[rows[0] + int((rows[-1] - rows[0]) * 0.6):rows[-1] + 1]
    cols = np.where(lower.any(axis=0))[0]
    dx = int(round(center_x - (cols[0] + cols[-1]) / 2))
    dy = baseline - rows[-1]
    return np.roll(np.roll(frame, dy, axis=0), dx, axis=1)


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("in_dir")
    parser.add_argument("out_dir")
    parser.add_argument("--glob", required=True)
    parser.add_argument("--center-x", type=int, required=True)
    parser.add_argument("--baseline", type=int, required=True)
    parser.add_argument("--margin", type=int, default=6)
    args = parser.parse_args()
    out = Path(args.out_dir)
    out.mkdir(parents=True, exist_ok=True)
    for path in sorted(Path(args.in_dir).glob(args.glob)):
        frame = np.array(Image.open(path).convert("RGBA"))
        frame = realign(clean(frame, args.margin), args.center_x, args.baseline)
        Image.fromarray(frame).save(out / path.name)
        print(path.name)


if __name__ == "__main__":
    main()
