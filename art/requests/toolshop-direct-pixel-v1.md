# 忍具店老板：直接像素站立样张

状态：已交付站姿候选，未接入游戏。用户2026-10-05要求开始其他NPC像素图，使用已认可skill。本轮先做一张站立图，不扩动作、不接入。修改范围仅 `art/deliveries/toolshop-direct-pixel-v1/`。

先读 `skills/terraria-npc-pixel-art/SKILL.md` 和 `references/workflow.md`。使用内置imagegen直接生成粗像素人物，不从写实立绘像素化，不以脚本从零拼人物。

参考：
- 风格：skill `assets/approved-iruka-6x.png` / `approved-iruka-right.png`，仅风格，保留大色块、暖色明暗、清楚轮廓与真实小帧眼睛，不复制疤或眼睛格数。
- 身份与衣装：`art/deliveries/tool-shop-npc-v1/source/ToolShop_Idle_Walk.png` 第一格及原生成 `source/generated_ToolShop_Idle_Walk.png` 第一人，阅读原DELIVERY.md；现游戏 `ShinobiPrototype/Content/NPCs/ToolShopkeeper.png`、代码 `ToolShopkeeper.cs`。项目里的天天父亲，中年忍具商人、单个圆发髻、短胡须、酒红中式盘扣上衣、深色工作围裙、腰袋、深裤与裹腿/布鞋。不要改成达兹纳或复制他的斗笠。

目标：一张面朝右完整站立样张，朴实、有经验、友善精明；自然稍壮的体型，发髻、胡须、酒红上衣、围裙大色块可辨。眼睛按本人的中年眼型设计，和伊鲁卡不同，胡须与皮肤区分。省嘴线但胡须可有轮廓；少量明显盘扣即可，不在围裙堆细小工具。62屏幕像素高，80×80透明画布，脚底y75，身体中线x40，显示1倍。

使用skill通用 `scripts/export_sprite.cjs` 导出，检查真实PNG面部与胡须，检查采样相位。保留原图、完整提示词、导出manifest。交付 `ToolShopkeeper_Idle_Right.png` / `Left.png`、1倍/3倍明暗对比（与伊鲁卡、卡卡西站姿并排）、6倍头部、preview.html 和 DELIVERY.md。0/255alpha、透明RGB零、身高脚底、镜像、无裁切。不要构建/替换当前游戏贴图，站姿候选待用户确认。
