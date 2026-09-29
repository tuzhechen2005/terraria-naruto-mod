using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;

namespace ShinobiPrototype.Content.Projectiles;

public sealed class ZabuzaSlash : ModProjectile
{
    public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.MagicMissile}";

    public override void SetDefaults()
    {
        Projectile.width = ZabuzaCombatRules.SlashWidth;
        Projectile.height = ZabuzaCombatRules.SlashHeight;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = ZabuzaCombatRules.SlashActiveTicks;
    }

    public override void AI()
    {
        int ownerIndex = (int)Projectile.ai[0];
        if (ownerIndex < 0 || ownerIndex >= Main.maxNPCs || !Main.npc[ownerIndex].active)
        {
            Projectile.Kill();
            return;
        }

        NPC owner = Main.npc[ownerIndex];
        int direction = Projectile.ai[1] >= 0f ? 1 : -1;
        Projectile.Center = owner.Center + new Vector2(direction * ZabuzaCombatRules.SlashBladeOffsetX,
            ZabuzaCombatRules.SlashBladeOffsetY);
        Projectile.velocity = Vector2.Zero;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false; // The active blade is drawn by the owner's release pose.
    }
}
