using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;

namespace ShinobiPrototype.Content.Projectiles;

// The warning senbon: flies out of the mist to a point on the deck at the player's feet, sticks there with a
// "ting", then fades. It never deals damage.
public sealed class SenbonWarning : ModProjectile
{
    public override string Texture => "ShinobiPrototype/Content/Projectiles/HakuSenbon";

    private Vector2 Target => new(Projectile.ai[0], Projectile.ai[1]);
    private ref float StuckTicks => ref Projectile.localAI[0];

    public override void SetDefaults()
    {
        Projectile.width = 8;
        Projectile.height = 8;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 600;
    }

    public override void AI()
    {
        if (StuckTicks > 0f)
        {
            Projectile.velocity = Vector2.Zero;
            if (++StuckTicks > BridgeRules.SenbonStuckTicks)
                Projectile.Kill();
            return;
        }

        Projectile.rotation = Projectile.velocity.ToRotation();
        if (Projectile.localAI[1]++ == 0f)
            SoundEngine.PlaySound(SoundID.Item1 with { Pitch = 0.5f }, Projectile.Center);
        if (Vector2.Distance(Projectile.Center, Target) <= BridgeRules.SenbonSpeed)
        {
            Projectile.Center = Target;
            StuckTicks = 1f;
            SoundEngine.PlaySound(SoundID.Tink, Projectile.Center);
            for (int i = 0; i < 6; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
        float fade = StuckTicks > BridgeRules.SenbonStuckTicks - 30
            ? (BridgeRules.SenbonStuckTicks - StuckTicks) / 30f
            : 1f;
        // The needle's tip is its right end; when stuck, the tip sits at the impact point.
        Vector2 origin = StuckTicks > 0f ? new Vector2(texture.Width - 2, texture.Height / 2f) : texture.Size() / 2f;
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(225, 252, 255) * fade,
            Projectile.rotation, origin, 1.5f, SpriteEffects.None);
        return false;
    }
}
