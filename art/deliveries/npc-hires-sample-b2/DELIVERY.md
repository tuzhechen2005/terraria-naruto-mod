status: delivered
request ID: npc-hires-sample-b2

## Delivered files

| Path | Dimensions | Alpha |
| --- | ---: | --- |
| `Ibiki_Idle.png` | 80×80 | binary 0/255 |
| `Anko_Idle.png` | 80×80 | binary 0/255 |
| `Hayate_Idle.png` | 80×80 | binary 0/255 |
| `source/Ibiki_source.png` | 1254×1254 | transparent, mixed edge alpha |
| `source/Anko_source.png` | 1268×1241 | transparent, mixed edge alpha |
| `source/Hayate_source.png` | 1254×1254 | transparent, mixed edge alpha |
| `preview.png` | 1440×390 | opaque comparison sheet; six characters at 1× and 3× |
| `faces_8x.png` | 768×256 | 8× head crops with pixel grid and row numbers |

## Prompt summary

Generated each examiner separately with the built-in `image_gen__imagegen` tool. The accepted Tazuna, Iruka, and Kakashi images were strict pixel-art style references. Each character's old NPC sheet was used only for identity, clothing, and colors. Prompts specified the right-facing three-quarter pose, oversized head, hard pixel clusters, upper-left lighting, dark hue-shifted outline, distinctive face and clothing traits, and transparent background. The generated sources were reduced and cleaned into game-size sprites with a 19–20-color target, hard alpha, and isolated-pixel cleanup.

## Checks performed

- Inspected all three generated source images and the finished `preview.png` and `faces_8x.png` visually.
- Confirmed every game sprite is 80×80 with 0/255 alpha, topmost visible pixel at y=16, bottommost visible pixel at y=76, and approximately centered around x=40.
- Confirmed sprites face right and preserve the requested broad silhouettes: Ibiki's dark head wrap and long coat; Anko's short spiky purple ponytail, tan coat, mesh shirt, and orange skirt; Hayate's head wrap, tired face, green vest, and back sword.
- Ran `python3 scripts/pixel_noise.py` on the three game sprites:

  ```text
  Ibiki_Idle.png: 1190 px, 18 colours, 17 isolated (1.4%)
  Anko_Idle.png: 973 px, 19 colours, 32 isolated (3.3%)
  Hayate_Idle.png: 1163 px, 18 colours, 35 isolated (3.0%)
  ```

## Remaining game-side checks

- Claude Code should inspect the 1× sprites in Terraria and zoom into `faces_8x.png` for the specified eye, scar, mouth, dark-circle, and facial-proportion criteria. Ibiki's fine scars and Anko's raised mouth corner are subtle at 1× and may need hand-pixel revision during acceptance.
- Integration into NPC textures, animation behavior, and in-game readability have not been tested. No source code was changed.
