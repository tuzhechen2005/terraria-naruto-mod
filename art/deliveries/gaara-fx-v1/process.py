from pathlib import Path
from PIL import Image, ImageDraw
import shutil

OUT = Path(__file__).parent
GEN = Path('/Users/tuzhechen/.codex/generated_images/01a0fb20-5131-7860-af36-0b64647b0433')
JOBS = [
    ('SandWall', 'exec-b9f2f5a5-5536-44c4-8ade-c8b5e2d8a8a0.png', 3, (40, 80)),
    ('SandShuriken', 'exec-9fd23fdc-d098-40eb-ae3e-eaa7b8e0f3c5.png', 2, (16, 16)),
    ('QuicksandMark', 'exec-5b119318-0c84-4940-8a72-bc33a6b520fc.png', 1, (48, 12)),
    ('SandPillar', 'exec-c0ee7b13-586f-47cd-92dd-88fc8efb0211.png', 3, (40, 96)),
    ('SandWave', 'exec-c2e74e82-a45c-4fa6-9cc8-c333b619e367.png', 3, (64, 40)),
]
# Palette taken from the gourd browns and light sand on Gaara_Idle_0.png.
PALETTE = [(24,18,20), (101,73,48), (144,101,60), (198,151,96), (235,194,136)]

def frame(src, idx, count, size, name):
    W,H = src.size
    seg = src.crop((round(idx*W/count),0,round((idx+1)*W/count),H))
    alpha=seg.getchannel('A')
    mask=alpha.point(lambda x: 255 if x >= 32 else 0)
    box=mask.getbbox()
    if not box: raise RuntimeError(f'empty {name} {idx}')
    crop=seg.crop(box)
    artw,arth=size[0]//2,size[1]//2
    pad = 1 if artw>10 else 0
    fitw,fith=artw-2*pad,arth-2*pad
    scale=min(fitw/crop.width,fith/crop.height)
    sw,sh=max(1,round(crop.width*scale)),max(1,round(crop.height*scale))
    small=crop.resize((sw,sh),Image.Resampling.BOX)
    p=small.load()
    for y in range(sh):
      for x in range(sw):
        r,g,b,a=p[x,y]
        if a<100:
          p[x,y]=(0,0,0,0)
          continue
        lum=(r*0.299+g*0.587+b*0.114)
        # Hard pixel clusters, color steps tied to Gaara's gourd.
        ix=0 if lum<62 else 1 if lum<105 else 2 if lum<151 else 3 if lum<203 else 4
        p[x,y]=(*PALETTE[ix],255)
    canvas=Image.new('RGBA',(artw,arth))
    x=(artw-sw)//2
    y=arth-sh-pad if name in ('SandWall','SandPillar','SandWave') else (arth-sh)//2
    canvas.alpha_composite(small,(x,y))
    if name == 'QuicksandMark':
        # Restore the generated oval after reduction to only six art pixels tall.
        canvas=Image.new('RGBA',(artw,arth))
        d=ImageDraw.Draw(canvas)
        dark=(*PALETTE[1],255); mid=(*PALETTE[2],255)
        gold=(*PALETTE[3],255); light=(*PALETTE[4],255)
        d.line([(3,2),(5,1),(9,0)],fill=mid)
        d.line([(12,0),(18,1),(21,2)],fill=gold)
        d.point([(6,1),(7,1),(14,1),(15,1),(20,2)],fill=light)
        d.point([(1,3),(2,3),(21,3),(22,3)],fill=dark)
        d.line([(3,4),(8,5),(16,5),(21,4)],fill=mid)
        d.line([(5,4),(11,4),(18,4)],fill=gold)
        d.point([(0,4),(8,2),(11,2),(15,3),(23,4)],fill=gold)
    return canvas.resize(size,Image.Resampling.NEAREST)

frames=[]
for name,file,count,size in JOBS:
    shutil.copyfile(GEN/file,OUT/'source'/f'{name}_generated.png')
    src=Image.open(GEN/file).convert('RGBA')
    for i in range(count):
        fn=f'Fx{name}' + (f'_{i}' if count>1 else '') + '.png'
        im=frame(src,i,count,size,name)
        im.save(OUT/fn)
        frames.append((fn,im))

# Contact sheet: native-scale strips and 4x nearest-neighbor on dark/light tiles.
row_names=[job[0] for job in JOBS]
width=1860
row_heights=[max(job[3][1]*4+job[3][1]+46,160) for job in JOBS]
preview=Image.new('RGB',(width,sum(row_heights)+42),(32,35,39))
d=ImageDraw.Draw(preview)
y=12
for (name,_,count,size),rh in zip(JOBS,row_heights):
    d.text((10,y),f'Fx{name}: 1x / 4x',fill=(240,240,232))
    native=[(fn,im) for fn,im in frames if fn.startswith('Fx'+name)]
    left=150
    for bg in [(52,57,64),(226,222,209)]:
        panel=Image.new('RGB',(835,rh-20),bg)
        px=6
        for fn,im in native:
            panel.paste(im,(px,6),im)
            big=im.resize((im.width*4,im.height*4),Image.Resampling.NEAREST)
            panel.paste(big,(px,im.height+12),big)
            px+=max(im.width*4+8,52)
        preview.paste(panel,(left,y+18))
        left+=843
    y+=rh
preview.save(OUT/'preview.png')
print('wrote',len(frames),'frames')
