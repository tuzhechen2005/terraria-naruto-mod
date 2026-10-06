# Hiruzen direct pixel idle: generation prompt

Built-in `image_gen__imagegen`, transparent background, with three local reference images:

1. `assets/approved-iruka-6x.png` from the `terraria-npc-pixel-art` skill: style only.
2. `art/reference/hokage_rock_web/Hiruzen_Sarutobi.png`: identity only.
3. `art/deliveries/hiruzen-npc-v2/source/Hiruzen_Idle_Walk.png`: costume and accessory only.

```text
Use case: stylized-concept. Asset type: ONE direct-pixel full-body idle sprite source for a Terraria Naruto town NPC, intended for export to an 80×80 game canvas at about 57 visible pixels high. The attached images have separate roles: Image 1, approved Iruka enlarged sprite, STYLE ONLY: warm broad color masses, clean stepped pixel edges, clear tiny-game silhouette; do not copy his face, scar, eye size, hair, or clothes. Image 2, Hiruzen anime portrait, IDENTITY ONLY: elderly Hiruzen Sarutobi, narrow small horizontal eyes, gray-white short pointed beard, broad white Hokage hat with red brim and red fire symbol. Image 3, existing Hiruzen spritesheet, COSTUME AND ACCESSORY ONLY: white robe with red edging, dark inner collar, long brown smoking pipe, shorter elderly stature; do not copy blur or sheet layout. Generate a SINGLE isolated figure facing screen-right in a relaxed standing 3/4 side view, feet both visible, kind yet authoritative. Entire figure from hat to feet unobstructed, generous transparent margin. The hat and robe must leave the face readable. Make pipe visible at face/hand without obscuring the narrow eyes or beard. Direct coarse pixel art from the outset, not a pixelated illustration. Design around 24–32 rows of broad visual color blocks, warm light upper left, few large shadows, precise stepped hard edges, limited detail. Exposed face has small narrow elderly eyes, no mouth line, no dense wrinkles. No extra characters, no animation frames, no text, no scenery, no floor, no ground shadow, no checkerboard backdrop, no soft glow, no antialiasing or semitransparent halo. Actual transparent background.
```

Export: `export_sprite.cjs --input Hiruzen_Source.png --out <delivery-dir> --prefix Hiruzen_Idle --canvas 80x80 --height 57 --baseline 76 --pixel 1 --phase-x 0.5 --phase-y 0.5 --alpha 128 --facing right`.

The source PNG has semitransparent pixels. The exported game frames harden alpha to 0 or 255 and clear RGB in transparent pixels.
