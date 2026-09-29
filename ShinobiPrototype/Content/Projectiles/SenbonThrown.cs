using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Projectiles;

// A thrown senbon: flies straight, then starts to drop. Sometimes slows its target (Haku aims for pressure points).
// ai[0] = 1 marks needles thrown by the ice-mirror minion, which deal summon damage.
public sealed class SenbonThrown : ModProjectile
{
    private const int StraightTicks = 24;
    private const int SlowChanceOneIn = 10;

    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    public override void SetDefaults()
    {
        Projectile.width = 8;
        Projectile.height = 8;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 120;
        Projectile.aiStyle = -1;
    }

    public override void OnSpawn(IEntitySource source)
    {
        if (Projectile.ai[0] == 1f)
            Projectile.DamageType = DamageClass.Summon;
    }

    public override void AI()
    {
        if (++Projectile.localAI[0] > StraightTicks)
            Projectile.velocity.Y = System.Math.Min(Projectile.velocity.Y + 0.25f, 14f);
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Main.rand.NextBool(SlowChanceOneIn))
            target.AddBuff(BuffID.Slow, 60);
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 3; i++)
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Silver, 0f, 0f, 0, default, 0.7f);
    }
}
