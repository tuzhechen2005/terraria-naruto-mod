using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Content.Items;

public sealed class ArenaChallengeScroll : ModItem
{
    public override string Texture => "ShinobiPrototype/Content/Items/ExamAdmissionScroll";

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.rare = ItemRarityID.Green;
    }

    public override bool CanUseItem(Player player) =>
        ExamRules.CanChallenge(player.GetModPlayer<StoryPlayer>().ExamStage, StoryWorld.WaveComplete) &&
        !NPC.AnyNPCs(ModContent.NPCType<ArenaRivalBoss>());

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<ArenaRivalBoss>());
        return true;
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient<HeavenScroll>().AddIngredient<EarthScroll>()
            .AddIngredient(ItemID.Wood, 10).AddIngredient(ItemID.IronBar, 3)
            .AddTile(TileID.WorkBenches).Register();
        CreateRecipe().AddIngredient<HeavenScroll>().AddIngredient<EarthScroll>()
            .AddIngredient(ItemID.Wood, 10).AddIngredient(ItemID.LeadBar, 3)
            .AddTile(TileID.WorkBenches).Register();
    }
}
