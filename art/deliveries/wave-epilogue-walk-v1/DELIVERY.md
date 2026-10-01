status: delivered
request ID: wave-epilogue-walk-v1

## Delivered files

- `Zabuza_Rise_0.png`–`Zabuza_Rise_2.png`
- `Zabuza_Stagger_0.png`–`Zabuza_Stagger_3.png`
- `Zabuza_Collapse_0.png`–`Zabuza_Collapse_2.png`
- `Haku_Rise_0.png`–`Haku_Rise_2.png`
- `Haku_Stagger_0.png`–`Haku_Stagger_3.png`
- `Haku_Collapse_0.png`–`Haku_Collapse_2.png`
- `source/Zabuza_sheet.png`, `source/Haku_sheet.png`
- `preview.png` (dark/light backgrounds by column)
- `alignment_overlay.png` (all frames overlaid, with center and ground guides)

## Dimensions and alpha

- All ten Zabuza game frames: 288 × 128 RGBA; alpha values only 0 and 255; bottom occupied row y=123.
- All ten Haku game frames: 144 × 96 RGBA; alpha values only 0 and 255; bottom occupied row y=91.
- Source sheets: 1983 × 793 RGBA each, with transparent background. Source alpha may contain intermediate values; game frames have binary alpha.
- Preview: 1530 × 888 RGB. Alignment overlay: 576 × 128 RGB.

## Prompt summary

Built-in image generation, using only the request's existing character and lying frames as references: two right-facing pixel-art pose sheets with three rising frames, four wounded walking frames, and three collapsing frames each. Zabuza is unarmed, bandaged, hunched, with limp arms; Haku's face is visible and his upright poses clutch his chest. Both end prone near their existing lying silhouettes.

## Checks performed

- Inspected both generated source sheets and the final game-scale preview on dark and light backgrounds.
- Removed isolated source-image specks from Haku's first two rising frames.
- Verified twenty expected game frames exist, their exact dimensions, nonempty silhouettes, binary alpha, and common foot baselines by script.
- Inspected `alignment_overlay.png` for overall centering and ground alignment.

## Remaining game-side checks

- Import frames into the mod and check texture switching, facing, animation timing, and movement distance in both boss-death orders.
- Check the handoff from `Collapse_2` to the existing `*_Lying.png` in the actual scene, including any code-side origin or scale adjustments.
- Build and inspect in Terraria; neither build nor in-game acceptance was run by the art worker.
