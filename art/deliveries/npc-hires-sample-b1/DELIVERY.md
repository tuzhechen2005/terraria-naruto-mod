# npc-hires-sample-b1

- status: delivered
- request ID: `npc-hires-sample-b1`
- generation: built-in `image_gen__imagegen` with transparent background and the three accepted sprites as pixel-art references; each old NPC texture was used only for character design and color.

## Delivered files

| File | Dimensions | Alpha |
| --- | ---: | --- |
| `ToolShopkeeper_Idle.png` | 80×80 | binary 0/255 |
| `Hiruzen_Idle.png` | 80×80 | binary 0/255 |
| `HakuForest_Idle.png` | 80×80 | binary 0/255 |
| `source/ToolShopkeeper_source.png` | 1254×1254 | 256 levels |
| `source/Hiruzen_source.png` | 1149×1369 | 256 levels |
| `source/HakuForest_source.png` | 1230×1278 | 256 levels |
| `preview.png` | 1524×403 | opaque |
| `faces_8x.png` | 1080×300 | opaque |

The generated source images retain their original soft alpha and faint low-alpha edge noise. The three game-size PNGs have hard pixel edges and only transparent or opaque pixels.

## Prompt summary

- ToolShopkeeper: middle-aged, kind but shrewd; round black topknot, trimmed moustache and chin beard, burgundy Chinese jacket, canvas apron with kunai, dark trousers and leg wraps.
- Hiruzen: elderly Third Hokage; white/red hat with 火, white hair and neck drape, white/red robe, grey-white goatee, pipe, fine age marks.
- HakuForest: unmasked, gently feminine-looking boy; long black hair and two face-framing strands, light pink kimono top, pale trousers, herb basket.
- All: standing right-facing three-quarter view, large head, hard color steps, upper-left light, dark hue-shifted outlines, transparent background.

## Checks performed

- Visually inspected all three generated sources, final game-size sprites, a six-character 1×/3× comparison against the accepted sprites, and an 8× face grid with row numbers.
- All final sprites: visible bounding box height 61 pixels, top y=16 and feet at y=76; centered near x=40; exactly 20 opaque RGB colors; alpha values only 0 and 255.
- `python3 scripts/pixel_noise.py` results: `ToolShopkeeper_Idle.png` 1108 opaque pixels, 21 isolated (1.9%); `Hiruzen_Idle.png` 1276 opaque pixels, 43 isolated (3.4%); `HakuForest_Idle.png` 1260 opaque pixels, 40 isolated (3.2%). All are below the 4% request threshold.
- Checked full silhouettes, character-facing direction, signature props, and face legibility in the 1× and 8× previews.

## Remaining game-side checks

- Claude Code should inspect individual face pixels and decide whether these samples meet the requested likeness standard.
- Integrate the selected sprites, build the mod, and inspect in-game scale, lighting, animation/frame behavior, and NPC recognition. No build or in-game acceptance was performed by this art worker.
