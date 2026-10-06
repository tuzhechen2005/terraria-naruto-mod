#!/usr/bin/env python3
"""Re-export the six M11 ice VFX sprites from the retained imagegen sheet."""

from pathlib import Path
from PIL import Image, ImageDraw
import json

ROOT = Path(__file__).resolve().parent
SOURCE = ROOT / "source" / "wave-ice-vfx-sheet.png"
SPECS = [
    # name, source isolation window, game canvas, visible-art height/width cap
    ("IceMirror", (70, 0, 450, 535), (128, 192), (108, 180)),
    ("MirrorCracks", (575, 0, 955, 510), (128, 192), (88, 155)),
    ("MirrorShards", (1040, 0, 1500, 510), (128, 128), (112, 112)),
    ("NeedleGlow", (10, 680, 510, 850), (96, 32), (90, 21)),
    ("FrostCloud", (550, 525, 1010, 1010), (128, 128), (116, 116)),
    ("MirrorHalo", (1080, 520, 1480, 1010), (128, 192), (108, 180)),
]


def zero_hidden_rgb(im):
    pix = im.load()
    for y in range(im.height):
        for x in range(im.width):
            r, g, b, a = pix[x, y]
            if a < 5:
                pix[x, y] = (0, 0, 0, 0)
    return im


def export():
    sheet = Image.open(SOURCE).convert("RGBA")
    assert sheet.size == (1536, 1024), sheet.size
    (ROOT / "sprites").mkdir(exist_ok=True)
    manifest = {"request_id": "wave-ice-vfx-v1", "source": "source/wave-ice-vfx-sheet.png",
                "preview_loop": {"file": "preview/mirror-loop.apng", "frames": 9, "frame_duration_ms": 100,
                                 "purpose": "illustrative layer timing only; game-side timing remains to be set"},
                "danger_note": "These images are visual layers, not new hitboxes or abilities. NeedleGlow decorates the existing damaging needle; MirrorShards and FrostCloud are non-damaging.",
                "assets": []}
    sprites = {}
    for name, window, canvas, cap in SPECS:
        cut = zero_hidden_rgb(sheet.crop(window))
        bbox = cut.getchannel("A").getbbox()
        assert bbox, name
        art = cut.crop(bbox)
        scale = min(cap[0] / art.width, cap[1] / art.height)
        size = (max(1, round(art.width * scale)), max(1, round(art.height * scale)))
        art = art.resize(size, Image.Resampling.LANCZOS)
        art = zero_hidden_rgb(art)
        final = Image.new("RGBA", canvas)
        xy = ((canvas[0] - size[0]) // 2, (canvas[1] - size[1]) // 2)
        final.alpha_composite(art, xy)
        path = ROOT / "sprites" / f"{name}.png"
        final.save(path)
        sprites[name] = final
        manifest["assets"].append({"file": f"sprites/{name}.png", "source_window": list(window),
                                   "isolated_alpha_bbox": [bbox[0]+window[0], bbox[1]+window[1], bbox[2]+window[0], bbox[3]+window[1]],
                                   "canvas": list(canvas), "art_size": list(size), "art_origin": list(xy),
                                   "anchor": [canvas[0]//2, canvas[1]//2],
                                   "facing": "right" if name == "NeedleGlow" else "axial",
                                   "role": ("existing needle visual emphasis; align tip with projectile hitbox" if name == "NeedleGlow"
                                            else "decorative aftermath; no damage" if name in ("MirrorShards", "FrostCloud")
                                            else "mirror visual/telegraph; gameplay hitbox determined by game code"),
                                   "suggested_visibility": ("during existing projectile flight" if name == "NeedleGlow"
                                                            else "brief fade after mirror break" if name == "MirrorShards"
                                                            else "brief low-opacity ambient use" if name == "FrostCloud"
                                                            else "only active mirror; allow normal mirrors to stay dim" if name == "MirrorHalo"
                                                            else "during mirror damage" if name == "MirrorCracks"
                                                            else "during mirror form and active state")})
    (ROOT / "manifest.json").write_text(json.dumps(manifest, indent=2, ensure_ascii=False) + "\n")
    return sprites


def preview(sprites, bg, path):
    cell_w, cell_h = 160, 224
    board = Image.new("RGBA", (3*cell_w, 2*cell_h), bg)
    for i, (name, _, canvas, _) in enumerate(SPECS):
        x = (i%3)*cell_w + (cell_w-canvas[0])//2
        y = (i//3)*cell_h + (cell_h-canvas[1])//2
        board.alpha_composite(sprites[name], (x,y))
    board.convert("RGB").save(path)


def loop(sprites):
    # A compact 1x composition: form, active halo/needle, cracks, break/aftermath.
    frames=[]
    frame_plan=[(0.45,0,0,0),(0.8,0.25,0,0),(1,0.6,0,0),
                (1,1,0,0),(1,0.8,0.5,0),(1,0.5,1,0),
                (0.6,0.1,0.8,0.5),(0.2,0,0.2,1),(0,0,0,0.55)]
    for mirror_a, halo_a, crack_a, shard_a in frame_plan:
        frame=Image.new("RGBA", (256,256), (20,35,49,255))
        def place(name, x, y, opacity):
            art=sprites[name].copy()
            art.putalpha(art.getchannel("A").point(lambda a:round(a*opacity)))
            frame.alpha_composite(art,(x,y))
        if mirror_a: place("IceMirror",64,25,mirror_a)
        if halo_a: place("MirrorHalo",64,25,halo_a)
        if crack_a: place("MirrorCracks",64,25,crack_a)
        if shard_a: place("MirrorShards",64,57,shard_a)
        frames.append(frame)
    frames[0].save(ROOT/"preview"/"mirror-loop.apng", save_all=True, append_images=frames[1:], duration=100, loop=0)


if __name__ == "__main__":
    (ROOT / "preview").mkdir(exist_ok=True)
    sprites=export()
    preview(sprites,(205,219,226,255),ROOT/"preview"/"light-1x.png")
    preview(sprites,(20,35,49,255),ROOT/"preview"/"dark-1x.png")
    loop(sprites)
