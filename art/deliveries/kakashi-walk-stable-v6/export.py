"""Reproducible extraction of the generated six-pose Kakashi walk sheet."""
from pathlib import Path
from PIL import Image
import numpy as np
import shutil

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[2]
SOURCE = HERE / 'source_walk_sheet.png'
IDLE = ROOT / 'art/deliveries/kakashi-direct-pixel-anim-v2/frames/Kakashi_00_Idle.png'
OLD = IDLE.parent
BEFORE = ROOT / 'art/deliveries/kakashi-eye-restore-v5/frames'
FRAMES = HERE / 'frames'
RUNS = [(87,338),(446,707),(803,1042),(1145,1395),(1492,1755),(1856,2085)]

def rgba_hard(im):
    a = np.asarray(im.convert('RGBA')).copy()
    a[:,:,3] = np.where(a[:,:,3] >= 128, 255, 0)
    a[a[:,:,3] == 0,:3] = 0
    return Image.fromarray(a)

def panel(im, x0, x1):
    # Fixed source vertical crop and one common scale for all six poses.
    p = im.crop((x0, 126, x1+1, 648))
    w = round(p.width * 62 / 522)
    p = rgba_hard(p.resize((w,62),Image.Resampling.NEAREST))
    out = Image.new('RGBA',(80,80))
    # Place every pose on a common pelvis axis and foot baseline.
    out.paste(p,(37 - w//2,14),p)
    return out

def backdrop(im, scale, color):
    b=Image.new('RGBA',(80,80),color)
    b.alpha_composite(im)
    return b.convert('RGB').resize((80*scale,80*scale),Image.Resampling.NEAREST)

def main():
    FRAMES.mkdir(exist_ok=True)
    src=Image.open(SOURCE).convert('RGBA')
    idle=Image.open(IDLE).convert('RGBA')
    poses=[]
    for i,(x0,x1) in enumerate(RUNS,1):
        p=panel(src,x0,x1)
        # Remove generated head, neck and collar. Reuse the approved Idle block
        # at exactly the same coordinates in every frame, including its alpha.
        a=np.asarray(p).copy()
        a[14:37,20:52]=0
        p=Image.fromarray(a)
        p.alpha_composite(idle.crop((20,14,52,37)),(20,14))
        p=rgba_hard(p)
        p.save(FRAMES/f'Kakashi_{i:02d}_Walk.png')
        poses.append(p)
    for name in ['Kakashi_00_Idle','Kakashi_07_Jump','Kakashi_08_Sit',
                 'Kakashi_09_Throw','Kakashi_10_Throw','Kakashi_11_Throw',
                 'Kakashi_Trapped_0','Kakashi_Trapped_1']:
        shutil.copyfile(OLD/(name+'.png'),FRAMES/(name+'.png'))
    strip=Image.new('RGBA',(480,80))
    for i,p in enumerate(poses): strip.alpha_composite(p,(80*i,0))
    strip.save(HERE/'walk_strip.png')
    # Build one palette from the full cycle, then apply it to every GIF frame.
    dark_tiles=[backdrop(p,1,'#303740') for p in poses]
    palette_strip=Image.new('RGB',(480,80))
    for i,t in enumerate(dark_tiles): palette_strip.paste(t,(80*i,0))
    palette=palette_strip.quantize(colors=256,method=Image.Quantize.MEDIANCUT)
    for scale in (1,3):
        tiles=[t.quantize(palette=palette).resize((80*scale,80*scale),Image.Resampling.NEAREST)
               for t in dark_tiles]
        tiles[0].save(HERE/f'walk_{scale}x.gif',save_all=True,append_images=tiles[1:],duration=140,
                      loop=0,disposal=2,optimize=False)
    poses[0].save(HERE/'walk_lossless.apng',save_all=True,append_images=poses[1:],
                  duration=140,loop=0,disposal=2,blend=0)
    comp=Image.new('RGB',(480,160),'#303740')
    for i,p in enumerate(poses):
        old=Image.open(BEFORE/f'Kakashi_{i+1:02d}_Walk.png').convert('RGBA')
        comp.paste(backdrop(old,1,'#303740'),(i*80,0))
        comp.paste(backdrop(p,1,'#303740'),(i*80,80))
    comp.save(HERE/'before_after.png')
    preview=Image.new('RGB',(480,720),'#303740')
    for i,p in enumerate(poses):
        preview.paste(backdrop(p,3,'#303740'),((i%2)*240,(i//2)*240))
    preview.save(HERE/'preview_3x.png')

if __name__ == '__main__': main()
