using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.NPCs;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Common.Systems;

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
        float tilesFromBridge = WaveBridgeWorld.DistanceToBridgeTiles(player.Center);
        if (ChallengeRules.CanUseStoryZabuzaScroll(anotherBossActive, tilesFromBridge))
            return true;

        if (player.whoAmI == Main.myPlayer)
            Main.NewText(Loc.Get(anotherBossActive ? "Wave.AlreadyFighting"
                : !WaveBridgeWorld.Site.HasValue ? "Wave.NoBridge" : "Wave.ScrollWrongPlace"), 250, 150, 100);
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
        // Craftable once the lake ambush is over and this character has seen the pair in the mist at the bridge.
        Condition story = new(Mod.GetLocalization("Conditions.WaveScroll"), () =>
            StoryRules.ScrollCraftable(StoryWorld.WaveComplete, StoryWorld.LakeDone,
                Main.LocalPlayer.GetModPlayer<MistEncounterPlayer>().SawPreview, StoryWorld.ZabuzaFought));
        CreateRecipe().AddIngredient<MistInsignia>(3).AddIngredient(ItemID.Wood, 10)
            .AddTile(TileID.WorkBenches).AddCondition(story).Register();
    }
}
