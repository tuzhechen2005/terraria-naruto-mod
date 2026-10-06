import collections
import colorsys
import json
import math
import struct
import subprocess
import zlib
from pathlib import Path

ROOT = Path(__file__).parent
NAMES = ('Hiruzen', 'Anko', 'Hayate', 'Ibiki')

def read_png(path):
    meta = json.loads(subprocess.check_output(['ffprobe','-v','error','-select_streams','v:0','-show_entries','stream=width,height','-of','json',str(path)]))['streams'][0]
    w, h = meta['width'], meta['height']
    raw = subprocess.check_output(['ffmpeg','-v','error','-i',str(path),'-f','rawvideo','-pix_fmt','rgba','-'])
    return w, h, [tuple(raw[i:i+4]) for i in range(0,len(raw),4)]

def write_png(path, w, h, pixels):
    raw = b''.join(b'\0' + bytes(v for p in pixels[y*w:(y+1)*w] for v in p) for y in range(h))
    def chunk(t, d):
        return struct.pack('>I',len(d))+t+d+struct.pack('>I',zlib.crc32(t+d)&0xffffffff)
    path.write_bytes(b'\x89PNG\r\n\x1a\n'+chunk(b'IHDR',struct.pack('>IIBBBBB',w,h,8,6,0,0,0))+chunk(b'IDAT',zlib.compress(raw,9))+chunk(b'IEND',b''))

def palette20(pixels, limit=20):
    counts=collections.Counter(p[:3] for p in pixels if p[3])
    boxes=[list(counts)]
    while len(boxes)<limit:
        scores=[]
        for i,box in enumerate(boxes):
            ranges=[max(c[k] for c in box)-min(c[k] for c in box) for k in range(3)]
            scores.append((max(ranges)*sum(counts[c] for c in box),i,ranges.index(max(ranges))))
        score,i,axis=max(scores)
        if score==0 or len(boxes[i])<2: break
        box=sorted(boxes.pop(i),key=lambda c:c[axis]); total=sum(counts[c] for c in box); n=0
        for cut,c in enumerate(box):
            n+=counts[c]
            if n>=total/2: break
        cut=min(max(cut+1,1),len(box)-1)
        boxes.extend((box[:cut],box[cut:]))
    palette=[]
    for box in boxes:
        total=sum(counts[c] for c in box)
        palette.append(tuple(round(sum(c[k]*counts[c] for c in box)/total) for k in range(3)))
    return palette

def nearest(c,palette):
    return min(palette,key=lambda p:sum((c[k]-p[k])**2 for k in range(3)))

def make_sprite(name):
    w,h,src=read_png(ROOT/'source'/f'{name}_source.png')
    opaque=[(i%w,i//w) for i,p in enumerate(src) if p[3]>=128]
    x0=min(x for x,y in opaque);x1=max(x for x,y in opaque)
    y0=min(y for x,y in opaque);y1=max(y for x,y in opaque)
    height=61; width=round((x1-x0+1)*height/(y1-y0+1))
    left=(80-width)//2; top=77-height
    out=[(0,0,0,0)]*(80*80)
    for yy in range(height):
        sy=min(y1, y0+int((yy+.5)*(y1-y0+1)/height))
        for xx in range(width):
            sx=min(x1, x0+int((xx+.5)*(x1-x0+1)/width))
            r,g,b,a=src[sy*w+sx]
            if a>=128:
                if name=='Hayate':
                    hue,sat,val=colorsys.rgb_to_hsv(r/255,g/255,b/255)
                    if .12 <= hue <= .48 and sat>.14 and val<.75:
                        r,g,b=(round(val*155),round(val*171),round(val*215))
                out[(top+yy)*80+left+xx]=(r,g,b,255)
    pal=palette20(out, 12 if name=='Anko' else 20)
    out=[(*nearest(p[:3],pal),255) if p[3] else (0,0,0,0) for p in out]
    # Replace singleton color specks with the closest adjacent color cluster.
    for _ in range(12):
        changes=0
        for y in range(80):
            for x in range(80):
                c=out[y*80+x]
                if not c[3]: continue
                neighbors=[out[(y+dy)*80+x+dx] for dx,dy in ((1,0),(-1,0),(0,1),(0,-1)) if 0<=x+dx<80 and 0<=y+dy<80 and out[(y+dy)*80+x+dx][3]]
                if any(sum(abs(c[k]-q[k]) for k in range(3))<=24 for q in neighbors):continue
                if neighbors:
                    q=min(neighbors,key=lambda q:sum((c[k]-q[k])**2 for k in range(3)))
                    out[y*80+x]=q; changes+=1
                else: out[y*80+x]=(0,0,0,0); changes+=1
        if not changes:break
    if name=='Ibiki':
        scar=(148,103,82,255)
        for x,y in ((42,26),(42,27),(41,28),(41,29),(40,30),(40,31),(39,32),(39,33),
                    (43,34),(44,34),(45,34),(46,34),(47,34),(48,34)):
            if out[y*80+x][3]:out[y*80+x]=scar
    if name=='Anko':
        smile=(40,25,48,255)
        for x,y in ((44,36),(45,37),(46,37),(47,37),(48,36)):
            if out[y*80+x][3]:out[y*80+x]=smile
    write_png(ROOT/f'{name}_Idle.png',80,80,out)
    return out

def checker(w,h,cell=8):
    return [(58,58,66,255) if ((x//cell+y//cell)%2)==0 else (78,78,87,255) for y in range(h) for x in range(w)]

def paste(dst,dw,dh,src,sw,sh,ox,oy,scale=1):
    for y in range(sh):
        for x in range(sw):
            p=src[y*sw+x]
            if p[3]<128:continue
            for yy in range(scale):
                dy=oy+y*scale+yy
                if not 0<=dy<dh:continue
                for xx in range(scale):
                    dx=ox+x*scale+xx
                    if 0<=dx<dw:dst[dy*dw+dx]=p

sprites={n:make_sprite(n) for n in NAMES}
accepted=Path('art/deliveries/npc-hires-accepted')
other=[]
for file in ('Tazuna_Idle.png','Iruka_Idle.png','Kakashi_Idle.png','HakuForest_Idle.png','ToolShopkeeper_Idle.png'):
    w,h,p=read_png(accepted/file)
    assert (w,h)==(80,80)
    other.append(p)
all_sprites=[sprites[n] for n in NAMES]+other

pw,ph=9*80*3,340
preview=checker(pw,ph,16)
for i,p in enumerate(all_sprites):
    paste(preview,pw,ph,p,80,80,i*80,0,1)
    paste(preview,pw,ph,p,80,80,i*240,96,3)
write_png(ROOT/'preview.png',pw,ph,preview)

# Four face crops, 8x, with one-logical-pixel grid and logical row numbers.
fw,fh=4*472,264
faces=checker(fw,fh,16)
digits={'0':('111','101','101','101','111'),'1':('010','110','010','010','111'),'2':('111','001','111','100','111'),'3':('111','001','111','001','111'),'4':('101','101','111','001','001'),'5':('111','100','111','001','111'),'6':('111','100','111','101','111'),'7':('111','001','010','010','010'),'8':('111','101','111','101','111'),'9':('111','101','111','001','111')}
for i,n in enumerate(NAMES):
    p=sprites[n]; ox=i*472+28
    for row,y in enumerate(range(16,46)):
        for col,x in enumerate(range(12,67)):
            c=p[y*80+x]
            if c[3]:
                for yy in range(8):
                    for xx in range(8): faces[(row*8+yy)*fw+ox+col*8+xx]=c
            for xx in range(8):faces[(row*8)*fw+ox+col*8+xx]=(85,85,94,255)
            for yy in range(8):faces[(row*8+yy)*fw+ox+col*8]=(85,85,94,255)
        if row%2==0:
            for j,d in enumerate(str(y)):
                for yy,line in enumerate(digits[d]):
                    for xx,ch in enumerate(line):
                        if ch=='1':faces[(row*8+1+yy)*fw+i*472+4+j*4+xx]=(245,245,245,255)
write_png(ROOT/'faces_8x.png',fw,fh,faces)

for n,p in sprites.items():
    colors={c[:3] for c in p if c[3]}
    opaque=sum(c[3]>0 for c in p)
    isolated=0
    for y in range(80):
        for x in range(80):
            c=p[y*80+x]
            if not c[3]:continue
            if not any(0<=x+dx<80 and 0<=y+dy<80 and p[(y+dy)*80+x+dx][3] and sum(abs(c[k]-p[(y+dy)*80+x+dx][k]) for k in range(3))<=24 for dx,dy in ((1,0),(-1,0),(0,1),(0,-1))): isolated+=1
    print(n,'colors',len(colors),'opaque',opaque,'isolated',isolated, f'{isolated/opaque*100:.1f}%', 'alpha',{c[3] for c in p})
