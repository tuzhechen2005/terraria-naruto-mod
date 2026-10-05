"""Draw and review 80 px town NPC samples after the generated source art."""
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

OUT = Path(__file__).parent
O = '#211b29'
SK = '#f5aa75'; LIGHT = '#ffd09a'; SHADE = '#bd714d'; SCAR = '#955b51'
WHITE = '#fff5df'; WARM = '#d9d3cc'; GRAY = '#a6a6b4'
NAVY = '#243657'; NAVY_L = '#36517b'; NAVY_D = '#172541'
GREEN = '#596e45'; GREEN_L = '#829258'; GREEN_D = '#3e4d38'

def canvas():
    im = Image.new('RGBA', (80, 80))
    return im, ImageDraw.Draw(im)

def poly(d, points, color, outline=O):
    d.polygon(points, fill=color)
    if outline:
        d.line(points + [points[0]], fill=outline, width=1)

def rect(d, box, color, outline=None):
    d.rectangle(box, fill=color, outline=outline)

def line(d, points, color, width=1):
    d.line(points, fill=color, width=width)

def ninja_legs(d):
    # Both toes and knees point right; back leg is farther left and darker.
    poly(d, [(34,55),(43,55),(43,64),(40,70),(33,70),(31,64)], NAVY_D)
    poly(d, [(43,55),(51,54),(53,64),(50,70),(43,70),(41,63)], NAVY)
    rect(d, (35,59,39,64), NAVY_L)
    rect(d, (46,59,49,65), NAVY_L)
    poly(d, [(32,68),(40,68),(41,72),(32,72)], WHITE)
    poly(d, [(44,68),(52,68),(52,72),(44,72)], WHITE)
    line(d, [(33,70),(40,70)], GRAY)
    line(d, [(45,70),(51,70)], GRAY)
    poly(d, [(32,72),(41,72),(43,75),(31,76)], NAVY_D)
    poly(d, [(44,72),(54,72),(56,75),(43,76)], NAVY_D)
    rect(d, (35,73,39,74), SK)
    rect(d, (48,73,52,74), SK)

def ninja_vest(d):
    # 25 px shoulder span including the sleeves, with the near side to right.
    poly(d, [(31,39),(48,39),(53,43),(52,57),(40,59),(30,57),(28,45)], GREEN)
    poly(d, [(32,44),(36,43),(38,56),(31,55)], GREEN_D, None)
    poly(d, [(36,43),(48,42),(50,47),(49,55),(36,55)], GREEN_L, None)
    line(d, [(45,43),(45,57)], GREEN_D)
    line(d, [(36,49),(50,49)], GREEN_D)
    rect(d, (36,50,43,54), GREEN, O)
    rect(d, (47,50,50,54), GREEN, O)
    line(d, [(40,51),(40,53)], GREEN_D)
    poly(d, [(31,39),(35,38),(37,44),(33,47),(29,44)], GREEN_D)
    poly(d, [(45,39),(49,39),(52,43),(49,47),(45,45)], GREEN_D)

def tazuna():
    im,d=canvas()
    # Legs: slight right-facing stagger, narrow body.
    poly(d, [(33,55),(42,55),(42,65),(39,71),(32,71),(30,64)], '#414c4a')
    poly(d, [(42,55),(51,54),(53,65),(51,71),(44,71),(40,63)], '#414c4a')
    rect(d, (34,61,38,68), '#59605b')
    rect(d, (45,61,49,68), '#59605b')
    rect(d, (32,69,40,72), WARM)
    rect(d, (44,69,52,72), WARM)
    poly(d, [(31,72),(41,72),(43,75),(30,76)], '#554136')
    poly(d, [(43,72),(53,72),(56,75),(42,76)], '#554136')
    line(d, [(33,73),(40,73)], '#a7784f')
    line(d, [(46,73),(52,73)], '#a7784f')
    # Dark waistcoat and cream rolled sleeves, 25 px shoulders.
    poly(d, [(31,39),(47,39),(53,44),(51,58),(41,60),(30,57),(28,45)], '#ece0c4')
    poly(d, [(31,40),(37,41),(40,57),(31,57),(29,47)], '#302e2f')
    poly(d, [(44,40),(49,41),(52,46),(50,57),(43,57)], '#302e2f')
    rect(d, (34,49,37,55), '#504438')
    rect(d, (46,48,48,55), '#504438')
    poly(d, [(34,39),(37,40),(38,51),(35,55),(33,47)], WHITE)
    poly(d, [(44,40),(47,40),(48,50),(46,54),(44,47)], WHITE)
    line(d, [(36,43),(36,50)], GRAY)
    poly(d, [(29,42),(32,43),(33,51),(29,56),(26,53),(27,46)], '#ece0c4')
    poly(d, [(50,42),(53,44),(54,52),(52,56),(49,52)], '#ece0c4')
    poly(d, [(27,53),(31,53),(32,57),(28,59),(26,56)], SK)
    poly(d, [(52,52),(55,52),(57,56),(54,59),(51,56)], SK)
    # Bottle belongs to the near hand, at right.
    poly(d, [(55,52),(59,52),(61,56),(60,64),(56,64),(54,56)], '#b18651')
    rect(d, (56,49,59,52), '#e4c696')
    rect(d, (57,56,59,60), '#e4c696')
    # Back of hair left; face and projecting nose right. 23 px head height.
    poly(d, [(28,24),(29,20),(34,17),(44,17),(49,19),(53,23),(54,30),(51,37),(43,39),(34,38),(29,34),(27,29)], WARM)
    poly(d, [(38,23),(45,22),(51,25),(52,30),(54,31),(52,35),(47,38),(38,37),(36,32)], SK)
    poly(d, [(40,23),(49,24),(51,28),(42,28)], LIGHT, None)
    rect(d, (43,33,50,36), SHADE)
    poly(d, [(27,25),(29,20),(34,17),(38,18),(43,17),(48,18),(52,20),(54,24),(49,25),(47,22),(41,24),(36,22),(30,27)], WHITE)
    rect(d, (30,20,33,22), WARM)
    rect(d, (45,18,48,19), WARM)
    # Two small round lenses visibly shifted toward right.
    d.ellipse((38,26,43,31), outline=O, width=1)
    d.ellipse((46,26,51,31), outline=O, width=1)
    line(d, [(43,27),(46,27)], O)
    rect(d, (40,27,41,28), WHITE)
    rect(d, (42,28,42,29), O)
    rect(d, (48,27,49,28), WHITE)
    rect(d, (50,28,50,29), O)
    rect(d, (53,30,54,32), SHADE)
    # Narrow moustache, mouth and bright beard separated into clear bands.
    poly(d, [(33,32),(38,33),(43,34),(49,33),(52,32),(53,35),(50,39),(44,40),(36,39),(32,36)], WHITE)
    line(d, [(34,34),(39,35)], WARM)
    line(d, [(47,35),(51,34)], WARM)
    line(d, [(41,36),(47,36)], O)
    rect(d, (39,38,46,39), GRAY)
    return im

def iruka():
    im,d=canvas()
    ninja_legs(d)
    # Sleeve and scroll behind the vest; hand on near (right) side.
    poly(d, [(29,43),(33,42),(35,51),(31,56),(27,55),(26,49)], NAVY)
    poly(d, [(49,42),(53,44),(55,52),(52,56),(49,52)], NAVY)
    ninja_vest(d)
    poly(d, [(27,53),(31,53),(32,57),(28,59),(26,56)], SK)
    poly(d, [(52,53),(55,52),(56,57),(54,59),(51,56)], SK)
    poly(d, [(54,46),(59,44),(61,47),(57,53),(53,53)], WHITE)
    rect(d, (59,44,61,47), SHADE)
    line(d, [(56,50),(60,46)], WARM)
    # Rearward high ponytail, the recognizable flick.
    poly(d, [(34,23),(29,21),(26,19),(28,19),(25,17),(31,18),(28,17),(35,19),(38,23)], O)
    poly(d, [(29,20),(26,18),(34,21),(34,23)], '#343039', None)
    # Side-turned head: visible ear left, both eyes toward right, nose on contour.
    poly(d, [(32,23),(36,19),(44,18),(50,20),(53,24),(54,32),(51,37),(46,40),(36,39),(31,34)], O)
    poly(d, [(39,24),(48,22),(52,26),(52,31),(54,32),(51,36),(47,38),(39,37),(37,32)], SK)
    rect(d, (39,25,48,27), LIGHT)
    poly(d, [(32,23),(35,19),(42,18),(47,19),(51,22),(48,24),(41,22),(35,26)], O)
    rect(d, (33,29,35,31), SHADE)
    line(d, [(35,30),(39,30)], SK, 2)
    # Band and small metal plate, positioned above the eyes.
    poly(d, [(34,22),(39,20),(50,21),(53,24),(51,26),(35,26)], NAVY_D)
    poly(d, [(42,21),(50,22),(50,25),(41,25)], GRAY)
    line(d, [(43,21),(48,21)], WHITE)
    line(d, [(46,23),(48,23)], O)
    # Both eyes sit to the right; near eye leads the gaze.
    line(d, [(40,28),(43,28)], O)
    line(d, [(47,28),(50,28)], O)
    rect(d, (40,29,43,30), WHITE)
    rect(d, (47,29,51,30), WHITE)
    rect(d, (42,29,42,30), O)
    rect(d, (50,29,50,30), O)
    # Precisely seven pixels, wholly within face, just below the eyes.
    line(d, [(42,33),(48,33)], SCAR)
    rect(d, (53,31,54,32), SHADE)
    # Clean peach chin and lifted smile, with no brown lower-face fill.
    line(d, [(44,36),(47,37),(50,35)], O)
    rect(d, (46,36,48,36), WHITE)
    return im

def kakashi():
    im,d=canvas()
    ninja_legs(d)
    poly(d, [(29,43),(33,42),(35,51),(31,56),(27,55),(26,49)], NAVY)
    poly(d, [(49,42),(53,44),(55,52),(52,56),(49,52)], NAVY)
    ninja_vest(d)
    poly(d, [(27,53),(31,53),(32,57),(27,58)], NAVY_D)
    poly(d, [(52,52),(55,52),(56,57),(54,59)], NAVY_D)
    # Near hand reads an orange book.
    poly(d, [(54,46),(59,43),(62,45),(60,54),(56,55)], '#da6b2d')
    line(d, [(56,47),(59,45)], '#ffb165')
    line(d, [(59,46),(58,53)], '#9b4729')
    # Silver hair goes up and back left, rather than rising symmetrically.
    poly(d, [(29,29),(26,25),(30,25),(25,20),(31,22),(28,17),(35,20),(34,17),(40,19),(42,17),(46,20),(50,18),(50,24),(54,24),(52,32),(48,38),(36,39)], WARM)
    poly(d, [(29,23),(31,20),(35,23),(36,18),(40,23),(42,18),(45,24),(49,21),(49,27),(34,29)], WHITE, None)
    line(d, [(31,22),(36,25)], GRAY)
    line(d, [(39,21),(42,25)], GRAY)
    # Ear and skin wedge below hair: right/front eye is exposed.
    poly(d, [(38,26),(45,24),(51,27),(53,32),(51,36),(45,39),(38,37),(36,32)], SK)
    rect(d, (34,29,36,32), SHADE)
    poly(d, [(45,26),(50,27),(51,31),(44,30)], LIGHT, None)
    # Diagonal forehead protector ends before right eye, covering rear left eye.
    poly(d, [(29,26),(36,24),(45,23),(45,27),(41,31),(36,34),(32,32)], NAVY_D)
    poly(d, [(36,26),(43,24),(44,27),(38,30)], GRAY)
    line(d, [(38,26),(42,25)], WHITE)
    line(d, [(40,28),(42,27)], O)
    # Lazy open right eye has upper lid, visible white and low pupil.
    line(d, [(45,29),(51,29)], O)
    rect(d, (46,30,51,31), WHITE)
    rect(d, (48,30,49,31), O)
    rect(d, (53,31,53,32), SHADE)
    # Mask starts at nose bridge: a small raised bump on upper contour.
    poly(d, [(34,33),(43,34),(47,33),(49,32),(52,34),(53,36),(49,40),(40,40),(34,37)], NAVY_D)
    line(d, [(46,34),(49,33),(51,34)], NAVY_L)
    line(d, [(41,38),(49,38)], NAVY)
    return im

names = ['Tazuna', 'Iruka', 'Kakashi']
images = [tazuna(), iruka(), kakashi()]
for name, im in zip(names, images):
    im.save(OUT / f'{name}_Idle.png')

# Match the Guide by visible height, not by its transparent atlas cell.
guide = Image.open(OUT / '../../reference/terraria/NPC_22_x4.png').convert('RGBA')
guide = guide.crop(guide.getbbox())
guide = guide.resize((round(guide.width * 61 / guide.height), 61), Image.Resampling.NEAREST)
bg = (35, 45, 64, 255)
preview = Image.new('RGBA', (1000, 455), bg)
pd = ImageDraw.Draw(preview)
for i, (name, im) in enumerate(zip(names, images)):
    x = 40 + i*245
    pd.text((x, 10), name, fill=WHITE)
    preview.alpha_composite(im, (x+45, 35))
    pd.line((x, 111, x+190, 111), fill=WARM)
    preview.alpha_composite(im.resize((240,240), Image.Resampling.NEAREST), (x, 148))
    pd.line((x, 376, x+240, 376), fill=WARM)
x = 790
pd.text((x, 10), 'Guide', fill=WHITE)
preview.alpha_composite(guide, (x+55, 111-guide.height))
preview.alpha_composite(guide.resize((guide.width*3, guide.height*3), Image.Resampling.NEAREST), (x+5, 376-guide.height*3))
pd.line((x, 111, x+180, 111), fill=WARM)
pd.line((x, 376, x+180, 376), fill=WARM)
preview.convert('RGB').save(OUT/'preview.png')

faces = Image.new('RGBA', (800, 290), bg)
fd = ImageDraw.Draw(faces)
for i, (name, im) in enumerate(zip(names, images)):
    x=12+i*270
    fd.text((x, 7), name, fill=WHITE)
    head=im.crop((23, 10, 56, 42)).resize((264, 256), Image.Resampling.NEAREST)
    faces.alpha_composite(head, (x, 26))
    for gx in range(x, x+265, 8):
        fd.line((gx, 26, gx, 281), fill=(116,125,142,120))
    for gy in range(26, 283, 8):
        fd.line((x, gy, x+263, gy), fill=(116,125,142,120))
faces.convert('RGB').save(OUT/'faces_8x.png')
