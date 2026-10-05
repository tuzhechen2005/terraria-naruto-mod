#!/bin/bash
set -euo pipefail
project_root="$(cd "$(dirname "$0")/.." && pwd -P)"
mkdir -p "$project_root/tools/GameMode/bin"
/usr/bin/swiftc -swift-version 5 -target "$(uname -m)-apple-macos13.0" -framework Carbon \
  "$project_root/tools/GameMode/Core.swift" "$project_root/tools/GameMode/ProcessIO.swift" \
  "$project_root/tools/GameMode/InputSources.swift" \
  "$project_root/tests/GameMode/main.swift" -o "$project_root/tools/GameMode/bin/GameModeTests"
"$project_root/tools/GameMode/bin/GameModeTests"
