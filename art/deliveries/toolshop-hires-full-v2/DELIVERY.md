# toolshop-hires-full-v2

- status: delivered
- request ID: `toolshop-hires-full-v2`
- Source art: built-in `image_gen__imagegen` generated `ToolShopkeeper_generated_pose_source.png` (2172×724 RGBA, partial alpha), then the selected poses were reduced and corrected against the accepted idle sprite. No API key or fallback CLI was used.

## Delivered files

- `ToolShopkeeper_00_Idle.png`
- `ToolShopkeeper_01_Walk.png`
- `ToolShopkeeper_02_Walk.png`
- `ToolShopkeeper_03_Walk.png`
- `ToolShopkeeper_04_Walk.png`
- `ToolShopkeeper_05_Walk.png`
- `ToolShopkeeper_06_Walk.png`
- `ToolShopkeeper_07_Jump.png`
- `ToolShopkeeper_08_Sit.png`
- `ToolShopkeeper_09_Throw.png`
- `ToolShopkeeper_10_Throw.png`
- `ToolShopkeeper_11_Throw.png`
- `sheet_preview.png` (3840×400 RGBA)
- `walk.gif` (80×80, six frames, transparent)
- `heads_8x.png` (2784×200 RGBA)
- `torso_check.png` (3840×240 RGBA)
- `ToolShopkeeper_generated_pose_source.png` (2172×724 RGBA, generation source)

All twelve game frame PNGs are 80×80 RGBA with alpha values **0 and 255 only**. The three PNG previews also use binary alpha. The generated source has partial alpha and is not a game frame.

## Prompt summary

Generate an even sprite contact sheet using the accepted tool shop owner idle as the identity reference and the approved Iruka and Tazuna sheets as motion references: three-quarter right-facing ninja shopkeeper; black topknot, burgundy mandarin-collar jacket with frog buttons, waist pouch, and one consistent dark knee-length canvas apron with front pocket and kunai handles; six walks, jump, one-head seated kunai-polishing pose, and three throw phases; transparent background and crisp pixel-art intent. The generated poses were reduced to game size, recolored to the accepted idle palette, and corrected with pixels from the accepted head and torso.

## Checks performed

- `ToolShopkeeper_00_Idle.png` is a byte-for-byte pixel copy of the accepted idle image.
- The accepted head crop is pixel identical in all twelve frames, translated down 8 pixels in Jump and Sit.
- All twelve frames have a visible foot pixel at y=76; frame sizes and binary alpha were checked by script.
- Visual inspection of `sheet_preview.png`, `torso_check.png`, and the 80×80 frames: right-facing profile, single head in Sit, consistent central jacket/apron, seated stool and cloth/kunai, walk poses and throw progression.
- Alpha connected-component check: one connected character component in each frame; the released kunai in `10_Throw` is the sole allowed detached component.
- `python3 scripts/pixel_noise.py` (run with the Pillow-enabled `/usr/local/bin/python3.14`):

```text
00 Idle   21 / 1108 = 1.9%
01 Walk   39 / 1169 = 3.3%
02 Walk   26 / 1139 = 2.3%
03 Walk   32 / 1104 = 2.9%
04 Walk   24 / 1075 = 2.2%
05 Walk   33 / 1151 = 2.9%
06 Walk   26 / 1022 = 2.5%
07 Jump   22 /  904 = 2.4%
08 Sit    22 / 1179 = 1.9%
09 Throw  29 / 1196 = 2.4%
10 Throw  30 / 1375 = 2.2%
11 Throw  25 / 1159 = 2.2%
```

## Remaining game-side checks

- Import the twelve frame PNGs into the mod and confirm 1× sprite alignment and animation timing in tModLoader.
- Check walking loop, jump, seated polishing action, and kunai release at game scale and against the game's backgrounds. No build or in-game test was run by the art worker.
