from pathlib import Path
from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parent
B3 = ROOT.parent / "npc-hires-sample-b3"


def make_anko():
    im = Image.open(B3 / "Anko_Idle.png").convert("RGBA")
    d = ImageDraw.Draw(im)
    outline = (7, 3, 14, 255)
    hair_dark = (40, 25, 48, 255)
    hair_mid = (71, 45, 81, 255)
    skin = (253, 197, 143, 255)
    skin_light = (252, 229, 198, 255)
    skin_shadow = (221, 160, 117, 255)
    eye_white = (255, 245, 223, 255)
    iris = (110, 74, 76, 255)
    metal = (166, 166, 180, 255)
    metal_light = (217, 211, 204, 255)

    # Keep the b3 ponytail, body, and rear hair. Replace the obscuring front fringe.
    d.rectangle((39, 25, 52, 38), fill=(0, 0, 0, 0))
    d.polygon([(39, 26), (42, 24), (49, 24), (52, 27), (52, 31),
               (51, 35), (48, 38), (41, 38), (38, 34)], fill=outline)
    d.polygon([(39, 29), (42, 27), (49, 27), (51, 30), (50, 35),
               (47, 37), (42, 37), (39, 34)], fill=skin)
    d.polygon([(40, 30), (41, 28), (43, 28), (42, 34), (40, 35), (39, 33)], fill=skin_shadow)
    d.line([(42, 36), (46, 37), (48, 36)], fill=skin_shadow)
    d.line([(44, 29), (49, 29)], fill=skin_light)
    # Konoha forehead plate and short loose wisps.
    d.rectangle((40, 25, 51, 27), fill=hair_dark)
    d.rectangle((43, 25, 49, 27), fill=metal)
    d.line([(44, 25), (48, 25)], fill=metal_light)
    d.point((46, 26), fill=hair_dark)
    d.line([(39, 27), (41, 28)], fill=hair_mid)
    d.line([(41, 27), (42, 29)], fill=hair_dark)
    d.line([(50, 27), (51, 29)], fill=hair_dark)
    d.line([(38, 30), (38, 33)], fill=skin_shadow)
    d.point((46, 26), fill=hair_dark)
    # Two readable eyes at game scale: eyeliner, sclera, purple-brown iris, glint.
    d.line([(42, 31), (46, 31)], fill=outline)
    d.rectangle((43, 32, 46, 34), fill=eye_white)
    d.rectangle((45, 33, 46, 34), fill=iris)
    d.point((45, 32), fill=eye_white)
    d.line([(49, 31), (51, 31)], fill=outline)
    d.rectangle((49, 32, 50, 34), fill=eye_white)
    d.point((50, 34), fill=iris)
    d.point((50, 32), fill=eye_white)
    # Small nose and a one-sided raised smile.
    d.point((51, 34), fill=skin_shadow)
    d.line([(44, 35), (46, 36), (49, 36), (50, 35), (51, 34)], fill=iris)
    im.save(ROOT / "Anko_Idle.png")


def make_hayate():
    im = Image.open(B3 / "Hayate_Idle.png").convert("RGBA")
    px = im.load()
    # Collapse nearly identical old outline shades to make room in the 20-color palette.
    dark = (15, 5, 25, 255)
    for y in range(80):
        for x in range(80):
            if px[x, y][:3] in {(14, 4, 24), (11, 4, 19), (19, 9, 31), (15, 6, 29)} and px[x, y][3]:
                px[x, y] = dark
    d = ImageDraw.Draw(im)
    g_shadow = (62, 77, 56, 255)
    g_mid = (89, 110, 69, 255)
    g_light = (130, 146, 88, 255)
    navy = (38, 36, 65, 255)
    blade = (166, 166, 180, 255)
    # Diagonal wrapped sword grip behind the near shoulder.
    d.line([(35, 42), (28, 34)], fill=dark, width=5)
    d.line([(34, 41), (28, 34)], fill=navy, width=3)
    for x, y in [(28, 34), (30, 36), (32, 38)]:
        d.line([(x, y), (x + 1, y - 1)], fill=blade, width=1)
    d.ellipse((26, 31, 29, 34), fill=dark)
    d.line([(27, 32), (28, 32)], fill=blade)
    # Only the flak vest is green. Blue sleeves and trousers stay as b3.
    d.polygon([(36, 39), (40, 38), (49, 38), (53, 41), (53, 51),
               (51, 53), (36, 53), (34, 50), (34, 42)], fill=dark)
    d.polygon([(37, 40), (40, 39), (49, 39), (52, 41), (52, 50),
               (50, 52), (37, 52), (35, 49), (35, 42)], fill=g_mid)
    d.line([(37, 41), (39, 39), (41, 41)], fill=g_light)
    d.line([(49, 39), (51, 41)], fill=g_light)
    d.line([(36, 49), (51, 49)], fill=g_shadow)
    d.line([(37, 52), (50, 52)], fill=g_shadow)
    d.line([(45, 40), (45, 51)], fill=dark)
    d.rectangle((38, 43, 43, 47), outline=g_shadow)
    d.rectangle((47, 43, 51, 47), outline=g_shadow)
    d.point((39, 44), fill=g_light)
    d.point((48, 44), fill=g_light)
    d.line([(36, 42), (36, 48)], fill=g_shadow)
    d.line([(52, 42), (52, 48)], fill=g_shadow)
    im.save(ROOT / "Hayate_Idle.png")


make_anko()
make_hayate()


def make_previews():
    accepted = ROOT.parent / "npc-hires-accepted"
    sprites = [
        ("Hiruzen", accepted / "Hiruzen_Idle.png"),
        ("Ibiki", accepted / "Ibiki_Idle.png"),
        ("Iruka", accepted / "Iruka_Idle.png"),
        ("Kakashi", accepted / "Kakashi_Idle.png"),
        ("Haku", accepted / "HakuForest_Idle.png"),
        ("Tazuna", accepted / "Tazuna_Idle.png"),
        ("Shopkeeper", accepted / "ToolShopkeeper_Idle.png"),
        ("Anko b4", ROOT / "Anko_Idle.png"),
        ("Hayate b4", ROOT / "Hayate_Idle.png"),
    ]
    canvas = Image.new("RGBA", (2160, 386), (47, 49, 58, 255))
    d = ImageDraw.Draw(canvas)
    for i, (label, path) in enumerate(sprites):
        im = Image.open(path).convert("RGBA")
        cx = i * 240 + 120
        canvas.alpha_composite(im, (cx - 40, 22))
        canvas.alpha_composite(im.resize((240, 240), Image.Resampling.NEAREST), (i * 240, 138))
        d.text((i * 240 + 8, 7), label, fill=(244, 239, 224, 255))
    d.text((8, 111), "1x", fill=(244, 239, 224, 255))
    d.text((8, 125), "3x", fill=(244, 239, 224, 255))
    canvas.save(ROOT / "preview.png")

    face_canvas = Image.new("RGBA", (600, 328), (47, 49, 58, 255))
    fd = ImageDraw.Draw(face_canvas)
    for i, (name, path) in enumerate(sprites[-2:]):
        im = Image.open(path).convert("RGBA")
        crop = im.crop((27, 14, 57, 44))
        ox = 36 + i * 300
        oy = 42
        for y in range(30):
            for x in range(30):
                shade = (58, 60, 70, 255) if (x + y) % 2 == 0 else (65, 67, 76, 255)
                fd.rectangle((ox + 8 * x, oy + 8 * y, ox + 8 * x + 7, oy + 8 * y + 7), fill=shade)
        face_canvas.alpha_composite(crop.resize((240, 240), Image.Resampling.NEAREST), (ox, oy))
        for n in range(31):
            fd.line((ox + n * 8, oy, ox + n * 8, oy + 240), fill=(91, 93, 101, 180))
            fd.line((ox, oy + n * 8, ox + 240, oy + n * 8), fill=(91, 93, 101, 180))
        fd.text((ox, 12), name + " 8x", fill=(244, 239, 224, 255))
        for row in range(0, 30, 2):
            fd.text((ox - 23, oy + row * 8), str(14 + row), fill=(225, 220, 208, 255))
        fd.text((ox, 293), "x=27..56, y=14..43", fill=(225, 220, 208, 255))
    face_canvas.save(ROOT / "faces_8x.png")


make_previews()
