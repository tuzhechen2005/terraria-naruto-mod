# 素材请求：mod-icon-v5

- 状态：integrated。内置编辑、尺寸导出、文字检查、README 预览与构建已完成。
- 提出者及提交号：Codex，基于 `40303c2`。
- 用户明确选择的名称：**Terruto**。用户要求把现有图标中的 Naruto 字标也改成这个名字。
- 任务类型：`text-localization`，编辑现有位图中的字标，沿用已接入的 v4 构图与风格。
- 用户已授权这次文字替换并接入；这不是重新设计人物或构图。

## 编辑目标与参考

1. **编辑目标**：`art/deliveries/mod-icon-v4/source/generated-icon.png`，原始高分辨率封面。必须先使用 `view_image` 查看，再通过内置 `image_gen__imagegen` 编辑。
2. **成品尺寸参考**：`art/deliveries/mod-icon-v4/readme-icon.png`、`art/deliveries/mod-icon-v4/icon.png`。只用于核对采样后的像素风与尺寸，不要把小图当作高分辨率编辑目标。

图片和文字冲突时以图片中的原构图为准；底部字标的文字必须按下方要求替换。

## 唯一视觉改动

- 将底部橙金色石质像素字标 **NARUTO** 替换为 **TERRUTO**，严格逐字为 **T E R R U T O**，共七个字母。
- 使用与现有字标相同的粗壮像素字形、橙金色亮面、深橙阴影、深色立体描边和石质底座。七个字母需要适当收窄、均匀分配宽度，完整置于现有的底部字标区域。
- 不增加副标题或其他文字。不留下原字标的任何字母。
- 保留少年鸣人与卡卡西的脸、服装、姿势和位置；保留手与螺旋丸的连接、蓝色能量、橙色查克拉、木叶村、火影岩、树木、建筑、原边框及角部装饰。
- 保持原图的方形画幅和布局，背景仍为完整不透明像素画。编辑仅限字标所在的下部区域，不重新画人物或上半部场景。

## 工具与交付

- 使用 imagegen 技能和内置 `image_gen__imagegen` 的现有图编辑能力。不要用 API 密钥、CLI 图像 API、SVG 或脚本字体绘制字标；后处理仅用于采样、限色和尺寸预览。
- 将最终选择的编辑源图复制到 `art/deliveries/mod-icon-v5/source/edited-icon.png`。保留完整的内置编辑提示词于 `DELIVERY.md`。
- `icon.png`：80×80，原封面的完整构图，最多 64 色，无抖动，检查七个字母是否正确且可辨。
- `icon_small.png`：30×30，沿用 v4 的人物区域裁切，保留鸣人、卡卡西和螺旋丸；如此小的尺寸不强塞字标。
- `readme-icon.png`：源图采样为 160×160，最多 128 色，无抖动，再最近邻放大到 640×640。底部必须能清楚读出 TERRUTO。
- `preview.png`：展示 README 成品、80×80 原尺寸与放大、30×30 原尺寸与放大。使用白色与 GitHub 深色底检查边缘和可读性。
- `DELIVERY.md`：记录内置工具、完整编辑提示词、文件尺寸/模式/透明度、采样方法、字母核对、人物与场景保留情况、尚未进行的游戏侧检查。
- 只写 `art/deliveries/mod-icon-v5/`，不修改源码、请求单或 Git。开发助手将核对成品后接入本次已授权的字标修改。

## 接入记录

- 内置图像编辑已完成，源图为 `art/deliveries/mod-icon-v5/source/edited-icon.png`。后台 worker 在源图生成后、尺寸导出阶段长时间未继续产出，开发助手停止该 worker，并用这张编辑源图完成尺寸采样与预览；没有重复生图或用脚本重画字标。
- 接入 `ShinobiPrototype/icon.png`（80×80）、`ShinobiPrototype/icon_small.png`（30×30）和 `docs/images/mod-icon.png`（640×640），两版 README 继续以 320×320 显示封面。
- 核对字标逐字为 T E R R U T O；README 与 80×80 图标均可辨，人物与场景保留原构图。PNG 解码、尺寸、色数与交付文件一致性检查通过，两版 GitHub Markdown 与浏览器预览通过。
- 使用独立存档目录运行 `./scripts/verify-mac.sh`：13 组规则测试与模组打包通过，0 错误；保留既有告警。尚未在游戏内重新加载并检查图标。
