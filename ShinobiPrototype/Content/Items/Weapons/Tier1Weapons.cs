using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Buffs;
using ShinobiPrototype.Content.Items.Tier1;
using ShinobiPrototype.Content.Projectiles;
using ShinobiPrototype.Content.Tiles;

namespace ShinobiPrototype.Content.Items.Weapons;

// Tier one's weapons (specs/装备与忍术系统.spec.md, "第 1 档数值"; benchmarks there: Platinum Broadsword about 56 a
// second, Enchanted Sword 66, Wand of Sparking 32).

// 修行忍刀: a three-swing string; the third swing sends a slash arc (0.8x, through enemies); hidden in stealth, the arc
// is twice as strong and larger. About 47 a second, 55 with the arcs.
public sealed class TrainingNinjato : ModItem
{
    public const int ComboLength = 3;
    public const float ArcShare = 0.8f;
    public const float StealthArcShare = 1.6f;

    public override void SetDefaults()
    {
        Item.damage = 14;
        Item.DamageType = DamageClass.Melee;
        Item.width = 40;
        Item.height = 40;
        Item.useTime = 18;
        Item.useAnimation = 18;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 5f;
        Item.autoReuse = true;
        Item.UseSound = SoundID.Item1;
        Item.shoot = ModContent.ProjectileType<NinjatoSlashArc>();
        Item.shootSpeed = 9f;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 40);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type,
        int damage, float knockback)
    {
        if (!player.GetModPlayer<NinjatoPlayer>().Swing())
            return false;
        bool hidden = player.GetModPlayer<StealthPlayer>().Hidden;
        Projectile.NewProjectile(source, player.Center, velocity, type, (int)(damage * (hidden ? StealthArcShare : ArcShare)),
            knockback, player.whoAmI, hidden ? 1f : 0f);
        return false;
    }

    public override void AddRecipes() =>
        CreateRecipe().AddRecipeGroup(RecipeGroupID.IronBar, 10).AddIngredient<RoughCloth>(2).AddIngredient<ToolBlueprint>()
            .AddTile<ToolWorkbenchTile>().Register();
}

// Counts the swings of the training ninjato; the string starts again after a short pause.
public sealed class NinjatoPlayer : ModPlayer
{
    private const int ResetTicks = 45;
    private int swings;
    private uint lastSwing;

    // True on the swing that finishes the string.
    public bool Swing()
    {
        if (Main.GameUpdateCount - lastSwing > ResetTicks)
            swings = 0;
        lastSwing = Main.GameUpdateCount;
        swings = swings % TrainingNinjato.ComboLength + 1;
        return swings == TrainingNinjato.ComboLength;
    }
}

// 鬼之兄弟的锁链手甲 (the Demon Brothers): a clawed gauntlet on a chain. The claw hooks the first enemy, drags it toward
// the player and poisons it; thrown at a wall, it pulls the player there (a short reach). About 38 a second.
public sealed class DemonChainGauntlet : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 18;
        Item.DamageType = DamageClass.Melee;
        Item.width = 36;
        Item.height = 36;
        Item.useTime = 28;
        Item.useAnimation = 28;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.knockBack = 4f;
        Item.UseSound = SoundID.Item1;
        Item.shoot = ModContent.ProjectileType<GauntletClaw>();
        Item.shootSpeed = 15f;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 60);
    }

    public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;
}

// 火遁·凤仙火之术: a fan of five small fireballs that may set enemies alight. About 35 a second when three land.
public sealed class PhoenixFlowerJutsu : ModItem
{
    public const int Count = 5;
    public const float Spread = 0.3f;

    public override void SetDefaults()
    {
        Item.damage = 7;
        Item.DamageType = DamageClass.Magic;
        Item.mana = 6;
        Item.width = 28;
        Item.height = 28;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;
        Item.knockBack = 1.5f;
        Item.UseSound = SoundID.Item20;
        Item.shoot = ModContent.ProjectileType<PhoenixFlowerFire>();
        Item.shootSpeed = 9f;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 50);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type,
        int damage, float knockback)
    {
        for (int i = 0; i < Count; i++)
        {
            float angle = MathHelper.Lerp(-Spread, Spread, i / (float)(Count - 1)) + Main.rand.NextFloat(-0.05f, 0.05f);
            Projectile.NewProjectile(source, position, velocity.RotatedBy(angle) * Main.rand.NextFloat(0.85f, 1.1f), type, damage,
                knockback, player.whoAmI);
        }
        return false;
    }
}

// 通灵·忍犬 (Kakashi's lesson): Pakkun runs the enemies down and bites; a bitten enemy carries his scent for 5 seconds
// and takes 3 more from every minion hit (like a whip's tag).
public sealed class NinkenScroll : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
        ItemID.Sets.LockOnIgnoresCollision[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 8;
        Item.DamageType = DamageClass.Summon;
        Item.mana = 10;
        Item.width = 28;
        Item.height = 28;
        Item.useTime = 36;
        Item.useAnimation = 36;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.noMelee = true;
        Item.knockBack = 2f;
        Item.UseSound = SoundID.Item44;
        Item.buffType = ModContent.BuffType<PakkunBuff>();
        Item.shoot = ModContent.ProjectileType<PakkunMinion>();
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 50);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type,
        int damage, float knockback)
    {
        player.AddBuff(Item.buffType, 2);
        Projectile minion = Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
        minion.originalDamage = Item.damage;
        SoundEngine.PlaySound(SoundID.Item8, Main.MouseWorld);
        for (int i = 0; i < 20; i++)
            Dust.NewDust(Main.MouseWorld - new Vector2(16f), 32, 32, DustID.Smoke, 0f, -1f, 100, default, 1.4f);
        return false;
    }
}
