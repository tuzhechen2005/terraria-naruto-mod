from pathlib import Path
from collections import Counter
from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parents[3]
OUT = Path(__file__).resolve().parent
K = ROOT/'art/deliveries/kakashi-npc-v4/source'
T = ROOT/'art/deliveries/tazuna-npc-v1/source/Tazuna_Idle_Walk.png'
S = OUT/'source'
P = {
 'o':(9,7,13,255), 'k':(24,16,17,255), 'h':(39,25,23,255),
 'd':(31,33,58,255), 'b':(25,27,48,255),
 'm':(139,136,144,255), 'l':(217,212,222,255),
 's':(235,166,112,255), 't':(185,112,76,255),
 'r':(117,62,48,255), 'e':(24,16,17,255),
 'p':(202,193,197,255), 'g':(72,98,42,255),
}
HAIR={(217,212,222),(139,136,144),(202,193,197),(221,217,225)}

def px(im,x,y,c):
 if 0<=x<28 and 0<=y<25: im.putpixel((x,y),P[c])

def head(tile):
 pts=[(x,y) for y in range(16) for x in range(28)
      if tile.getpixel((x,y))[3] and tile.getpixel((x,y))[:3] in HAIR]
 ax=min(x for x,y in pts); ay=min(y for x,y in pts)
 # Remove the old hair, mask, and angled forehead protector within the head.
 # The replacement is constructed one art pixel at a time on Kakashi's body.
 for y in range(max(0,ay-1),min(25,ay+7)):
  for x in range(max(0,ax-2),min(28,ax+8)):
   tile.putpixel((x,y),(0,0,0,0))
 # High ponytail: compact two-pixel strand, raised above the skull.
 for dx,dy,c in [(-4,-1,'o'),(-3,-1,'k'),(-2,-1,'o'),
                 (-4,0,'o'),(-3,0,'h'),(-2,0,'k'),
                 (-3,1,'o'),(-2,1,'k'),(-1,1,'o'),(-1,2,'k')]: px(tile,ax+dx,ay+dy,c)
 # Hair cap and its dark outside contour.
 for dx in (0,1,2,3): px(tile,ax+dx,ay-1,'o')
 for dx in (-1,0,1,2,3,4): px(tile,ax+dx,ay,'h' if dx in (0,1) else 'k')
 px(tile,ax-1,ay,'o');px(tile,ax+5,ay,'o')
 for dx in (-1,0,1,2,3,4,5): px(tile,ax+dx,ay+1,'h' if dx in (0,1,2) else 'k')
 px(tile,ax-1,ay+1,'o');px(tile,ax+6,ay+1,'o')
 # Blue cloth band and bright metal plate, horizontally above the eyes.
 for dx in range(0,7): px(tile,ax+dx,ay+2,'d')
 px(tile,ax-1,ay+2,'o');px(tile,ax+7,ay+2,'o')
 for dx in range(0,7): px(tile,ax+dx,ay+3,'b')
 for dx in (2,3,4,5): px(tile,ax+dx,ay+3,'m')
 px(tile,ax+4,ay+3,'l')
 px(tile,ax-1,ay+3,'o');px(tile,ax+7,ay+3,'o')
 # Two eyes, two skin tones, the horizontal bridge-of-nose scar, and jaw.
 for dy in (4,5):
  for dx in range(0,7): px(tile,ax+dx,ay+dy,'s' if dx<4 else 't')
  px(tile,ax-1,ay+dy,'o');px(tile,ax+7,ay+dy,'o')
 px(tile,ax+2,ay+4,'e');px(tile,ax+5,ay+4,'e')
 for dx in (2,3,4,5): px(tile,ax+dx,ay+5,'r')
 for dx in range(1,7): px(tile,ax+dx,ay+6,'t' if dx>3 else 's')
 px(tile,ax,ay+6,'o');px(tile,ax+7,ay+6,'o')
 return tile

def sheet(src, count):
 im=Image.open(K/src).convert('RGBA')
 assert im.size==(224*count,200)
 result=Image.new('RGBA',im.size)
 for j in range(count):
  tile=im.crop((224*j,0,224*(j+1),200)).resize((28,25),Image.Resampling.NEAREST)
  tile=head(tile)
  result.paste(tile.resize((224,200),Image.Resampling.NEAREST),(224*j,0))
 return result

walk=sheet('Kakashi_Idle_Walk.png',7)
actions=sheet('Kakashi_Idle_Jump_Sit_Throw.png',6)
# Sit: a small open mission scroll in the hands. Existing seated body stays intact.
tile=actions.crop((448,0,672,200)).resize((28,25),Image.Resampling.NEAREST)
for x,y,c in [(15,14,'e'),(16,14,'p'),(17,14,'l'),(18,14,'p'),(19,14,'e'),
              (15,15,'e'),(16,15,'l'),(17,15,'p'),(18,15,'l'),(19,15,'e'),
              (16,16,'r'),(19,16,'r')]:px(tile,x,y,c)
actions.paste(tile.resize((224,200),Image.Resampling.NEAREST),(448,0))
# Talk: preserve Kakashi's idle stance while changing only forearm/scroll pixels.
talk=Image.new('RGBA',(672,200))
idle=walk.crop((0,0,224,200)).resize((28,25),Image.Resampling.NEAREST)
for j in range(3):
 tile=idle.copy()
 if j:
  # The left hand holds a rolled mission document; the right hand rises on frame 2.
  for x,y,c in [(9,14,'e'),(10,14,'p'),(11,14,'l'),(12,14,'p'),(13,14,'e'),
                (9,15,'e'),(10,15,'l'),(11,15,'p'),(12,15,'l'),(13,15,'e')]: px(tile,x,y,c)
 if j==1:
  for x,y,c in [(18,12,'o'),(19,12,'s'),(19,13,'t'),(18,13,'d')]:px(tile,x,y,c)
 if j==2:
  for x,y,c in [(18,11,'o'),(19,11,'s'),(20,11,'t'),(18,12,'d'),(19,12,'s'),(18,13,'d')]:px(tile,x,y,c)
 talk.paste(tile.resize((224,200),Image.Resampling.NEAREST),(224*j,0))

for name,im in [('Iruka_Idle_Walk.png',walk),('Iruka_Idle_Jump_Sit_Throw.png',actions),('Iruka_Talk.png',talk)]: im.save(S/name)

# Comparison on neutral dark and light grounds at exact art scale and 4x.
taz=Image.open(T).convert('RGBA').crop((0,0,224,200)).resize((28,25),Image.Resampling.NEAREST)
kak=Image.open(K/'Kakashi_Idle_Walk.png').convert('RGBA').crop((0,0,224,200)).resize((28,25),Image.Resampling.NEAREST)
iru=walk.crop((0,0,224,200)).resize((28,25),Image.Resampling.NEAREST)
for scale in (1,4):
 w,h=3*(28*scale+20)+20,25*scale+56
 canvas=Image.new('RGB',(w,h),(43,52,63))
 d=ImageDraw.Draw(canvas)
 for j,(label,sprite) in enumerate([('Tazuna',taz),('Kakashi',kak),('Iruka',iru)]):
  x=20+j*(28*scale+20)
  d.text((x,9),label,fill=(235,239,242))
  tile=sprite.resize((28*scale,25*scale),Image.Resampling.NEAREST)
  canvas.paste(tile,(x,32),tile)
 canvas.save(OUT/f'Iruka_comparison_{scale}x.png')
