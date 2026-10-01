from PIL import Image, ImageDraw, ImageFont
from pathlib import Path
import math
ROOT=Path(__file__).resolve().parents[1]
BASE=ROOT.parent/'orochimaru-style-v3'
SRC=ROOT/'source'
W,H=112,88
idle=Image.open(BASE/'Orochimaru_Idle.png').convert('RGBA')
disguise=Image.open(BASE/'Orochimaru_Disguise.png').convert('RGBA')
sets=[
 ('idle_walk_generated.png',[(0,290),(290,550),(550,810),(810,1090),(1090,1360),(1360,1630),(1630,1910),(1910,2172)],['Orochimaru_Idle_0','Orochimaru_Idle_1','Orochimaru_Idle_2','Orochimaru_Idle_3','Orochimaru_Walk_0','Orochimaru_Walk_1','Orochimaru_Walk_2','Orochimaru_Walk_3'],334),
 ('disguise_reveal_generated.png',[(0,260),(260,550),(550,830),(830,1110),(1110,1420),(1420,1660),(1660,1910),(1910,2172)],['Orochimaru_Disguise_0','Orochimaru_DisguiseWalk_0','Orochimaru_DisguiseWalk_1','Orochimaru_DisguiseWalk_2','Orochimaru_DisguiseWalk_3','Orochimaru_Reveal_0','Orochimaru_Reveal_1','Orochimaru_Reveal_2'],381),
 ('emerge_sink_hurt_generated.png',[(0,315),(315,640),(640,900),(900,1240),(1240,1550),(1550,1830),(1830,2200)],['Orochimaru_Emerge_0','Orochimaru_Emerge_1','Orochimaru_Emerge_2','Orochimaru_Sink_0','Orochimaru_Sink_1','Orochimaru_Sink_2','Orochimaru_Hurt_0'],399)]
# Reference colors keep game-scale art tied to the approved v3 sprites.
palette=[]
for ref in (idle,disguise):
 for r,g,b,a in ref.getdata():
  if a==255 and (r,g,b) not in palette:palette.append((r,g,b))
def nearest(c):
 r,g,b=c
 return min(palette,key=lambda q: 2*(r-q[0])**2+3*(g-q[1])**2+2*(b-q[2])**2)
def create(im,window,full_h,name):
 x0,x1=window
 sub=im.crop((x0,0,x1,im.height))
 mask=sub.getchannel('A').point(lambda a:255 if a>=110 else 0)
 box=mask.getbbox()
 if not box:raise ValueError(name)
 sub=sub.crop(box)
 scale=62/full_h
 ah=max(1,round(sub.height*scale/2));aw=max(1,round(sub.width*scale/2))
 if name.startswith('Orochimaru_Reveal_') or name=='Orochimaru_Sink_0':ah=min(31,ah)
 sub=sub.resize((aw,ah),Image.Resampling.LANCZOS)
 pix=Image.new('RGBA',(aw,ah))
 src=sub.load();dst=pix.load()
 for y in range(ah):
  for x in range(aw):
   r,g,b,a=src[x,y]
   if a>=105:
    cr,cg,cb=nearest((r,g,b));dst[x,y]=(cr,cg,cb,255)
 pix=pix.resize((aw*2,ah*2),Image.Resampling.NEAREST)
 out=Image.new('RGBA',(W,H))
 px=2*round((56-aw)/2)
 py=84-ah*2
 out.alpha_composite(pix,(px,py))
 if name in ('Orochimaru_Hurt_0','Orochimaru_Sink_2'):
  small=out.resize((56,44),Image.Resampling.NEAREST)
  visited=set();components=[]
  for yy in range(44):
   for xx in range(56):
    if (xx,yy) in visited or small.getpixel((xx,yy))[3]==0:continue
    stack=[(xx,yy)];visited.add((xx,yy));component=[]
    while stack:
     point=stack.pop();component.append(point)
     x,y=point
     for nxt in ((x-1,y),(x+1,y),(x,y-1),(x,y+1)):
      if 0<=nxt[0]<56 and 0<=nxt[1]<44 and nxt not in visited and small.getpixel(nxt)[3]:
       visited.add(nxt);stack.append(nxt)
    components.append(component)
  keep=set(max(components,key=len))
  for yy in range(44):
   for xx in range(56):
    if small.getpixel((xx,yy))[3] and (xx,yy) not in keep:small.putpixel((xx,yy),(0,0,0,0))
  out=small.resize((W,H),Image.Resampling.NEAREST)
 return out
def idle_sway(index):
 if index==0:return idle.copy()
 art=idle.resize((56,44),Image.Resampling.NEAREST)
 pixels=art.load()
 # One-pixel contour changes keep the approved face, stance, and rope fixed.
 changes={1:[(19,23,20,23),(35,27,34,27)],2:[(20,25,21,25),(35,26,34,26),(35,27,34,27)],3:[(19,24,20,24),(20,25,21,25),(35,28,34,28)]}
 for x,y,sx,sy in changes[index]:pixels[x,y]=pixels[sx,sy]
 return art.resize((W,H),Image.Resampling.NEAREST)
names=[]
for source,windows,labels,full_h in sets:
 im=Image.open(SRC/source).convert('RGBA')
 for win,name in zip(windows,labels):
  if name.startswith('Orochimaru_Idle_'):frame=idle_sway(int(name[-1]))
  elif name=='Orochimaru_Disguise_0':frame=disguise
  elif name=='Orochimaru_Emerge_2':frame=idle
  else:frame=create(im,win,full_h,name)
  frame.save(ROOT/(name+'.png'))
  names.append(name)
# Preview: each action in its own row, 1x and 4x, approved idle reference at the top.
rows=[('v3 reference',['Orochimaru_Idle_0']),('Idle',[f'Orochimaru_Idle_{i}' for i in range(4)]),('Walk',[f'Orochimaru_Walk_{i}' for i in range(4)]),('Disguise',['Orochimaru_Disguise_0']),('DisguiseWalk',[f'Orochimaru_DisguiseWalk_{i}' for i in range(4)]),('Reveal',[f'Orochimaru_Reveal_{i}' for i in range(3)]),('Emerge',[f'Orochimaru_Emerge_{i}' for i in range(3)]),('Sink',[f'Orochimaru_Sink_{i}' for i in range(3)]),('Hurt',['Orochimaru_Hurt_0'])]
preview=Image.new('RGB',(1100,len(rows)*490),(28,34,44))
d=ImageDraw.Draw(preview)
for ri,(label,frames) in enumerate(rows):
 top=ri*490;d.text((12,top+12),label,fill=(245,245,245))
 for j,name in enumerate(frames):
  sprite=Image.open(ROOT/(name+'.png')).convert('RGBA')
  x=150+j*185
  preview.paste(sprite,(x,top+38),sprite)
  big=sprite.resize((448,352),Image.Resampling.NEAREST)
  # 4x frames overlap if placed at full canvas width, so use a narrower crop of sprite center.
  crop=big.crop((112,0,336,352))
  preview.paste(crop,(x-55,top+130),crop)
  d.text((x,top+120),name.replace('Orochimaru_',''),fill=(220,220,220))
preview.save(ROOT/'preview.png')
print(len(names),'frames',preview.size)
