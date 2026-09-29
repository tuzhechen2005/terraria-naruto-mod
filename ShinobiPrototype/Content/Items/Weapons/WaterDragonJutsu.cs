using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.Items.Weapons;

// 水遁·水龙弹之术 (magic, from Zabuza and Haku): a jutsu, not a staff. A water dragon that curls towards enemies and
// passes through several. Uses mana. First pass (2026-09-29).
public sealed class WaterDragonJutsu : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 28;
        Item.DamageType = DamageClass.Magic;
        Item.mana = 12;
        Item.width = 32;
        Item.height = 32;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.noMelee = true;
        Item.knockBack = 4f;
        Item.autoReuse = true;
        Item.UseSound = SoundID.Item21;
        Item.shoot = ModContent.ProjectileType<WaterDragonFriendly>();
        Item.shootSpeed = 8f;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(gold: 1);
    }
}
