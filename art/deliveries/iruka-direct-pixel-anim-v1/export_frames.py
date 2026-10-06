"""Crop generated pose art, sample at one scale, and anchor Iruka frames."""
from pathlib import Path
from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parent
REF = Image.open(ROOT / "source/approved_idle.png").convert("RGBA")
WALK = Image.open(ROOT / "source/generated_walk_strip.png").convert("RGBA")
ACTION = Image.open(ROOT / "source/generated_action_strip.png").convert("RGBA")
NAMES = ["Iruka_00_Idle"] + [f"Iruka_{i:02d}_Walk" for i in range(1, 7)] + ["Iruka_07_Jump", "Iruka_08_Sit"] + [f"Iruka_{i:02d}_Throw" for i in range(9, 12)]
SCALE = 0.1  # one sample ratio for all generated sources
HEAD_BOX = (17, 14, 46, 40)
HEAD = REF.crop(HEAD_BOX)
HEAD_X = 23  # approved reference shifted +6 to anchor vest at x=40
HEAD_Y = [14, 14, 13, 14, 13, 14, 14, 3, 19, 14, 14, 14]
FOOT_Y = [75] * 7 + [69] + [75] * 4


def split(sheet, count, index):
    left = round(index * sheet.width / count)
    right = round((index + 1) * sheet.width / count)
    crop = sheet.crop((left, 0, right, sheet.height))
    crop.save(ROOT / "source" / f"{NAMES[len(frames)]}_pose.png")
    return crop


def green_center(image):
    pix = image.load()
    xs = []
    for y in range(image.height):
        for x in range(image.width):
            r, g, b, a = pix[x, y]
            if a > 127 and g > r * 1.13 and g > b * 1.12 and g > 65:
                xs.append(x)
    return round(sum(xs) / len(xs))


def harden(image):
    pix = image.load()
    for y in range(image.height):
        for x in range(image.width):
            r, g, b, a = pix[x, y]
            pix[x, y] = (r, g, b, 255) if a >= 128 else (0, 0, 0, 0)
    return image


frames = []
final_head_y = [14]
idle = Image.new("RGBA", (80, 80))
idle.alpha_composite(REF, (6, 0))
frames.append(idle)

for frame_index in range(1, 12):
    if frame_index <= 6:
        source = split(WALK, 6, frame_index - 1)
    else:
        source = split(ACTION, 5, frame_index - 7)
    body_x = green_center(source)
    bbox = source.getchannel("A").getbbox()
    sampled = source.resize((round(source.width * SCALE), round(source.height * SCALE)), Image.Resampling.NEAREST)
    sampled = harden(sampled)
    out = Image.new("RGBA", (80, 80))
    dest_x = 40 - round(body_x * SCALE)
    dest_y = FOOT_Y[frame_index] - round((bbox[3] - 1) * SCALE)
    out.alpha_composite(sampled, (dest_x, dest_y))
    # Restore the approved face, hair and headband as an exact pixel patch.
    # Clear the generated head. Keep the windup fist to the left of x=27.
    pixels = out.load()
    cutoff = HEAD_Y[frame_index] + 26
    for y in range(max(0, cutoff)):
        for x in range(18, 59):
            if frame_index == 9 and x < 28 and y >= 25:
                continue
            pixels[x, y] = (0, 0, 0, 0)
    out.alpha_composite(HEAD, (HEAD_X, HEAD_Y[frame_index]))
    out = harden(out)
    # Align the actual visible foot row; generated strips can have low alpha
    # specks below the feet, so their source bounding boxes are insufficient.
    bottom = out.getchannel("A").getbbox()[3] - 1
    foot_shift = FOOT_Y[frame_index] - bottom
    shifted = Image.new("RGBA", (80, 80))
    shifted.alpha_composite(out, (0, foot_shift))
    out = shifted
    final_head_y.append(HEAD_Y[frame_index] + foot_shift)
    if frame_index == 11:
        for yy in range(80):
            for xx in range(20):
                out.putpixel((xx, yy), (0, 0, 0, 0))
    # Discard detached generation specks without altering the main figure.
    pixels = out.load()
    seen = set()
    for yy in range(80):
        for xx in range(80):
            if pixels[xx, yy][3] == 0 or (xx, yy) in seen:
                continue
            component = []
            stack = [(xx, yy)]
            seen.add((xx, yy))
            while stack:
                cx, cy = stack.pop()
                component.append((cx, cy))
                for nx, ny in ((cx-1,cy),(cx+1,cy),(cx,cy-1),(cx,cy+1)):
                    if 0 <= nx < 80 and 0 <= ny < 80 and pixels[nx,ny][3] and (nx,ny) not in seen:
                        seen.add((nx,ny))
                        stack.append((nx,ny))
            if len(component) < 4:
                for cx, cy in component:
                    pixels[cx,cy] = (0,0,0,0)
    frames.append(out)

frame_dir = ROOT / "frames"
frame_dir.mkdir(exist_ok=True)
for name, frame in zip(NAMES, frames):
    frame.save(frame_dir / f"{name}.png")

# One row of all frames, shown against light and dark backgrounds at 1x and 3x.
preview = Image.new("RGB", (80 * 12 * 3, 80 * (1 + 3) * 2), "#252a32")
draw = ImageDraw.Draw(preview)
for bg_i, bg in enumerate(("#d2d0c8", "#252a32")):
    for scale_i, scale in enumerate((1, 3)):
        y = bg_i * 320 + (0 if scale_i == 0 else 80)
        draw.rectangle((0, y, 80 * 12 * 3 - 1, y + 80 * scale - 1), fill=bg)
        for j, frame in enumerate(frames):
            rgba = frame.resize((80 * scale, 80 * scale), Image.Resampling.NEAREST)
            preview.paste(rgba, (j * 80 * scale, y), rgba)
preview.save(ROOT / "strip_1x_3x.png")

walk = []
for frame in frames[1:7]:
    canvas = Image.new("RGBA", (240, 240), "#252a32")
    canvas.alpha_composite(frame.resize((240, 240), Image.Resampling.NEAREST))
    walk.append(canvas.convert("P", palette=Image.Palette.ADAPTIVE))
walk[0].save(ROOT / "walk.gif", save_all=True, append_images=walk[1:], duration=100, loop=0, disposal=2)

heads = Image.new("RGBA", (12 * 29 * 6, 26 * 6), "#252a32")
for i, frame in enumerate(frames):
    crop = frame.crop((23, final_head_y[i], 52, final_head_y[i] + 26))
    heads.alpha_composite(crop.resize((29 * 6, 26 * 6), Image.Resampling.NEAREST), (i * 29 * 6, 0))
heads.save(ROOT / "heads_6x.png")
