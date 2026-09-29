using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Players;

namespace ShinobiPrototype.Content.Items;

public sealed class MissionScroll : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.rare = ItemRarityID.White;
        Item.UseSound = SoundID.Item4;
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
            Main.NewText(player.GetModPlayer<StoryPlayer>().CurrentObjective(), 100, 200, 245);
        return true;
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.Wood, 1).Register();
    }
}
