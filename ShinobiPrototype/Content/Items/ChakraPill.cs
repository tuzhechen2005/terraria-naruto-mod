using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Buffs;

namespace ShinobiPrototype.Content.Items;

// 兵粮丸: emergency chakra, gated by Chakra Sickness so it cannot be chained.
public sealed class ChakraPill : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.useStyle = ItemUseStyleID.EatFood;
        Item.useTime = 17;
        Item.useAnimation = 17;
        Item.useTurn = true;
        Item.UseSound = SoundID.Item2;
        Item.consumable = true;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(copper: 50);
    }

    public override bool CanUseItem(Player player) => !player.HasBuff<ChakraSickness>();

    public override bool? UseItem(Player player)
    {
        player.GetModPlayer<ChakraPlayer>().Restore(ChakraRules.PillRestore);
        player.AddBuff(ModContent.BuffType<ChakraSickness>(), ChakraRules.PillSicknessTicks);
        if (player.whoAmI == Main.myPlayer)
            CombatText.NewText(player.getRect(), new Microsoft.Xna.Framework.Color(45, 170, 235), ChakraRules.PillRestore);
        return true;
    }

    public override void AddRecipes()
    {
        CreateRecipe(2).AddIngredient(ItemID.Mushroom).AddIngredient(ItemID.Daybloom)
            .AddTile(TileID.WorkBenches).Register();
    }
}
