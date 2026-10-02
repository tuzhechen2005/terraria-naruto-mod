from pathlib import Path
from collections import Counter
from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parents[4]
OUT = Path(__file__).resolve().parents[1]
BASE = Image.open(ROOT/'art/deliveries/gaara-base-v2/Gaara_Idle_0.png').convert('RGBA')
ATLAS = Image.open(OUT/'source/generated_key_poses.png').convert('RGBA')
STYLE = Image.open(ROOT/'art/deliveries/tazuna-npc-v1/Guide_Tazuna_comparison.png').convert('RGB')
PAL = list(dict.fromkeys(p[:3] for p in BASE.getdata() if p[3] == 255))
SCALE = .23
ANCHORS = [274, 250, 215, 250, 177, 165, 220, 250, 250]

def nearest(c):
    return min(PAL, key=lambda p: (p[0]-c[0])**2*.8+(p[1]-c[1])**2+(p[2]-c[2])**2*.7)

cache={}
def pose(i):
    x=i%3*512; y=i//3*341
    tile=ATLAS.crop((x,y,x+512,min(y+341,1024)))
    mask=tile.getchannel('A').point(lambda v: 255 if v>=180 else 0)
    bbox=mask.getbbox()
    tile.putalpha(mask)
    # Pixelize on the 2x2 game grid, retaining the generated pose and base palette.
    nw=round(512*SCALE/2); nh=round(tile.height*SCALE/2)
    tile=tile.resize((nw,nh),Image.Resampling.NEAREST)
    p=tile.load()
    for yy in range(nh):
        for xx in range(nw):
            c=p[xx,yy]
            if c[3]:
                rgb=c[:3]
                if rgb not in cache: cache[rgb]=nearest(rgb)
                p[xx,yy]=(*cache[rgb],255)
            else: p[xx,yy]=(0,0,0,0)
    tile=tile.resize((nw*2,nh*2),Image.Resampling.NEAREST)
    canvas=Image.new('RGBA',(112,88))
    tx=56-round(ANCHORS[i]*SCALE/2)*2
    ty=84-round(bbox[3]*SCALE/2)*2
    canvas.alpha_composite(tile,(tx,ty))
    # Remove detached single logical pixels left from antialias fragments.
    logical=canvas.resize((56,44),Image.Resampling.NEAREST)
    px=logical.load()
    for yy in range(44):
        for xx in range(56):
            if px[xx,yy][3]==0: continue
            neighbors=sum(0<=xx+dx<56 and 0<=yy+dy<44 and px[xx+dx,yy+dy][3]>0 for dx,dy in ((1,0),(-1,0),(0,1),(0,-1)))
            if neighbors==0: px[xx,yy]=(0,0,0,0)
    return logical.resize((112,88),Image.Resampling.NEAREST)

def save(name,im):
    im.save(OUT/(name+'.png'))

save('Gaara_Idle_0',BASE)
# Breathing changes are confined to one art pixel on the upper torso, hair tip and gourd tie.
def breathe(im,phase):
    a=im.copy()
    if phase==0:return a
    # Move a few coherent 2x2 clusters by one logical pixel; never move the feet.
    shifts={1:[(52,34),(54,34)],2:[(52,34),(54,34),(64,20)],3:[(52,34),(54,34),(64,20),(30,34)],4:[(52,34),(54,34),(64,20)],5:[(52,34),(54,34)]}[phase]
    for x,y in shifts:
        patch=im.crop((x,y,x+2,y+2))
        if patch.getbbox():
            a.paste((0,0,0,0),(x,y,x+2,y+2))
            a.alpha_composite(patch,(x,y-2 if y<30 else y+2))
    # A single changing hair-edge pixel makes each held breath phase legible at game scale.
    tips={1:(52,16,0),2:(54,18,-2),3:(60,16,-2),4:(46,18,-2),5:(64,20,-2)}
    tx,ty,dy=tips[phase]
    chip=im.crop((tx,ty,tx+2,ty+2))
    if chip.getbbox():
        a.paste((0,0,0,0),(tx,ty,tx+2,ty+2))
        if dy:a.alpha_composite(chip,(tx,ty+dy))
    return a
for i in range(1,6):save(f'Gaara_Idle_{i}',breathe(BASE,i))

walk_a,walk_b=pose(1),pose(2)
def master_upper_walk(im):
    shifted=Image.new('RGBA',(112,88))
    shifted.alpha_composite(im,(-10,2))
    # The approved head, crossed arms and gourd remain the same in every walk frame.
    upper=BASE.crop((0,0,112,56))
    shifted.alpha_composite(upper,(0,0))
    return shifted
walk_a,walk_b=master_upper_walk(walk_a),master_upper_walk(walk_b)
# Eight-beat small walk; repeated contact poses receive a slight arm/hem phase from base.
walks=[BASE,walk_a,walk_b,walk_a,BASE,walk_b,walk_a,walk_b]
for i,im in enumerate(walks):
    if i in (3,6): im=breathe(im,1)
    if i in (4,5,7): im=breathe(im,2 if i!=7 else 3)
    if i in (3,5,6,7):
        dx={3:-2,5:2,6:2,7:-2}[i]
        moved=Image.new('RGBA',(112,88))
        moved.alpha_composite(im.crop((0,60,112,84)),(dx,60))
        im.paste((0,0,0,0),(0,60,112,84))
        im.alpha_composite(moved)
    save(f'Gaara_Walk_{i}',im)

save('Gaara_CastIn_0',pose(3))
cast=pose(4)
for i in range(3):
    im=cast.copy()
    if i==0:
        # Cast opens with only the nearest portion of sand.
        for x in range(94,112): im.paste((0,0,0,0),(x,0,x+1,88))
    elif i==1:
        for x in range(106,112): im.paste((0,0,0,0),(x,0,x+1,88))
    save(f'Gaara_Cast_{i}',im)
wave=pose(5)
save('Gaara_WaveIn_0',pose(3))
for i in range(3):
    im=wave.copy()
    if i==0: im.paste((0,0,0,0),(90,0,112,88))
    elif i==1: im.paste((0,0,0,0),(102,0,112,88))
    save(f'Gaara_Wave_{i}',im)
shield=pose(6)
save('Gaara_Shield_0',shield)
save('Gaara_Shield_1',breathe(shield,1))
save('Gaara_Hurt_0',pose(7))

# Cracked armour is painted on the approved base, so proportions and straps stay identical.
def cracked(phase):
    im=breathe(BASE,phase+1)
    d=ImageDraw.Draw(im)
    shell=(225,185,127,255); shadow=(137,89,58,255)
    for pts in [((62,30),(64,32),(62,34)),((66,38),(64,40)),((64,46),(66,48)),((48,48),(46,50))]:
        d.line(pts,fill=shell,width=2)
    d.rectangle((64,32,65,33),fill=shadow)
    d.rectangle((67,48,69,49),fill=shell)
    # Replace the relaxed eye with a narrow, fierce brow.
    d.line(((62,28),(66,28)),fill=(37,23,20,255),width=2)
    return im.resize((56,44),Image.Resampling.NEAREST).resize((112,88),Image.Resampling.NEAREST)
for i in range(4):save(f'Gaara_Cracked_Idle_{i}',cracked(i))

# 1x1 minimap portrait taken from the master head and outlined in its own 30x30 canvas.
head=BASE.crop((42,16,72,42))
portrait=Image.new('RGBA',(30,30))
portrait.alpha_composite(head,(0,2))
save('Gaara_Head_Boss',portrait)

names=[*[f'Gaara_Idle_{i}' for i in range(6)],*[f'Gaara_Walk_{i}' for i in range(8)],'Gaara_CastIn_0',*[f'Gaara_Cast_{i}' for i in range(3)],'Gaara_WaveIn_0',*[f'Gaara_Wave_{i}' for i in range(3)],*[f'Gaara_Shield_{i}' for i in range(2)],'Gaara_Hurt_0',*[f'Gaara_Cracked_Idle_{i}' for i in range(4)],'Gaara_Head_Boss']
# Contact sheet: left is 1x, right is 4x. Both include base and Tazuna style reference.
cols=7; rows=(len(names)+cols-1)//cols
board=Image.new('RGB',(800,rows*110+320),(35,39,51)); d=ImageDraw.Draw(board)
d.text((16,12),'GAARA-SET-V2 / 1x frames',fill='white')
d.text((16,rows*110+48),'Approved base and Tazuna style reference',fill='white')
for n,name in enumerate(names):
    im=Image.open(OUT/(name+'.png')).convert('RGBA')
    col=n%cols; row=n//cols
    x=12+col*112; y=42+row*110
    board.paste(im,(x,y),im)
    d.text((x,y+89),name.replace('Gaara_','')[:15],fill='white')
    # Extra 4x panel is stored in a separate canvas below.
board.paste(BASE.resize((224,176),Image.Resampling.NEAREST),(20,rows*110+70),BASE.resize((224,176),Image.Resampling.NEAREST))
board.paste(STYLE,(260,rows*110+70))
board.save(OUT/'preview_1x.png')
large=Image.new('RGB',(3*460,((len(names)+2)//3)*400+320),(35,39,51));dl=ImageDraw.Draw(large)
for n,name in enumerate(names):
    im=Image.open(OUT/(name+'.png')).convert('RGBA').resize((448,352),Image.Resampling.NEAREST)
    x=(n%3)*460;y=(n//3)*400
    large.paste(im,(x,y),im)
    dl.text((x+10,y+355),name,fill='white')
large.paste(BASE.resize((336,264),Image.Resampling.NEAREST),(20,large.height-285),BASE.resize((336,264),Image.Resampling.NEAREST))
large.paste(STYLE,(390,large.height-280))
large.save(OUT/'preview_4x.png')
# Combined preview provides both scales plus the specified reference images.
combo=Image.new('RGB',(large.width,board.height+large.height),(35,39,51));combo.paste(board,(0,0));combo.paste(large,(0,board.height));combo.save(OUT/'preview.png')
# Alignment check: alpha overlay of 8 walk and 6 idle poses, same baseline.
over=Image.new('RGBA',(224,88))
for n in names[:14]:
    im=Image.open(OUT/(n+'.png')).convert('RGBA')
    tint=(0,180,250,40) if 'Idle' in n else (255,190,0,40)
    layer=Image.new('RGBA',im.size)
    layer.putdata([tint if p[3] else (0,0,0,0) for p in im.getdata()])
    over.alpha_composite(layer,(0 if 'Idle' in n else 112,0))
check=Image.new('RGB',(896,400),(35,39,51));dc=ImageDraw.Draw(check)
check.paste(over.resize((896,352),Image.Resampling.NEAREST),(0,0),over.resize((896,352),Image.Resampling.NEAREST))
dc.line((0,336,896,336),fill='white',width=1)
dc.text((20,366),'Idle 6 frames                         Walk 8 frames / common foot line y=83',fill='white')
check.save(OUT/'alignment_overlay.png')
print('wrote',len(names),'game PNGs')
