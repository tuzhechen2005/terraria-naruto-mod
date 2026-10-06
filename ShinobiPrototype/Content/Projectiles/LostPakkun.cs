using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Projectiles;

// Pakkun lost on the surface for Kakashi's first lesson (KakashiLessonPlayer): he sits sniffing the ground until his
// owner finds him. A projectile owned by that player, so it needs no server-side NPC.
public sealed class LostPakkun : ModProjectile
{
    public override string Texture => "ShinobiPrototype/Content/Projectiles/Pakkun_Idle";

    public override void SetDefaults()
    {
        Projectile.width = 28;
        Projectile.height = 22;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 10 * 60 * 60;
        Projectile.netImportant = true;
    }

    public override bool? CanDamage() => false;

    public override void AI()
    {
        Projectile.velocity.X = 0f;
        Projectile.velocity.Y = System.Math.Min(Projectile.velocity.Y + 0.3f, 8f);
        Player owner = Main.player[Projectile.owner];
        Projectile.spriteDirection = owner.Center.X >= Projectile.Center.X ? 1 : -1;
        if (Main.rand.NextBool(40))
            Dust.NewDust(Projectile.BottomLeft - new Vector2(0f, 4f), Projectile.width, 4, DustID.Dirt, 0f, -1f);
    }

    public override bool OnTileCollide(Vector2 oldVelocity) => false;

    public override bool PreDraw(ref Color lightColor)
    {
        bool sniff = Main.GameUpdateCount / 30 % 4 == 0;
        Texture2D art = ModContent.Request<Texture2D>($"ShinobiPrototype/Content/Projectiles/{(sniff ? "Pakkun_Run_1" : "Pakkun_Idle")}").Value;
        Main.EntitySpriteDraw(art, Projectile.Bottom - Main.screenPosition + new Vector2(0f, 2f), null, lightColor, 0f,
            new Vector2(art.Width / 2f, art.Height), 1f, Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
        return false;
    }
}
