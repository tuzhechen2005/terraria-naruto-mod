# 三代火影：直接像素站立样张

状态：已交付站姿候选，未接入游戏。用户2026-10-05要求开始其他NPC像素图，使用已认可skill。本轮先做一张站立图，不扩动作、不接入。修改范围仅 `art/deliveries/hiruzen-direct-pixel-v1/`。

先读 `skills/terraria-npc-pixel-art/SKILL.md` 和 `references/workflow.md`。内置imagegen直接生成粗像素人物，不从写实立绘像素化，不以脚本从零拼人物。

参考：
- 风格：skill `assets/approved-iruka-6x.png` 和真实 `approved-iruka-right.png`，仅画风。保留大块暖色、明确剪影与真实小帧特征，不复制眼睛格数和鼻疤。
- 身份：`art/reference/hokage_rock_web/Hiruzen_Sarutobi.png`，老人、小且细长的眼睛、白色短胡须、火影帽；只用于身份，原作图不复制到交付。
- 现有角色与服装：`art/deliveries/hiruzen-npc-v2/source/Hiruzen_Idle_Walk.png` 第一格、`ShinobiPrototype/Content/NPCs/Hiruzen.cs`。戴白色宽火影帽，红色帽沿/火标记，白袍红色饰边、深色内领，持长棕色烟斗。保持此前老人略矮的设定。

目标：一张面朝右的完整站立样张，温和有威严，宽帽与袍不遮掉脸，灰白胡须、烟斗可辨，眼睛按老人细长眼设计，不放成伊鲁卡大眼。普通裸露脸省略嘴线，不用密集皱纹代替年龄特征。人物约57屏幕像素高（含帽），80×80透明画布，脚底y75，身体中线x40，显示1倍；暖光、大色块、少细纹，非写实非插画。

使用skill通用 `scripts/export_sprite.cjs` 导出，先从真实PNG检查眼睛和胡须，必要时检查采样相位。保留原图、完整提示词、导出参数manifest。交付 `Hiruzen_Idle_Right.png` / `Left.png`、1倍/3倍明暗对比（与伊鲁卡、卡卡西站姿并排）、6倍头部、preview.html 和 DELIVERY.md。0/255alpha、透明RGB零、身高脚底、镜像、无裁切。不运行游戏构建，不替换现有Hiruzen贴图；状态应标明站姿候选待用户确认。
