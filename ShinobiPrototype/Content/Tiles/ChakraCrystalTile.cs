using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using ShinobiPrototype.Content.Items;

namespace ShinobiPrototype.Content.Tiles;

// Chakra Crystal as found underground: a 2x2 object like the Life Crystal, mined for the item.
public sealed class ChakraCrystalTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileSpelunker[Type] = true;
        Main.tileOreFinderPriority[Type] = 650;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(0, 1);
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.addTile(Type);

        DustType = DustID.BlueCrystalShard;
        HitSound = SoundID.Shatter;
        AddMapEntry(new Color(45, 170, 235), CreateMapEntryName());
        RegisterItemDrop(ModContent.ItemType<ChakraCrystal>());
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        r = 0.08f;
        g = 0.35f;
        b = 0.55f;
    }
}
