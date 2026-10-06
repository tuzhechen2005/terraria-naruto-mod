"""Postprocess one generated cycle, reusing the approved chest interior only.

No joint/limb geometry is replaced. Generated body colours share one palette;
head/neck/collar and the small stationary chest patch retain approved pixels.
"""
from pathlib import Path
from PIL import Image
import numpy as np
import shutil
import json
HERE=Path(__file__).resolve().parent
SRC=HERE.parent/'kakashi-walk-stable-v6/frames'
OUT=HERE/'kakashi_stable_frames'
ROI=(35,37,42,47)  # Chest interior, above pelvis, outside arms and contour.

def main():
    OUT.mkdir(exist_ok=True)
    idle=Image.open(SRC/'Kakashi_00_Idle.png').convert('RGBA')
    files=[SRC/f'Kakashi_{i:02d}_Walk.png' for i in range(1,7)]
    originals=[Image.open(p).convert('RGBA') for p in files]
    # Palette is derived from the whole generated body sequence; not six
    # separately quantized palettes. Background RGB is never used as paint.
    opaque=np.concatenate([np.asarray(im)[37:][np.asarray(im)[37:,:,3]==255,:3] for im in originals])
    swatch=Image.fromarray(opaque[None,:,:]).quantize(colors=32,method=Image.Quantize.MEDIANCUT)
    changed=[]
    for p,im in zip(files,originals):
        before=np.asarray(im).copy()
        a=np.asarray(im.convert('RGB').quantize(palette=swatch,dither=Image.Dither.NONE).convert('RGB')).copy()
        final=before.copy();final[37:,:,:3]=a[37:]
        x0,y0,x1,y1=ROI
        assert np.all(before[y0:y1,x0:x1,3]==255)
        assert np.all(np.asarray(idle)[y0:y1,x0:x1,3]==255)
        final[y0:y1,x0:x1]=np.asarray(idle)[y0:y1,x0:x1]
        final[final[:,:,3]==0,:3]=0
        assert np.array_equal(final[:,:,3],before[:,:,3])
        assert np.array_equal(final[:37],before[:37])
        Image.fromarray(final).save(OUT/p.name)
        changed.append(int(np.any(before!=final,axis=2).sum()))
    for p in SRC.glob('*.png'):
        if '_Walk' not in p.name: shutil.copyfile(p,OUT/p.name)
    (HERE/'kakashi_stability.json').write_text(json.dumps({'generated_input':str(SRC),'final_frames':str(OUT),
        'approved_head_region':[20,14,52,37],'approved_chest_interior':list(ROI),
        'shared_generated_body_palette_colors':32,'alpha_geometry_unchanged':True,'changed_pixels':changed},indent=2)+'\n')
if __name__=='__main__':main()
