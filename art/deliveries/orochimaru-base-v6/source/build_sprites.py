from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / 'source'
FILES = {k: SOURCE / f'Orochimaru_source_{k}.png' for k in 'ABC'}
OUT = ROOT
W, H = 56, 44

def sprite(src):
    im = Image.open(src).convert('RGBA')
    a = im.getchannel('A')
    bbox = a.point(lambda x: 255 if x >= 128 else 0).getbbox()
    crop = im.crop(bbox)
    h = 34
    w = round(crop.width * h / crop.height)
    crop = crop.resize((w, h), Image.Resampling.LANCZOS)
    # Quantize only solid source colors; avoid semitransparent border halos.
    mask = crop.getchannel('A').point(lambda x: 255 if x >= 128 else 0)
    rgb = Image.new('RGB', crop.size, (8, 8, 15))
    rgb.paste(crop, mask=crop.getchannel('A'))
    rgb = rgb.quantize(colors=28, method=Image.Quantize.MEDIANCUT, dither=Image.Dither.NONE).convert('RGB')
    crop = Image.merge('RGBA', (*rgb.split(), mask))
    canvas = Image.new('RGBA', (W,H), (0,0,0,0))
    canvas.paste(crop, (28-w//2, 42-h), crop)
    return canvas

sprites = {k:sprite(v) for k,v in FILES.items()}

# The source edits drifted by a few large pixels below the neck. Keep one body
# across all three samples, as specified, and change only the head region.
base = sprites['A']
for k in 'BC':
    s = sprites[k]
    merged = base.copy()
    merged.paste(s.crop((19, 7, 37, 17)), (19,7))
    sprites[k] = merged

# Retouch eyes at the art-pixel grid. Two readable gold eyes and extended
# violet shadow survive the 112x88 export; B has a larger pale face and C
# has a colder, narrower expression.
for k, im in sprites.items():
    # Rebuild the tiny face in hard color blocks. A has the canonical slim
    # profile, B adds one face column, C lengthens the colder eye line.
    d = ImageDraw.Draw(im)
    outline = (21,20,30,255)
    skin = (220,214,207,255) if k=='C' else (235,224,211,255)
    shadow = (154,145,158,255) if k=='C' else (185,169,166,255)
    violet = (117,54,142,255)
    gold = (239,196,58,255)
    d.rectangle((28,10,33 if k!='B' else 34,14), fill=skin)
    d.point((28,13), fill=shadow)
    d.point((29,14), fill=shadow)
    d.point((34 if k=='B' else 33,13), fill=shadow)
    d.line((27,10,29,9,32,9,34,10), fill=outline, width=1)
    d.line((27,11,27,14,29,15,32,15), fill=outline, width=1)
    d.point((34 if k=='B' else 33,14), fill=outline)
    # Violet extends beyond both eyes. Each gold eye is 2x2 art pixels.
    d.line((27,10,30,10), fill=violet, width=1)
    d.line((31,10,34 if k!='C' else 35,10), fill=violet, width=1)
    for x in (28,32):
        d.rectangle((x,11,x+1,12), fill=gold)
        d.point((x+1,11), fill=outline)
    d.point((34,12), fill=shadow)  # one-pixel nose bridge
    d.line((31,14,33 if k!='B' else 34,14), fill=outline, width=1)
    d.point((33 if k!='B' else 34,13), fill=outline)  # raised corner
    if k=='C':
        d.line((28,11,29,11), fill=violet, width=1)
        d.line((32,11,33,11), fill=violet, width=1)
        d.point((28,12), fill=gold)
        d.point((32,12), fill=gold)
    # Isolated one-pixel fragments are generated resize artifacts.
    seen=set()
    for py in range(H):
        for px in range(W):
            if (px,py) in seen or im.getpixel((px,py))[3]==0: continue
            stack=[(px,py)]; comp=[]; seen.add((px,py))
            while stack:
                cx,cy=stack.pop(); comp.append((cx,cy))
                for ny in range(max(0,cy-1),min(H,cy+2)):
                    for nx in range(max(0,cx-1),min(W,cx+2)):
                        if (nx,ny) not in seen and im.getpixel((nx,ny))[3]:
                            seen.add((nx,ny));stack.append((nx,ny))
            if len(comp)==1: im.putpixel(comp[0], (0,0,0,0))
    big = im.resize((112,88), Image.Resampling.NEAREST)
    big.save(OUT / f'Orochimaru_Idle_{k}.png')

def checker(size, scale=8):
    bg = Image.new('RGBA', size, '#363d49')
    d = ImageDraw.Draw(bg)
    for y in range(0,size[1],scale):
        for x in range(0,size[0],scale):
            if (x//scale+y//scale)%2==0:
                d.rectangle((x,y,x+scale-1,y+scale-1), fill='#424956')
    return bg

compare = [Image.open(OUT / f'Orochimaru_Idle_{k}.png').convert('RGBA') for k in 'ABC']
project = ROOT.parents[2]
compare += [Image.open(project / p).convert('RGBA') for p in [
    'ShinobiPrototype/Content/NPCs/Orochimaru_Idle_0.png',
    'art/deliveries/gaara-set-v2/Gaara_Idle_0.png',
    'ShinobiPrototype/Content/NPCs/Neji_Idle_0.png']]
labels = ['A anime', 'B readable', 'C sinister', 'v5', 'Gaara', 'Neji']
font = ImageFont.load_default()
panel = Image.new('RGBA', (6*120+8, 88+28), '#222934')
d = ImageDraw.Draw(panel)
for i,(im,label) in enumerate(zip(compare,labels)):
    x = i*120+4
    panel.alpha_composite(checker((112,88)), (x,24))
    panel.alpha_composite(im, (x,24))
    d.text((x+2,6), label, fill='white', font=font)
panel.save(OUT/'preview_1x.png')
panel.resize((panel.width*4,panel.height*4),Image.Resampling.NEAREST).save(OUT/'preview_4x.png')
preview = Image.new('RGBA', (panel.width*4,panel.height*5+16), '#222934')
preview.alpha_composite(panel,(0,0))
preview.alpha_composite(panel.resize((panel.width*4,panel.height*4),Image.Resampling.NEAREST),(0,panel.height+16))
preview.save(OUT/'preview.png')
for k in 'ABC':
    im = Image.open(OUT / f'Orochimaru_Idle_{k}.png').convert('RGBA')
    im.crop((40,12,76,38)).resize((288,208),Image.Resampling.NEAREST).save(OUT/f'face_{k}_8x.png')
