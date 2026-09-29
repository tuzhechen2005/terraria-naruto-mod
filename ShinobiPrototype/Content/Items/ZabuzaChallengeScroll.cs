using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.NPCs;
using ShinobiPrototype.Common;

namespace ShinobiPrototype.Content.Items;

public sealed class ZabuzaChallengeScroll : ModItem
{
    public override string Texture => "ShinobiPrototype/Content/Items/MissionScroll";

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.consumable = false;
        Item.rare = ItemRarityID.Green;
    }

    public override bool CanUseItem(Player player)
    {
        bool anotherBossActive = NPC.AnyNPCs(ModContent.NPCType<ZabuzaBoss>()) ||
                                 NPC.AnyNPCs(ModContent.NPCType<HakuBoss>());
        if (ChallengeRules.CanUseStoryZabuzaScroll(anotherBossActive))
            return true;

        if (player.whoAmI == Main.myPlayer)
            Main.NewText("白或再不斩正在场上，不能重复召唤。", 250, 150, 100);
        return false;
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<ZabuzaBoss>());
        return true;
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient<MistInsignia>(3).AddIngredient(ItemID.Wood, 10)
            .AddTile(TileID.WorkBenches).Register();
    }
}
