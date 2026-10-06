status: delivered
request ID: iruka-npc-v5

## Delivered files

- `art/deliveries/iruka-npc-v5/source/Iruka_Idle.png` — 224×200 RGBA game-size source, transparent background, alpha values 0/255 only.
- `art/deliveries/iruka-npc-v5/source/generated_Iruka_Idle.png` — 1327×1185 RGBA original built-in image generation output, transparent background with intermediate alpha values.
- `art/deliveries/iruka-npc-v5/in_game_preview.png` — 684×204 RGB-equivalent opaque RGBA comparison, ordered Iruka, Tazuna, tool shopkeeper.

## Prompt summary

Built-in image generation used the tool shopkeeper game texture and Tazuna/tool shopkeeper source sheets as visual references. The prompt specified a single right-facing Iruka idle pose with high upward ponytail, thin horizontal nose-bridge scar, Konoha forehead protector, shaded green flak vest, navy clothing, shin wraps, sandals, and mission scroll. It requested a warm, clean, chunky pixel-art treatment matching the reference NPCs. The selected generated image was redrawn on a 28×25 art-pixel grid to preserve readable details at game scale.

## Checks performed

- Visually inspected the generated original, 224×200 source, and side-by-side game-size preview.
- Confirmed the final source is exactly 8× nearest-neighbor enlargement of its 28×25 pixel grid, with no antialiasing or partial alpha.
- Confirmed upright stance, right-facing profile, separate raised ponytail silhouette, nose-bridge scar above the smile, readable vest pockets and mission scroll, and two-value alpha.
- Preview uses each NPC's first idle frame reduced to 2 pixels per art pixel, scaled 1.36×, then enlarged 3× with nearest-neighbor sampling.

## Remaining game-side checks

- Project owner reviews the idle sample before requesting any walking, talking, sitting, jumping, or throwing frames.
- Developer integrates the approved sample and checks scale, feet placement, facing direction, and appearance against the two reference NPCs in tModLoader. No game build or in-game check was performed in this art-only delivery.
