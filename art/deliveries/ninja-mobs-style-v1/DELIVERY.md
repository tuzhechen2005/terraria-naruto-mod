# ninja-mobs-style-v1

status: delivered
request ID: ninja-mobs-style-v1

## Delivered files

| File | Dimensions | Alpha |
| --- | --- | --- |
| `Ronin_Idle.png` | 112×88 | binary 0/255 |
| `Ronin_Slash_0.png` | 112×88 | binary 0/255 |
| `Ronin_Slash_1.png` | 112×88 | binary 0/255 |
| `RogueGenin_Idle.png` | 112×88 | binary 0/255 |
| `RogueGenin_Throw.png` | 112×88 | binary 0/255 |
| `RogueGenin_Slash_1.png` | 112×88 | binary 0/255 |
| `preview-1x.png` | 560×88 | opaque |
| `source/Ronin_generated_strip.png` | 2172×724 | graded transparency, unmodified generated source |
| `source/RogueGenin_generated_strip.png` | 2171×724 | graded transparency, unmodified generated source |

## Prompt summary

Built-in image generation produced a three-pose strip for each enemy using the existing Candidate idle and Tazuna sprite sheet as visual references. The ronin is a brown and dark-blue civilian swordsman with a drawn katana; the rogue genin is a charcoal and muted-purple masked young ninja with a scratched metal forehead protector and kunai. Both use a dark outline, hard color ramps, upper-left light, right-facing poses, and transparent backgrounds.

## Checks performed

- Inspected the generated strips and all six cropped game-size images visually.
- Sampled source strips to the 2×2 art-pixel grid, aligned feet to row 83, hardened final sprite alpha to 0/255, repaired the rogue forehead scratch, and removed an isolated pixel from its close-range pose.
- Confirmed all six game-size sprites are 112×88, transparent, and aligned to the 2×2 grid. The idle visible bounds are 60–62 pixels tall, matching the Candidate idle's 62-pixel height.
- Inspected `preview-1x.png` at actual scale: ronin, rogue genin, Candidate, Rain Genin, and Tazuna, left to right. This preview uses the requested references only.

## Remaining game-side checks

- Load sprites in tModLoader and inspect every animation transition, draw origin, weapon hit timing, and readability against actual biome backgrounds.
- Confirm the throw and slash sprites line up with the game's collision and projectile logic.
