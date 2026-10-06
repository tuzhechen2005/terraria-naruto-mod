#!/usr/bin/env python3
"""Re-export six RGBA sprites, review preview, and a short loop from source.png."""
from pathlib import Path
import json
from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parent
SOURCE = Image.open(ROOT / "source.png").convert("RGBA")
# Source image is 1536x1024. These cuts fall inside the transparent gutters.
ITEMS = [
    ("WaterDragonHead", (0, 0, 520, 530), (192, 128), (21, 64), "right", "danger core"),
    ("WaterDragonBody", (520, 0, 1080, 530), (256, 96), (128, 48), "right; joins at both ends", "danger core with decorative outer foam"),
    ("WaterGather", (1080, 0, 1536, 530), (192, 192), (96, 96), "clockwise gather", "telegraph; no damage"),
    ("WaterSlash", (0, 530, 520, 1024), (256, 192), (127, 99), "opens right", "decorative slash; character attack owns damage"),
    ("WaterSplash", (520, 530, 1080, 1024), (256, 128), (128, 112), "spreads left and right", "aftermath; no damage"),
    ("MistPuff", (1080, 530, 1536, 1024), (128, 128), (64, 64), "drifts right", "teleport warning; no damage"),
]


def clean_zero_rgb(im):
    px = im.load()
    for y in range(im.height):
        for x in range(im.width):
            if px[x, y][3] == 0:
                px[x, y] = (0, 0, 0, 0)
    return im


def composite(sprite, color):
    bg = Image.new("RGBA", sprite.size, color)
    bg.alpha_composite(sprite)
    return bg.convert("RGB")


manifest = {"source": "source.png", "source_dimensions": list(SOURCE.size), "sprites": []}
sprites = []
for name, cell, size, anchor, facing, role in ITEMS:
    raw = SOURCE.crop(cell)
    alpha = raw.getchannel("A")
    # A low threshold locates the generated art; the crop retains unmodified alpha.
    box = alpha.point(lambda value: 255 if value >= 12 else 0).getbbox()
    if box is None:
        raise ValueError(f"Missing sprite: {name}")
    padding = 3
    crop = (max(0, box[0] - padding), max(0, box[1] - padding),
            min(raw.width, box[2] + padding), min(raw.height, box[3] + padding))
    cut = raw.crop(crop)
    scale = min((size[0] - 12) / cut.width, (size[1] - 12) / cut.height)
    scaled = cut.resize((round(cut.width * scale), round(cut.height * scale)), Image.Resampling.LANCZOS)
    canvas = Image.new("RGBA", size, (0, 0, 0, 0))
    offset = ((size[0] - scaled.width) // 2, (size[1] - scaled.height) // 2)
    canvas.alpha_composite(scaled, offset)
    clean_zero_rgb(canvas).save(ROOT / "sprites" / f"{name}.png")
    sprites.append(canvas)
    manifest["sprites"].append({
        "file": f"sprites/{name}.png", "canvas": list(size), "source_cell": list(cell),
        "crop_in_cell": list(crop), "source_crop": [cell[0] + crop[0], cell[1] + crop[1], cell[0] + crop[2], cell[1] + crop[3]],
        "anchor": list(anchor), "direction": facing, "purpose": role,
    })

(ROOT / "manifest.json").write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + "\n")

# Review sheet: each sprite at 1x on light and dark grounds, with labels outside art.
preview = Image.new("RGB", (2 * 286, 3 * 236), "#253341")
draw = ImageDraw.Draw(preview)
for i, (item, sprite) in enumerate(zip(ITEMS, sprites)):
    name, _, _, _, _, _ = item
    col, row = i % 2, i // 2
    x, y = col * 286 + 15, row * 236 + 26
    for j, color in enumerate(("#132329", "#e6edf0")):
        thumb = sprite.copy()
        thumb.thumbnail((126, 180), Image.Resampling.LANCZOS)
        preview.paste(composite(thumb, color), (x + j * 134, y))
    draw.text((x, row * 236 + 6), name, fill="white")
preview.save(ROOT / "preview.png")

# Three displayed water components cycle gently in place; the source art is reused.
frames = []
for k in range(12):
    frame = Image.new("RGBA", (512, 256), (12, 27, 34, 255))
    for idx, loc in ((1, (12, 18)), (0, (284, 20)), (4, (128, 132))):
        art = sprites[idx].copy()
        art.putalpha(art.getchannel("A").point(lambda a: round(a * (0.72 + 0.25 * (1 - abs(k - 5.5) / 5.5)))))
        dx = (k % 6) - 3 if idx == 1 else 0
        frame.alpha_composite(art, (loc[0] + dx, loc[1]))
    frames.append(frame.convert("P", palette=Image.Palette.ADAPTIVE))
frames[0].save(ROOT / "water_loop.gif", save_all=True, append_images=frames[1:], duration=85, loop=0, disposal=2)
