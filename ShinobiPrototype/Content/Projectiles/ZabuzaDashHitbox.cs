using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Content.Projectiles;

// The blade out in front of Zabuza while he dashes. His body hitbox (36x84) is much narrower than the sprite and the
// Kubikiribocho swung ahead of him, so a dash that visibly ran through the player often missed (user, 2026-09-30).
// Follows him along his dash direction and ends with the dash.
public sealed class ZabuzaDashHitbox : ModProjectile
{
    public const int Size = 80;
    public const float Lead = 30f;

    public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.MagicMissile}";

    public override void SetDefaults()
    {
        Projectile.width = Size;
        Projectile.height = Size;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = ZabuzaCombatRules.DashMaxActiveTicks + 10;
    }

    public override void AI()
    {
        int ownerIndex = (int)Projectile.ai[0];
        NPC owner = ownerIndex >= 0 && ownerIndex < Main.maxNPCs ? Main.npc[ownerIndex] : null;
        if (owner is not { active: true } || owner.type != ModContent.NPCType<ZabuzaBoss>() ||
            (int)owner.ai[0] is not (ZabuzaCombatRules.DashActive or ZabuzaCombatRules.DashChainActive))
        {
            Projectile.Kill();
            return;
        }
        Vector2 heading = owner.velocity.LengthSquared() > 1f ? Vector2.Normalize(owner.velocity) : new Vector2(owner.direction, 0f);
        Projectile.Center = owner.Center + heading * Lead;
        Projectile.velocity = Vector2.Zero;
    }

    public override bool PreDraw(ref Color lightColor) => false;
}
