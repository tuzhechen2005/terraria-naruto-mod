using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;

namespace ShinobiPrototype.Content.Projectiles;

public sealed class ZabuzaWaterWave : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 6;
        ProjectileID.Sets.TrailingMode[Type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = ZabuzaCombatRules.WaterWidth;
        Projectile.height = ZabuzaCombatRules.WaterHeight;
        Projectile.hostile = true;
        Projectile.tileCollide = true;
        Projectile.ignoreWater = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = ZabuzaCombatRules.WaterWaveLifetimeTicks;
    }

    public override void AI()
    {
        Lighting.AddLight(Projectile.Center, 0.12f, 0.42f, 0.55f);
        Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;
        Projectile.rotation = Projectile.velocity.ToRotation();
        if (Projectile.velocity.X < 0f)
            Projectile.rotation += MathHelper.Pi;
        if (Main.rand.NextBool(3))
            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(18f, 8f),
                DustID.Water, -Projectile.velocity * 0.13f, 40,
                Projectile.ai[0] > 0.5f ? new Color(210, 110, 255) : new Color(100, 240, 255),
                1.25f).noGravity = true;
    }

    public override bool PreDraw(ref Color lightColor) => ZabuzaWaterVisuals.Draw(Projectile,
        new Color(65, 235, 255), ZabuzaCombatRules.WaterWaveDrawScale);
}
