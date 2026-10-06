# Generation prompts — kakashi-direct-pixel-anim-v1

Built-in `image_gen__imagegen` was used three times with transparent background. Each call used the two approved references: `Kakashi_Idle_h62_Right.png` and `source/kakashi-generated-edited.png` from `kakashi-direct-pixel-v1`.

1. `generated_walk_strip.png`: six equally spaced, right-facing, relaxed walking poses in one row, alternating legs and keeping the exact Kakashi colors, one visible white-and-pupil eye, mask, hair, headband and vest. No grid, text, ground or shadow.
2. `generated_action_strip.png`: five right-facing poses in order: tucked jump, seated orange-book reading, kunai windup, arm-extended release, and recovery. No drawn kunai or chair.
3. `generated_trapped_strip.png`: two floating, curled water-prison poses facing right; bent knees, open dark-blue gloved pushing hand, silver hair and headband cloth drifting upward. No drawn water sphere.

All calls requested hard-edged, transparent pixel-art source sheets suitable for downsampling to a 62-pixel standing body. See `../export_frames.py` for the exact normalization, approved sampling phase, pose crop boundaries and head compositing used for the game-size images.
