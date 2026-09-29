#!/usr/bin/env python3
"""Turn one 40x56 head-gear frame into a vanilla-layout head equip sheet (40x1120, 20 frames of 56).

Vanilla heads bob by a pixel or two during the walk cycle; the per-frame vertical offsets were measured from a
vanilla helmet (Armor_Head_1) and stored in scripts/vanilla_head_frame_offsets.json.
Usage: python3 scripts/build_head_equip.py IN_FRAME.png OUT_SHEET.png
"""
import json
import sys
from pathlib import Path
from PIL import Image

offsets = json.loads((Path(__file__).parent / "vanilla_head_frame_offsets.json").read_text())
frame = Image.open(sys.argv[1]).convert("RGBA")
sheet = Image.new("RGBA", (40, 56 * len(offsets)), (0, 0, 0, 0))
for i, dy in enumerate(offsets):
    sheet.paste(frame, (0, i * 56 + dy), frame)
sheet.save(sys.argv[2])
print(sys.argv[2], sheet.size)
