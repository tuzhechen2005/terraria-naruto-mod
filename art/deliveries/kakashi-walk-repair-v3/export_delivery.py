"""Export the imagegen walk sheet at a single fixed scale and build previews."""
from pathlib import Path
import json
import shutil
from PIL import Image, ImageDraw
import numpy as np

ROOT = Path(__file__).resolve().parent
OLD = ROOT.parent / "kakashi-direct-pixel-anim-v2" / "frames"
SHEET = ROOT / "source" / "generated_walk_sheet.png"
FRAMES = ROOT / "frames"
SCALE = 62 / 567
BASELINE = 75
XRUNS = [(71, 349), (445, 694), (782, 1049), (1147, 1388), (1493, 1767), (1857, 2117)]
NAMES = (["Kakashi_00_Idle"] + [f"Kakashi_{i:02d}_Walk" for i in range(1, 7)]
         + ["Kakashi_07_Jump", "Kakashi_08_Sit"]
         + [f"Kakashi_{i:02d}_Throw" for i in range(9, 12)]
         + ["Kakashi_Trapped_0", "Kakashi_Trapped_1"])


def harden(im):
    arr = np.asarray(im.convert("RGBA")).copy()
    arr[arr[:, :, 3] < 128] = (0, 0, 0, 0)
    arr[arr[:, :, 3] >= 128, 3] = 255
    return Image.fromarray(arr, "RGBA")


def green_center(im):
    a = np.asarray(im.convert("RGBA"))
    r, g, b, alpha = [a[:, :, i].astype(int) for i in range(4)]
    yy, xx = np.indices(alpha.shape)
    vest = (alpha >= 128) & (g > r * 1.08) & (g > b * 1.1) & (g > 55)
    vest &= (yy >= 250) & (yy <= 455)
    if not vest.any():
        raise ValueError("Vest center could not be located")
    return float(np.median(xx[vest]))


def paste_on_bg(im, bg, size=1):
    enlarged = im.resize((im.width * size, im.height * size), Image.Resampling.NEAREST)
    canvas = Image.new("RGBA", enlarged.size, bg)
    canvas.alpha_composite(enlarged)
    return canvas.convert("RGB")


def build():
    FRAMES.mkdir(exist_ok=True)
    source = Image.open(SHEET).convert("RGBA")
    assert source.size == (2172, 724)
    provenance = []
    for i, (left, right) in enumerate(XRUNS, 1):
        cell = source.crop((left, 0, right + 1, source.height))
        bbox = cell.getchannel("A").point(lambda a: 255 if a >= 128 else 0).getbbox()
        assert bbox and bbox[0] == 0 and bbox[2] == cell.width
        crop = cell.crop(bbox)
        target = crop.resize((round(crop.width * SCALE), round(crop.height * SCALE)), Image.Resampling.NEAREST)
        target = harden(target)
        center = green_center(cell) - bbox[0]
        center_at_scale = round(center * SCALE)
        dx = 40 - center_at_scale
        dy = BASELINE - (target.height - 1)
        frame = Image.new("RGBA", (80, 80))
        frame.paste(target, (dx, dy))
        frame = harden(frame)
        frame.save(FRAMES / f"Kakashi_{i:02d}_Walk.png")
        provenance.append({"frame": f"Kakashi_{i:02d}_Walk.png", "source_x": [left, right],
                           "source_bbox_in_cell": bbox, "scale": SCALE, "paste_xy": [dx, dy],
                           "vest_center_x": center})

    for name in NAMES:
        if "Walk" not in name:
            shutil.copyfile(OLD / f"{name}.png", FRAMES / f"{name}.png")

    frames = [Image.open(FRAMES / f"{name}.png").convert("RGBA") for name in NAMES]
    old_walk = [Image.open(OLD / f"Kakashi_{i:02d}_Walk.png").convert("RGBA") for i in range(1, 7)]

    # Four rows: 1x light, 3x light, 1x dark, 3x dark.
    strip = Image.new("RGB", (14 * 240, 640), "#252a32")
    draw = ImageDraw.Draw(strip)
    for row, bg in enumerate(("#d2d0c8", "#252a32")):
        for mag, top in ((1, row * 320), (3, row * 320 + 80)):
            draw.rectangle((0, top, strip.width - 1, top + 80 * mag - 1), fill=bg)
            for j, frame in enumerate(frames):
                panel = paste_on_bg(frame, bg, mag)
                strip.paste(panel, (j * 240, top))
    strip.save(ROOT / "strip_1x_3x.png")

    anim = []
    compare = []
    for before, after in zip(old_walk, frames[1:7]):
        small = paste_on_bg(after, "#d2d0c8")
        big = paste_on_bg(after, "#252a32", 3)
        canvas = Image.new("RGB", (320, 240), "#252a32")
        canvas.paste(small, (0, 80))
        canvas.paste(big, (80, 0))
        anim.append(canvas)
        side = Image.new("RGB", (480, 240), "#252a32")
        side.paste(paste_on_bg(before, "#252a32", 3), (0, 0))
        side.paste(big, (240, 0))
        compare.append(side)
    anim[0].save(ROOT / "walk.gif", save_all=True, append_images=anim[1:], duration=130, loop=0)
    compare[0].save(ROOT / "before_after_walk.gif", save_all=True,
                    append_images=compare[1:], duration=130, loop=0)

    waist = Image.new("RGB", (6 * 2 * 40 * 8, 38 * 8), "#252a32")
    for j, (before, after) in enumerate(zip(old_walk, frames[1:7])):
        for k, frame in enumerate((before, after)):
            crop = paste_on_bg(frame.crop((20, 30, 60, 68)), "#252a32", 8)
            waist.paste(crop, ((j * 2 + k) * 320, 0))
    waist.save(ROOT / "waist_8x.png")

    (ROOT / "manifest.json").write_text(json.dumps({"source": str(SHEET.relative_to(ROOT)),
        "canvas": [80, 80], "scale": SCALE, "baseline_y": BASELINE, "frames": provenance,
        "unchanged_source": str(OLD), "unchanged_frames": [n + ".png" for n in NAMES if "Walk" not in n]},
        indent=2, ensure_ascii=False) + "\n")


if __name__ == "__main__":
    build()
