status: delivered
request ID: iruka-npc-v4

## Delivered files

| Path | Purpose | Dimensions | Alpha |
| --- | --- | --- | --- |
| `source/Iruka_Idle_Walk.png` | 8× pixel-art source strip | 1568×200, seven 224×200 cells | RGBA, 0/255 only |
| `source/Iruka_Idle_Jump_Sit_Throw.png` | 8× pixel-art source strip | 1344×200, six 224×200 cells | RGBA, 0/255 only |
| `Iruka_Idle_Walk_game.png` | native art-pixel strip | 196×25, seven 28×25 cells | RGBA, 0/255 only |
| `Iruka_Idle_Jump_Sit_Throw_game.png` | native art-pixel strip | 168×25, six 28×25 cells | RGBA, 0/255 only |
| `source/generated_Iruka_Idle_Walk.png` | built-in imagegen pose reference | 2171×724 | RGBA |
| `source/generated_Iruka_Idle_Jump_Sit_Throw.png` | built-in imagegen pose reference | 2172×724 | RGBA |
| `face_grid_check.png` | Guide and Iruka heads on a 12× art-pixel grid | 280×140 | RGBA |
| `face_game_scale_check.png` | Guide and Iruka heads at 1 art pixel per screen pixel | 34×14 | RGBA |
| `Tazuna_ToolShop_Iruka_comparison.png` | idle figures side by side; 4× top row, 1× bottom row | 336×131 | RGBA |
| `build.py` | reproducible crop, palette, pixel finishing and preview script | — | — |

## Pose order

- `Iruka_Idle_Walk`: Idle, Walk 1–6.
- `Iruka_Idle_Jump_Sit_Throw`: Idle, Jump, Sit with scrolls, Throw preparation, Throw release with rightward kunai, Throw recovery.

## Prompt and art construction

Built-in `image_gen__imagegen` generated two transparent Iruka pose references using the named Tazuna, tool-shop and vanilla-head images. The prompt specified a right-facing, smiling teacher with pointed black ponytail, Leaf forehead protector, green pocketed chunin vest, dark navy clothes, white leg wraps, blue sandals and archive scrolls, in the requested seven and six poses. The generated bodies were cropped, sampled to the art grid, palette-limited and finished pixel by pixel. The generated faces were removed. Every final head, hair, protector and face pixel was drawn directly on the art grid.

The main head is **10×10 art pixels** (`x=11–20`, `y=0–9` within each 28×25 cell), with a small ponytail projection behind it. The skin field is 8×5 pixels. The two eyes use pupil columns `x=14,17` and adjacent white columns `x=15,18`, each at `y=6–7`; brows are at `y=5`. The nose projects to `x=21,y=8`. The bridge scar is one pixel high at `x=15–18,y=8`, in a skin-adjacent shade. The smile mark is at `x=18,y=9`.

## Checks performed

- Inspected both generated reference strips and the final strips, enlarged face grid, native-scale face comparison and Tazuna/tool-shop comparison.
- Verified the two final source strips are exactly nearest-neighbor 8× enlargements of the native art-pixel strips; every 8×8 block is solid.
- Verified all final source and game strip alpha values are exactly 0 or 255.
- Verified all frame cells are 28×25 art pixels, the figure faces right, no neighboring cells touch, all visible frames end on art-pixel row 22, and the two idle frames are identical.
- Verified the 4× and 1× comparison rows contain Tazuna, tool-shop owner and Iruka in that order.

## Remaining game-side checks

- Integrate the selected strip into the mod's NPC animation and inspect eyes, scar, ponytail, scroll, kunai and walk cycle in Terraria beside Tazuna and the tool-shop owner. Asset inspection is not in-game acceptance.
