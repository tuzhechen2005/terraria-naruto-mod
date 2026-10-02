from pathlib import Path
from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parents[4]
OUT = Path(__file__).resolve().parents[1]
V5A = ROOT / 'art/deliveries/orochimaru-set-v5a'
V5B = ROOT / 'art/deliveries/orochimaru-set-v5b'
TAZUNA = ROOT / 'ShinobiPrototype/Content/NPCs/Tazuna.png'

GROUPS = {
    'Hands': ['HandsIn_0', 'Hands_0', 'Hands_1', 'Hands_2'],
    'Dash': ['DashIn_0', 'Dash_0', 'Dash_1'],
    'Wind': ['WindIn_0', 'Wind_0', 'Wind_1'],
    'Neck': ['NeckIn_0', 'Neck_0'],
    'Seal': ['SealIn_0', 'Seal_0', 'Seal_1'],
    'Summon': ['SummonIn_0', 'Summon_0', 'Summon_1'],
}

# Placement is the top-left of the exact head crop from the v5a idle frame.
HEAD_POS = {
    'HandsIn_0': (38, 10), 'Hands_0': (26, 14),
    'Hands_1': (24, 10), 'Hands_2': (48, 12),
    'DashIn_0': (62, 36), 'Dash_0': (68, 48), 'Dash_1': (76, 50),
    'WindIn_0': (44, 10), 'Wind_0': (44, 8), 'Wind_1': (62, 24),
    'NeckIn_0': (58, 16),
    'SealIn_0': (46, 8), 'Seal_0': (32, 12), 'Seal_1': (42, 10),
    'SummonIn_0': (50, 10), 'Summon_0': (52, 28), 'Summon_1': (60, 36),
}

base = Image.open(V5A / 'Orochimaru_Idle_0.png').convert('RGBA')
# The patch takes the original two eyes, facial shading, hair and jaw in one
# untouched group of 2x2 pixels. Transparent pixels retain their alpha.
head = base.crop((52, 12, 86, 46))

def grid_offset(im):
    scores = []
    for oy in range(2):
        for ox in range(2):
            good = total = 0
            for y in range(oy, 86, 2):
                for x in range(ox, 110, 2):
                    block = [im.getpixel((x + dx, y + dy)) for dy in range(2) for dx in range(2)]
                    if any(p[3] for p in block):
                        total += 1
                        good += len(set(block)) == 1
            scores.append((good / total, -ox - oy, ox, oy))
    return max(scores)[2:]

def checker(size, tile=8):
    w, h = size
    bg = Image.new('RGBA', size)
    px = bg.load()
    for y in range(h):
        for x in range(w):
            c = 88 if (x // tile + y // tile) % 2 else 108
            px[x, y] = (c, c, c + 3, 255)
    return bg

for group, names in GROUPS.items():
    for name in names:
        frame = Image.open(V5B / f'Orochimaru_{name}.png').convert('RGBA')
        ox, oy = grid_offset(frame)
        if ox or oy:
            aligned = Image.new('RGBA', frame.size)
            aligned.alpha_composite(frame, (-ox, -oy))
            frame = aligned
        if name in HEAD_POS:
            hx, hy = HEAD_POS[name]
            frame.alpha_composite(head, (hx, hy))
        frame.save(OUT / f'Orochimaru_{name}.png')

tazuna = Image.open(TAZUNA).convert('RGBA').crop((0, 0, 112, 88))
cols = 6
row_height = 480
preview = Image.new('RGB', (cols * 448, len(GROUPS) * row_height), '#29292e')
draw = ImageDraw.Draw(preview)
for row, (group, names) in enumerate(GROUPS.items()):
    entries = [('Base', base), ('Tazuna', tazuna)] + [
        (name, Image.open(OUT / f'Orochimaru_{name}.png').convert('RGBA')) for name in names
    ]
    for col, (label, im) in enumerate(entries):
        x, y = col * 120, row * row_height
        draw.text((x + 2, y + 2), label, fill='white')
        bg = checker((112, 88))
        bg.alpha_composite(im)
        preview.paste(bg.convert('RGB'), (x, y + 16))
        big = bg.resize((112 * 4, 88 * 4), Image.Resampling.NEAREST)
        # Each enlarged frame occupies its own 4x column in a second strip.
        big_strip = OUT / 'source' / f'{group}_4x.png'
        if col == 0:
            strip = Image.new('RGB', (cols * 448, 352), '#29292e')
        strip.paste(big.convert('RGB'), (col * 448, 0))
    strip.save(big_strip)
    preview.paste(strip, (0, row * row_height + 112))
preview.save(OUT / 'preview.png')
