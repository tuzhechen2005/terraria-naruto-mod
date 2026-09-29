using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ShinobiPrototype.Content.Tiles;

// Boss trophies from Zabuza and Haku: 3x3 wall decorations like vanilla's boss trophies.
public abstract class WaveTrophyTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.FramesOnKillWall[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
        TileObjectData.addTile(Type);
        DustType = DustID.WoodFurniture;
        AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Trophy"));
    }
}

public sealed class ZabuzaTrophyTile : WaveTrophyTile
{
}

public sealed class HakuTrophyTile : WaveTrophyTile
{
}
