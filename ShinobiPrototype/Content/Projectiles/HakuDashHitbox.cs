using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Content.Projectiles;

public sealed class HakuDashHitbox : ModProjectile
{
    public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.MagicMissile}";

    public override void SetDefaults()
    {
        Projectile.width = 38;
        Projectile.height = 38;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 18;
    }

    public override void AI()
    {
        int ownerIndex = (int)Projectile.ai[0];
        if (ownerIndex < 0 || ownerIndex >= Main.maxNPCs || !Main.npc[ownerIndex].active ||
            Main.npc[ownerIndex].type != ModContent.NPCType<HakuBoss>())
        {
            Projectile.Kill();
            return;
        }

        NPC owner = Main.npc[ownerIndex];
        int direction = Projectile.ai[1] >= 0f ? 1 : -1;
        Projectile.Center = owner.Center + new Vector2(direction * 24f, 0f);
        Projectile.velocity = Vector2.Zero;
    }

    public override bool PreDraw(ref Color lightColor) => false;
}
