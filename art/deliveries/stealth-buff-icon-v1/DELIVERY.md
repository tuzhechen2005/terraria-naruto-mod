status: delivered
request ID: stealth-buff-icon-v1

## Delivered files

- `StealthBuff.png` — 32×32 RGBA game icon; alpha values are only 0 and 255.
- `StealthBuff_source.png` — 1293×1216 RGBA image generated with the built-in image tool; contains transparency and intermediate alpha.
- `preview.png` — 384×180 opaque comparison on dark and light backgrounds, showing the icon at 1× and 4×.

## Prompt summary

Deep purple-gray smoke around a silver kunai tip angled up and right, in a compact Terraria-style pixel-art buff icon with a dark outline and transparent background.

## Checks performed

- Inspected the generated source and the 1×/4× preview visually. The kunai points up and right; smoke surrounds its lower portion. The icon remains distinct on dark and light backgrounds at 32×32.
- Checked the game icon is 32×32 RGBA, has only binary alpha, and consists entirely of uniform 2×2 pixel blocks.
- Looked for the named `Buff_10` and `Buff_115` reference files in the repository; neither is present. The preview therefore contains the new icon against two backgrounds, without original-icon side-by-side images.

## Remaining game-side checks

- Place `StealthBuff.png` in `ShinobiPrototype/Content/Buffs/`, build the mod, and inspect the icon in the actual buff UI.
- Compare it with Terraria `Buff_10` and `Buff_115` in game for scale and distinction.
