from pathlib import Path
from PIL import Image, ImageDraw
import math

HERE = Path(__file__).resolve().parent
OUT = HERE.parent
ROOT = OUT.parents[2]
OLD = ROOT / 'art/deliveries/orochimaru-set-v11b'
BASE = ROOT / 'art/deliveries/orochimaru-base-v11/Orochimaru_Idle_0.png'
GEN = HERE / 'generated_components.png'

INK = (20, 18, 25, 255)
HAIR = (36, 33, 46, 255)
HAIR_HI = (61, 55, 72, 255)
PALE = (230, 214, 194, 255)
PALE_MID = (196, 177, 163, 255)
PALE_DARK = (139, 122, 120, 255)
PURPLE = (74, 55, 82, 255)
PURPLE_HI = (102, 78, 111, 255)
TRANSPARENT = (0, 0, 0, 0)

def old(name):
    return Image.open(OLD / (name + '.png')).convert('RGBA')

def save(name, im):
    im.save(OUT / (name + '.png'))

def hard_alpha(im):
    im = im.convert('RGBA')
    pix = im.load()
    for y in range(im.height):
        for x in range(im.width):
            r,g,b,a = pix[x,y]
            pix[x,y] = (r,g,b,255) if a >= 128 else TRANSPARENT
    return im

# Four grounded poses. Stretching the old second pose between its crown and
# foot row fixes the old 8 px vertical bounce without moving the feet.
d0 = old('Orochimaru_Dash_0')
d1_old = old('Orochimaru_Dash_1')
d1 = Image.new('RGBA', (224,136), TRANSPARENT)
d1.paste(d1_old.crop((45,90,176,132)).resize((131,50), Image.Resampling.NEAREST), (45,82))
d2 = d0.copy()
dd = ImageDraw.Draw(d2)
dd.line([(69,87),(62,85),(58,85)], fill=HAIR_HI, width=1)
dd.line([(140,125),(145,127),(151,127)], fill=PALE_MID, width=1)
d3 = d1.copy()
dd = ImageDraw.Draw(d3)
dd.line([(79,92),(71,90),(66,90)], fill=HAIR_HI, width=1)
dd.line([(151,128),(155,130),(161,130)], fill=PALE_MID, width=1)
for i, im in enumerate([d0,d1,d2,d3]):
    save(f'Orochimaru_Dash_{i}', im)

# Hands: the two arm phases from the pose reference; the hold pose uses two
# separate normal-length sleeves and two explicitly open cuffs.
save('Orochimaru_Hands_0', old('Orochimaru_Hands_0'))
save('Orochimaru_Hands_1', old('Orochimaru_Hands_1'))
hands = old('Orochimaru_Hands_0')
dr = ImageDraw.Draw(hands)
# Cover the old single foreground cuff without touching the head or torso.
dr.rectangle((128,63,154,82), fill=TRANSPARENT)
for y0 in (61,73):
    dr.polygon([(126,y0+3),(130,y0),(147,y0),(151,y0+2),(151,y0+8),(147,y0+10),(130,y0+9),(126,y0+7)], fill=INK)
    dr.polygon([(128,y0+3),(132,y0+2),(147,y0+2),(149,y0+3),(149,y0+7),(145,y0+8),(130,y0+7)], fill=PALE)
    dr.line([(130,y0+2),(143,y0+2)], fill=(248,235,211,255), width=1)
    dr.line([(132,y0+7),(148,y0+7)], fill=PALE_MID, width=1)
    dr.ellipse((148,y0+1,154,y0+9), fill=INK)
    dr.ellipse((150,y0+2,153,y0+8), fill=HAIR)
save('Orochimaru_Hands_2', hands)

# End frames are intermediate postures already present in the named pose
# reference, now assigned to the reverse transition.
for action in ['Hands','Wind','Seal','Summon','Neck','Dash']:
    save(f'Orochimaru_{action}Out_0', old(f'Orochimaru_{action}In_0'))

neck = old('Orochimaru_Neck_0')
dr = ImageDraw.Draw(neck)
# A six-pixel root growing from the existing open collar toward upper right.
dr.polygon([(125,52),(128,47),(133,43),(137,47),(133,52),(129,56)], fill=INK)
dr.polygon([(127,51),(130,47),(133,45),(135,47),(132,51),(130,53)], fill=PALE)
dr.line([(128,51),(132,48)], fill=(247,230,207,255), width=1)
dr.line([(130,54),(133,51)], fill=PALE_DARK, width=1)
save('Orochimaru_Neck_0', neck)

# Crop the exact base face and hair at native pixels. Extend only the back
# hair, retaining the original gold eye and purple makeup pixels.
base = Image.open(BASE).convert('RGBA')
head = Image.new('RGBA',(56,44),TRANSPARENT)
dh = ImageDraw.Draw(head)
dh.polygon([(3,19),(12,15),(21,12),(34,9),(45,11),(40,29),(31,36),(20,32),(7,28)], fill=INK)
dh.polygon([(5,20),(17,16),(28,12),(39,12),(36,29),(27,31),(17,27),(7,26)], fill=HAIR)
dh.line([(5,20),(17,17),(30,13)], fill=HAIR_HI, width=1)
dh.line([(4,26),(17,24),(27,27)], fill=HAIR_HI, width=1)
# This tight rectangle excludes the tunic/shoulder in the base sprite.
head.alpha_composite(base.crop((108,28,127,56)), (35,5))
# Mouth opens below the preserved eye and cheek; two pale fang pixels.
dh = ImageDraw.Draw(head)
dh.polygon([(46,27),(52,27),(55,30),(52,35),(46,33)], fill=INK)
dh.polygon([(48,29),(53,30),(52,33),(48,32)], fill=(87,39,64,255))
dh.point((49,28),fill=PALE)
dh.point((53,31),fill=PALE)
# Horizontal flesh connection at the left center.
dh.rectangle((0,19,8,26), fill=PALE_MID)
dh.line((0,19,8,19),fill=INK,width=1)
dh.line((0,26,8,26),fill=INK,width=1)
dh.line((0,20,8,20),fill=PALE,width=1)
save('Orochimaru_NeckHead', head)

seg = Image.new('RGBA',(12,12),TRANSPARENT)
sd = ImageDraw.Draw(seg)
for y in range(1,11):
    color = INK if y in (1,10) else (PALE if y<5 else PALE_MID if y<8 else PALE_DARK)
    sd.line((0,y,11,y),fill=color,width=1)
sd.line((0,2,11,2),fill=(246,230,208,255),width=1)
save('Orochimaru_NeckSegment',seg)

# The generated snake head supplies the silhouette and scale pattern.
gen = Image.open(GEN).convert('RGBA')
snake = gen.crop((1155,246,1677,534)).resize((22,14),Image.Resampling.LANCZOS)
snake = hard_alpha(snake)
# Reassert high-contrast tiny eye, fang, and tongue after reduction.
sd = ImageDraw.Draw(snake)
sd.point((12,4),fill=(229,186,87,255))
sd.point((17,8),fill=PALE)
sd.line((19,10,21,11),fill=(118,39,82,255),width=1)
save('FxSnakeHand_Head',snake)

snake_seg = Image.new('RGBA',(8,8),TRANSPARENT)
sd = ImageDraw.Draw(snake_seg)
for y in range(1,7):
    sd.line((0,y,7,y),fill=INK if y in (1,6) else PURPLE if y>=4 else PURPLE_HI,width=1)
for x,y in [(2,2),(5,3),(3,5)]:
    sd.point((x,y),fill=(122,99,129,255))
save('FxSnakeHand_Segment',snake_seg)

def paste_center(canvas,im,x,y):
    canvas.alpha_composite(im,(x,y))

# Preview: every panel has the same native 224x136 coordinate system.
names = ['Orochimaru_Dash_0','Orochimaru_Dash_1','Orochimaru_Dash_2','Orochimaru_Dash_3']
panels=[]
for name in names:
    panels.append(Image.open(OUT/(name+'.png')).convert('RGBA'))
hp = hands.copy()
for cx,cy in [(152,65),(152,77)]:
    for k in range(3):
        paste_center(hp,snake_seg,cx+2+k*7,cy-4)
    paste_center(hp,snake,cx+22,cy-7)
panels.append(hp)
for angle in (0,30):
    p=Image.new('RGBA',(280,160),TRANSPARENT)
    paste_center(p,neck,0,24)
    ox,oy=133,49
    for k in range(6):
        x=round(ox+k*11*math.cos(math.radians(angle)))
        y=round(oy-k*11*math.sin(math.radians(angle)))
        if angle==0:
            paste_center(p,seg,x,y+24-6)
        else:
            rot=seg.rotate(angle,Image.Resampling.NEAREST,expand=True)
            paste_center(p,rot,x,y+24-rot.height//2)
    x=round(ox+6*11*math.cos(math.radians(angle)))
    y=round(oy-6*11*math.sin(math.radians(angle)))
    paste_center(p,head,x,y+24-22)
    panels.append(p)

bg=(69,70,78,255)
panel_w,panel_h=280,160
canvas=Image.new('RGBA',(4*panel_w,2*panel_h),bg)
for i,p in enumerate(panels):
    x=(i%4)*panel_w; y=(i//4)*panel_h
    canvas.alpha_composite(p,(x,y if p.height==160 else y+24))
    d=ImageDraw.Draw(canvas)
    d.line((x,y+155,x+panel_w-1,y+155),fill=(130,130,138,255),width=1)
canvas.convert('RGB').save(OUT/'preview.png')
canvas.convert('RGB').save(OUT/'preview_1x.png')
canvas.convert('RGB').resize((canvas.width*3,canvas.height*3),Image.Resampling.NEAREST).save(OUT/'preview_3x.png')
