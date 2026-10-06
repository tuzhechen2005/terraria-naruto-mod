from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[2]
BASE = ROOT / 'art/deliveries/npc-hires-sample-v2b'
GUIDE = ROOT / 'art/reference/terraria/NPC_22_x4.png'

O = '#211b29'
SK = '#f5aa75'; SL = '#ffd09a'; SD = '#bd714d'; SC = '#955b51'
WH = '#fff5df'; WS = '#d9d3cc'; WD = '#a6a6b4'
NV = '#243657'; NL = '#36517b'; ND = '#172541'
GR = '#596e45'; GL = '#829258'; GD = '#3e4d38'

def poly(d, pts, fill, outline=O):
    d.polygon(pts, fill=fill)
    if outline: d.line(pts + [pts[0]], fill=outline, width=1)

def line(d, pts, fill, width=1): d.line(pts, fill=fill, width=width)
def rect(d, box, fill, outline=None): d.rectangle(box, fill=fill, outline=outline)

def body(name):
    old = Image.open(BASE / f'{name}_Idle.png').convert('RGBA')
    im = Image.new('RGBA', (80,80))
    # A narrower torso and legs, retained from the approved v2b color-block style.
    crop = old.crop((20,39,66,77)).resize((32,38), Image.Resampling.NEAREST)
    im.alpha_composite(crop, (24,39))
    return im, ImageDraw.Draw(im)

def tazuna():
    im,d=body('Tazuna')
    # Back of head and hair; forehead projects to the right.
    poly(d,[(31,22),(35,17),(45,16),(51,19),(53,25),(54,32),(50,37),(41,39),(33,36),(29,30)],WS)
    poly(d,[(36,23),(46,21),(51,24),(51,34),(47,37),(38,36),(34,31)],SK)
    rect(d,(41,23,49,26),SL)
    # The rear ear and sideburn reinforce the direction.
    poly(d,[(32,27),(36,27),(37,33),(34,35),(31,32)],SK)
    rect(d,(33,29,34,31),SD)
    poly(d,[(29,23),(31,19),(36,17),(39,18),(43,16),(48,18),(51,19),(54,23),(51,25),(48,22),(43,23),(38,21),(33,24),(30,28)],WH)
    line(d,[(31,23),(34,21)],WD)
    line(d,[(42,18),(47,19)],WS)
    # Near and far eyes both sit on the right side of the face.
    d.ellipse((40,26,44,31),outline=O)
    d.ellipse((46,25,51,31),outline=O)
    line(d,[(44,27),(46,27)],O)
    rect(d,(42,28,42,29),O)
    rect(d,(49,27,49,29),O)
    line(d,[(39,25),(43,25)],SC)
    line(d,[(47,24),(50,24)],SC)
    poly(d,[(51,29),(54,30),(55,32),(52,33)],SD)
    # White moustache, clean mouth gap, full beard.
    poly(d,[(36,32),(40,33),(45,34),(50,32),(53,33),(54,35),(50,39),(43,40),(35,37)],WH)
    line(d,[(39,34),(44,35)],WS)
    line(d,[(47,35),(51,34)],WS)
    line(d,[(43,37),(48,37)],O)
    return im

def iruka():
    im,d=body('Iruka')
    # Swept ponytail behind the head, while the face points right.
    poly(d,[(33,23),(27,21),(22,19),(27,19),(24,17),(29,18),(28,16),(34,19),(38,23)],O)
    poly(d,[(31,21),(27,18),(33,20),(35,22)],'#343039',None)
    poly(d,[(32,23),(37,18),(47,18),(52,21),(54,27),(53,34),(49,38),(38,39),(32,35),(30,29)],O)
    poly(d,[(36,24),(48,22),(51,25),(52,33),(49,37),(40,37),(35,33)],SK)
    rect(d,(42,25,49,28),SL)
    poly(d,[(32,27),(36,27),(38,32),(35,34),(32,32)],SK)
    rect(d,(34,29,35,30),SD)
    poly(d,[(31,24),(35,19),(40,18),(47,19),(52,22),(50,25),(43,23),(37,25),(33,27)],O)
    poly(d,[(34,23),(40,21),(50,22),(52,25),(35,27)],ND)
    poly(d,[(42,21),(49,21),(50,25),(41,25)],WD)
    line(d,[(43,22),(47,22)],WH)
    # Two compact eyes, nose, seven-pixel scar across the bridge, happy mouth.
    line(d,[(40,28),(43,27)],O)
    line(d,[(47,27),(50,28)],O)
    rect(d,(40,29,43,31),WH)
    rect(d,(47,29,50,31),WH)
    rect(d,(42,29,42,30),O)
    rect(d,(49,29,49,30),O)
    line(d,[(44,33),(50,33)],SC)
    rect(d,(51,31,53,32),SD)
    line(d,[(44,36),(47,37),(50,35)],O)
    return im

def kakashi():
    im,d=body('Kakashi')
    # Spikes fan toward the rear (left); peak-to-chin height 24 pixels.
    poly(d,[(29,24),(24,22),(29,20),(27,18),(34,20),(33,16),(39,19),(42,15),(46,19),(49,16),(51,21),(55,19),(53,28),(52,35),(47,38),(36,38)],WS)
    poly(d,[(30,23),(36,18),(39,23),(43,17),(46,23),(50,19),(51,26),(35,27)],WH,None)
    line(d,[(33,21),(38,24)],WD)
    line(d,[(44,20),(47,24)],WD)
    # Far ear and skin. The headband slopes down to cover the rear eye only.
    poly(d,[(34,26),(43,23),(50,25),(53,28),(52,35),(47,38),(37,37),(32,32)],SK)
    poly(d,[(32,28),(36,27),(38,32),(35,34),(32,32)],SK)
    rect(d,(34,29,35,31),SD)
    poly(d,[(31,27),(36,24),(44,23),(48,24),(47,27),(39,30),(33,31)],ND)
    poly(d,[(36,25),(43,23),(46,24),(42,27),(37,28)],WD)
    line(d,[(38,25),(42,24)],WH)
    # One visible, half-open eye on the right; near eye stays above the mask.
    line(d,[(45,28),(50,28)],O)
    rect(d,(46,29,50,30),WH)
    rect(d,(48,29,49,30),O)
    poly(d,[(35,32),(43,32),(48,31),(51,32),(53,34),(50,38),(39,39),(34,36)],ND)
    line(d,[(49,32),(52,33),(53,34)],NL)
    line(d,[(39,36),(48,37)],NV)
    return im

names=['Tazuna','Iruka','Kakashi']
images=[tazuna(),iruka(),kakashi()]
for name,im in zip(names,images): im.save(HERE/f'{name}_Idle.png')

# Identical 1x and 3x comparisons, including the guide at matched character height.
bg=(35,45,64,255)
guide=Image.open(GUIDE).convert('RGBA')
guide=guide.crop(guide.getbbox())
guide=guide.resize((round(guide.width*61/guide.height),61),Image.Resampling.NEAREST)
preview=Image.new('RGBA',(1000,460),bg); d=ImageDraw.Draw(preview)
for i,(name,im) in enumerate(zip(names,images)):
    x=50+250*i
    d.text((x,10),name,fill=WH)
    preview.alpha_composite(im,(x+45,35))
    d.line((x,112,x+190,112),fill=WS)
    preview.alpha_composite(im.resize((240,240),Image.Resampling.NEAREST),(x,150))
    d.line((x,380,x+240,380),fill=WS)
x=800;d.text((x,10),'Guide',fill=WH)
preview.alpha_composite(guide,(x+55,112-guide.height))
preview.alpha_composite(guide.resize((guide.width*3,guide.height*3),Image.Resampling.NEAREST),(x+15,380-guide.height*3))
d.line((x,112,x+180,112),fill=WS);d.line((x,380,x+180,380),fill=WS)
preview.convert('RGB').save(HERE/'preview.png')

heads=Image.new('RGBA',(800,290),bg); h=ImageDraw.Draw(heads)
for i,(name,im) in enumerate(zip(names,images)):
    x=12+270*i
    h.text((x,7),name,fill=WH)
    crop=im.crop((24,11,57,43)).resize((264,256),Image.Resampling.NEAREST)
    heads.alpha_composite(crop,(x,26))
    for gx in range(x,x+265,8): h.line((gx,26,gx,281),fill=(116,125,142,120))
    for gy in range(26,283,8): h.line((x,gy,x+263,gy),fill=(116,125,142,120))
heads.convert('RGB').save(HERE/'faces_8x.png')
