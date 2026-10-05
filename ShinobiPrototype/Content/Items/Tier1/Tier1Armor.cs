using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Tiles;

namespace ShinobiPrototype.Content.Items.Tier1;

// Tier one's two armours (specs/装备与忍术系统.spec.md, numbers approved 2026-10-04), made at the tool workbench:
// the Academy training clothes lean on logs and defence, the genin combat gear on stealth and ninja tools.
// Art: armor-academy-v1, armor-genin-v1 (drawn on vanilla's ninja and copper templates).
public abstract class Tier1ArmorPiece : ModItem
{
    protected abstract int Defense { get; }

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 20;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 30);
        Item.defense = Defense;
    }
}

public static class Tier1Armor
{
    public const float AcademyLogRegen = 1.15f;
    public const float GeninToolFill = 1.5f;
    public const float GeninStealthBonus = 0.1f;   // stealth damage 30% → 40%

    // Each piece: rough cloth plus iron or lead (Academy), or plus a rogue headband and silver or tungsten (genin).
    internal static void Academy(ModItem item, int cloth, int bars) =>
        item.CreateRecipe().AddIngredient<RoughCloth>(cloth).AddRecipeGroup(RecipeGroupID.IronBar, bars)
            .AddTile<ToolWorkbenchTile>().Register();

    internal static void Genin(ModItem item, int cloth, int headbands, int bars)
    {
        foreach (int bar in new[] { ItemID.SilverBar, ItemID.TungstenBar })
        {
            Recipe recipe = item.CreateRecipe().AddIngredient<RoughCloth>(cloth).AddIngredient(bar, bars);
            if (headbands > 0)
                recipe.AddIngredient<RogueHeadband>(headbands);
            recipe.AddTile<ToolWorkbenchTile>().Register();
        }
    }
}

[AutoloadEquip(EquipType.Head)]
public sealed class AcademyTrainingHead : Tier1ArmorPiece
{
    public static LocalizedText SetBonus { get; private set; }
    protected override int Defense => 5;

    public override void SetStaticDefaults() => SetBonus = this.GetLocalization("SetBonus");

    public override bool IsArmorSet(Item head, Item body, Item legs) =>
        body.type == ModContent.ItemType<AcademyTrainingBody>() && legs.type == ModContent.ItemType<AcademyTrainingLegs>();

    public override void UpdateArmorSet(Player player)
    {
        player.setBonus = SetBonus.Value;
        var logs = player.GetModPlayer<SubstitutionPlayer>();
        logs.ExtraLogs++;
        logs.LogRegenSpeed *= Tier1Armor.AcademyLogRegen;
    }

    public override void AddRecipes() => Tier1Armor.Academy(this, 4, 6);
}

[AutoloadEquip(EquipType.Body)]
public sealed class AcademyTrainingBody : Tier1ArmorPiece
{
    protected override int Defense => 5;
    public override void AddRecipes() => Tier1Armor.Academy(this, 6, 8);
}

[AutoloadEquip(EquipType.Legs)]
public sealed class AcademyTrainingLegs : Tier1ArmorPiece
{
    protected override int Defense => 4;
    public override void AddRecipes() => Tier1Armor.Academy(this, 5, 7);
}

[AutoloadEquip(EquipType.Head)]
public sealed class GeninCombatHead : Tier1ArmorPiece
{
    public static LocalizedText SetBonus { get; private set; }
    protected override int Defense => 4;

    public override void SetStaticDefaults() => SetBonus = this.GetLocalization("SetBonus");

    public override bool IsArmorSet(Item head, Item body, Item legs) =>
        body.type == ModContent.ItemType<GeninCombatBody>() && legs.type == ModContent.ItemType<GeninCombatLegs>();

    public override void UpdateArmorSet(Player player)
    {
        player.setBonus = SetBonus.Value;
        player.GetModPlayer<ToolStealthPlayer>().FillSpeed *= Tier1Armor.GeninToolFill;
        player.GetModPlayer<StealthPlayer>().ExtraDamageBonus += Tier1Armor.GeninStealthBonus;
    }

    public override void AddRecipes() => Tier1Armor.Genin(this, 3, 1, 6);
}

[AutoloadEquip(EquipType.Body)]
public sealed class GeninCombatBody : Tier1ArmorPiece
{
    protected override int Defense => 4;
    public override void AddRecipes() => Tier1Armor.Genin(this, 5, 1, 8);
}

[AutoloadEquip(EquipType.Legs)]
public sealed class GeninCombatLegs : Tier1ArmorPiece
{
    protected override int Defense => 2;
    public override void AddRecipes() => Tier1Armor.Genin(this, 4, 0, 7);
}
