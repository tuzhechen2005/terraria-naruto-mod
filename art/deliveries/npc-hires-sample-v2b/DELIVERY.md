# npc-hires-sample-v2b

- status: delivered
- request ID: `npc-hires-sample-v2b`
- Method: built-in `image_gen__imagegen` for three transparent source concepts; game-size sprites drawn and checked on the 80×80 pixel grid from those concepts. No image API key or fallback CLI used.

## Delivered files

| File | Dimensions | Alpha |
| --- | ---: | --- |
| `Tazuna_Idle.png` | 80×80 | Binary, 0/255 |
| `Iruka_Idle.png` | 80×80 | Binary, 0/255 |
| `Kakashi_Idle.png` | 80×80 | Binary, 0/255 |
| `source/Tazuna_Generated.png` | 1227×1282 | Transparent source, alpha 0–255 |
| `source/Iruka_Generated.png` | 1222×1287 | Transparent source, alpha 0–255 |
| `source/Kakashi_Generated.png` | 1246×1262 | Transparent source, alpha 0–255 |
| `preview.png` | 1000×460 | Opaque review sheet |
| `faces_8x.png` | 800×290 | Opaque review sheet |
| `draw_sprites.py` | — | Reproducible sprite and review sheet drawing script |

All paths above are relative to `art/deliveries/npc-hires-sample-v2b/`.

## Prompt summary

- Tazuna: stocky elderly bridge builder, fluffy white hair, thin round glasses with visible eyes, beard, towel, vest, loose trousers and sake bottle.
- Iruka: friendly teacher, high black ponytail, straight lit forehead plate, thin horizontal nose scar, green pocketed vest, scroll and ninja clothes.
- Kakashi: swept silver spikes, diagonal lit forehead plate over the left eye, exposed half-open right eye, navy mask, green vest and orange book.
- All source prompts requested one right-facing full-body pixel-art character, transparent background, clean clusters and flat shades.

## Pixel measurements and checks

| Character | Height excluding raised hair | Head height | Face width | Foot line | Colours | Isolated pixels |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Tazuna | 60 px | 24 px | ~17 px | y=76 | 18 | 27/1810 (1.5%) |
| Iruka | 60 px | 23 px | ~17 px | y=76 | 16 | 20/1581 (1.3%) |
| Kakashi | 61 px | 24 px | ~17 px | y=76 | 14 | 33/1684 (2.0%) |

The raised ponytail and silver spikes reach y=11; the ordinary head and body remain within the requested 60–62 px height. All three silhouettes are centred near x=40 and use right-facing three-quarter poses. Examined the three generated source images, exact-size preview, and 8× face crops. Checked the sprite dimensions, alpha values, colour counts and bounds with Pillow. Ran the request's `python3 scripts/pixel_noise.py` on all three game-size images; output:

```text
art/deliveries/npc-hires-sample-v2b/Tazuna_Idle.png: 1810 px, 18 colours, 27 isolated (1.5%)
art/deliveries/npc-hires-sample-v2b/Iruka_Idle.png: 1581 px, 16 colours, 20 isolated (1.3%)
art/deliveries/npc-hires-sample-v2b/Kakashi_Idle.png: 1684 px, 14 colours, 33 isolated (2.0%)
```

## Remaining game-side checks

- Integrate the three Idle images into the mod and inspect their silhouettes and facial recognition at actual 1× game scale.
- Check Terraria lighting, backgrounds, NPC draw direction and alignment in-game. No game-side build or in-game acceptance was run by the art worker.
