using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.Items.NinjaTools;

// 手里剑 (tier one, sold at the ninja tool shop): never used up, glances off walls twice. Its stealth throw is the
// Shadow Shuriken: a great windmill shuriken through several enemies, with a second hidden in its shadow a moment
// behind. Art: ninja-tools-v1; until it is in, vanilla's shuriken.
public sealed class Shuriken : NinjaTool
{
    public override string Texture => ModContent.HasAsset("ShinobiPrototype/Content/Items/NinjaTools/Shuriken")
        ? "ShinobiPrototype/Content/Items/NinjaTools/Shuriken"
        : $"Terraria/Images/Item_{ItemID.Shuriken}";

    public override void SetDefaults()
    {
        Item.damage = 12;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 22;
        Item.height = 22;
        Item.useTime = 16;
        Item.useAnimation = 16;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.knockBack = 2f;
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = true;
        Item.shoot = ModContent.ProjectileType<ShurikenThrown>();
        Item.shootSpeed = 12f;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 40);
    }

    protected override void StealthThrow(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity,
        int type, int damage, float knockback)
    {
        int shadow = ModContent.ProjectileType<ShadowShuriken>();
        Projectile.NewProjectile(source, position, velocity * 1.15f, shadow, (int)(damage * 2.5f), knockback * 1.5f, player.whoAmI);
        // The one hidden in its shadow, a moment behind.
        Projectile.NewProjectile(source, position, velocity * 1.15f, shadow, (int)(damage * 2.5f), knockback * 1.5f, player.whoAmI,
            ShadowShuriken.HiddenTicks);
    }
}

// 起爆符 (tier one, sold at the ninja tool shop): thrown and used up; it sticks to the first enemy or surface it touches
// and goes off a second later. Its stealth throw is three at once in a fan. Art: ninja-tools-v1; until it is in,
// vanilla's sticky grenade.
public sealed class PaperBomb : NinjaTool
{
    public override string Texture => ModContent.HasAsset("ShinobiPrototype/Content/Items/NinjaTools/PaperBomb")
        ? "ShinobiPrototype/Content/Items/NinjaTools/PaperBomb"
        : $"Terraria/Images/Item_{ItemID.StickyGrenade}";

    public override void SetDefaults()
    {
        Item.damage = 45;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 16;
        Item.height = 24;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.knockBack = 5f;
        Item.UseSound = SoundID.Item1;
        Item.consumable = true;
        Item.maxStack = Item.CommonMaxStack;
        Item.shoot = ModContent.ProjectileType<PaperBombThrown>();
        Item.shootSpeed = 9f;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(silver: 3);
    }

    protected override void StealthThrow(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity,
        int type, int damage, float knockback)
    {
        for (int i = -1; i <= 1; i++)
            Projectile.NewProjectile(source, position, velocity.RotatedBy(i * 0.22f), type, damage, knockback, player.whoAmI);
    }
}
