using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using ShinobiPrototype.Common.Systems;

namespace ShinobiPrototype.Content.Projectiles;

// Technique effects drawn from their own art (user, 2026-10-01: Orochimaru's purple dust was far too faint; effects
// like Zabuza's): "Content/Projectiles/<Name>.png", or "<Name>_<n>.png" for animations. Without the art the old dust
// stays in place, so every caller checks Has first.
public static class FxArt
{
    public static bool Has(string name) => ClientVisualAssets.Has(ClientVisualAssets.Projectiles, name);

    public static Texture2D Get(string name) => ClientVisualAssets.Get(ClientVisualAssets.Projectiles, name);

    // Frame i of an animation (wrapping), or null if the animation is not there.
    public static Texture2D Frame(string name, int i, int frames)
    {
        return ClientVisualAssets.Frame(ClientVisualAssets.Projectiles, name, i, frames);
    }

    // Drawn centred, at whole pixels; facing left mirrors art drawn facing right.
    public static void Draw(Texture2D texture, Vector2 worldCenter, Color color, float rotation = 0f, float scale = 1f, int facing = 1)
    {
        Vector2 at = worldCenter - Main.screenPosition;
        at = new Vector2((float)System.Math.Round(at.X), (float)System.Math.Round(at.Y));
        Main.EntitySpriteDraw(texture, at, null, color, rotation, texture.Size() / 2f, scale,
            facing < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
    }
}
