"""Build Academy Training clothes on Terraria's 2x pixel grid.

The generated costume concept in this folder sets the design and palette. The
Terraria references are read only for pose positions, panel placement, the
comparison preview, and the outside-template pixel counts.
"""
from pathlib import Path
from PIL import Image, ImageDraw, ImageOps

HERE = Path(__file__).resolve().parent
OUT = HERE.parent
ROOT = HERE.parents[3]
ARMOR = ROOT / 'art/reference/terraria/armor'
PLAYER = ROOT / 'art/reference/terraria/player'

INK = '#25272b'
IVORY = ['#786f60', '#a69d89', '#d3cab1', '#eee5ce', '#fff3da']
GREEN = ['#35433d', '#52665a', '#758a70', '#9aaa88', '#b7c3a3']
BROWN = ['#332923', '#52382c', '#754c35', '#956a47', '#b48b60']
BLUE = ['#303841', '#414d59', '#596776', '#758493', '#929ea1']
STRAW = ['#6a5136', '#98734a', '#c39a60', '#e0bd79', '#f2d18a']
SKIN = '#d8a584'

def canvas():
    return Image.new('RGBA', (20, 28))

def poly(d, pts, fill, outline=INK):
    d.polygon(pts, fill=fill)
    if outline: d.line(pts + [pts[0]], fill=outline, width=1, joint='curve')

def line(d, pts, fill, width=1):
    d.line(pts, fill=fill, width=width)

def full(im):
    return im.resize((40, 56), Image.Resampling.NEAREST)

def head(frame):
    im=canvas(); d=ImageDraw.Draw(im)
    shift=-1 if frame in (7,8,9,14,15,16) else 0
    swing=(-1,0,1,1,0,-1,1,-1,0,1,1,0,-1,0,1,-1,0,0,0,0)[frame]
    lift= -2 if frame in (10,11,12,13) else 0
    # Narrow forehead wrap. Hair and all facial pixels remain uncovered.
    poly(d,[(5,8+shift),(7,7+shift),(14,7+shift),(16,8+shift),(16,10+shift),(5,10+shift)],IVORY[2])
    line(d,[(6,8+shift),(14,8+shift)],IVORY[4]); line(d,[(6,10+shift),(14,10+shift)],IVORY[0])
    poly(d,[(14,8+shift),(16,8+shift),(17,9+shift),(16,11+shift),(14,10+shift)],IVORY[3])
    d.point((15,9+shift),fill=BROWN[2])
    # Separate, animated short ties at the back-right of the head.
    poly(d,[(16,10+shift),(17,10+shift),(18+swing,13+shift+lift),(17+swing,15+shift+lift),(16+swing,13+shift+lift)],IVORY[2])
    poly(d,[(15,10+shift),(16,11+shift),(16+swing,14+shift+lift),(15+swing,16+shift+lift),(14+swing,14+shift+lift)],IVORY[3])
    return full(im)

def torso(variant=0):
    im=canvas();d=ImageDraw.Draw(im)
    hem=24 if variant==0 else 23
    # Custom broad short-coat silhouette, two flared lower panels.
    poly(d,[(7,12),(13,12),(16,14),(16,19),(17,hem),(13,hem),(11,23),(9,hem),(4,hem),(5,19),(5,14)],GREEN[2])
    poly(d,[(5,15),(7,13),(10,13),(9,19),(6,21),(5,20)],GREEN[3],None)
    poly(d,[(13,13),(16,15),(16,20),(14,22),(12,20)],GREEN[1],None)
    line(d,[(6,17),(6,21),(5,23)],GREEN[0]);line(d,[(14,16),(15,19),(16,22)],GREEN[0])
    line(d,[(7,22),(8,23)],GREEN[4]);line(d,[(13,22),(14,23)],GREEN[4])
    # Dark repaired shoulder patch with visible stitches.
    poly(d,[(6,13),(9,12),(10,14),(8,16),(6,16)],GREEN[0],None)
    for x,y in ((7,13),(8,14),(7,15)):d.point((x,y),fill=IVORY[2])
    # Left overlap is a light diagonal band, running viewer-right to lower-left.
    poly(d,[(10,13),(12,13),(9,18),(7,20),(6,19)],IVORY[1])
    line(d,[(11,13),(8,18),(7,19)],IVORY[4]);line(d,[(8,14),(11,18)],GREEN[0])
    # Wrapped broad sash, off-center knot, animated trailing ends.
    poly(d,[(5,19),(16,19),(16,22),(5,22)],BROWN[2])
    line(d,[(6,20),(15,20)],BROWN[4]);line(d,[(5,22),(16,22)],BROWN[0])
    poly(d,[(13,19),(15,19),(16,20),(15,22),(13,21)],BROWN[3])
    tail=variant%3-1
    poly(d,[(15,21),(16,22),(16+tail,27),(14+tail,27),(14,23)],BROWN[2])
    line(d,[(15,23),(15+tail,26)],BROWN[4])
    poly(d,[(13,21),(14,22),(13-tail,26),(12-tail,25)],BROWN[1])
    return full(im)

def sleeve(box, frame, near=False):
    im=canvas();d=ImageDraw.Draw(im)
    x0,y0,x1,y1=box
    # Coordinates from reference panel locate the arm, but the silhouette is new.
    x0=max(1,x0//2-1);x1=min(19,(x1+1)//2+1)
    y0=max(1,y0//2);y1=min(27,(y1+1)//2)
    mid=(x0+x1)//2
    if y1-y0<5:y1=y0+5
    poly(d,[(x0+1,y0),(x1-2,y0),(x1,y0+2),(x1-1,y0+4),(x0,y0+4)],GREEN[2])
    line(d,[(x0+1,y0+1),(mid,y0+1)],GREEN[4]);line(d,[(x1-2,y0+2),(x1-1,y0+3)],GREEN[0])
    poly(d,[(x0,y0+4),(x1-1,y0+4),(x1-1,y0+6),(x0+1,y0+6)],GREEN[3])
    line(d,[(x0+1,y0+5),(x1-2,y0+5)],GREEN[4])
    if y1>y0+8:
        poly(d,[(x0+2,y0+6),(x1-2,y0+6),(x1-2,y1),(x0+2,y1)],IVORY[2])
        for yy in range(y0+7,y1,2):line(d,[(x0+2,yy),(x1-3,yy+1)],IVORY[0])
        line(d,[(x0+3,y0+7),(x0+3,y1-1)],IVORY[4])
    if near: d.point((mid,y0+2),fill=GREEN[4])
    return full(im)

def legs(frame):
    im=canvas();d=ImageDraw.Draw(im)
    shift= -1 if frame in (7,8,9,14,15,16) else 0
    pose=frame%5
    l=-1 if pose in (1,2) else 0
    r=1 if pose in (3,4) else 0
    if frame in (10,11,12,13):l=-2;r=2
    # Tunic covers waist; trousers start near y=20, lower than torso hem.
    poly(d,[(6,20+shift),(14,20+shift),(15+r,23+shift),(14+r,25+shift),(11+r,25+shift),(10,23+shift),(9+l,25+shift),(6+l,25+shift),(5+l,23+shift)],BLUE[2])
    line(d,[(7,21+shift),(10,22+shift)],BLUE[4]);line(d,[(12,21+shift),(14,22+shift)],BLUE[4])
    d.point((8+l,23+shift),fill=BLUE[3]);d.point((13+r,23+shift),fill=BLUE[3])
    # Crossed shin bandages with two brown straw open-toe sandals.
    for x in (7+l,12+r):
        poly(d,[(x-1,24+shift),(x+1,24+shift),(x+1,26+shift),(x-1,26+shift)],IVORY[2])
        line(d,[(x-1,24+shift),(x+1,25+shift)],IVORY[0]);line(d,[(x+1,24+shift),(x-1,25+shift)],IVORY[4])
        poly(d,[(x-2,26+shift),(x+2,26+shift),(x+3,27+shift),(x-2,27+shift)],STRAW[2])
        line(d,[(x-1,26+shift),(x+1,27+shift)],STRAW[0]);d.point((x+2,26+shift),fill=SKIN)
    return full(im)

def build():
    headsheet=Image.new('RGBA',(40,1120));legsheet=Image.new('RGBA',(40,1120))
    body=Image.new('RGBA',(360,224))
    ref=Image.open(ARMOR/'Armor_1.png').convert('RGBA')
    for f in range(20):
        headsheet.alpha_composite(head(f),(0,f*56));legsheet.alpha_composite(legs(f),(0,f*56))
    for row in range(4):
        for col in range(9):
            box=ref.crop((col*40,row*56,col*40+40,row*56+56)).getchannel('A').getbbox()
            if box is None:continue
            if col in (0,1) and row in (0,2):part=torso(row+col)
            elif col in (0,1) and row in (1,3):part=sleeve(box,row+col,True)
            else:part=sleeve(box,row+col,col>4)
            body.alpha_composite(part,(col*40,row*56))
    for name,im in [('AcademyTrainingHead_Head.png',headsheet),('AcademyTrainingBody_Body.png',body),('AcademyTrainingLegs_Legs.png',legsheet)]:im.save(OUT/name)
    # Icons are individual folded garment objects, not reduced equip frames.
    icons=[]
    h=canvas();d=ImageDraw.Draw(h);poly(d,[(3,9),(6,7),(16,7),(17,9),(16,12),(4,12)],IVORY[2]);line(d,[(4,9),(15,8)],IVORY[4]);poly(d,[(15,10),(17,11),(18,16),(16,17)],IVORY[2]);poly(d,[(14,11),(16,12),(15,18),(13,17)],IVORY[3]);icons.append(h)
    b=canvas();d=ImageDraw.Draw(b);poly(d,[(5,6),(8,5),(12,5),(15,6),(18,9),(16,13),(15,12),(15,20),(5,20),(5,12),(4,13),(2,9)],GREEN[2]);poly(d,[(5,6),(8,6),(8,9),(5,10)],GREEN[0],None);line(d,[(9,6),(12,9),(7,15)],IVORY[3],2);line(d,[(5,15),(15,15)],BROWN[2],2);poly(d,[(13,15),(15,15),(16,22),(14,22)],BROWN[2]);icons.append(b)
    l=canvas();d=ImageDraw.Draw(l);poly(d,[(5,5),(15,5),(16,8),(14,18),(12,20),(10,12),(8,20),(6,18),(4,8)],BLUE[2]);line(d,[(6,13),(9,14)],BLUE[4]);line(d,[(11,14),(14,13)],BLUE[4]);line(d,[(6,18),(8,19)],IVORY[3],2);line(d,[(12,19),(14,18)],IVORY[3],2);line(d,[(5,21),(8,21)],STRAW[3],2);line(d,[(12,21),(15,21)],STRAW[3],2);icons.append(l)
    small_icons=[]
    for name,im in zip(['AcademyTrainingHead.png','AcademyTrainingBody.png','AcademyTrainingLegs.png'],icons):
        piece=im.crop(im.getbbox());piece.thumbnail((14,14),Image.Resampling.NEAREST)
        small=Image.new('RGBA',(15,15));small.alpha_composite(piece,((15-piece.width)//2,(15-piece.height)//2))
        small=small.resize((30,30),Image.Resampling.NEAREST);small.save(OUT/name);small_icons.append(small)
    preview(headsheet,body,legsheet,small_icons)
    counts={}
    for new,old,refname in [(headsheet,Image.open(ARMOR/'Armor_Head_22.png').convert('RGBA'),'head'),(body,ref,'body'),(legsheet,Image.open(ARMOR/'Armor_Legs_1.png').convert('RGBA'),'legs')]:
        mask=old.getchannel('A'); count=0
        for y in range(new.height):
            for x in range(new.width):
                if new.getpixel((x,y))[3] and (y>=old.height or mask.getpixel((x,y))==0):count+=1
        counts[refname]=count
    print('outside reference masks:',counts)

def preview(headsheet,body,legsheet,icons):
    # Game-order comparison at representative standing, two walking, jumping poses.
    bg=Image.new('RGBA',(1550,410),'#202b35');d=ImageDraw.Draw(bg)
    hair=Image.open(PLAYER/'Player_Hair_1.png').convert('RGBA')
    face=Image.open(PLAYER/'Player_0_0.png').convert('RGBA')
    def tint(src, colors):
        dst=src.copy();px=dst.load()
        for yy in range(dst.height):
            for xx in range(dst.width):
                r,g,b,a=px[xx,yy]
                if a:
                    v=(r+g+b)//3
                    from PIL import ImageColor
                    c=ImageColor.getrgb(colors[min(4,v//52)])
                    px[xx,yy]=(c[0],c[1],c[2],a)
        return dst
    hair=tint(hair,['#211d1a','#49372b','#78573a','#9c774c','#c19b65'])
    face=tint(face,['#493025','#875842','#ad7959','#d6a47b','#f1c79b'])
    ninja_b=Image.open(ARMOR/'Armor_14.png').convert('RGBA')
    ninja_l=Image.open(ARMOR/'Armor_Legs_14.png').convert('RGBA')
    ninja_h=Image.open(ARMOR/'Armor_Head_22.png').convert('RGBA')
    copper_b=Image.open(ARMOR/'Armor_1.png').convert('RGBA')
    copper_l=Image.open(ARMOR/'Armor_Legs_1.png').convert('RGBA')
    choices=[('Academy',headsheet,body,legsheet),('Ninja',ninja_h,ninja_b,ninja_l),('Copper',None,copper_b,copper_l)]
    frames=(0,5,7,10)
    for ci,(label,hs,bs,ls) in enumerate(choices):
        xbase=20+ci*510;d.text((xbase,8),label+'  stand / walk A / walk B / jump',fill='white')
        for pi,frame in enumerate(frames):
            tile=Image.new('RGBA',(40,56))
            # Rear arm panel, trousers, torso, face/hair, headwear, front arm panel.
            tile.alpha_composite(bs.crop((320,0,360,56)))
            tile.alpha_composite(ls.crop((0,frame*56,40,frame*56+56)))
            tile.alpha_composite(bs.crop((0,0,40,56)))
            tile.alpha_composite(face.crop((0,frame*56,40,frame*56+56)))
            tile.alpha_composite(hair.crop((0,(frame%14)*56,40,(frame%14+1)*56)))
            if hs:tile.alpha_composite(hs.crop((0,frame*56,40,frame*56+56)))
            tile.alpha_composite(bs.crop((80,0,120,56)))
            bg.alpha_composite(tile,(xbase+pi*125,33))
            bg.alpha_composite(tile.resize((120,168),Image.Resampling.NEAREST),(xbase+pi*125,104))
    d=ImageDraw.Draw(bg);d.text((20,290),'Icons: 1x and 3x',fill='white')
    for i,icon in enumerate(icons):
        bg.alpha_composite(icon,(20+i*145,323))
        bg.alpha_composite(icon.resize((90,90),Image.Resampling.NEAREST),(55+i*145,303))
    bg.convert('RGB').save(OUT/'preview.png')

if __name__=='__main__':build()
