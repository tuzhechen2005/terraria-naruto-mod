using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Projectiles;

// The first seal jutsu (specs/装备与忍术系统.spec.md). Art: seal-jutsu-v1 (FxGreatFireball, FxFireBurst, FxChidori,
// FxChidoriTrail); until it is in, dust and vanilla flame stand in.

// 分身术: a shadowy copy of the player at one side (ai[0] = -1 or 1), drawn from the player themself. It takes one hit
// in the player's place (SubstitutionPlayer asks TakeHit first), then goes up in smoke.
public sealed class ShadowClone : ModProjectile
{
    public const int Lifetime = 8 * 60;

    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    public override void SetDefaults()
    {
        Projectile.width = 20;
        Projectile.height = 42;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = Lifetime;
        Projectile.aiStyle = -1;
    }

    public override void AI()
    {
        Player owner = Main.player[Projectile.owner];
        if (!owner.active || owner.dead)
        {
            Projectile.Kill();
            return;
        }
        if (Projectile.localAI[0] == 0f)
        {
            Projectile.localAI[0] = 1f;
            Puff(Projectile.Center);
        }
        // Beside the player, a step off, drifting a little.
        float side = Projectile.ai[0];
        Vector2 want = owner.Center + new Vector2(side * 40f, (float)Math.Sin((Main.GameUpdateCount + side * 30f) * 0.07f) * 3f);
        Projectile.Center = Vector2.Lerp(Projectile.Center, want, 0.25f);
    }

    // A clone of this player takes the hit: true if there was one.
    public static bool TakeHit(Player player)
    {
        int type = ModContent.ProjectileType<ShadowClone>();
        foreach (Projectile p in Main.ActiveProjectiles)
            if (p.owner == player.whoAmI && p.type == type)
            {
                p.Kill();
                player.SetImmuneTimeForAllTypes(40);
                SoundEngine.PlaySound(SoundID.Item8 with { Pitch = 0.4f }, p.Center);
                CombatText.NewText(player.getRect(), new Color(170, 200, 255), "分身！");
                return true;
            }
        return false;
    }

    public override void OnKill(int timeLeft) => Puff(Projectile.Center);

    private static void Puff(Vector2 at)
    {
        if (Main.dedServ)
            return;
        for (int i = 0; i < 16; i++)
            Dust.NewDustPerfect(at + Main.rand.NextVector2Circular(12f, 20f), DustID.Smoke, Main.rand.NextVector2Circular(2f, 2f),
                100, default, 1.4f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Player owner = Main.player[Projectile.owner];
        // Fading in its last second.
        float shadow = 0.45f + 0.5f * Math.Max(0f, 1f - Projectile.timeLeft / 60f);
        try
        {
            Main.PlayerRenderer.DrawPlayer(Main.Camera, owner, owner.position + (Projectile.Center - owner.Center), owner.fullRotation,
                owner.fullRotationOrigin, Math.Min(0.95f, shadow));
        }
        catch (Exception)
        {
            // Drawing a player outside its own pass is a best effort; a smoke trail stands in if it fails.
            Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, Vector2.Zero, 150, default, 1f).noGravity = true;
        }
        return false;
    }
}

// 火遁·豪火球之术: a great fireball that burns through enemies and bursts where it ends.
public sealed class GreatFireball : ModProjectile
{
    public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.BallofFire}";

    public override void SetDefaults()
    {
        Projectile.width = 56;
        Projectile.height = 56;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Generic;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
        Projectile.timeLeft = 75;
        Projectile.tileCollide = true;
        Projectile.aiStyle = -1;
    }

    public override void AI()
    {
        Projectile.rotation += 0.12f * Math.Sign(Projectile.velocity.X == 0f ? 1f : Projectile.velocity.X);
        Lighting.AddLight(Projectile.Center, 1.2f, 0.6f, 0.15f);
        if (!Main.dedServ)
            for (int i = 0; i < 3; i++)
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24f, 24f), DustID.Torch,
                    -Projectile.velocity * 0.2f + Main.rand.NextVector2Circular(1f, 1f), 0, default, 2f).noGravity = true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire3, 180);

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        if (Projectile.owner == Main.myPlayer)
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FireBurst>(),
                (int)(Projectile.damage * 0.8f), 6f, Projectile.owner);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (FxArt.Frame("FxGreatFireball", (int)(Main.GameUpdateCount / 5), 4) is { } art)
        {
            FxArt.Draw(art, Projectile.Center, Color.White, Projectile.velocity.ToRotation(), 1.5f);
            return false;
        }
        Texture2D t = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
        Main.EntitySpriteDraw(t, Projectile.Center - Main.screenPosition, null, new Color(255, 170, 60, 0), Projectile.rotation,
            t.Size() / 2f, 3.2f, SpriteEffects.None);
        return false;
    }
}

// Where the great fireball ends: a burst of flame all round.
public sealed class FireBurst : ModProjectile
{
    private const int Life = 14;

    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    public override void SetDefaults()
    {
        Projectile.width = 160;
        Projectile.height = 160;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Generic;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.timeLeft = Life;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
    }

    public override void AI()
    {
        if (Projectile.localAI[0]++ == 0f && !Main.dedServ && !FxArt.Has("FxFireBurst_0"))
            for (int i = 0; i < 40; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Main.rand.NextVector2Circular(8f, 8f), 0, default, 2.4f).noGravity = true;
        Lighting.AddLight(Projectile.Center, 1.6f, 0.8f, 0.2f);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire3, 240);

    public override bool PreDraw(ref Color lightColor)
    {
        int age = Life - Projectile.timeLeft;
        if (FxArt.Frame("FxFireBurst", Math.Min(3, age * 4 / Life), 4) is { } art)
            FxArt.Draw(art, Projectile.Center, Color.White, 0f, 1.5f);
        return false;
    }
}

// 千鸟: lightning in the hand and a straight charge (ai velocity = the direction), the player carried along and safe
// for its length, every enemy in the way struck once.
public sealed class ChidoriCharge : ModProjectile
{
    private const int ChargeTicks = 16;
    private const float ChargeSpeed = 19f;

    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    public override void SetDefaults()
    {
        Projectile.width = 44;
        Projectile.height = 44;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Generic;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.timeLeft = ChargeTicks;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
    }

    private Vector2 Direction => Projectile.velocity.SafeNormalize(Vector2.UnitX);

    public override void AI()
    {
        Player owner = Main.player[Projectile.owner];
        if (!owner.active || owner.dead)
        {
            Projectile.Kill();
            return;
        }
        if (Projectile.localAI[0]++ == 0f)
            SoundEngine.PlaySound(SoundID.Item94, owner.Center);
        Vector2 dir = Direction;
        owner.velocity = dir * ChargeSpeed;
        owner.direction = dir.X >= 0f ? 1 : -1;
        owner.immune = true;
        owner.immuneNoBlink = true;
        owner.immuneTime = Math.Max(owner.immuneTime, 6);
        owner.fallStart = (int)(owner.position.Y / 16f);
        Projectile.Center = owner.Center + dir * 22f;
        Lighting.AddLight(Projectile.Center, 0.5f, 0.8f, 1.4f);
        if (!Main.dedServ && !FxArt.Has("FxChidori_0"))
            for (int i = 0; i < 4; i++)
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(14f, 14f), DustID.Electric,
                    Main.rand.NextVector2Circular(3f, 3f), 0, default, 1.1f).noGravity = true;
    }

    public override void OnKill(int timeLeft)
    {
        Player owner = Main.player[Projectile.owner];
        owner.velocity *= 0.25f;
        owner.immuneNoBlink = false;
        if (!Main.dedServ)
            for (int i = 0; i < 24; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.Electric, Main.rand.NextVector2Circular(5f, 5f), 0, default, 1.3f).noGravity = true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        SoundEngine.PlaySound(SoundID.Item122 with { Volume = 0.6f }, target.Center);
        target.AddBuff(BuffID.Electrified, 120);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Vector2 dir = Direction;
        int age = (int)Projectile.localAI[0];
        if (FxArt.Frame("FxChidoriTrail", age / 3, 3) is { } trail)
            FxArt.Draw(trail, Projectile.Center - dir * 52f, Color.White, dir.ToRotation(), 1.5f);
        if (FxArt.Frame("FxChidori", (int)(Main.GameUpdateCount / 3), 4) is { } hand)
            FxArt.Draw(hand, Projectile.Center, Color.White, 0f, 1.5f);
        return false;
    }
}
