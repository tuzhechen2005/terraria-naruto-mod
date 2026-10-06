#!/usr/bin/env python3
"""Restore the approved Kakashi Idle eye pixels in six walk frames."""
from pathlib import Path
from PIL import Image, ImageChops, ImageDraw
import shutil

ROOT = Path(__file__).resolve().parent
SOURCE = ROOT.parent / 'npc-direct-pixel-round1' / 'kakashi_frames'
APPROVED = ROOT.parent / 'kakashi-direct-pixel-anim-v2' / 'frames' / 'Kakashi_00_Idle.png'
FRAMES = ROOT / 'frames'
FRAMES.mkdir(exist_ok=True)
DONOR_BOX = (36, 25, 47, 31)  # left, top, right-exclusive, bottom-exclusive
OFFSETS = {1:(3,2), 2:(2,2), 3:(2,2), 4:(2,0), 5:(1,0), 6:(2,0)}

def hard_alpha(image):
    for p in image.getdata():
        if p[3] not in (0,255) or (p[3] == 0 and p[:3] != (0,0,0)):
            return False
    return True

donor = Image.open(APPROVED).convert('RGBA')
assert donor.size == (80,80) and hard_alpha(donor)
assert ImageChops.difference(donor, Image.open(SOURCE / 'Kakashi_00_Idle.png').convert('RGBA')).getbbox() is None
patch = donor.crop(DONOR_BOX)
shutil.copy2(APPROVED, ROOT / 'approved_idle_source.png')
rows=[]
for path in sorted(SOURCE.glob('*.png')):
    src = Image.open(path).convert('RGBA')
    assert src.size == (80,80) and hard_alpha(src)
    index = int(path.stem.split('_')[1]) if path.stem.split('_')[1].isdigit() else None
    if index in OFFSETS:
        dx,dy = OFFSETS[index]
        box = (DONOR_BOX[0]+dx,DONOR_BOX[1]+dy,DONOR_BOX[2]+dx,DONOR_BOX[3]+dy)
        out = src.copy()
        for py in range(patch.height):
            for px in range(patch.width):
                tx,ty=box[0]+px,box[1]+py
                donor_pixel=patch.getpixel((px,py))
                target_pixel=src.getpixel((tx,ty))
                if donor_pixel[3] == 255 and target_pixel[3] == 255:
                    out.putpixel((tx,ty),donor_pixel)
        changed_xy=[(x,y) for y in range(80) for x in range(80) if src.getpixel((x,y)) != out.getpixel((x,y))]
        assert changed_xy and all(box[0]<=x<box[2] and box[1]<=y<box[3] for x,y in changed_xy)
        bounds=(min(x for x,y in changed_xy),min(y for x,y in changed_xy),max(x for x,y in changed_xy)+1,max(y for x,y in changed_xy)+1)
        assert src.getchannel('A').tobytes()==out.getchannel('A').tobytes(), 'silhouette/alpha changed'
        assert hard_alpha(out)
        out.save(FRAMES/path.name)
        count=len(changed_xy)
        rows.append((path.stem, box, bounds, count))
    else:
        shutil.copy2(path, FRAMES/path.name)
        assert (FRAMES/path.name).read_bytes()==path.read_bytes()
assert len(list(FRAMES.glob('*.png')))==14

# Contact sheets preserve source pixels; enlargement uses nearest neighbour.
ids = [0,1,2,3,4,5,6]
frames = [Image.open(FRAMES / f'Kakashi_{i:02d}_{"Idle" if i==0 else "Walk"}.png').convert('RGBA') for i in ids]
befores = [Image.open(SOURCE / f'Kakashi_{i:02d}_{"Idle" if i==0 else "Walk"}.png').convert('RGBA') for i in ids]
BG=(48,55,65,255)
def paste_rgba(canvas, im, xy):
    canvas.alpha_composite(im,xy)

def sheet_crop(items, scale, crop_box, name):
    w=(crop_box[2]-crop_box[0])*scale
    h=(crop_box[3]-crop_box[1])*scale
    gap=8
    canvas=Image.new('RGBA',(len(items)*(w+gap)+gap,h+2*gap),BG)
    for j,im in enumerate(items):
        tile=im.crop(crop_box).resize((w,h),Image.Resampling.NEAREST)
        paste_rgba(canvas,tile,(gap+j*(w+gap),gap))
    canvas.convert('RGB').save(ROOT/name)

sheet_crop(frames,8,(32,20,54,39),'eyes_after_8x.png')
sheet_crop(befores+frames,8,(32,20,54,39),'eyes_before_after_8x.png')
sheet_crop(frames,1,(0,0,80,80),'walk_full_1x.png')
sheet_crop(frames,3,(0,0,80,80),'walk_full_3x.png')
frames[1].save(ROOT/'walk_01_game_size.png')
frames[1].resize((240,240),Image.Resampling.NEAREST).save(ROOT/'walk_01_3x.png')
gif_frames=[]
for frame in frames[1:]:
    gif_frame=Image.new('RGBA',(80,80),BG)
    gif_frame.alpha_composite(frame)
    gif_frames.append(gif_frame.convert('RGB').resize((240,240),Image.Resampling.NEAREST))
gif_frames[0].save(ROOT/'walk_cycle.gif',save_all=True,append_images=gif_frames[1:],duration=130,loop=0)
for name, box, bounds, count in rows:
    print(f'{name}: ROI {box}; changed bounds {bounds}; changed pixels {count}')
print('14 frames: 80x80 RGBA; binary alpha and zero transparent RGB; 8 untouched frames byte-identical')
