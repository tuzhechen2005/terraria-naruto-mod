using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;

namespace ShinobiPrototype.Content.Items;

public sealed class NinjutsuManual : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.rare = ItemRarityID.Blue;
        Item.UseSound = SoundID.Item4;
    }

    public override bool AltFunctionUse(Player player) => true;

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI != Main.myPlayer)
            return true;

        StoryPlayer story = player.GetModPlayer<StoryPlayer>();
        if (player.altFunctionUse != 2)
        {
            story.CycleManualNature();
            Main.NewText($"忍术研习：{TrainingRules.NatureName(story.ManualNature)}遁。右键学习；本系需 1 颗坠星，跨系需 3 颗，均需 5 木材。", 100, 200, 245);
            return true;
        }

        int nature = story.ManualNature;
        if (story.ChakraNature == 0)
        {
            Main.NewText("先完成木叶基础训练并确定查克拉性质。", 250, 150, 100);
            return true;
        }
        if (story.HasLearned(nature))
        {
            Main.NewText($"已学会{TrainingRules.NatureName(nature)}遁。", 100, 200, 245);
            return true;
        }

        int starCost = TrainingRules.LearningStarCost(story.ChakraNature, nature);
        if (player.CountItem(ItemID.FallenStar) < starCost || player.CountItem(ItemID.Wood) < 5)
        {
            Main.NewText($"学习{TrainingRules.NatureName(nature)}遁需要 {starCost} 颗坠星和 5 木材。", 250, 150, 100);
            return true;
        }

        for (int i = 0; i < starCost; i++)
            player.ConsumeItem(ItemID.FallenStar);
        for (int i = 0; i < 5; i++)
            player.ConsumeItem(ItemID.Wood);
        story.Learn(nature);
        if (player.CountItem(ModContent.ItemType<ElementalTechnique>()) == 0)
            player.QuickSpawnItem(player.GetSource_Misc("ShinobiNinjutsu"), ModContent.ItemType<ElementalTechnique>());
        Main.NewText($"学会{TrainingRules.NatureName(nature)}遁！跨系仅增加学习成本，学成后的招式威力相同。", 100, 200, 245);
        return true;
    }

    public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.Wood, 1).AddTile(TileID.WorkBenches).Register();
}
