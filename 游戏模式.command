#!/bin/bash
set -euo pipefail
project_root="$(cd "$(dirname "$0")" && pwd -P)"
game_mode_app="$project_root/tools/GameMode/bin/泰拉瑞亚游戏模式.app"
if [[ ! -x "$game_mode_app/Contents/MacOS/GameMode" ]]; then
  "$project_root/scripts/build-game-mode.sh"
fi
/usr/bin/open "$game_mode_app"
