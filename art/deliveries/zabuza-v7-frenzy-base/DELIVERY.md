# Delivery: zabuza-v7-frenzy-base

- status: delivered
- request ID: `zabuza-v7-frenzy-base`

## Delivered files

All seven PNGs are transparent horizontal source sheets. Each starts with a standard upright idle size-reference pose; action poses follow left to right.

| File | Pose order | Dimensions | Alpha |
|---|---|---:|---|
| `source/Zabuza_FrenzyIdle.png` | Reference, idle 1–4 | 1168 × 336 | Binary: transparent 0, opaque 255 |
| `source/Zabuza_FrenzyRun.png` | Reference, run 1–6 | 1848 × 336 | Binary: transparent 0, opaque 255 |
| `source/Zabuza_FrenzyWindup.png` | Reference, windup 1–3 | 1120 × 336 | Binary: transparent 0, opaque 255 |
| `source/Zabuza_FrenzySlash.png` | Reference, slash start / full forward sweep / recovery | 1072 × 336 | Binary: transparent 0, opaque 255 |
| `source/Zabuza_FrenzySeal.png` | Reference, hand seal 1–3 | 1052 × 336 | Binary: transparent 0, opaque 255 |
| `source/Zabuza_FrenzyLeap.png` | Reference, takeoff / descent | 892 × 336 | Binary: transparent 0, opaque 255 |
| `source/Zabuza_FrenzyDash.png` | Reference, dash 1–2 | 1160 × 336 | Binary: transparent 0, opaque 255 |

## Prompt summary

Created seven separate horizontal sprite sheets of frenzy-phase Momochi Zabuza facing right. The character has an uncovered angular face, sharp teeth, a red eye, black sleeveless clothing and trousers, gray striped wraps, sandals, a side-offset Mist forehead protector, and the broad notched executioner sword. The supplied Zabuza pixel-art sample guided the angular outline and hard-shaded pixel style. Each sheet begins with the same shoulder-sword standing reference pose.

## Checks performed

- Opened and visually inspected the named style samples and Zabuza design references.
- Rejected generated candidates with the wrong character, covered face, or unrelated effects; selected seven action-matched sheets.
- Normalized each sheet to 336 px artwork height using nearest-neighbor pixel sampling at 84 px then 4× enlargement.
- Confirmed every PNG has only alpha values 0 and 255 and no partially transparent edge pixels.
- Confirmed each sheet uses at most 32 opaque RGB colors and its visible pose silhouettes face right, remain separated, and read at the 84 px game-height preview scale.

## Remaining game-side checks

- Compare the size-reference poses beside the approved style sheets in Terraria and tune scale or offsets if needed.
- Verify the near-black clothing remains readable against the encounter background.
- Check frame timing, loop transitions, weapon alignment, and pose readability in-game; these source sheets have not been integrated or tested in Terraria.
