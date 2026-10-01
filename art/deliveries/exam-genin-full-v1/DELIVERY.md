status: delivered
request ID: exam-genin-full-v1

# Delivered files

Game frames (all 112×88 PNG, RGBA with only alpha 0/255):

- `RainGenin_Walk_0.png`
- `RainGenin_Walk_1.png`
- `RainGenin_Walk_2.png`
- `RainGenin_Walk_3.png`
- `RainGenin_Jump.png`
- `Candidate_Walk_0.png`
- `Candidate_Walk_1.png`
- `Candidate_Walk_2.png`
- `Candidate_Walk_3.png`
- `Candidate_Jump.png`

Source images (2172×724 PNG, RGBA with transparency and intermediate edge alpha):

- `source/RainGenin_generated_sheet.png`
- `source/Candidate_generated_sheet.png`

Comparison preview: `preview-1x.png` (672×210 PNG, RGB). It places each character's existing Idle sample beside the five new frames at native game scale.

# Prompt summary

Generated two transparent five-pose pixel-art source sheets with the built-in image generation tool, using the corresponding `exam-genin-style-v1` Idle image as the visual reference. Each sheet depicts four right-facing running poses and one jump pose. RainGenin retains the muted gray-brown cloak and umbrella; Candidate retains the dark green outfit and kunai. The resulting game frames use the exact 18-color palette of each character's Idle image.

# Checks performed

- Inspected both generated source sheets and the final native-size preview.
- Checked all ten frames are 112×88, have only alpha values 0/255, and preserve uniform 2×2 pixel blocks.
- Checked each frame's opaque colors are a subset of its existing Idle sample's palette.
- Checked the lowest opaque pixel is on y=83 in every frame. Walk frame top rows match the Idle samples: RainGenin y=4 and Candidate y=22.
- Visually checked right-facing direction, distinct running leg positions, jump silhouettes, and readability at 1× scale.

# Remaining game-side checks

- Import frames into the mod and verify the walk order loops smoothly during actual movement, including the Idle-to-walk and walk-to-jump transitions.
- Check animation speed, apparent character alignment, terrain contact, and umbrella movement in tModLoader. No game-side check was run for this art delivery.
