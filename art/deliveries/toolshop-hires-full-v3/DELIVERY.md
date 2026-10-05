status: delivered
request ID: toolshop-hires-full-v3

## Delivered files

- `ToolShopkeeper_01_Walk.png` through `ToolShopkeeper_06_Walk.png`: six 80×80 RGBA game sprites, alpha values 0/255.
- `ToolShopkeeper_08_Sit.png`: 80×80 RGBA game sprite, alpha values 0/255.
- `ToolShopkeeper_10_Throw.png`: 80×80 RGBA game sprite, alpha values 0/255.
- `sheet_preview.png`: 960×800 RGBA, opaque preview of all 12 frames at 1× and 3×. The four previously accepted v2 frames are included unchanged in the preview.
- `walk.gif`: 80×80, six-frame indexed-color animation with transparency.
- `ToolShopkeeper_pose_source.png`: 1774×887 RGBA generated pose-reference sheet, contains partial alpha.

All paths above are relative to `art/deliveries/toolshop-hires-full-v3/`.

## Prompt summary

Built-in `image_gen__imagegen` generated a transparent pose sheet using the accepted shopkeeper, v2 walk/sit/throw sprites, and the Iruka seated reference. It requested six outlined, sleeve-covered walking fists, a seated cloth-and-kunai pose, and a fully extended throwing arm. The game sprites were then constructed from the existing v2 pixels with local pose changes to preserve the established sprite style and exact head pixels.

## Checks performed

- Visually inspected the named project reference frames, the generated pose sheet, and the 1×/3× final preview.
- Verified all eight game PNGs are 80×80 with alpha only 0 or 255, right-facing silhouettes, and bottommost opaque pixel at y=76.
- Verified the walking and throwing heads match the accepted idle head at the same coordinates. The seated head matches the accepted idle head exactly in its head region after a 12-pixel downward shift.
- Ran `scripts/pixel_noise.py` on each game frame: walking frames 2.6%–3.6%, sitting 3.2%, throwing 2.6%, all below 4%.
- Verified one connected opaque component per walking and seated frame. The throwing frame has two: the character and the intentionally airborne kunai.
- Inspected the full animation preview at native and enlarged scale. Confirmed the seated character is supported by the stool and the throwing arm extends toward the separated kunai.

## Remaining game-side checks

- Import and animate the frames in tModLoader; check cadence, sprite registration/hitbox alignment, apron/limb layering, and readability against in-game backgrounds.
- Confirm the seated cloth, kunai, and stool are understandable at native display scale during gameplay.
