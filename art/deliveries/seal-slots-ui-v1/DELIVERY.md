status: delivered
request ID: seal-slots-ui-v1

## Delivered files

- `SealSlotBack.png` — 52×52 RGBA; alpha values 0/230.
- `SealSlotEmpty_2.png` — 32×32 RGBA; alpha values 0/255.
- `SealSlotEmpty_4.png` — 32×32 RGBA; alpha values 0/255.
- `SealSlotEmpty_6.png` — 32×32 RGBA; alpha values 0/255.
- `SealSlotPanel.png` — 72×176 RGBA; alpha values 0/255.
- `preview.png` — 728×396 RGBA, opaque composite showing 1× and 2× arrangements on a dark inventory-style background. It uses the existing `ScrollFireball.png` and `ScrollChidori.png` for the filled slots.
- `source/slot-panel-generated.png` — 1536×1024 RGBA, transparent generated source sheet.
- `source/empty-icons-generated.png` — 2172×724 RGBA, transparent generated source sheet.

## Prompt summary

Generated a crisp, front-facing pixel-art ninja seal slot and parchment scroll panel with dark indigo, vermilion, and gold accents, plus three hand-seal glyphs numbered 2, 4, and 6. Cropped the generated sheets, scaled them to game dimensions with nearest-neighbor sampling, and refined the slot center and tiny numerals for readability.

## Checks performed

- Inspected both generated source images and all game-size assets.
- Verified dimensions and alpha values by reading the final PNG files.
- Reviewed the 1× and 2× composite preview, including the red Fireball and blue Chidori scrolls. Both remain identifiable against the dark slot centers.
- Checked square slot silhouettes, upright scroll direction, isolated transparent edges, and the 2/4/6 glyphs at game scale.

## Remaining game-side checks

- Load the textures in tModLoader and check the three slot positions beside the ammo column at the player's UI scale and resolution.
- Check hover, item placement, and the actual inventory background in game; adjust placement or art only if those checks reveal overlap or poor contrast.
