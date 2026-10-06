using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Content.Items;

// Neji's challenge (specs/M2_中忍考试篇.spec.md 3.5): Kakashi passes it on once the finals are open. Used on the stadium
// field it starts the sparring bout; not used up, so the bout can be fought again.
public sealed class NejiChallengeScroll : ModItem
{
    public override string Texture => "ShinobiPrototype/Content/Items/ExamAdmissionScroll";

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useTime = 45;
        Item.useAnimation = 45;
        Item.rare = ItemRarityID.Orange;
    }

    public override bool CanUseItem(Player player)
    {
        if (NPC.AnyNPCs(ModContent.NPCType<Neji>()) || NPC.AnyNPCs(ModContent.NPCType<Gaara>()))
            return false;
        ExamStage stage = player.GetModPlayer<ChuninExamPlayer>().Stage;
        string refusal = stage < ExamStage.Finals ? Loc.Get("Neji.TooEarly")
            : !ExamSiteWorld.InArena(ExamSiteWorld.Stadium, player.Center) ? Loc.Get("Neji.WrongPlace")
            : null;
        if (refusal == null)
            return true;
        if (player.whoAmI == Main.myPlayer)
            Main.NewText(refusal, 200, 220, 255);
        return false;
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Neji>());
        return true;
    }
}
