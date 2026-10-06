#!/usr/bin/env python3
"""Review M11 generated layers at native size and in illustrative compositions.

Only exports/composes retained textures. Does not draw new raster VFX, alter
gameplay or claim screenshots of Terraria. GIF has one shared palette; APNG
preserves soft alpha and colours without GIF palette loss.
"""
from pathlib import Path
import math
import json
from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parents[1]
ASSETS = ROOT / 'ShinobiPrototype/Assets/Vfx/Wave'
OUT = ROOT / 'art/deliveries/wave-vfx-review-v1'
GROUPS = {
    'water': ['WaterDragonHead', 'WaterDragonBody', 'WaterGather', 'WaterSlash', 'WaterSplash', 'MistPuff'],
    'ice': ['IceMirror', 'MirrorCracks', 'MirrorShards', 'NeedleGlow', 'FrostCloud', 'MirrorHalo'],
    'demon': ['DemonGhost', 'DemonPress', 'SwordSpinArc', 'ChakraImpact'],
}

def asset(name):
    return Image.open(ASSETS / (name + '.png')).convert('RGBA')

def place(board, art, xy, size=None, opacity=1, rotation=0, flip=False):
    if isinstance(art, str):
        art = asset(art)
    else:
        art = art.copy()
    if size:
        art = art.resize(tuple(max(1, round(n)) for n in size), Image.Resampling.LANCZOS)
    if flip:
        art = art.transpose(Image.Transpose.FLIP_LEFT_RIGHT)
    if rotation:
        art = art.rotate(rotation, Image.Resampling.BICUBIC, expand=True)
    if opacity < 1:
        art.putalpha(art.getchannel('A').point(lambda a: round(a * max(0, opacity))))
    board.alpha_composite(art, (round(xy[0]-art.width/2), round(xy[1]-art.height/2)))

def save_loop(frames, stem, ms=100):
    frames = [f.convert('RGB') for f in frames]
    montage = Image.new('RGB', (frames[0].width, frames[0].height * len(frames)))
    for i, im in enumerate(frames):
        montage.paste(im, (0, i * im.height))
    palette = montage.quantize(colors=256, method=Image.Quantize.MEDIANCUT)
    indexed = [im.quantize(palette=palette, dither=Image.Dither.NONE) for im in frames]
    indexed[0].save(OUT/(stem+'.gif'), save_all=True, append_images=indexed[1:],
        duration=ms, loop=0, optimize=False, disposal=2)
    frames[0].save(OUT/(stem+'.png'), save_all=True, append_images=frames[1:],
        duration=ms, loop=0, disposal=0, blend=0)

def backdrop(color):
    board=Image.new('RGBA',(960,480),color)
    d=ImageDraw.Draw(board)
    d.text((18,12),'M11 VFX COMPOSITION - illustrative, not in-game footage',fill='#b8c9d8')
    for y, x0, x1 in ((420,20,930),(335,220,490),(300,600,905)):
        d.rectangle((x0,y,x1,y+5),fill='#657778')
    # A simple 42px player-height scale marker, not a generated game sprite.
    d.rectangle((456,377,475,418),outline='#e8d29f',width=2)
    d.text((434,438),'42px reference',fill='#dacbaa')
    return board

def actors(board, demon=False, press=False):
    for name, pos, height in [('Zabuza',(115,349),126),('Haku',(825,271),105)]:
        art=Image.open(ROOT/f'ShinobiPrototype/Content/NPCs/{name}_Idle_0.png').convert('RGBA')
        box=art.getbbox();art=art.crop(box)
        art=art.resize((round(art.width*height/art.height),height),Image.Resampling.NEAREST)
        board.alpha_composite(art,(round(pos[0]-art.width/2),round(pos[1]-art.height)))

def main():
    OUT.mkdir(parents=True,exist_ok=True)
    metadata=[]
    for group,names in GROUPS.items():
        for bg,color in [('dark','#142331'),('light','#d0dce1')]:
            board=Image.new('RGBA',(900,720 if len(names)>4 else 780),color)
            d=ImageDraw.Draw(board)
            for i,name in enumerate(names):
                art=asset(name);col,row=i%3,i//3
                x,y=col*300,row*(360 if len(names)>4 else 390)
                d.text((x+12,y+10),name,fill='#d4e6f2' if bg=='dark' else '#233542')
                place(board,art,(x+150,y+195)) # actual asset size, no thumbnail
            board.convert('RGB').save(OUT/f'{group}-{bg}-1x.png')
        metadata.extend(names)
    frames=[]
    for k in range(24):
        board=backdrop('#162b37');p=(k%8)/7;stage=k//8
        if stage==0:
            actors(board)
            place(board,'WaterGather',(170,262),(150,150),.35+.5*p,rotation=-p*180)
            d=ImageDraw.Draw(board);d.line((240,295,748,295),fill='#6cddd5',width=2)
            d.line((240,350,748,350),fill='#6cddd5',width=2)
            d.text((240,365),'locked water route; phase 1',fill='#73e5d7')
        elif stage==1:
            actors(board)
            x=285+400*p
            for n in range(3):
                # Crop retained transparent padding so adjoining flow occupies
                # the actual segment span, with a small overlap at both joins.
                body=asset('WaterDragonBody');body=body.crop(body.getbbox())
                place(board,body,(x-100-n*95,322),(130,58),.92-n*.12)
            place(board,'WaterDragonHead',(x,319),(192,128))
            place(board,'WaterSplash',(x-100,372),(160+100*p,80+30*p),1-p)
            # The second step marks only a portion of landings, leaving a lane.
            for nx in (635,710):
                place(board,'NeedleGlow',(nx,243),(65,22),.4+.5*p,rotation=-90)
            ImageDraw.Draw(board).text((485,185),'separate ice warning; safe lane remains',fill='#ceeefa')
        else:
            for n,(x,y) in enumerate([(600,150),(790,130),(895,285)]):
                place(board,'IceMirror',(x,y),(85,127),1-p*.3)
                if n==0:
                    place(board,'MirrorHalo',(x,y),(64,131),.85)
                place(board,'MirrorCracks',(x,y),(64,117),p)
                if p>.55:place(board,'MirrorShards',(x,y),(100*p,100*p),(p-.55)*2)
            place(board,'DemonPress' if p>.5 else 'DemonGhost',(126,210),(205,256),.6)
            actors(board)
            place(board,'SwordSpinArc',(280,230),(150,150),.8,rotation=-p*360)
            place(board,'ChakraImpact',(170,275),(110+90*p,110+90*p),1-p)
            # Example ring uses 16 logical slots, skipping the same two slots.
            for i in range(16):
                if i in (3,4):continue
                a=i*math.tau/16;x=470+math.cos(a)*117;y=288+math.sin(a)*105
                place(board,'NeedleGlow',(x,y),(40,13),.4+.6*p,rotation=180-math.degrees(a))
            ImageDraw.Draw(board).text((340,147),'needle array: 16 slots, 2-slot gap',fill='#e2f4ff')
        frames.append(board)
    save_loop(frames,'combined-loop')
    frames[13].convert('RGB').save(OUT/'water-composition.png')
    frames[22].convert('RGB').save(OUT/'frenzy-composition.png')
    (OUT/'preview.json').write_text(json.dumps({'assets':metadata,'animation_frames':24,
        'frame_ms':100,'native_sheets':'*-1x.png','scene':'Illustrative, not in-game footage',
        'actor_height_reference':{'Zabuza':126,'Haku':105,'player_marker':42},
        'no_gameplay_or_collision_changed_by_preview':True},indent=2)+'\n')

if __name__=='__main__':main()
