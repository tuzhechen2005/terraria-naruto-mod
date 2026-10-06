from pathlib import Path
from PIL import Image, ImageDraw
import shutil

ROOT = Path(__file__).resolve().parent
SRC = ROOT.parent / 'kakashi-direct-pixel-anim-v1' / 'frames'
DST = ROOT / 'frames'
DST.mkdir(exist_ok=True)
for path in SRC.glob('*.png'):
    shutil.copy2(path, DST / path.name)

fixes = ['Kakashi_03_Walk', 'Kakashi_04_Walk', 'Kakashi_05_Walk', 'Kakashi_11_Throw']
for name in fixes:
    im = Image.open(SRC / f'{name}.png').convert('RGBA')
    px = im.load()
    if name in ('Kakashi_03_Walk', 'Kakashi_04_Walk'):
        # Adjacent approved walk frames supply a waist that stays within the
        # standing silhouette. Keep each target's original legs and feet.
        donor_name = 'Kakashi_02_Walk' if name == 'Kakashi_03_Walk' else 'Kakashi_06_Walk'
        donor = Image.open(SRC / f'{donor_name}.png').convert('RGBA')
        for y in range(38, 54):
            for x in range(20, 60):
                px[x, y] = donor.getpixel((x, y))
    else:
        # A connected rear sleeve and peach hand from the neighboring walk
        # pose replace the detached blue hook. Throw sits two pixels higher.
        donor = Image.open(SRC / 'Kakashi_06_Walk.png').convert('RGBA')
        shift = 0 if name == 'Kakashi_05_Walk' else 2
        for y in range(32 if shift else 34, 54 if shift else 56):
            for x in range(16, 36):
                px[x, y] = donor.getpixel((x, y + shift))
    im.save(DST / f'{name}.png')

names = (["Kakashi_00_Idle"] + [f"Kakashi_{i:02d}_Walk" for i in range(1, 7)]
         + ["Kakashi_07_Jump", "Kakashi_08_Sit"]
         + [f"Kakashi_{i:02d}_Throw" for i in range(9, 12)]
         + ["Kakashi_Trapped_0", "Kakashi_Trapped_1"])
frames = [Image.open(DST / f'{n}.png').convert('RGBA') for n in names]

sheet = Image.new('RGB', (14 * 80 * 3, 640), '#252a32')
draw = ImageDraw.Draw(sheet)
for row, color in enumerate(('#d2d0c8', '#252a32')):
    for mag, yoff in ((1, row * 320), (3, row * 320 + 80)):
        draw.rectangle((0, yoff, sheet.width - 1, yoff + 80 * mag - 1), fill=color)
        for j, frame in enumerate(frames):
            large = frame.resize((80 * mag, 80 * mag), Image.Resampling.NEAREST)
            sheet.paste(large, (j * 80 * mag, yoff), large)
sheet.save(ROOT / 'strip_1x_3x.png')

walk = []
for frame in frames[1:7]:
    canvas = Image.new('RGBA', (240, 240), '#252a32')
    canvas.alpha_composite(frame.resize((240, 240), Image.Resampling.NEAREST))
    walk.append(canvas.convert('P', palette=Image.Palette.ADAPTIVE))
walk[0].save(ROOT / 'walk.gif', save_all=True, append_images=walk[1:], duration=120, loop=0, disposal=2)

pair = Image.new('RGB', (4 * 2 * 80 * 8, 80 * 8), '#80bf80')
for j, name in enumerate(fixes):
    old = Image.open(SRC / f'{name}.png').convert('RGBA').resize((640, 640), Image.Resampling.NEAREST)
    new = Image.open(DST / f'{name}.png').convert('RGBA').resize((640, 640), Image.Resampling.NEAREST)
    pair.paste(old, (j * 1280, 0), old)
    pair.paste(new, (j * 1280 + 640, 0), new)
pair.save(ROOT / 'fix_8x.png')
