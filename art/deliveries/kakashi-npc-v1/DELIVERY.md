status: delivered
request ID: kakashi-npc-v1

# Delivered files

- `source/Kakashi_Idle_Walk.png` — 1568 × 200 RGBA, 7 frames from left to right: Idle reference, Walk 1, Walk 2, Walk 3, Walk 4, Walk 5, Walk 6.
- `source/Kakashi_Idle_Jump_Sit_Throw.png` — 1344 × 200 RGBA, 6 frames from left to right: Idle reference, Jump, Sit with orange book, Throw preparation, Throw release, Throw recovery.

Both sheets have 28 × 25 art-pixel cells at 8 × 8 screen pixels per art pixel. The standing references are 23 art pixels high. Frames are separated, face right, and share a foot baseline. Background alpha is 0; every visible sprite pixel has alpha 255. There are no partial-alpha pixels.

# Prompt summary

Built-in `image_gen__imagegen` produced two transparent Kakashi NPC action sheets in chunky Terraria-like pixel art. The prompts specified silver spiky hair, a slanted forehead protector covering one eye, a navy lower-face mask, green jonin vest, navy clothes and sandals, white shin wraps, and a small tool pouch. The second sheet also specified the orange reading book and kunai throw stages. The generated art was sampled into exact 8 × 8 blocks and alpha was made binary to meet the requested source format.

# Checks performed

- Inspected the named Zabuza, Haku, and vanilla Terraria NPC references before generation.
- Inspected the generated images and both final sheets on dark and light backgrounds at game scale.
- Verified both PNG dimensions, RGBA format, binary alpha, and identical color within each 8 × 8 screen-pixel block.
- Verified the standing reference height, frame separation, right-facing direction, and shared foot baseline.

# Remaining game-side checks

- Import and run the project's frame-processing script; review animation timing and walk-cycle transitions in tModLoader.
- Check dialogue, sitting, jumping, and kunai throw alignment in game. No in-game acceptance was performed by the art worker.
