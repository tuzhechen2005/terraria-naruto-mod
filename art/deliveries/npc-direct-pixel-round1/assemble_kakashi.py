"""Assemble generated full-body poses; only translate whole frames for anchoring.

No torso/leg splicing, new pixels, per-pose scaling, or face painting.
Run from any directory. Outputs are in this delivery, not the game tree.
"""
from pathlib import Path
import hashlib
import argparse
import json
import shutil
import subprocess
import sys
from PIL import Image
import numpy as np

ROOT = Path(__file__).resolve().parent
REPO = ROOT.parents[2]
OLD = ROOT.parent / 'kakashi-direct-pixel-anim-v2' / 'frames'
V3 = ROOT.parent / 'kakashi-walk-repair-v3' / 'frames'
V4 = ROOT.parent / 'kakashi-walk-phase-v4' / 'frames'
EYES = ROOT.parent / 'kakashi-eye-restore-v5' / 'frames'
OUT = ROOT / 'kakashi_frames'
NAMES = ['Kakashi_00_Idle'] + [f'Kakashi_{i:02d}_Walk' for i in range(1, 7)] + [
    'Kakashi_07_Jump', 'Kakashi_08_Sit', 'Kakashi_09_Throw',
    'Kakashi_10_Throw', 'Kakashi_11_Throw', 'Kakashi_Trapped_0', 'Kakashi_Trapped_1']
SOURCES = [V3 / f'Kakashi_{i:02d}_Walk.png' for i in (1, 2, 4)] + [
    V4 / f'Kakashi_{name}.png' for name in ('OppositeContact', 'OppositePassing', 'ReturnLift')]


def waist_axis(frame):
    """Measure lower vest outline, independent of moving arms/feet and head."""
    a = np.asarray(frame).astype(int)
    r, g, b, alpha = [a[:, :, i] for i in range(4)]
    vest = (alpha == 255) & (g >= r * .9) & (g > b * 1.2) & (g > 40)
    centers = []
    for y in range(45, 51):
        xs = np.flatnonzero(vest[y])
        if len(xs) >= 6:
            centers.append(float(xs[0] + xs[-1]) / 2)
    if not centers:
        raise ValueError('Could not measure vest waist')
    return float(np.median(centers))


def backdrop(frame, scale, color):
    panel = Image.new('RGBA', frame.size, color)
    panel.alpha_composite(frame)
    return panel.convert('RGB').resize((80 * scale, 80 * scale), Image.Resampling.NEAREST)


def save_animation(frames, path):
    # Quantize the whole sequence once: identical input pixels must keep the
    # same displayed colour across frames. APNG preserves the original pixels.
    montage = Image.new('RGB', (frames[0].width * len(frames), frames[0].height))
    for i, frame in enumerate(frames):
        montage.paste(frame, (i * frame.width, 0))
    palette = montage.quantize(colors=256, method=Image.Quantize.MEDIANCUT)
    indexed = [frame.quantize(palette=palette, dither=Image.Dither.NONE) for frame in frames]
    indexed[0].save(path, save_all=True, append_images=indexed[1:], duration=130,
                    loop=0, disposal=2, optimize=False)
    frames[0].save(path.with_suffix('.png'), save_all=True, append_images=frames[1:],
                   duration=130, loop=0, disposal=0, blend=0)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--final-frames', type=Path, help='Validated complete 14-frame delivery')
    args = parser.parse_args()
    OUT.mkdir(exist_ok=True)
    target = waist_axis(Image.open(OLD / 'Kakashi_00_Idle.png').convert('RGBA'))
    manifest = {'waist_axis_target': target, 'transforms': [], 'preserved_frames': []}
    for name in NAMES:
        if 'Walk' not in name:
            shutil.copyfile(OLD / (name + '.png'), OUT / (name + '.png'))
            manifest['preserved_frames'].append(name)
    for i, source in enumerate(SOURCES, 1):
        frame = Image.open(source).convert('RGBA')
        assert frame.size == (80, 80)
        axis = waist_axis(frame)
        dx = round(target - axis)
        box = frame.getbbox()
        assert box[0] + dx > 0 and box[2] + dx < 80
        aligned = Image.new('RGBA', (80, 80))
        aligned.paste(frame, (dx, 0))
        assert sum(aligned.getchannel('A').getdata()) == sum(frame.getchannel('A').getdata())
        aligned.save(OUT / f'Kakashi_{i:02d}_Walk.png')
        manifest['transforms'].append({'slot': i, 'source': str(source.relative_to(REPO)),
            'source_sha256': hashlib.sha256(source.read_bytes()).hexdigest(),
            'source_waist_axis': axis, 'translate_x': dx, 'translate_y': 0,
            'final_waist_axis': waist_axis(aligned)})
    # Keep OUT as the immutable input for compose_eyes.py. Never overwrite it
    # with restored eyes, or that script could no longer reproduce its edit.
    final_dir = EYES if (EYES.parent / 'DELIVERY.md').exists() else OUT
    if args.final_frames:
        final_dir = args.final_frames.resolve()
    assert len(list(final_dir.glob('*.png'))) == 14
    if final_dir == EYES:
        manifest['eye_restore'] = str(EYES.relative_to(REPO))
    manifest['final_frame_directory'] = str(final_dir.relative_to(REPO))
    frames = [Image.open(final_dir / (name + '.png')).convert('RGBA') for name in NAMES]
    old_walk = [Image.open(OLD / f'Kakashi_{i:02d}_Walk.png').convert('RGBA') for i in range(1, 7)]
    previews, comparison = [], []
    strip = Image.new('RGB', (6 * 240, 640), '#303740')
    for row, bg in enumerate(('#d9d9cf', '#303740')):
        for i, frame in enumerate(frames[1:7]):
            strip.paste(backdrop(frame, 1, bg), (i * 240 + 80, row * 320))
            strip.paste(backdrop(frame, 3, bg), (i * 240, row * 320 + 80))
    for before, after in zip(old_walk, frames[1:7]):
        tile = Image.new('RGB', (320, 240), '#303740')
        tile.paste(backdrop(after, 1, '#d9d9cf'), (0, 80))
        tile.paste(backdrop(after, 3, '#303740'), (80, 0))
        previews.append(tile)
        both = Image.new('RGB', (480, 240), '#303740')
        both.paste(backdrop(before, 3, '#303740'), (0, 0))
        both.paste(backdrop(after, 3, '#303740'), (240, 0))
        comparison.append(both)
    strip.save(ROOT / 'kakashi_walk_strip.png')
    save_animation(previews, ROOT / 'kakashi_walk.gif')
    save_animation(comparison, ROOT / 'kakashi_before_after.gif')
    subprocess.run([sys.executable, str(REPO / 'scripts/build_hires_npc_sheet.py'),
        '--head', '29,16', str(ROOT / 'Kakashi_game.png'), str(ROOT / 'Kakashi_head_check.png')]
        + [str(final_dir / (name + '.png')) for name in NAMES[:12]], check=True)
    manifest['game_sheet'] = 'Kakashi_game.png'
    (ROOT / 'kakashi_integration.json').write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + '\n')


if __name__ == '__main__':
    main()
