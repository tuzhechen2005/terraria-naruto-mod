using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Content.Items;

// Orochimaru's shed skin (specs/M2_中忍考试篇.spec.md section 5): used on the jungle surface it calls him back for
// another try. Not used up. Placeholder art: vanilla leather.
public sealed class SnakeSkin : ModItem
{
    public override string Texture => $"Terraria/Images/Item_{ItemID.Leather}";

    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 24;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useTime = 45;
        Item.useAnimation = 45;
        Item.rare = ItemRarityID.Orange;
    }

    public override bool CanUseItem(Player player)
    {
        if (NPC.AnyNPCs(ModContent.NPCType<Orochimaru>()))
            return false;
        if (player.ZoneJungle && player.ZoneOverworldHeight)
            return true;
        if (player.whoAmI == Main.myPlayer)
            Main.NewText("蛇蜕上还留着那股气味……要在丛林的地表才能把他引出来。", 190, 150, 230);
        return false;
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Orochimaru>());
        return true;
    }
}
