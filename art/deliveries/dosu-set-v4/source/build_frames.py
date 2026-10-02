"""Build the delivered Dosu frames from the named 2x2-pixel base.

Uses ffmpeg only to decode PNGs. The drawing and PNG encoder use stdlib.
"""
from __future__ import annotations

import math
import struct
import subprocess
import zlib
from collections import Counter
from pathlib import Path

HERE = Path(__file__).resolve().parent
OUT = HERE.parent
ROOT = HERE.parents[3]
BASE_PATH = ROOT / "art/deliveries/dosu-base-v3/Dosu_Idle_0.png"
OLD_DIR = ROOT / "ShinobiPrototype/Content/NPCs"
W, H = 56, 44
EMPTY = (0, 0, 0, 0)


def read_png(path: Path, width: int, height: int):
    raw = subprocess.check_output(
        ["ffmpeg", "-v", "error", "-i", str(path), "-f", "rawvideo", "-pix_fmt", "rgba", "-"],
    )
    assert len(raw) == width * height * 4, (path, len(raw))
    return [[tuple(raw[4 * (y * width + x):4 * (y * width + x) + 4])
             for x in range(width)] for y in range(height)]


def png_chunk(kind, data):
    return struct.pack(">I", len(data)) + kind + data + struct.pack(">I", zlib.crc32(kind + data))


def save_png(path: Path, pix):
    height = len(pix)
    width = len(pix[0])
    raw = bytearray()
    for row in pix:
        raw.append(0)
        for pixel in row:
            raw.extend(pixel)
    payload = bytearray(b"\x89PNG\r\n\x1a\n")
    payload += png_chunk(b"IHDR", struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0))
    payload += png_chunk(b"IDAT", zlib.compress(bytes(raw), 9))
    payload += png_chunk(b"IEND", b"")
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_bytes(payload)


def blank(w=W, h=H):
    return [[EMPTY for _ in range(w)] for _ in range(h)]


def put(a, x, y, color):
    if 0 <= x < len(a[0]) and 0 <= y < len(a):
        a[y][x] = color


def rect(a, x0, y0, x1, y1, color):
    for y in range(y0, y1 + 1):
        for x in range(x0, x1 + 1):
            put(a, x, y, color)


full = read_png(BASE_PATH, 112, 88)
base = [[full[y * 2][x * 2] for x in range(W)] for y in range(H)]
palette = [color for color, _ in Counter(c for row in base for c in row if c[3]).most_common()]
C = {"outline": palette[0], "fur_shadow": palette[1], "fur": palette[2],
     "pants": palette[3], "fur_mid": palette[4], "metal": palette[5],
     "metal_mid": palette[6], "ink2": palette[7], "cape_mid": palette[8],
     "fur_light": palette[9], "bandage_light": palette[10],
     "metal_dark": palette[11], "bandage": palette[12], "metal_blue": palette[13],
     "metal_shade": palette[14], "skin_shadow": palette[15],
     "skin": palette[16], "metal_high": palette[17], "metal_dim": palette[18],
     "metal_deep": palette[19], "metal_nearblack": palette[20],
     "metal_edge": palette[21], "bandage_white": palette[22],
     "skin_light": palette[23], "skin_mid": palette[24],
     "skin_deep": palette[25], "skin_bright": palette[26]}


def clean_base():
    a = [row[:] for row in base]
    # Replace the flat, ladder-like guard from the base with shaded fur below it.
    for y in range(22, 35):
        for x in range(17, 24):
            if a[y][x] in {C[k] for k in ("metal", "metal_mid", "metal_dark", "metal_blue", "metal_shade", "metal_high", "metal_dim", "metal_deep", "metal_nearblack", "metal_edge", "bandage_white")}:
                a[y][x] = C["fur_shadow"] if x <= 19 else C["fur_mid"]
    # Close the crown, keep a short dark gray untidy hair tuft above the wraps.
    for x, y, k in [(30, 11, "outline"), (31, 11, "metal_edge"), (32, 11, "metal_nearblack"),
                    (33, 12, "metal_deep"), (29, 12, "ink2"), (32, 12, "metal_mid")]:
        put(a, x, y, C[k])
    # Darker material break under the light side of the fur mantle.
    for y in range(23, 33):
        for x in range(24, 29):
            if a[y][x] == C["fur_mid"] and (x + y) % 3 == 0:
                a[y][x] = C["fur_shadow"]
    return a


def line(a, x0, y0, x1, y1, color, width=1):
    steps = max(abs(x1 - x0), abs(y1 - y0), 1)
    for i in range(steps + 1):
        x = round(x0 + (x1 - x0) * i / steps)
        y = round(y0 + (y1 - y0) * i / steps)
        for yy in range(y - width // 2, y + (width + 1) // 2):
            for xx in range(x - width // 2, x + (width + 1) // 2):
                put(a, xx, yy, color)


def guard(a, x, y, angle="down", shoulder=None):
    """Cylindrical four-port guard, all colors taken from the base."""
    if shoulder is not None:
        tx, ty = {"down": (x + 2, y), "up": (x + 2, y + 11),
                  "right": (x, y + 2), "left": (x + 11, y + 2),
                  "diag_up": (x + 2, y + 9), "diag_down": (x + 2, y)}[angle]
        line(a, *shoulder, tx, ty, C["outline"], 4)
        line(a, *shoulder, tx, ty, C["bandage"], 2)
        line(a, *shoulder, tx, ty, C["bandage_light"], 1)
    for v in range(12):
        for u in range(6):
            # Rounded caps, solid nearly black edge, four distinct metal shades.
            if v in (0, 11) and u in (0, 5):
                continue
            if u in (0, 5) or v in (0, 11):
                col = C["outline"]
            elif u == 1:
                col = C["metal_dark"]
            elif u == 2:
                col = C["metal_mid"]
            elif u == 3:
                col = C["metal"]
            else:
                col = C["metal_high"]
            if v in (1, 10) and 1 <= u <= 4:
                col = C["metal_shade"] if v == 1 else C["metal_blue"]
            if v in (2, 4, 6, 8) and u in (3, 4):
                col = C["metal_nearblack"] if u == 3 else C["metal_edge"]
            if angle == "down":
                px, py = x + u, y + v
            elif angle == "up":
                px, py = x + u, y + 11 - v
            elif angle == "right":
                px, py = x + v, y + 5 - u
            elif angle == "left":
                px, py = x + 11 - v, y + u
            elif angle == "diag_up":
                px, py = x + v, y + 9 - v // 2 + (2 - u)
            else:
                px, py = x + v, y + v // 2 + (2 - u)
            put(a, px, py, col)


def transformed(a, lean=0, bob=0, crouch=0, leap=0, stride=0, flare=0):
    out = blank()
    for y in range(H):
        for x in range(W):
            c = a[y][x]
            if not c[3]:
                continue
            # Idle breathing bobs the torso, while planted feet retain y=83.
            dy = (bob if y < 35 else 0) - leap + (crouch if y < 35 else 0)
            dx = round(lean * max(0, 34 - y) / 23)
            if y >= 35:
                # Alternating leg position; torso and arm stay registered.
                phase = (y - 34) / 7
                dx += round(stride * phase * (1 if x < 27 else -1))
                if y >= 39 and abs(stride) >= 2 and ((stride > 0 and x < 27) or (stride < 0 and x >= 27)):
                    dy -= 1
            elif y >= 30 and x < 24:
                dx -= flare
            put(out, x + dx, y + dy, c)
    return out


def palette_coverage(a):
    """Retain at least 22 base colors using nearby material highlights."""
    present = {c for row in a for c in row if c[3]}
    missing = [c for c in palette if c not in present]
    for col in missing:
        # Find the most similar already present interior color. At this tiny
        # scale, an adjacent shade is a deliberate material ramp, not noise.
        candidates = []
        for y in range(14, 35):
            for x in range(17, 38):
                old = a[y][x]
                if not old[3] or any(a[yy][xx][3] == 0 for xx, yy in ((x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1))):
                    continue
                dist = sum((old[i] - col[i]) ** 2 for i in range(3))
                candidates.append((dist, y, x))
        if candidates:
            _, y, x = min(candidates)
            a[y][x] = col
    return a


def outline(a):
    old = [row[:] for row in a]
    for y in range(1, H - 1):
        for x in range(1, W - 1):
            if not old[y][x][3]:
                continue
            if any(not old[yy][xx][3] for xx, yy in ((x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1))):
                a[y][x] = C["outline"]
    return a


def connect_silhouette(a):
    """Close small animation gaps so every pose remains one readable figure."""
    while True:
        remaining = {(x, y) for y in range(H) for x in range(W) if a[y][x][3]}
        groups = []
        while remaining:
            stack = [remaining.pop()]
            group = set(stack)
            while stack:
                x, y = stack.pop()
                for p in ((x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)):
                    if p in remaining:
                        remaining.remove(p)
                        group.add(p)
                        stack.append(p)
            groups.append(group)
        if len(groups) <= 1:
            return a
        groups.sort(key=len, reverse=True)
        body = groups[0]
        loose = groups[1]
        x0, y0, x1, y1 = min(
            ((x0, y0, x1, y1) for x0, y0 in body for x1, y1 in loose),
            key=lambda v: (v[0] - v[2]) ** 2 + (v[1] - v[3]) ** 2,
        )
        line(a, x0, y0, x1, y1, C["outline"], 2)


def to_game(a):
    return [[a[y // 2][x // 2] for x in range(112)] for y in range(88)]


master = clean_base()
frames = {}


def add(name, *, lean=0, bob=0, crouch=0, leap=0, stride=0, flare=0,
        armor=(17, 23, "down"), shoulder=None, hem=0):
    a = transformed(master, lean, bob, crouch, leap, stride, flare)
    armor = (armor[0], armor[1] + bob, armor[2])
    if shoulder is not None:
        shoulder = (shoulder[0], shoulder[1] + bob)
    if hem:
        # A small cape hem shift during breathing/walking.
        for y in (32, 33, 34):
            x = 18 - hem if y == 34 else 19 - hem
            put(a, x, y + bob - leap, C["outline"])
            put(a, x + 1, y + bob - leap, C["fur_shadow"])
    if shoulder is None:
        guard(a, *armor)
    else:
        guard(a, *armor, shoulder=shoulder)
    palette_coverage(a)
    connect_silhouette(a)
    frames[name] = a
    save_png(OUT / (name + ".png"), to_game(a))


for i, (bob, hem) in enumerate([(0, 0), (0, 1), (-1, 1), (-1, 0), (0, -1), (0, 0)]):
    add(f"Dosu_Idle_{i}", bob=bob, hem=hem)

for i, (stride, bob, hem) in enumerate([(0, 0, 0), (1, -1, 1), (2, 0, 1), (3, 0, 2),
                                        (0, 0, 0), (-1, -1, -1), (-2, 0, -1), (-3, 0, -2)]):
    add(f"Dosu_Walk_{i}", stride=stride, bob=bob, hem=hem)

add("Dosu_Hurt_0", lean=-7, bob=0, stride=-2, armor=(15, 19, "diag_up"), shoulder=(25, 24))
add("Dosu_DrillWindupIn_0", lean=-2, flare=1, armor=(12, 21, "down"), shoulder=(24, 23))
add("Dosu_DrillWindup_0", lean=-3, flare=2, armor=(11, 20, "down"), shoulder=(24, 23))
add("Dosu_DrillWindup_1", lean=-4, bob=-1, flare=2, armor=(10, 20, "down"), shoulder=(23, 22))
add("Dosu_Drill_0", lean=4, stride=3, flare=2, armor=(34, 20, "right"), shoulder=(30, 23))
add("Dosu_Drill_1", lean=5, stride=4, bob=-1, flare=2, armor=(37, 20, "right"), shoulder=(31, 23))
add("Dosu_WaveIn_0", lean=-1, armor=(26, 10, "up"), shoulder=(29, 22))
add("Dosu_Wave_0", lean=-1, armor=(27, 8, "up"), shoulder=(29, 22))
add("Dosu_Wave_1", lean=2, armor=(35, 16, "right"), shoulder=(30, 22))
add("Dosu_Wave_2", lean=1, armor=(31, 21, "diag_down"), shoulder=(30, 22))
add("Dosu_SlamIn_0", crouch=1, armor=(29, 7, "up"), shoulder=(29, 22))
add("Dosu_Slam_0", armor=(30, 5, "up"), shoulder=(29, 22))
add("Dosu_Slam_1", crouch=5, stride=3, flare=2, armor=(34, 29, "down"), shoulder=(30, 27))
add("Dosu_Slam_2", crouch=2, stride=1, armor=(31, 21, "diag_down"), shoulder=(29, 24))
add("Dosu_LeapIn_0", crouch=4, stride=2, armor=(20, 26, "down"), shoulder=(28, 27))
add("Dosu_Leap_0", crouch=5, stride=2, armor=(20, 26, "down"), shoulder=(28, 27))
add("Dosu_Leap_1", leap=7, lean=3, stride=3, flare=2, armor=(31, 24, "down"), shoulder=(29, 17))
add("Dosu_Leap_2", crouch=5, stride=3, flare=2, armor=(32, 26, "down"), shoulder=(28, 28))

# 1x1 pixel minimap portrait from the canonical head, no automatic smoothing.
head = blank(30, 30)
src = frames["Dosu_Idle_0"]
for y in range(30):
    for x in range(30):
        # Tight crop around the bandaged face, tuft, eye and upper mantle.
        head[y][x] = src[11 + y // 4][29 + x // 4]
save_png(OUT / "Dosu_Head_Boss.png", head)


def scale(a, n):
    return [[a[y // n][x // n] for x in range(len(a[0]) * n)]
            for y in range(len(a) * n)]


def composite_preview(names, n, path):
    cols = 5
    cw = 112 * n + 16
    ch = 88 * n + 22
    rows = math.ceil(len(names) / cols)
    bg = (35, 40, 54, 255)
    canvas = [[bg for _ in range(cw * cols)] for _ in range(ch * rows)]
    for i, name in enumerate(names):
        if name == "Dosu_base_v3":
            src = base
        elif name.startswith("Old_"):
            full_ref = read_png(OLD_DIR / (name[4:] + ".png"), 112, 88)
        elif name == "Tazuna":
            # One 56x56 in-game frame, at native size, feet aligned at y=83.
            native = read_png(OLD_DIR / "Tazuna.png", 56, 672)
            full_ref = blank(112, 88)
            for yy in range(56):
                for xx in range(56):
                    full_ref[28 + yy][28 + xx] = native[yy][xx]
        else:
            src = frames[name]
        pix = scale(full_ref if name == "Tazuna" or name.startswith("Old_") else to_game(src), n)
        ox = (i % cols) * cw + 8
        oy = (i // cols) * ch + 8
        for y, row in enumerate(pix):
            for x, c in enumerate(row):
                if c[3]:
                    canvas[oy + y][ox + x] = c
        label(canvas, ox, oy + 88 * n + 4, name)
    save_png(path, canvas)


FONT = {
    "A": ("010", "101", "111", "101", "101"),
    "B": ("110", "101", "110", "101", "110"),
    "C": ("011", "100", "100", "100", "011"),
    "D": ("110", "101", "101", "101", "110"),
    "E": ("111", "100", "110", "100", "111"),
    "F": ("111", "100", "110", "100", "100"),
    "G": ("011", "100", "101", "101", "011"),
    "H": ("101", "101", "111", "101", "101"),
    "I": ("111", "010", "010", "010", "111"),
    "J": ("001", "001", "001", "101", "010"),
    "K": ("101", "101", "110", "101", "101"),
    "L": ("100", "100", "100", "100", "111"),
    "M": ("101", "111", "111", "101", "101"),
    "N": ("101", "111", "111", "111", "101"),
    "O": ("010", "101", "101", "101", "010"),
    "P": ("110", "101", "110", "100", "100"),
    "R": ("110", "101", "110", "101", "101"),
    "S": ("011", "100", "010", "001", "110"),
    "T": ("111", "010", "010", "010", "010"),
    "U": ("101", "101", "101", "101", "111"),
    "V": ("101", "101", "101", "101", "010"),
    "W": ("101", "101", "111", "111", "101"),
    "X": ("101", "101", "010", "101", "101"),
    "Y": ("101", "101", "010", "010", "010"),
    "Z": ("111", "001", "010", "100", "111"),
    "0": ("111", "101", "101", "101", "111"),
    "1": ("010", "110", "010", "010", "111"),
    "2": ("110", "001", "010", "100", "111"),
    "3": ("110", "001", "010", "001", "110"),
    "4": ("101", "101", "111", "001", "001"),
    "5": ("111", "100", "110", "001", "110"),
    "6": ("011", "100", "110", "101", "010"),
    "7": ("111", "001", "010", "010", "010"),
    "8": ("010", "101", "010", "101", "010"),
    "9": ("010", "101", "011", "001", "110"),
    "_": ("000", "000", "000", "000", "111"),
    " ": ("000", "000", "000", "000", "000"),
}


def label(a, x, y, value):
    for ch in value.upper():
        glyph = FONT.get(ch, FONT[" "])
        for yy, row in enumerate(glyph):
            for xx, bit in enumerate(row):
                if bit == "1":
                    put(a, x + xx, y + yy, (212, 216, 224, 255))
        x += 4


old_names = ["Old_" + f.stem for f in sorted(OLD_DIR.glob("Dosu_*.png"))
             if f.name != "Dosu_Head_Boss.png"]
names = list(frames) + ["Dosu_base_v3"] + old_names + ["Tazuna"]
composite_preview(names, 1, OUT / "preview_1x.png")
composite_preview(names, 4, OUT / "preview_4x.png")

small = read_png(OUT / "preview_1x.png", (112 + 16) * 5, (88 + 22) * math.ceil(len(names) / 5))
large = read_png(OUT / "preview_4x.png", (112 * 4 + 16) * 5, (88 * 4 + 22) * math.ceil(len(names) / 5))
big_w = len(large[0])
combined = [[(35, 40, 54, 255) for _ in range(big_w)] for _ in range(len(small) + len(large) + 22)]
for y, row in enumerate(small):
    combined[y] = row + combined[y][len(row):]
for y, row in enumerate(large):
    combined[len(small) + 22 + y] = row
label(combined, 8, len(small) + 8, "4X")
save_png(OUT / "preview.png", combined)
(OUT / "preview_index.txt").write_text(
    "Grid order: five columns, left to right, top to bottom.\n"
    + "\n".join(f"{i + 1:02d} {name}" for i, name in enumerate(names)) + "\n",
    encoding="utf-8",
)


def overlay(series, path):
    # Each color is a different frame; transparency in the canvas marks the
    # common registered silhouette. Preview includes an adjacent 4x version.
    cols = [(219, 80, 80, 130), (233, 160, 63, 130), (227, 216, 73, 130),
            (81, 207, 149, 130), (82, 167, 226, 130), (151, 120, 219, 130),
            (225, 117, 188, 130), (235, 235, 235, 130)]
    a = blank(112, 88)
    for i, name in enumerate(series):
        for y in range(44):
            for x in range(56):
                if frames[name][y][x][3]:
                    for yy in (y * 2, y * 2 + 1):
                        for xx in (x * 2, x * 2 + 1):
                            a[yy][xx] = cols[i]
    save_png(path, a)


overlay([f"Dosu_Idle_{i}" for i in range(6)], OUT / "idle_overlay.png")
overlay([f"Dosu_Walk_{i}" for i in range(8)], OUT / "walk_overlay.png")

counts = {name: len({c for row in a for c in row if c[3]}) for name, a in frames.items()}
print(f"frames={len(frames)} colors min={min(counts.values())} max={max(counts.values())}")
for name, count in counts.items():
    if count < 22:
        print("LOW COLOR", name, count)
