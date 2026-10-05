status: delivered
request ID: ninja-mobs-full-v2

## Delivered files

- Ronin: `Ronin_Idle.png`, `Ronin_Slash_0.png`, `Ronin_Slash_1.png`, `Ronin_Walk_0.png`, `Ronin_Walk_1.png`, `Ronin_Walk_2.png`, `Ronin_Walk_3.png`, `Ronin_Jump.png`.
- Rogue genin: `RogueGenin_Idle.png`, `RogueGenin_Throw.png`, `RogueGenin_Slash_0.png`, `RogueGenin_Slash_1.png`, `RogueGenin_Walk_0.png`, `RogueGenin_Walk_1.png`, `RogueGenin_Walk_2.png`, `RogueGenin_Walk_3.png`, `RogueGenin_Jump.png`.
- Generated source images: `source/Ronin_generated_sheet.png` (2172×724 RGBA), `source/RogueGenin_generated_sheet.png` (2204×713 RGBA).
- Pixel-grid construction source: `source/build_frames.py`.
- Previews: `preview_1x.png` (1138×252), `preview_3x.png` (3414×756), `face_grid_check.png` (600×230).

All 17 game frames are 112×88 RGBA PNGs with genuine transparency. Their alpha values are only 0 or 255. The two generated source sheets have an alpha channel with soft-edge values; they are pose references, not game sprites. The opaque preview backgrounds are for inspection.

## Prompt and construction

The built-in image generation tool produced separate transparent strips for the brown-clad, sword-carrying ronin and the uncovered-face, scratched-headband rogue genin. Both prompts requested a consistent right-facing character across the action sequence, Terraria-style hard shading, and a large readable head. The game frames were then placed directly on a 56×44 logical-art-pixel grid and enlarged 2× with nearest-neighbor scaling. Heads, including hair, facial features and rogue headband, were drawn directly at logical-pixel resolution; they were not reduced from the generated images.

## Checks performed

- Confirmed all 17 filenames, 112×88 dimensions, binary alpha, and exact 2×2 screen-pixel blocks.
- Confirmed the non-jump feet stop at or above screen row 83, the game frames face right, and blades and kunai remain inside the canvas.
- Compared the 1× and 3× lineups with the original Guide and Tazuna image references. The preview uses the Guide as a 46-screen-pixel height reference; a separate original-player image was not supplied among the named project references.
- Compared the heads beside the supplied vanilla Guide head on the same art grid at 12× and at 1× game size in `face_grid_check.png`. Ronin and rogue base heads occupy logical rows 19–28 (10 high). The head plus back hair/nose spans logical columns 22–34 (13 wide); the visible face is about 6×6. In both heads the dark pupil is at x=30, y=24–25; eye white is at x=31, y=24–25; the brow is x=30–31, y=22; the nose bump is x=33, y=26; the mouth is x=32, y=28. The rogue's scored metal forehead plate is at y=21–22. The same head drawing is used for all non-jump poses and translated upward four logical pixels for jump.
- Inspected the generated strips, the 1× lineup and the face-grid comparison visually. No hood or face mask is present on the rogue genin frames.

## Remaining game-side checks

- Copy the frame PNGs into the mod, run the mod build, and inspect walking, attacking and jumping in Terraria beside the actual player at game scale. Confirm animation timing, collision alignment, weapon reach and the final height against the original player. Build and in-game checks were not run by the art worker.
