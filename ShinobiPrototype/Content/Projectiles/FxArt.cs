using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Projectiles;

// Technique effects drawn from their own art (user, 2026-10-01: Orochimaru's purple dust was far too faint; effects
// like Zabuza's): "Content/Projectiles/<Name>.png", or "<Name>_<n>.png" for animations. Without the art the old dust
// stays in place, so every caller checks Has first.
public static class FxArt
{
    private const string Root = "ShinobiPrototype/Content/Projectiles/";

    public static bool Has(string name) => ModContent.HasAsset(Root + name);

    public static Texture2D Get(string name) => ModContent.Request<Texture2D>(Root + name).Value;

    // Frame i of an animation (wrapping), or null if the animation is not there.
    public static Texture2D Frame(string name, int i, int frames)
    {
        string path = $"{name}_{((i % frames) + frames) % frames}";
        return Has(path) ? Get(path) : null;
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
