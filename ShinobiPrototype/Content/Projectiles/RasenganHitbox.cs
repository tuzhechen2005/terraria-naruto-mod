using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Projectiles;

public sealed class RasenganHitbox : ModProjectile
{
    public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.MagicMissile}";

    public override void SetDefaults()
    {
        Projectile.width = 38;
        Projectile.height = 38;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.penetrate = 1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 32;
    }

    public override void AI()
    {
        Player owner = Main.player[Projectile.owner];
        if (!owner.active || owner.dead)
        {
            Projectile.Kill();
            return;
        }

        int elapsed = 32 - Projectile.timeLeft;
        int direction = Projectile.ai[0] >= 0 ? 1 : -1;

        if (elapsed < 15)
        {
            // Wind-up: the projectile cannot damage anything yet.
            Projectile.Center = owner.Center + new Vector2(direction * 18f, -4f);
        }
        else if (elapsed < 25)
        {
            owner.velocity.X = direction * 12f;
            Projectile.Center = owner.Center + new Vector2(direction * 22f, -4f);
        }
        else
        {
            Projectile.Center = owner.Center + new Vector2(direction * 20f, -4f);
        }

        Projectile.rotation += 0.3f * direction;
        Lighting.AddLight(Projectile.Center, 0.1f, 0.45f, 0.7f);
    }

    public override bool? CanDamage() => 32 - Projectile.timeLeft >= 15;

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.velocity.X += Projectile.ai[0] >= 0 ? 4f : -4f;
        for (int i = 0; i < 15; i++)
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueCrystalShard);
    }
}
