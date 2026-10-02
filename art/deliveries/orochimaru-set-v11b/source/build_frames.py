from pathlib import Path
from PIL import Image, ImageDraw, ImageFont
import numpy as np
import shutil
from scipy.ndimage import label

ROOT=Path(__file__).resolve().parents[1]
SRC=ROOT/'source'
BASE=Image.open(ROOT.parent/'orochimaru-base-v11'/'Orochimaru_Idle_0.png').convert('RGBA')
GEN=Path('/Users/tuzhechen/.codex/generated_images/01a0fd6b-3cd1-7f80-b38c-2284a93f706a')
SHEETS={
 'Hands':('exec-a0e4062f-7004-4fa0-89c8-769d028e7876.png',4,['HandsIn_0','Hands_0','Hands_1','Hands_2'],[(307,158),(810,159),(1329,158),(1971,158)]),
 'Dash':('exec-7416a3bd-ffe9-460a-be3f-a8ad8f5f1ec5.png',3,['DashIn_0','Dash_0','Dash_1'],[(456,357),(1304,443),(2104,488)]),
 'Wind':('exec-85d81841-3ae1-4c29-9f92-8867c39ea36f.png',3,['WindIn_0','Wind_0','Wind_1'],[(272,85),(1015,91),(2037,257)]),
 'Neck':('exec-467c6f44-0472-4cd2-8cc5-12dfce53bc46.png',2,['NeckIn_0','Neck_0'],[(768,228),None]),
 'Seal':('exec-7848ebdc-22e6-4820-9f12-289d2a59c38a.png',3,['SealIn_0','Seal_0','Seal_1'],[(443,145),(1112,160),(1827,202)]),
 'Summon':('exec-ac0d642b-93ef-443c-8343-f5c5551e22f6.png',3,['SummonIn_0','Summon_0','Summon_1'],[(497,151),(1250,235),(2011,320)]),
}
# Base artwork is the only palette source. Each generated pose supplies geometry.
head_positions={}
pal=np.array(sorted({p[:3] for n,p in (BASE.getcolors(100000) or []) if p[3]>0}),dtype=np.int32)
face=BASE.crop((108,33,122,59))
# Skin and eyes from the chosen base remain byte-for-byte unchanged after compositing.
for action,(fn,count,names,faces) in SHEETS.items():
 shutil.copy2(GEN/fn,SRC/f'{action}_generated.png')
 sheet=Image.open(GEN/fn).convert('RGBA')
 for i,(name,headpt) in enumerate(zip(names,faces)):
  # Imagegen arranged each action in equal-width cells.
  left=round(i*sheet.width/count); right=round((i+1)*sheet.width/count)
  panel=sheet.crop((left,0,right,sheet.height))
  a=np.asarray(panel.getchannel('A'))
  yy,xx=np.where(a>=128)
  if len(xx)==0: raise RuntimeError(name+' empty')
  box=(int(xx.min()),int(yy.min()),int(xx.max()+1),int(yy.max()+1))
  art=panel.crop(box)
  scale=.15 if action=='Neck' else (.16 if action=='Wind' else .17)
  art=art.resize((round(art.width*scale),round(art.height*scale)),Image.Resampling.NEAREST)
  arr=np.asarray(art).copy()
  opaque=arr[:,:,3]>=128
  colors=arr[:,:,:3].astype(np.int32)
  d=((colors[:,:,None,:]-pal[None,None,:,:])**2).sum(axis=3)
  mapped=pal[d.argmin(axis=2)].astype(np.uint8)
  arr[:,:,:3]=mapped;arr[:,:,3]=np.where(opaque,255,0).astype(np.uint8)
  art=Image.fromarray(arr,'RGBA')
  canvas=Image.new('RGBA',(224,136))
  # Align ground row and place each figure's visible silhouette near x=112.
  x=112-art.width//2; y=132-art.height
  canvas.alpha_composite(art,(x,y))
  if headpt is not None:
   # Face position in the resized image from source-sheet coordinates.
   fx=x+round((headpt[0]-left-box[0])*scale)
   fy=y+round((headpt[1]-box[1])*scale)
   # Clear generated face only, retaining action-specific hair and shoulder silhouette.
   pix=canvas.load()
   for py in range(max(0,fy-3),min(136,fy+22)):
    for px in range(max(0,fx-7),min(224,fx+13)):
     r,g,b,a0=pix[px,py]
     if a0 and r>55 and r>g*.92 and g>b*.82 and (r+g+b)>220:
      pix[px,py]=(0,0,0,0)
   # The chosen v11 head is pasted without resizing or recoloring.
   # Base face core has upper-left at (109,34); align it to face landmark.
   canvas.alpha_composite(face,(fx-11,fy-5))
   head_positions[name]=(fx,fy)
  else:
   # Neck_0 has an open collar and no pixels above it. Remove stray generated hair.
   pass
  # Keep lowest opaque pixel at exact game-ground row 131.
  bb=canvas.getbbox()
  if bb:
   shifted=Image.new('RGBA',(224,136))
   shifted.alpha_composite(canvas,(0,131-(bb[3]-1)))
   canvas=shifted
  # Retain the character's main connected silhouette; discard clipped pose-sheet fragments.
  aa=np.asarray(canvas.getchannel('A'))>0
  components,component_count=label(aa,np.ones((3,3),dtype=np.uint8))
  if component_count>1:
   sizes=np.bincount(components.ravel()); sizes[0]=0
   keep=components==sizes.argmax()
   cleaned=np.asarray(canvas).copy();cleaned[~keep]=0
   canvas=Image.fromarray(cleaned)
  canvas.save(ROOT/f'Orochimaru_{name}.png')

# One preview at actual pixel size and nearest-neighbor 3x, plus 4x head comparisons.
names=[n for _,(_,_,ns,_) in SHEETS.items() for n in ns]
thumbs=[('Base',BASE)]+[(n,Image.open(ROOT/f'Orochimaru_{n}.png').convert('RGBA')) for n in names]
cols=4; cellw=242; cellh=164
preview=Image.new('RGB',(cols*cellw,((len(thumbs)+cols-1)//cols)*cellh),'#30313a')
for k,(label,im) in enumerate(thumbs):
 x=(k%cols)*cellw;y=(k//cols)*cellh
 ImageDraw.Draw(preview).text((x+4,y+3),label,fill='white')
 bg=Image.new('RGB',im.size,'#55565c');bg.paste(im,mask=im.getchannel('A'))
 preview.paste(bg,(x+9,y+21))
preview.save(ROOT/'preview_1x.png')
big=preview.resize((preview.width*3,preview.height*3),Image.Resampling.NEAREST)
big.save(ROOT/'preview_3x.png')
combined=Image.new('RGB',(big.width,preview.height+big.height+30),'#30313a')
ImageDraw.Draw(combined).text((5,4),'1x',fill='white')
combined.paste(preview,(0,20))
ImageDraw.Draw(combined).text((5,preview.height+23),'3x',fill='white')
combined.paste(big,(0,preview.height+30))
combined.save(ROOT/'preview.png')
# Compare every frame head to the chosen base crop at exactly 4x.
base_head=BASE.crop((94,24,130,70))
headcmp=Image.new('RGB',(19*300,240),'#30313a')
for k,(label,im) in enumerate(thumbs):
 x=k*300
 ImageDraw.Draw(headcmp).text((x+2,2),label,fill='white')
 if k==0:
  c=base_head
 elif label=='Neck_0':
  c=im.crop((90,24,126,70))
 else:
  fx,fy=head_positions[label]
  c=im.crop((fx-18,fy-18,fx+18,fy+28))
 bg=Image.new('RGB',c.size,'#55565c');bg.paste(c,mask=c.getchannel('A'))
 headcmp.paste(bg.resize((c.width*4,c.height*4),Image.Resampling.NEAREST),(x,30))
headcmp.save(ROOT/'head_comparison_4x.png')
print('wrote',len(names),'frames')
