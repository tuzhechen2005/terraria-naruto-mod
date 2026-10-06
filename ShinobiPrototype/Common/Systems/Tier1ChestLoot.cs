using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.Items.Tier1;
using ShinobiPrototype.Content.Items.Weapons;

namespace ShinobiPrototype.Common.Systems;

// Tier one's finds (specs/装备与忍术系统.spec.md): the Phoenix Flower jutsu in about one underground wooden or gold
// chest in six, the training post in about one underground gold chest in eight. The rogue hideouts add more later.
public sealed class Tier1ChestLoot : ModSystem
{
    private const int WoodenChestStyle = 0;
    private const int GoldChestStyle = 1;
    public const int PhoenixFlowerOneIn = 6;
    public const int TrainingPostOneIn = 8;

    public override void PostWorldGen()
    {
        foreach (Chest chest in Main.chest)
        {
            if (chest == null || chest.y < Main.worldSurface)
                continue;
            Tile tile = Main.tile[chest.x, chest.y];
            if (tile.TileType != TileID.Containers)
                continue;
            int style = tile.TileFrameX / 36;
            if (style is WoodenChestStyle or GoldChestStyle && WorldGen.genRand.NextBool(PhoenixFlowerOneIn))
                Put(chest, ModContent.ItemType<PhoenixFlowerJutsu>());
            if (style == GoldChestStyle && WorldGen.genRand.NextBool(TrainingPostOneIn))
                Put(chest, ModContent.ItemType<TrainingPost>());
        }
    }

    private static void Put(Chest chest, int type)
    {
        foreach (Item item in chest.item)
            if (item.IsAir)
            {
                item.SetDefaults(type);
                return;
            }
    }
}
