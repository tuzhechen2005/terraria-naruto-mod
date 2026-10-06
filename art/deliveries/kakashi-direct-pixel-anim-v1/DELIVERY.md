status: delivered

# kakashi-direct-pixel-anim-v1

## Delivered files

- `frames/Kakashi_00_Idle.png` through `frames/Kakashi_11_Throw.png`: twelve 80×80 RGBA right-facing NPC frames.
- `frames/Kakashi_Trapped_0.png`, `frames/Kakashi_Trapped_1.png`: two 80×80 RGBA floating water-prison frames, centered near (40,40), without a water sphere.
- `source/generated_walk_strip.png` (2172×724 RGBA), `source/generated_action_strip.png` (2070×760 RGBA), `source/generated_trapped_strip.png` (1774×887 RGBA): original built-in image generation outputs, copied from `$CODEX_HOME/generated_images`.
- `source/PROMPTS.md`: source prompt summary and reference images.
- `export_frames.py`: repeatable source crop, normalization, sample, alignment, approved head paste and preview export.
- `strip_1x_3x.png` (3360×640 RGB): fourteen frames on light and dark grounds at 1× and 3×.
- `walk.gif` (240×240, six frames, 120 ms/frame): right-facing walk loop at 3×.
- `heads_6x.png` (2856×216 RGB): head comparison for all frames.
- `necks_8x.png` (4032×304 RGB): mask-to-collar and shoulder comparison for all frames.

## Source and export

The approved idle is `../kakashi-direct-pixel-v1/Kakashi_Idle_h62_Right.png`. Frame 00 is a pixel-exact copy shifted 3 px left, placing the vest seam at x=40. The source manifest gives scale `0.06072477962781587`, phase `(0.8, 0.5)`, source box `(362,318,796,1339)` and approved 62-pixel standing height. Generated sheets have different native magnifications; `export_frames.py` normalizes them to that source magnification and samples all frames at the same approved scale and phase. Effective native-sheet sample ratios are 0.115 for walk/actions and 0.080 for trapped poses. Poses are never individually resized to a common height.

Every ground-facing pose reuses the approved 26×29 head/collar patch `(27,14,53,43)`, shifted left to `x=24..49`, with its top row adjusted to the generated body (walk y=14; jump y=17; sit y=25; throws y=14,13,12). The rectangle includes the entire mask lower edge and collar so no generated second jaw or bare neck remains. Action arms intersecting the rectangle were restored outside the approved opaque pixels; the sitting book was retained below the approved head/collar. The two trapped frames share a pixel-identical head region; their hands and legs retain separate generated poses. Only the trapped head is derived from the trapped generation so the upward hair and headband cloth remain visible.

## Checks performed

- Visually inspected original generation sheets, the 1×/3× light-and-dark contact sheet, the 6× heads and 8× necks. The right-facing silhouette, single lazy eye with white and dark pupil, dark mask, olive vest, orange sitting book, open pushing hand and no water sphere remain readable at 1×.
- Verified 14 final PNGs, each 80×80 RGBA, alpha values only 0/255, transparent RGB all zero, and no pixel touches the canvas edge.
- Verified all ground frames end at y=75; jump ends at y=69, six pixels clear of the ground. Trapped bounding boxes are centered around (40,40), ending at y=69 and not touching ground.
- Verified the idle is an exact horizontal translation of the approved 80×80 file; all eleven other ground-pose heads preserve every opaque approved head/collar pixel. Verified six distinct walking frames and connected character silhouettes, including jump and throw arms.
- Checked the mask lower edge and collar in all fourteen panels of `necks_8x.png`; no visible neck gap or second jaw. Checked light and dark backgrounds for stray pixels and transparency halos.

## Remaining game-side checks

Claude should assemble the twelve town NPC frames into `Kakashi.png` and the head texture, place the two trapped frames separately, then verify at true 1× in Terraria: walk loop and horizontal anchoring, chair seat alignment, throw release timing with the kunai projectile, trapped pose inside the water sphere, and actual texture scale. No in-game test or build was performed by this art worker.
