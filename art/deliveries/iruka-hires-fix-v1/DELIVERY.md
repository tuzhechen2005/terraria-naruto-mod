status: delivered
request ID: iruka-hires-fix-v1

## Delivered files

- `Iruka_10_Throw.png` — 80×80 RGBA game sprite; alpha values only 0 and 255.
- `Iruka_10_Throw_source.png` — 1254×1254 RGBA selected imagegen source; transparent with partial-alpha edge pixels.
- `throw_preview.png` — 960×400 RGBA opaque preview of frames 09, 10, 11 side by side at 1× and 4×.

## Prompt summary

Built-in imagegen used frames 09 and 11 as image references to depict Iruka facing right at the instant of kunai release: right arm extended from the shoulder, open skin-tone hand, detached gray kunai, left arm swung back, forward weight, transparent background and crisp pixel-art styling. The 80×80 sprite was refined using the reference palette and exact reference head pixels.

## Checks performed

- Visually inspected the generated source, 80×80 sprite, and 1×/4× three-frame preview for silhouette, facing direction, shoulder-to-hand continuity, kunai separation, and game-scale readability.
- Confirmed head pixels through row 44 match both reference frames exactly.
- Confirmed sprite is 80×80 with binary alpha (0/255), and the lowest opaque pixel is at y=76.
- `python3 scripts/pixel_noise.py art/deliveries/iruka-hires-fix-v1/Iruka_10_Throw.png`: 1349 opaque pixels, 14 colors, 29 isolated pixels (2.1%, below 4%).

## Remaining game-side checks

- Replace frame 10 in the mod and check the three-frame throw animation in Terraria at native scale, including the kunai release timing and right-facing pose.
