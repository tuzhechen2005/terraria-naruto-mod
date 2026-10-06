"""Reproduce the M11 demon VFX exports from source/DemonVfxSheet.png."""

from pathlib import Path
from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parent
SOURCE = ROOT / "source" / "DemonVfxSheet.png"
SPRITES = ROOT / "sprites"
PREVIEWS = ROOT / "previews"
SPRITES.mkdir(exist_ok=True)
PREVIEWS.mkdir(exist_ok=True)

sheet = Image.open(SOURCE).convert("RGBA")
assert sheet.size == (1536, 1024)

# Source layout is a widely spaced 2x2 sheet. Press smoke slightly crosses
# the horizontal midpoint, so the split is at y=530; the lower split is x=800.
items = [
    ("DemonGhost", (0, 0, 768, 530), (256, 320), (240, 230), (128, 300)),
    ("DemonPress", (768, 0, 1536, 530), (256, 320), (240, 230), (128, 300)),
    ("SwordSpinArc", (0, 530, 800, 1024), (192, 192), (180, 156), (96, 96)),
    ("ChakraImpact", (800, 530, 1536, 1024), (192, 192), (176, 176), (96, 96)),
]


def clear_hidden_rgb(im):
    px = im.load()
    for y in range(im.height):
        for x in range(im.width):
            r, g, b, a = px[x, y]
            if a < 4:
                px[x, y] = (0, 0, 0, 0)
            elif a == 0:
                px[x, y] = (0, 0, 0, 0)
    return im


sprites = {}
for name, crop, canvas_size, max_size, anchor in items:
    q = sheet.crop(crop)
    mask = q.getchannel("A").point(lambda a: 255 if a >= 5 else 0)
    bbox = mask.getbbox()
    assert bbox is not None, name
    q = clear_hidden_rgb(q.crop(bbox))
    q.thumbnail(max_size, Image.Resampling.LANCZOS)
    canvas = Image.new("RGBA", canvas_size)
    if name.startswith("Demon"):
        # Shared visual baseline. The smoke tip fades just above y=300.
        left = anchor[0] - q.width // 2
        top = anchor[1] - q.height
    else:
        left = anchor[0] - q.width // 2
        top = anchor[1] - q.height // 2
    canvas.alpha_composite(q, (left, top))
    canvas = clear_hidden_rgb(canvas)
    canvas.save(SPRITES / f"{name}.png")
    sprites[name] = canvas


def checker(size, dark):
    base = (23, 23, 34) if dark else (224, 224, 224)
    alt = (38, 38, 53) if dark else (245, 245, 245)
    im = Image.new("RGBA", size, base)
    d = ImageDraw.Draw(im)
    for y in range(0, size[1], 16):
        for x in range(0, size[0], 16):
            if (x // 16 + y // 16) % 2:
                d.rectangle((x, y, x + 15, y + 15), fill=alt)
    return im


positions = [(32, 22), (320, 22), (65, 370), (352, 370)]
for dark in (False, True):
    preview = checker((608, 590), dark)
    for sprite, pos in zip(sprites.values(), positions):
        preview.alpha_composite(sprite, pos)
    preview.convert("RGB").save(PREVIEWS / ("at_1x_dark.png" if dark else "at_1x_light.png"))

# A short, illustrative loop only. Game timing and transforms remain up to
# the integrator. GIF is on a dark matte because GIF lacks graded alpha.
frames = []
for i in range(12):
    f = checker((608, 590), True)
    phase = i % 6
    ghost_name = "DemonGhost" if phase < 3 else "DemonPress"
    f.alpha_composite(sprites[ghost_name], positions[0])
    f.alpha_composite(sprites["DemonPress"], positions[1])
    arc = sprites["SwordSpinArc"].rotate(i * -9, Image.Resampling.BICUBIC)
    f.alpha_composite(arc, positions[2])
    ring = sprites["ChakraImpact"]
    scale = 0.75 + 0.025 * i
    ring = ring.resize((round(192 * scale), round(192 * scale)), Image.Resampling.LANCZOS)
    ring.putalpha(ring.getchannel("A").point(lambda a: round(a * (1 - 0.055 * i))))
    f.alpha_composite(ring, (positions[3][0] + (192 - ring.width) // 2,
                             positions[3][1] + (192 - ring.height) // 2))
    frames.append(f.convert("RGB"))
frames[0].save(PREVIEWS / "short_loop.gif", save_all=True, append_images=frames[1:],
               duration=100, loop=0, optimize=True)
