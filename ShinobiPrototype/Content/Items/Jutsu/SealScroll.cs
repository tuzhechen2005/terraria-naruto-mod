using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.Items.Jutsu;

// A jutsu scroll (specs/装备与忍术系统.spec.md): kept in the seal slot of its number of seals beside the inventory
// (SealSlotsUI), cast with that slot's key (SealPlayer). Its damage is general damage: all-damage bonuses and
// the ninjutsu power from Naruto gear apply, no single class. Art: seal-jutsu-v1 (until it is in, the mission scroll).
public abstract class SealScroll : ModItem
{
    public abstract int Seals { get; }
    // The hand seals shown overhead as they form (zodiac seals).
    public abstract string Signs { get; }
    public abstract int ChakraCost { get; }
    // Time before it can be cast again (the great techniques).
    public virtual int CooldownTicks => 0;
    protected abstract int BaseDamage { get; }
    protected abstract string ArtName { get; }

    public override string Texture => ModContent.HasAsset($"ShinobiPrototype/Content/Items/Jutsu/{ArtName}")
        ? $"ShinobiPrototype/Content/Items/Jutsu/{ArtName}"
        : "ShinobiPrototype/Content/Items/MissionScroll";

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 28;
        Item.damage = BaseDamage;
        Item.DamageType = DamageClass.Generic;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 50);
    }

    // The damage as cast now.
    public int Damage(Player player) =>
        BaseDamage <= 0 ? 0 : (int)(player.GetTotalDamage(DamageClass.Generic).ApplyTo(BaseDamage) *
                                    player.GetModPlayer<SealPlayer>().NinjutsuPower);

    public abstract void Cast(Player player);
}

// 分身术 (two seals): two shadowy copies of the player for a while; each takes one hit in the player's place, before
// any log (SubstitutionPlayer), warned binds included.
public sealed class ScrollClone : SealScroll
{
    public override int Seals => 2;
    public override string Signs => "未巳";
    public override int ChakraCost => 15;
    protected override int BaseDamage => 0;
    protected override string ArtName => "ScrollClone";

    public override void Cast(Player player)
    {
        foreach (Projectile p in Main.ActiveProjectiles)
            if (p.owner == player.whoAmI && p.type == ModContent.ProjectileType<ShadowClone>())
                p.Kill();
        for (int side = -1; side <= 1; side += 2)
            Projectile.NewProjectile(player.GetSource_Misc("SealJutsu"), player.Center, Vector2.Zero,
                ModContent.ProjectileType<ShadowClone>(), 0, 0f, player.whoAmI, side);
    }
}

// 火遁·豪火球之术 (four seals): a great fireball towards the cursor that swells to thirteen tiles, rolls through walls
// burning what it touches and the ground below, and bursts at the end (SealRules).
public sealed class ScrollFireball : SealScroll
{
    public override int Seals => 4;
    public override string Signs => "巳未申亥";
    public override int ChakraCost => SealRules.FireballCost;
    public override int CooldownTicks => SealRules.FireballCooldownTicks;
    protected override int BaseDamage => 32;
    protected override string ArtName => "ScrollFireball";

    public override void Cast(Player player)
    {
        if (player.whoAmI != Main.myPlayer)
            return;
        Vector2 aim = player.DirectionTo(Main.MouseWorld);
        Projectile.NewProjectile(player.GetSource_Misc("SealJutsu"), player.Center + aim * 24f, aim * SealRules.FireballSpeed,
            ModContent.ProjectileType<GreatFireball>(), Damage(player), 5f, player.whoAmI);
    }
}

// 千鸟 (six seals): lightning gathers in the hand, then a forty-tile charge towards the cursor, through small enemies and
// into the first boss, leaving lightning chakra on the way (SealRules).
public sealed class ScrollChidori : SealScroll
{
    public override int Seals => 6;
    public override string Signs => "丑卯申卯申卯";
    public override int ChakraCost => SealRules.ChidoriCost;
    public override int CooldownTicks => SealRules.ChidoriCooldownTicks;
    protected override int BaseDamage => 120;
    protected override string ArtName => "ScrollChidori";

    public override void Cast(Player player)
    {
        if (player.whoAmI != Main.myPlayer)
            return;
        Vector2 aim = player.DirectionTo(Main.MouseWorld);
        Projectile.NewProjectile(player.GetSource_Misc("SealJutsu"), player.Center, aim,
            ModContent.ProjectileType<ChidoriCharge>(), Damage(player), 8f, player.whoAmI);
    }
}
