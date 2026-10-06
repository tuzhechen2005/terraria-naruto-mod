# npc-hires-sample-v3a

- status: delivered
- request ID: `npc-hires-sample-v3a`
- Delivered game sprites: `Tazuna_Idle.png`, `Iruka_Idle.png`, `Kakashi_Idle.png`
- Delivered generated source art: `source/Tazuna_Generated.png`, `source/Iruka_Generated.png`, `source/Kakashi_Generated.png`
- Review sheets: `preview.png` (1×, 3×, and height-matched vanilla Guide), `faces_8x.png` (8× heads with pixel grid)
- Reproducible sprite drawing script: `make_sprites.py`

## Dimensions and transparency

| File | Size | Alpha |
| --- | --- | --- |
| `Tazuna_Idle.png` | 80×80 | Transparent; only 0/255 alpha |
| `Iruka_Idle.png` | 80×80 | Transparent; only 0/255 alpha |
| `Kakashi_Idle.png` | 80×80 | Transparent; only 0/255 alpha |
| `source/Tazuna_Generated.png` | 1254×1254 | Transparent RGBA source; includes edge antialiasing |
| `source/Iruka_Generated.png` | 1233×1275 | Transparent RGBA source; includes edge antialiasing |
| `source/Kakashi_Generated.png` | 1246×1263 | Transparent RGBA source; includes edge antialiasing |
| `preview.png` | 1000×455 | Opaque review sheet |
| `faces_8x.png` | 800×290 | Opaque review sheet |

## Prompt summary

Built-in `image_gen__imagegen` generated one transparent full-body source image per character using the corresponding v2b source and the vanilla Guide as visual references. Prompts requested a slim, right-facing three-quarter town-NPC pose, clean square pixel blocks, a dark colored contour, three to four hard value steps, and upper-left light. Character prompts retained Tazuna's round glasses, white beard, and bottle; Iruka's ponytail, clean chin, short nose-bridge scar, and smile; and Kakashi's swept-back silver spikes, diagonal left-eye protector, visible lazy right eye, nose-shaped mask, and orange book. The 80×80 sprites were then drawn at final pixel scale with those sources as the visual guide.

## Checks performed

- Inspected generated sources and final 1×, 3×, and gridded 8× reviews.
- All three sprites: 80×80; visible art y=17–76 (60 pixels tall), feet at y=76; genuine binary transparency; right-facing profiles; shoulders no wider than 26 pixels.
- Face details checked at 8×: two right-shifted eyes on Tazuna and Iruka, nose at right facial edge, Iruka's seven-pixel dark red-brown scar entirely within the face and clean chin, Kakashi's diagonal protector over the rear left eye and exposed half-open right eye.
- `python3 scripts/pixel_noise.py` result:

```text
Tazuna_Idle.png: 1547 px, 16 colours, 16 isolated (1.0%)
Iruka_Idle.png: 1434 px, 15 colours, 20 isolated (1.4%)
Kakashi_Idle.png: 1473 px, 16 colours, 33 isolated (2.2%)
```

## Remaining game-side checks

- Import into the mod and inspect at native 1× scale beside town NPCs during idle and walking movement.
- Confirm sprite origin, collision/hitbox alignment, horizontal flip when walking left, and visibility over varied world backgrounds.
- No tModLoader build or in-game acceptance was run by this art worker.
