# 素材请求：kakashi-direct-pixel-anim-v2

- 状态：已交付并接入（2026-10-05，NpcSheet.KakashiScale = 1；游戏内未实测）。10 投掷背后仍有一小截钩状色块，待用户看
- 提出者及提交号：Claude Code 验收 `kakashi-direct-pixel-anim-v1`。14 帧里 10 帧通过，**只重做 4 帧**：`Kakashi_03_Walk`、`Kakashi_04_Walk`、`Kakashi_05_Walk`、`Kakashi_11_Throw`。
- 先读 `art/deliveries/kakashi-direct-pixel-anim-v1/DELIVERY.md` 与 `export_frames.py`，沿用完全相同的比例、相位、贴头矩形（包含面罩下缘与领口）、x=40 对齐和脚底 y=75。
- 问题（见 `art/deliveries/kakashi-direct-pixel-anim-v1/claude_review_8x.png`，从左到右 03、04、05、10、11；10 通过，只作对照）：
  1. **03、04 Walk**：马甲下摆在腰前鼓出一块方形绿色色块，像盒子或口袋，人物轮廓被撑歪。马甲下摆应和站立帧一样平直收在腰上。
  2. **05 Walk、11 Throw**：背后（画面左侧、肩下）伸出一条深蓝色钩状色块，像尾巴；应是后侧手臂自然垂下或摆动，袖子和手连贯，手是肤色/手套。
- 做法：可重新生成这几个姿势，或对原生成图做局部编辑后重新导出；03～05 要和其余走路帧连成同一循环（腿的相位与 v1 一致：03、04、05 分别接在 02 之后、06 之前）。其余 10 帧不要改。
- 交付（`art/deliveries/kakashi-direct-pixel-anim-v2/`）：`frames/` 下**完整 14 帧**（未改的 10 帧原样复制自 v1），`walk.gif`、`strip_1x_3x.png`、`fix_8x.png`（4 帧改前/改后放大 8 倍）、`DELIVERY.md`。
- 交付后接入提交号：待填
