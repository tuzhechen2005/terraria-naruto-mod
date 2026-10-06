from pathlib import Path
from PIL import Image, ImageDraw, ImageOps

ROOT = Path(__file__).resolve().parents[3]
OUT = Path(__file__).resolve().parents[1]
REF = ROOT / 'reference' / 'terraria' / 'armor'

IVORY = [(46, 38, 36), (150, 139, 118), (215, 203, 172), (244, 231, 199)]
GREEN = [(44, 44, 40), (95, 106, 88), (145, 157, 126), (192, 198, 164)]
BROWN = [(43, 30, 27), (84, 53, 43), (126, 83, 62), (163, 119, 87)]
BLUE = [(35, 37, 46), (62, 69, 84), (91, 101, 120), (126, 135, 151)]
STRAW = [(52, 37, 29), (122, 82, 50), (177, 128, 75), (214, 170, 105)]


def level(rgb):
    v = (rgb[0] + rgb[1] + rgb[2]) / 3
    return 0 if v < 55 else 1 if v < 125 else 2 if v < 195 else 3


def recolor(name, target, kind):
    src = Image.open(REF / name).convert('RGBA')
    if kind == 'head':
        if src.height == 1118:
            padded = Image.new('RGBA', (40, 1120))
            padded.paste(src, (0, 0))
            src = padded
    dst = Image.new('RGBA', src.size)
    sp, dp = src.load(), dst.load()
    for y in range(src.height):
        for x in range(src.width):
            r, g, b, a = sp[x, y]
            if not a:
                continue
            shade = level((r, g, b))
            if kind == 'head':
                palette = IVORY
                # The original ninja headwear retains the tied-band silhouette.
                if (x >= 24 and (y % 56) >= 22) or (x < 12 and (y % 56) >= 22):
                    palette = BROWN if shade == 0 else IVORY
            elif kind == 'body':
                ly = y % 56
                palette = GREEN
                if x % 360 < 80 and 38 <= ly <= 42:
                    palette = BROWN
                elif x >= 80 and ly >= 32 and (x // 40) % 3 != 0:
                    palette = IVORY
            else:
                frame_top = (y // 56) * 56
                frame = src.crop((0, frame_top, 40, frame_top + 56))
                box = frame.getbbox()
                rel = (y - frame_top - box[1]) / max(1, box[3] - box[1]) if box else 0
                palette = BLUE if rel < 0.50 else IVORY if rel < 0.79 else STRAW
            dp[x, y] = (*palette[shade], 255)
    dst.save(OUT / target)
    return dst


head = recolor('Armor_Head_22.png', 'AcademyTrainingHead_Head.png', 'head')
body = recolor('Armor_1.png', 'AcademyTrainingBody_Body.png', 'body')
legs = recolor('Armor_Legs_1.png', 'AcademyTrainingLegs_Legs.png', 'legs')

source = Image.open(OUT / 'source' / 'AcademyTraining_generated.png').convert('RGBA')
icon_specs = [
    ('AcademyTrainingHead.png', (475, 378, 790, 580), IVORY, (30, 22)),
    ('AcademyTrainingBody.png', (810, 378, 1180, 800), GREEN, (30, 30)),
    ('AcademyTrainingLegs.png', (1185, 382, 1525, 980), BLUE, (24, 32)),
]
icons = []
for filename, box, dominant, size in icon_specs:
    crop = source.crop(box)
    # Keep the central item silhouette and discard antialias fringe.
    alpha = crop.getchannel('A').point(lambda a: 255 if a >= 180 else 0)
    crop.putalpha(alpha)
    bbox = crop.getbbox()
    if bbox:
        crop = crop.crop(bbox)
    crop.thumbnail(size, Image.Resampling.NEAREST)
    icon = Image.new('RGBA', (32, 32))
    icon.alpha_composite(crop, ((32 - crop.width) // 2, (32 - crop.height) // 2))
    pal = IVORY + GREEN + BROWN + BLUE + STRAW
    pix = icon.load()
    for y in range(32):
        for x in range(32):
            r, g, b, a = pix[x, y]
            if not a:
                continue
            # Color quantization removes smooth source pixels while keeping its form.
            q = min(pal, key=lambda c: (r-c[0])**2 + (g-c[1])**2 + (b-c[2])**2)
            pix[x, y] = (*q, 255)
    icon.save(OUT / filename)
    icons.append(icon)


def avatar(frame):
    image = Image.new('RGBA', (40, 56))
    d = ImageDraw.Draw(image)
    # Neutral player face and hair beneath the equipment for a registration preview.
    d.rectangle((12, 11, 29, 28), fill=(49, 31, 25))
    d.rectangle((14, 17, 27, 28), fill=(216, 152, 112))
    d.rectangle((18, 21, 19, 22), fill=(35, 30, 30))
    d.rectangle((25, 21, 26, 22), fill=(35, 30, 30))
    image.alpha_composite(legs.crop((0, frame*56, 40, (frame+1)*56)))
    image.alpha_composite(body.crop((0, 0, 40, 56)))
    image.alpha_composite(head.crop((0, frame*56, 40, (frame+1)*56)))
    return image


preview = Image.new('RGBA', (552, 210), (42, 44, 48, 255))
d = ImageDraw.Draw(preview)
d.text((16, 12), 'STAND', fill=(238, 231, 208))
d.text((120, 12), 'WALK', fill=(238, 231, 208))
preview.alpha_composite(avatar(0).resize((120, 168), Image.Resampling.NEAREST), (4, 30))
preview.alpha_composite(avatar(6).resize((120, 168), Image.Resampling.NEAREST), (110, 30))
d.text((242, 12), 'ICONS 1x', fill=(238, 231, 208))
d.text((242, 65), 'ICONS 3x', fill=(238, 231, 208))
for i, icon in enumerate(icons):
    preview.alpha_composite(icon, (240 + i*37, 27))
    preview.alpha_composite(icon.resize((96, 96), Image.Resampling.NEAREST), (240 + i*100, 77))
preview.save(OUT / 'preview.png')
