using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Content.Projectiles;

public sealed class HakuPrismShard : ModProjectile
{
    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 14;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 140;
    }

    // ai[2] optionally overrides the warning time (Thousand Water Needles uses a longer one).
    private float WarningTicks => Projectile.ai[2] > 0f ? Projectile.ai[2] : WaveDuoRules.PrismWarningTicks;

    public override bool ShouldUpdatePosition() => Projectile.ai[0] >= WarningTicks;

    public override bool? CanDamage() => Projectile.ai[0] >= WarningTicks;

    public override void AI()
    {
        int owner = (int)Projectile.ai[1] - 1;
        if (owner < 0 || owner >= Main.maxNPCs || !Main.npc[owner].active ||
            Main.npc[owner].type != ModContent.NPCType<HakuBoss>())
        {
            Projectile.Kill();
            return;
        }

        Projectile.ai[0]++;
        Projectile.tileCollide = Projectile.ai[0] >= WarningTicks;
        Projectile.rotation = Projectile.velocity.ToRotation();
        Lighting.AddLight(Projectile.Center, 0.22f, 0.54f, 0.74f);
        if (Main.netMode != NetmodeID.Server &&
            (Projectile.ai[0] < WarningTicks || Main.rand.NextBool(2)))
        {
            Vector2 point = Projectile.Center + Main.rand.NextVector2Circular(13f, 13f);
            Dust.NewDustPerfect(point, DustID.IceTorch,
                Projectile.ai[0] < WarningTicks
                    ? Vector2.Zero : -Projectile.velocity * 0.12f,
                25, new Color(175, 245, 255), 1.25f).noGravity = true;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Rectangle source = new(0, 0, texture.Width, texture.Height);
        Vector2 center = Projectile.Center - Main.screenPosition;
        float pulse = Projectile.ai[0] < WarningTicks
            ? 0.75f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 13f) * 0.18f : 1f;
        Vector2 origin = new Vector2(source.Width, source.Height) * 0.5f;
        WaveVfx.Needle(Main.spriteBatch, Projectile.Center + Projectile.rotation.ToRotationVector2() *
            Projectile.width * 0.5f, Projectile.rotation,
            Projectile.ai[0] < WarningTicks ? Projectile.ai[0] / WarningTicks : 0f);
        Main.spriteBatch.Draw(texture, center + new Vector2(2f, 2f), source,
            new Color(20, 85, 165, 190), Projectile.rotation, origin,
            1f, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(texture, center, source,
            new Color(220, 250, 255) * pulse, Projectile.rotation, origin,
            1f, SpriteEffects.None, 0f);
        return false;
    }

    public override void OnKill(int timeLeft) => WaveVfxBursts.Spawn(WaveVfxBursts.Kind.Frost,
        Projectile.Center, 40f);
}
