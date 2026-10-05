#!/bin/bash
set -euo pipefail
project_root="$(cd "$(dirname "$0")/.." && pwd -P)"
app_path="$project_root/tools/GameMode/bin/泰拉瑞亚游戏模式.app"
mkdir -p "$app_path/Contents/MacOS" "$app_path/Contents/Resources"
/usr/bin/swiftc -swift-version 5 -O -target "$(uname -m)-apple-macos13.0" -framework Carbon -framework AppKit \
  "$project_root/tools/GameMode/Core.swift" "$project_root/tools/GameMode/ProcessIO.swift" \
  "$project_root/tools/GameMode/InputSources.swift" \
  "$project_root/tools/GameMode/main.swift" \
  -o "$app_path/Contents/MacOS/GameMode"
cp "$project_root/tools/GameMode/Info.plist" "$app_path/Contents/Info.plist"
/usr/bin/codesign --force --sign - "$app_path"
printf '已生成：%s\n' "$app_path"
