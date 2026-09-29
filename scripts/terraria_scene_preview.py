#!/usr/bin/env python3
"""Place boss sprites next to vanilla Terraria sprites on grass to judge whether they fit.

Vanilla PNGs come from tools/XnbExtract (Terraria's compressed XNB textures):
  python3 scripts/terraria_scene_preview.py <vanilla_png_dir> <unused> <zabuza.png> <haku.png> <out.png>
"""
import sys
from PIL import Image
X, S, zab, haku, outname = sys.argv[1:6]
W, H = 560, 170
scene = Image.new('RGBA', (W, H), (120, 170, 235, 255))
grass = Image.open(X + '/Tiles_2.png').convert('RGBA')
dirt = Image.open(X + '/Tiles_0.png').convert('RGBA')
top = grass.crop((18, 0, 34, 16)); fill = dirt.crop((18, 18, 34, 34))
ground = 130
for x in range(0, W, 16):
    scene.alpha_composite(top, (x, ground))
    for y in range(ground + 16, H, 16):
        scene.alpha_composite(fill, (x, y))
def frame(path, h):
    im = Image.open(path).convert('RGBA'); return im.crop((0, 0, im.width, h))
def place(im, cx):
    bb = im.getbbox(); im = im.crop(bb)
    scene.alpha_composite(im, (int(cx - im.width / 2), ground - im.height))
place(frame(X + '/NPC_22.png', 56), 40)
place(frame(X + '/NPC_3.png', 48), 90)
place(frame(X + '/NPC_439.png', 64), 150)
place(Image.open(zab).convert('RGBA'), 270)
place(Image.open(haku).convert('RGBA'), 380)
place(frame(X + '/NPC_4.png', 166), 480)
scene.resize((W * 3, H * 3), Image.NEAREST).save(outname)
