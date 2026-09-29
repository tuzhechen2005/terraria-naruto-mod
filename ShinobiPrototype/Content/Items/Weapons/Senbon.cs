using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.Items.Weapons;

// 千本 (ranged, from Zabuza and Haku): a fan of three needles per throw, never used up. First pass (2026-09-29).
public sealed class Senbon : ModItem
{
    private const int Needles = 3;
    private const float SpreadRadians = 0.1f;

    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    public override void SetDefaults()
    {
        Item.damage = 10;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 28;
        Item.height = 28;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.knockBack = 1.5f;
        Item.autoReuse = true;
        Item.UseSound = SoundID.Item1;
        Item.shoot = ModContent.ProjectileType<SenbonThrown>();
        Item.shootSpeed = 13f;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(gold: 1);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity,
        int type, int damage, float knockback)
    {
        for (int i = 0; i < Needles; i++)
        {
            float angle = (i - (Needles - 1) / 2f) * SpreadRadians;
            Projectile.NewProjectile(source, position, velocity.RotatedBy(angle), type, damage, knockback, player.whoAmI);
        }
        return false;
    }
}
