from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).parent
SOURCES = {
    'Tazuna': 'Tazuna_Generated.png',
    'Iruka': 'Iruka_Generated.png',
    'Kakashi': 'Kakashi_Generated.png',
}

def base_sprite(name):
    src = Image.open(ROOT / 'source' / SOURCES[name]).convert('RGBA')
    bbox = src.getchannel('A').point(lambda x: 255 if x >= 128 else 0).getbbox()
    # Box-resample the painted source, then redraw pixels and features below.
    crop = src.crop(bbox)
    width = round(crop.width * 62 / crop.height)
    small = crop.resize((width, 62), Image.Resampling.BOX)
    mask = small.getchannel('A').point(lambda a: 255 if a >= 130 else 0)
    quant = small.convert('RGB').quantize(colors=30, method=Image.Quantize.MEDIANCUT, dither=Image.Dither.NONE).convert('RGBA')
    quant.putalpha(mask)
    out = Image.new('RGBA', (80, 80))
    out.alpha_composite(quant, ((80-width)//2, 15))
    return out

INK = '#211b24'
SKIN_DARK = '#a85535'
SKIN_MID = '#df8b5c'
SKIN_LIGHT = '#ffb87e'
WHITE_DARK = '#a6a0a5'
WHITE_MID = '#ddd5cb'
WHITE_LIGHT = '#fff2db'
BLUE_DARK = '#171e39'
BLUE_MID = '#28385c'

def tazuna(im):
    d=ImageDraw.Draw(im)
    # Solid skin planes, tiny stepped nose and white beard with a hard border.
    d.polygon([(36,23),(44,22),(48,25),(48,31),(46,34),(40,35),(35,32)],fill=SKIN_MID)
    d.polygon([(38,24),(44,23),(46,25),(45,31),(39,31),(37,29)],fill=SKIN_LIGHT)
    d.rectangle((35,28,36,32),fill=SKIN_DARK)
    d.point((48,30),fill=SKIN_DARK)
    d.point((47,31),fill=SKIN_LIGHT)
    # Hairline and two circular rims remain distinct at one-to-one scale.
    d.polygon([(34,22),(36,19),(40,18),(42,20),(45,19),(47,22),(46,24),(43,22),(40,22),(38,24),(35,24)],fill=WHITE_MID)
    d.line([(35,21),(38,19),(41,19)],fill=WHITE_LIGHT,width=1)
    for x in (37,43):
        d.polygon([(x+1,26),(x+3,26),(x+4,27),(x+4,29),(x+3,30),(x+1,30),(x,29),(x,27)],fill=INK)
        d.rectangle((x+1,27,x+3,29),fill='#e5d9c8')
        d.point((x+2,27),fill=WHITE_LIGHT)
        d.point((x+2,28),fill=INK)
    d.line((41,27,42,27),fill=INK)
    d.point((38,25),fill='#70402f'); d.point((44,25),fill='#70402f')
    d.polygon([(35,32),(38,31),(40,32),(43,31),(46,32),(48,34),(46,37),(43,39),(39,39),(36,37)],fill=WHITE_MID)
    d.line((36,33,39,33),fill=WHITE_LIGHT)
    d.line((43,33,46,33),fill=WHITE_LIGHT)
    d.line((39,35,44,35),fill='#633b35')
    d.line((40,36,44,36),fill=INK)
    d.point((46,36),fill=WHITE_LIGHT)

def iruka(im):
    d=ImageDraw.Draw(im)
    # Rebuild a broad, clear face. Hair and headband stay on top.
    d.polygon([(37,27),(46,27),(49,30),(48,35),(44,38),(39,37),(36,33)],fill=SKIN_MID)
    d.polygon([(38,28),(45,28),(47,30),(46,34),(43,36),(39,35),(38,32)],fill=SKIN_LIGHT)
    d.line((36,29,36,33),fill=SKIN_DARK)
    d.line((48,31,49,32),fill=SKIN_DARK)
    d.point((48,33),fill=SKIN_LIGHT)
    # Straight blue band, metal plate and a one-pixel highlight.
    d.polygon([(36,24),(45,23),(48,25),(48,27),(36,27)],fill=BLUE_DARK)
    d.rectangle((40,24,46,27),fill='#98a4aa',outline=INK)
    d.line((41,25,44,25),fill='#eef0e7')
    d.point((43,26),fill='#354359')
    # Two shaped eyes and brows.
    d.line((39,28,42,28),fill='#392625')
    d.line((44,28,47,28),fill='#392625')
    d.rectangle((39,29,42,31),fill='#fff4dc')
    d.rectangle((44,29,47,31),fill='#fff4dc')
    d.line((39,29,42,29),fill=INK)
    d.line((44,29,47,29),fill=INK)
    d.rectangle((41,30,42,31),fill='#242638')
    d.rectangle((46,30,47,31),fill='#242638')
    # Scar: exactly one pixel high across nose bridge; tiny nose corner below.
    d.line((42,33,46,33),fill='#955338')
    d.point((48,34),fill=SKIN_DARK)
    d.line((42,36,45,36),fill='#743f37')
    d.point((46,35),fill='#743f37')

def kakashi(im):
    d=ImageDraw.Draw(im)
    # Preserve the generated spiky silhouette, repair visible eye and mask.
    d.polygon([(35,31),(44,31),(47,34),(47,38),(43,40),(35,39),(32,36)],fill=SKIN_MID)
    d.polygon([(36,32),(43,32),(46,34),(44,37),(37,37),(35,35)],fill=SKIN_LIGHT)
    # Slanted band covers character's left eye; silver plate follows the slope.
    d.polygon([(30,30),(35,27),(45,29),(48,32),(46,34),(36,32),(31,34)],fill=BLUE_DARK)
    d.polygon([(35,28),(41,29),(43,31),(41,32),(35,30)],fill='#a7afb6')
    d.line((36,29,40,30),fill='#f3f1e9')
    d.point((39,31),fill='#414c60')
    # One half-lidded eye. Lower nose outline is a single-step contour.
    d.line((41,34,45,34),fill=INK)
    d.line((41,35,44,35),fill='#fff0dc')
    d.point((43,35),fill=INK)
    d.point((46,36),fill=SKIN_DARK)
    d.polygon([(34,38),(37,37),(45,37),(47,38),(46,43),(41,44),(35,42)],fill=BLUE_DARK)
    d.line((37,38,45,38),fill=BLUE_MID)
    d.line((36,39,36,42),fill='#39486c')

REPAIRS={'Tazuna':tazuna,'Iruka':iruka,'Kakashi':kakashi}

def proportion_pass(sprite):
    # Keep the designed face width while matching the requested ~20 px head height.
    out=Image.new('RGBA',(80,80))
    out.alpha_composite(sprite.crop((0,15,80,44)).resize((80,21),Image.Resampling.NEAREST),(0,15))
    out.alpha_composite(sprite.crop((0,44,80,77)).resize((80,41),Image.Resampling.NEAREST),(0,36))
    return out

for n in SOURCES:
    sprite=base_sprite(n)
    REPAIRS[n](sprite)
    sprite=proportion_pass(sprite)
    sprite.save(ROOT / f'{n}_Idle.png')

sprites={n: Image.open(ROOT / f'{n}_Idle.png').convert('RGBA') for n in SOURCES}
guide=Image.open(ROOT.parent.parent / 'reference' / 'terraria' / 'NPC_22_x4.png').convert('RGBA')
guide=guide.crop(guide.getchannel('A').getbbox())
guide=guide.resize((26,46),Image.Resampling.NEAREST).resize((35,63),Image.Resampling.NEAREST)
guide_canvas=Image.new('RGBA',(80,80))
guide_canvas.alpha_composite(guide,((80-guide.width)//2,77-guide.height))

preview=Image.new('RGB',(1000,460),'#263043')
d=ImageDraw.Draw(preview)
items=list(sprites.items())+[('Guide ×1.36',guide_canvas)]
for col,(name,sprite) in enumerate(items):
    x=col*250
    d.text((x+8,8),name,fill='#fff6de')
    preview.paste(sprite,(x+85,30),sprite)
    d.line((x+50,107,x+190,107),fill='#8791a1')
    d.text((x+8,117),'1x',fill='#b8c7d9')
    enlarged=sprite.resize((240,240),Image.Resampling.NEAREST)
    preview.paste(enlarged,(x+5,165),enlarged)
    d.line((x+5,395,x+245,395),fill='#8791a1')
    d.text((x+8,420),'3x',fill='#b8c7d9')
preview.save(ROOT/'preview.png')

faces=Image.new('RGB',(3*280,310),'#263043')
d=ImageDraw.Draw(faces)
for col,(name,sprite) in enumerate(sprites.items()):
    x=col*280+12
    d.text((x,8),name,fill='#fff6de')
    crop=sprite.crop((24,13,56,45)).resize((256,256),Image.Resampling.NEAREST)
    faces.paste(crop,(x,30),crop)
    for line in range(33):
        p=line*8
        d.line((x+p,30,x+p,286),fill='#5c6880',width=1)
        d.line((x,30+p,x+256,30+p),fill='#5c6880',width=1)
    d.text((x,290),'8x / 1px grid',fill='#b8c7d9')
faces.save(ROOT/'faces_8x.png')
