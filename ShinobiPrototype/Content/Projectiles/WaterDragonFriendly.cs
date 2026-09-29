using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;

namespace ShinobiPrototype.Content.Projectiles;

// The player's water dragon: Zabuza's dragon art, curling gently towards the nearest enemy and piercing three.
public sealed class WaterDragonFriendly : ModProjectile
{
    private const float HomingRange = 420f;
    private const float TurnRate = 0.06f;

    public override string Texture => "ShinobiPrototype/Content/Projectiles/ZabuzaWaterDragon";

    public override void SetDefaults()
    {
        Projectile.width = 36;
        Projectile.height = 20;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.penetrate = 3;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 100;
        Projectile.aiStyle = -1;
    }

    public override void AI()
    {
        NPC target = null;
        float best = HomingRange;
        foreach (NPC npc in Main.ActiveNPCs)
        {
            float distance = npc.Distance(Projectile.Center);
            if (npc.CanBeChasedBy(Projectile) && distance < best)
            {
                best = distance;
                target = npc;
            }
        }
        if (target != null)
        {
            float speed = Projectile.velocity.Length();
            Vector2 wanted = Projectile.DirectionTo(target.Center) * speed;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, wanted, TurnRate);
        }
        Projectile.rotation = Projectile.velocity.ToRotation();
        if (Main.rand.NextBool(3))
            Dust.NewDustPerfect(Projectile.Center, DustID.Water, -Projectile.velocity * 0.2f, 100, default, 1.1f).noGravity = true;
    }

    public override bool PreDraw(ref Color lightColor) =>
        ZabuzaWaterVisuals.Draw(Projectile, new Color(100, 220, 255), ZabuzaCombatRules.WaterDragonDrawScale);
}
