using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Common.Systems;

namespace ShinobiPrototype.Content.Items;

// Kakashi's recommendation for the Chūnin Exams (specs/M2_中忍考试篇.spec.md): he hands it over once Wave Country is
// done, and again if it is lost. Used inside the Academy it opens the written test.
public sealed class ExamAdmissionScroll : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useTime = 25;
        Item.useAnimation = 25;
        Item.rare = ItemRarityID.Blue;
    }

    public override bool CanUseItem(Player player)
    {
        if (player.whoAmI != Main.myPlayer)
            return false;
        ChuninExamPlayer exam = player.GetModPlayer<ChuninExamPlayer>();
        string refusal = exam.Stage switch
        {
            ExamStage.Locked => "先完成波之国的任务。",
            ExamStage.NoVillage => "这个世界没有木叶隐村，中忍考试需要新建的世界。",
            ExamStage.Written when !exam.CanSitWritten => "你在第十题放弃了。等明天天亮再来重考。",
            ExamStage.Written when !KonohaWorld.InBuilding("忍者学校", player.Center) =>
                "第一试在木叶的忍者学校举行（阿吽之门往西）。进了学校再用。",
            ExamStage.Written => null,
            _ => "第一试已经合格了。手册的任务页写着下一步。",
        };
        if (refusal != null)
        {
            Main.NewText(refusal, 250, 200, 120);
            return false;
        }
        return true;
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
            WrittenExamSystem.Open();
        return true;
    }
}
