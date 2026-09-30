from pathlib import Path
from PIL import Image, ImageDraw, ImageFont
import shutil

HERE = Path(__file__).resolve().parent.parent
SOURCE = HERE / 'source'
PROJECT = HERE.parents[2]
GENERATED = Path('/Users/tuzhechen/.codex/generated_images/01a0f440-0edb-7752-a6ed-b924fa3f6961')

jobs = [
    ('Gaara_Walk', 'exec-3f3c62fc-389b-4d63-a92c-5a644476aa7a.png', 'Gaara_Walk_source.png', [0, 545, 1135, 1660, 2115], (112, 88), 64, 83, [39, 40, 39, 40], 'Gaara_Idle_0.png'),
    ('Gaara_Cracked_Idle', 'exec-26540bf8-132e-4585-8b16-31c25b6fd32e.png', 'Gaara_Cracked_source.png', [0, 565, 1110, 1630, 2172], (112, 88), 64, 83, [36, 36, 36, 36], 'Gaara_Idle_0.png'),
    ('Gaara_Beast_Arm', 'exec-119d9d90-7440-469f-8922-1c1aa893eb23.png', 'Gaara_Beast_Arm_source.png', [0, 650, 1610, 2172], (176, 104), 94, 99, [14, 35, 22], 'Gaara_Beast_Idle_0.png'),
    ('Dosu_Idle', 'exec-ce728296-21d8-4ffc-b179-092b70652274.png', 'Dosu_Idle_source.png', [0, 525, 1025, 1535, 2025], (112, 88), 60, 83, [29, 29, 29, 29], 'Dosu_Drill_0.png'),
    ('Dosu_Walk', 'exec-7ca31423-b460-493c-b688-27c89468bd39.png', 'Dosu_Walk_source.png', [0, 600, 1020, 1540, 2001], (112, 88), 60, 83, [22, 26, 22, 26], 'Dosu_Drill_0.png'),
    ('Dosu_Hurt', 'exec-b11b0cd0-c14f-4eb2-803a-ae01b5b0ee01.png', 'Dosu_Hurt_source.png', [0, 1415], (112, 88), 60, 83, [23], 'Dosu_Drill_0.png'),
]

def nonzero_bbox(im):
    return im.getchannel('A').point(lambda n: 255 if n >= 128 else 0).getbbox()

def frame_crop(im, left, right):
    section = im.crop((left, 0, right, im.height))
    box = nonzero_bbox(section)
    if not box:
        raise ValueError(f'Empty crop: {left}:{right}')
    return section.crop(box)

def load_reference(name):
    folder = 'gaara-base-v1' if name.startswith('Gaara') else 'dosu-base-v1'
    return Image.open(PROJECT / 'art' / 'deliveries' / folder / name).convert('RGBA')

def composite_bg(sprite, color, scale=2):
    bg = Image.new('RGBA', sprite.size, color)
    bg.alpha_composite(sprite)
    return bg.convert('RGB').resize((sprite.width * scale, sprite.height * scale), Image.Resampling.NEAREST)

def main():
    HERE.mkdir(parents=True, exist_ok=True)
    SOURCE.mkdir(parents=True, exist_ok=True)
    outputs = []
    for prefix, generated_name, source_name, cuts, size, desired_height, foot, lefts, reference in jobs:
        source_path = SOURCE / source_name
        shutil.copyfile(GENERATED / generated_name, source_path)
        sheet = Image.open(source_path).convert('RGBA')
        raw = [frame_crop(sheet, cuts[i], cuts[i+1]) for i in range(len(cuts)-1)]
        ratio = desired_height / max(x.height for x in raw)
        for i, (part, left) in enumerate(zip(raw, lefts)):
            scaled = part.resize((max(1, round(part.width * ratio)), max(1, round(part.height * ratio))), Image.Resampling.BOX)
            scaled.putalpha(scaled.getchannel('A').point(lambda n: 255 if n >= 128 else 0))
            canvas = Image.new('RGBA', size)
            if left + scaled.width > size[0]:
                left = size[0] - scaled.width
            if left < 0:
                raise ValueError(f'{prefix}_{i} exceeds canvas width')
            canvas.alpha_composite(scaled, (left, foot + 1 - scaled.height))
            bbox = nonzero_bbox(canvas)
            if bbox is None or bbox[3] != foot + 1:
                raise ValueError(f'{prefix}_{i} foot misaligned: {bbox}')
            path = HERE / f'{prefix}_{i}.png'
            canvas.save(path)
            outputs.append((path, reference))

    # The generated opposite stride repeated the first leg silhouette. Reverse only
    # the trouser/foot region so the loop visibly alternates while facing right.
    opposite = HERE / 'Gaara_Walk_2.png'
    walk2 = Image.open(opposite).convert('RGBA')
    legs = walk2.crop((0, 58, 112, 88)).transpose(Image.Transpose.FLIP_LEFT_RIGHT)
    walk2.paste(legs, (0, 58))
    walk2.save(opposite)

    row_h = 226
    preview = Image.new('RGB', (750, row_h * len(outputs) + 34), '#e2e2e2')
    draw = ImageDraw.Draw(preview)
    draw.text((8, 7), 'file / new dark / new light / original reference dark / original reference light', fill='#151515')
    for idx, (path, ref_name) in enumerate(outputs):
        sprite = Image.open(path).convert('RGBA')
        reference = load_reference(ref_name)
        y = 34 + idx * row_h
        draw.text((8, y+6), path.name, fill='#111111')
        draw.text((8, y+25), f'reference: {ref_name}', fill='#333333')
        for column, (art, bg) in enumerate([(sprite, '#20242a'), (sprite, '#dedede'), (reference, '#20242a'), (reference, '#dedede')]):
            tile = composite_bg(art, bg)
            x = 8 + column * 184
            preview.paste(tile, (x, y+46))
    preview.save(HERE / 'preview.png')

    # Each tile overlays the requested frame and the original combat/idle reference.
    cols = 4
    tile_w, tile_h = 220, 148
    align = Image.new('RGB', (cols*tile_w, ((len(outputs)+cols-1)//cols)*tile_h), '#21242a')
    d = ImageDraw.Draw(align)
    for idx, (path, ref_name) in enumerate(outputs):
        frame = Image.open(path).convert('RGBA')
        ref = load_reference(ref_name)
        tile = Image.new('RGBA', frame.size, '#21242a')
        ref_layer = ref.copy()
        ref_layer.putalpha(ref_layer.getchannel('A').point(lambda n: round(n*0.38)))
        if ref.size == frame.size:
            tile.alpha_composite(ref_layer)
        frame_layer = frame.copy()
        frame_layer.putalpha(frame_layer.getchannel('A').point(lambda n: round(n*0.75)))
        tile.alpha_composite(frame_layer)
        x = (idx % cols)*tile_w
        y = (idx // cols)*tile_h
        align.paste(tile.convert('RGB'), (x+2, y+22))
        d.text((x+2,y+2), path.name, fill='#ffffff')
        foot = 99 if 'Beast' in path.name else 83
        d.line((x+2,y+22+foot,x+2+frame.width,y+22+foot), fill='#00ff66', width=1)
        d.line((x+2+(64 if 'Beast' in path.name else 56),y+22,x+2+(64 if 'Beast' in path.name else 56),y+22+frame.height),fill='#ffeb33',width=1)
    align.save(HERE / 'alignment-overlay.png')

    print(f'wrote {len(outputs)} frame PNGs, preview.png, alignment-overlay.png')
    for path, _ in outputs:
        im = Image.open(path)
        print(path.name, im.size, nonzero_bbox(im), set(im.getchannel('A').getdata()))

if __name__ == '__main__':
    main()
