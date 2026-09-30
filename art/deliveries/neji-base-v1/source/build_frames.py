from pathlib import Path
from PIL import Image, ImageDraw
from collections import Counter

ROOT = Path(__file__).resolve().parent.parent
SRC = ROOT / 'source'
W, H = 56, 44  # one art pixel becomes one 2x2 screen block
T = (0, 0, 0, 0)


def ref(name):
    return Image.open(SRC / f'Neji_{name}.png').convert('RGBA').resize((W, H), Image.Resampling.NEAREST)


idle, palm, rotation, sixtyfour = [ref(n) for n in ('Idle', 'Palm', 'Rotation', 'SixtyFour')]
body_palette = [rgb for rgb, n in Counter(p[:3] for p in idle.getdata() if p[3] > 0).most_common()]
effect_palette = [(24, 90, 117), (35, 138, 172), (74, 204, 225), (162, 241, 245)]
palette = body_palette + effect_palette


def generated(name, target_w, target_h, cx=28, foot=42):
    image = Image.open(SRC / f'Neji_{name}_generated.png').convert('RGBA')
    alpha = image.getchannel('A').point(lambda a: 255 if a >= 128 else 0)
    bbox = alpha.getbbox()
    image = image.crop(bbox)
    image = image.resize((target_w, target_h), Image.Resampling.NEAREST)
    canvas = Image.new('RGBA', (W, H), T)
    x, y = cx - target_w // 2, foot - target_h
    for j in range(target_h):
        for i in range(target_w):
            r, g, b, a = image.getpixel((i, j))
            if a < 128:
                continue
            colors = palette if b > r + 15 and g > r + 10 else body_palette
            color = min(colors, key=lambda c: (r-c[0])**2 + (g-c[1])**2 + (b-c[2])**2)
            if 0 <= x+i < W and 0 <= y+j < H:
                canvas.putpixel((x+i, y+j), (*color, 255))
    return canvas


run = generated('Run', 46, 29)
hurt = generated('Hurt', 37, 34)
alt = generated('PalmAlternate', 49, 35)


def deform(image, upper_dx=0, upper_dy=0, mid_dx=0, lower_dx=0, hair_dx=0):
    out = Image.new('RGBA', (W, H), T)
    for y in range(H):
        for x in range(W):
            px = image.getpixel((x, y))
            if px[3] == 0:
                continue
            dx = upper_dx if y < 22 else mid_dx if y < 31 else lower_dx if y < 39 else 0
            dy = upper_dy if y < 25 else 0
            if y < 19 and x < 27:
                dx += hair_dx
            nx, ny = x + dx, y + dy
            if 0 <= nx < W and 0 <= ny < H:
                out.putpixel((nx, ny), px)
    return out


def veins(image, x, y):
    out = image.copy()
    d = ImageDraw.Draw(out)
    c = (*effect_palette[1], 255)
    for xx, yy in ((x,y), (x-1,y+1), (x-2,y+2), (x+1,y-1)):
        if 0 <= xx < W and 0 <= yy < H and out.getpixel((xx,yy))[3]:
            d.point((xx, yy), fill=c)
    return out


def palm_glow(image, shift=0):
    out = image.copy()
    d = ImageDraw.Draw(out)
    x, y = 43 + shift, 20
    for dx, dy, c in ((0,0,3),(1,0,2),(0,-1,2),(-1,1,1)):
        if 0 <= x+dx < W and 0 <= y+dy < H:
            d.point((x+dx,y+dy), fill=(*effect_palette[c],255))
    return out


def strike_trail(image, offset):
    out = image.copy()
    d = ImageDraw.Draw(out)
    for q in range(3):
        x = 40 - offset - q*2
        y = 19 + q%2
        d.line((x, y, x+2, y), fill=(*effect_palette[1 + (q == 0)],255))
    return out


def limit_colors(image, maximum=20):
    counts = Counter(p[:3] for p in image.getdata() if p[3])
    if len(counts) <= maximum:
        return image
    azure = [c for c, _ in counts.most_common() if c[2] > c[0]+20 and c[1] > c[0]+15]
    kept = list(dict.fromkeys(azure[:4] + [c for c, _ in counts.most_common()]))[:maximum]
    remap = {c: min(kept, key=lambda k: sum((c[i]-k[i])**2 for i in range(3))) for c in counts if c not in kept}
    out = image.copy()
    out.putdata([(*remap.get(p[:3],p[:3]),p[3]) if p[3] else T for p in image.getdata()])
    return out


def rotation_turn(base, mode):
    # Keep the outer chakra ring fixed while showing four body angles.
    if mode == 0:
        return base.copy()
    ring = Image.new('RGBA', (W, H), T)
    body = Image.new('RGBA', (W, H), T)
    for y in range(H):
        for x in range(W):
            p = base.getpixel((x,y))
            if not p[3]:
                continue
            if p[2] > p[0] + 20 and p[1] > p[0] + 15:
                ring.putpixel((x,y),p)
            else:
                body.putpixel((x,y),p)
    if mode == 1:
        body = body.resize((36,H),Image.Resampling.NEAREST)
        layer = Image.new('RGBA',(W,H),T); layer.alpha_composite(body,(10,0)); body=layer
    elif mode == 2:
        body = body.transpose(Image.Transpose.FLIP_LEFT_RIGHT)
    else:
        body = body.transpose(Image.Transpose.FLIP_LEFT_RIGHT).resize((36,H),Image.Resampling.NEAREST)
        layer = Image.new('RGBA',(W,H),T); layer.alpha_composite(body,(10,0)); body=layer
    ring.alpha_composite(body)
    return ring


frames = {
    'Idle': [idle, deform(idle, upper_dy=-1), deform(idle, upper_dx=1, upper_dy=-1, hair_dx=-1), deform(idle, upper_dy=-1)],
    'Run': [run, deform(run, upper_dx=1, mid_dx=1, hair_dx=-1), deform(run, upper_dx=2, mid_dx=1, lower_dx=-1, hair_dx=-2), deform(run, upper_dx=1, lower_dx=1, hair_dx=-1)],
    'Palm': [palm_glow(veins(palm, 32, 14)), palm_glow(veins(alt, 34, 12), 3), palm_glow(veins(deform(palm, upper_dx=2, mid_dx=1), 34, 14), 2)],
    'Rotation': [rotation_turn(rotation, i) for i in range(4)],
    'SixtyFour_Windup': [veins(sixtyfour, 30, 13), veins(deform(sixtyfour, upper_dx=1, upper_dy=1), 31, 14)],
    'SixtyFour_Strike': [strike_trail(veins(palm, 32, 14), 0), strike_trail(veins(alt, 34, 12), 2), strike_trail(veins(deform(palm,upper_dx=2,mid_dx=1), 34, 14), 4)],
    'Hurt': [hurt],
}

for action, poses in frames.items():
    for index, image in enumerate(poses):
        # Native 2x2 block scale, binary alpha, no smoothing.
        game = limit_colors(image).resize((112,88), Image.Resampling.NEAREST)
        game.save(ROOT / f'Neji_{action}_{index}.png')

tile_w, tile_h = 112, 88
max_cols = max(len(x) for x in frames.values())
preview = Image.new('RGB', (max_cols*tile_w*2, len(frames)*tile_h), (32,36,42))
align = Image.new('RGB', (len(frames)*tile_w, tile_h), (31,36,45))
for row, (action, poses) in enumerate(frames.items()):
    stack = Image.new('RGBA', (tile_w,tile_h), T)
    for col, pose in enumerate(poses):
        game = Image.open(ROOT / f'Neji_{action}_{col}.png')
        ghost = game.copy()
        ghost.putalpha(ghost.getchannel('A').point(lambda a: int(a/max(1,len(poses)))))
        stack.alpha_composite(ghost)
        for side,bg in enumerate(((205,207,207),(31,36,45))):
            x, y = (side*max_cols+col)*tile_w, row*tile_h
            tile = Image.new('RGBA', (tile_w,tile_h), (*bg,255))
            tile.alpha_composite(game)
            preview.paste(tile.convert('RGB'),(x,y))
    tile = Image.new('RGBA',(tile_w,tile_h),(31,36,45,255))
    tile.alpha_composite(stack)
    align.paste(tile.convert('RGB'),(row*tile_w,0))
    d=ImageDraw.Draw(align)
    x=row*tile_w
    d.line((x+56,0,x+56,87),fill=(180,62,76),width=1)
    d.line((x,84,x+111,84),fill=(180,62,76),width=1)
    d.text((x+2,2),action,fill=(250,250,250))
preview.save(ROOT/'preview.png')
align.save(ROOT/'alignment-check.png')
