"""Claude 2026-10-05: reattach Iruka's head to the body (user: some frames' heads looked detached).

export_frames.py pastes the approved head rows y=14..39 only, so the two rows under the chin (the jaw underside and
neck, y=40..41 in the idle frame) came from each generated pose: walk frames lost the neck and jump/throw frames kept
a second generated chin poking out under the face. This copies the idle frame's neck rows under the pasted head and
clears generated skin left in the rows below it, between the arms.
Reads frames_before_neck_fix/, writes frames/.
"""
from pathlib import Path
from PIL import Image

ROOT = Path(__file__).resolve().parent
SRC, OUT = ROOT / "frames_before_neck_fix", ROOT / "frames"
idle = Image.open(SRC / "Iruka_00_Idle.png").convert("RGBA")
HEAD_X, HEAD_W, IDLE_TOP = 23, 29, 14


def skin(p):
    r, g, b, a = p
    return a and r > 120 and r > g * 1.15 and r > b * 1.4


def reddish(p):
    r, g, b, a = p
    return a and r > g * 1.25 and r > b * 1.25


def green(p):
    r, g, b, a = p
    return g > r * 1.1 and g > b * 1.1


# Idle neck: rows 40-41 under the chin, everything that is not vest.
neck = [(x, dy, idle.getpixel((x, IDLE_TOP + 26 + dy))) for dy in (0, 1) for x in range(HEAD_X, HEAD_X + HEAD_W)
        if idle.getpixel((x, IDLE_TOP + 26 + dy))[3] and not green(idle.getpixel((x, IDLE_TOP + 26 + dy)))]
patch = idle.crop((HEAD_X, IDLE_TOP, HEAD_X + HEAD_W, IDLE_TOP + 26))


def head_top(im):
    for y in range(60):
        if all(im.getpixel((HEAD_X + x, y + yy)) == patch.getpixel((x, yy))
               for x in range(HEAD_W) for yy in range(26) if patch.getpixel((x, yy))[3]):
            return y
    raise SystemExit("approved head not found")


for f in sorted(SRC.glob("Iruka_*.png")):
    im = Image.open(f).convert("RGBA")
    if f.name != "Iruka_00_Idle.png":
        top = head_top(im)
        # Generated chin remnants: skin and its reddish outline under the head, between the arm on the left and an
        # outstretched hand on the right (x 36..55); then dark specks left hanging.
        zone = [(x, y) for y in range(top + 26, top + 32) for x in range(36, 56)]
        for x, y in zone:
            if skin(im.getpixel((x, y))) or reddish(im.getpixel((x, y))):
                im.putpixel((x, y), (0, 0, 0, 0))
        for x, y in zone:
            if True:
                p = im.getpixel((x, y))
                if p[3] and max(p[:3]) < 70:
                    around = [im.getpixel((x + dx, y + dy))[3] for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1))
                              if 0 <= x + dx < 80 and 0 <= y + dy < 80]
                    if sum(1 for a in around if a) <= 1:
                        im.putpixel((x, y), (0, 0, 0, 0))
        for x, dy, p in neck:
            im.putpixel((x, top + 26 + dy), p)
    im.save(OUT / f.name)
    print(f.name)
