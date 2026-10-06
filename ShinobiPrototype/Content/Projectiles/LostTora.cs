using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;

namespace ShinobiPrototype.Content.Projectiles;

// Tora, the cat every genin team has chased (Iruka's D-rank mission, MissionPlayer): she sits on the surface until her
// chaser comes within a dozen tiles, then bolts, pausing for breath now and then; touching her catches her. A
// projectile owned by that player, like LostPakkun. Her art faces left.
public sealed class LostTora : ModProjectile
{
    public override string Texture => "ShinobiPrototype/Content/NPCs/Tora_Idle";

    private ref float RunTicks => ref Projectile.ai[0];
    private ref float RestTicks => ref Projectile.ai[1];

    public override void SetDefaults()
    {
        Projectile.width = 30;
        Projectile.height = 22;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 20 * 60 * 60;
        Projectile.netImportant = true;
    }

    public override bool? CanDamage() => false;

    public override void AI()
    {
        Player owner = Main.player[Projectile.owner];
        float dx = Projectile.Center.X - owner.Center.X;
        bool near = Projectile.Distance(owner.Center) < MissionRules.ToraFleeTiles * 16f;
        if (RestTicks > 0f)
            RestTicks--;
        else if (near && RunTicks <= 0f)
            RunTicks = MissionRules.ToraRunTicks;

        if (RunTicks > 0f)
        {
            if (--RunTicks <= 0f)
                RestTicks = MissionRules.ToraRestTicks;
            Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, (dx >= 0f ? 1f : -1f) * MissionRules.ToraSpeed, 0.2f);
        }
        else
        {
            Projectile.velocity.X *= 0.8f;
        }
        Projectile.velocity.Y = System.Math.Min(Projectile.velocity.Y + 0.3f, 8f);
        if (System.Math.Abs(Projectile.velocity.X) > 0.3f)
            Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
        else
            Projectile.spriteDirection = dx < 0f ? 1 : -1;
    }

    // Leaps over a wall in her way instead of stopping.
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.X != oldVelocity.X && RunTicks > 0f && Projectile.velocity.Y == 0f)
            Projectile.velocity.Y = -6.5f;
        if (Projectile.velocity.X != oldVelocity.X)
            Projectile.velocity.X = 0f;
        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        string frame = System.Math.Abs(Projectile.velocity.X) > 0.4f ? $"Tora_Run_{Main.GameUpdateCount / 5 % 4}" : "Tora_Idle";
        Texture2D art = ModContent.Request<Texture2D>($"ShinobiPrototype/Content/NPCs/{frame}").Value;
        Main.EntitySpriteDraw(art, Projectile.Bottom - Main.screenPosition + new Vector2(0f, 2f), null, lightColor, 0f,
            new Vector2(art.Width / 2f, art.Height), 1f, Projectile.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
        return false;
    }
}
