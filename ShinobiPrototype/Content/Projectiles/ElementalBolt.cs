using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Projectiles;

public sealed class ElementalBolt : ModProjectile
{
    public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.MagicMissile}";

    public override void SetDefaults()
    {
        Projectile.width = 18;
        Projectile.height = 18;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.penetrate = 1;
        Projectile.tileCollide = true;
        Projectile.timeLeft = 90;
    }

    public override void AI()
    {
        Color color = NatureColor((int)Projectile.ai[0]);
        Lighting.AddLight(Projectile.Center, color.ToVector3() * 0.5f);
        if (Main.rand.NextBool(2))
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MagicMirror, newColor: color);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        switch ((int)Projectile.ai[0])
        {
            case 1: target.AddBuff(BuffID.OnFire, 180); break;
            case 2: target.AddBuff(BuffID.Wet, 180); break;
            case 3: target.velocity.X += Projectile.velocity.X >= 0 ? 5f : -5f; break;
            case 4: target.AddBuff(BuffID.Slow, 120); break;
            case 5: target.AddBuff(BuffID.Electrified, 120); break;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Rectangle rect = new((int)(Projectile.position.X - Main.screenPosition.X),
            (int)(Projectile.position.Y - Main.screenPosition.Y), Projectile.width, Projectile.height);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, NatureColor((int)Projectile.ai[0]));
        return false;
    }

    private static Color NatureColor(int nature) => nature switch
    {
        1 => new Color(235, 80, 35),
        2 => new Color(55, 155, 235),
        3 => new Color(150, 220, 130),
        4 => new Color(165, 125, 80),
        5 => new Color(245, 225, 75),
        _ => Color.White
    };
}
