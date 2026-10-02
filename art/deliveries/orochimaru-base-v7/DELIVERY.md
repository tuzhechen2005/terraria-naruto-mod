status: delivered
request ID: orochimaru-base-v7

## Delivered files

- `art/deliveries/orochimaru-base-v7/Orochimaru_Idle_0.png` — game-size standing sprite, 176×128 RGBA; alpha is strictly 0 or 255.
- `art/deliveries/orochimaru-base-v7/Orochimaru_generated_source.png` — selected built-in imagegen source, 1470×1070 RGBA; contains partial alpha at edges and is not the game-size asset.
- `art/deliveries/orochimaru-base-v7/preview.png` — 1900×950 opaque RGB comparison sheet with the new base, auto-reduced start, source C thumbnail, Zabuza, and Gaara at 1.5×; all are shown at 1× and 4× screen size, followed by 8× face crops of the new base and start.

## Prompt summary

Built-in `image_gen__imagegen` edited the v6 C auto-reduced sprite using source C for identity and details, Tazuna for hard-shaded pixel style, Zabuza for boss pixel density, and v6 `Orochimaru_Idle_C.png` as the face treatment to avoid. The prompt requested a right-facing, full-body, transparent Orochimaru with a narrow pale face, a tiny gold eye beside violet shadow, long blue-black hair, ivory tunic, thick purple rope belt, dark trousers, wraps, sandals, continuous dark outline, and clean 2×2 art-pixel clusters. The selected source was reduced to the target art grid; one excess violet face pixel was replaced with an existing skin color so the eye remains one gold art pixel beside one violet art pixel.

## Checks performed

- Visually inspected all named image references, the generated source, the final 1× sprite, the 4× comparison, and the 8× face crop.
- Confirmed final sprite is 176×128, right-facing, and has opaque bounds x=68–109 and y=24–123; center is x=88 and the lowest opaque row is y=123.
- Confirmed every 2×2 screen-pixel block is uniform, alpha values are exactly 0/255, and the image has 49 colors including transparency.
- Confirmed the opaque silhouette forms one connected component on the art-pixel grid.
- Compared apparent game-scale size and readability beside Zabuza and Gaara in `preview.png`.

## Remaining game-side checks

- Import into tModLoader and confirm placement, facing, transparency, and appearance at actual gameplay zoom.
- Review the base in motion before deriving the boss animation set. Game-side acceptance was not run by the art worker.
