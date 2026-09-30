from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parent.parent
SRC = ROOT / 'source'
idle = Image.open(SRC / 'Gaara_Idle.png').convert('RGBA')
cast = Image.open(SRC / 'Gaara_Cast.png').convert('RGBA')
shield = Image.open(SRC / 'Gaara_Shield.png').convert('RGBA')
beast = Image.open(SRC / 'Gaara_Transformed.png').convert('RGBA')

INK = (49, 35, 37, 255)
DARK = (95, 69, 53, 255)
SAND = (174, 126, 75, 255)
MID = (204, 159, 101, 255)
LIGHT = (234, 192, 127, 255)
HIGHLIGHT = (246, 213, 151, 255)
GOLD = (255, 212, 39, 255)
MARK = (42, 48, 64, 255)

frames = []

def put(action, i, im):
    im.save(ROOT / f'Gaara_{action}_{i}.png')
    frames.append((action, i, im.copy()))

def d(im):
    return ImageDraw.Draw(im)

def sand_stream(im, phase):
    dr = d(im)
    # The mouth of the gourd stays fixed; grains make the four-frame loop.
    for x, y in [(47, 34+phase%2), (46, 39+(phase+1)%3),
                 (44+phase%2, 45), (45, 51+phase%3)][:phase+1]:
        dr.point((x,y), fill=DARK)
        dr.point((x+1,y), fill=LIGHT)

for i in range(4):
    im = idle.copy()
    sand_stream(im, i)
    put('Idle', i, im)

for i, dx in enumerate((0, 2, 0, -2)):
    im = idle.copy()
    # Shift only the feet/legs; torso, head and gourd keep a stable center.
    im.paste((0,0,0,0), (39, 65, 77, 84))
    legs = idle.crop((39,65,77,84))
    im.alpha_composite(legs, (39+dx,65))
    dr = d(im)
    if i in (1,3):
        dr.line([(54-dx,79),(54-dx*2,83)], fill=INK, width=2)
    sand_stream(im, i)
    put('Walk', i, im)

for i in range(3):
    if i == 0:
        im = idle.copy()
        dr = d(im)
        dr.line([(62,48),(67,44),(70,43)], fill=INK, width=3)
        dr.line([(62,47),(68,43)], fill=(222,166,124,255), width=2)
    else:
        im = cast.copy()
        if i == 1:
            # Smaller initial sand cluster at the extended hand.
            im.paste((0,0,0,0), (80,27,112,60))
            dr = d(im)
            dr.ellipse((80,39,88,47), fill=SAND, outline=INK)
            dr.point((84,40), fill=LIGHT)
    put('Cast', i, im)

for i in range(2):
    im = shield.copy()
    if i == 0:
        # Rising wall, only its lower half has formed.
        im.paste((0,0,0,0), (67,19,105,51))
        dr = d(im)
        dr.polygon([(62,82),(65,65),(73,56),(84,53),(95,65),(101,83)],
                   fill=MID, outline=INK)
        dr.line([(69,78),(74,68),(82,66)], fill=LIGHT, width=2)
    put('Shield', i, im)

for i in range(3):
    im = cast.copy()
    im.paste((0,0,0,0), (76,25,112,61))
    dr = d(im)
    dr.line([(72,47),(77,55)], fill=INK, width=3)
    dr.line([(72,46),(77,54)], fill=(222,166,124,255), width=2)
    end = (82, 98, 111)[i]
    dr.polygon([(67,82),(72,76),(78,79),(83,74),(91,78),(end,73),
                (end,83)], fill=SAND, outline=INK)
    dr.line([(72,81),(82,79),(88,80),(min(end,104),77)], fill=LIGHT, width=2)
    for x in range(75, end, 8):
        dr.point((x,72+i%2), fill=LIGHT)
    put('Wave', i, im)

im = idle.copy()
dr = d(im)
dr.line([(60,45),(58,51),(62,54),(59,59)], fill=DARK, width=1)
dr.line([(48,51),(46,56),(49,59)], fill=DARK, width=1)
for x,y in [(62,54),(64,57),(47,63),(58,66)]:
    dr.rectangle((x,y,x+1,y+1), fill=LIGHT)
put('Hurt', 0, im)

for i in range(4):
    im = idle.copy()
    dr = d(im)
    # Cracked armour on face and visible arms, plus falling grains.
    dr.line([(62,27),(60,29),(63,31),(61,33)], fill=DARK)
    dr.line([(64,39),(61,42),(63,45)], fill=DARK)
    dr.line([(51,41),(49,44),(51,47)], fill=DARK)
    dr.point((64,30), fill=INK)
    for x,y in [(47+i%2,53+i),(62+(i%2),54+i),(45,59+i%2)]:
        dr.point((x,y), fill=MID)
        dr.point((x+1,y), fill=LIGHT)
    put('Cracked_Idle', i, im)

def beast_eye(im):
    dr = d(im)
    # Replace the checked pupil in the approved sample with a four-point star.
    dr.rectangle((74,24,80,30), fill=GOLD)
    dr.point((77,24), fill=INK)
    dr.point((77,25), fill=INK)
    dr.point((74,27), fill=INK)
    dr.point((75,27), fill=INK)
    dr.point((77,27), fill=INK)
    dr.point((79,27), fill=INK)
    dr.point((80,27), fill=INK)
    dr.point((77,29), fill=INK)
    dr.point((77,30), fill=INK)
    dr.point((76,26), fill=INK)
    dr.point((78,26), fill=INK)
    dr.point((76,28), fill=INK)
    dr.point((78,28), fill=INK)

for i in range(4):
    im = beast.copy()
    beast_eye(im)
    dr = d(im)
    # Moving tail tip and breathing dust. Feet remain at y=100.
    for n in range(i):
        x = 21+n*6
        dr.point((x,39+(i%2)*2), fill=LIGHT)
        dr.point((x+1,40+(i%2)*2), fill=SAND)
    if i in (1,2):
        dr.line([(95,44),(100,43),(103,45)], fill=LIGHT, width=2)
    put('Beast_Idle', i, im)

for i in range(3):
    im = beast.copy()
    beast_eye(im)
    dr = d(im)
    if i == 0:
        dr.polygon([(110,49),(126,40),(141,38),(150,43),(145,49),
                    (128,54)], fill=MID, outline=INK)
        dr.line([(119,47),(138,43)], fill=LIGHT, width=2)
    elif i == 1:
        dr.polygon([(112,48),(133,43),(157,47),(170,53),(167,59),
                    (150,56),(129,60)], fill=SAND, outline=INK)
        dr.line([(127,49),(149,50),(165,54)], fill=LIGHT, width=2)
        dr.line([(146,54),(162,57)], fill=MARK, width=2)
    else:
        dr.polygon([(111,51),(131,53),(145,64),(144,72),(136,68),
                    (121,59)], fill=MID, outline=INK)
        dr.line([(122,56),(138,66)], fill=LIGHT, width=2)
    put('Beast_Arm', i, im)

for i in range(3):
    im = beast.copy()
    beast_eye(im)
    dr = d(im)
    if i == 0:
        dr.line([(87,35),(93,37),(98,36)], fill=LIGHT, width=2)
    elif i == 1:
        dr.polygon([(86,34),(94,31),(99,35),(98,41),(89,41)],
                   fill=INK)
        dr.ellipse((119,27,149,57), fill=SAND, outline=INK, width=2)
        dr.ellipse((126,33,143,51), fill=LIGHT, outline=MID, width=2)
        dr.arc((112,26,155,58), 170, 330, fill=HIGHLIGHT, width=2)
        for x,y in [(109,37),(115,28),(153,41),(157,47)]:
            dr.rectangle((x,y,x+2,y+1), fill=MID)
    else:
        dr.line([(87,35),(94,34),(98,36)], fill=MID, width=2)
        for x,y in [(110,35),(119,30),(128,41)]:
            dr.point((x,y), fill=LIGHT)
    put('Beast_Bullet', i, im)

actions = ['Idle','Walk','Cast','Shield','Wave','Hurt','Cracked_Idle',
           'Beast_Idle','Beast_Arm','Beast_Bullet']
groups = {a:[im for aa,_,im in frames if aa == a] for a in actions}
row_h = 140
cell_w = 182
label_w = 112
preview = Image.new('RGB',(label_w + cell_w*8,row_h*len(actions)),(245,245,245))
pd = ImageDraw.Draw(preview)
for row,a in enumerate(actions):
    pd.text((5,row*row_h+8),a,fill=(25,25,25))
    for j,im in enumerate(groups[a]):
        for bg in range(2):
            x = label_w + (j*2+bg)*cell_w
            y = row*row_h+22
            pd.rectangle((x,y,x+cell_w-4,y+110),fill=(37,42,49) if bg else (216,216,216))
            preview.paste(im,(x+3,y+3),im)
            pd.text((x+3,y+112),f'{j} {"dark" if bg else "light"}',fill=(30,30,30))
preview.save(ROOT/'preview.png')

align = Image.new('RGBA',(label_w+176*len(actions),130),(0,0,0,0))
ad = ImageDraw.Draw(align)
for j,a in enumerate(actions):
    x = label_w+j*176
    center = 64 if a.startswith('Beast_') else 56
    foot = 100 if a.startswith('Beast_') else 84
    ad.line((x+center,0,x+center,103),fill=(255,0,0,140))
    ad.line((x,foot,x+175,foot),fill=(255,0,0,140))
    for k,im in enumerate(groups[a]):
        tint = im.copy()
        alpha = tint.getchannel('A').point(lambda v: 0 if v == 0 else max(70,180//len(groups[a])))
        tint.putalpha(alpha)
        align.alpha_composite(tint,(x,0))
    ad.text((x+2,109),a,fill=(0,0,0,255))
align.save(ROOT/'alignment-overlay.png')
print(f'{len(frames)} frames')
