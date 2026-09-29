using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Common.Systems;

namespace ShinobiPrototype.Content.Items;

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
        Item.UseSound = SoundID.Item4;
    }

    public override bool CanUseItem(Player player)
    {
        if (!StoryWorld.WaveComplete)
        {
            if (player.whoAmI == Main.myPlayer)
                Main.NewText("白和再不斩都击败后，才能报名中忍考试。", 250, 150, 100);
            return false;
        }
        if (player.GetModPlayer<StoryPlayer>().ExamStage != 0)
        {
            if (player.whoAmI == Main.myPlayer)
                Main.NewText("你已经报名。使用任务卷轴查看当前考试目标。", 100, 200, 245);
            return false;
        }
        return true;
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer && player.GetModPlayer<StoryPlayer>().RegisterExam())
            Main.NewText("中忍考试报名完成。前往丛林：地表寻天卷轴，地下寻地卷轴。", 100, 220, 160);
        return true;
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.Wood, 5).AddIngredient(ItemID.IronBar)
            .AddTile(TileID.WorkBenches).Register();
        CreateRecipe().AddIngredient(ItemID.Wood, 5).AddIngredient(ItemID.LeadBar)
            .AddTile(TileID.WorkBenches).Register();
    }
}
