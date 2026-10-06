from pathlib import Path
from PIL import Image, ImageDraw
import shutil

ROOT = Path('/Users/tuzhechen/Documents/ChatGPT/泰拉瑞亚火影模组')
OUT = ROOT / 'art/deliveries/iruka-npc-v4'
SRC = OUT / 'source'
SRC.mkdir(parents=True, exist_ok=True)
GEN1 = SRC/'generated_Iruka_Idle_Walk.png'
GEN2 = SRC/'generated_Iruka_Idle_Jump_Sit_Throw.png'
if not GEN1.exists():
    shutil.copy2('/Users/tuzhechen/.codex/generated_images/01a10a7d-c71a-7dc2-9e9e-af70f526e54f/exec-5a27aeb6-3fd9-4398-965b-9920b797b0b0.png', GEN1)
if not GEN2.exists():
    shutil.copy2('/Users/tuzhechen/.codex/generated_images/01a10a7d-c71a-7dc2-9e9e-af70f526e54f/exec-7d33e79a-fe17-4879-ac5d-a574ee80969e.png', GEN2)

INK='#221a1e'; HAIR='#262125'; HAIR_SHADE='#3c3030'; HAIR_LIGHT='#59443a'
BLUE='#263d65'; BLUE_HI='#4b6790'; SILVER='#cbd5d5'; SILVER_HI='#f2ece0'
SKIN='#f6a36f'; SKIN_HI='#ffc196'; SKIN_SHADE='#d77755'; SCAR='#dd8966'
VEST_DARK='#38442c'; VEST='#67764d'; VEST_HI='#9aa475'
NAVY_DARK='#1b2842'; NAVY='#2d4061'; NAVY_HI='#526383'
WHITE='#dbd9c8'; WHITE_HI='#f4efe3'; BROWN='#815035'; PARCH='#e6c994'

def clean_crop(image, x0, x1):
    crop=image.crop((x0,0,x1,image.height)).convert('RGBA')
    a=crop.getchannel('A').point(lambda v:255 if v>=110 else 0)
    crop.putalpha(a)
    box=crop.getbbox()
    return crop.crop(box)

def body(image,x0,x1,kind):
    crop=clean_crop(image,x0,x1)
    w=min(27,max(1,round(crop.width*23/crop.height*1.15)))
    mini=crop.resize((w,23),Image.Resampling.BOX)
    alpha=mini.getchannel('A').point(lambda v:255 if v>=145 else 0)
    mini.putalpha(alpha)
    # Rebuild every generated color into a small hard-edged material palette.
    palette=[INK,HAIR,HAIR_SHADE,HAIR_LIGHT,BLUE,BLUE_HI,SILVER,SILVER_HI,
             SKIN,SKIN_HI,SKIN_SHADE,VEST_DARK,VEST,VEST_HI,
             NAVY_DARK,NAVY,NAVY_HI,WHITE,WHITE_HI,BROWN,PARCH]
    palette_rgb=[tuple(bytes.fromhex(c[1:])) for c in palette]
    p=mini.load()
    for y in range(23):
        for x in range(w):
            r,g,b,a=p[x,y]
            if not a: continue
            choice=min(range(len(palette_rgb)),key=lambda j:sum((z-v)**2 for z,v in zip((r,g,b),palette_rgb[j])))
            p[x,y]=(*palette_rgb[choice],255)
    canvas=Image.new('RGBA',(28,25))
    canvas.alpha_composite(mini,((28-w)//2,0 if kind!='jump' else 0))
    # Remove all generated head pixels; this part is drawn directly on the 1-pixel art grid.
    d=ImageDraw.Draw(canvas)
    d.rectangle((0,0,27,10),fill=(0,0,0,0))
    return canvas

def head(im):
    d=ImageDraw.Draw(im)
    def px(x,y,c):d.point((x,y),fill=c)
    def rect(box,c):d.rectangle(box,fill=c)
    # Ten-by-ten main head at x=11..20, y=0..9. Ponytail projects behind it.
    rect((11,2,19,4),INK)
    rect((12,1,18,3),HAIR)
    rect((14,1,17,1),HAIR_LIGHT)
    rect((12,2,16,2),HAIR_SHADE)
    # A separated, pointed fan of hair, rather than a square block.
    for x,y,c in [(10,0,HAIR_LIGHT),(8,1,HAIR),(9,1,HAIR_LIGHT),(10,1,HAIR_SHADE),
                  (7,2,INK),(8,2,HAIR_LIGHT),(9,2,HAIR_LIGHT),(10,2,HAIR),(11,2,HAIR),
                  (8,3,INK),(9,3,HAIR),(10,3,HAIR_SHADE),(11,3,HAIR),
                  (9,4,INK),(10,4,HAIR),(11,4,BLUE)]:px(x,y,c)
    # Blue forehead cloth and polished metal plate; neither touches the eyes.
    rect((12,3,20,4),BLUE)
    rect((13,3,14,3),BLUE_HI)
    rect((16,3,19,3),SILVER_HI)
    rect((16,4,19,4),SILVER)
    px(17,4,BLUE);px(18,4,BLUE_HI)
    px(20,3,INK);px(20,4,INK)
    # Face is a clean 7x5 skin field with two-tone shading.
    rect((13,5,20,9),SKIN)
    rect((13,5,14,8),SKIN_SHADE)
    rect((16,5,19,5),SKIN_HI)
    rect((19,6,20,7),SKIN_HI)
    px(13,9,INK);px(14,9,SKIN_SHADE);px(15,9,SKIN)
    px(20,9,SKIN_SHADE)
    # Dark brows; each eye has pupil and white in neighboring columns for two rows.
    rect((14,5,15,5),HAIR)
    rect((17,5,18,5),HAIR)
    for y in (6,7):
        for x in (14,17):px(x,y,INK)
        for x in (15,18):px(x,y,WHITE_HI)
    # One art-pixel-high bridge scar, never a filled patch.
    rect((15,8,18,8),SCAR)
    px(20,8,SKIN_HI);px(21,8,SKIN_SHADE) # one-pixel nose projection
    px(18,9,HAIR_SHADE);px(19,9,SKIN_HI) # slight smile
    px(12,5,HAIR);px(12,6,HAIR_SHADE);px(12,7,INK)
    px(13,8,SKIN_SHADE)
    # Neck and collar bridge the directly drawn head to the sampled body.
    rect((13,10,18,10),NAVY_DARK)
    px(16,10,SKIN_SHADE)

def finish_body(im,kind):
    d=ImageDraw.Draw(im)
    # Hard-edged green vest, standing collar, shoulder and two pocket blocks.
    d.rectangle((11,11,18,15),fill=VEST_DARK)
    d.rectangle((12,11,17,14),fill=VEST)
    d.rectangle((12,11,14,11),fill=VEST_HI)
    d.rectangle((17,11,17,14),fill=VEST_DARK)
    d.point((14,12),fill=VEST_HI)
    d.rectangle((12,14,13,14),fill=VEST_HI)
    d.rectangle((15,14,16,14),fill=VEST_HI)
    d.point((14,15),fill=INK)
    d.rectangle((12,10,13,11),fill=NAVY_DARK)
    d.rectangle((17,10,18,11),fill=NAVY_DARK)
    # A pale archive roll remains visible at his rear hand in the neutral pose.
    if kind=='idle':
        d.rectangle((8,15,10,16),fill=BROWN)
        d.rectangle((8,14,9,15),fill=PARCH)
        d.point((8,14),fill=WHITE_HI)
    if kind!='sit':
        # A short horizontal band on each visible lower leg.
        alpha=im.getchannel('A')
        runs=[]
        for x in range(4,25):
            if alpha.getpixel((x,19)):
                if not runs or x>runs[-1][-1]+1:runs.append([x])
                else:runs[-1].append(x)
        for run in runs:
            if len(run)>=2:
                mid=run[len(run)//2]
                d.point((mid,19),fill=WHITE_HI)
                d.point((max(run[0],mid-1),19),fill=WHITE)

im1=Image.open(GEN1).convert('RGBA')
im2=Image.open(GEN2).convert('RGBA')
walk_ranges=[(42,261),(283,581),(603,836),(866,1170),(1180,1501),(1558,1852),(1849,2160)]
action_ranges=[(42,277),(306,614),(628,1001),(1024,1370),(1377,1788),(1800,2172)]

def make_frames(image,ranges,kinds):
    frames=[]
    for (x0,x1),kind in zip(ranges,kinds):
        fr=body(image,x0,x1,kind)
        finish_body(fr,kind)
        head(fr)
        frames.append(fr)
    return frames

walk=make_frames(im1,walk_ranges,['idle']+['walk']*6)
actions=make_frames(im2,action_ranges,['idle','jump','sit','throw','throw','throw'])
# Use the same exact idle frame as the first strip.
actions[0]=walk[0].copy()
# A tiny but readable rightward kunai on the release frame.
release=ImageDraw.Draw(actions[4])
release.rectangle((20,12,22,12),fill=INK)
release.rectangle((23,12,25,12),fill=SILVER)
release.point((26,12),fill=INK)
release.point((24,11),fill=SILVER_HI)

def save_strip(frames,name):
    small=Image.new('RGBA',(28*len(frames),25))
    for i,fr in enumerate(frames):small.alpha_composite(fr,(28*i,0))
    small.save(OUT/(name+'_game.png'))
    large=small.resize((small.width*8,small.height*8),Image.Resampling.NEAREST)
    large.save(SRC/(name+'.png'))

save_strip(walk,'Iruka_Idle_Walk')
save_strip(actions,'Iruka_Idle_Jump_Sit_Throw')

# Head comparison against the provided Guide art grid, both at 12x and native game scale.
grid=Image.open(ROOT/'art/reference/terraria/vanilla_heads_grid.png').convert('RGBA')
guide_head=grid.crop((0,0,12*12,10*12))
our_head=walk[0].crop((10,0,20,10)).resize((120,120),Image.Resampling.NEAREST)
check=Image.new('RGBA',(280,140),'#28272c')
check.alpha_composite(guide_head,(8,8))
check.alpha_composite(our_head,(152,8))
check.save(OUT/'face_grid_check.png')

guide1=grid.crop((0,0,12*12,10*12)).resize((12,10),Image.Resampling.NEAREST)
one=Image.new('RGBA',(34,14),'#323035')
one.alpha_composite(guide1,(2,2))
one.alpha_composite(walk[0].crop((10,0,20,10)),(20,2))
one.save(OUT/'face_game_scale_check.png')

tz=Image.open(ROOT/'art/deliveries/tazuna-npc-v1/source/Tazuna_Idle_Walk.png').convert('RGBA').crop((0,0,224,200))
shop=Image.open(ROOT/'art/deliveries/tool-shop-npc-v1/source/ToolShop_Idle_Walk.png').convert('RGBA').crop((0,0,224,200))
ir=walk[0].resize((224,200),Image.Resampling.NEAREST)
cmp=Image.new('RGBA',(112*3,100+6+25),'#302f34')
for i,ref in enumerate((tz,shop,ir)):
    cmp.alpha_composite(ref.resize((112,100),Image.Resampling.NEAREST),(112*i,0))
    cmp.alpha_composite(ref.resize((28,25),Image.Resampling.NEAREST),(112*i+42,106))
cmp.save(OUT/'Tazuna_ToolShop_Iruka_comparison.png')
