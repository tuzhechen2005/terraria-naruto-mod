using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;

namespace ShinobiPrototype.Content.Items;

public sealed class NinjaProfileScroll : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.rare = ItemRarityID.White;
        Item.UseSound = SoundID.Item4;
    }

    public override bool AltFunctionUse(Player player) => true;

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI != Main.myPlayer)
            return true;

        StoryPlayer story = player.GetModPlayer<StoryPlayer>();
        if (story.Stage > 0)
        {
            Main.NewText($"已确定：{TrainingRules.StyleName(story.CombatStyle)}倾向，{TrainingRules.NatureName(story.ChakraNature)}属性。", 100, 200, 245);
            return true;
        }

        if (player.altFunctionUse == 2)
            story.CycleNature();
        else
            story.CycleStyle();
        Main.NewText($"战斗倾向：{TrainingRules.StyleName(story.CombatStyle)}；查克拉性质：{TrainingRules.NatureName(story.ChakraNature)}。左键切换倾向，右键切换性质；选好后用苦无完成训练并锁定。", 100, 200, 245);
        return true;
    }

    public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.Wood, 1).Register();
}
