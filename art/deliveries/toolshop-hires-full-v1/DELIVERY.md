# Delivery: toolshop-hires-full-v1

- status: delivered
- request ID: `toolshop-hires-full-v1`

## Delivered files

- `ToolShopkeeper_00_Idle.png` — exact copy of the accepted idle reference.
- `ToolShopkeeper_01_Walk.png` through `ToolShopkeeper_06_Walk.png` — six walking frames.
- `ToolShopkeeper_07_Jump.png` — knees raised, feet off the ground.
- `ToolShopkeeper_08_Sit.png` — sitting and wiping a kunai.
- `ToolShopkeeper_09_Throw.png` through `ToolShopkeeper_11_Throw.png` — wind-up, released kunai, recovery.
- `sheet_preview.png` — twelve frames in one row at 1×, repeated below at 3×.
- `walk.gif` — looping six-frame walk preview.
- `heads_8x.png` — all twelve heads at 8×.
- `ToolShopkeeper_generated_pose_source.png` — imagegen source used for the pose work.

## Dimensions and alpha

- Every game frame is 80×80 RGBA PNG with alpha values only 0 and 255.
- `sheet_preview.png`: 2880×320 RGBA PNG, alpha 0/255.
- `heads_8x.png`: 2784×192 RGBA PNG, alpha 0/255.
- `walk.gif`: 80×80, six frames, with transparency.
- Generated source: 2172×724 RGBA PNG; this high-resolution source has intermediate alpha values and is not a game frame.

## Prompt summary

The built-in image generator used the accepted Tool Shopkeeper idle as the identity reference and the completed Iruka sheet as the pose/style reference. It produced six walk poses, a jump, a seated kunai-cleaning pose, and a three-part kunai throw on transparency. The accepted shopkeeper head was then reused pixel-for-pixel in every 80×80 game frame; body pixels were reduced to the accepted idle palette and binary alpha.

## Checks performed

- Visually inspected the generated source, 1×/3× preview, and 8× head strip.
- Verified idle is byte-for-byte identical to the accepted reference (`cmp`).
- Verified each frame uses only the accepted idle's 20 opaque colours; all twelve head regions are pixel-identical to the accepted head; all frame alpha values are 0/255.
- Bounding boxes stay within 80×80. Standing, walking, sitting, and throwing frames end at y=76; jump ends at y=67.
- Calculated the same four-neighbour/tolerance-24 isolated-pixel metric used by `scripts/pixel_noise.py`: idle 1.90%; walk frames 1.54%, 3.26%, 1.85%, 3.79%, 2.79%, 2.48%; jump 1.83%; sit 3.27%; throw frames 2.35%, 1.42%, 1.52%. Every frame is below 4%.
- `python3 scripts/pixel_noise.py` could not run in this worker environment because Pillow is unavailable; the matching calculation was run directly against decoded RGBA pixels.
- Verified PNG/GIF dimensions with `ffprobe`; GIF contains six frames.

## Remaining game-side checks

- Integrate the 12 frames into the NPC animation and verify origin, feet placement, walk-loop timing, jump, sitting interaction, and thrown-kunai timing in tModLoader.
- Confirm the arm and kunai read well against bright and dark in-game backgrounds at 1×.
