using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Projectiles;

// The first ninja tools' projectiles (Items/NinjaTools). Art: ninja-tools-v1 (ShurikenThrown, ShadowShuriken,
// PaperBombStuck); until it is in, vanilla's shuriken and sticky grenade.

public sealed class ShurikenThrown : ModProjectile
{
    private int bounces = 2;

    public override string Texture => FxArt.Has("ShurikenThrown_0") ? "ShinobiPrototype/Content/Projectiles/ShurikenThrown_0"
        : $"Terraria/Images/Projectile_{ProjectileID.Shuriken}";

    public override void SetDefaults()
    {
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.penetrate = 2;
        Projectile.timeLeft = 180;
        Projectile.aiStyle = -1;
    }

    public override void AI()
    {
        Projectile.rotation += 0.5f * Math.Sign(Projectile.velocity.X == 0f ? 1f : Projectile.velocity.X);
        if (++Projectile.localAI[0] > 24)
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.2f, 12f);
    }

    // Glances off walls twice.
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (bounces-- <= 0)
            return true;
        if (Projectile.velocity.X != oldVelocity.X)
            Projectile.velocity.X = -oldVelocity.X * 0.85f;
        if (Projectile.velocity.Y != oldVelocity.Y)
            Projectile.velocity.Y = -oldVelocity.Y * 0.85f;
        SoundEngine.PlaySound(SoundID.Dig with { Pitch = 0.6f, Volume = 0.5f }, Projectile.Center);
        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (FxArt.Frame("ShurikenThrown", (int)(Main.GameUpdateCount / 3), 2) is { } art)
        {
            FxArt.Draw(art, Projectile.Center, lightColor, Projectile.rotation, 1f);
            return false;
        }
        return true;
    }
}

// 影手里剑: a great shuriken through several enemies. ai[0] > 0: the one hidden in the first one's shadow, unseen and
// harmless until it comes out ai[0] ticks later.
public sealed class ShadowShuriken : ModProjectile
{
    public const int HiddenTicks = 10;

    public override string Texture => FxArt.Has("ShadowShuriken_0") ? "ShinobiPrototype/Content/Projectiles/ShadowShuriken_0"
        : $"Terraria/Images/Projectile_{ProjectileID.Shuriken}";

    private bool Hidden => Projectile.ai[0] > 0f;

    public override void SetDefaults()
    {
        Projectile.width = 40;
        Projectile.height = 40;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.penetrate = 5;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.timeLeft = 120;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = false;
    }

    public override bool? CanDamage() => Hidden ? false : null;

    public override void AI()
    {
        if (Hidden)
        {
            if (--Projectile.ai[0] <= 0f)
                SoundEngine.PlaySound(SoundID.Item1 with { Pitch = -0.3f }, Projectile.Center);
            // Waiting in the shadow: it holds back by the same distance it will lag.
            Projectile.position -= Projectile.velocity;
            return;
        }
        Projectile.rotation += 0.45f * Math.Sign(Projectile.velocity.X == 0f ? 1f : Projectile.velocity.X);
        Lighting.AddLight(Projectile.Center, 0.3f, 0.25f, 0.4f);
        if (!Main.dedServ && Main.rand.NextBool(2))
            Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, -Projectile.velocity * 0.1f, 160, Color.Black, 1.1f).noGravity = true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (Hidden)
            return false;
        if (FxArt.Frame("ShadowShuriken", (int)(Main.GameUpdateCount / 3), 2) is { } art)
        {
            FxArt.Draw(art, Projectile.Center, lightColor, Projectile.rotation, 1f);
            return false;
        }
        Texture2D t = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
        Main.EntitySpriteDraw(t, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, t.Size() / 2f, 2.6f,
            SpriteEffects.None);
        return false;
    }
}

// The paper bomb in flight, then stuck to the first enemy or surface it touches (following an enemy), going off a
// second later: a blast that hurts enemies only.
public sealed class PaperBombThrown : ModProjectile
{
    private const int FuseTicks = 60;
    private const float BlastRadius = 56f;

    public override string Texture => FxArt.Has("PaperBombStuck_0") ? "ShinobiPrototype/Content/Projectiles/PaperBombStuck_0"
        : $"Terraria/Images/Projectile_{ProjectileID.StickyGrenade}";

    private int StuckTo => (int)Projectile.ai[0] - 1;   // the NPC it is stuck to, or -1
    private bool Stuck => Projectile.ai[1] > 0f;

    public override void SetDefaults()
    {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 300;
        Projectile.aiStyle = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    // Only the blast hurts.
    public override bool? CanDamage() => Projectile.timeLeft <= 2 ? null : false;

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        Vector2 nearest = Vector2.Clamp(Projectile.Center, targetHitbox.TopLeft(), targetHitbox.BottomRight());
        return Vector2.Distance(nearest, Projectile.Center) <= BlastRadius;
    }

    public override void AI()
    {
        if (!Stuck)
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.25f, 12f);
            Projectile.rotation += 0.2f * Math.Sign(Projectile.velocity.X == 0f ? 1f : Projectile.velocity.X);
            foreach (NPC npc in Main.ActiveNPCs)
                if (npc.CanBeChasedBy() && npc.Hitbox.Intersects(Projectile.Hitbox))
                {
                    StickTo(npc.whoAmI + 1, Projectile.Center - npc.Center);
                    break;
                }
            return;
        }
        if (StuckTo >= 0)
        {
            NPC npc = Main.npc[StuckTo];
            if (!npc.active)
                Projectile.ai[0] = 0f;
            else
                Projectile.Center = npc.Center + new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
        }
        Projectile.velocity = Vector2.Zero;
        if (Projectile.timeLeft % 10 == 0)
            SoundEngine.PlaySound(SoundID.MenuTick with { Pitch = 0.6f, Volume = 0.6f }, Projectile.Center);
        if (!Main.dedServ && Main.rand.NextBool(3))
            Dust.NewDustPerfect(Projectile.Center, DustID.Torch, new Vector2(0f, -1f), 0, default, 0.9f).noGravity = true;
    }

    private void StickTo(int npcPlusOne, Vector2 offset)
    {
        Projectile.ai[0] = npcPlusOne;
        Projectile.ai[1] = 1f;
        Projectile.localAI[0] = offset.X;
        Projectile.localAI[1] = offset.Y;
        Projectile.timeLeft = FuseTicks;
        Projectile.tileCollide = false;
        Projectile.netUpdate = true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (!Stuck)
            StickTo(0, Vector2.Zero);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        if (Main.dedServ)
            return;
        for (int i = 0; i < 30; i++)
            Dust.NewDustPerfect(Projectile.Center, i % 2 == 0 ? DustID.Torch : DustID.Smoke, Main.rand.NextVector2Circular(6f, 6f),
                i % 2 == 0 ? 0 : 100, default, 2f).noGravity = true;
        if (FxArt.Has("FxFireBurst_0"))
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PaperBombBlast>(), 0, 0f, Projectile.owner);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D art = FxArt.Frame("PaperBombStuck", Stuck ? (int)(Main.GameUpdateCount / 5) : 0, 2);
        if (art != null)
        {
            FxArt.Draw(art, Projectile.Center, lightColor, Stuck ? 0f : Projectile.rotation, 1f);
            return false;
        }
        return true;
    }
}

// Just the look of a paper bomb going off (the bomb itself dealt the damage).
public sealed class PaperBombBlast : ModProjectile
{
    private const int Life = 16;

    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = false;
        Projectile.timeLeft = Life;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        int age = Life - Projectile.timeLeft;
        if (FxArt.Frame("FxFireBurst", Math.Min(3, age * 4 / Life), 4) is { } art)
            FxArt.Draw(art, Projectile.Center, Color.White, 0f, 1.1f);
        return false;
    }
}
