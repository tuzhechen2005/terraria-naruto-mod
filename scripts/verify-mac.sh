#!/bin/sh
set -eu

project_root=$(CDPATH= cd -- "$(dirname -- "$0")/.." && pwd)
dotnet_x64=${DOTNET_X64:-"$HOME/.dotnet-x64/dotnet"}
tml_dir=${TML_DIR:-"$HOME/Library/Application Support/Steam/steamapps/common/tModLoader"}
native_dir="$tml_dir/Libraries/Native/OSX"

if [ ! -x "$dotnet_x64" ]; then
  echo "Missing x64 .NET 8 SDK: $dotnet_x64" >&2
  exit 1
fi
if [ ! -f "$tml_dir/tMLMod.targets" ] || [ ! -f "$native_dir/libFNA3D.0.dylib" ]; then
  echo "Missing tModLoader build files: $tml_dir" >&2
  exit 1
fi

temp_dir=$(mktemp -d "${TMPDIR:-/tmp}/shinobi-build.XXXXXX")
trap 'rm -rf "$temp_dir"' EXIT HUP INT TERM
ln -s "$native_dir/libFNA3D.0.dylib" "$temp_dir/libFNA3D.dylib"
cat > "$temp_dir/dotnet" <<'SH'
#!/bin/sh
export DYLD_LIBRARY_PATH="$SHINOBI_NATIVE_SHIM:$SHINOBI_NATIVE_DIR"
exec "$SHINOBI_DOTNET_X64" "$@"
SH
chmod +x "$temp_dir/dotnet"

export SHINOBI_NATIVE_SHIM="$temp_dir"
export SHINOBI_NATIVE_DIR="$native_dir"
export SHINOBI_DOTNET_X64="$dotnet_x64"
export DOTNET_ROOT="$(dirname "$dotnet_x64")"
export PATH="$temp_dir:$DOTNET_ROOT:$PATH"

cd "$project_root"
for suite in ChakraRules ExamRules ChallengeRules ZabuzaCombatRules DebugModeRules WaveDuoRules; do
  "$dotnet_x64" run --project "tests/$suite/$suite.Tests.csproj" --nologo
done
"$dotnet_x64" build ShinobiPrototype/ShinobiPrototype.csproj \
  "-p:TmlTargetsPath=$tml_dir/tMLMod.targets" --nologo
