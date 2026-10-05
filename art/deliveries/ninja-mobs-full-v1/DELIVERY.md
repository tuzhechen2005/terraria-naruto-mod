status: delivered
request ID: ninja-mobs-full-v1

# Delivered files

Game frames: 112×88 PNG, RGBA, transparent background, alpha values 0/255 only.

- `Ronin_Idle.png`
- `Ronin_Slash_0.png`
- `Ronin_Slash_1.png`
- `Ronin_Walk_0.png`
- `Ronin_Walk_1.png`
- `Ronin_Walk_2.png`
- `Ronin_Walk_3.png`
- `Ronin_Jump.png`
- `RogueGenin_Idle.png`
- `RogueGenin_Throw.png`
- `RogueGenin_Slash_0.png`
- `RogueGenin_Slash_1.png`
- `RogueGenin_Walk_0.png`
- `RogueGenin_Walk_1.png`
- `RogueGenin_Walk_2.png`
- `RogueGenin_Walk_3.png`
- `RogueGenin_Jump.png`

Generated source images, copied from the built-in image tool's `$CODEX_HOME/generated_images` output:

- `source/Ronin_generated_sheet.png` — 2172×724 PNG, RGBA, graded alpha.
- `source/RogueGenin_generated_sheet.png` — 2192×717 PNG, RGBA, graded alpha.

Comparison image: `preview-1x.png` — 1008×208 PNG, opaque RGB. It shows all delivered frames at native game scale, with the approved Candidate Idle reference in the upper-right tile.

# Prompt summary

Used the built-in `image_gen__imagegen` tool with the approved `ninja-mobs-style-v1` frames as strict image references. The ronin sheet depicts four heavy right-facing run poses and one jump. The rogue genin sheet depicts four low ninja-run poses, a jump, and a kunai-stab anticipation. Kept each character's clothing, weapon, dark outline, upper-left lighting, and limited palette. Both source sheets were requested with transparent backgrounds and no scenery or text.

# Checks performed

- Inspected both generated pose sheets, individual game frames, and the 1× comparison image.
- Sampled each game frame to solid 2×2 screen-pixel blocks; confirmed every frame is 112×88 and uses only alpha 0/255.
- Mapped all frames of each character to one shared palette: 32 opaque colors for the ronin and 34 for the rogue genin, including the rogue's two metal tones.
- Rebuilt the rogue's bright forehead plate on every frame and drew a one-art-pixel diagonal dark scratch across it; checked visibility in the 1× preview.
- Aligned the lowest opaque pixel of every frame to y=83; checked right-facing silhouettes and distinct consecutive run poses.
- Removed an isolated pixel from `RogueGenin_Walk_3`; retained the separate thrown kunai in `RogueGenin_Throw`.

# Remaining game-side checks

- Import and review walk-loop order and speed, Idle-to-walk and walk-to-jump transitions, draw origin, ground contact, and background readability in tModLoader.
- Check slash and throw frame timing against collision and projectile behavior. No game-side validation was run for this delivery.
