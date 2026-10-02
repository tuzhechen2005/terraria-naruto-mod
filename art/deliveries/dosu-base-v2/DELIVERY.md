status: delivered
request ID: dosu-base-v2

## Delivered files

- `Dosu_Idle_0.png` — game-size idle sprite, 112×88 RGBA, binary alpha (0/255); visible bounds x=34–77, y=24–83.
- `source/generated_Dosu_Idle_0.png` — selected built-in ImageGen source, 1415×1112 RGBA with partial alpha.
- `preview.png` — 1792×518 opaque comparison: new Dosu, existing idle, existing drill, and Tazuna; native 1× above nearest-neighbor 4×.

## Prompt summary

Used existing `Dosu_Idle_0.png` and `Dosu_Drill_0.png` as image references for identity and equipment, and the Tazuna comparison image as the style reference. Requested a right-facing, hunched, bandage-wrapped standing sprite with one exposed eye, Sound forehead protector, gray-brown fur collar, and perforated metal sound amplifier. Requested connected dark outlines, broad pixel clusters, and hard upper-left-lit color steps on a transparent background.

## Checks performed

- Inspected the generated source and the final sprite visually against both Dosu frames and Tazuna at native and 4× scale.
- Cropped the generated character, selected a 22×30 logical-pixel rendering, placed it on a 56×44 logical canvas, then doubled pixels to 112×88. The selected frame has 33 RGBA colors and 0/255 alpha only.
- Confirmed facing right, center at x=56, lowest occupied row y=83, and a 2×2 screen-pixel grid.
- Confirmed the game-size silhouette and amplifier hole row remain readable in the comparison preview.

## Remaining game-side checks

- Integrate the idle frame and inspect it at the game's 1.5× display scale during animation and combat.
- Confirm that the new base can be used consistently to derive all Dosu motion and attack frames.
