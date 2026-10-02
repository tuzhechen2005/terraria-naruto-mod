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

// Gaara's bullets (user, 2026-10-02). Art: gaara-fx-v1 (FxQuicksandMark, FxSandPillar); until it is in, sand dust
// stands in.

// Quicksand: a churning mark on the ground for ai[0] ticks (harmless), then a pillar of sand bursts out of it.
// Spawned anywhere above the spot; it settles onto the ground below on its first tick.
public sealed class Quicksand : ModProjectile
{
    private const int PillarTicks = 30;

    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    private bool Waiting => Projectile.ai[0] > 0f;

    public override void SetDefaults()
    {
        Projectile.width = 40;
        Projectile.height = 110;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 600;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = false;
    }

    public override bool? CanDamage() => Waiting ? false : null;

    public override void AI()
    {
        Projectile.velocity = Vector2.Zero;
        if (Projectile.localAI[0] == 0f)
        {
            Projectile.localAI[0] = 1f;
            Projectile.Bottom = new Vector2(Projectile.Center.X, Ground(Projectile.Center));
        }
        if (Waiting)
        {
            if (--Projectile.ai[0] <= 0f)
            {
                Projectile.timeLeft = PillarTicks;
                SoundEngine.PlaySound(SoundID.Item69 with { Volume = 0.7f }, Projectile.Bottom);
            }
            else if (!Main.dedServ && !FxArt.Has("FxQuicksandMark") && Main.rand.NextBool(2))
                Dust.NewDustPerfect(Projectile.Bottom + new Vector2(Main.rand.NextFloat(-22f, 22f), -2f), DustID.Sand,
                    new Vector2(0f, -1f), 60, default, 1.2f).noGravity = true;
            return;
        }
        if (!Main.dedServ && !FxArt.Has("FxSandPillar_0"))
            for (int i = 0; i < 4; i++)
                Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), DustID.Sand, new Vector2(0f, -4f), 60, default, 1.5f)
                    .noGravity = true;
    }

    // The top of the first solid ground at or below `at` (a few tiles up first, so a spot marked from just under a
    // ledge still finds it), or `at` itself if there is none close by.
    private static float Ground(Vector2 at)
    {
        int x = (int)(at.X / 16f);
        for (int y = (int)(at.Y / 16f) - 3; y < (int)(at.Y / 16f) + 30; y++)
            if (WorldGen.InWorld(x, y, 2) && WorldGen.SolidTile(x, y) && !WorldGen.SolidTile(x, y - 1))
                return y * 16f;
        return at.Y;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Color light = BossSprites.Lit(lightColor, 0.6f);
        if (Waiting)
        {
            if (!FxArt.Has("FxQuicksandMark"))
                return false;
            // Blinking faster as it is about to burst.
            int period = Projectile.ai[0] < 18f ? 3 : 7;
            Color mark = (int)(Projectile.ai[0] / period) % 2 == 1 ? light * 0.55f : light;
            Texture2D art = FxArt.Get("FxQuicksandMark");
            FxArt.Draw(art, Projectile.Bottom - new Vector2(0f, art.Height * 0.75f - 2f), mark, 0f, 1.5f);
            return false;
        }
        int age = PillarTicks - Projectile.timeLeft;
        if (FxArt.Has($"FxSandPillar_{Math.Min(2, age / 10)}"))
        {
            Texture2D pillar = FxArt.Get($"FxSandPillar_{Math.Min(2, age / 10)}");
            FxArt.Draw(pillar, Projectile.Bottom - new Vector2(0f, pillar.Height * 0.75f), light, 0f, 1.5f);
        }
        return false;
    }
}

// A pellet of sand flicked at the player while he walks: small, falling a little, gone on the first wall.
public sealed class SandPellet : ModProjectile
{
    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    public override void SetDefaults()
    {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.hostile = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 180;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = true;
    }

    public override void AI()
    {
        Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.08f, 10f);
        Projectile.rotation += 0.25f * Math.Sign(Projectile.velocity.X);
        if (!Main.dedServ && Main.rand.NextBool(3))
            Dust.NewDustPerfect(Projectile.Center, DustID.Sand, -Projectile.velocity * 0.1f, 80, default, 0.9f).noGravity = true;
    }

    public override void OnKill(int timeLeft)
    {
        if (!Main.dedServ)
            for (int i = 0; i < 6; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.Sand, Main.rand.NextVector2Circular(2f, 2f), 60, default, 1.1f);
    }

    // A small clod of sand in hard steps (dark outline, then the gourd's sand colours), 2×2 art pixels like the rest.
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D pixel = TextureAssets.MagicPixel.Value;
        Color light = BossSprites.Lit(lightColor, 0.6f);
        Vector2 at = Projectile.Center - Main.screenPosition;
        at = new Vector2(MathF.Round(at.X), MathF.Round(at.Y));
        void Block(int x, int y, int w, int h, Color c) =>
            Main.EntitySpriteDraw(pixel, at + new Vector2(x, y), new Rectangle(0, 0, 1, 1), c.MultiplyRGB(light), 0f, Vector2.Zero,
                new Vector2(w, h), SpriteEffects.None);
        Block(-4, -6, 8, 12, new Color(74, 46, 28));
        Block(-6, -4, 12, 8, new Color(74, 46, 28));
        Block(-4, -4, 8, 8, new Color(196, 150, 92));
        Block(-2, -4, 4, 4, new Color(232, 200, 140));
        Block(0, 0, 4, 4, new Color(150, 104, 60));
        return false;
    }
}
