using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Projectiles;

// A shuriken thrown by a candidate in the Forest of Death: flies straight and spins (jump it or step aside).
public sealed class ExamShuriken : ModProjectile
{
    public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Shuriken}";

    public override void SetDefaults()
    {
        Projectile.width = 18;
        Projectile.height = 18;
        Projectile.hostile = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 120;
        Projectile.aiStyle = -1;
    }

    public override void AI() => Projectile.rotation += 0.45f * System.Math.Sign(Projectile.velocity.X == 0f ? 1f : Projectile.velocity.X);
}
