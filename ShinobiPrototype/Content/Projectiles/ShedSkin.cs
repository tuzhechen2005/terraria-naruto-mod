using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Projectiles;

// What is left where the disguised Orochimaru was struck down: the candidate's body gone grey and limp like a shed
// skin, lying on the ground, fading after a while. Harmless; ai[0] is the way the body faced.
public sealed class ShedSkin : ModProjectile
{
    private const int LifeTicks = 60 * 8;

    public override string Texture => "ShinobiPrototype/Content/NPCs/ForestCanopyCandidate";

    public override void SetDefaults()
    {
        Projectile.width = 56;
        Projectile.height = 14;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.penetrate = -1;
        Projectile.tileCollide = true;
        Projectile.timeLeft = LifeTicks;
        Projectile.aiStyle = -1;
    }

    public override void AI()
    {
        Projectile.velocity.X = 0f;
        Projectile.velocity.Y = System.Math.Min(Projectile.velocity.Y + 0.3f, 8f);
        if (Projectile.localAI[0]++ == 0f && !Main.dedServ)
            for (int i = 0; i < 30; i++)
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(30f, 20f), DustID.Smoke,
                    Main.rand.NextVector2Circular(1.5f, 1.5f), 120, new Color(200, 200, 190), 1.5f);
        if (Projectile.timeLeft < 60)
            Projectile.alpha = (int)(255 * (1f - Projectile.timeLeft / 60f));
    }

    public override bool OnTileCollide(Vector2 oldVelocity) => false;

    // The candidate's first frame laid on its side, washed out to a pale skin colour.
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Rectangle frame = new(0, 0, texture.Width, texture.Height / 7);
        Color skin = new Color(214, 206, 186).MultiplyRGB(lightColor) * Projectile.Opacity;
        int facing = Projectile.ai[0] >= 0f ? 1 : -1;
        Main.EntitySpriteDraw(texture, Projectile.Bottom - Main.screenPosition + new Vector2(0f, -8f), frame, skin,
            facing * MathHelper.PiOver2, new Vector2(frame.Width / 2f, frame.Height - 6f), 1f,
            facing > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
        return false;
    }
}
