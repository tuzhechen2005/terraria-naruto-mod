# Orochimaru tween asset delivery

- status: delivered
- request ID: `orochimaru-tween-v1`
- delivered path: `art/deliveries/orochimaru-tween-v1/`

## Delivered files

- `Orochimaru_Walk_0.png` through `Orochimaru_Walk_7.png`: 8 walk frames.
- `Orochimaru_Idle_0.png` through `Orochimaru_Idle_5.png`: 6 breathing frames.
- `Orochimaru_HandsIn_0.png`, `Orochimaru_DashIn_0.png`, `Orochimaru_WindIn_0.png`, `Orochimaru_SealIn_0.png`, `Orochimaru_SummonIn_0.png`, `Orochimaru_NeckIn_0.png`: 6 attack entry frames.
- `preview.png`: each action on a separate row, beginning with the approved idle reference. The attack rows also show the existing first attack frame.
- `strip_idle_walk_idle.png` and `strip_idle_{hands,dash,wind,seal,summon,neck}_idle.png`: playback strips in animation order.
- `source/generated_motion_study.png`: built-in ImageGen pose study. It is reference art, not a game sprite.
- `source/build_frames.py`: deterministic pixel-edit source for the delivered game frames and previews.

## Dimensions and alpha

- All 20 game frames: 112 × 88 PNG, RGBA, alpha values only 0 and 255, every 2 × 2 block uniform.
- Playback strips: 88 px high, RGBA with 0/255 alpha; walk strip 1120 × 88 and six attack strips 448 × 88.
- `preview.png`: 3024 × 2336, opaque RGB preview with checkerboard.
- ImageGen source study: 1774 × 887 RGBA with partial alpha. It is intentionally excluded from game assets.

## Prompt summary and construction

Used the built-in `image_gen__imagegen` tool with the approved `Orochimaru_Idle_0.png` as a character reference. The prompt requested a transparent pixel-art motion study: eight slow walk poses, six subtle breathing poses, and six attack gestures with consistent face, hair, robe, rope, sandals, and right-facing direction. The study was inspected, then the final game frames were constructed by native-grid pixel edits of the existing idle sprite. Attack entry frames also use pixels from the corresponding existing first attack frame. The generated study's colors and proportions were not transferred to the game frames.

## Checks performed

- Visually inspected the approved idle, existing attack frames, Tazuna style comparison, generated study, and final `preview.png` at enlarged pixel scale.
- Verified exactly 20 game frames, each 112 × 88 with binary transparency, a strict 2 × 2 pixel grid, right-facing appearance, and feet ending at row 83 (84 exclusive).
- Verified every opaque game-frame color exists in the approved idle or one of the six existing first attack frames.
- Compared each sequence side by side with the approved standing frame in `preview.png`; reviewed the walk and attack strips for silhouette continuity.
- No game build or in-game playback was run by the art worker.

## Remaining game-side checks

- Integrate the frames into the NPC animation state logic and verify sequence timing, especially the idle-to-dash and attack return transitions, in tModLoader.
- Confirm hitboxes, sprite origin, foot alignment, and direction when the boss turns.
- Check the animation at actual gameplay speed and adjust hold times or individual pixels if any movement still appears abrupt.
