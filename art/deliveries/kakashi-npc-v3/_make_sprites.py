from pathlib import Path
import subprocess

ROOT = Path(__file__).parent
SRC = ROOT / 'source'
SRC.mkdir(exist_ok=True)
S = 8
W, H = 28, 25
C = {
    'outline': (19, 24, 43, 255), 'hair_dark': (74, 80, 95, 255),
    'hair_mid': (156, 164, 177, 255), 'hair_light': (225, 229, 232, 255),
    'skin': (219, 162, 121, 255), 'skin_light': (244, 194, 146, 255),
    'navy_dark': (31, 40, 78, 255), 'navy': (55, 65, 120, 255),
    'navy_light': (78, 89, 143, 255), 'mask': (26, 38, 68, 255),
    'green_dark': (53, 75, 55, 255), 'green': (95, 125, 80, 255),
    'green_light': (135, 157, 105, 255), 'metal': (175, 186, 193, 255),
    'metal_light': (225, 229, 227, 255), 'wrap': (233, 231, 222, 255),
    'shoe': (28, 32, 57, 255), 'orange': (225, 99, 29, 255),
    'orange_light': (251, 153, 53, 255), 'orange_dark': (118, 48, 23, 255),
}

def canvas():
    return [[(0,0,0,0) for _ in range(W)] for _ in range(H)]

def dot(a, x, y, color):
    if 0 <= x < W and 0 <= y < H:
        a[y][x] = C[color]

def rect(a, x0, y0, x1, y1, color):
    for y in range(y0, y1 + 1):
        for x in range(x0, x1 + 1):
            dot(a, x, y, color)

def line(a, x0, y0, x1, y1, color, width=1):
    n = max(abs(x1-x0), abs(y1-y0), 1)
    for i in range(n+1):
        x = round(x0 + (x1-x0)*i/n)
        y = round(y0 + (y1-y0)*i/n)
        for oy in range(-(width//2), (width+1)//2):
            for ox in range(-(width//2), (width+1)//2):
                dot(a, x+ox, y+oy, color)

def poly(a, points, color):
    for y in range(H):
        for x in range(W):
            inside = False
            j = len(points)-1
            for i in range(len(points)):
                xi, yi = points[i]; xj, yj = points[j]
                if (yi > y) != (yj > y) and x < (xj-xi)*(y-yi)/(yj-yi) + xi:
                    inside = not inside
                j = i
            if inside: dot(a, x, y, color)

def head(a, dy=0):
    # Large left-swept spikes, dark silhouette, three pale tiers.
    poly(a, [(7,8+dy),(8,5+dy),(6,5+dy),(9,3+dy),(8,2+dy),(11,3+dy),
             (11,1+dy),(13,2+dy),(15,0+dy),(16,2+dy),(18,1+dy),
             (19,3+dy),(21,3+dy),(20,5+dy),(21,7+dy),(19,9+dy),
             (15,10+dy),(10,9+dy)], 'outline')
    poly(a, [(8,7+dy),(10,4+dy),(10,3+dy),(12,4+dy),(12,2+dy),
             (15,3+dy),(16,2+dy),(18,4+dy),(20,4+dy),(19,7+dy),
             (16,8+dy),(10,8+dy)], 'hair_mid')
    dot(a, 15, 0+dy, 'hair_dark')
    rect(a, 9,5+dy,12,6+dy,'hair_light')
    rect(a, 12,3+dy,16,5+dy,'hair_light')
    rect(a, 16,4+dy,18,5+dy,'hair_light')
    rect(a, 8,7+dy,11,7+dy,'hair_dark')
    rect(a, 15,6+dy,19,7+dy,'hair_dark')
    # Face points right. The slanted plate covers the far (left) eye;
    # the one visible eye is a short horizontal slit below the band.
    rect(a, 13,8+dy,20,12+dy,'outline')
    rect(a, 14,8+dy,19,9+dy,'skin')
    rect(a, 16,8+dy,19,8+dy,'skin_light')
    rect(a, 17,9+dy,19,9+dy,'outline')
    rect(a, 12,7+dy,20,8+dy,'navy_dark')
    rect(a, 13,8+dy,17,8+dy,'metal')
    rect(a, 14,7+dy,18,7+dy,'metal_light')
    # Chunky Leaf engraving on the plate.
    dot(a, 15,7+dy,'green_dark')
    dot(a, 16,8+dy,'green_dark')
    rect(a, 14,9+dy,20,11+dy,'mask')
    rect(a, 15,9+dy,17,9+dy,'skin_light')
    rect(a, 17,9+dy,18,9+dy,'outline')
    dot(a, 20,10+dy,'navy_dark')

def torso(a, dy=0):
    rect(a, 10,11+dy,19,19+dy,'outline')
    rect(a, 11,12+dy,18,18+dy,'green_dark')
    rect(a, 12,12+dy,17,17+dy,'green')
    rect(a, 13,12+dy,16,12+dy,'green_light')
    rect(a, 11,14+dy,18,14+dy,'green_light')
    rect(a, 11,17+dy,18,18+dy,'green_dark')
    rect(a, 13,13+dy,14,18+dy,'green_dark')
    # Two broad high-contrast chest scroll pockets.
    rect(a, 11,15+dy,13,17+dy,'outline')
    rect(a, 12,15+dy,13,16+dy,'green_light')
    rect(a, 16,15+dy,18,17+dy,'outline')
    rect(a, 16,15+dy,17,16+dy,'green_light')
    rect(a, 11,12+dy,12,13+dy,'green_light')
    rect(a, 17,12+dy,18,13+dy,'green_light')

def arm(a, side, handx, handy, dy=0):
    sx = 10 if side == 'back' else 19
    sy = 13+dy
    elbowx = round((sx+handx)/2)
    elbowy = round((sy+handy)/2)
    line(a,sx,sy,elbowx,elbowy,'outline',4)
    line(a,sx,sy,elbowx,elbowy,'navy',2)
    line(a,elbowx,elbowy,handx,handy,'outline',4)
    line(a,elbowx,elbowy,handx,handy,'navy_light',2)
    rect(a,handx-1,handy-1,handx+1,handy+1,'outline')
    rect(a,handx,handy-1,handx+1,handy,'shoe')

def leg(a, hipx, knee, foot, dy=0):
    hx,hy=hipx,18+dy; kx,ky=knee; fx,fy=foot
    line(a,hx,hy,kx,ky,'outline',5)
    line(a,hx,hy,kx,ky,'navy',3)
    line(a,kx,ky,fx,fy-1,'outline',5)
    line(a,kx,ky,fx,fy-1,'navy_dark',3)
    line(a,kx,ky+1,round((kx+fx)/2),round((ky+fy)/2),'wrap',2)
    rect(a,fx-1,fy-1,fx+2,fy,'outline')
    rect(a,fx,fy-1,fx+2,fy-1,'shoe')

WALKS = [
    ((12,21),(12,24),(17,21),(17,24),(8,18),(21,18)),
    ((10,21),(8,24),(18,21),(21,24),(7,17),(22,19)),
    ((11,21),(10,24),(18,21),(20,24),(8,19),(21,17)),
    ((13,21),(13,24),(17,21),(17,24),(9,18),(20,18)),
    ((12,21),(10,24),(19,21),(22,24),(8,17),(22,19)),
    ((11,21),(9,24),(18,21),(19,24),(8,19),(21,17)),
    ((12,21),(12,24),(17,21),(17,24),(9,18),(20,18)),
]

def walking(i):
    a=canvas(); k1,f1,k2,f2,h1,h2=WALKS[i]
    leg(a,12,k1,f1); leg(a,17,k2,f2)
    arm(a,'back',*h1); torso(a); arm(a,'front',*h2); head(a)
    a=narrow_body(a)
    pockets(a)
    return a

def pockets(a, dy=0):
    rect(a,11,15+dy,13,17+dy,'green_dark')
    rect(a,11,15+dy,12,16+dy,'green_light')
    rect(a,16,15+dy,18,17+dy,'green_dark')
    rect(a,16,15+dy,17,16+dy,'green_light')

def narrow_body(a):
    # Ibiki's accepted idle measures 11 art pixels across his lower torso.
    # Kakashi's armor and sleeves stay within a comparable 11-12 pixel span.
    b=[row[:] for row in a]
    for y in range(12,H):
        b[y]=[(0,0,0,0) for _ in range(W)]
        for x in range(W):
            if a[y][x][3]:
                nx=round(14+(x-14)*0.68)
                b[y][nx]=a[y][x]
    return b

def action(kind):
    if kind=='idle': return walking(0)
    a=canvas()
    if kind=='jump':
        leg(a,12,(10,18),(8,20),-2)
        leg(a,17,(19,18),(21,20),-2)
        arm(a,'back',8,13,-2); torso(a,-2); arm(a,'front',21,14,-2); head(a,-2)
    elif kind=='sit':
        # Crossed legs and orange book in front of the mask.
        head(a,5); torso(a,5)
        rect(a,8,22,20,24,'outline'); rect(a,9,22,19,23,'navy')
        rect(a,11,23,16,24,'wrap')
        arm(a,'back',12,19,5); arm(a,'front',19,20,5)
        rect(a,18,14,23,20,'orange_dark')
        rect(a,19,15,22,19,'orange')
        rect(a,19,15,21,16,'orange_light')
        dot(a,21,18,'outline')
    else:
        leg(a,12,(10,21),(8,24)); leg(a,17,(20,21),(22,24))
        if kind=='windup':
            arm(a,'back',9,11); torso(a); arm(a,'front',22,17); head(a)
            line(a,8,10,6,9,'outline',2)
        elif kind=='throw':
            arm(a,'back',8,18); torso(a); arm(a,'front',24,13); head(a)
            line(a,25,13,27,13,'metal',1)
        elif kind=='recover':
            arm(a,'back',8,14); torso(a); arm(a,'front',20,18); head(a)
    a=narrow_body(a)
    if kind!='sit': pockets(a, -2 if kind=='jump' else 0)
    return a

def sheet(frames,path):
    w=len(frames)*W*S; h=H*S
    buf=bytearray(w*h*4)
    for fi,frame in enumerate(frames):
        for y in range(H):
            for x in range(W):
                c=frame[y][x]
                for oy in range(S):
                    off=((y*S+oy)*w+(fi*W+x)*S)*4
                    buf[off:off+S*4]=bytes(c)*S
    subprocess.run(['ffmpeg','-loglevel','error','-y','-f','rawvideo','-pix_fmt','rgba',
                    '-s',f'{w}x{h}','-i','-','-frames:v','1',str(path)],input=buf,check=True)

sheet([walking(i) for i in range(7)],SRC/'Kakashi_Idle_Walk.png')
sheet([action(x) for x in ['idle','jump','sit','windup','throw','recover']],
      SRC/'Kakashi_Idle_Jump_Sit_Throw.png')
