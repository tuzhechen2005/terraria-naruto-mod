using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace ShinobiPrototype.Content.Projectiles;

internal static class ZabuzaWaterVisuals
{
    private static readonly Vector2[] Directions =
    {
        Vector2.UnitX, -Vector2.UnitX, Vector2.UnitY, -Vector2.UnitY
    };

    internal static bool Draw(Projectile projectile, Color normalRim, float scale)
    {
        Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
        Vector2 origin = texture.Size() * 0.5f;
        Color rim = projectile.ai[0] > 0.5f ? new Color(225, 105, 255) : normalRim;
        float rotation = projectile.rotation;
        for (int i = 4; i >= 2; i -= 2)
        {
            if (projectile.oldPos[i] == Vector2.Zero)
                continue;
            Vector2 trail = projectile.oldPos[i] + projectile.Size * 0.5f - Main.screenPosition;
            Main.spriteBatch.Draw(texture, trail, null, rim * (i == 2 ? 0.32f : 0.18f),
                rotation, origin, scale, SpriteEffects.None, 0f);
        }

        Vector2 center = projectile.Center - Main.screenPosition;
        foreach (Vector2 direction in Directions)
            Main.spriteBatch.Draw(texture, center + direction * 3f, null,
                new Color(7, 25, 48) * 0.92f, rotation, origin, scale,
                SpriteEffects.None, 0f);
        foreach (Vector2 direction in Directions)
            Main.spriteBatch.Draw(texture, center + direction * 1.5f, null,
                rim * 0.8f, rotation, origin, scale, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(texture, center, null, Color.White, rotation, origin, scale,
            SpriteEffects.None, 0f);
        return false;
    }
}
