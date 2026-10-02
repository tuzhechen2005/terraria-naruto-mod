from pathlib import Path
from PIL import Image, ImageDraw
import numpy as np

ROOT = Path(__file__).parent
SOURCE = ROOT / 'source'
FRAME = (112, 88)
GOLD = (239, 197, 60, 255)
PURPLE = (133, 54, 158, 255)
DARK = (25, 18, 32, 255)

# Source pose sheets are cropped at their clear panel gaps. Positions are in
# half-resolution art pixels, then enlarged by nearest-neighbor to the game grid.
POSES = [
    ('Hands', 0, (0, 0, 628, 805), 31, 16),
    ('Hands', 1, (628, 0, 1460, 805), 31, 13),
    ('Hands', 2, (1460, 0, 1954, 805), 31, 17),
    ('Dash', 0, (0, 0, 1050, 724), 22, 7),
    ('Dash', 1, (1050, 0, 2172, 724), 19, 7),
    ('Wind', 0, (0, 0, 850, 887), 31, 16),
    ('Wind', 1, (850, 0, 1774, 887), 31, 15),
    ('Seal', 0, (0, 0, 610, 1024), 31, 17),
    ('Seal', 1, (610, 0, 1536, 1024), 31, 11),
    ('Summon', 0, (0, 0, 870, 852), 31, 16),
    ('Summon', 1, (870, 0, 1846, 852), 24, 10),
    ('Neck', 0, (0, 0, 710, 887), 31, 15),
]

EYES = {
    ('Hands', 0): (26, 15), ('Hands', 1): (26, 15),
    ('Hands', 2): (27, 15), ('Dash', 0): (41, 24),
    ('Dash', 1): (48, 27), ('Wind', 0): (28, 14),
    ('Wind', 1): (37, 16), ('Seal', 0): (27, 15),
    ('Seal', 1): (29, 16), ('Summon', 0): (32, 16),
    ('Summon', 1): (33, 23),
}

def prepared(name, region):
    im = Image.open(SOURCE / f'{name}_generated.png').convert('RGBA').crop(region)
    a = np.asarray(im).copy()
    alpha = a[:, :, 3]
    if name == 'Seal':
        # The generated violet lighting has an alpha-bearing dark wash. Keep
        # the figure and bright fingertip flames, discard that wash.
        light = np.max(a[:, :, :3], axis=2)
        keep = (alpha >= 220) & (light >= 32)
    else:
        keep = alpha >= 128
    a[:, :, 3] = np.where(keep, 255, 0).astype(np.uint8)
    a[:, :, :3] *= keep[:, :, None]
    im = Image.fromarray(a, 'RGBA')
    box = im.getbbox()
    if not box:
        raise ValueError(f'empty source {name} {region}')
    return im.crop(box)

def drop_noise(art):
    array = np.asarray(art).copy()
    active = array[:, :, 3] > 0
    seen = np.zeros(active.shape, dtype=bool)
    for sy in range(44):
        for sx in range(56):
            if not active[sy, sx] or seen[sy, sx]:
                continue
            stack = [(sx, sy)]
            seen[sy, sx] = True
            part = []
            while stack:
                xx, yy = stack.pop()
                part.append((xx, yy))
                for dy in (-1, 0, 1):
                    for dx in (-1, 0, 1):
                        nx, ny = xx+dx, yy+dy
                        if 0 <= nx < 56 and 0 <= ny < 44 and active[ny, nx] and not seen[ny, nx]:
                            seen[ny, nx] = True
                            stack.append((nx, ny))
            if len(part) < 9:
                for xx, yy in part:
                    array[yy, xx] = (0, 0, 0, 0)
    return Image.fromarray(array, 'RGBA')

def art_sprite(im, height, x, bottom=42, eye=None, summon_ground=False, keep_flames=False):
    width = max(1, round(im.width * height / im.height))
    if x + width > 56:
        width = 56 - x
    mini = im.resize((width, height), Image.Resampling.BOX)
    a = np.asarray(mini).copy()
    a[:, :, 3] = np.where(a[:, :, 3] >= 128, 255, 0).astype(np.uint8)
    a[:, :, :3] *= (a[:, :, 3] > 0)[:, :, None]
    mini = Image.fromarray(a, 'RGBA')
    art = Image.new('RGBA', (56, 44))
    art.alpha_composite(mini, (x, bottom-height))
    if not keep_flames:
        art = drop_noise(art)
    if eye:
        ex, ey = eye
        # One gold slit eye and a purple lid survive the 2x2 final grid.
        art.putpixel((ex-1, ey), PURPLE)
        art.putpixel((ex, ey), GOLD)
        art.putpixel((ex+1, ey), DARK)
    if summon_ground:
        draw = ImageDraw.Draw(art)
        draw.arc((24, 37, 43, 41), 0, 359, fill=DARK, width=1)
        draw.line((29, 39, 39, 39), fill=DARK, width=1)
        draw.line((34, 37, 34, 41), fill=DARK, width=1)
    return art.resize(FRAME, Image.Resampling.NEAREST)

for name, n, region, height, x in POSES:
    sprite = art_sprite(prepared(name, region), height, x,
                        eye=EYES.get((name, n)),
                        summon_ground=(name == 'Summon' and n == 1),
                        keep_flames=(name == 'Seal'))
    sprite.save(ROOT / f'Orochimaru_{name}_{n}.png')

# Detachable neck components have their own native canvas sizes.
head = prepared('Neck', (710, 0, 1360, 680))
head_art = head.resize((23, max(1, round(head.height * 23 / head.width))), Image.Resampling.BOX)
head_art.thumbnail((24, 20), Image.Resampling.BOX)
head_canvas = Image.new('RGBA', (24, 20))
head_canvas.alpha_composite(head_art, (0, (20-head_art.height)//2))
head_canvas.putpixel((18, 7), PURPLE)
head_canvas.putpixel((19, 7), GOLD)
head_canvas.putpixel((20, 7), DARK)
head_canvas = head_canvas.resize((48, 40), Image.Resampling.NEAREST)
head_canvas.putalpha(head_canvas.getchannel('A').point(lambda x: 255 if x >= 128 else 0))
head_canvas.save(ROOT / 'Orochimaru_NeckHead.png')

# Rebuild the central shaft sampled from the generated neck. Matching left
# and right terminal columns make repeated segments join without cap seams.
neck = prepared('Neck', (1360, 300, 1774, 600))
neck_art = neck.resize((8, max(1, round(neck.height * 8 / neck.width))), Image.Resampling.BOX)
segment = Image.new('RGBA', (8, 8))
d = ImageDraw.Draw(segment)
d.rectangle((0, 1, 7, 6), fill=DARK)
d.rectangle((0, 2, 7, 5), fill=(226, 194, 166, 255))
d.line((0, 2, 7, 2), fill=(249, 223, 193, 255))
d.line((0, 5, 7, 5), fill=(173, 137, 121, 255))
segment = segment.resize((16, 16), Image.Resampling.NEAREST)
segment.putalpha(segment.getchannel('A').point(lambda x: 255 if x >= 128 else 0))
segment.save(ROOT / 'Orochimaru_NeckSegment.png')

# Review board: 1x and 4x rows for each action, plus the accepted Idle sample.
idle = Image.open(ROOT.parent / 'orochimaru-style-v3' / 'Orochimaru_Idle.png').convert('RGBA')
groups = [('Hands', 3), ('Dash', 2), ('Wind', 2), ('Seal', 2), ('Summon', 2), ('Neck', 1)]
cell_w, row_h = 460, 480
board = Image.new('RGB', (cell_w * 4, row_h * len(groups) + 230), '#202936')
draw = ImageDraw.Draw(board)
for row, (name, count) in enumerate(groups):
    y = row * row_h
    draw.text((8, y+4), name, fill='white')
    for j in range(count):
        p = Image.open(ROOT / f'Orochimaru_{name}_{j}.png').convert('RGBA')
        board.paste(p, (j*cell_w+8, y+25), p)
        four = p.resize((448, 352), Image.Resampling.NEAREST)
        board.paste(four, (j*cell_w+8, y+116), four)
    board.paste(idle, (cell_w*3+8, y+25), idle)
    draw.text((cell_w*3+8, y+4), 'v3 Idle', fill='#ffe793')
head = Image.open(ROOT / 'Orochimaru_NeckHead.png').convert('RGBA')
segment = Image.open(ROOT / 'Orochimaru_NeckSegment.png').convert('RGBA')
y = row_h * len(groups)
draw.text((8, y+4), 'Neck parts: head / repeat segment', fill='white')
board.paste(head, (8, y+27), head)
board.paste(segment, (90, y+27), segment)
board.paste(head.resize((192,160), Image.Resampling.NEAREST), (8, y+68), head.resize((192,160), Image.Resampling.NEAREST))
board.paste(segment.resize((64,64), Image.Resampling.NEAREST), (220, y+68), segment.resize((64,64), Image.Resampling.NEAREST))
board.save(ROOT / 'preview.png')
