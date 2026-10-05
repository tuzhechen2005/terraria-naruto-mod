from PIL import Image, ImageDraw, ImageFont
from pathlib import Path

OUT = Path(__file__).parent
O = '#211b29'
SK = '#f5aa75'; SL = '#ffd09a'; SD = '#bd714d'; SC = '#955b51'
WH = '#fff5df'; WS = '#d9d3cc'; WD = '#a6a6b4'
NV = '#243657'; NL = '#36517b'; ND = '#172541'
GR = '#596e45'; GL = '#829258'; GD = '#3e4d38'

def new():
    im = Image.new('RGBA', (80, 80))
    return im, ImageDraw.Draw(im)

def poly(d, pts, fill, outline=O): d.polygon(pts, fill=fill); d.line(pts + [pts[0]], fill=outline, width=1)
def rect(d, xy, fill, outline=None): d.rectangle(xy, fill=fill, outline=outline)
def line(d, pts, fill, width=1): d.line(pts, fill=fill, width=width)

def tazuna():
    im,d=new()
    # Wide wrapped trouser legs and sandals.
    poly(d,[(29,55),(41,56),(41,66),(39,72),(29,72),(27,65)],'#414c4a')
    poly(d,[(42,55),(53,54),(55,65),(53,72),(43,72),(40,66)],'#414c4a')
    rect(d,(31,60,36,68),'#59605b');rect(d,(46,60,51,68),'#59605b')
    rect(d,(29,69,39,72),WS);rect(d,(44,69,54,72),WS)
    poly(d,[(28,72),(40,72),(41,75),(27,76)],'#554136')
    poly(d,[(43,72),(55,72),(58,75),(42,76)],'#554136')
    line(d,[(30,73),(39,73)],'#a7784f');line(d,[(46,73),(54,73)],'#a7784f')
    # Torso, rolled sleeve and prominent towel.
    poly(d,[(29,39),(48,39),(54,45),(53,57),(47,60),(28,58),(24,49)],'#ece0c4')
    poly(d,[(30,40),(36,41),(38,57),(29,57),(26,50)],'#302e2f')
    poly(d,[(43,40),(50,40),(54,46),(51,58),(43,58)],'#302e2f')
    rect(d,(31,47,35,56),'#504438');rect(d,(46,46,49,56),'#504438')
    poly(d,[(32,38),(36,38),(37,51),(34,55),(31,49)],WH)
    poly(d,[(44,38),(48,39),(49,51),(46,54),(44,47)],WH)
    line(d,[(34,43),(35,50)],WD);line(d,[(46,43),(47,50)],WD)
    poly(d,[(25,42),(30,43),(31,52),(27,56),(22,53),(21,47)],'#eee0c2')
    poly(d,[(51,42),(56,42),(60,49),(56,55),(52,52)],'#eee0c2')
    poly(d,[(23,52),(28,52),(29,57),(24,59),(21,55)],SK)
    poly(d,[(55,51),(60,50),(62,55),(59,59),(55,57)],SK)
    # Sake bottle at the right hand.
    poly(d,[(58,51),(62,51),(63,55),(62,64),(57,64),(56,55)],'#b18651')
    rect(d,(58,49,61,51),'#e4c696');rect(d,(58,56,61,61),'#e4c696')
    # Hair silhouette/main head: 24 pixels high, face about 16 pixels wide.
    poly(d,[(29,21),(34,18),(43,17),(49,18),(53,22),(54,32),(51,38),(35,39),(29,34),(27,28)],WS)
    poly(d,[(33,23),(44,21),(51,24),(52,34),(47,38),(34,36),(30,31)],SK)
    rect(d,(38,23,48,27),SL);rect(d,(43,33,49,36),SD)
    poly(d,[(27,23),(29,18),(34,17),(36,17),(41,18),(44,17),(48,18),(52,18),(54,23),(49,25),(46,22),(40,24),(34,22),(30,27)],WH)
    rect(d,(29,20,32,23),WS);rect(d,(43,17,46,19),WS);rect(d,(49,20,52,22),WS)
    # Two thin round frames; warm visible eyes behind them.
    d.ellipse((35,25,41,31),outline=O,width=1)
    d.ellipse((44,25,50,31),outline=O,width=1)
    line(d,[(41,27),(44,27)],O)
    rect(d,(38,27,39,28),WH);rect(d,(39,28,39,29),O)
    rect(d,(47,27,48,28),WH);rect(d,(48,28,48,29),O)
    line(d,[(35,24),(40,24)],'#715f5a');line(d,[(45,24),(49,24)],'#715f5a')
    rect(d,(51,29,53,30),SD) # small nose, no projecting column
    # Clean three-tone beard with exposed mouth.
    poly(d,[(31,31),(36,32),(39,34),(48,33),(52,31),(53,35),(51,39),(46,40),(38,40),(32,37)],WH)
    line(d,[(33,34),(38,35)],WS);line(d,[(47,35),(51,34)],WS)
    rect(d,(36,37,43,38),WD);line(d,[(38,36),(45,36)],O)
    return im

def iruka():
    im,d=new()
    # Narrow ninja trousers, wraps and blue sandals.
    poly(d,[(33,55),(42,55),(42,67),(39,72),(32,71)],NV)
    poly(d,[(44,55),(51,54),(52,70),(45,72),(42,65)],NV)
    rect(d,(34,60,39,66),NL);rect(d,(46,61,49,67),NL)
    rect(d,(32,68,40,71),WH);rect(d,(45,68,52,71),WH)
    line(d,[(33,69),(39,69)],WD);line(d,[(46,69),(51,69)],WD)
    poly(d,[(31,72),(40,72),(41,75),(30,76)],ND)
    poly(d,[(44,72),(53,72),(56,75),(43,76)],ND)
    rect(d,(34,73,38,74),SK);rect(d,(48,73,52,74),SK)
    # Scroll under arm and sleeves.
    poly(d,[(25,43),(31,42),(35,49),(31,57),(25,54),(22,49)],NV)
    poly(d,[(50,42),(56,44),(58,52),(54,57),(49,53)],NV)
    poly(d,[(24,52),(30,52),(31,58),(26,59),(23,56)],SK)
    poly(d,[(53,54),(58,53),(59,58),(54,59)],SK)
    poly(d,[(53,47),(61,45),(63,49),(57,55),(53,54)],WH)
    rect(d,(61,45,63,49),'#a9674d');line(d,[(56,51),(61,47)],WS)
    # Vest: partitioned pockets, collar.
    poly(d,[(31,39),(47,39),(54,45),(52,58),(30,58),(28,45)],GR)
    rect(d,(32,45,49,55),GL)
    line(d,[(41,43),(41,57)],GD);line(d,[(32,49),(49,49)],GD)
    rect(d,(33,50,39,54),GR,O);rect(d,(43,50,49,54),GR,O)
    line(d,[(36,51),(36,54)],GD);line(d,[(46,51),(46,54)],GD)
    poly(d,[(30,39),(35,38),(37,45),(32,47),(29,44)],GD)
    poly(d,[(45,38),(50,39),(53,44),(48,47),(45,45)],GD)
    # Raised brush-tip ponytail, behind enlarged head.
    poly(d,[(33,22),(27,19),(21,16),(27,16),(21,12),(31,15),(29,11),(36,17),(39,23)],O)
    poly(d,[(30,18),(24,15),(32,17),(34,20)],'#343039')
    # Main head 23 pixels, face 16 wide.
    poly(d,[(32,20),(39,17),(48,18),(53,22),(54,31),(50,38),(35,39),(31,33)],O)
    poly(d,[(35,23),(48,22),(52,26),(52,34),(48,38),(36,37),(33,32)],SK)
    rect(d,(37,24,46,28),SL);rect(d,(37,34,46,36),SD)
    poly(d,[(31,22),(35,18),(41,17),(47,19),(51,21),(49,24),(42,22),(36,25),(33,27)],O)
    rect(d,(34,27,35,30),SD)
    # Headband and lit metal plate.
    poly(d,[(33,22),(37,20),(51,21),(53,25),(34,26)],ND)
    rect(d,(39,21,49,24),WD,O);rect(d,(41,21,45,21),WH)
    line(d,[(44,23),(46,22),(48,23)],O)
    # Brows, eyes with whites and pupils, scar, smiling mouth.
    line(d,[(37,28),(41,27)],O);line(d,[(45,27),(49,28)],O)
    rect(d,(37,29,41,31),WH);rect(d,(45,29,49,31),WH)
    rect(d,(39,29,40,30),O);rect(d,(47,29,48,30),O)
    rect(d,(39,29,39,29),WH);rect(d,(47,29,47,29),WH)
    line(d,[(35,33),(50,33)],SC)
    rect(d,(51,31,52,32),SD)
    line(d,[(41,36),(46,37),(49,36)],O)
    return im

def kakashi():
    im,d=new()
    poly(d,[(33,55),(42,55),(42,67),(39,72),(32,71)],NV)
    poly(d,[(44,55),(51,54),(52,70),(45,72),(42,65)],NV)
    rect(d,(34,61,39,66),NL);rect(d,(46,61,49,67),NL)
    rect(d,(32,68,40,71),WS);rect(d,(45,68,52,71),WS)
    poly(d,[(31,72),(40,72),(41,75),(30,76)],ND)
    poly(d,[(44,72),(53,72),(56,75),(43,76)],ND)
    rect(d,(34,73,38,74),SK);rect(d,(48,73,52,74),SK)
    # Arms and little orange book.
    poly(d,[(25,42),(31,41),(34,49),(31,56),(26,56),(22,49)],NV)
    poly(d,[(49,41),(55,43),(57,52),(54,56),(49,54)],NV)
    poly(d,[(24,52),(30,52),(31,57),(25,58)],ND)
    poly(d,[(53,51),(58,51),(59,57),(55,58)],ND)
    poly(d,[(55,46),(62,43),(65,45),(63,55),(58,55)],'#da6b2d')
    line(d,[(57,47),(62,45)],'#ffb165');line(d,[(62,46),(61,53)],'#9b4729')
    # Vest, pockets, collar.
    poly(d,[(30,39),(48,39),(54,44),(52,58),(30,58),(27,45)],GR)
    rect(d,(32,45,49,55),GL)
    line(d,[(41,43),(41,57)],GD);line(d,[(32,49),(49,49)],GD)
    rect(d,(33,50,39,54),GR,O);rect(d,(43,50,49,54),GR,O)
    line(d,[(36,51),(36,54)],GD);line(d,[(46,51),(46,54)],GD)
    poly(d,[(30,39),(35,38),(37,45),(32,47),(29,44)],GD)
    poly(d,[(45,38),(50,39),(53,44),(48,47),(45,45)],GD)
    # Spiky silver-white silhouette: main head 16..39 with top spikes.
    poly(d,[(29,23),(24,20),(30,20),(28,16),(35,19),(36,13),(41,18),(46,11),(48,18),(55,14),(54,21),(60,19),(55,27),(53,36),(46,39),(34,37)],WS)
    poly(d,[(32,21),(38,17),(43,22),(48,16),(49,23),(55,19),(51,26),(34,27)],WH)
    line(d,[(33,21),(38,23)],WD);line(d,[(43,20),(46,23)],WD)
    # 16px wide visible face; diagonal headband covering left eye.
    poly(d,[(34,25),(47,23),(53,27),(52,35),(47,38),(36,36),(32,31)],SK)
    rect(d,(41,26,48,29),SL)
    poly(d,[(30,27),(35,24),(53,23),(54,27),(33,30)],ND)
    poly(d,[(36,25),(46,23),(49,25),(39,28)],WD)
    line(d,[(40,25),(45,24)],WH)
    # The exposed right eye has a lid, white and pupil; mask's nose fold is visible.
    line(d,[(44,29),(50,29)],O)
    rect(d,(45,30,50,31),WH)
    rect(d,(47,30,48,31),O)
    rect(d,(47,30,47,30),WH)
    poly(d,[(34,32),(44,33),(50,32),(53,34),(51,38),(38,39),(33,36)],ND)
    line(d,[(47,33),(50,34),(52,34)],NL)
    line(d,[(40,36),(49,37)],NV)
    return im

names=['Tazuna','Iruka','Kakashi']
images=[tazuna(),iruka(),kakashi()]
for name,im in zip(names,images): im.save(OUT/f'{name}_Idle.png')

# Review sheet: exact 1x and nearest-neighbour 3x, with vanilla Guide at 1.36x.
bg=(35,45,64,255)
preview=Image.new('RGBA',(1000,460),bg)
d=ImageDraw.Draw(preview)
guide=Image.open(OUT/'../../reference/terraria/NPC_22_x4.png').convert('RGBA')
b=guide.getbbox();guide=guide.crop(b)
guide=guide.resize((max(1,round(guide.width*0.34)),max(1,round(guide.height*0.34))),Image.Resampling.NEAREST)
for i,(name,im) in enumerate(zip(names,images)):
    x=50+250*i
    d.text((x,10),name,fill=WH)
    preview.alpha_composite(im,(x+45,35))
    d.line((x,112,x+190,112),fill=WS)
    preview.alpha_composite(im.resize((240,240),Image.Resampling.NEAREST),(x,150))
    d.line((x,380,x+240,380),fill=WS)
x=800;d.text((x,10),'Guide @1.36',fill=WH)
preview.alpha_composite(guide,(x+50,112-guide.height))
preview.alpha_composite(guide.resize((guide.width*3,guide.height*3),Image.Resampling.NEAREST),(x,380-guide.height*3))
d.line((x,112,x+180,112),fill=WS);d.line((x,380,x+180,380),fill=WS)
preview.convert('RGB').save(OUT/'preview.png')

heads=Image.new('RGBA',(800,290),bg);h=ImageDraw.Draw(heads)
for i,(name,im) in enumerate(zip(names,images)):
    x=12+270*i
    h.text((x,7),name,fill=WH)
    crop=im.crop((24,11,57,43)).resize((264,256),Image.Resampling.NEAREST)
    heads.alpha_composite(crop,(x,26))
    for gx in range(x,x+265,8):h.line((gx,26,gx,281),fill=(116,125,142,120))
    for gy in range(26,283,8):h.line((x,gy,x+263,gy),fill=(116,125,142,120))
heads.convert('RGB').save(OUT/'faces_8x.png')
