from pathlib import Path
from PIL import Image, ImageDraw, ImageOps

ROOT = Path(__file__).resolve().parents[1]
BASE = Image.open(ROOT.parent / 'orochimaru-base-v5' / 'Orochimaru_Idle_0.png').convert('RGBA')
S = Image.Resampling.NEAREST
T = (0, 0, 0, 0)
INK = (20, 17, 30, 255)
FACE = (211, 207, 210, 255)
SHADE = (151, 143, 164, 255)
PURPLE = (120, 67, 136, 255)
SKIN = (244, 233, 213, 255)


def art(im):
    return im.resize((im.width // 2, im.height // 2), S)


def game(im):
    return im.resize((im.width * 2, im.height * 2), S)


def put(name, im):
    game(im).save(ROOT / f'{name}.png')


b = art(BASE)
d = ImageDraw.Draw(b)
# Remove the heavy chin L, then place a narrow asymmetric smile near the forward edge.
d.rectangle((36, 18, 39, 18), fill=SHADE)
d.rectangle((38, 17, 40, 17), fill=INK)
d.point((41, 16), fill=INK)

for i, phase in enumerate((0, 0, 1, 1, 0, -1)):
    f = b.copy()
    if i:
        # One art-pixel movement only at the hair end, belt knot and chest.
        f.paste(T, (18, 25, 24, 33))
        f.paste(b.crop((18, 25, 24, 33)), (18 + phase, 25), b.crop((18, 25, 24, 33)))
        if i in (2, 3, 5):
            f.putpixel((28, 25), PURPLE)
    put(f'Orochimaru_Idle_{i}', f)


def walk_frame(i):
    f = b.copy()
    d = ImageDraw.Draw(f)
    d.rectangle((16, 32, 41, 41), fill=T)
    left = b.crop((18, 31, 28, 42))
    right = b.crop((29, 31, 42, 42))
    offsets = ((0, 0), (-1, 0), (-2, -1), (-1, 0), (0, 0), (1, 0), (2, -1), (1, 0))
    a, c = offsets[i]
    f.alpha_composite(left, (18 + a, 31 + min(c, 0)))
    f.alpha_composite(right, (29 - a, 31 + max(c, 0)))
    return f


for i in range(8):
    put(f'Orochimaru_Walk_{i}', walk_frame(i))

h = b.copy()
top = b.crop((14, 5, 46, 29)).rotate(8, resample=S, expand=False)
ImageDraw.Draw(h).rectangle((14, 5, 46, 29), fill=T)
h.alpha_composite(top, (13, 4))
put('Orochimaru_Hurt_0', h)

# Transformation frames retain the base face and costume pixel language.
r0 = b.copy()
rd = ImageDraw.Draw(r0)
rd.polygon(((38, 8), (43, 10), (42, 16), (39, 19), (36, 18)), fill=(160, 124, 112, 255))
rd.line(((38, 8), (41, 10), (39, 15)), fill=INK, width=1)
rd.point((40, 13), fill=(231, 196, 160, 255))
put('Orochimaru_Reveal_0', r0)
r1 = b.copy()
rd = ImageDraw.Draw(r1)
rd.line(((40, 18), (44, 19), (46, 22)), fill=(141, 55, 103, 255), width=2)
rd.point((46, 22), fill=(188, 81, 129, 255))
put('Orochimaru_Reveal_1', r1)
put('Orochimaru_Reveal_2', b)


def rise(name, top, bottom, dest_y):
    f = Image.new('RGBA', b.size, T)
    part = b.crop((0, top, b.width, bottom))
    f.alpha_composite(part, (0, dest_y))
    if 'Emerge' in name:
        ed = ImageDraw.Draw(f)
        ed.polygon(((20, 41), (24, 39), (33, 40), (40, 39), (44, 41)), fill=INK)
        ed.line(((23, 40), (30, 41), (39, 40)), fill=SKIN, width=1)
    put(name, f)


rise('Orochimaru_Emerge_0', 6, 24, 24)
rise('Orochimaru_Emerge_1', 6, 34, 14)
put('Orochimaru_Emerge_2', b)
rise('Orochimaru_Sink_0', 6, 32, 16)
rise('Orochimaru_Sink_1', 6, 26, 22)
sink2 = Image.new('RGBA', b.size, T)
sink2.alpha_composite(b.crop((14, 6, 44, 20)), (14, 28))
sd = ImageDraw.Draw(sink2)
sd.line(((39, 39), (40, 36), (41, 37)), fill=SKIN, width=2)
sd.line(((39, 39), (40, 36), (41, 37)), fill=INK, width=1)
put('Orochimaru_Sink_2', sink2)

neck = Image.new('RGBA', (24, 20), T)
neck.alpha_composite(b.crop((20, 5, 44, 21)), (0, 2))
nd = ImageDraw.Draw(neck)
nd.polygon(((0, 10), (14, 11), (17, 19), (0, 19)), fill=T)
nd.polygon(((0, 13), (7, 8), (16, 6), (14, 13), (4, 17)), fill=INK)
nd.line(((2, 13), (9, 9), (14, 8)), fill=(40, 38, 59, 255), width=2)
nd.rectangle((19, 14, 22, 16), fill=INK)
nd.point((20, 15), fill=SKIN)
nd.point((22, 15), fill=SKIN)
put('Orochimaru_NeckHead', neck)

seg = Image.new('RGBA', (8, 8), T)
sd = ImageDraw.Draw(seg)
sd.rectangle((0, 2, 7, 6), fill=INK)
sd.rectangle((0, 3, 7, 5), fill=SHADE)
sd.line((0, 3, 7, 3), fill=SKIN)
sd.line((0, 4, 7, 4), fill=FACE)
put('Orochimaru_NeckSegment', seg)

head = Image.new('RGBA', (30, 30), T)
head.alpha_composite(BASE.crop((58, 12, 86, 38)), (1, 2))
head.save(ROOT / 'Orochimaru_Head_Boss.png')
