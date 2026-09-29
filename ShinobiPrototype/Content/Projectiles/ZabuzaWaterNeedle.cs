using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;

namespace ShinobiPrototype.Content.Projectiles;

public sealed class ZabuzaWaterNeedle : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 6;
        ProjectileID.Sets.TrailingMode[Type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = ZabuzaCombatRules.NeedleWidth;
        Projectile.height = ZabuzaCombatRules.NeedleHeight;
        Projectile.hostile = true;
        Projectile.tileCollide = true;
        Projectile.ignoreWater = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = ZabuzaCombatRules.WaterNeedleLifetimeTicks;
    }

    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation();
        Lighting.AddLight(Projectile.Center, 0.18f, 0.38f, 0.48f);
        if (Main.rand.NextBool(4))
            Dust.NewDustPerfect(Projectile.Center, DustID.Water,
                -Projectile.velocity * 0.1f, 30,
                Projectile.ai[0] > 0.5f ? new Color(220, 145, 255) : new Color(180, 245, 255),
                1f).noGravity = true;
    }

    public override bool PreDraw(ref Color lightColor) => ZabuzaWaterVisuals.Draw(Projectile,
        new Color(190, 245, 255), ZabuzaCombatRules.WaterNeedleDrawScale);
}
