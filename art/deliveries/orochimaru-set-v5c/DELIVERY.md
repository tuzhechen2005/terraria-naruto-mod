# Orochimaru set v5c delivery

- status: delivered
- request ID: `orochimaru-set-v5c`
- delivery directory: `art/deliveries/orochimaru-set-v5c/`

## Delivered game frames

- `Orochimaru_HandsIn_0.png`, `Orochimaru_Hands_0.png`, `Orochimaru_Hands_1.png`, `Orochimaru_Hands_2.png`
- `Orochimaru_DashIn_0.png`, `Orochimaru_Dash_0.png`, `Orochimaru_Dash_1.png`
- `Orochimaru_WindIn_0.png`, `Orochimaru_Wind_0.png`, `Orochimaru_Wind_1.png`
- `Orochimaru_NeckIn_0.png`, `Orochimaru_Neck_0.png`
- `Orochimaru_SealIn_0.png`, `Orochimaru_Seal_0.png`, `Orochimaru_Seal_1.png`
- `Orochimaru_SummonIn_0.png`, `Orochimaru_Summon_0.png`, `Orochimaru_Summon_1.png`
- `preview.png`: all 18 frames beside the v5a base and Tazuna at 1× and 4×.
- `source/Orochimaru_{Hands,Dash,Wind,Neck,Seal,Summon}_generated.png`: six original built-in imagegen pose sheets, copied from `$CODEX_HOME/generated_images`.
- `source/{Hands,Dash,Wind,Neck,Seal,Summon}_reference_sheet.png`: approved v5b poses and old game pose references used as imagegen inputs.
- `source/build_frames.py`: reproducible frame alignment, head placement, and preview assembly.
- `source/{Hands,Dash,Wind,Neck,Seal,Summon}_4x.png`: individual enlarged comparison strips.
- `source/generated_review.png`: generated pose sheet contact preview.

## Format

All 18 game frames are **112×88 RGBA PNG** with alpha values **0 or 255 only**. Every nontransparent pixel lies on the even-aligned 2×2 art grid. The lowest opaque row is `y=83` in every frame. The generated source sheet dimensions are: Hands 1536×1024; Dash 2172×724; Wind 1989×791; Neck 1774×887; Seal 2172×724; Summon 2172×724. These source sheets have RGBA transparency; game frames use binary alpha.

## Prompt summary and construction

Built-in imagegen received the v5a idle frame as character and face identity reference, the corresponding approved v5b pose group plus old in-game pose examples as a reference sheet, and Tazuna as the pixel-art style reference. Prompts requested transparent right-facing action poses with pale frontal face, two gold snake eyes, purple eye shadow, dark hair behind the face, cream tunic, purple rope, connected outline, hard 3–4 shade ramps, and no effects or background.

The final game sprites retain the approved v5b body poses and their palette, align the eight odd-column v5b frames to the even 2×2 grid, and copy the same 34×34-pixel v5a idle head region into each headed frame. `Neck_0` intentionally omits the head and neck for game assembly with `NeckHead` and `NeckSegment`.

## Checks performed

- Inspected the six generated image sheets and the final 1×/4× contact preview.
- Compared each headed frame with the v5a base at 4×; confirmed both gold eyes remain visible. The headless `Neck_0` is the intentional exception.
- Verified 18 expected filenames, canvas dimensions, binary alpha, exact 2×2 block uniformity, and opaque foot row `y=83`.
- Checked right-facing silhouette and readability in the 1× preview; checked the head, outline and pose joins in the 4× preview.
- Confirmed body colors remain within the v5a idle palette.

## Remaining game-side checks

Not run here: import into the mod, 1.5× in-game rendering, animation timing and transitions, and alignment with separate snakes, wind, purple flame, and neck pieces. The current development assistant should validate these after integration.
