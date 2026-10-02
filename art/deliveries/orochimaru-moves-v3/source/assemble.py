"""Build 2x2 pixel-grid boss frames from the approved v3 masters."""
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

HERE = Path(__file__).resolve().parent
OUT = HERE.parent
ROOT = HERE.parents[3]
STYLE = ROOT / "art/deliveries/orochimaru-style-v3"
PALETTE_IMAGE = ROOT / "art/requests/orochimaru-palette-v3.png"
W, H = 56, 44

def logical(path):
    im = Image.open(path).convert("RGBA")
    assert im.size == (112, 88)
    # The approved sprite already consists of solid 2x2 blocks.
    return im.resize((W, H), Image.Resampling.NEAREST)

idle = logical(STYLE / "Orochimaru_Idle.png")
disguise_raw = logical(STYLE / "Orochimaru_Disguise.png")
palette = set(Image.open(PALETTE_IMAGE).convert("RGB").getdata())
greens = [(24, 42, 25), (37, 59, 30), (54, 79, 37), (73, 101, 46)]
allowed = palette | set(greens)

def nearest(rgb, choices):
    return min(choices, key=lambda c: sum((a-b)**2 for a,b in zip(rgb,c)))

def enforce_palette(im):
    o = im.copy()
    for y in range(H):
        for x in range(W):
            r,g,b,a = o.getpixel((x,y))
            if not a: continue
            if (r,g,b) in allowed: continue
            choices = greens if g > r*1.13 and g > b*1.06 and g < 160 else palette
            o.putpixel((x,y), (*nearest((r,g,b), choices),255))
    return o

def long_hair(disguise):
    out = disguise.copy()
    # The identical approved hair pixels sit behind the disguise's face and shoulders.
    for y in range(11,25):
        for x in range(17,32):
            p = idle.getpixel((x,y))
            if not p[3]: continue
            r,g,b,_ = p
            if max(r,g,b) < 115 and b >= r*.75 and (x < 28 or y < 19):
                out.putpixel((x,y),p)
    return out

disguise = enforce_palette(long_hair(disguise_raw))

def save(name, im):
    im = enforce_palette(im)
    im.resize((112,88),Image.Resampling.NEAREST).save(OUT / f"Orochimaru_{name}.png")
    return im

frames = {}
def emit(name, im): frames[name] = save(name,im)

# Breathing shifts only the lower sleeve and sash; all face pixels remain identical.
for i in range(4):
    f = idle.copy()
    if i in (1,2):
        for x in range(21,36):
            y = 28
            p = idle.getpixel((x,y))
            if p[3] and p[0] > 70: f.putpixel((x,y-1),p)
    emit(f"Idle_{i}",f)

def walking(master, phase):
    # Reposition the approved garment/leg pixels on whole art-pixel steps.
    # The torso, face and hair are always copied verbatim from the master.
    out = master.copy()
    shifts = [(-2,1),(-1,0),(2,-1),(1,-2)]
    back, front = shifts[phase]
    for y in range(34,H):
        for x in range(W): out.putpixel((x,y),(0,0,0,0))
    for y in range(34,H):
        for x in range(W):
            p=master.getpixel((x,y))
            if not p[3]: continue
            dx=back if x<28 else front
            # Keep each foot on the same ground line; pose changes are lateral.
            nx=max(0,min(W-1,x+dx))
            out.putpixel((nx,y),p)
    # Restore central hems at the seam between fixed torso and moving legs.
    for y in range(32,36):
        for x in range(23,34):
            p=master.getpixel((x,y))
            if p[3]:out.putpixel((x,y),p)
    return out

for i in range(4):
    emit(f"Walk_{i}",walking(idle,i))
emit("Disguise_0",disguise)
for i in range(4):
    emit(f"DisguiseWalk_{i}",walking(disguise,i))

# Tear the false face away in two steps, while retaining the approved body pixels.
emit("Reveal_0",disguise)
r1=disguise.copy()
for y in range(16,24):
    for x in range(30,39):
        if (x+y)%4!=0:
            p=idle.getpixel((x,y))
            if p[3]:r1.putpixel((x,y),p)
# A small lifted flap, made from approved pale skin colors.
skin=nearest((232,211,185),palette)
for x,y in [(39,20),(40,21),(40,22),(39,23),(38,24)]:r1.putpixel((x,y),(*skin,255))
emit("Reveal_1",r1)
emit("Reveal_2",idle)

# Submergence keeps the original sprite width and face; the earth line reveals it.
soil=[nearest(c,palette) for c in [(52,38,43),(32,26,34),(77,46,48)]]
def ground_cut(master,cut,emerging=False):
    out=master.copy()
    for y in range(cut,H):
        for x in range(W):out.putpixel((x,y),(0,0,0,0))
    if cut<H:
        for x in range(18,39):
            if (x*7+cut)%5!=0:
                out.putpixel((x,cut),(*soil[(x+cut)%3],255))
        for x in (18,21,35,38):
            if cut+1<H:out.putpixel((x,cut+1),(*soil[x%3],255))
    return out

emit("Emerge_0",ground_cut(idle,22))
emit("Emerge_1",ground_cut(idle,34))
emit("Emerge_2",idle)
emit("Sink_0",ground_cut(idle,36))
emit("Sink_1",ground_cut(idle,28))
emit("Sink_2",ground_cut(idle,13))

# Knockback: move the upper figure left while retaining its approved pixels.
hurt=Image.new("RGBA",(W,H))
for y in range(H):
    for x in range(W):
        p=idle.getpixel((x,y))
        if not p[3]:continue
        dx= -3 if y<25 else (-2 if y<33 else 0)
        hurt.putpixel((x+dx,y),p)
emit("Hurt_0",hurt)

order=[("Idle",4),("Walk",4),("DisguiseWalk",4),("Reveal",3),("Emerge",3),("Sink",3),("Hurt",1)]
scale=3; tile_w=112*scale; tile_h=88*scale; label_h=26
preview=Image.new("RGB",(tile_w*5+70,(tile_h+label_h)*len(order)),(26,32,42))
draw=ImageDraw.Draw(preview)
for row,(action,count) in enumerate(order):
    yy=row*(tile_h+label_h)
    draw.text((8,yy+6),action,fill="white")
    images=[("v3",idle)]+[(str(i),frames[f"{action}_{i}"]) for i in range(count)]
    for col,(label,im) in enumerate(images):
        xx=70+col*tile_w
        preview.paste(im.resize((tile_w,tile_h),Image.Resampling.NEAREST),(xx,yy),im.getchannel("A").resize((tile_w,tile_h),Image.Resampling.NEAREST))
        draw.text((xx+8,yy+tile_h+4),label,fill="white")
preview.save(OUT/"preview.png")

sequence=[frames[f"Idle_{i}"] for i in range(4)]+[frames[f"Walk_{i}"] for i in range(4)]+[frames[f"Idle_{i}"] for i in range(4)]
strip=Image.new("RGB",(len(sequence)*112*3,88*3),(26,32,42))
for i,im in enumerate(sequence):
    big=im.resize((112*3,88*3),Image.Resampling.NEAREST)
    mask=im.getchannel("A").resize(big.size,Image.Resampling.NEAREST)
    strip.paste(big,(i*112*3,0),mask)
strip.save(OUT/"idle_walk_idle_strip.png")

gif_frames=[]
for im in sequence:
    canvas=Image.new("RGB",(112*3,88*3),(26,32,42))
    big=im.resize(canvas.size,Image.Resampling.NEAREST)
    canvas.paste(big,(0,0),im.getchannel("A").resize(canvas.size,Image.Resampling.NEAREST))
    gif_frames.append(canvas)
gif_frames[0].save(OUT/"idle_walk_idle.gif",save_all=True,append_images=gif_frames[1:],duration=130,loop=0,optimize=False)
