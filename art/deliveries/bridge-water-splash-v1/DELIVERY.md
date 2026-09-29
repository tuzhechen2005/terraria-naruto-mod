# Delivery: bridge-water-splash-v1

- Status: delivered
- Request ID: `bridge-water-splash-v1`

## Files

- `art/deliveries/bridge-water-splash-v1/WaterImpact.png` — game-size sprite, 32×32 PNG, RGBA with alpha; transparent background pixels and partial edge alpha preserved.
- `art/deliveries/bridge-water-splash-v1/WaterImpact-source.png` — selected generated source, 1347×1167 PNG, RGBA with alpha and transparent background.

## Prompt summary

Generated a direction-neutral, centered radial blue-cyan water impact burst with chunky droplets, bright cyan highlights, deeper blue interior clusters, a clear pixel-art silhouette, transparent background, and no characters, weapons, text, beam, haze, or realistic water. Used the two named Zabuza projectile sprites only as palette and pixel-style references.

## Checks performed

- Inspected the named needle and wave references for cyan-blue palette and sprite style.
- Visually inspected the generated source and selected the radial burst version.
- Confirmed both PNGs retain alpha; confirmed the game sprite is 32×32 and the source is 1347×1167.
- Inspected the 32×32 silhouette enlarged with nearest-neighbor pixels over dark and light backgrounds. The radial droplets and central splash remain readable on both.

## Remaining game-side checks

- Preview in Terraria/tModLoader at native size against actual gameplay scenes.
- Confirm projectile impact timing, origin, and layering after the mod integration is chosen.
