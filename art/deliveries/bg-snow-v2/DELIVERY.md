status: blocked
request ID: bg-snow-v2

## 阻塞原因

内置 `image_gen__imagegen` 在生成远景 `far.png` 时返回 HTTP 429 `usage_limit_reached`（Plus 使用额度耗尽，工具返回约 3249 秒后重置）。本次未生成或交付任何 PNG；按请求未使用 API key 或备用 CLI。

## 待完成

额度重置后，使用内置图像工具生成并检查 `far.png`、`mid.png`、`close.png`，制作日间与夜间双次平铺叠放预览，核对尺寸、透明度、像素轮廓、接缝及游戏尺度可读性，然后更新本文件为 `status: delivered`。游戏内视差、昼夜效果仍需项目开发助手接入后验收。
