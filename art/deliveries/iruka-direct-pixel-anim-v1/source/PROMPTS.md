# Built-in image generation prompts

Both calls used `image_gen__imagegen` with the approved `Iruka_B_eye_two_cells.png` as the image reference and `transparent_background: true`.

## Walk source

Six equal, separated, right-facing full-body cells showing a natural six-phase teacher's walking cycle. Match the approved Iruka's ponytail, forehead protector, eye, scar, mouthless face, olive vest, blue clothing, white wraps, brown sandals, chunky warm pixel art and proportions. Alternate legs and arms, keep torso centered and head nearly level. Transparent gaps; no text, props, ground or extra people.

## Action source

Five equal, separated, right-facing full-body cells: knees-tucked jump; seated pose on an invisible chair with horizontal thighs and hanging shins; kunai throw windup; forward arm release; recovery. Match the same approved identity, palette, outfit and pixel style. Keep limbs attached. No actual kunai, chair, text, ground or extra people.

The generated heads were replaced in the final 80×80 frames with the exact approved head pixels. See `../export_frames.py` for the deterministic sampling, anchoring and compositing steps.
