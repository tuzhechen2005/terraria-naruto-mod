using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;

namespace ShinobiPrototype.Content.Projectiles;

public sealed class ZabuzaWaterDragon : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 6;
        ProjectileID.Sets.TrailingMode[Type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = ZabuzaCombatRules.DragonWidth;
        Projectile.height = ZabuzaCombatRules.DragonHeight;
        Projectile.hostile = true;
        Projectile.tileCollide = true;
        Projectile.ignoreWater = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = ZabuzaCombatRules.WaterDragonLifetimeTicks;
    }

    public override void AI()
    {
        Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;
        Projectile.rotation = Projectile.velocity.ToRotation();
        if (Projectile.velocity.X < 0f)
            Projectile.rotation += MathHelper.Pi;
        Lighting.AddLight(Projectile.Center, 0.15f, 0.42f, 0.57f);
        if (Main.rand.NextBool(2))
            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(28f, 12f),
                DustID.Water, -Projectile.velocity * 0.12f, 30,
                Projectile.ai[0] > 0.5f ? new Color(215, 115, 255) : new Color(125, 230, 255),
                1.3f).noGravity = true;
    }

    public override bool PreDraw(ref Color lightColor) => ZabuzaWaterVisuals.Draw(Projectile,
        new Color(100, 220, 255), ZabuzaCombatRules.WaterDragonDrawScale);

    public override void OnKill(int timeLeft)
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        SoundEngine.PlaySound(SoundID.Splash, Projectile.Center);
        for (int i = 0; i < 12; i++)
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water);
    }
}
