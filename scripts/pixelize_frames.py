#!/usr/bin/env python3
"""Turn a generated pose atlas into fixed-canvas Terraria pixel frames.

The image model draws poses freely; this script does the deterministic part:
split poses, shrink them with one shared scale onto a 2x2 art-pixel grid,
harden alpha, limit the palette, and align feet and body centerline.

Example:
  python3 scripts/pixelize_frames.py atlas.png out/ --prefix Zabuza \
    --canvas 176x112 --center-x 88 --baseline 108 --body-height 84 \
    --order Idle:4,Run:6,Windup:3,Slash:3,Seal:3,Leap:2,Dash:2 --skip 4
"""

import argparse
from pathlib import Path

import numpy as np
from PIL import Image
from scipy import ndimage


def components(alpha: np.ndarray, min_area: int):
    mask = alpha >= 128
    # Close small gaps so a sword and its wielder stay one pose.
    mask = ndimage.binary_closing(mask, iterations=3)
    labels, count = ndimage.label(mask)
    boxes = ndimage.find_objects(labels)
    found = []
    for index, box in enumerate(boxes, start=1):
        area = int((labels[box] == index).sum())
        if area >= min_area:
            found.append((box, index))
    return labels, found


def reading_order(found):
    """Sort poses in rows (top to bottom), then left to right."""
    items = sorted(found, key=lambda f: (f[0][0].start + f[0][0].stop) / 2)
    rows, current, row_center = [], [], None
    for item in items:
        y0, y1 = item[0][0].start, item[0][0].stop
        center = (y0 + y1) / 2
        if row_center is not None and center - row_center > (y1 - y0) * 0.5:
            rows.append(current)
            current, row_center = [], None
        current.append(item)
        if row_center is None:
            row_center = center
    if current:
        rows.append(current)
    ordered = []
    for row in rows:
        ordered.extend(sorted(row, key=lambda f: f[0][1].start))
    return ordered


def pixelize(rgba: np.ndarray, scale: float, colors: int, pixel: int = 2) -> Image.Image:
    image = Image.fromarray(rgba, "RGBA")
    art_w = max(1, round(image.width * scale / pixel))
    art_h = max(1, round(image.height * scale / pixel))
    small = image.resize((art_w, art_h), Image.LANCZOS)
    data = np.array(small)
    opaque = data[..., 3] >= 128
    data[..., 3] = np.where(opaque, 255, 0)
    rgb = Image.fromarray(data[..., :3], "RGB").quantize(colors, method=Image.MEDIANCUT,
                                                          dither=Image.NONE).convert("RGB")
    data[..., :3] = np.array(rgb)
    data[~opaque] = 0
    small = Image.fromarray(data, "RGBA")
    return small.resize((art_w * pixel, art_h * pixel), Image.NEAREST)


def body_center_x(frame: np.ndarray) -> float:
    """Horizontal center of the lower body (legs), ignoring a long weapon."""
    alpha = frame[..., 3] > 0
    rows = np.where(alpha.any(axis=1))[0]
    lower = alpha[rows[0] + int((rows[-1] - rows[0]) * 0.6):rows[-1] + 1]
    cols = np.where(lower.any(axis=0))[0]
    return (cols[0] + cols[-1]) / 2


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("atlas")
    parser.add_argument("out_dir")
    parser.add_argument("--prefix", required=True)
    parser.add_argument("--canvas", required=True, help="WxH")
    parser.add_argument("--center-x", type=int, required=True)
    parser.add_argument("--baseline", type=int, required=True)
    parser.add_argument("--body-height", type=int, required=True,
                        help="target height (screen px) of the reference pose")
    parser.add_argument("--reference-pose", type=int, default=0,
                        help="index of an upright pose used to fix the shared scale")
    parser.add_argument("--reference-fraction", type=float, default=1.0,
                        help="fraction of the reference pose bbox that is the body")
    parser.add_argument("--order", required=True, help="Action:count,... in atlas order")
    parser.add_argument("--skip", default="", help="comma-separated pose indices to drop")
    parser.add_argument("--colors", type=int, default=24)
    parser.add_argument("--pixel", type=int, default=2, help="screen pixels per art pixel")
    parser.add_argument("--min-area", type=int, default=1500)
    parser.add_argument("--preview", default="")
    args = parser.parse_args()

    width, height = (int(v) for v in args.canvas.lower().split("x"))
    atlas = np.array(Image.open(args.atlas).convert("RGBA"))
    labels, found = components(atlas[..., 3], args.min_area)
    poses = reading_order(found)
    skip = {int(s) for s in args.skip.split(",") if s.strip()}
    poses = [p for i, p in enumerate(poses) if i not in skip]

    plan = []
    for part in args.order.split(","):
        action, count = part.split(":")
        plan.extend((action, n) for n in range(int(count)))
    if len(plan) != len(poses):
        raise SystemExit(f"Order expects {len(plan)} poses but found {len(poses)} after skipping.")

    ref_box = poses[args.reference_pose][0]
    ref_height = (ref_box[0].stop - ref_box[0].start) * args.reference_fraction
    scale = args.body_height / ref_height

    out = Path(args.out_dir)
    out.mkdir(parents=True, exist_ok=True)
    frames = []
    for (box, index), (action, n) in zip(poses, plan):
        crop = atlas[box].copy()
        crop[labels[box] != index] = 0
        frame = np.array(pixelize(crop, scale, args.colors, args.pixel))
        canvas = np.zeros((height, width, 4), dtype=np.uint8)
        center = body_center_x(frame)
        left = int(round((args.center_x - center) / args.pixel) * args.pixel)
        top = args.baseline - frame.shape[0]
        top -= top % args.pixel
        for y in range(frame.shape[0]):
            cy = top + y
            if not 0 <= cy < height:
                continue
            for x in range(frame.shape[1]):
                cx = left + x
                if 0 <= cx < width and frame[y, x, 3]:
                    canvas[cy, cx] = frame[y, x]
        clipped = frame.shape[0] > height or left < 0 or left + frame.shape[1] > width
        name = f"{args.prefix}_{action}_{n}.png"
        Image.fromarray(canvas, "RGBA").save(out / name)
        frames.append(canvas)
        print(f"{name}: art {frame.shape[1]}x{frame.shape[0]}{' CLIPPED' if clipped else ''}")

    if args.preview:
        columns = 6
        rows = (len(frames) + columns - 1) // columns
        sheet = Image.new("RGBA", (columns * width * 2, rows * height * 2 * 2), (0, 0, 0, 0))
        for i, frame in enumerate(frames):
            tile = Image.fromarray(frame, "RGBA").resize((width * 2, height * 2), Image.NEAREST)
            for band, color in enumerate(((40, 42, 54, 255), (214, 222, 230, 255))):
                x = (i % columns) * width * 2
                y = ((i // columns) * 2 + band) * height * 2
                sheet.paste(Image.new("RGBA", tile.size, color), (x, y))
                sheet.alpha_composite(tile, (x, y))
        sheet.save(args.preview)
    print(f"scale {scale:.4f}, {len(frames)} frames")


if __name__ == "__main__":
    main()
