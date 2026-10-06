"""Export generated Kakashi pose strips to 80x80 game frames.

The generated sheets have different native magnifications. Each sheet is
normalized to the approved source's magnification, then sampled at the
approved 0.0607247796 ratio and (0.8, 0.5) phase.
"""
from pathlib import Path
import numpy as np
from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parent
BASE = Image.open(ROOT.parent / "kakashi-direct-pixel-v1" / "Kakashi_Idle_h62_Right.png").convert("RGBA")
SCALE = 0.06072477962781587
EFFECTIVE = {"walk": 0.115, "action": 0.115, "trapped": 0.080}
NAMES = (["Kakashi_00_Idle"] + [f"Kakashi_{i:02d}_Walk" for i in range(1, 7)]
         + ["Kakashi_07_Jump", "Kakashi_08_Sit"]
         + [f"Kakashi_{i:02d}_Throw" for i in range(9, 12)]
         + ["Kakashi_Trapped_0", "Kakashi_Trapped_1"])
HEAD_BOX = (27, 14, 53, 43)  # entire approved mask/neck through collar
HEAD_X = 24  # shift reference -3 so the vest seam lies at x=40
BASE_SHIFT = -3


def harden(im):
    a = np.asarray(im.convert("RGBA")).copy()
    a[a[:, :, 3] < 128] = (0, 0, 0, 0)
    a[a[:, :, 3] >= 128, 3] = 255
    return Image.fromarray(a, "RGBA")


def split(sheet, count, index):
    if count == 5:
        # The action generator did not honor equal cells: the windup fist
        # crosses its nominal boundary. These cuts follow the empty gutters.
        cuts = (0, 420, 735, 1195, 1685, sheet.width)
        return sheet.crop((cuts[index], 0, cuts[index + 1], sheet.height))
    left = round(index * sheet.width / count)
    right = round((index + 1) * sheet.width / count)
    return sheet.crop((left, 0, right, sheet.height))


def sample(im, kind):
    # Conceptually normalize source by EFFECTIVE/SCALE, then use one approved
    # sample ratio and phase. Direct indexing avoids an intermediate huge file.
    eff = EFFECTIVE[kind]
    a = np.asarray(im.convert("RGBA"))
    w, h = round(im.width * eff), round(im.height * eff)
    xx = np.minimum(im.width - 1, np.floor((np.arange(w) + .8) / eff).astype(int))
    yy = np.minimum(im.height - 1, np.floor((np.arange(h) + .5) / eff).astype(int))
    return harden(Image.fromarray(a[yy[:, None], xx[None, :]], "RGBA"))


def green_center(im):
    a = np.asarray(im.convert("RGBA"))
    r, g, b, alpha = [a[:, :, i] for i in range(4)]
    mask = ((alpha >= 128) & (g > r * 1.13) & (g > b * 1.10) & (g > 65))
    ys, xs = np.where(mask)
    if not len(xs):
        return im.width // 2
    # Restrict to the upper torso, where the vest seam controls horizontal jitter.
    lo, hi = np.quantile(ys, [.2, .8])
    xs = xs[(ys >= lo) & (ys <= hi)]
    return float(np.median(xs))


def place(cell, kind, foot=None):
    bb = cell.getchannel("A").point(lambda a: 255 if a >= 128 else 0).getbbox()
    src = sample(cell, kind)
    eff = EFFECTIVE[kind]
    if foot is None:
        dx = 40 - round((bb[0] + bb[2] - 1) * eff / 2)
        dy = 40 - round((bb[1] + bb[3] - 1) * eff / 2)
    else:
        dx = 40 - round(green_center(cell) * eff)
        dy = foot - round((bb[3] - 1) * eff)
    out = Image.new("RGBA", (80, 80))
    out.paste(src, (dx, dy), src)
    return keep_largest(harden(out)), (dx, dy), bb


def keep_largest(im):
    """Discard neighboring-cell debris; retain the single character."""
    a = np.asarray(im).copy()
    mask = a[:, :, 3] > 0
    seen = np.zeros(mask.shape, bool)
    groups = []
    for y in range(80):
        for x in range(80):
            if not mask[y, x] or seen[y, x]:
                continue
            q, group = [(x, y)], []
            seen[y, x] = True
            while q:
                xx, yy = q.pop()
                group.append((xx, yy))
                for nx, ny in ((xx-1,yy),(xx+1,yy),(xx,yy-1),(xx,yy+1),
                               (xx-1,yy-1),(xx+1,yy-1),(xx-1,yy+1),(xx+1,yy+1)):
                    if 0 <= nx < 80 and 0 <= ny < 80 and mask[ny,nx] and not seen[ny,nx]:
                        seen[ny,nx] = True
                        q.append((nx,ny))
            groups.append(group)
    if groups:
        largest = max(groups, key=len)
        retained = np.zeros_like(mask)
        for x,y in largest:
            retained[y,x] = True
        a[~retained] = 0
    return Image.fromarray(a, "RGBA")


def restore_head(im, top, index):
    a = np.asarray(im).copy()
    source_pixels = a.copy()
    book = None
    if index == 8:
        # The orange book sits in front of the mask and vest; retain it after
        # replacing the head, without carrying across the generated face.
        orange = (a[:,:,0] > 130) & (a[:,:,0] > a[:,:,1] * 1.25) & (a[:,:,1] > 45)
        yy, xx = np.indices(orange.shape)
        orange &= (yy >= top + 17) & (yy <= top + 36) & (xx >= 39) & (xx <= 57)
        book = np.zeros_like(a)
        book[orange] = a[orange]
    # Remove the source head, including its old mask and neck. The release arm
    # and the windup fist live outside this narrow head area and are retained.
    y0, y1 = max(0, top - 2), min(80, top + 29)
    a[y0:y1, 24:54] = 0
    im = Image.fromarray(a, "RGBA")
    patch = BASE.crop(HEAD_BOX)
    im.alpha_composite(patch, (HEAD_X, top))
    approved = np.zeros((80, 80), bool)
    approved[top:top + patch.height, HEAD_X:HEAD_X + patch.width] = np.asarray(patch)[:, :, 3] > 0
    if index in (7, 9):
        # Keep the left raised arm where head clearance intersects its sleeve.
        arm = np.zeros_like(a)
        arm[max(0,top+16):min(80,top+31), 24:34] = source_pixels[max(0,top+16):min(80,top+31), 24:34]
        arm[approved] = 0
        im.alpha_composite(Image.fromarray(arm, "RGBA"))
    if index == 10:
        # Release arm runs behind the mask into the forward hand.
        arm = np.zeros_like(a)
        arm[max(0,top+17):min(80,top+30), 43:66] = source_pixels[max(0,top+17):min(80,top+30), 43:66]
        arm[approved] = 0
        im.alpha_composite(Image.fromarray(arm, "RGBA"))
    if book is not None:
        # Keep every approved head/collar pixel exact; book lies in front of
        # the generated lower vest and knee outside that approved rectangle.
        book[approved] = 0
        im.alpha_composite(Image.fromarray(book, "RGBA"))
    return harden(im)


def align_foot(im, target):
    bb = im.getchannel("A").getbbox()
    shift = target - (bb[3] - 1)
    result = Image.new("RGBA", (80, 80))
    result.paste(im, (0, shift), im)
    return harden(result), shift


def make():
    sheets = {k: Image.open(ROOT / "source" / f"generated_{k}_strip.png").convert("RGBA")
              for k in EFFECTIVE}
    idle = Image.new("RGBA", (80, 80))
    idle.paste(BASE, (BASE_SHIFT, 0), BASE)
    frames = [harden(idle)]
    head_tops = [14]
    provenance = []
    for i in range(1, 14):
        if i <= 6:
            kind, cell_no, count = "walk", i - 1, 6
        elif i <= 11:
            kind, cell_no, count = "action", i - 7, 5
        else:
            kind, cell_no, count = "trapped", i - 12, 2
        cell = split(sheets[kind], count, cell_no)
        foot = None if kind == "trapped" else (69 if i == 7 else 75)
        frame, origin, bbox = place(cell, kind, foot)
        if kind != "trapped":
            top = origin[1] + round(bbox[1] * EFFECTIVE[kind])
            # Ground heads and hair keep the approved pixels. Use the source
            # body to choose height, then restore the complete approved mask.
            frame = restore_head(frame, top, i)
            if i <= 6:
                frame = keep_largest(frame)
            frame, shift = align_foot(frame, foot)
            head_tops.append(top + shift)
        else:
            head_tops.append(None)
        frames.append(frame)
        provenance.append((NAMES[i], kind, cell_no, origin, bbox, head_tops[-1]))
    # The two trapped heads must agree pixel for pixel. Copy the first trapped
    # head/cloth region, keeping each pose's distinct pushing hand and legs.
    first = np.asarray(frames[12]).copy()
    second = np.asarray(frames[13]).copy()
    second[8:38, 25:53] = first[8:38, 25:53]
    frames[13] = harden(Image.fromarray(second, "RGBA"))

    outdir = ROOT / "frames"
    outdir.mkdir(exist_ok=True)
    for name, frame in zip(NAMES, frames):
        frame.save(outdir / f"{name}.png")

    # Light/dark rows, each with 1x and 3x panels.
    sheet = Image.new("RGB", (14 * 80 * 3, 640), "#252a32")
    draw = ImageDraw.Draw(sheet)
    for row, color in enumerate(("#d2d0c8", "#252a32")):
        for mag, yoff in ((1, row * 320), (3, row * 320 + 80)):
            draw.rectangle((0, yoff, sheet.width - 1, yoff + 80 * mag - 1), fill=color)
            for j, frame in enumerate(frames):
                large = frame.resize((80 * mag, 80 * mag), Image.Resampling.NEAREST)
                sheet.paste(large, (j * 80 * mag, yoff), large)
    sheet.save(ROOT / "strip_1x_3x.png")

    walk = []
    for frame in frames[1:7]:
        canvas = Image.new("RGBA", (240, 240), "#252a32")
        canvas.alpha_composite(frame.resize((240, 240), Image.Resampling.NEAREST))
        walk.append(canvas.convert("P", palette=Image.Palette.ADAPTIVE))
    walk[0].save(ROOT / "walk.gif", save_all=True, append_images=walk[1:],
                 duration=120, loop=0, disposal=2)

    heads = Image.new("RGB", (14 * 34 * 6, 36 * 6), "#252a32")
    necks = Image.new("RGB", (14 * 36 * 8, 38 * 8), "#252a32")
    for j, frame in enumerate(frames):
        top = head_tops[j] if j < 12 else 9
        h = frame.crop((22, max(0, top - 2), 56, max(0, top - 2) + 36))
        heads.paste(h.resize((34 * 6, 36 * 6), Image.Resampling.NEAREST), (j * 34 * 6, 0),
                    h.resize((34 * 6, 36 * 6), Image.Resampling.NEAREST))
        n = frame.crop((22, max(0, top + 14), 58, max(0, top + 14) + 38))
        big = n.resize((36 * 8, 38 * 8), Image.Resampling.NEAREST)
        necks.paste(big, (j * 36 * 8, 0), big)
    heads.save(ROOT / "heads_6x.png")
    necks.save(ROOT / "necks_8x.png")
    for line in provenance:
        print(line)


if __name__ == "__main__":
    make()
