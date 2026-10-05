status: delivered

# Kakashi standing sample

- Request ID: `kakashi-direct-pixel-v1`
- Delivered game-size frames: `Kakashi_Idle_final_Right.png`, `Kakashi_Idle_final_Left.png` (80×80 RGBA; alpha only 0/255; transparent RGB zero)
- Inspection images: `Kakashi_Idle_final_6x.png` (480×480 RGBA with binary alpha), `compare.png` (760×570 opaque RGB comparison of Kakashi and approved Iruka at 1× and 3× on light and dark backgrounds)
- Source images: `source/kakashi-generated-original.png`, `source/kakashi-generated-edited.png` (both 1122×1402 RGBA with partial alpha), plus `source/PROMPT.md`

## Prompt summary

Generated one right-facing Kakashi pixel sprite with approved Iruka as style-only reference and old Kakashi art as identity-only reference. Requested silver hair swept up-left, a slanted headband covering the far eye, one narrow lazy visible eye, navy mask, green flak vest, relaxed pose, and transparent background. A targeted image edit shortened the generated hair. Full prompt summary is in `source/PROMPT.md`. Used the built-in image tool; no API key or fallback CLI.

## Export and checks

- Deterministic export using `skills/terraria-npc-pixel-art/scripts/export_sprite.cjs`, `NODE_PATH` set to the local workspace runtime's `pngjs` dependency.
- Parameters: canvas 80×80, height 70, baseline exclusive 76, pixel 1, sample phase (0.5, 0.5), alpha threshold 128, source facing right. Source alpha bounding box: (362,318)–(796,1339). Scale: 0.06856023506366307. Full manifest: `Kakashi_Idle_final_manifest.json`.
- Visible bounding box: x=25–54, y=6–75. Hair-to-feet silhouette height 70 pixels, with feet on y=75; no canvas clipping. Body below the hair is close to the approved Iruka height.
- Verified both game frames are 80×80, have only alpha 0 and 255, and have zero RGB in transparent pixels. Verified left is an exact horizontal mirror of right.
- Visually inspected generated source, 1× frame, 6× inspection image, and `compare.png`. At 1×, the silver hair, mask, vest, right-facing profile, and one narrow visible eye remain identifiable. No second eye is visible.

## Remaining game-side checks

- User review of `compare.png` at native size before producing an animation sheet.
- No game texture, frame dimensions, or NPC scale were changed here. After any later integration, inspect in tModLoader for in-world size, facing, idle alignment, and head icon behavior. No game build or game-side visual test was run for this art-only delivery.

## Claude 补充：统一身高（用户 2026-10-05）

- 用户看 compare.png：“很不错啊这个卡卡西 真不错 就是身高要统一一下”。
- Claude 用同一生成源 `source/kakashi-generated-edited.png` 重新导出，没有重新生图：`--height 62 --phase-x 0.8 --phase-y 0.5`（与伊鲁卡相同的总高与采样相位），输出 `Kakashi_Idle_h62_Right/Left.png`、`_6x.png`、`_manifest.json`，可见范围 y=14～75，与伊鲁卡站立帧头顶、脚底一致。默认相位 0.5/0.5 在 62 高时漏掉了眼白和瞳孔；九种相位对比后选 0.8/0.5。
- `compare_h62.png`：伊鲁卡、卡卡西 62、原 70 版并排（1 倍、3 倍，明暗两种背景）。卡卡西比例更接近成人，统一总高后头比伊鲁卡小一些。
- 仍是样张，未接入游戏。
