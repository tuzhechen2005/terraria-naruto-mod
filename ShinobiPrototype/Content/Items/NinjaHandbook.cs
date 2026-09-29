using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Systems;

namespace ShinobiPrototype.Content.Items;

// 忍者手册: opens the handbook panel (objective, jutsu, chakra pages).
public sealed class NinjaHandbook : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.rare = ItemRarityID.White;
        Item.UseSound = SoundID.MenuOpen;
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
            HandbookSystem.Toggle();
        return true;
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.Wood).Register();
    }
}
