from pathlib import Path
from PIL import Image, ImageDraw, ImageFilter
import math
import shutil
from collections import Counter

ROOT = Path('/Users/tuzhechen/Documents/ChatGPT/泰拉瑞亚火影模组')
OUT = ROOT / 'art/deliveries/dosu-set-v3'
SRC = ROOT / 'ShinobiPrototype/Content/NPCs'
GEN = Path('/Users/tuzhechen/.codex/generated_images/01a0fb42-de7c-7283-bb88-d2c370dd8715/exec-f0d563ae-9361-42a6-86f3-04f9a9bcf5d7.png')
OUT.mkdir(parents=True, exist_ok=True)
(OUT/'source').mkdir(exist_ok=True)
shutil.copy2(GEN, OUT/'source/Dosu_Idle_0_generated.png')

# All frames are painted on a 56 x 44 art-pixel canvas and enlarged by exactly 2.
PAL = [(16, 11, 20), (39, 34, 45), (62, 55, 65), (86, 75, 81),
       (115, 100, 101), (147, 126, 121), (175, 153, 143),
       (65, 68, 80), (101, 104, 115), (146, 149, 160), (190, 191, 197),
       (197, 186, 176), (224, 216, 206), (241, 235, 225),
       (169, 125, 91), (216, 172, 125)]

def snap(im):
    im = im.convert('RGBA')
    pix = im.load()
    for y in range(im.height):
        for x in range(im.width):
            r,g,b,a = pix[x,y]
            if a < 110:
                pix[x,y] = (0,0,0,0)
            else:
                c = min(PAL, key=lambda v: (r-v[0])**2+(g-v[1])**2+(b-v[2])**2)
                pix[x,y] = (*c,255)
    # Close the outer contour and remove isolated specks.
    original = im.copy()
    op = original.load()
    for y in range(1,im.height-1):
        for x in range(1,im.width-1):
            if op[x,y][3] == 0:
                continue
            n = sum(op[x+dx,y+dy][3] > 0 for dx,dy in ((1,0),(-1,0),(0,1),(0,-1)))
            if n == 0:
                pix[x,y] = (0,0,0,0)
            elif n < 4 and op[x,y][:3] not in (PAL[0],PAL[1]):
                pix[x,y] = (*PAL[0],255)
    return im

def generated_base():
    im = Image.open(GEN).convert('RGBA')
    box = im.getchannel('A').point(lambda a: 255 if a >= 80 else 0).getbbox()
    im = im.crop(box)
    im = im.resize((25,30),Image.Resampling.BOX)
    im = snap(im)
    canvas = Image.new('RGBA',(56,44))
    canvas.alpha_composite(im,(14,12))
    return canvas

def old_pose(name):
    im = Image.open(SRC/f'{name}.png').convert('RGBA')
    im = im.resize((56,44),Image.Resampling.BOX)
    im=snap(im)
    src=im.copy(); p=im.load(); q=src.load()
    # Remove high-frequency flecks in the old pose references without moving edges.
    for y in range(1,43):
        for x in range(1,55):
            if not q[x,y][3]: continue
            neighbours=[q[x+dx,y+dy][:3] for dy in (-1,0,1) for dx in (-1,0,1)
                        if q[x+dx,y+dy][3]]
            if len(neighbours)<8: continue
            color,count=Counter(neighbours).most_common(1)[0]
            if count>=5 and q[x,y][:3]!=color: p[x,y]=(*color,255)
    return im

def save(name, im):
    im = snap(im)
    # Put grounded frames on the same lowest art-pixel row (screen y=83).
    if 'Leap_1' not in name:
        box=im.getchannel('A').getbbox()
        if box and box[3] != 42:
            shifted=Image.new('RGBA',(56,44)); shifted.alpha_composite(im,(0,42-box[3])); im=shifted
    im.resize((112,88),Image.Resampling.NEAREST).save(OUT/f'{name}.png')

base=generated_base()
for i in range(6):
    im=base.copy()
    # Two-pixel breathing loop: shoulder and hem are changed within the same silhouette.
    if i in (1,2,3,4):
        p=im.load(); shade=PAL[4] if i in (1,4) else PAL[5]
        for x,y in ((20,20),(21,20),(23,22),(27,29),(31,33)):
            if p[x,y][3]: p[x,y]=(*shade,255)
    if i in (2,3):
        p=im.load()
        for x,y in ((19,35),(20,36),(33,35)):
            if p[x,y][3]: p[x,y]=(*PAL[3],255)
    save(f'Dosu_Idle_{i}',im)

for i in range(8):
    im=old_pose(f'Dosu_Walk_{i//2}')
    if i%2:
        # Between poses, the upper body leads and the next pose's legs follow.
        following=old_pose(f'Dosu_Walk_{(i//2+1)%4}')
        for y in range(33,44):
            for x in range(56): im.putpixel((x,y),following.getpixel((x,y)))
    save(f'Dosu_Walk_{i}',im)

names=['Dosu_Hurt_0','Dosu_DrillWindup_0','Dosu_DrillWindup_1',
       'Dosu_Drill_0','Dosu_Drill_1',
       'Dosu_Wave_0','Dosu_Wave_1','Dosu_Wave_2',
       'Dosu_Slam_0','Dosu_Slam_1','Dosu_Slam_2',
       'Dosu_Leap_0','Dosu_Leap_1','Dosu_Leap_2']
for name in names: save(name,old_pose(name))
for start,target in [('Dosu_DrillWindupIn_0','Dosu_DrillWindup_0'),
                     ('Dosu_WaveIn_0','Dosu_Wave_0'),
                     ('Dosu_SlamIn_0','Dosu_Slam_0'),
                     ('Dosu_LeapIn_0','Dosu_Leap_0')]:
    im=old_pose(target)
    # Lead-in combines idle lower body with the beginning of each technique.
    for y in range(29,44):
        for x in range(56):
            im.putpixel((x,y),base.getpixel((x,y)))
    save(start,im)

head=Image.open(SRC/'Dosu_Head_Boss.png').convert('RGBA')
head=snap(head)
head.save(OUT/'Dosu_Head_Boss.png')

names=sorted(p.stem for p in OUT.glob('Dosu_*.png') if p.stem!='Dosu_Head_Boss')
ref=[ROOT/'art/deliveries/dosu-base-v3/Dosu_Idle_0.png',SRC/'Dosu_Idle_0.png',ROOT/'art/deliveries/tazuna-npc-v1/source/Tazuna_Idle_Walk.png']
cellw,cellh=126,132
all_names=['base reference','old idle','Tazuna']+names
sheet=Image.new('RGB',(cellw*6,cellh*math.ceil(len(all_names)/6)),(43,49,59)); d=ImageDraw.Draw(sheet)
for i,name in enumerate(all_names):
    p=ref[i] if i<3 else OUT/f'{name}.png'
    im=Image.open(p).convert('RGBA')
    if i==2: im=im.crop((0,0,256,256))
    im.thumbnail((112,100),Image.Resampling.NEAREST)
    x=(i%6)*cellw+(cellw-im.width)//2;y=(i//6)*cellh+4
    sheet.paste(im,(x,y),im)
    d.text(((i%6)*cellw+2,(i//6)*cellh+108),name.replace('Dosu_',''),fill='white')
sheet.save(OUT/'preview.png')
sheet.resize((sheet.width*4,sheet.height*4),Image.Resampling.NEAREST).save(OUT/'preview_4x.png')

for series,count in [('Idle',6),('Walk',8)]:
    overlay=Image.new('RGBA',(112,88))
    for i in range(count):
        im=Image.open(OUT/f'Dosu_{series}_{i}.png').convert('RGBA')
        a=im.getchannel('A').point(lambda v: 70 if v else 0)
        im.putalpha(a);overlay.alpha_composite(im)
    overlay.save(OUT/f'Dosu_{series}_overlay.png')
