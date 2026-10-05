from pathlib import Path
from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parents[4]
OUT = Path(__file__).resolve().parents[1]
REF = ROOT / 'art/reference/terraria/armor'
CONCEPT = Image.open(OUT / 'source/genin_combat_concept.png').convert('RGBA')

NAVY = [(5, 8, 22), (12, 18, 42), (24, 36, 73), (43, 59, 104)]
LEATHER = [(17, 12, 14), (42, 26, 24), (75, 46, 35), (110, 73, 53)]
STEEL = [(20, 24, 36), (68, 75, 94), (154, 165, 181), (224, 230, 235)]
WHITE = [(35, 39, 54), (95, 103, 123), (182, 190, 202), (242, 244, 242)]


def shade(px):
    v = (px[0] * 3 + px[1] * 5 + px[2] * 2) // 10
    return 0 if v < 36 else 1 if v < 75 else 2 if v < 130 else 3


def recolor(src, palette):
    dst = Image.new('RGBA', src.size)
    for y in range(0, src.height, 2):
        for x in range(0, src.width, 2):
            p = src.getpixel((x, y))
            if p[3] == 0:
                continue
            color = palette[shade(p)] + (255,)
            for yy in range(y, min(y + 2, src.height)):
                for xx in range(x, min(x + 2, src.width)):
                    if src.getpixel((xx, yy))[3]:
                        dst.putpixel((xx, yy), color)
    return dst


def clipped_block(im, mask, box, color):
    x0, y0, x1, y1 = box
    for y in range(y0, y1):
        for x in range(x0, x1):
            if 0 <= x < im.width and 0 <= y < im.height and mask.getpixel((x, y)):
                im.putpixel((x, y), color + (255,))


head = Image.new('RGBA', (40, 1120))
for frame in range(20):
    tile = Image.open(REF / 'Armor_Head_22.png').convert('RGBA').crop((0, frame * 56, 40, min((frame + 1) * 56, 1118)))
    if tile.height < 56:
        padded = Image.new('RGBA', (40, 56))
        padded.alpha_composite(tile)
        tile = padded
    dest = recolor(tile, NAVY)
    mask = tile.getchannel('A')
    b = mask.getbbox()
    if b:
        x, y = b[0], b[1]
        # Bright forehead plate on the face-facing side, two-pixel pixel-art grid.
        clipped_block(dest, mask, (x + 2, y + 6, x + 12, y + 12), STEEL[0])
        clipped_block(dest, mask, (x + 4, y + 6, x + 10, y + 10), STEEL[3])
        clipped_block(dest, mask, (x + 6, y + 8, x + 8, y + 10), STEEL[0])
        clipped_block(dest, mask, (x + 10, y + 8, x + 12, y + 10), STEEL[1])
    head.alpha_composite(dest, (0, frame * 56))
head.save(OUT / 'GeninCombatHead_Head.png')


body_template = Image.open(REF / 'Armor_14.png').convert('RGBA')
body = recolor(body_template, NAVY)
body_mask = body_template.getchannel('A')
for row in (0, 2):
    for col in (0, 1):
        region = (col * 40, row * 56, col * 40 + 40, row * 56 + 56)
        b = body_mask.crop(region).getbbox()
        if not b:
            continue
        left, top = col * 40 + b[0], row * 56 + b[1]
        # One diagonal strap, then two tiny silver kunai hilts.
        for step in range(5):
            clipped_block(body, body_mask, (left + 4 + step * 2, top + step * 2,
                                             left + 8 + step * 2, top + step * 2 + 2), LEATHER[2])
        clipped_block(body, body_mask, (left + 8, top + 4, left + 10, top + 8), STEEL[3])
        clipped_block(body, body_mask, (left + 12, top + 6, left + 14, top + 10), STEEL[2])
body.save(OUT / 'GeninCombatBody_Body.png')


legs_template = Image.open(REF / 'Armor_Legs_14.png').convert('RGBA')
legs = Image.new('RGBA', (40, 1120))
for frame in range(20):
    tile = legs_template.crop((0, frame * 56, 40, (frame + 1) * 56))
    dest = recolor(tile, NAVY)
    mask = tile.getchannel('A')
    b = mask.getbbox()
    if b:
        x0, y0, x1, y1 = b
        # Extend the frame-local trouser shafts above the original sandal mask.
        # The original foot position remains untouched and determines the pose.
        trouser_top = max(30, y0 - 10)
        d = ImageDraw.Draw(dest)
        for sx0, sx1 in ((x0 + 2, x0 + 8), (x1 - 8, x1 - 2)):
            d.rectangle((sx0, trouser_top, sx1 - 1, y0 + 3), fill=NAVY[0] + (255,))
            d.rectangle((sx0 + 2, trouser_top + 2, sx1 - 2, y0 - 1), fill=NAVY[2] + (255,))
            d.rectangle((sx0, y0 - 2, sx1 - 1, y0 + 1), fill=WHITE[3] + (255,))
            d.rectangle((sx0, y0 + 2, sx1 - 1, y0 + 3), fill=WHITE[2] + (255,))
        d.rectangle((x1 - 8, trouser_top + 2, x1 - 3, trouser_top + 7), fill=LEATHER[1] + (255,))
        d.rectangle((x1 - 6, trouser_top + 2, x1 - 3, trouser_top + 5), fill=LEATHER[2] + (255,))
        h = y1 - y0
        wrap_y = y0 + max(4, h // 2)
        clipped_block(dest, mask, (x0, wrap_y, x1, min(wrap_y + 4, y1)), WHITE[3])
        clipped_block(dest, mask, (x0, wrap_y + 2, x1, min(wrap_y + 4, y1)), WHITE[2])
        # Thigh-side pouch, kept within the original sprite silhouette.
        clipped_block(dest, mask, (x1 - 6, y0 + 2, x1 - 2, min(y0 + 8, y1)), LEATHER[2])
        clipped_block(dest, mask, (x1 - 6, y0 + 2, x1 - 2, min(y0 + 4, y1)), LEATHER[3])
    legs.alpha_composite(dest, (0, frame * 56))
legs.save(OUT / 'GeninCombatLegs_Legs.png')


def icon(box, size, filename):
    crop = CONCEPT.crop(box)
    alpha = crop.getchannel('A')
    # Discard near-transparent specks before finding the actual item silhouette.
    hard = alpha.point(lambda a: 255 if a >= 160 else 0)
    b = hard.getbbox()
    if b:
        crop = crop.crop(b)
    w, h = crop.size
    target_w, target_h = size
    scale = min(target_w / w, target_h / h)
    small = crop.resize((max(1, round(w * scale)), max(1, round(h * scale))), Image.Resampling.BOX)
    dst = Image.new('RGBA', size)
    palette = NAVY + LEATHER + STEEL + WHITE + [(219, 142, 95), (252, 181, 124)]
    for y in range(small.height):
        for x in range(small.width):
            r, g, b, a = small.getpixel((x, y))
            if a < 128:
                continue
            color = min(palette, key=lambda p: (p[0] - r) ** 2 + (p[1] - g) ** 2 + (p[2] - b) ** 2)
            dst.putpixel(((target_w - small.width) // 2 + x, (target_h - small.height) // 2 + y), color + (255,))
    dst.save(OUT / filename)


icon((470, 390, 945, 610), (28, 20), 'GeninCombatHead.png')
icon((940, 280, 1385, 705), (30, 30), 'GeninCombatBody.png')
icon((1400, 280, 1774, 805), (28, 30), 'GeninCombatLegs.png')


# Game-scale compositing check: standing and a walking head/leg frame, plus icons
# at their actual size and at three times nearest-neighbor magnification.
preview = Image.new('RGBA', (470, 255), (53, 58, 67, 255))
def player(frame, x, y):
    layer = Image.new('RGBA', (40, 56))
    d = ImageDraw.Draw(layer)
    d.rectangle((12, 14, 24, 32), fill=(223, 146, 104, 255))
    d.rectangle((10, 10, 24, 16), fill=(24, 19, 24, 255))
    d.rectangle((12, 31, 31, 53), fill=NAVY[1] + (255,))
    layer.alpha_composite(body.crop((0, 0, 40, 56)))
    layer.alpha_composite(legs.crop((0, frame * 56, 40, (frame + 1) * 56)))
    layer.alpha_composite(head.crop((0, frame * 56, 40, (frame + 1) * 56)))
    preview.alpha_composite(layer.resize((120, 168), Image.Resampling.NEAREST), (x, y))

player(0, 12, 20)
player(5, 145, 20)
names = ['GeninCombatHead.png', 'GeninCombatBody.png', 'GeninCombatLegs.png']
for i, name in enumerate(names):
    im = Image.open(OUT / name).convert('RGBA')
    preview.alpha_composite(im, (280 + i * 50, 18))
    preview.alpha_composite(im.resize((im.width * 3, im.height * 3), Image.Resampling.NEAREST), (280 + (i % 2) * 100, 55 + (i // 2) * 105))
preview.save(OUT / 'preview.png')
