# zabuza-c-frenzy-base delivery

- status: delivered
- request ID: `zabuza-c-frenzy-base`
- tool: built-in `image_gen__imagegen`; seven transparent action sheets were generated, then isolated and quantized to the requested 8×8 pixel grid.

## Delivered files

All files are RGBA PNGs. Alpha values are exclusively 0 (transparent background) or 255 (opaque art); there are no semitransparent pixels. In every sheet the first pose is the same 312×352 pixel idle size reference. Frames follow left to right.

| File | Dimensions | Pose order |
| --- | ---: | --- |
| `source/Zabuza_FrenzyIdle.png` | 1784×400 | Reference idle; breathing 1–4 |
| `source/Zabuza_FrenzyRun.png` | 2328×408 | Reference idle; running 1–6 |
| `source/Zabuza_FrenzyWindup.png` | 1488×472 | Reference idle; windup 1–3 |
| `source/Zabuza_FrenzySlash.png` | 1608×472 | Reference idle; slash start, blade fully right at waist height, recovery |
| `source/Zabuza_FrenzySeal.png` | 1248×400 | Reference idle; sword planted and hand signs 1–3 |
| `source/Zabuza_FrenzyLeap.png` | 1200×440 | Reference idle; takeoff, descending pose |
| `source/Zabuza_FrenzyDash.png` | 1352×400 | Reference idle; low forward rush 1–2 |

## Prompt summary

Right-facing, unmasked berserk Zabuza in the approved Terraria-native C pixel style: spiky hair, Mist forehead protector, charcoal sleeveless outfit, striped guards, sandals, exposed fanged face and red eyes, neck bandage scraps, and executioner sword with the blade hole. Each sheet requested a single row of separated poses, a transparent background, hard pixel edges, and the action sequence above. References used: the named C-style Zabuza sample, the two named Zabuza appearance images, and the named Terraria NPC sample.

## Checks performed

- Visually inspected the generated sheets and final PNGs for identity, right-facing direction, action order, blade placement, separated silhouettes, and readability at the reduced source scale.
- Confirmed all seven requested action counts plus the leading idle reference.
- Verified every PNG has only alpha 0/255 and every 8×8 block has a single RGBA value.
- Verified the leading idle reference has identical 312×352 pixel content in all seven sheets; bottom alignment is consistent within each sheet.

## Remaining game-side checks

- Claude should extract and scale individual frames, then inspect animation timing, hitbox placement, weapon clearance, and readability in Terraria at actual game size.
- In-game visual acceptance has not been run by the art worker.
