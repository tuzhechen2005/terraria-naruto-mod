import subprocess, struct, zlib, collections
from pathlib import Path
R=Path(__file__).resolve().parents[3]
D=Path(__file__).resolve().parent

def read(path):
    dim=subprocess.check_output(['sips','-g','pixelWidth','-g','pixelHeight',str(path)],text=True)
    w=int(dim.split('pixelWidth: ')[1].splitlines()[0]);h=int(dim.split('pixelHeight: ')[1].splitlines()[0])
    b=subprocess.check_output(['ffmpeg','-loglevel','error','-i',str(path),'-f','rawvideo','-pix_fmt','rgba','-'])
    return w,h,bytearray(b)

def save(path,w,h,b):
    def chunk(k,data):return struct.pack('>I',len(data))+k+data+struct.pack('>I',zlib.crc32(k+data)&0xffffffff)
    rows=b''.join(b'\0'+bytes(b[y*w*4:(y+1)*w*4]) for y in range(h))
    path.write_bytes(b'\x89PNG\r\n\x1a\n'+chunk(b'IHDR',struct.pack('>2I5B',w,h,8,6,0,0,0))+chunk(b'IDAT',zlib.compress(rows,9))+chunk(b'IEND',b''))

def put(b,w,x,y,c):b[(y*w+x)*4:(y*w+x)*4+4]=bytes(c)
def art(b,w,x,y,c):
    for yy in (2*y,2*y+1):
      for xx in (2*x,2*x+1):put(b,w,xx,yy,c)
def get(b,w,x,y):return tuple(b[(y*w+x)*4:(y*w+x)*4+4])
def crop(image,box):
    w,h,b=image;x0,y0,x1,y1=box;ow=x1-x0;oh=y1-y0;o=bytearray(ow*oh*4)
    for y in range(oh):o[y*ow*4:(y+1)*ow*4]=b[((y+y0)*w+x0)*4:((y+y0)*w+x1)*4]
    return ow,oh,o

def nearest(im,w2,h2):
    w,h,b=im;o=bytearray(w2*h2*4)
    for y in range(h2):
      sy=min(h-1,y*h//h2)
      for x in range(w2):
        sx=min(w-1,x*w//w2)
        put(o,w2,x,y,get(b,w,sx,sy))
    return w2,h2,o

def bbox(im,threshold=1):
    w,h,b=im;pts=[(i%w,i//w) for i in range(w*h) if b[i*4+3]>=threshold]
    return min(x for x,y in pts),min(y for x,y in pts),max(x for x,y in pts)+1,max(y for x,y in pts)+1

def paste(canvas,im,ox,oy):
    w,h,b=im;cw,ch,cb=canvas
    for y in range(h):
      for x in range(w):
        c=get(b,w,x,y)
        if c[3] and 0<=x+ox<cw and 0<=y+oy<ch:put(cb,cw,x+ox,y+oy,c)

start=read(R/'art/reviews/orochimaru-v6/A_auto_52px.png')
w,h,b=start
# Change only full 2x2 art cells. Face: visible eye, restrained violet shadow, cool skin, fine mouth and pointed chin.
skin=(226,218,207,255);light=(248,239,224,255);shade=(176,158,156,255)
gold=(225,189,52,255);violet=(110,67,121,255);ink=(15,14,19,255)
for x,y,c in [
 (44,17,skin),(45,17,light),(46,17,shade),
 (44,18,violet),(45,18,gold),(46,18,skin),
 (44,19,skin),(45,19,light),(46,19,shade),
 (44,20,skin),(45,20,light),(46,20,ink),
 (44,21,shade),(45,21,skin),
 # Blue-gray directional hair sheen; leave dark framing locks and the right cheek free.
 (41,16,(54,62,84,255)),(42,16,(54,62,84,255)),
 (42,17,(47,55,76,255)),(41,19,(47,55,76,255)),
 ]:art(b,w,x,y,c)
# Fine mouth: keep one dark cell at front, not a horizontal bar.
shift=bytearray(w*h*4)
for yy in range(h-2):
    for xx in range(w-4):
        put(shift,w,xx+4,yy+2,get(b,w,xx,yy))
b=shift
final=(w,h,b)
save(D/'Orochimaru_Idle_0.png',w,h,b)

# One-to-one screen-scale comparison; source A is a matching-height reference thumbnail.
source=read(R/'art/deliveries/orochimaru-base-v6/source/Orochimaru_source_A.png')
source=nearest(crop(source,bbox(source,128)),42,98)
zab=read(R/'ShinobiPrototype/Content/NPCs/Zabuza_Idle_0.png');zab=crop(zab,bbox(zab))
gaara=read(R/'ShinobiPrototype/Content/NPCs/Gaara_Idle_0.png');gaara=nearest(crop(gaara,bbox(gaara)),int((bbox(gaara)[2]-bbox(gaara)[0])*1.5),int((bbox(gaara)[3]-bbox(gaara)[1])*1.5))
start_im=read(R/'art/reviews/orochimaru-v6/A_auto_52px.png')
new=crop(final,bbox(final));old=crop(start_im,bbox(start_im))
items=[new,old,source,zab,gaara]
# Dark backdrop makes transparent sprite silhouettes legible while retaining transparent delivered sprite.
canvas=(1600,740,bytearray((34,41,56,255)*1600*740))
xs=[0,300,600,900,1200]
for i,im in enumerate(items):
    iw,ih,_=im;paste(canvas,im,xs[i]+(300-iw)//2,140-ih)
for i,im in enumerate(items):
    im4=nearest(im,im[0]*4,im[1]*4)
    paste(canvas,im4,xs[i]+(300-im4[0])//2,570-im4[1])
face=crop(final,(84,34,100,50));face=nearest(face,128,128)
paste(canvas,face,1450,590)
save(D/'preview.png',*canvas)
# Summary to stdout for QA.
alpha=collections.Counter(b[3::4]);colors=set(tuple(b[i:i+4]) for i in range(0,len(b),4) if b[i+3])
print('final',w,h,'bbox',bbox(final),'alpha',dict(alpha),'opaque colors',len(colors))
print('2x2 grid',all(get(b,w,x,y)==get(b,w,x+1,y)==get(b,w,x,y+1)==get(b,w,x+1,y+1) for y in range(0,h,2) for x in range(0,w,2)))
