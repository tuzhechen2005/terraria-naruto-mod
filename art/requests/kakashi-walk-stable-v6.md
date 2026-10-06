# 卡卡西走路：消除闪烁、锁定同一人物、连续六帧

状态：已交付，开发助手在round1中稳定胸口内区/身体色板后接入，构建通过，实机待验收。用户看到v5走路反馈「感觉怪怪的，一闪一闪，感觉没有对齐」。当前开发助手已经确认：v3前三帧+v4后三帧虽然腰轴接近，但眼睛y29跳到y27、头形和衣服版型/色块变动，六姿势并非连续运动；GIF各帧独立量化还添细微色漂移。此请求明确授权后台美术工作，只写 art/deliveries/kakashi-walk-stable-v6/，不碰游戏、其他交付、请求、Git。

先读 skills/terraria-npc-pixel-art/SKILL.md 和 imagegen 技能。目标是修现有动画，不再换人物外观。以用户认可的 art/deliveries/kakashi-direct-pixel-anim-v2/frames/Kakashi_00_Idle.png 为唯一头脸/体型/衣服基准，保留眼白较多的单只懒眼，不扩大眼睛。

参考：
- 上述Idle（80x80、body62、feet75）
- art/deliveries/kakashi-eye-restore-v5/frames/ 六个Walk：待修动作，不能当稳定头脸基准。
- art/deliveries/npc-direct-pixel-round1/kakashi_walk_strip.png：闪烁的坏例，前三后三区别明显。
- art/deliveries/npc-direct-pixel-round1/assemble_kakashi.py：当前拼装来源，仅供定位，不沿用六姿势顺序。

## 制作策略

用内置imagegen对同一底稿设计连贯六帧慢走（左右脚交替：contact A / down-passing A / up A / contact B / down-passing B / up B，最后回到contact A）。正面四分之三朝右，与Idle一致。每帧是同一人物，固定头部位置、头发轮廓、护额/眼睛/面罩，避免任何面部闪动；普通城镇慢走不必加上下跳。马甲胸口和骨盆轴稳定，胳膊小摆幅，胯下连贯。不从两个独立生成风格各取半循环。

**允许且优先将已认可Idle整块头部（含脖子/领口）原样复用到六帧相同坐标**，比只贴眼睛更可靠。机械裁切/平移、复用已认可局部可做，不用脚本从零画人物。若生成改变已认可的头脸，最终一定复用原头；清除被替代的旧头轮廓，不能留下重影。脖子与肩领须确实匹配，不能只贴下巴。躯干可在同一母帧基础上复用以消除衣服纹理抖动，但一定先让图像生成的胯/裤腰/腿根与母帧几何匹配，再按锚点合成；禁止重现v2在不匹配腰线处剪贴上半身、下半身分离的错误。不强求大步幅，宁可小而连续的完整慢走。

导出共同比例，80x80、feet75，不能每帧各自拉高62。各帧公用材质色板，保留认可色彩。不可六帧重复Idle冒充走路。白绷带腿贯穿整循环同一条腿，左右脚/手摆连贯；避免某半循环人变宽、衣服变亮、头在肩上滑动。

## 验收与交付

- 最终14帧frames/命名同v5（6Walk替换，其他8帧从v2原样copy）。
- 同一固定头区逐像素一致；记录头复用区域/锚点、腰/髋/脚底坐标。六帧前后叠看、正常1倍循环、3倍循环、逐帧并排检查。不要只算alpha连通。
- 公用全循环GIF palette，或同时交APNG作无损对照；不能让GIF独立量化每帧产生假闪。无优化丢帧、固定背景、适当disposal。交 PNG strip、1x/3x GIF、APNG，提供before/after。
- 透明alpha0/255，透明RGB0，无裁切。旧非Walk8帧byte-identical，Idle眼不改。
- 完整源图、提示词、可复现导出/局部复用脚本、参数、检查结果及DELIVERY.md。
- 有明显闪动/身体连接问题先修正再交付，不把生成次数或静态每帧好看当成功。不用额外做其他NPC。
