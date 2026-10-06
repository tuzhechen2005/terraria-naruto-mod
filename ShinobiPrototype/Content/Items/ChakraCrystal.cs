using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;

namespace ShinobiPrototype.Content.Items;

// Like a Life Crystal: permanently raises maximum chakra, up to the pre-Hardmode cap.
public sealed class ChakraCrystal : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 30;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.UseSound = SoundID.Item4;
        Item.consumable = true;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(silver: 75);
    }

    public override bool CanUseItem(Player player) =>
        ChakraRules.CanUseCrystal(player.GetModPlayer<ChakraPlayer>().Crystals);

    public override bool? UseItem(Player player)
    {
        if (!player.GetModPlayer<ChakraPlayer>().TryUseCrystal())
            return false;
        if (player.whoAmI == Main.myPlayer)
            CombatText.NewText(player.getRect(), new Color(45, 170, 235), Loc.Get("Chakra.CrystalUsed", ChakraRules.CrystalBonus));
        return true;
    }
}
