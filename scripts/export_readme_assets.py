#!/usr/bin/env python3
"""Export README previews from the current game textures; requires Pillow.

Run from any directory with: python3 scripts/export_readme_assets.py
Only documentation assets are written. NPC sheets use their runtime frame
dimensions, and every enlargement uses nearest-neighbour sampling.
"""

from pathlib import Path

from PIL import Image


ROOT = Path(__file__).resolve().parents[1]
NPCS = ROOT / "ShinobiPrototype/Content/NPCs"
ITEMS = ROOT / "ShinobiPrototype/Content/Items"
OUTPUT = ROOT / "docs/images/readme"


def load(path):
    with Image.open(path) as image:
        return image.convert("RGBA")


def union_bounds(frames):
    bounds = [frame.getchannel("A").getbbox() for frame in frames]
    return (
        min(b[0] for b in bounds), min(b[1] for b in bounds),
        max(b[2] for b in bounds), max(b[3] for b in bounds),
    )


def on_canvas(frame, bounds, size, scale=1):
    sprite = frame.crop(bounds)
    if sprite.width > size[0] or sprite.height > size[1] - 4:
        raise ValueError(f"Sprite {sprite.size} does not fit canvas {size}")
    canvas = Image.new("RGBA", size)
    canvas.alpha_composite(sprite, ((size[0] - sprite.width) // 2, size[1] - sprite.height - 4))
    return canvas.resize((size[0] * scale, size[1] * scale), Image.Resampling.NEAREST)


def save_gif(frames, path):
    # One palette for the whole loop avoids colour flicker; reserve index 255
    # for transparency. Full-frame disposal prevents trails on GitHub.
    opaque = []
    for frame in frames:
        pixels = frame.get_flattened_data() if hasattr(frame, "get_flattened_data") else frame.getdata()
        opaque.extend((r, g, b) for r, g, b, a in pixels if a)
    swatches = Image.new("RGB", (len(opaque), 1))
    swatches.putdata(opaque)
    palette = swatches.quantize(colors=255, method=Image.Quantize.MEDIANCUT)
    palette.putpalette(palette.getpalette()[:765] + [0, 0, 0])
    indexed = []
    for frame in frames:
        image = frame.convert("RGB").quantize(palette=palette, dither=Image.Dither.NONE)
        image.paste(255, mask=frame.getchannel("A").point(lambda alpha: 255 if alpha == 0 else 0))
        indexed.append(image)
    indexed[0].save(path, save_all=True, append_images=indexed[1:],
                    duration=170, loop=0, transparency=255, background=255,
                    disposal=2, optimize=False)


def main():
    OUTPUT.mkdir(parents=True, exist_ok=True)
    for name in ("Zabuza", "Haku", "Dosu", "Gaara", "Neji", "Orochimaru"):
        frames = []
        while (NPCS / f"{name}_Idle_{len(frames)}.png").exists():
            frames.append(load(NPCS / f"{name}_Idle_{len(frames)}.png"))
        bounds = union_bounds(frames)
        size = (108, 136) if name == "Orochimaru" else (108, 104)
        previews = [on_canvas(frame, bounds, size, 2) for frame in frames]
        save_gif(previews, OUTPUT / f"{name.lower()}.gif")

    for name in ("Kakashi", "Tazuna", "Ibiki", "Hiruzen"):
        sheet = load(NPCS / f"{name}.png")
        height = 64 if name == "Ibiki" else 56
        frame = sheet.crop((0, 0, sheet.width, height))
        on_canvas(frame, union_bounds([frame]), (56, 64), 3).save(OUTPUT / f"{name.lower()}.png")

    for source, target in (
        ("NinjaHandbook", "ninja-handbook"), ("HeavenScroll", "heaven-scroll"),
        ("EarthScroll", "earth-scroll"), ("Weapons/Kubikiribocho", "kubikiribocho"),
        ("Weapons/Senbon", "senbon"), ("Weapons/WaterDragonJutsu", "water-dragon"),
        ("Weapons/IceMirrorJutsu", "ice-mirror"),
        ("StyleCores/SharinganCore1", "sharingan"),
        ("StyleCores/EightGatesCore", "eight-gates"),
        ("StyleCores/ByakuganCore", "byakugan"),
        ("Jutsu/ScrollClone", "clone-scroll"),
        ("Jutsu/ScrollFireball", "fireball-scroll"),
        ("Jutsu/ScrollChidori", "chidori-scroll"),
    ):
        frame = load(ITEMS / f"{source}.png")
        if source.startswith("StyleCores/"):
            frame.resize((96, 96), Image.Resampling.NEAREST).save(OUTPUT / f"{target}.png")
        else:
            # Centre inventory icons in matching square cells.
            bounds = union_bounds([frame])
            sprite = frame.crop(bounds)
            canvas = Image.new("RGBA", (56, 56))
            canvas.alpha_composite(sprite, ((56 - sprite.width) // 2, (56 - sprite.height) // 2))
            canvas.resize((112, 112), Image.Resampling.NEAREST).save(OUTPUT / f"{target}.png")

    phases = Image.new("RGBA", (528, 104))
    for i, action in enumerate(("Idle", "Cracked_Idle", "Beast_Idle")):
        frame = load(NPCS / f"Gaara_{action}_0.png")
        preview = on_canvas(frame, union_bounds([frame]), (176, 104))
        phases.alpha_composite(preview, (i * 176, 0))
    phases.save(OUTPUT / "gaara-phases.png")
    print(f"Exported 24 README previews to {OUTPUT.relative_to(ROOT)}")


if __name__ == "__main__":
    main()
