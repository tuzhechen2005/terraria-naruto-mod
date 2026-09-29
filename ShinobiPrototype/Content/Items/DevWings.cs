using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Players;

namespace ShinobiPrototype.Content.Items;

// Developer-only (from /m0 items): free flight in any direction that speeds up the longer you hold a direction.
public sealed class DevWings : ModItem
{
    public override string Texture => $"Terraria/Images/Item_{ItemID.AngelWings}";

    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 26;
        Item.accessory = true;
        Item.rare = ItemRarityID.Red;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<DevFlightPlayer>().Equipped = true;
        player.noFallDmg = true;
        if (!hideVisual)
            player.wings = ContentSamples.ItemsByType[ItemID.AngelWings].wingSlot;
    }
}
