# 再不斩 M5 美术提示词

使用内置 imagegen 编辑已有像素风角色图。最终选用提示词：

## 跑步过渡帧 `run-mid-source.png`

> Use case: stylized-concept. Asset type: MID-STRIDE PASSING FRAME for a 2D side-scrolling game run cycle, NOT a long-stride hero pose. Input image is character/style reference. Same masked swordsman, same black spiky hair, diagonal headband, white mask, sleeveless charcoal tunic, grey forearm/shin guards, giant cleaver with round hole held behind shoulder, facing RIGHT. Draw one full-body frame with BOTH FEET CLOSE TOGETHER DIRECTLY BELOW HIPS, one knee lifted high, the other foot planted, compact narrow silhouette as legs pass each other. Upper body leaning forward with recognizable same face and sword. Crisp pixel art of same style and proportions, completely visible on transparent canvas, baseline at bottom. This must visibly differ from the reference's wide split legs. No extra characters/background/text/shadow.

参考图：`art/zabuza-v2/run-source.png`。

## 跑步宽步变体 `run-alt-source.png`

> Use case: stylized-concept. Asset type: alternate full-body 2D pixel-art running animation frame for the existing Terraria mod boss. Input image: edit target, the established Zabuza-inspired masked swordsman running sprite. Preserve EXACT identity, headband, mask, black spiky hair, sleeveless dark outfit, gray guards, giant rectangular cleaver with circular hole, same proportions, palette, side-view facing RIGHT, transparent background, scale and pixel-art style. Change only the running gait: move the formerly trailing left leg forward and the formerly leading right leg backward, bend knees into the opposite contact phase, swap arm swing subtly, keep body head and sword silhouette consistent and foot baseline consistent. One character only, full body and sword completely visible, centered with transparent margin. No text, no shadow, no background, no extra objects.

参考图：`art/zabuza-v2/run-source.png`。

## 普通头肩特写 `portrait-source.png`

> Use case: stylized-concept. Asset type: Terraria boss minimap and health-bar head portrait sprite. Input image is exact character identity/style reference. Create one large CLOSE-UP BUST PORTRAIT of the same Zabuza-inspired masked ninja's HEAD AND SHOULDERS ONLY: spiky black hair, diagonal dark forehead band with small silver metal plate, intense right-facing eyes, white cloth lower-face mask and neck wrapping, charcoal sleeveless shoulders. Pixel-art style matching input, strong clean readable silhouette at 40x40 downscale, head fills most of square, shoulders cropped at lower edge. Transparent background, no frame, no sword, no text, no extra subjects. Keep identity consistent; make face lighter and more readable than existing full-body sprite.

参考图：`art/zabuza-v2/idle-source.png`。

## 鬼人头肩特写 `portrait-demon-source.png`

> Use case: precise-object-edit. Asset type: alternate phase-two pixel-art boss head portrait. Input image is the edit target, the masked swordsman head-and-shoulders portrait. Keep exactly the same face, hair, metal headband, mask, clothing, crop, scale and transparent background. Change only expression and supernatural effects: eyes become furious with a restrained bright violet glint, jagged violet/purple demonic chakra wisps surround the hair and shoulders, slight violet rim-light on the dark fabric; face and mask remain clearly readable and not swallowed by darkness. One head bust, no sword, no text, no frame, no extra subject, pixel art with crisp silhouette at 40x40 downscale.

参考图：本目录的 `portrait-source.png`。

`art/Prepare-ZabuzaV4.ps1` 将原图最近邻缩至游戏 PNG。素材均为原创生成，不复制现成动漫/游戏画面。
