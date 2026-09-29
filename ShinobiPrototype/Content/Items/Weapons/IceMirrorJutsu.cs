using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.Buffs;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.Items.Weapons;

// 秘术·魔镜冰晶 (summon, from Zabuza and Haku): a jutsu that raises an ice mirror beside the player, which throws
// fans of senbon at enemies. One minion slot per mirror. First pass (2026-09-29).
public sealed class IceMirrorJutsu : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
        ItemID.Sets.LockOnIgnoresCollision[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 11;
        Item.DamageType = DamageClass.Summon;
        Item.mana = 10;
        Item.width = 32;
        Item.height = 32;
        Item.useTime = 36;
        Item.useAnimation = 36;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.noMelee = true;
        Item.knockBack = 1f;
        Item.UseSound = SoundID.Item28;
        Item.buffType = ModContent.BuffType<IceMirrorBuff>();
        Item.shoot = ModContent.ProjectileType<IceMirrorMinion>();
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(gold: 1);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity,
        int type, int damage, float knockback)
    {
        player.AddBuff(Item.buffType, 2);
        Projectile minion = Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback,
            player.whoAmI);
        minion.originalDamage = Item.damage;
        return false;
    }
}
