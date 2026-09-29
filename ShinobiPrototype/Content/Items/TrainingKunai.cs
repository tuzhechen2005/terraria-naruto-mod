using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.Items;

// The starting kunai: a thrown ranged weapon that is never used up (M1 spec).
public sealed class TrainingKunai : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 9;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 18;
        Item.height = 18;
        Item.useTime = 18;
        Item.useAnimation = 18;
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
}
