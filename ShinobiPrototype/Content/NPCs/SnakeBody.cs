using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace ShinobiPrototype.Content.NPCs;

// Orochimaru's snakes drawn in code, chunky pixel blocks like the rest of the art: a head and a tapering body of
// segments trailing behind it, with a wave running from the head to the tail so the body writhes as it goes (user,
// 2026-10-01: the snakes must wriggle). Used for the small summoned snakes and the giant one.
public readonly record struct SnakeLook(Color Body, Color Belly, Color Outline, Color Eye);

public static class SnakeBody
{
    public static readonly SnakeLook Green = new(new Color(84, 140, 62), new Color(196, 200, 120), new Color(22, 40, 20),
        new Color(250, 210, 60));
    public static readonly SnakeLook Purple = new(new Color(104, 72, 132), new Color(206, 184, 150), new Color(28, 18, 40),
        new Color(250, 200, 50));

    // Where segment i (0 = the head) sits: behind the head along the ground, lifted by the travelling wave.
    public static Vector2 Segment(Vector2 headBottom, int dir, int i, int segments, float size, float time)
    {
        float spacing = size * 0.62f;
        float along = i * spacing;
        float amplitude = size * 0.5f * Math.Min(1f, 0.25f + i / 4f) * (1f - 0.4f * i / segments);
        float lift = (float)Math.Sin(time * 0.2f - i * 0.75f) * amplitude;
        float sway = (float)Math.Cos(time * 0.2f - i * 0.75f) * size * 0.08f;
        return headBottom + new Vector2(-dir * along + sway, -SizeAt(i, segments, size) / 2f - Math.Max(0f, lift));
    }

    public static float SizeAt(int i, int segments, float size) =>
        i == 0 ? size * 1.15f : size * (1f - 0.4f * (float)Math.Pow(i / (float)segments, 1.6));

    public static void Draw(SpriteBatch spriteBatch, Vector2 headBottom, int dir, int segments, float size, float time,
        SnakeLook look, Color light, float opacity = 1f)
    {
        Texture2D pixel = TextureAssets.MagicPixel.Value;
        Color Lit(Color c) => c.MultiplyRGB(light) * opacity;
        // Tail first, so the head is drawn on top.
        for (int i = segments - 1; i >= 0; i--)
        {
            Vector2 center = Segment(headBottom, dir, i, segments, size, time) - Main.screenPosition;
            float s = SizeAt(i, segments, size);
            int w = Snap(i == 0 ? s * 1.3f : s), h = Snap(s);
            int x = Snap(center.X - w / 2f), y = Snap(center.Y - h / 2f);
            Block(spriteBatch, pixel, x - 2, y - 2, w + 4, h + 4, Lit(look.Outline));
            Block(spriteBatch, pixel, x, y, w, h, Lit(look.Body));
            // The pale belly along the bottom and a darker stripe down the back, in hard bands.
            Block(spriteBatch, pixel, x, y + h - Snap(h * 0.3f), w, Snap(h * 0.3f), Lit(look.Belly));
            if (i % 2 == 1)
                Block(spriteBatch, pixel, x + w / 4, y, w / 2, Snap(h * 0.25f), Lit(Color.Lerp(look.Body, look.Outline, 0.45f)));
            if (i == 0)
            {
                int eye = Math.Max(2, Snap(s * 0.16f));
                int ex = dir > 0 ? x + w - eye * 3 : x + eye * 2;
                Block(spriteBatch, pixel, ex, y + eye, eye, eye, Lit(look.Eye));
                // The tongue flicks out now and then.
                if ((int)(time / 20f) % 3 == 0)
                {
                    int tx = dir > 0 ? x + w + 2 : x - 2 - eye * 2;
                    Block(spriteBatch, pixel, tx, y + h / 2, eye * 2, Math.Max(2, eye / 2), Lit(new Color(200, 40, 50)));
                }
            }
        }
    }

    private static int Snap(float v) => (int)Math.Round(v / 2f) * 2;

    private static void Block(SpriteBatch spriteBatch, Texture2D pixel, int x, int y, int w, int h, Color color) =>
        spriteBatch.Draw(pixel, new Rectangle(x, y, Math.Max(2, w), Math.Max(2, h)), new Rectangle(0, 0, 1, 1), color);
}
