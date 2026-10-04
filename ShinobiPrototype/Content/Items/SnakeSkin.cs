using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.NPCs;
using ShinobiPrototype.Common;

namespace ShinobiPrototype.Content.Items;

// Orochimaru's shed skin (specs/M2_中忍考试篇.spec.md section 5): used on the jungle surface it calls him. He leaves one
// behind, and it can be made, so a character who missed him or never got him to half can still try. Not used up.
// Placeholder art: vanilla leather.
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
            Main.NewText(Loc.Get("Orochimaru.SkinWrongPlace"), 190, 150, 230);
        return false;
    }

    public override void SetStaticDefaults() => ItemID.Sets.SortingPriorityBossSpawns[Type] = 11;

    public override void AddRecipes() =>
        CreateRecipe().AddIngredient(ItemID.Vine, 3).AddIngredient(ItemID.JungleSpores, 10).AddIngredient(ItemID.Stinger, 5)
            .AddTile(TileID.Anvils).Register();

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Orochimaru>());
        return true;
    }
}
