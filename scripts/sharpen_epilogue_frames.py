#!/usr/bin/env python3
"""Repaint epilogue frames (wave-epilogue-walk-v2) in the battle frames' palette, keeping every pixel's position.

User, 2026-10-01: Zabuza's lying frames looked soft beside his battle sprite: the clothes were one near-black mass, the
skin a five-step orange gradient over a fifth of the body, and the grey wraps and white mask were missing. The
silhouette and pose stay exactly as delivered (so the rise and collapse still join the walk); only colours change, and
every Rise / Stagger / Collapse frame goes through the same rules so the scene does not change style mid-way:

- clothes take the battle ramp (outline, deep, dark, mid, light blue-grey), lit from the top-left: the first body
  pixel under the top edge is one step lighter;
- the silhouette edge and every cloth pixel touching skin become outline, which separates the limbs;
- skin is pressed to the battle frames' two tones, outlined like the rest;
- greys become the wrap greys; at the head the bandage white.

Works on the 2x2 art-pixel grid. Usage: sharpen_epilogue_frames.py SRC DST [SRC DST ...]
Inputs are the frames as first put in the game (commit 6d87a4a: the Stagger frames there are aligned by the torso,
unlike the delivery), e.g. in bash:
  for a in Rise_0 Rise_1 Rise_2 Stagger_0 Stagger_1 Stagger_2 Stagger_3 Collapse_0 Collapse_1 Collapse_2; do
    f=ShinobiPrototype/Content/NPCs/Zabuza_$a.png; git show 6d87a4a:$f > /tmp/$a.png
    python3 scripts/sharpen_epilogue_frames.py /tmp/$a.png $f; done
"""
import sys

from PIL import Image

OUTLINE = (2, 1, 8)
CLOTH = [(5, 5, 16), (17, 17, 29), (33, 32, 43), (47, 49, 64), (64, 64, 81)]
SKIN_LIGHT = (229, 180, 146)
SKIN_SHADE = (155, 109, 93)
WRAP_DARK = (106, 105, 117)
WRAP_LIGHT = (174, 173, 187)
BANDAGE = (235, 235, 238)


def classify(c):
    r, g, b, a = c
    if a == 0:
        return None
    if r - b > 15 and r > 60:
        return "skin"
    if (r + g + b) / 3 > 60:
        return "grey"
    return "cloth"


def lum(c):
    return (c[0] + c[1] + c[2]) / 3


def repaint(src, dst):
    im = Image.open(src).convert("RGBA")
    w, h = im.size
    gw, gh = w // 2, h // 2
    px = im.load()
    cells = [[px[x * 2, y * 2] for x in range(gw)] for y in range(gh)]
    kind = [[classify(c) for c in row] for row in cells]

    def at(x, y):
        return kind[y][x] if 0 <= x < gw and 0 <= y < gh else None

    def edge(x, y):
        return any(at(x + dx, y + dy) is None for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)))

    xs = [x for y in range(gh) for x in range(gw) if kind[y][x] is not None]
    ys = [y for y in range(gh) for x in range(gw) if kind[y][x] is not None]
    lying = max(xs) - min(xs) > (max(ys) - min(ys)) * 2
    head_from = min(xs) + (max(xs) - min(xs)) * 7 // 10
    head_to = min(ys) + (max(ys) - min(ys)) * 3 // 10

    def in_head(x, y):
        # Lying, the head is at the right end (he faces right); on his feet or knees, the top of the figure.
        return x >= head_from if lying else y <= head_to

    out = [[None] * gw for _ in range(gh)]
    for y in range(gh):
        for x in range(gw):
            k = kind[y][x]
            if k is None:
                continue
            c = cells[y][x]
            if k == "cloth":
                if edge(x, y) or any(at(x + dx, y + dy) == "skin" for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1))):
                    out[y][x] = OUTLINE
                    continue
                L = lum(c)
                step = 0 if L <= 8 else 1 if L <= 16 else 2 if L <= 24 else 3
                # Lit from the top-left: the first pixel inside the top edge catches the light.
                if at(x, y - 1) is None or at(x, y - 2) is None or at(x - 1, y - 1) is None:
                    step = min(step + 2, len(CLOTH) - 1)
                out[y][x] = CLOTH[step]
            elif k == "skin":
                skin_neighbours = sum(at(x + dx, y + dy) == "skin" for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)))
                if edge(x, y):
                    # Outlined like the battle frames, except a thin tip (toes, fingers) that would vanish.
                    out[y][x] = OUTLINE if skin_neighbours >= 2 else SKIN_SHADE
                    continue
                # Two flat tones, not the delivered five-step gradient: shade only where it was really dark.
                out[y][x] = SKIN_LIGHT if lum(c) > 105 else SKIN_SHADE
            else:
                L = lum(c)
                # Light greys at the head are the mask bandage.
                out[y][x] = BANDAGE if L > 150 or (in_head(x, y) and L > 75) else WRAP_LIGHT if L > 95 else WRAP_DARK
    res = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    rp = res.load()
    for y in range(gh):
        for x in range(gw):
            if out[y][x] is None:
                continue
            for dy in (0, 1):
                for dx in (0, 1):
                    rp[x * 2 + dx, y * 2 + dy] = out[y][x] + (255,)
    res.save(dst)


if __name__ == "__main__":
    args = sys.argv[1:]
    if not args or len(args) % 2:
        sys.exit(__doc__)
    for i in range(0, len(args), 2):
        repaint(args[i], args[i + 1])
