using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Projectiles;

// The log left where the player stood after a successful Substitution Jutsu. Purely visual.
public sealed class SubstitutionLog : ModProjectile
{
    private const int Lifetime = 50;

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 32;
        Projectile.aiStyle = -1;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.penetrate = -1;
        Projectile.tileCollide = true;
        Projectile.timeLeft = Lifetime;
    }

    public override void AI()
    {
        Projectile.velocity.X = 0f;
        Projectile.velocity.Y = System.Math.Min(Projectile.velocity.Y + 0.35f, 10f);
        if (Projectile.timeLeft < 12)
            Projectile.alpha = (int)MathHelper.Lerp(255f, 0f, Projectile.timeLeft / 12f);
    }

    public override bool OnTileCollide(Vector2 oldVelocity) => false;

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 14; i++)
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke,
                0f, -1.2f, 100, default, 1.3f);
    }
}
