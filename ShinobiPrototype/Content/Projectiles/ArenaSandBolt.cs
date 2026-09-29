using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Projectiles;

public sealed class ArenaSandBolt : ModProjectile
{
    public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.MagicMissile}";

    public override void SetDefaults()
    {
        Projectile.width = 18;
        Projectile.height = 18;
        Projectile.hostile = true;
        Projectile.tileCollide = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 100;
    }

    public override void AI()
    {
        Projectile.rotation += 0.1f;
        if (Main.rand.NextBool(2))
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sand);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Rectangle rect = new((int)(Projectile.position.X - Main.screenPosition.X),
            (int)(Projectile.position.Y - Main.screenPosition.Y), Projectile.width, Projectile.height);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, new Color(205, 168, 100));
        return false;
    }
}
