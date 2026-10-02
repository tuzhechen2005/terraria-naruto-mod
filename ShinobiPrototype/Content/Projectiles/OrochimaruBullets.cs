using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Content.Projectiles;

// Orochimaru's bullets (user, 2026-10-02: Terraria bosses fight with projectiles). Art: orochimaru-bullets-v1
// (FxSnakeBullet, FxVenomGlob, FxVenomPool, FxKusanagi); until it is in, the snake-hand head and dust stand in.

// A small snake flung at the player: the swarm and the snakes he flicks while walking. The snake rain uses it too
// (ai[1] = 1): it hangs over the player for ai[0] ticks, plainly visible and blinking, with a line down to where it
// will land (user, 2026-10-02: the rain was hard to see), then drops, slower and bigger than a flung one.
public sealed class SnakeBullet : ModProjectile
{
    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    private bool Rain => Projectile.ai[1] > 0f;
    private bool Waiting => Projectile.ai[0] > 0f;
    private float Size => Rain ? 2.25f : 1.5f;

    public override void SetDefaults()
    {
        Projectile.width = 18;
        Projectile.height = 18;
        Projectile.hostile = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 300;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = false;
    }

    public override bool? CanDamage() => Waiting ? false : null;

    public override void AI()
    {
        if (Rain && Projectile.localAI[0] == 0f)
        {
            Projectile.localAI[0] = 1f;
            Projectile.Resize(26, 26);
        }
        if (Waiting)
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.rotation = MathHelper.PiOver2;
            if (--Projectile.ai[0] <= 0f)
            {
                Projectile.velocity = new Vector2(0f, 9f);
                SoundEngine.PlaySound(SoundID.Item17 with { Volume = 0.5f }, Projectile.Center);
            }
            return;
        }
        Projectile.rotation = Projectile.velocity.ToRotation();
        if (!Main.dedServ && !FxArt.Has("FxSnakeBullet_0") && !FxArt.Has("FxSnakeHand_Head"))
            Dust.NewDustPerfect(Projectile.Center, DustID.PurpleTorch, Vector2.Zero, 0, default, 1.1f).noGravity = true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        // Rain snakes are drawn bright whatever the light, so they show against the sky and the trees.
        Color color = Rain ? Color.White : BossSprites.Lit(lightColor, 0.6f);
        if (Waiting)
        {
            DrawDropLine();
            // Blinking faster as it is about to drop.
            int period = Projectile.ai[0] < 20f ? 4 : 8;
            if ((int)(Projectile.ai[0] / period) % 2 == 1)
                color *= 0.45f;
        }
        Texture2D art = FxArt.Frame("FxSnakeBullet", (int)(Main.GameUpdateCount / 6), 2) ??
                        (FxArt.Has("FxSnakeHand_Head") ? FxArt.Get("FxSnakeHand_Head") : null);
        if (art == null)
            return false;
        bool left = Projectile.velocity.X < 0f;
        FxArt.Draw(art, Projectile.Center, color, left ? Projectile.rotation - MathHelper.Pi : Projectile.rotation, Size, left ? -1 : 1);
        return false;
    }

    // A dashed violet line straight down from the waiting snake to the ground below it.
    private void DrawDropLine()
    {
        Texture2D pixel = TextureAssets.MagicPixel.Value;
        int x = (int)(Projectile.Center.X / 16f);
        int y0 = (int)(Projectile.Center.Y / 16f);
        int ground = y0;
        while (ground < y0 + 40 && WorldGen.InWorld(x, ground) && !WorldGen.SolidTile(x, ground))
            ground++;
        float bottom = ground * 16f;
        for (float y = Projectile.Center.Y + 16f; y < bottom; y += 14f)
        {
            Vector2 at = new Vector2(Projectile.Center.X - 1f, y) - Main.screenPosition;
            Main.EntitySpriteDraw(pixel, at, new Rectangle(0, 0, 1, 1), new Color(230, 120, 255) * 0.75f, 0f, Vector2.Zero,
                new Vector2(3f, 7f), SpriteEffects.None);
        }
    }
}

// A glob of venom spat in an arc; where it lands it leaves a pool.
public sealed class VenomGlob : ModProjectile
{
    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 300;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = true;
    }

    public override void AI()
    {
        Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.28f, 14f);
        if (!Main.dedServ && (!FxArt.Has("FxVenomGlob_0") || Main.rand.NextBool(3)))
            Dust.NewDustPerfect(Projectile.Center, DustID.Venom, Projectile.velocity * 0.1f, 0, default, 1.2f).noGravity = true;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Poisoned, 4 * 60);

    public override void OnKill(int timeLeft)
    {
        if (Main.myPlayer == Projectile.owner)
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Bottom, Vector2.Zero, ModContent.ProjectileType<VenomPool>(),
                ExamBossRules.VenomPoolDamage, 0f, Projectile.owner);
        if (!Main.dedServ)
            for (int i = 0; i < 10; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.Venom, Main.rand.NextVector2Circular(2.5f, 2.5f), 0, default, 1.3f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (FxArt.Frame("FxVenomGlob", (int)(Main.GameUpdateCount / 8), 2) is { } art)
            FxArt.Draw(art, Projectile.Center, BossSprites.Lit(lightColor, 0.6f), 0f, 1.5f);
        return false;
    }
}

// A pool of venom on the ground for a few seconds: standing in it poisons.
public sealed class VenomPool : ModProjectile
{
    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    public override void SetDefaults()
    {
        Projectile.width = 70;
        Projectile.height = 14;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = ExamBossRules.VenomPoolTicks;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = true;
    }

    public override void AI()
    {
        // Settle onto the ground, then stay.
        Projectile.velocity.X = 0f;
        Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.4f, 8f);
        if (Projectile.timeLeft < 40)
            Projectile.alpha = (int)(255 * (1f - Projectile.timeLeft / 40f));
        if (!Main.dedServ && Main.rand.NextBool(6))
            Dust.NewDustPerfect(new Vector2(Main.rand.NextFloat(Projectile.Left.X, Projectile.Right.X), Projectile.Bottom.Y - 4f),
                DustID.Venom, new Vector2(0f, -0.8f), 0, default, 1f).noGravity = true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity) => false;

    public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Poisoned, 3 * 60);

    public override bool PreDraw(ref Color lightColor)
    {
        if (FxArt.Frame("FxVenomPool", (int)(Main.GameUpdateCount / 10), 3) is { } art)
            FxArt.Draw(art, Projectile.Bottom - new Vector2(0f, art.Height * 0.75f), BossSprites.Lit(lightColor, 0.6f) * Projectile.Opacity, 0f, 1.5f);
        return false;
    }
}

// The Kusanagi sword: a thin aiming line first, then the blade shoots out of his mouth along it, long and straight.
// ai[0] is the angle; the projectile sits at the root of the blade.
public sealed class KusanagiBlade : ModProjectile
{
    public const int AimTicks = 34;
    private const int StrikeTicks = 14;

    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    private int Tick => (int)Projectile.localAI[0];
    private float Angle => Projectile.ai[0];
    private Vector2 Along => Angle.ToRotationVector2();

    public override void SetDefaults()
    {
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = AimTicks + StrikeTicks;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        Projectile.velocity = Vector2.Zero;
        Projectile.localAI[0]++;
        if (Tick == AimTicks)
            SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
    }

    private float Reach => Tick < AimTicks ? 0f : ExamBossRules.KusanagiReachPx * Math.Min(1f, (Tick - AimTicks + 1) / 4f);

    public override bool? CanDamage() => Tick >= AimTicks ? null : false;

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float collision = 0f;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
            Projectile.Center + Along * Reach, 14f, ref collision);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D pixel = TextureAssets.MagicPixel.Value;
        Vector2 at = Projectile.Center - Main.screenPosition;
        if (Tick < AimTicks)
        {
            // The aiming line: thin, blinking faster as the strike nears.
            if ((Tick / Math.Max(2, 6 - Tick / 8)) % 2 == 0)
                Main.EntitySpriteDraw(pixel, at, new Rectangle(0, 0, 1, 1), new Color(220, 80, 220) * 0.8f, Angle, new Vector2(0f, 0.5f),
                    new Vector2(ExamBossRules.KusanagiReachPx, 2f), SpriteEffects.None);
            return false;
        }
        if (FxArt.Has("FxKusanagi"))
        {
            Texture2D blade = FxArt.Get("FxKusanagi");
            Main.EntitySpriteDraw(blade, at, null, Color.White, Angle, new Vector2(0f, blade.Height / 2f),
                new Vector2(Reach / blade.Width, 1.5f), SpriteEffects.None);
        }
        else
            Main.EntitySpriteDraw(pixel, at, new Rectangle(0, 0, 1, 1), new Color(225, 230, 240), Angle, new Vector2(0f, 0.5f),
                new Vector2(Reach, 6f), SpriteEffects.None);
        return false;
    }
}
