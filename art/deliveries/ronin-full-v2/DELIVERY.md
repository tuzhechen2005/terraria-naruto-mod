status: delivered
request ID: ronin-full-v2

## Delivered files

Game-size PNGs (112×88 RGBA, alpha only 0/255):

- `Ronin_Idle.png` — unchanged approved sample
- `Ronin_Slash_0.png` — unchanged approved sample
- `Ronin_Slash_1.png` — unchanged approved sample
- `Ronin_Walk_0.png` through `Ronin_Walk_3.png` — four run frames
- `Ronin_Jump.png` — jump frame

Original built-in image-generation PNGs (1415×1112 RGBA, with intermediate alpha):

- `source/Ronin_Walk_0_imagegen.png` through `source/Ronin_Walk_3_imagegen.png`
- `source/Ronin_Jump_imagegen.png`

Preview: `comparison_3x.png` (3360×264 RGB), left to right: approved Ronin Idle, four new run frames, new jump frame, four Rogue Genin run references. Gray preview backing is not in the game PNGs.

## Prompt summary

The built-in image-generation tool edited the approved Ronin Idle as an image reference for four distinct heavy-jogging poses and one bent-knee jump. Both approved slash frames guided character identity; the Rogue Genin run reference guided motion clarity only. Prompts requested the same right-facing swordsman, low-held sword, dark outline, clean pixel clusters, transparent background, and no orange speckle. The generated poses were brought to 112×88 with nearest-neighbor pixel scaling, colors selected from the approved Idle palette, and the approved Idle upper body and sword retained across the new frames.

## Checks performed

- Visually inspected original references, generated outputs, game-size frames, and the 3× comparison preview.
- All eight game PNGs are 112×88; alpha is strictly 0 or 255 and all follow the 2×2 screen-pixel art grid.
- The three approved samples are byte-for-byte copies of the named references.
- Every opaque RGB color in each **new** frame is in the approved Ronin Idle color set. The copied slash samples contain their own original colors and were intentionally left unchanged.
- Walk frames face right, stay near x=56, and reach bottom row y=83. Jump feet are raised; its alpha bounding box ends at y=73.
- At 3× preview and native game scale, the head, torso and blade stay consistent while the leg positions change across the run frames.

## Remaining game-side checks

- Import the eight game-size PNGs into the mod and inspect the four-frame loop, timing, foot contact, and jump transition inside tModLoader.
- Check sword readability against real game backgrounds and confirm frame alignment/hitbox in motion.
