using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.Items;

// The starting kunai: a thrown ranged weapon that is never used up (M1 spec), and a ninja tool (NinjaTool): its
// stealth throw is three kunai at once in a fan. Tier one, below vanilla throwing knives: never used up (user, 2026-10-03).
public sealed class TrainingKunai : NinjaTools.NinjaTool
{
    public override void SetDefaults()
    {
        Item.damage = 7;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 18;
        Item.height = 18;
        Item.useTime = 22;
        Item.useAnimation = 22;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.knockBack = 2f;
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = true;
        Item.shoot = ModContent.ProjectileType<KunaiThrown>();
        Item.shootSpeed = 11f;
        Item.rare = ItemRarityID.White;
    }

    protected override void StealthThrow(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
        Microsoft.Xna.Framework.Vector2 position, Microsoft.Xna.Framework.Vector2 velocity, int type, int damage, float knockback)
    {
        for (int i = -1; i <= 1; i++)
            Projectile.NewProjectile(source, position, velocity.RotatedBy(i * 0.14f) * 1.1f, type, damage, knockback, player.whoAmI);
    }
}
