using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Items;

// Kakashi's recommendation for the Chūnin Exams (specs/M2_中忍考试篇.spec.md): he hands it over once Wave Country is
// done, and again if it is lost. It is the pass Morino Ibiki asks for at the Academy before the written test.
public sealed class ExamAdmissionScroll : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.rare = ItemRarityID.Blue;
    }
}
