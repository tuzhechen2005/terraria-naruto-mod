using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Content.Items;

public sealed class HakuChallengeScroll : ModItem
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
        return ChallengeRules.CanUseHakuScroll(
            NPC.AnyNPCs(ModContent.NPCType<HakuBoss>()) ||
            NPC.AnyNPCs(ModContent.NPCType<ZabuzaBoss>()));
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<ZabuzaBoss>());
        return true;
    }

    public override void UpdateInventory(Player player)
    {
        int stack = Item.stack;
        Item.SetDefaults(ModContent.ItemType<ZabuzaChallengeScroll>());
        Item.stack = stack;
    }
}
