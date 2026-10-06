using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Content.Items;

// Rematches for the exam bouts (master spec, "可玩性与引导": every boss can be fought again with a summon). Each is
// used where its bout is held, once the character has reached that bout in the exams. Not used up.
public abstract class ExamRematchItem : ModItem
{
    protected abstract int BossType { get; }
    protected abstract ExamSite? Site { get; }
    protected abstract ExamStage ReachedAt { get; }
    protected abstract string WrongPlace { get; }
    protected abstract string TooEarly { get; }

    public override void SetStaticDefaults() => ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;

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
        if (NPC.AnyNPCs(BossType))
            return false;
        string refusal = player.GetModPlayer<ChuninExamPlayer>().Stage < ReachedAt ? TooEarly
            : !ExamSiteWorld.InArena(Site, player.Center) ? WrongPlace
            : null;
        if (refusal == null)
            return true;
        if (player.whoAmI == Main.myPlayer)
            Main.NewText(refusal, 250, 200, 120);
        return false;
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
            NPC.SpawnOnPlayer(player.whoAmI, BossType);
        return true;
    }
}

// Dosu again, in the tower hall.
public sealed class SoundNinjaToken : ExamRematchItem
{
    public override string Texture => "ShinobiPrototype/Content/Items/ExamAdmissionScroll";
    protected override int BossType => ModContent.NPCType<Dosu>();
    protected override ExamSite? Site => ExamSiteWorld.Tower;
    protected override ExamStage ReachedAt => ExamStage.Prelims;
    protected override string WrongPlace => Loc.Get("Dosu.WrongPlace");
    protected override string TooEarly => Loc.Get("Dosu.TooEarly");

    public override void AddRecipes()
    {
        CreateRecipe().AddRecipeGroup(RecipeGroupID.IronBar, 5).AddIngredient(ItemID.ShadowScale, 5)
            .AddTile(TileID.Anvils).Register();
        CreateRecipe().AddRecipeGroup(RecipeGroupID.IronBar, 5).AddIngredient(ItemID.TissueSample, 5)
            .AddTile(TileID.Anvils).Register();
    }
}

// Gaara again, on the stadium field.
public sealed class SandGourd : ExamRematchItem
{
    public override string Texture => $"Terraria/Images/Item_{ItemID.ClayPot}";
    protected override int BossType => ModContent.NPCType<Gaara>();
    protected override ExamSite? Site => ExamSiteWorld.Stadium;
    protected override ExamStage ReachedAt => ExamStage.Finals;
    protected override string WrongPlace => Loc.Get("Gaara.WrongPlace");
    protected override string TooEarly => Loc.Get("Gaara.TooEarly");

    public override void AddRecipes() =>
        CreateRecipe().AddIngredient(ItemID.SandBlock, 50).AddIngredient(ItemID.Bone, 10)
            .AddTile(TileID.Anvils).Register();
}
