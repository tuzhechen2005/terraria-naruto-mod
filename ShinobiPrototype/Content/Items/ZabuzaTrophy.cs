using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.Tiles;

namespace ShinobiPrototype.Content.Items;

// Zabuza's boss trophy (about 1 in 10 per win), hung on a wall.
public sealed class ZabuzaTrophy : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<ZabuzaTrophyTile>());
        Item.width = 32;
        Item.height = 32;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(gold: 1);
    }
}
