# XnbExtract

Extracts vanilla Terraria textures (LZX-compressed XNB) to raw RGBA using the FNA
decompressor bundled with tModLoader, for side-by-side style checks only. Extracted
vanilla art must stay local (`art/reference/terraria/` is gitignored).

    ~/.dotnet-x64/dotnet build tools/XnbExtract
    DYLD_LIBRARY_PATH="$TML/Libraries/Native/OSX" ~/.dotnet-x64/dotnet \
      tools/XnbExtract/bin/Debug/net8.0/Xnbx.dll <out_dir> <Terraria.app/.../Images/NPC_22.xnb> ...

Each `.rgba` file holds width, height, format (int32) then premultiplied RGBA bytes;
`scripts/terraria_scene_preview.py`'s neighbors convert them to PNG (see git history of this commit).
