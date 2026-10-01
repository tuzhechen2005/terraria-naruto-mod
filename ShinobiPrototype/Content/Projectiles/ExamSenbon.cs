using Terraria;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Projectiles;

// A senbon thrown by exam genin: flies straight, then starts to drop. With ai[0] > 0 it is one of the Rain genin's
// umbrella rain instead: it hangs unseen for that many ticks (a glint marks where it will fall), then drops.
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

    public override bool? CanDamage() => Projectile.ai[0] > 0f ? false : null;

    public override bool PreDraw(ref Microsoft.Xna.Framework.Color lightColor) => Projectile.ai[0] <= 0f;

    public override void AI()
    {
        if (Projectile.ai[0] > 0f)
        {
            Projectile.velocity = Microsoft.Xna.Framework.Vector2.Zero;
            if (--Projectile.ai[0] <= 0f)
                Projectile.velocity = new Microsoft.Xna.Framework.Vector2(0f, 10f);
            else if (!Main.dedServ && Main.GameUpdateCount % 4 == 0)
                Dust.NewDustPerfect(Projectile.Center, Terraria.ID.DustID.SilverFlame, Microsoft.Xna.Framework.Vector2.Zero, 120,
                    default, 0.9f).noGravity = true;
            Projectile.rotation = Microsoft.Xna.Framework.MathHelper.PiOver2;
            return;
        }
        if (++Projectile.localAI[0] > StraightTicks)
            Projectile.velocity.Y = System.Math.Min(Projectile.velocity.Y + 0.2f, 12f);
        Projectile.rotation = Projectile.velocity.ToRotation();
    }
}
