using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.DamageClasses;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.Items.Taijutsu;

// Taijutsu weapons (specs/流派系统.spec.md, "体术武器"): every punch throws a wave of air pressure 10–15 tiles, harder
// up close; right click is the Leaf Whirlwind, a dash kick through enemies with a moment of invulnerability. The open
// Eight Gates make the pressure bigger and carry it further.
public abstract class TaijutsuWeapon : ModItem
{
    protected abstract int Damage { get; }
    protected abstract int UseTime { get; }
    protected abstract float PressureSpeed { get; }
    protected abstract int PressureTicks { get; }

    public override void SetDefaults()
    {
        Item.DamageType = ModContent.GetInstance<TaijutsuDamage>();
        Item.damage = Damage;
        Item.knockBack = 3f;
        Item.useStyle = ItemUseStyleID.Rapier;
        Item.useTime = UseTime;
        Item.useAnimation = UseTime;
        Item.autoReuse = true;
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.shoot = ModContent.ProjectileType<TaijutsuPressure>();
        Item.shootSpeed = PressureSpeed;
        Item.UseSound = SoundID.Item7;
        Item.width = 24;
        Item.height = 24;
    }

    public override bool AltFunctionUse(Player player) => true;

    public override bool CanUseItem(Player player) =>
        player.altFunctionUse != 2 || player.GetModPlayer<TaijutsuPlayer>().KickReady;

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity,
        int type, int damage, float knockback)
    {
        if (player.altFunctionUse == 2)
        {
            player.GetModPlayer<TaijutsuPlayer>().Kick(source, damage);
            return false;
        }
        int gates = player.GetModPlayer<StyleCorePlayer>().Gates;
        Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI,
            StyleCoreRules.PressureScale(gates), PressureTicks + 4 * gates);
        return false;
    }
}

// Training wraps: the first taijutsu weapon, made at a workbench.
public sealed class TrainingWraps : TaijutsuWeapon
{
    public override string Texture => $"Terraria/Images/Item_{ItemID.Silk}";
    protected override int Damage => 14;
    protected override int UseTime => 15;
    protected override float PressureSpeed => 10f;
    protected override int PressureTicks => 20;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 20);
    }

    public override void AddRecipes() =>
        CreateRecipe().AddRecipeGroup(RecipeGroupID.Wood, 10).AddIngredient(ItemID.Silk, 3).AddTile(TileID.WorkBenches).Register();
}

// Rock Lee's leg weights, from Gaara: faster, harder, further.
public sealed class LeeLegWeights : TaijutsuWeapon
{
    public override string Texture => $"Terraria/Images/Item_{ItemID.HermesBoots}";
    protected override int Damage => 30;
    protected override int UseTime => 12;
    protected override float PressureSpeed => 12f;
    protected override int PressureTicks => 22;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.knockBack = 4f;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(gold: 1);
    }
}

// The Leaf Whirlwind's cooldown and the dash itself.
public sealed class TaijutsuPlayer : ModPlayer
{
    private const int KickCooldownTicks = 90;
    private const int KickTicks = 16;
    private const float KickSpeed = 13f;

    private int cooldown;

    public bool KickReady => cooldown <= 0;

    public void Kick(IEntitySource source, int damage)
    {
        cooldown = KickCooldownTicks;
        int direction = Main.MouseWorld.X >= Player.Center.X ? 1 : -1;
        Player.direction = direction;
        Player.velocity.X = direction * KickSpeed;
        Player.SetImmuneTimeForAllTypes(KickTicks + 4);
        Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ModContent.ProjectileType<WhirlwindKick>(),
            (int)(damage * 1.5f), 6f, Player.whoAmI, KickTicks);
        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item1, Player.Center);
    }

    public override void PostUpdate()
    {
        if (cooldown > 0)
            cooldown--;
    }
}
