from pathlib import Path
from PIL import Image, ImageDraw, ImageFont
import math

HERE = Path(__file__).resolve().parent.parent
BASE = Image.open(HERE.parent / 'orochimaru-base-v11' / 'Orochimaru_Idle_0.png').convert('RGBA')
W, H = BASE.size
PALE = (236, 221, 197, 255)
SHADOW = (161, 147, 134, 255)
OUTLINE = (36, 35, 44, 255)
PURPLE = (108, 73, 124, 255)
TONGUE = (148, 52, 110, 255)


def save(name, im):
    im.save(HERE / f'{name}.png')


def deform(top=0, waist=0, legs=(0, 0), lifts=(0, 0), chest=0, hair=0):
    out = Image.new('RGBA', (W, H))
    src = BASE.load()
    for y in range(28, 132):
        for x in range(80, 145):
            color = src[x, y]
            if not color[3]:
                continue
            if y <= 58:
                dx = top
            elif y <= 93:
                dx = round(top + (waist - top) * (y - 58) / 35)
                if chest and 56 <= y <= 70 and 100 <= x <= 116:
                    dx -= chest
            else:
                side = 0 if x < 114 else 1
                blend = min(1, (y - 93) / 17)
                dx = round(waist + legs[side] * blend)
            if hair and 58 <= y <= 89 and x < 106:
                dx -= hair
            dy = 0
            if y >= 115:
                side = 0 if x < 114 else 1
                dy = -lifts[side]
            tx, ty = x + dx, y + dy
            if 0 <= tx < W and 0 <= ty < H:
                out.putpixel((tx, ty), color)
    return out


# The selected v11 base is copied byte for byte as frame zero.
save('Orochimaru_Idle_0', BASE)
idle_settings = [
    (0, 0, 0), (1, 0, 0), (1, 1, 0), (0, 1, 1), (-1, 0, 1)
]
for i, (top, chest, hair) in enumerate(idle_settings, 1):
    save(f'Orochimaru_Idle_{i}', deform(top=top, chest=chest, hair=hair))

walk_settings = [
    (0, 0, (-2, 2), (0, 0), 0),
    (1, 0, (-4, 3), (0, 1), 1),
    (1, 1, (-5, 4), (0, 2), 2),
    (0, 1, (-2, 2), (0, 1), 1),
    (0, 0, (2, -2), (0, 0), 0),
    (-1, 0, (3, -4), (1, 0), 1),
    (-1, -1, (4, -5), (2, 0), 2),
    (0, -1, (2, -2), (1, 0), 1),
]
for i, (top, waist, legs, lifts, hair) in enumerate(walk_settings):
    frame = deform(top, waist, legs, lifts, hair=hair)
    if frame.getchannel('A').getbbox()[3] == 131:
        # The planted sole stays on the same game-world row through the cycle.
        for x in range(W):
            pixel = frame.getpixel((x, 130))
            if pixel[3]:
                frame.putpixel((x, 131), pixel)
    save(f'Orochimaru_Walk_{i}', frame)

save('Orochimaru_Hurt_0', deform(top=8, waist=2, legs=(-2, 2), hair=4))

reveal = BASE.copy()
d = ImageDraw.Draw(reveal)
# A thin flap of shed disguise curls away from the visible true face.
d.polygon([(124, 43), (129, 43), (136, 39), (139, 43), (135, 49), (129, 53), (125, 51)], fill=SHADOW, outline=OUTLINE)
d.line([(127, 44), (132, 45), (136, 42)], fill=PALE, width=1)
save('Orochimaru_Reveal_0', reveal)

reveal = BASE.copy()
d = ImageDraw.Draw(reveal)
d.line([(125, 50), (131, 51), (138, 54), (145, 54), (149, 51)], fill=TONGUE, width=2)
d.point((149, 50), fill=PURPLE)
save('Orochimaru_Reveal_1', reveal)
save('Orochimaru_Reveal_2', deform(top=1, chest=1))


def ground_clip(src, visible_height):
    # Move the unchanged original artwork down until only the requested height shows.
    bbox = src.getchannel('A').getbbox()
    shift = 132 - visible_height - bbox[1]
    out = Image.new('RGBA', (W, H))
    out.alpha_composite(src, (0, shift))
    clean = Image.new('RGBA', (W, H))
    clean.paste(out.crop((0, 0, W, 132)), (0, 0))
    return clean


for i, visible in enumerate((42, 69, 104)):
    frame = ground_clip(BASE, visible)
    if i < 2:
        d = ImageDraw.Draw(frame)
        d.line([(89, 131), (94, 129), (99, 130)], fill=SHADOW)
        d.line([(125, 130), (132, 129), (138, 131)], fill=SHADOW)
        d.point((94, 130), fill=PALE)
        d.point((132, 130), fill=PALE)
    save(f'Orochimaru_Emerge_{i}', frame)
for i, visible in enumerate((83, 63, 34)):
    save(f'Orochimaru_Sink_{i}', ground_clip(BASE, visible))

# Separate head based directly on v11's hair and face pixels.
neck = Image.new('RGBA', (72, 60))
d = ImageDraw.Draw(neck)
d.rectangle((0, 29, 42, 37), fill=OUTLINE)
d.rectangle((0, 30, 42, 36), fill=SHADOW)
d.rectangle((0, 31, 42, 34), fill=PALE)
d.line((0, 35, 42, 35), fill=SHADOW)
neck.alpha_composite(BASE.crop((98, 28, 132, 56)), (20, 8))
d = ImageDraw.Draw(neck)
# Fang and dark open jaw below the original expression; the eye pixels are untouched.
d.polygon([(46, 30), (50, 30), (54, 33), (52, 37), (47, 36)], fill=OUTLINE)
d.point((50, 32), fill=PALE)
d.point((52, 33), fill=PALE)
for pts in [[(25, 14), (14, 11), (6, 13)], [(25, 21), (13, 19), (4, 22)]]:
    d.line(pts, fill=OUTLINE, width=2)
save('Orochimaru_NeckHead', neck)

segment = Image.new('RGBA', (24, 24))
d = ImageDraw.Draw(segment)
d.rectangle((0, 7, 23, 16), fill=OUTLINE)
d.rectangle((0, 8, 23, 15), fill=SHADOW)
d.rectangle((0, 9, 23, 13), fill=PALE)
d.line((0, 14, 23, 14), fill=SHADOW)
save('Orochimaru_NeckSegment', segment)

portrait = Image.new('RGBA', (30, 30))
portrait.alpha_composite(BASE.crop((103, 28, 133, 58)), (0, 0))
save('Orochimaru_Head_Boss', portrait)

# Contact sheet: every frame next to the exact base at both requested scales.
names = [f'Orochimaru_Idle_{i}' for i in range(6)] + [f'Orochimaru_Walk_{i}' for i in range(8)] + ['Orochimaru_Hurt_0'] + [f'Orochimaru_Reveal_{i}' for i in range(3)] + [f'Orochimaru_Emerge_{i}' for i in range(3)] + [f'Orochimaru_Sink_{i}' for i in range(3)] + ['Orochimaru_NeckHead', 'Orochimaru_NeckSegment', 'Orochimaru_Head_Boss']
bg = (28, 29, 38, 255)
cw, ch = 224 + 224 * 3 + 48, 136 * 3 + 44
preview = Image.new('RGBA', (cw * 3, ch * math.ceil(len(names) / 3)), bg)
d = ImageDraw.Draw(preview)
for j, name in enumerate(names):
    col, row = j % 3, j // 3
    ox, oy = col * cw, row * ch
    im = Image.open(HERE / f'{name}.png').convert('RGBA')
    preview.alpha_composite(BASE, (ox, oy + 18))
    preview.alpha_composite(im, (ox + 232, oy + 18))
    preview.alpha_composite(im.resize((im.width * 3, im.height * 3), Image.Resampling.NEAREST), (ox + 456, oy + 18))
    d.text((ox + 5, oy + 3), f'v11 base | {name} | 3x', fill=(240, 240, 240, 255))
preview.save(HERE / 'preview.png')

for group, count in [('Idle', 6), ('Walk', 8)]:
    overlay = Image.new('RGBA', (W, H))
    for i in range(count):
        im = Image.open(HERE / f'Orochimaru_{group}_{i}.png').convert('RGBA')
        tinted = Image.new('RGBA', (W, H))
        color = [(235, 104, 96), (102, 193, 251), (155, 223, 121), (227, 192, 92)][i % 4]
        tinted.putdata([(*color, 70) if a else (0, 0, 0, 0) for a in im.getchannel('A').getdata()])
        overlay.alpha_composite(tinted)
    overlay.save(HERE / f'Orochimaru_{group}_overlay.png')
