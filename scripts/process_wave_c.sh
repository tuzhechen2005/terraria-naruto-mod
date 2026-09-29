#!/bin/sh
# Rebuild the Terraria-native (C-style, 2x2 art pixels) Wave Country frames from Codex sources.
# Sources are drawn at 8x of the art grid; each sheet starts with a size-reference pose.
set -eu
cd "$(dirname "$0")/.."
OUT=${1:-$(mktemp -d)}
P="python3 -W ignore scripts/pixelize_frames.py"
Z="--prefix Zabuza --canvas 288x128 --center-x 144 --baseline 124 --body-height 84 --scale 0.25 --skip 0 --colors 0 --pixel 2 --nearest --auto --min-area 3000"
B=art/deliveries/zabuza-c-base/source
for a in Idle:4 Run:6 Windup:3 Slash:3 Seal:3 Leap:2 Dash:2; do
  $P "$B/Zabuza_${a%%:*}.png" $OUT/z $Z --order "$a"
done
T=art/deliveries/zabuza-c-transition-frenzy/source
for a in Kneel:2 Roar:3 FrenzyRoar:3 Throw:3 Unarmed:6 Catch:2; do
  $P "$T/Zabuza_${a%%:*}.png" $OUT/z $Z --order "$a"
done
F=art/deliveries/zabuza-c-frenzy-base/source
for a in FrenzyIdle:4 FrenzyRun:6 FrenzyWindup:3 FrenzySlash:3 FrenzySeal:3 FrenzyLeap:2 FrenzyDash:2; do
  $P "$F/Zabuza_${a%%:*}.png" $OUT/z $Z --order "$a"
done
H="--prefix Haku --canvas 144x96 --center-x 72 --baseline 92 --body-height 76 --scale 0.25 --skip 0 --colors 0 --pixel 2 --nearest --auto --min-area 3000"
HS=art/deliveries/haku-c/source
for a in Idle:4 Move:4 Throw:4 Dash:3; do
  $P "$HS/Haku_${a%%:*}.png" $OUT/h $H --order "$a"
done
$P $HS/Haku_Emerge.png $OUT/h --prefix Haku --canvas 160x96 --center-x 80 --baseline 92 --body-height 76 \
  --scale 0.25 --skip 0 --colors 0 --pixel 2 --nearest --auto --min-area 3000 --order Emerge:4
echo "$OUT"
# Props: single-object sources drawn at 8x; shrink each to its art size, draw at 2x2.
python3 -W ignore - "$OUT" <<'PY'
import sys
from pathlib import Path
import numpy as np
from PIL import Image
out = Path(sys.argv[1]) / "p"
out.mkdir(parents=True, exist_ok=True)
def prop(src, name, w, h):
    im = Image.open(src).convert("RGBA")
    im = im.crop(im.getbbox())
    im.thumbnail((w, h), Image.NEAREST)
    a = np.array(im)
    a[..., 3] = np.where(a[..., 3] < 64, 0, np.where(a[..., 3] > 220, 255, a[..., 3]))
    canvas = Image.new("RGBA", (w, h))
    canvas.alpha_composite(Image.fromarray(a), ((w - im.width) // 2, (h - im.height) // 2))
    canvas.resize((w * 2, h * 2), Image.NEAREST).save(out / name)
H = "art/deliveries/haku-c/source/"
prop(H + "Haku_IceMirror_Whole.png", "IceMirror_0.png", 24, 36)
prop(H + "Haku_IceMirror_Cracked.png", "IceMirror_1.png", 24, 36)
prop(H + "Haku_IceMirror_Shattered.png", "IceMirror_2.png", 24, 36)
prop(H + "Haku_Senbon.png", "HakuSenbon.png", 16, 3)
prop("art/deliveries/zabuza-c-transition-frenzy/source/ZabuzaThrownSword.png", "ZabuzaThrownSword.png", 44, 16)
print("props ->", out)
PY
