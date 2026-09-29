using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Projectiles;

// The starting kunai in flight: straight for a moment, then dropping like a thrown knife. Uses the item's art,
// which points up-right.
public sealed class KunaiThrown : ModProjectile
{
    private const int StraightTicks = 18;

    public override string Texture => "ShinobiPrototype/Content/Items/TrainingKunai";

    public override void SetDefaults()
    {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 150;
        Projectile.aiStyle = -1;
        Projectile.scale = 0.8f;
    }

    public override void AI()
    {
        if (++Projectile.localAI[0] > StraightTicks)
            Projectile.velocity.Y = System.Math.Min(Projectile.velocity.Y + 0.3f, 14f);
        Projectile.rotation = Projectile.velocity.ToRotation() + Microsoft.Xna.Framework.MathHelper.PiOver4;
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 3; i++)
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Iron, 0f, 0f, 0, default, 0.7f);
    }
}
