using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.DamageClasses;

namespace ShinobiPrototype.Content.Projectiles;

// A punch's air pressure (TaijutsuWeapon): hits harder in its first ticks, while it is still the fist. ai[0] is its
// size (the open Eight Gates grow it), ai[1] how long it flies. Drawn with dust for now.
public sealed class TaijutsuPressure : ModProjectile
{
    private const int FistTicks = 6;
    private const float FistBonus = 1.5f;

    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    public override void SetDefaults()
    {
        Projectile.width = 26;
        Projectile.height = 26;
        Projectile.friendly = true;
        Projectile.DamageType = ModContent.GetInstance<TaijutsuDamage>();
        Projectile.penetrate = 3;
        Projectile.timeLeft = 20;
        Projectile.aiStyle = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 12;
        Projectile.hide = true;
    }

    public override void AI()
    {
        if (Projectile.localAI[0] == 0f)
        {
            Projectile.localAI[0] = 1f;
            float scale = Projectile.ai[0] <= 0f ? 1f : Projectile.ai[0];
            Projectile.Resize((int)(26 * scale), (int)(26 * scale));
            Projectile.timeLeft = Projectile.ai[1] > 0f ? (int)Projectile.ai[1] : 20;
        }
        Projectile.localAI[1]++;
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.velocity *= 0.985f;
        if (Main.netMode == NetmodeID.Server)
            return;
        // An arc of air across the front.
        Vector2 across = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2);
        for (int i = 0; i < 3; i++)
        {
            Vector2 at = Projectile.Center + across * Main.rand.NextFloat(-0.5f, 0.5f) * Projectile.height;
            Dust dust = Dust.NewDustPerfect(at, DustID.Cloud, Projectile.velocity * 0.2f, 120, default, 1f + Projectile.ai[0] * 0.2f);
            dust.noGravity = true;
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Projectile.localAI[1] <= FistTicks)
            modifiers.SourceDamage *= FistBonus;
    }
}

// The Leaf Whirlwind: a dash kick that rides along with the player. ai[0] is how long it lasts.
public sealed class WhirlwindKick : ModProjectile
{
    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    public override void SetDefaults()
    {
        Projectile.width = 56;
        Projectile.height = 50;
        Projectile.friendly = true;
        Projectile.DamageType = ModContent.GetInstance<TaijutsuDamage>();
        Projectile.penetrate = -1;
        Projectile.timeLeft = 16;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
        Projectile.hide = true;
    }

    public override void AI()
    {
        Player owner = Main.player[Projectile.owner];
        if (Projectile.localAI[0] == 0f)
        {
            Projectile.localAI[0] = 1f;
            if (Projectile.ai[0] > 0f)
                Projectile.timeLeft = (int)Projectile.ai[0];
        }
        if (!owner.active || owner.dead)
        {
            Projectile.Kill();
            return;
        }
        Projectile.Center = owner.Center + new Vector2(owner.direction * 14f, 0f);
        if (Main.netMode != NetmodeID.Server)
            for (int i = 0; i < 3; i++)
                Dust.NewDustPerfect(owner.Center + Main.rand.NextVector2Circular(26f, 22f), DustID.GreenTorch,
                    new Vector2(-owner.direction * 2f, 0f), 0, default, 1.2f).noGravity = true;
    }
}
