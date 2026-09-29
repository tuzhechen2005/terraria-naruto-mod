using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.Items;

public sealed class TrainingRasengan : ModItem
{
    public override string Texture => $"Terraria/Images/Item_{ItemID.MagicMissile}";

    public override void SetDefaults()
    {
        Item.damage = 75;
        Item.DamageType = DamageClass.Magic;
        Item.width = 24;
        Item.height = 24;
        Item.useTime = 28;
        Item.useAnimation = 28;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.knockBack = 5f;
        Item.UseSound = SoundID.Item20;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.shoot = ModContent.ProjectileType<RasenganHitbox>();
        Item.shootSpeed = 0f;
        Item.rare = ItemRarityID.Blue;
    }

    public override bool CanUseItem(Player player)
    {
        return player.GetModPlayer<ChakraPlayer>().Chakra >= ChakraPlayer.RasenganCost;
    }

    public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
        Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (!player.GetModPlayer<ChakraPlayer>().TrySpend(ChakraPlayer.RasenganCost))
            return false;

        Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback,
            player.whoAmI, player.direction);
        return false;
    }
}
