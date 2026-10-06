from pathlib import Path
from PIL import Image, ImageDraw
import json

ROOT = Path(__file__).resolve().parent
SOURCE = ROOT / "source" / "generated_opposite_sheet.png"
V3 = ROOT.parent / "kakashi-walk-repair-v3" / "frames"
NAMES = ["OppositeContact", "OppositePassing", "ReturnLift"]
src = Image.open(SOURCE).convert("RGBA")
assert src.size == (2172, 724)
panels = []
for i in range(3):
    panel = src.crop((i * 724, 0, (i + 1) * 724, 724))
    alpha = panel.getchannel("A").point(lambda a: 255 if a >= 128 else 0)
    box = alpha.getbbox()
    assert box, NAMES[i]
    panels.append((panel, box))

# One scale for all three poses. The tallest generated figure reaches 62 pixels.
scale = 62 / max(b[3] - b[1] for _, b in panels)
out_frames = ROOT / "frames"
out_frames.mkdir(exist_ok=True)
frames = []
parameters = {"source_size": src.size, "canvas": [80, 80], "baseline_y": 75,
              "scale": scale, "sample_phase": [0.5, 0.5], "alpha_threshold": 128,
              "panels": {}}
for name, (panel, box) in zip(NAMES, panels):
    cut = panel.crop(box)
    w, h = round(cut.width * scale), round(cut.height * scale)
    small = cut.resize((w, h), Image.Resampling.NEAREST)
    alpha = small.getchannel("A").point(lambda a: 255 if a >= 128 else 0)
    small.putalpha(alpha)
    px = small.load()
    for y in range(h):
        for x in range(w):
            if px[x, y][3] == 0:
                px[x, y] = (0, 0, 0, 0)
    frame = Image.new("RGBA", (80, 80))
    left = (80 - w) // 2
    top = 76 - h
    frame.alpha_composite(small, (left, top))
    path = out_frames / f"Kakashi_{name}.png"
    frame.save(path)
    frames.append(frame)
    parameters["panels"][name] = {"source_box_in_panel": box, "sampled_size": [w, h],
                                   "frame_offset": [left, top], "file": str(path.relative_to(ROOT))}

def on_bg(im, factor, bg):
    tile = Image.new("RGBA", (80, 80), bg)
    tile.alpha_composite(im)
    return tile.convert("RGB").resize((80 * factor, 80 * factor), Image.Resampling.NEAREST)

for factor in (3, 6):
    sheet = Image.new("RGB", (80 * factor * 3, 80 * factor * 2), "white")
    for i, frame in enumerate(frames):
        sheet.paste(on_bg(frame, factor, (35, 40, 50, 255)), (i * 80 * factor, 0))
        sheet.paste(on_bg(frame, factor, (230, 228, 220, 255)), (i * 80 * factor, 80 * factor))
    sheet.save(ROOT / f"opposite_check_{factor}x.png")

old = [Image.open(V3 / f"Kakashi_{i:02d}_Walk.png").convert("RGBA") for i in (1, 2, 4)]
cycle = old + frames
strip = Image.new("RGBA", (80 * 6, 80))
for i, frame in enumerate(cycle):
    strip.alpha_composite(frame, (i * 80, 0))
strip.save(ROOT / "cycle_strip.png")
cycle[0].save(ROOT / "cycle.gif", save_all=True, append_images=cycle[1:], duration=130,
              loop=0, disposal=2, transparency=0)
(ROOT / "export_parameters.json").write_text(json.dumps(parameters, indent=2) + "\n")
