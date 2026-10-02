"""Rebuild the delivered 2x pixel frames from the approved in-game pixels."""

from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parents[4]
NPC = ROOT / "ShinobiPrototype/Content/NPCs"
OUT = Path(__file__).resolve().parent.parent
SCALE = 2
SIZE = (56, 44)


def read(name):
    im = Image.open(NPC / f"Orochimaru_{name}.png").convert("RGBA")
    return im.resize(SIZE, Image.Resampling.NEAREST)


def save(name, im):
    im.resize((112, 88), Image.Resampling.NEAREST).save(OUT / f"Orochimaru_{name}.png")


def region_shift(im, box, dx, dy, predicate=None):
    src = im.copy()
    x0, y0, x1, y1 = box
    for y in range(y0, y1):
        for x in range(x0, x1):
            px = src.getpixel((x, y))
            if px[3] and (predicate is None or predicate(x, y, px)):
                im.putpixel((x, y), (0, 0, 0, 0))
    for y in range(y0, y1):
        for x in range(x0, x1):
            px = src.getpixel((x, y))
            if px[3] and (predicate is None or predicate(x, y, px)):
                xx, yy = x + dx, y + dy
                if 0 <= xx < 56 and 0 <= yy < 44:
                    im.putpixel((xx, yy), px)
    return im


base = read("Idle_0")

# Walk cycle: move the legs in opposition, then settle the upper body onto
# them. Each change is one native art pixel, which is a 2x2 screen pixel block.
steps = [
    (0, 0, 0, 0), (1, -1, 0, -1), (2, -2, -1, -1), (1, -1, 0, 0),
    (0, 0, 0, 0), (-1, 1, 0, -1), (-2, 2, -1, -1), (-1, 1, 0, 0),
]
walk = []
for i, (front, back, sway, bob) in enumerate(steps):
    im = Image.new("RGBA", SIZE)
    # The trouser legs and sandals remain on the common baseline.
    for y in range(31, 42):
        for x in range(16, 40):
            px = base.getpixel((x, y))
            if not px[3]:
                continue
            dx = front if x < 28 else back
            # Join the trouser legs to the waist before the stride widens.
            if y < 34:
                dx = round(dx * (y - 30) / 4)
            xx = x + dx
            if 0 <= xx < 56:
                im.putpixel((xx, y), px)
    for y in range(10, 33):
        for x in range(16, 41):
            px = base.getpixel((x, y))
            if not px[3]:
                continue
            dy = bob if y < 29 else round(bob * (33 - y) / 4)
            dx = sway if y < 28 else 0
            im.putpixel((x + dx, y + dy), px)
    # The forward sleeve and trailing hair follow the gait by one pixel.
    if i in (1, 2, 5, 6):
        im = region_shift(im, (33, 23, 40, 31), 1 if i < 4 else -1, 0)
        im = region_shift(im, (18, 15, 25, 24), -1 if i < 4 else 1, 0,
                          lambda x, y, p: p[0] < 120 and p[2] > p[0])
    walk.append(im)
    save(f"Walk_{i}", im)

# The original breathing pixels form a quiet loop. Two added frames delay
# shoulder and hair motion before it settles back to the approved idle pose.
idle = [base.copy(), read("Idle_1"), read("Idle_2"), read("Idle_3")]
idle4 = base.copy()
idle4 = region_shift(idle4, (18, 17, 23, 23), -1, 0,
                     lambda x, y, p: p[0] < 115)
idle5 = base.copy()
idle5 = region_shift(idle5, (19, 19, 24, 25), 1, 0,
                     lambda x, y, p: p[0] < 105)
idle.extend([idle4, idle5])
for i, im in enumerate(idle):
    save(f"Idle_{i}", im)


def transition(kind):
    target = read(f"{kind}_0")
    im = base.copy()
    # Preserve the approved face and scalp. Change only a small set of limb,
    # shoulder, and robe pixels toward the first attack pose.
    boxes = {
        "Hands": [(32, 19, 42, 29, -2, 1), (19, 24, 25, 31, 0, 0)],
        "Wind": [(33, 21, 42, 30, -1, 0), (16, 22, 25, 30, 1, 0)],
        "Seal": [(19, 20, 27, 30, 1, 0), (32, 19, 39, 29, -1, 1)],
        "Summon": [(18, 22, 27, 34, 1, 0), (33, 21, 43, 32, -1, 0)],
        "Neck": [(17, 22, 27, 32, 1, 0), (34, 20, 41, 31, -1, 0)],
    }[kind]
    for x0, y0, x1, y1, dx, dy in boxes:
        # Remove the idle limb in the working zone, leaving the core torso.
        for y in range(y0, min(y1, 32)):
            for x in range(x0, x1):
                if x <= 23 or x >= 34:
                    im.putpixel((x, y), (0, 0, 0, 0))
        for y in range(y0, y1):
            for x in range(x0, x1):
                px = target.getpixel((x, y))
                xx, yy = x + dx, y + dy
                if px[3] and 0 <= xx < 56 and 0 <= yy < 44:
                    im.putpixel((xx, yy), px)
    # Put the exact idle facial pixels back after arm edits.
    for y in range(11, 22):
        for x in range(25, 38):
            px = base.getpixel((x, y))
            if px[3]:
                im.putpixel((x, y), px)
    return im


transitions = {}
for action in ("Hands", "Wind", "Seal", "Summon", "Neck"):
    im = transition(action)
    transitions[action] = im
    save(f"{action}In_0", im)

# A launch into the horizontal dash is a distinct leaning pose. Warp the
# approved standing pixels around the planted feet, keeping all source colors.
dash = Image.new("RGBA", SIZE)
for y in range(11, 42):
    for x in range(16, 40):
        px = base.getpixel((x, y))
        if not px[3]:
            continue
        if y < 24:
            dx, dy = 4, 2
        elif y < 32:
            dx, dy = 2, 1
        elif y < 37:
            dx, dy = 1, 0
        else:
            dx, dy = 0, 0
        dash.putpixel((x + dx, y + dy), px)
transitions["Dash"] = dash
save("DashIn_0", dash)


def checker(im, bg, cell=8):
    out = Image.new("RGBA", im.size, bg[0])
    draw = ImageDraw.Draw(out)
    for y in range(0, im.height, cell):
        for x in range(0, im.width, cell):
            if (x // cell + y // cell) % 2:
                draw.rectangle((x, y, x + cell - 1, y + cell - 1), fill=bg[1])
    out.alpha_composite(im)
    return out


def preview_row(label, frames, scale=3):
    w, h = 112 * scale, 88 * scale
    row = Image.new("RGBA", (w * len(frames), h + 28), (28, 32, 44, 255))
    draw = ImageDraw.Draw(row)
    for i, frame in enumerate(frames):
        box = checker(frame.resize((w, h), Image.Resampling.NEAREST),
                      ((35, 40, 54, 255), (45, 50, 66, 255)), 12)
        row.alpha_composite(box, (i * w, 28))
        draw.text((i * w + 6, 7), label if i == 0 else str(i - 1), fill="white")
    return row


rows = [preview_row("REF", [base, *walk]), preview_row("REF", [base, *idle])]
for action in ("Hands", "Dash", "Wind", "Seal", "Summon", "Neck"):
    rows.append(preview_row(action, [base, transitions[action], read(f"{action}_0")]))
sheet = Image.new("RGBA", (max(r.width for r in rows), sum(r.height for r in rows)),
                  (28, 32, 44, 255))
y = 0
for row in rows:
    sheet.alpha_composite(row, (0, y))
    y += row.height
sheet.convert("RGB").save(OUT / "preview.png")

# Frame strips are deliberately sequenced for quick playback review.
strips = {
    "idle_walk_idle": [base, *walk, base],
    **{f"idle_{a.lower()}_idle": [base, transitions[a], read(f"{a}_0"), base]
       for a in ("Hands", "Dash", "Wind", "Seal", "Summon", "Neck")},
}
for name, frames in strips.items():
    strip = Image.new("RGBA", (112 * len(frames), 88))
    for i, frame in enumerate(frames):
        strip.alpha_composite(frame.resize((112, 88), Image.Resampling.NEAREST), (112 * i, 0))
    strip.save(OUT / f"strip_{name}.png")
