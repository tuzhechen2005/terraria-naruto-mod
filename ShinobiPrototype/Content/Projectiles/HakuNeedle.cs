using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Systems;

namespace ShinobiPrototype.Content.Projectiles;

public sealed class HakuNeedle : ModProjectile
{
    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    public override void SetDefaults()
    {
        Projectile.width = 20;
        Projectile.height = 10;
        Projectile.hostile = true;
        Projectile.tileCollide = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 90;
    }

    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation();
        Lighting.AddLight(Projectile.Center, 0.18f, 0.43f, 0.62f);
        if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(2))
            Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * 0.8f,
                DustID.IceTorch, -Projectile.velocity * 0.08f, 20,
                new Color(175, 245, 255), 1.15f).noGravity = true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Rectangle needle = new(0, 0, texture.Width, texture.Height);
        Vector2 center = Projectile.Center - Main.screenPosition;
        Vector2 origin = new Vector2(needle.Width, needle.Height) * 0.5f;
        WaveVfx.Needle(Main.spriteBatch, Projectile.Center + Projectile.rotation.ToRotationVector2() *
            Projectile.width * 0.5f, Projectile.rotation);
        Main.spriteBatch.Draw(texture, center + new Vector2(2f, 2f), needle,
            new Color(25, 90, 170, 210), Projectile.rotation, origin,
            1f, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(texture, center, needle,
            new Color(225, 252, 255), Projectile.rotation, origin,
            1f, SpriteEffects.None, 0f);
        return false;
    }

    public override void OnKill(int timeLeft) => WaveVfxBursts.Spawn(WaveVfxBursts.Kind.Frost,
        Projectile.Center, 34f);
}
