#!/bin/sh
# Build the mod into a private tModLoader save directory (so it works while the game is open), let a headless server
# generate a new world, dump the Hidden Leaf Village and the Chunin Exam landmarks (KonohaDump) and render them with
# vanilla textures.
#
#   ./scripts/konoha-worldgen-check.sh <work_dir> [seed] [size 1|2|3]
#
# Writes <work_dir>/konoha-<seed>-<size>.png and prints vanilla's housing verdict for every designed home.
set -eu
project_root=$(CDPATH= cd -- "$(dirname -- "$0")/.." && pwd)
work=$1; seed=${2:-12345}; size=${3:-1}
dotnet_x64="$HOME/.dotnet-x64/dotnet"
tml_dir="$HOME/Library/Application Support/Steam/steamapps/common/tModLoader"
native_dir="$tml_dir/Libraries/Native/OSX"
save="$work/tml"
mkdir -p "$save/Mods" "$save/Worlds"
echo '["ShinobiPrototype"]' > "$save/Mods/enabled.json"

shim=$(mktemp -d "${TMPDIR:-/tmp}/shinobi-shim.XXXXXX")
trap 'rm -rf "$shim"' EXIT HUP INT TERM
ln -s "$native_dir/libFNA3D.0.dylib" "$shim/libFNA3D.dylib"
cat > "$shim/dotnet" <<'SH'
#!/bin/sh
export DYLD_LIBRARY_PATH="$SHINOBI_NATIVE_SHIM:$SHINOBI_NATIVE_DIR"
exec "$SHINOBI_DOTNET_X64" "$@"
SH
chmod +x "$shim/dotnet"
export SHINOBI_NATIVE_SHIM="$shim" SHINOBI_NATIVE_DIR="$native_dir" SHINOBI_DOTNET_X64="$dotnet_x64"
export DOTNET_ROOT="$(dirname "$dotnet_x64")"
PATH="$shim:$DOTNET_ROOT:$PATH"; export PATH

"$dotnet_x64" build "$project_root/ShinobiPrototype/ShinobiPrototype.csproj" "-p:TmlTargetsPath=$tml_dir/tMLMod.targets" \
  "-p:ExtraBuildModFlags=-tmlsavedirectory $save" --nologo -v q | grep -E "error|Build succeeded" | sort -u

name="konoha-$seed-$size"
dump="$work/$name.jsonl"
rm -f "$dump" "$save/Worlds/$name.wld" "$save/Worlds/$name.twld"
(cd "$tml_dir" && DYLD_LIBRARY_PATH="$shim:$native_dir" SHINOBI_KONOHA_DUMP="$dump" "$dotnet_x64" tModLoader.dll -server \
  -nosteam -tmlsavedirectory "$save" -autocreate "$size" -world "$save/Worlds/$name.wld" -worldname "$name" \
  -seed "$seed" -difficulty 0 -port 7799 -players 1 < /dev/null > "$work/$name.log" 2>&1) || true
if [ ! -s "$dump" ]; then
  echo "No dump written; see $work/$name.log" >&2
  tail -20 "$work/$name.log" >&2
  exit 1
fi
python3 "$project_root/scripts/render_konoha_world.py" "$dump" "$work/$name.png"
# The Chunin Exam landmarks (ExamSiteWorld), when the world has them.
for kind in Gate Tower Stadium HollowTree; do
  if [ -s "$work/$name-$kind.jsonl" ]; then
    echo "== $kind"
    python3 "$project_root/scripts/render_konoha_world.py" "$work/$name-$kind.jsonl" "$work/$name-$kind.png"
  fi
done
