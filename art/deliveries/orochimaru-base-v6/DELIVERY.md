status: delivered
request ID: orochimaru-base-v6

## Delivered files

| File | Dimensions | Alpha |
| --- | ---: | --- |
| `Orochimaru_Idle_A.png` | 112×88 | RGBA, 0/255 only |
| `Orochimaru_Idle_B.png` | 112×88 | RGBA, 0/255 only |
| `Orochimaru_Idle_C.png` | 112×88 | RGBA, 0/255 only |
| `source/Orochimaru_source_A.png` | 1171×1343 | RGBA, soft source alpha |
| `source/Orochimaru_source_B.png` | 1171×1343 | RGBA, soft source alpha |
| `source/Orochimaru_source_C.png` | 1171×1343 | RGBA, soft source alpha |
| `preview.png` | 2912×596 | RGBA, opaque comparison sheet |
| `preview_1x.png` | 728×116 | RGBA, opaque comparison sheet |
| `preview_4x.png` | 2912×464 | RGBA, opaque comparison sheet |
| `face_A_8x.png`, `face_B_8x.png`, `face_C_8x.png` | 288×208 each | RGBA, transparent |

Source files, the reproducible sizing and pixel cleanup script, and the external reference URL list are under `source/`. The image files were copied from the built-in image generator's `generated_images` directory.

## Prompt summary

Created a tall, slim Forest of Death Orochimaru facing right in three-quarter view, with pale narrow face, golden slit-pupil eyes, violet eye shadow, parted black hair in distinct front and back strands, ivory tunic, large purple rope knot, dark trousers and sandals. The pose has one lowered hand and one raised hand. B slightly enlarges the face; C makes the eyes and shadow colder. All three game sprites share the same body below the neck. The existing Tazuna and Gaara sprites guided rendering and scale; the old Orochimaru sprite was supplied as a failure reference.

## Checks performed

- Visually inspected the three generated source images, the 1× and 4× comparison against v5, Gaara and Neji, and all three 8× face crops.
- Confirmed 112×88 dimensions, exact binary alpha, 2×2 screen-pixel blocks, feet on row 83, midline approximately x=56, and right-facing silhouettes.
- Removed isolated art pixels and retouched the face at the art-pixel grid. Confirmed B/C body pixels below y=34 are identical to A.
- Searched for four Part 1 Forest of Death image references; their URLs are in `source/reference/SOURCES.md`. Direct saving of the external images failed because the sandbox could not resolve the source hosts. Project files outside this delivery were not modified.

## Remaining game-side checks

- Have the owner select A, B or C, then review the selected sprite in tModLoader at 1.5× scale against the actual arena/background.
- Verify the selected sprite's eye and hair readability in motion and adapt the full boss animation set from that selection.
