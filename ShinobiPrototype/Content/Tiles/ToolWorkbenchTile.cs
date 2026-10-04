using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using ShinobiPrototype.Common;

namespace ShinobiPrototype.Content.Tiles;

// The tool workbench (specs/装备与忍术系统.spec.md, tier one): where ninja gear is made from blueprints. 3x2, sold by
// the tool shopkeeper; it also serves as a work bench.
public sealed class ToolWorkbenchTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileTable[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
        TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
        TileObjectData.addTile(Type);
        AdjTiles = new int[] { TileID.WorkBenches };
        DustType = DustID.WoodFurniture;
        AddMapEntry(new Color(150, 105, 70), Loc.Text("Place.ToolWorkbench"));
    }
}
