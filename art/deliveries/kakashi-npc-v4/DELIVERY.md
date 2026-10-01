# Kakashi NPC v4 delivery

- status: delivered
- request ID: `kakashi-npc-v4`

## 交付文件

| 文件 | 尺寸 | Alpha | 姿势顺序 |
| --- | --- | --- | --- |
| `source/Kakashi_Idle_Walk.png` | 1568×200；7 格，每格 224×200 | RGBA，只有 0/255 | Idle；Walk 1–6 |
| `source/Kakashi_Idle_Jump_Sit_Throw.png` | 1344×200；6 格，每格 224×200 | RGBA，只有 0/255 | Idle；Jump；Sit（橙色小书）；Throw 蓄势、甩手、收手 |
| `source/generated_Kakashi_Idle_Walk.png` | 2171×724 | RGBA，含半透明生成边缘 | 内置图像工具原始姿势条，供追溯 |
| `source/generated_Kakashi_Idle_Jump_Sit_Throw.png` | 2172×724 | RGBA，含半透明生成边缘 | 内置图像工具原始姿势条，供追溯 |
| `Kakashi_Tazuna_comparison.png` | 1100×670 | RGB，不透明 | 达兹纳与卡卡西待机帧并排：游戏 1 倍和 4 倍、深浅背景 |

## 生成与处理

以达兹纳 `Tazuna_Idle_Walk.png` 和 `Tazuna_Idle_Jump_Sit_Throw.png` 为画风参考，使用内置 `image_gen__imagegen` 生成两条卡卡西姿势条。提示词要求银白刺猬头、遮左眼的斜护额、右眼、藏青面罩、明绿色上忍马甲和双口袋、藏青衣裤、白色绑腿，以及深色描边和硬色阶。将生成姿势采样到 28×25 美术像素格，再按最近邻放大 8 倍；做统一调色和透明度二值化。第二张的首帧复用第一张的 Idle，保证尺寸基准一致。

## 已检查

- 逐张目视检查源姿势条与达兹纳深浅背景并排预览；卡卡西各帧朝右，动作可分辨。
- 两张可接入源图分别为 7 格和 6 格；每格 224×200，像素色块严格 8×8 对齐。
- 可接入源图透明度只有 0 和 255，没有半透明杂边；姿势各处于独立帧格。
- 目视确认跳跃离地、坐姿持橙色书、投掷的三个阶段，以及银发、护额、面罩和绿色马甲在游戏尺度可辨。

## 尚需游戏侧检查

- 开发助手用 `scripts/build_npc_sheet.py` 接入并构建；本次美术交付未修改源码、未运行模组构建。
- 在游戏里与达兹纳同屏检查最终缩放、亮度、行走循环、坐姿与苦无发射时机；游戏内验收未运行。
