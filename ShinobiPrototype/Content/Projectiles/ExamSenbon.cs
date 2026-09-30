using Terraria;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Projectiles;

// A senbon thrown by exam genin: flies straight, then starts to drop.
public sealed class ExamSenbon : ModProjectile
{
    private const int StraightTicks = 30;

    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    public override void SetDefaults()
    {
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.hostile = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 150;
        Projectile.aiStyle = -1;
    }

    public override void AI()
    {
        if (++Projectile.localAI[0] > StraightTicks)
            Projectile.velocity.Y = System.Math.Min(Projectile.velocity.Y + 0.2f, 12f);
        Projectile.rotation = Projectile.velocity.ToRotation();
    }
}
