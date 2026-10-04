"""Sample the generated pose strips into binary-alpha, 2x screen-pixel sprites."""

from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parent.parent
SOURCE = ROOT / "source"
FRAMES = ROOT / "frames"
FRAMES.mkdir(exist_ok=True)


def crop_subject(image, left, right):
    part = image.crop((left, 0, right, image.height))
    alpha = part.getchannel("A").point(lambda a: 255 if a >= 128 else 0)
    bbox = alpha.getbbox()
    return part.crop(bbox)


def sample(part, size, inner):
    aw, ah = size[0] // 2, size[1] // 2
    iw, ih = inner
    factor = min(iw / part.width, ih / part.height)
    small = part.resize((round(part.width * factor), round(part.height * factor)), Image.Resampling.BOX)
    # Palette reduction happens at art-pixel resolution, before the exact 2x enlargement.
    mask = small.getchannel("A").point(lambda a: 255 if a >= 115 else 0)
    rgb = small.convert("RGB").quantize(colors=22, method=Image.Quantize.MEDIANCUT).convert("RGB")
    small = rgb.convert("RGBA")
    small.putalpha(mask)
    canvas = Image.new("RGBA", (aw, ah))
    canvas.alpha_composite(small, ((aw - small.width) // 2, ah - small.height))
    return canvas.resize(size, Image.Resampling.NEAREST)


pakkun = Image.open(SOURCE / "generated_Pakkun_poses.png").convert("RGBA")
p_bounds = [(12, 265), (273, 588), (591, 874), (875, 1174),
            (1191, 1490), (1494, 1814), (1815, 2085)]
p_names = ["Pakkun_Idle", "Pakkun_Run_0", "Pakkun_Run_1", "Pakkun_Run_2",
           "Pakkun_Run_3", "Pakkun_Bite_0", "Pakkun_Bite_1"]
for name, (left, right) in zip(p_names, p_bounds):
    sample(crop_subject(pakkun, left, right), (40, 32), (19, 15)).save(FRAMES / f"{name}.png")

# The open bite mouth collapsed to dark pixels when the generated art was reduced.
bite = Image.open(FRAMES / "Pakkun_Bite_0.png").convert("RGBA")
bite_art = bite.resize((20, 16), Image.Resampling.NEAREST)
bd = ImageDraw.Draw(bite_art)
bd.point((17, 8), fill="#f0e7da")  # upper tooth
bd.point((18, 9), fill="#b66c71")  # mouth interior
bite_art.resize((40, 32), Image.Resampling.NEAREST).save(FRAMES / "Pakkun_Bite_0.png")

tora = Image.open(SOURCE / "generated_Tora_poses.png").convert("RGBA")
t_bounds = [(31, 320), (331, 710), (718, 1072), (1073, 1492),
            (1493, 1883), (1884, 2140)]
t_names = ["Tora_Idle", "Tora_Run_0", "Tora_Run_1", "Tora_Run_2",
           "Tora_Run_3", "Tora_Caught"]
for name, (left, right) in zip(t_names, t_bounds):
    part = crop_subject(tora, left, right)
    if name == "Tora_Caught":
        # The generated dangling pose was too slender at critter scale.
        part = part.resize((round(part.width * 1.65), part.height), Image.Resampling.NEAREST)
    sample(part, (36, 28), (17, 13)).save(FRAMES / f"{name}.png")

icon = Image.open(SOURCE / "generated_Pakkun_Icon.png").convert("RGBA")
sample(crop_subject(icon, 60, 1200), (28, 28), (14, 14)).save(FRAMES / "Pakkun_Icon.png")

# A compact contact sheet at native scale and 3x scale on both common ground values.
font = ImageFont.load_default()
rows = [("Pakkun", p_names, 40, 32), ("Tora", t_names, 36, 28),
        ("Icon", ["Pakkun_Icon"], 28, 28)]
preview = Image.new("RGB", (1600, 1040), "#233044")
draw = ImageDraw.Draw(preview)
for bg_index, bg in enumerate(("#233044", "#d4cbb5")):
    top = bg_index * 520
    draw.rectangle((0, top, 1599, top + 519), fill=bg)
    ink = "#f6efe4" if bg_index == 0 else "#263044"
    draw.text((12, top + 7), f"{ 'Dark' if bg_index == 0 else 'Light' } background | 1x", fill=ink, font=font)
    draw.text((560, top + 7), "3x", fill=ink, font=font)
    for row_index, (title, names, fw, fh) in enumerate(rows):
        y = top + 32 + row_index * 160
        draw.text((12, y + 5), title, fill=ink, font=font)
        for i, name in enumerate(names):
            sprite = Image.open(FRAMES / f"{name}.png").convert("RGBA")
            x = 80 + i * 60
            preview.paste(sprite, (x, y), sprite)
            preview.paste(sprite.resize((fw * 3, fh * 3), Image.Resampling.NEAREST),
                          (560 + i * 135, y - 3), sprite.resize((fw * 3, fh * 3), Image.Resampling.NEAREST))
    # 20 x 40 px player-sized proxy for relative scale, with a simple head and body.
    proxy = Image.new("RGBA", (20, 40))
    pd = ImageDraw.Draw(proxy)
    pd.rectangle((6, 1, 13, 11), fill="#b9906c", outline="#28232a", width=2)
    pd.rectangle((4, 12, 15, 27), fill="#506a89", outline="#28232a", width=2)
    pd.rectangle((5, 28, 8, 39), fill="#55483b", outline="#28232a", width=2)
    pd.rectangle((11, 28, 14, 39), fill="#55483b", outline="#28232a", width=2)
    preview.paste(proxy, (505, top + 43), proxy)
    preview.paste(proxy.resize((60, 120), Image.Resampling.NEAREST),
                  (1520, top + 47), proxy.resize((60, 120), Image.Resampling.NEAREST))
    draw.text((492, top + 85), "player", fill=ink, font=font)
preview.save(ROOT / "preview.png")
