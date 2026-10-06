"""Build the six hand-pixeled Genin armor sprites and a game-scale comparison.

The generated concept PNG is a design reference. Terraria sheets are drawn on a
20x28 logical grid and enlarged 2x, using the vanilla sheets for registration.
"""
from pathlib import Path
from PIL import Image, ImageDraw, ImageOps

HERE = Path(__file__).resolve().parent
OUT = HERE.parent
REF = HERE.parents[2] / 'reference' / 'terraria'
ARM = REF / 'armor'
PLY = REF / 'player'

INK = '#171725'
NAVY = '#182951'
NAVYL = '#294477'
NAVYHI = '#3b6195'
BLUE0 = '#172f69'
BLUE1 = '#214a96'
BLUE2 = '#3169bc'
BLUE3 = '#4b85d4'
BLUE4 = '#75a5df'
WHITE0 = '#657486'
WHITE1 = '#a3afba'
WHITE2 = '#d8dce0'
WHITE3 = '#faf7ed'
MESH = '#303946'
BROWN0 = '#35251e'
BROWN1 = '#67412a'
BROWN2 = '#98643a'
BROWN3 = '#c28a51'
TROU0 = '#524c4b'
TROU1 = '#807770'
TROU2 = '#aaa096'
TROU3 = '#cdc0ad'
SKIN = '#e5a077'


def tile():
    return Image.new('RGBA', (20, 28))


def poly(d, points, fill, outline=INK):
    d.polygon(points, fill=fill)
    if outline:
        d.line(points + [points[0]], fill=outline, width=1)


def rect(d, box, fill, outline=None):
    d.rectangle(box, fill=fill, outline=outline)


def up(im):
    return im.resize((im.width * 2, im.height * 2), Image.Resampling.NEAREST)


def bbox_for(image, c, r):
    b = image.crop((c*40,r*56,c*40+40,r*56+56)).getchannel('A').getbbox()
    if not b:
        return None
    return (b[0]//2,b[1]//2,(b[2]+1)//2,(b[3]+1)//2)


def head_frame(i):
    im=tile(); d=ImageDraw.Draw(im)
    dy=-1 if i in (7,8,9,14,15,16) else 0
    sway=(-1,0,1,0)[i%4] if i not in (7,8,9,14,15,16) else -2
    # Only the forehead band: eyes, face and crown of hair remain open.
    poly(d,[(5,7+dy),(7,6+dy),(14,6+dy),(16,7+dy),(16,10+dy),(5,10+dy)],NAVY)
    rect(d,(5,7+dy,15,7+dy),NAVYHI)
    poly(d,[(12,8+dy),(15,8+dy),(16+sway,11+dy),(15+sway,14+dy),(13+sway,12+dy)],NAVYL)
    rect(d,(7,7+dy,13,10+dy),WHITE1,INK)
    rect(d,(8,7+dy,12,7+dy),WHITE3)
    rect(d,(8,8+dy,8,8+dy),WHITE3)
    rect(d,(12,9+dy,12,9+dy),WHITE0)
    # A small curled leaf engraving and two rivets.
    d.line([(9,9+dy),(10,8+dy),(11,8+dy),(11,9+dy),(10,9+dy)],fill=NAVY,width=1)
    d.point((7,9+dy),fill=WHITE3);d.point((13,9+dy),fill=WHITE3)
    return im


def torso(variant=0):
    im=tile();d=ImageDraw.Draw(im)
    shift=1 if variant else 0
    poly(d,[(7+shift,12),(13+shift,12),(16,14),(16,20),(14,22),(6,22),(5,20),(5,14)],BLUE1)
    poly(d,[(6,14),(8,13),(8,19),(7,21),(6,20)],BLUE3,None)
    poly(d,[(13,13),(15,14),(15,19),(13,21)],BLUE0,None)
    d.line([(8,14),(9,19),(8,21)],fill=BLUE4,width=1)
    d.line([(13,14),(12,18),(13,20)],fill=BLUE2,width=1)
    rect(d,(9,12,12,14),MESH,INK)
    d.point((10,13),fill=WHITE0); d.point((12,13),fill=WHITE0)
    poly(d,[(8,12),(10,12),(11,14),(10,15),(8,14)],WHITE2)
    poly(d,[(12,12),(14,12),(13,14),(11,15),(10,14)],WHITE3)
    # Diagonal chest strap and dangling knot intentionally grow outside copper.
    d.line([(7,14),(14,20)],fill=BROWN0,width=3)
    d.line([(7,14),(14,20)],fill=BROWN2,width=2)
    d.line([(8,14),(13,19)],fill=BROWN3,width=1)
    rect(d,(12,19,15,20),BROWN1,INK)
    d.line([(14,20),(15+variant,24)],fill=BROWN2,width=2)
    rect(d,(6,20,14,21),BLUE0)
    d.line([(7,21),(13,21)],fill=BLUE3,width=1)
    # Two wrapped kunai handles, dark steel blades, loop rings.
    for x,y in ((11,16),(13,17)):
        rect(d,(x,y,x,y+2),WHITE2,INK)
        d.point((x,y+1),fill=BROWN2)
        d.point((x,y-1),fill=WHITE3)
        d.line([(x,y+3),(x-1,y+4)],fill=WHITE0,width=1)
    return im


def arm(box, frame, rear=False):
    im=tile();d=ImageDraw.Draw(im)
    x0,y0,x1,y1=box
    cx=(x0+x1)//2
    if x1-x0<4: x0=cx-2;x1=cx+2
    x0=max(1,x0-1);x1=min(19,x1+1)
    y0=max(5,y0-1);y1=min(27,y1+1)
    if y1-y0<5: y1=y0+6
    # Separate sleeve, mesh underlayer, cuff and wrapped forearm.
    mid=y0+max(3,(y1-y0)//2)
    poly(d,[(x0+1,y0),(x1-2,y0),(x1,mid-1),(x1-1,mid+1),(x0,mid+1),(x0-1,mid-1)],BLUE1)
    d.line([(x0+2,y0+1),(x0+1,mid-2)],fill=BLUE3,width=1)
    d.line([(x1-2,y0+1),(x1-2,mid-1)],fill=BLUE0,width=1)
    rect(d,(x0,mid,x1-1,mid+1),WHITE2,INK)
    rect(d,(x0+1,mid+2,x1-2,min(y1-1,mid+3)),MESH,INK)
    if y1>mid+4:
        poly(d,[(x0+2,mid+4),(x1-2,mid+4),(x1-1,y1-1),(x0+1,y1-1)],WHITE1)
        for yy in range(mid+5,y1-1,2):
            d.line([(x0+2,yy),(x1-2,yy+1)],fill=WHITE3,width=1)
        d.point((cx,y1-1),fill=SKIN)
    if rear:
        d.point((cx,mid),fill=WHITE0)
    return im


def body_sheet():
    ref=Image.open(ARM/'Armor_1.png').convert('RGBA')
    sheet=Image.new('RGBA',(360,224))
    for r in range(4):
        for c in range(9):
            box=bbox_for(ref,c,r)
            if not box: continue
            if c in (0,1) and r in (0,2): piece=torso((c+r//2)%2)
            else: piece=arm(box,c+r*9,c in (2,3,4))
            sheet.alpha_composite(up(piece),(c*40,r*56))
    return sheet


def legs_frame(i,box):
    im=tile();d=ImageDraw.Draw(im)
    # Vanilla anchors determine step height; all clothing contours are new.
    top=max(18,box[1]-2)
    leap=i in (5,6,7,8,9,14,15,16)
    step=(-1,0,1,0)[i%4]
    left_x=6+step;right_x=11-step
    poly(d,[(6,top),(13,top),(15,top+2),(14,top+6),(12,top+7),(11,top+5),(10,top+7),(6,top+6),(5,top+3)],TROU1)
    poly(d,[(6,top+1),(9,top+1),(9,top+5),(7,top+6),(6,top+5)],TROU2,None)
    poly(d,[(11,top+1),(14,top+1),(14,top+4),(13,top+6),(11,top+5)],TROU3,None)
    d.line([(9,top+1),(10,top+4),(9,top+6)],fill=TROU0,width=1)
    # Pouch at wearer's right, on image left: outline, flap, clasp, two straps.
    rect(d,(4,top+1,7,top+5),BROWN1,INK)
    rect(d,(4,top+1,7,top+2),BROWN3)
    d.point((6,top+3),fill=WHITE3)
    d.line([(7,top+2),(9,top+2)],fill=BROWN0,width=1)
    d.line([(7,top+5),(9,top+5)],fill=BROWN0,width=1)
    # White left thigh wrap and highlights on folds.
    d.line([(11,top+3),(14,top+3)],fill=WHITE2,width=2)
    d.line([(12,top+2),(14,top+2)],fill=WHITE3,width=1)
    d.line([(7,top+4),(8,top+5)],fill=TROU3,width=1)
    d.line([(12,top+5),(13,top+6)],fill=TROU0,width=1)
    # Calf wraps follow each step; jump frames bend a foot upward.
    feet_y=min(27,top+10)
    if leap: feet_y=min(27,top+9)
    for x,dy in ((left_x,0),(right_x,1 if i%2 else 0)):
        d.line([(x,top+7),(x+2,top+7)],fill=WHITE3,width=2)
        d.line([(x,top+8),(x+2,top+8)],fill=WHITE1,width=1)
        d.line([(x,top+9),(x+2,top+9)],fill=WHITE3,width=1)
        fy=min(27,feet_y+dy)
        poly(d,[(x-1,fy-1),(x+2,fy-1),(x+3,fy),(x+3,fy+1),(x-1,fy+1)],BLUE1)
        d.line([(x,fy),(x+2,fy)],fill=BLUE3,width=1)
        d.point((x+3,fy),fill=SKIN)
    return im


def frame_sheet(make, refpath=None):
    sheet=Image.new('RGBA',(40,1120))
    ref=Image.open(refpath).convert('RGBA') if refpath else None
    for i in range(20):
        box=bbox_for(ref,0,i) if ref else None
        p=make(i,box) if ref else make(i)
        sheet.alpha_composite(up(p),(0,i*56))
    return sheet


def icon_head():
    im=tile();d=ImageDraw.Draw(im)
    poly(d,[(3,10),(5,8),(15,8),(17,10),(16,14),(4,14)],NAVY)
    d.line([(4,9),(16,9)],fill=NAVYHI,width=1)
    poly(d,[(14,13),(18,16),(17,19),(14,16)],NAVYL)
    rect(d,(6,9,14,13),WHITE1,INK)
    rect(d,(7,9,13,9),WHITE3)
    d.line([(9,12),(10,10),(12,10),(12,11),(11,11)],fill=NAVY,width=1)
    return up(im).crop((0,6,40,46)).resize((28,28),Image.Resampling.NEAREST)


def icon_body():
    im=tile();d=ImageDraw.Draw(im)
    poly(d,[(6,6),(9,5),(12,5),(15,6),(18,9),(16,13),(14,12),(14,20),(6,20),(6,12),(4,13),(2,10)],BLUE1)
    d.line([(7,7),(7,18)],fill=BLUE3,width=1)
    d.line([(13,7),(13,18)],fill=BLUE0,width=1)
    d.line([(3,11),(5,11)],fill=WHITE3,width=1)
    d.line([(15,11),(17,11)],fill=WHITE3,width=1)
    poly(d,[(8,6),(10,6),(11,9),(10,10),(8,8)],WHITE2)
    poly(d,[(11,6),(13,6),(12,9),(10,10)],WHITE3)
    d.line([(5,9),(14,17)],fill=BROWN1,width=2)
    d.line([(6,9),(13,16)],fill=BROWN3,width=1)
    d.point((12,11),fill=WHITE3);d.point((13,12),fill=WHITE3)
    return up(im).crop((0,4,40,44)).resize((30,30),Image.Resampling.NEAREST)


def icon_legs():
    im=tile();d=ImageDraw.Draw(im)
    poly(d,[(5,4),(15,4),(16,8),(14,20),(11,20),(10,12),(9,20),(6,20),(4,9)],TROU1)
    d.line([(6,5),(9,5)],fill=TROU3,width=1)
    d.line([(10,5),(10,11)],fill=TROU0,width=1)
    rect(d,(3,9,7,13),BROWN1,INK)
    d.line([(3,9),(7,9)],fill=BROWN3,width=1)
    d.point((5,11),fill=WHITE3)
    d.line([(11,10),(15,10)],fill=WHITE3,width=1)
    for x in (6,12):
        d.line([(x,17),(x+2,17)],fill=WHITE2,width=1)
        d.line([(x,19),(x+2,19)],fill=WHITE3,width=1)
        d.line([(x-1,20),(x+2,20)],fill=BLUE1,width=2)
        d.point((x+3,20),fill=SKIN)
    return up(im).crop((2,4,38,46)).resize((28,30),Image.Resampling.NEAREST)


def recolor_head(im):
    # Terraria reference heads are monochrome base layers; tint for readability.
    out=im.copy()
    px=out.load()
    for y in range(out.height):
        for x in range(out.width):
            r,g,b,a=px[x,y]
            if not a:continue
            lum=(r+g+b)//3
            px[x,y]=(min(255,int(lum*.62+95)),min(255,int(lum*.43+68)),min(255,int(lum*.32+51)),a)
    return out


def recolor_hair(im):
    out=im.copy();px=out.load()
    for y in range(out.height):
        for x in range(out.width):
            r,g,b,a=px[x,y]
            if a:
                lum=(r+g+b)//3
                px[x,y]=(22+lum//4,30+lum//4,49+lum//4,a)
    return out


def crop_frame(sheet,i):
    return sheet.crop((0,i*56,40,(i+1)*56))


def compose(head,body,legs,i):
    canvas=Image.new('RGBA',(40,56))
    # Renderer order requested: back arm, legs, torso, head, front arm.
    canvas.alpha_composite(body.crop((320,0,360,56)))
    canvas.alpha_composite(crop_frame(legs,i))
    canvas.alpha_composite(body.crop((0,0,40,56)))
    face=Image.open(PLY/'Player_0_0.png').convert('RGBA')
    hair=Image.open(PLY/'Player_Hair_1.png').convert('RGBA')
    canvas.alpha_composite(recolor_head(crop_frame(face,i)))
    hair_i=i%14
    canvas.alpha_composite(recolor_hair(crop_frame(hair,hair_i)))
    canvas.alpha_composite(crop_frame(head,i))
    canvas.alpha_composite(body.crop((280,0,320,56)))
    return canvas


def preview(head,body,legs,icons):
    indices=[0,1,3,7]
    vanilla_head=Image.open(ARM/'Armor_Head_22.png').convert('RGBA')
    vanilla_body=Image.open(ARM/'Armor_14.png').convert('RGBA')
    vanilla_legs=Image.open(ARM/'Armor_Legs_14.png').convert('RGBA')
    copper_body=Image.open(ARM/'Armor_1.png').convert('RGBA')
    copper_legs=Image.open(ARM/'Armor_Legs_1.png').convert('RGBA')
    blank_head=Image.new('RGBA',(40,1120))
    out=Image.new('RGB',(920,480),'#253048')
    d=ImageDraw.Draw(out)
    labels=['standing','walk 1','walk 2','jump']
    for row,scale in ((0,1),(1,3)):
        oy=20 if row==0 else 115
        d.text((8,oy),f'Genin {scale}x',fill='white')
        for col,i in enumerate(indices):
            img=compose(head,body,legs,i).resize((40*scale,56*scale),Image.Resampling.NEAREST)
            out.paste(img,(85+col*(130 if scale==3 else 65),oy+18),img)
            d.text((85+col*(130 if scale==3 else 65),oy+20+56*scale),labels[col],fill='#d8dce0')
    # Same standing frame, same composition and scale, side by side.
    y=330
    for j,(label,h,b,l) in enumerate([('Genin',head,body,legs),('Ninja',vanilla_head,vanilla_body,vanilla_legs),('Copper',blank_head,copper_body,copper_legs)]):
        img=compose(h,b,l,0).resize((120,168),Image.Resampling.NEAREST)
        x=525+j*130
        out.paste(img,(x,y-36),img)
        d.text((x+29,465),label,fill='white')
    d.text((8,323),'Item icons 1x / 3x',fill='white')
    for j,ic in enumerate(icons):
        x=20+j*115
        out.paste(ic,(x,350),ic)
        big=ic.resize((ic.width*3,ic.height*3),Image.Resampling.NEAREST)
        out.paste(big,(x+35,345),big)
    out.save(OUT/'preview.png')


def main():
    head=frame_sheet(head_frame)
    body=body_sheet()
    legs=frame_sheet(legs_frame,ARM/'Armor_Legs_1.png')
    icons=[icon_head(),icon_body(),icon_legs()]
    files=[('GeninCombatHead_Head.png',head),('GeninCombatBody_Body.png',body),('GeninCombatLegs_Legs.png',legs),('GeninCombatHead.png',icons[0]),('GeninCombatBody.png',icons[1]),('GeninCombatLegs.png',icons[2])]
    for name,im in files: im.save(OUT/name)
    preview(head,body,legs,icons)
    for name,im,ref in [('head',head,None),('body',body,ARM/'Armor_1.png'),('legs',legs,ARM/'Armor_Legs_1.png')]:
        if ref:
            mask=Image.open(ref).convert('RGBA').getchannel('A').point(lambda a:255 if a else 0)
            own=im.getchannel('A').point(lambda a:255 if a else 0)
            extra=sum(1 for a,b in zip(own.getdata(),mask.getdata()) if a and not b)
            print(name,'outside-template pixels',extra)
    print('built',len(files),'sprite files and preview')


if __name__=='__main__':main()
