from pathlib import Path
from PIL import Image, ImageDraw
import math

ROOT = Path(__file__).resolve().parent
SOURCE = ROOT / "source"


def clean_crop(name):
    im = Image.open(SOURCE / name).convert("RGBA")
    alpha = im.getchannel("A")
    box = alpha.point(lambda a: 255 if a >= 128 else 0).getbbox()
    im = im.crop(box)
    pixels = im.load()
    for y in range(im.height):
        for x in range(im.width):
            r, g, b, a = pixels[x, y]
            pixels[x, y] = (r, g, b, 255 if a >= 128 else 0)
    return im


def framed_sprite(name, target, size, origin, variant=None):
    src = clean_crop(name).resize(size, Image.Resampling.NEAREST)
    if variant == "kakashi":
        # A small upward drift, with a one-pixel tip of hair and palm change.
        src = src.copy()
        d = ImageDraw.Draw(src)
        d.point((24, 0), fill=(226, 232, 247, 255))
    if variant == "zabuza":
        # One-pixel breathing/cloth change while keeping feet and palm fixed.
        src = src.copy()
        d = ImageDraw.Draw(src)
        d.point((59, 72), fill=(52, 55, 72, 255))
        d.point((60, 72), fill=(0, 0, 0, 0))
    frame = Image.new("RGBA", target, (0, 0, 0, 0))
    frame.alpha_composite(src, origin)
    return frame


for n in range(2):
    framed_sprite("kakashi_imagegen.png", (56, 56), (37, 48), (9, 4 - n),
                  "kakashi" if n else None).save(ROOT / f"kakashi_trapped_{n}.png")
    framed_sprite("zabuza_imagegen.png", (288, 128), (105, 105), (86, 19),
                  "zabuza" if n else None).save(ROOT / f"zabuza_hold_{n}.png")

raw = Image.open(SOURCE / "water_sphere_imagegen.png").convert("RGBA")
box = raw.getchannel("A").point(lambda a: 255 if a >= 50 else 0).getbbox()
raw = raw.crop(box).resize((88, 88), Image.Resampling.NEAREST)

for phase in range(3):
    frame = Image.new("RGBA", (96, 96), (0, 0, 0, 0))
    src = raw.load()
    out = frame.load()
    for y in range(88):
        for x in range(88):
            dx = x - 43.5
            dy = y - 43.5
            theta = math.atan2(dy, dx)
            radial = math.hypot(dx, dy)
            rim = 43.4 + 0.65 * math.sin(5 * theta + phase * 0.8)
            if radial > rim:
                continue
            r, g, b, source_a = src[x, y]
            if radial < 37:
                # The generated center was nearly transparent. Restore the
                # requested 35-45% translucent freshwater body.
                blend = min(source_a / 140, 1)
                r = int(123 * (1 - blend) + r * blend)
                g = int(214 * (1 - blend) + g * blend)
                b = int(239 * (1 - blend) + b * blend)
                alpha = max(90, min(112, 98 + source_a // 10))
                if source_a > 115:
                    alpha = min(194, source_a)
            else:
                alpha = max(145, min(235, 110 + source_a))
                if source_a < 30:
                    r, g, b = 92, 204, 235
            out[x + 4, y + 4] = (r, g, b, alpha)
    # Small animated bubble highlights within the sphere.
    d = ImageDraw.Draw(frame)
    for bx, by in [(30, 32 - phase), (67, 58 - phase), (55, 71 - phase)]:
        d.rectangle((bx, by, bx + 1, by + 1), fill=(233, 254, 255, 210))
        d.point((bx + 2, by + 2), fill=(49, 162, 213, 175))
    frame.save(ROOT / f"water_sphere_{phase}.png")


def scene(backdrop):
    im = Image.new("RGBA", (288, 160), backdrop)
    zabuza = Image.open(ROOT / "zabuza_hold_0.png").convert("RGBA")
    kakashi = Image.open(ROOT / "kakashi_trapped_0.png").convert("RGBA")
    sphere = Image.open(ROOT / "water_sphere_0.png").convert("RGBA")
    im.alpha_composite(zabuza, (0, 0))
    im.alpha_composite(kakashi, (210, 55))
    im.alpha_composite(sphere, (184, 35))
    return im


dark = scene((17, 44, 76, 255))
light = scene((206, 238, 247, 255))
preview = Image.new("RGBA", (576, 160), (0, 0, 0, 0))
preview.alpha_composite(dark, (0, 0))
preview.alpha_composite(light, (288, 0))
preview.save(ROOT / "preview_composite.png")
preview.resize((1152, 320), Image.Resampling.NEAREST).save(ROOT / "preview_composite_2x.png")
