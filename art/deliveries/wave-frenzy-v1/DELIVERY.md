status: delivered
request_id: wave-frenzy-v1

## Delivered files

- `source/Zabuza_frenzy_sprite_sheet_generated.png` — 1448×1086 RGBA generated source sheet; transparent background with some partial-alpha edge pixels.
- `source/Zabuza_frenzy_sprite_sheet_opaque_alpha.png` — 1448×1086 RGBA source-sheet variant with binary alpha (transparent background; all retained character pixels fully opaque).
- `source/ZabuzaThrownSword_generated.png` — 2079×756 RGBA generated sword source; transparent background with some partial-alpha edge pixels.
- `ZabuzaThrownSword.png` — 88×32 RGBA game-size projectile; binary alpha, transparent background, sword points right.
- `preview.png` — 1448×543 opaque preview of the pose sheet over dark and light backgrounds.

The source sheet contains the eleven requested poses in reading order: Throw 0–2, Unarmed 0–5, Catch 0–1. Exact 224×112 character frame PNGs were not generated; the source sheet is provided for frame extraction and alignment. The requested standalone projectile is provided at game size.

## Prompt summary

Generated a transparent, limited-palette pixel-art Zabuza action sheet guided by the named base sprite sheet and the two named pose/design references. The sequence shows the windup, sword throw and empty-handed follow-through; six low running poses with a horizontal kunai and no sword; then reach and catch poses. Separately generated the broad, ring-holed sword as a horizontal right-pointing projectile. Requested crisp dark outlines, opaque character pixels, clear gaps, and no labels or effects.

## Checks performed

- Visually inspected the generated pose sheet and both named references, then inspected the delivery preview over dark and light backgrounds.
- Confirmed the sheet has eleven separated poses in the requested order, consistent overall character scale, right-facing poses, and no visible sword on the unarmed poses.
- Confirmed the running poses visibly carry a kunai in the mouth; the final throw pose has an extended empty hand; the catch sequence ends in an upright sword-holding pose.
- Confirmed generated source dimensions and alpha channels. The opaque-alpha sheet and 88×32 projectile use only alpha values 0 and 255; both retain transparent backgrounds.
- Confirmed the projectile silhouette points right and its blade hole is visible at 88×32.

## Remaining game-side checks

- Extract and align the character poses into 224×112 frames with centerline x=112 and foot baseline y=108; verify 1× readability and foot stability in-game.
- Check throw → unarmed dash → catch animation timing and transitions in Terraria/tModLoader.
- Check projectile rotation and readability in-game.
