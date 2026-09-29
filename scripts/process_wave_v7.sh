#!/bin/sh
# Rebuild the final-style (v7) Wave Country boss frames from Codex source sheets.
# Output goes to $1 (default: a temp dir); review, then copy into ShinobiPrototype/Content.
set -eu
cd "$(dirname "$0")/.."
OUT=${1:-$(mktemp -d)}
P="python3 -W ignore scripts/pixelize_frames.py"
Z="--prefix Zabuza --canvas 224x112 --center-x 112 --baseline 108 --body-height 84 --reference-fraction 0.89 --reference-pose 0 --skip 0 --colors 0 --pixel 1 --nearest"
B=art/deliveries/zabuza-v7-base/source
$P $B/Zabuza_Idle.png $OUT/z $Z --order Idle:4
$P $B/Zabuza_Run_Leap.png $OUT/z $Z --order Run:6,Leap:2
$P $B/Zabuza_Windup_Slash_Seal.png $OUT/z $Z --order Windup:3,Slash:3,Seal:2 \
  --centers 176,394,638,882,1086,1317,1629,1860,2077
cp $OUT/z/Zabuza_Seal_1.png $OUT/z/Zabuza_Seal_2.png
$P $B/Zabuza_Dash.png $OUT/z $Z --order Dash:2
T=art/deliveries/zabuza-v7-transition-frenzy/source
# This batch drew its reference pose with the sword lower, so its bbox is mostly body.
Z2=$(echo "$Z" | sed "s/--reference-fraction 0.89/--reference-fraction 0.97/")
for a in Kneel:2 Unarmed:6; do
  $P "$T/Zabuza_${a%%:*}.png" $OUT/z $Z2 --order "$a"
done
# These sheets have swords touching the next pose: seed each torso.
$P $T/Zabuza_Roar.png $OUT/z $Z2 --order Roar:3 --centers 258,815,1357,1928
$P $T/Zabuza_FrenzyRoar.png $OUT/z $Z2 --order FrenzyRoar:3 --centers 285,815,1371,1928
$P $T/Zabuza_Throw.png $OUT/z $Z2 --order Throw:3 --centers 285,869,1357,1928
$P $T/Zabuza_Catch.png $OUT/z $Z2 --order Catch:2 --centers 271,950,1941
F=art/deliveries/zabuza-v7-frenzy-base/source
for a in FrenzyIdle:4 FrenzyRun:6 FrenzyWindup:3 FrenzySlash:3 FrenzySeal:3 FrenzyLeap:2 FrenzyDash:2; do
  $P "$F/Zabuza_${a%%:*}.png" $OUT/z $Z2 --order "$a"
done
python3 scripts/recolor_clothes.py $OUT/z/Zabuza_Kneel_*.png $OUT/z/Zabuza_Roar_*.png \
  $OUT/z/Zabuza_FrenzyRoar_*.png $OUT/z/Zabuza_Throw_*.png $OUT/z/Zabuza_Unarmed_*.png $OUT/z/Zabuza_Catch_*.png
H="--prefix Haku --canvas 112x88 --center-x 56 --baseline 84 --body-height 72 --reference-pose 0 --skip 0 --colors 0 --pixel 1 --nearest"
HS=art/deliveries/haku-v5/source
$P $HS/Haku_Idle.png $OUT/h $H --order Idle:4
$P $HS/Haku_Throw_Dash.png $OUT/h $H --order Throw:4,Dash:3
for i in 0 1 2; do cp $OUT/h/Haku_Dash_$i.png $OUT/h/Haku_Move_$i.png; done
cp $OUT/h/Haku_Dash_1.png $OUT/h/Haku_Move_3.png
$P $HS/Haku_Emerge.png $OUT/h --prefix Haku --canvas 160x96 --center-x 80 --baseline 92 --body-height 72 \
  --reference-pose 0 --skip 0 --colors 0 --pixel 1 --nearest --order Emerge:4
$P art/deliveries/zabuza-demon-aura-v1/source/zabuza_demon_aura_source.png $OUT/a --prefix Zabuza \
  --canvas 176x160 --center-x 88 --baseline 156 --body-height 100 --reference-pose 0 --skip 0,7 \
  --colors 0 --pixel 1 --nearest --merge 14 --min-area 4000 --order Aura:6,AuraBurst:3
echo "$OUT"
