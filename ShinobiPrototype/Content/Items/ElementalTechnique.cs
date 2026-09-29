using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.Items;

public sealed class ElementalTechnique : ModItem
{
    private const int ChakraCost = 22;
    public override string Texture => "ShinobiPrototype/Content/Items/ChakraPalm";

    public override void SetDefaults()
    {
        Item.damage = 27;
        Item.DamageType = DamageClass.Magic;
        Item.width = 24;
        Item.height = 24;
        Item.useTime = 28;
        Item.useAnimation = 28;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 3f;
        Item.UseSound = SoundID.Item20;
        Item.noMelee = true;
        Item.shoot = ModContent.ProjectileType<ElementalBolt>();
        Item.shootSpeed = 8f;
        Item.rare = ItemRarityID.Blue;
    }

    public override bool AltFunctionUse(Player player) => true;

    public override bool CanUseItem(Player player) => player.altFunctionUse == 2 ||
        (player.GetModPlayer<StoryPlayer>().HasLearned(player.GetModPlayer<StoryPlayer>().ActiveNature) &&
         player.GetModPlayer<ChakraPlayer>().Chakra >= ChakraCost);

    public override bool? UseItem(Player player)
    {
        if (player.altFunctionUse != 2 || player.whoAmI != Main.myPlayer)
            return null;
        StoryPlayer story = player.GetModPlayer<StoryPlayer>();
        story.CycleActiveNature();
        Main.NewText($"当前忍术：{TrainingRules.NatureName(story.ActiveNature)}遁。左键施术消耗 {ChakraCost} 查克拉。", 100, 200, 245);
        return true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source,
        Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (player.altFunctionUse == 2 || !player.GetModPlayer<ChakraPlayer>().TrySpend(ChakraCost))
            return false;
        int nature = player.GetModPlayer<StoryPlayer>().ActiveNature;
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, nature);
        return false;
    }
}
