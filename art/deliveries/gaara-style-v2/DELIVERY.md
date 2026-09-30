# gaara-style-v2 delivery

- status: delivered
- request ID: `gaara-style-v2`
- generator: built-in `image_gen__imagegen`

## Delivered files

| File | Dimensions | Alpha |
| --- | ---: | --- |
| `Gaara_Idle.png` | 112×88 | Transparent background; visible pixels fully opaque |
| `Gaara_Cast.png` | 112×88 | Transparent background; visible pixels fully opaque |
| `Gaara_Shield.png` | 112×88 | Transparent background; visible pixels fully opaque |
| `Gaara_Transformed.png` | 176×104 | Transparent background; visible pixels fully opaque |
| `source/Gaara_Idle_generated.png` | 1067×1475 | Transparent background; generated partial edge alpha |
| `source/Gaara_Cast_generated.png` | 1414×1113 | Transparent background; generated partial edge alpha |
| `source/Gaara_Shield_generated.png` | 1414×1113 | Transparent background; generated partial edge alpha |
| `source/Gaara_Transformed_generated.png` | 1414×1113 | Transparent background; generated partial edge alpha |
| `preview.png` | 566×296 | Opaque RGB; light and dark backgrounds at 1× game scale |
| `scale-preview-haku-gaara-v1.png` | 566×144 | Opaque RGB; Haku, Gaara v1, Gaara v2 idle, transformed v2 at 1× game scale |

All paths are relative to `art/deliveries/gaara-style-v2/`.

## Prompt summary

Four separate transparent pixel-art source images were generated in the confirmed Terraria boss style. The prompts specified Chunin Exams Gaara with a black short-sleeve fitted one-piece, white shoulder and hip cloth, diagonal gourd strap and strap-mounted forehead protector, plus the four requested poses. The transformation prompt specified a sand arm continuous with the right shoulder, sand shell across right torso and face, a tanuki ear and ringed tail, navy markings, claws, and a gold star-pupil eye. Generated sources were reduced to a 2×2 screen-pixel art grid with a limited palette. Eye and forehead-marker pixels were reinforced for game-scale readability.

## Checks performed

- Inspected generated source images and final sprites against the named Gaara v1 and Haku v4 previews.
- Viewed all four sprites at native game scale on light and dark backgrounds and reviewed the side-by-side scale preview.
- Confirmed right-facing poses and visible sand-arm connection, tail, ear, gourd, sand stream, and shield silhouette.
- Confirmed exact canvas dimensions, horizontal alignment, and bottom visible pixel at y=83 for ordinary poses and y=99 for transformed.
- Confirmed game-size PNG alpha is only 0 or 255, with no partially transparent character pixels.
- Confirmed all game-size sprite pixels are uniform within their 2×2 art-pixel blocks; each sprite uses 18–20 opaque colors.

## Remaining game-side checks

- Import and render the sprites in tModLoader; verify facing, draw origin, ground contact, scale relative to Haku, and effect layering during the boss fight.
- Verify transformed hitbox and claw/tail reach against the rendered silhouette.
- Game-side checks were not run in this art-worker delivery.
