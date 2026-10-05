from pathlib import Path
from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parents[1]
INK = (27, 23, 30, 255)
SKIN = (226, 153, 105, 255)
SKIN_L = (248, 183, 126, 255)
SKIN_D = (164, 96, 69, 255)
WHITE = (247, 239, 213, 255)

R = dict(coat=(140, 90, 43, 255), light=(190, 128, 57, 255), dark=(81, 52, 43, 255), pant=(38, 43, 57, 255), pant_light=(71, 76, 91, 255), wrap=(202, 186, 157, 255), hair=(63, 42, 38, 255), hair_light=(113, 74, 52, 255), shoe=(113, 75, 42, 255))
N = dict(coat=(49, 48, 57, 255), light=(91, 86, 94, 255), dark=(32, 33, 43, 255), pant=(34, 37, 49, 255), pant_light=(70, 68, 79, 255), wrap=(86, 79, 78, 255), hair=(36, 34, 43, 255), hair_light=(75, 66, 77, 255), shoe=(59, 48, 49, 255))

def rect(d, box, color): d.rectangle(box, fill=color)
def line(d, pts, color, width=1): d.line(pts, fill=color, width=width, joint='curve')

def limb(d, pts, outer, inner):
    line(d, pts, INK, 4)
    line(d, pts, outer, 2)
    x, y = pts[-1]
    rect(d, (x-2, y, x+2, y), INK)
    rect(d, (x-1, y, x+1, y), inner)

def head(d, kind, dy=0):
    p = R if kind == 'Ronin' else N
    def q(box, c): rect(d, (box[0],box[1]+dy,box[2],box[3]+dy),c)
    # Head is x=23..34, y=19..28: the hair cap and face are composed directly on this grid.
    q((24,19,30,19),INK); q((22,20,32,21),INK)
    q((23,20,29,21),p['hair']); q((24,20,29,20),p['hair_light'])
    q((22,22,32,22),p['hair']); q((23,23,26,26),p['hair'])
    q((25,22,31,27),INK); q((27,23,32,27),SKIN_D)
    q((28,23,32,26),SKIN); q((29,24,32,25),SKIN_L)
    q((29,28,32,28),INK); q((28,27,32,27),SKIN_D)
    # Brow / clear 1-cell gap / two-cell white-and-pupil eye.
    q((30,22,31,22),INK)
    q((30,24,30,25),INK); q((31,24,31,25),WHITE)
    q((33,26,34,26),INK); q((33,26,33,26),SKIN_L)  # one-cell nose bump
    q((32,28,32,28),INK)  # mouth
    if kind == 'Ronin':
        q((23,19,26,20),p['hair']); q((22,19,24,20),INK)
        q((23,19,24,19),p['hair_light'])
        q((22,23,24,24),p['hair_light']); q((25,27,27,28),p['hair'])
    else:
        # Narrow steel plate with single-pixel diagonal scratch; face stays uncovered.
        q((26,21,32,22),INK); q((27,21,31,21),(169,171,172,255))
        q((27,22,31,22),(100,105,112,255))
        q((29,21,29,21),INK); q((30,22,30,22),INK)
        q((23,20,26,21),p['hair_light'])

def sword(d, pose, dy=0):
    if pose == 'Slash_0': a,b=(28,31+dy),(22,12+dy)
    elif pose == 'Slash_1': a,b=(31,32+dy),(49,39+dy)
    elif pose == 'Jump': a,b=(32,31+dy),(49,31+dy)
    else: a,b=(32,34+dy),(49,36+dy)
    line(d,[a,b],INK,4); line(d,[a,b],(213,206,179,255),2)
    line(d,[(a[0]+1,a[1]-1),(b[0],b[1]-1)],WHITE,1)
    rect(d,(a[0]-2,a[1]-1,a[0]+2,a[1]),INK)
    rect(d,(a[0]-1,a[1]-1,a[0]+1,a[1]-1),(160,114,56,255))

def kunai(d, pose, dy=0):
    if pose == 'Throw': a,b=(37,31+dy),(48,30+dy)
    elif pose == 'Slash_0': a,b=(26,28+dy),(22,19+dy)
    elif pose == 'Slash_1': a,b=(38,32+dy),(45,31+dy)
    else: a,b=(35,34+dy),(42,33+dy)
    line(d,[a,b],INK,3); line(d,[a,b],(165,173,183,255),1)
    rect(d,(b[0],b[1],b[0]+1,b[1]),WHITE)

def frame(kind, pose):
    im=Image.new('RGBA',(56,44),(0,0,0,0)); d=ImageDraw.Draw(im)
    p=R if kind=='Ronin' else N
    jump = pose=='Jump'; dy=-4 if jump else 0
    # Legs: alternating endpoints while head/torso remain fixed across walking frames.
    legs={
      'Idle': ((25,41),(31,41)), 'Walk_0': ((22,41),(34,39)),
      'Walk_1': ((24,41),(33,41)), 'Walk_2': ((28,39),(36,41)),
      'Walk_3': ((24,40),(33,41)), 'Slash_0': ((24,41),(34,41)),
      'Slash_1': ((23,41),(34,41)), 'Throw': ((23,41),(35,41)),
      'Jump': ((24,39),(34,39))}
    lf,rt=legs[pose]
    hip=(28,35+dy)
    for end, knee in [(lf,(lf[0]+1,38+dy)),(rt,(rt[0]-1,38+dy))]:
        foot=(end[0],end[1]+dy-1 if not jump else end[1]-3)
        limb(d,[hip,knee,foot],p['pant'],p['shoe'])
        line(d,[knee,foot],p['pant_light'],1)
    # Rear arm and stance shape.
    if pose.startswith('Walk_'):
        phase=int(pose[-1]); hand=(22 if phase in (0,1) else 29,34+dy)
    elif pose=='Throw': hand=(20,35+dy)
    else: hand=(22,35+dy)
    limb(d,[(25,30+dy),hand],p['coat'],SKIN_D)
    rect(d,(24,29+dy,31,35+dy),INK)
    rect(d,(25,29+dy,30,34+dy),p['coat'])
    rect(d,(25,29+dy,27,33+dy),p['light'])
    rect(d,(29,30+dy,30,34+dy),p['dark'])
    rect(d,(24,35+dy,32,36+dy),p['dark'])
    if kind=='Ronin':
        line(d,[(26,30+dy),(30,34+dy)],p['wrap'],2)
        rect(d,(23,36+dy,26,38+dy),p['coat'])
        rect(d,(30,36+dy,33,38+dy),p['coat'])
        rect(d,(26,35+dy,30,35+dy),(123,76,42,255))
    else:
        line(d,[(25,31+dy),(29,33+dy)],p['light'],1)
        rect(d,(25,35+dy,30,35+dy),(103,79,62,255))
        rect(d,(31,36+dy,33,38+dy),p['wrap'])
    # Weapons and front arm follow the generated motion sheets.
    if pose=='Slash_0': target=(27,29+dy)
    elif pose in ('Slash_1','Throw'): target=(37,32+dy)
    else: target=(33,34+dy)
    limb(d,[(30,30+dy),target],p['coat'],SKIN)
    if kind=='Ronin': sword(d,pose,dy)
    else: kunai(d,pose,dy)
    head(d,kind,dy)
    return im.resize((112,88),Image.Resampling.NEAREST)

RONIN=['Idle','Slash_0','Slash_1','Walk_0','Walk_1','Walk_2','Walk_3','Jump']
ROGUE=['Idle','Throw','Slash_0','Slash_1','Walk_0','Walk_1','Walk_2','Walk_3','Jump']
for kind,poses in [('Ronin',RONIN),('RogueGenin',ROGUE)]:
    for pose in poses:
        frame(kind,pose).save(ROOT/f'{kind}_{pose}.png')
