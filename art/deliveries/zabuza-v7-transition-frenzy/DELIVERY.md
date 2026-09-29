# Delivery: zabuza-v7-transition-frenzy

- status: delivered
- request ID: `zabuza-v7-transition-frenzy`

## Delivered files and pose order

All character sheets are transparent PNGs at **2172×724**. Every sheet places the standard idle scale reference first at the far left; remaining poses are listed left to right. All character artwork faces right.

- [`source/Zabuza_Kneel.png`](source/Zabuza_Kneel.png) — idle scale reference; Kneel 1 (bowed, one knee down, sword planted); Kneel 2 (settled one-knee pose).
- [`source/Zabuza_Roar.png`](source/Zabuza_Roar.png) — idle scale reference; Roar 1 (rising); Roar 2 (upright, head back); Roar 3 (peak roar, arms spread). Transition poses retain face bandages.
- [`source/Zabuza_FrenzyRoar.png`](source/Zabuza_FrenzyRoar.png) — idle scale reference; FrenzyRoar 1–3. Berserker poses have the full face exposed, red eye and sharp teeth; torn bandage pieces trail at the neck.
- [`source/Zabuza_Throw.png`](source/Zabuza_Throw.png) — idle scale reference; Throw 1 (wind-up); Throw 2 (swing); Throw 3 (release, sword clear of hands and arms extended).
- [`source/Zabuza_Unarmed.png`](source/Zabuza_Unarmed.png) — idle scale reference; Unarmed run cycle 1–6. Berserker has exposed face, a kunai visibly held in the teeth, and no carried sword.
- [`source/Zabuza_Catch.png`](source/Zabuza_Catch.png) — idle scale reference; Catch 1 (reaching for the returning sword); Catch 2 (recovered standing with sword over shoulder). Both catch poses have the face exposed.
- [`source/ZabuzaThrownSword.png`](source/ZabuzaThrownSword.png) — standalone horizontal sword projectile, tip facing right; **352×128** (4× the requested approximate 88×32 game size).

Alpha status: all seven PNGs have an alpha channel. The alpha plane was thresholded to binary transparency; FFmpeg signal checks report no pixels with intermediate alpha values (all alpha is 0 or 255). Character RGB artwork was preserved during the alpha cleanup. The sword was resized with nearest-neighbor sampling to 352×128.

## Prompt summary

Generated Zabuza transition and berserk action sheets plus a standalone executioner-sword projectile. Prompts followed the specified Zabuza/Haku pixel-sprite samples: thick dark outlines, angular shapes, limited palette, hard cel-shaded tones, black clothing, transparent background, and a consistent leftmost idle scale reference. The berserk poses specify a fully visible face and the running cycle specifies a clearly visible mouth-held kunai.

## Checks performed

- Opened and inspected every project reference named in the request.
- Visually reviewed each generated sheet and the final projectile; checked figure separation, right-facing direction, black clothing, pose readability, exposed berserk face, mouth-held kunai, and sword orientation.
- Checked PNG dimensions and alpha channel presence with `sips`.
- Checked alpha min/max and tested for intermediate alpha values with FFmpeg; intermediate-alpha pixel count is zero for every delivered image.
- Confirmed the projectile is 352×128 after nearest-neighbor resizing.

## Remaining game-side checks

- Use `scripts/pixelize_frames.py` to extract and normalize the source poses against each sheet's idle scale reference; compare the resulting character scale side by side with batch 1.
- Review individual frames at game resolution for silhouette, edge pixels, face and kunai readability, and animation continuity.
- Verify all action frame timing and sword attachment/release/catch alignment in tModLoader. No in-game checks were run for this delivery.
