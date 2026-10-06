#!/usr/bin/env python3
"""Validate retained M11 VFX deliveries and install their RGBA textures."""
from pathlib import Path
import json
import hashlib
import shutil
from PIL import Image

ROOT=Path(__file__).resolve().parents[1]
DEST=ROOT/'ShinobiPrototype/Assets/Vfx/Wave'
GROUPS={
    'wave-water-vfx-v1':{'WaterDragonHead':(192,128),'WaterDragonBody':(256,96),
        'WaterGather':(192,192),'WaterSlash':(256,192),'WaterSplash':(256,128),'MistPuff':(128,128)},
    'wave-ice-vfx-v1':{'IceMirror':(128,192),'MirrorCracks':(128,192),'MirrorShards':(128,128),
        'NeedleGlow':(96,32),'FrostCloud':(128,128),'MirrorHalo':(128,192)},
    'wave-demon-vfx-v1':{'DemonGhost':(256,320),'DemonPress':(256,320),
        'SwordSpinArc':(192,192),'ChakraImpact':(192,192)},
}

def inspect(path,size):
    im=Image.open(path)
    assert im.mode=='RGBA' and im.size==size,(path,im.mode,im.size)
    pixels=list(im.getdata());alphas=[p[3] for p in pixels]
    assert max(alphas)>0 and 0 in alphas and any(0<a<255 for a in alphas),path
    assert all(p[:3]==(0,0,0) for p in pixels if p[3]==0),path
    a=im.getchannel('A');w,h=im.size
    for box in [(0,0,w,1),(0,h-1,w,h),(0,0,1,h),(w-1,0,w,h)]:
        assert a.crop(box).getbbox() is None,(path,'clipped border')
    if path.stem in ('IceMirror','MirrorHalo','FrostCloud','ChakraImpact'):
        assert a.getpixel((w//2,h//2))<30,(path,'center must remain open')
    return {'canvas':list(size),'alpha_bbox':list(a.getbbox()),
        'semi_transparent_pixels':sum(0<x<255 for x in alphas),
        'sha256':hashlib.sha256(path.read_bytes()).hexdigest()}

def main():
    catalog={'spec':'specs/M11_波之国双首领战重设计.spec.md#9',
        'runtime_folder':'Assets/Vfx/Wave','blend':'premultiplied AlphaBlend through tML content loading',
        'textures':[],'short_loops':'art/deliveries/wave-vfx-review-v1',
        'not_a_complete_M11_gameplay_implementation':True}
    checked=[]
    # Validate every delivery first. An invalid asset never leaves a half-installed pack.
    for delivery,items in GROUPS.items():
        for name,size in items.items():
            source=ROOT/'art/deliveries'/delivery/'sprites'/(name+'.png')
            info=inspect(source,size);checked.append((source,name,info,delivery))
            # Runtime draws adjoining water segments from their content bounds,
            # not the larger transparent canvas. Catch stale export geometry.
            if name in ('WaterDragonHead','WaterDragonBody'):
                expected=[27,6,164,122] if name=='WaterDragonHead' else [50,6,205,90]
                assert info['alpha_bbox']==expected,(name,'update WaveVfx source rectangles')
    DEST.mkdir(parents=True,exist_ok=True)
    for source,name,info,delivery in checked:
        shutil.copyfile(source,DEST/(name+'.png'))
        info.update({'name':name,'delivery':delivery,'frames':1,
            'animation':'runtime phase/rotation/scale/opacity; retained source remains unchanged',
            'danger_core':name in ('WaterDragonHead','WaterDragonBody'),
            'hitbox_owner':'attack logic; no damage is created by textures or rendering',
            'anchor':[128,300] if name.startswith('Demon') else
                [84,16] if name=='NeedleGlow' else [info['canvas'][0]/2,info['canvas'][1]/2]})
        catalog['textures'].append(info)
    OUT=ROOT/'art/deliveries/wave-vfx-review-v1';OUT.mkdir(parents=True,exist_ok=True)
    (OUT/'catalog.json').write_text(json.dumps(catalog,indent=2,ensure_ascii=False)+'\n')
    print('16 RGBA textures validated and installed; alpha preserved; borders clear; metadata saved')

if __name__=='__main__':main()
