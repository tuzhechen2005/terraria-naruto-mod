using Terraria;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Items;

// Replaced by the Ninja Handbook; old copies turn into one when they reach the inventory.
public sealed class MissionScroll : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
    }

    public override void UpdateInventory(Player player)
    {
        int stack = Item.stack;
        Item.SetDefaults(ModContent.ItemType<NinjaHandbook>());
        Item.stack = stack;
    }
}
