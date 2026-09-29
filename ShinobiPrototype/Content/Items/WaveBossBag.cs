using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Systems;

namespace ShinobiPrototype.Content.Items;

// Treasure bag from Zabuza and Haku in Expert and Master mode, one per player: the same weapon and mask roll as a
// normal-mode win, plus some money. (The expert-only accessory is still to be decided with the user.)
public sealed class WaveBossBag : ModItem
{
    public override string Texture => $"Terraria/Images/Item_{ItemID.EyeOfCthulhuBossBag}";

    public override void SetStaticDefaults()
    {
        ItemID.Sets.BossBag[Type] = true;
        ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;
        Item.ResearchUnlockCount = 3;
    }

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 34;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.rare = ItemRarityID.Expert;
        Item.expert = true;
    }

    public override bool CanRightClick() => true;

    public override void RightClick(Player player)
    {
        IEntitySource source = player.GetSource_OpenItem(Type);
        foreach (WaveLoot loot in WaveLootRules.Roll(Main.rand.Next, Main.rand.NextDouble))
        {
            int type = WaveRewards.ItemFor(loot);
            if (type > 0)
                player.QuickSpawnItem(source, type);
        }
        player.QuickSpawnItem(source, ItemID.GoldCoin, 3);
    }
}
