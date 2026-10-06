using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Tiles;

namespace ShinobiPrototype.Content.Items.Tier1;

// Tier one's materials, accessories and the tool workbench (specs/装备与忍术系统.spec.md, "第 1 档数值"). Until an
// item's own art is in (tier1-items-v2), it borrows a vanilla item's picture.
public abstract class Tier1Item : ModItem
{
    protected abstract int StandIn { get; }
    protected virtual string StandInPath => $"Terraria/Images/Item_{StandIn}";

    public override string Texture => ModContent.HasAsset($"ShinobiPrototype/Content/Items/Tier1/{Name}")
        ? $"ShinobiPrototype/Content/Items/Tier1/{Name}"
        : StandInPath;
}

// --- Materials

public sealed class RoughCloth : Tier1Item
{
    protected override int StandIn => ItemID.Silk;

    public override void SetStaticDefaults() => Item.ResearchUnlockCount = 25;

    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 20;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.White;
        Item.value = Item.sellPrice(copper: 20);
    }
}

public sealed class RogueHeadband : Tier1Item
{
    protected override int StandIn => ItemID.NinjaHood;

    public override void SetStaticDefaults() => Item.ResearchUnlockCount = 25;

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 16;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 1);
    }
}

// One is used up in each recipe at the tool workbench.
public sealed class ToolBlueprint : Tier1Item
{
    protected override int StandIn => ItemID.Book;

    public override void SetStaticDefaults() => Item.ResearchUnlockCount = 25;

    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 24;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(silver: 50);
    }
}

// Paid for missions at the mission desk, and spent there (Iruka's exchange).
public sealed class MissionToken : Tier1Item
{
    protected override int StandIn => ItemID.DefenderMedal;

    public override void SetStaticDefaults() => Item.ResearchUnlockCount = 50;

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 24;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.Blue;
    }
}

// --- Accessories

public abstract class Tier1Accessory : Tier1Item
{
    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 28;
        Item.accessory = true;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 50);
    }
}

public sealed class KonohaHeadband : Tier1Accessory
{
    public const float Damage = 0.04f;
    public const int Chakra = 20;

    protected override int StandIn => 0;
    protected override string StandInPath => "ShinobiPrototype/Content/Items/ChuninHeadband";

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetDamage(DamageClass.Generic) += Damage;
        player.GetModPlayer<ChakraPlayer>().ExtraMaxChakra += Chakra;
    }

    public override void AddRecipes() =>
        CreateRecipe().AddIngredient(ItemID.Silk, 3).AddRecipeGroup(RecipeGroupID.IronBar, 4).AddIngredient<RogueHeadband>()
            .AddTile<ToolWorkbenchTile>().Register();
}

public sealed class ToolPouch : Tier1Accessory
{
    public const float FillSpeed = 1.25f;
    public const float Crit = 4f;

    protected override int StandIn => ItemID.TreasureMagnet;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.value = Item.buyPrice(gold: 3);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        ToolStealthPlayer tools = player.GetModPlayer<ToolStealthPlayer>();
        tools.FillSpeed *= FillSpeed;
        tools.ToolCrit += Crit;
    }
}

public sealed class PillPouch : Tier1Accessory
{
    public const float Regen = 1.3f;
    public const int SicknessTicks = 6 * 60;

    protected override int StandIn => ItemID.HerbBag;

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        ChakraPlayer chakra = player.GetModPlayer<ChakraPlayer>();
        chakra.RegenMultiplier *= Regen;
        chakra.PillSicknessTicks = System.Math.Min(chakra.PillSicknessTicks, SicknessTicks);
    }

    public override void AddRecipes() =>
        CreateRecipe().AddIngredient<ChakraPill>(10).AddIngredient(ItemID.Leather, 2).AddTile<ToolWorkbenchTile>().Register();
}

public sealed class TrainingPost : Tier1Accessory
{
    protected override int StandIn => ItemID.TargetDummy;

    public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<SubstitutionPlayer>().ExtraLogs++;
}

// --- The tool workbench

public sealed class ToolWorkbench : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<ToolWorkbenchTile>());
        Item.width = 32;
        Item.height = 24;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(gold: 1);
    }
}
