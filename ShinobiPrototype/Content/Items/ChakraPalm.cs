using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.Items;

public sealed class ChakraPalm : ModItem
{
    private const int ChakraCost = 18;

    public override void SetDefaults()
    {
        Item.damage = 32;
        Item.DamageType = DamageClass.Magic;
        Item.width = 24;
        Item.height = 24;
        Item.useTime = 27;
        Item.useAnimation = 27;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.knockBack = 2f;
        Item.UseSound = SoundID.Item20;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.shoot = ModContent.ProjectileType<RasenganHitbox>();
        Item.shootSpeed = 0f;
        Item.rare = ItemRarityID.White;
    }

    public override bool CanUseItem(Player player) => player.GetModPlayer<ChakraPlayer>().Chakra >= ChakraCost;

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source,
        Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (!player.GetModPlayer<ChakraPlayer>().TrySpend(ChakraCost))
            return false;

        Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback,
            player.whoAmI, player.direction, player.GetModPlayer<StoryPlayer>().ChakraNature);
        return false;
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.FallenStar, 1).AddIngredient(ItemID.Wood, 5)
            .AddTile(TileID.WorkBenches).Register();
    }
}
