using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Items;

public sealed class MistInsignia : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 99;
        Item.rare = ItemRarityID.White;
        Item.value = 0;
    }
}
