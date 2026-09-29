# Delivery: style-test-v3

- Status: delivered
- Request ID: `style-test-v3`

## Delivered files

| File | Dimensions | Alpha |
|---|---:|---|
| `source/Zabuza_style.png` | 344 × 336 px | RGBA; binary alpha (0/255); transparent background, all visible sprite pixels opaque |
| `source/Haku_style.png` | 168 × 288 px | RGBA; binary alpha (0/255); transparent background, all visible sprite pixels opaque |
| `Zabuza_style_final.png` | 43 × 42 px | RGBA; binary alpha (0/255) |
| `Haku_style_final.png` | 21 × 36 px | RGBA; binary alpha (0/255) |

The source files are nearest-neighbor 8× enlargements of the final game-size pixel art. Every source 8×8 cell is uniform; no antialiasing or semitransparent pixels remain.

## Prompt summary

Generated separate, transparent, full-body Terraria-style character sprites from the named Terraria and character references. Zabuza has spiky dark hair, a metal forehead plate, bandaged lower face, blue-black sleeveless clothes, striped gray guards and the broad shoulder-carried cleaver with a hole. Haku has dark bangs and tied hair with teal pin, a pale mask with bold red markings, teal short kimono and ivory lapels, olive-brown hakama and a belt tail. Both use compact blocky silhouettes, saturated colors, stepped shading and colored dark outlines.

## Checks performed

- Inspected generated images and the final game-size sprites visually.
- Confirmed final dimensions match the requested 42-pixel Zabuza and 36-pixel Haku heights.
- Confirmed alpha contains only 0 and 255 values.
- Confirmed each source file consists of uniform 8×8 color/alpha cells.
- Confirmed both transparent backgrounds and visible silhouettes; Zabuza's sword hole and Haku's red mask markings remain visible at game scale.

## Remaining game-side checks

- Place both sprites beside the original Terraria NPCs in `scene_current_mismatch.png` and review relative scale, readability and style fit in game.
- Verify facing/orientation and sprite alignment in the mod's intended NPC animation frames before integration.
