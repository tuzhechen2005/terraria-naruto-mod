using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace ShinobiPrototype.Common.Systems;

// M11 presentation helpers. Callers supply synced attack geometry and phase;
// nothing here chooses targets, creates damage, modifies tiles or advances AI.
// SpriteBatch stays in the caller's world-space AlphaBlend pass.
public static class WaveVfx
{
    public const string Folder = "Assets/Vfx/Wave";
    public const int MaxDragonSegments = 24;
    public const int MaxNeedleSlots = 16;
    private static readonly Color Water = new(95, 225, 232);
    private static readonly Color Ice = new(190, 242, 255);

    private static bool Draw(SpriteBatch batch, string asset, Vector2 world, Vector2 size,
        float alpha = 1f, float rotation = 0f, bool flip = false, Color? tint = null,
        Vector2? origin = null, Rectangle? source = null)
    {
        if (Main.dedServ || !float.IsFinite(alpha) || alpha <= 0f ||
            !float.IsFinite(rotation) ||
            !float.IsFinite(world.X) || !float.IsFinite(world.Y) ||
            !float.IsFinite(size.X) || !float.IsFinite(size.Y) || size.X <= 0f || size.Y <= 0f)
            return false;
        Texture2D art = ClientVisualAssets.Get(Folder, asset);
        if (art == null)
            return false;
        Vector2 contentSize = source.HasValue ? new Vector2(source.Value.Width, source.Value.Height) : art.Size();
        Vector2 point = world - Main.screenPosition;
        float reach = Math.Max(size.X, size.Y);
        Vector2 zoom = Main.GameViewMatrix.Zoom;
        Vector2 view = new Vector2(Main.screenWidth, Main.screenHeight) / zoom;
        Vector2 topLeft = (new Vector2(Main.screenWidth, Main.screenHeight) - view) * 0.5f;
        if (point.X + reach < topLeft.X || point.Y + reach < topLeft.Y ||
            point.X - reach > topLeft.X + view.X || point.Y - reach > topLeft.Y + view.Y)
            return false;
        batch.Draw(art, point, source, (tint ?? Color.White) * Math.Clamp(alpha, 0f, 1f),
            rotation, origin ?? contentSize * 0.5f, size / contentSize,
            flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        return true;
    }

    private static float Phase(float value) => float.IsFinite(value) ? Math.Clamp(value, 0f, 1f) : 0f;
    private static float Decoration => Math.Clamp(ShinobiClientConfig.Instance.WaveVfxStrength, 0, 100) / 100f;

    public static bool Ready(string name) => ClientVisualAssets.Get(Folder, name) != null;

    public static void Gather(SpriteBatch batch, Vector2 hands, float phase, float size = 150f)
    {
        float p = Phase(phase);
        Draw(batch, "WaterGather", hands, new Vector2(size * (0.65f + p * 0.35f)),
            (0.2f + p * 0.55f) * Decoration, p * MathHelper.Pi);
    }

    // This warning is mandatory regardless of decoration strength. Use the
    // SAME locked endpoints and lane width as the impending attack.
    public static void LockedRoute(SpriteBatch batch, Vector2 from, Vector2 to, float laneWidth,
        float phase, bool ice = false)
    {
        if (Main.dedServ || !float.IsFinite(laneWidth) || laneWidth <= 0f)
            return;
        Vector2 span = to - from;
        float length = span.Length();
        if (!float.IsFinite(length) || length < 1f)
            return;
        float angle = span.ToRotation();
        Vector2 normal = new(-span.Y / length, span.X / length);
        Color color = ice ? Ice : Water;
        float alpha = 0.4f + Phase(phase) * 0.4f;
        Texture2D pixel = TextureAssets.MagicPixel.Value;
        for (int side = -1; side <= 1; side += 2)
            batch.Draw(pixel, from + normal * laneWidth * 0.5f * side - Main.screenPosition,
                null, color * alpha, angle, Vector2.Zero, new Vector2(length, 2f), SpriteEffects.None, 0f);
    }

    // Centers and width are the caller's actual damage corridor. The caller
    // keeps the locked path across clients; visual flow never retargets it.
    public static void Dragon(SpriteBatch batch, ReadOnlySpan<Vector2> path, float coreWidth,
        float phase, bool dispersing = false)
    {
        if (path.Length < 2 || !float.IsFinite(coreWidth) || coreWidth <= 0f)
            return;
        int count = Math.Min(path.Length, MaxDragonSegments + 1);
        float p = Phase(phase);
        float alpha = dispersing ? 1f - p : 1f;
        for (int i = 1; i < count; i++)
        {
            Vector2 span = path[i] - path[i - 1];
            float length = span.Length();
            if (!float.IsFinite(length) || length < 1f)
                continue;
            Draw(batch, "WaterDragonBody", (path[i - 1] + path[i]) * 0.5f,
                new Vector2(length * 1.18f, coreWidth), alpha, span.ToRotation(),
                source: new Rectangle(50, 6, 155, 84));
        }
        Vector2 headDirection = path[count - 1] - path[count - 2];
        Draw(batch, "WaterDragonHead", path[count - 1], new Vector2(coreWidth * 137f / 116f, coreWidth),
            alpha, headDirection.ToRotation(), source: new Rectangle(27, 6, 137, 116));
        if (dispersing)
            Splash(batch, path[count - 1], p, coreWidth * 2f);
    }

    public static void Slash(SpriteBatch batch, Vector2 blade, int direction, float phase,
        float size = 180f, bool frenzy = false)
    {
        float p = Phase(phase);
        Draw(batch, frenzy ? "SwordSpinArc" : "WaterSlash", blade,
            new Vector2(size, size * 0.75f), (1f - p) * Decoration,
            (p - 0.5f) * 0.7f * Math.Sign(direction), direction < 0);
    }

    public static void Splash(SpriteBatch batch, Vector2 impact, float phase, float size = 180f)
    {
        float p = Phase(phase);
        Draw(batch, "WaterSplash", impact, new Vector2(size * (0.65f + p * 0.7f), size * 0.5f),
            (1f - p) * Decoration);
    }

    public static void Mist(SpriteBatch batch, Vector2 landing, float phase, float size = 90f)
    {
        float p = Phase(phase);
        // Landing warning retains a minimum opacity with decoration off.
        Draw(batch, "MistPuff", landing, new Vector2(size * (0.8f + p * 0.2f)),
            (0.4f + Decoration * 0.3f) * (1f - p * 0.4f));
    }

    public static void Mirror(SpriteBatch batch, Vector2 center, Vector2 size, float formation,
        float damage, float glow, bool frenzy = false, Texture2D silhouette = null)
    {
        float form = Phase(formation), bright = Phase(glow), crack = Phase(damage);
        Vector2 forming = new(size.X * (0.25f + form * 0.75f), size.Y);
        // Mirror and active-mirror indicator survive all decoration settings.
        Draw(batch, "IceMirror", center, forming, 0.45f + form * 0.55f);
        if (silhouette != null)
            Silhouette(batch, silhouette, silhouette.Bounds, center, silhouette.Size() * 0.5f,
                size.Y * 0.65f / silhouette.Height, false);
        if (crack > 0f)
            Draw(batch, "MirrorCracks", center, forming * new Vector2(0.75f, 0.92f), crack);
        if (bright > 0f || frenzy)
            Draw(batch, "MirrorHalo", center, size * new Vector2(0.75f, 1.03f) * (frenzy ? 1.08f : 1f),
                Math.Max(bright * 0.8f, frenzy ? 0.28f : 0f));
        Draw(batch, "FrostCloud", center, size * 0.85f, 0.14f * Decoration);
    }

    // Reuse Haku's current silhouette; do not bake another character into a
    // mirror texture. Caller draws this between the surface and crack layers.
    public static void Silhouette(SpriteBatch batch, Texture2D body, Rectangle source,
        Vector2 center, Vector2 origin, float scale, bool flip)
    {
        if (Main.dedServ || body == null)
            return;
        batch.Draw(body, center - Main.screenPosition, source, new Color(45, 90, 118) * 0.35f,
            0f, origin, scale, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
    }

    public static void Shatter(SpriteBatch batch, Vector2 center, float phase, float size = 100f)
    {
        float p = Phase(phase);
        Draw(batch, "MirrorShards", center, new Vector2(size * (0.7f + p)),
            (1f - p) * Decoration, p * 0.12f);
    }

    public static void Needle(SpriteBatch batch, Vector2 tip, float angle, float warning = 0f)
    {
        // Tip is the real needle endpoint. The soft tail is decoration only.
        Draw(batch, "NeedleGlow", tip, new Vector2(40f, 13f),
            warning > 0f ? 0.5f + Phase(warning) * 0.5f : 0.8f, angle,
            origin: new Vector2(84f, 16f));
    }

    public static void NeedleArray(SpriteBatch batch, ReadOnlySpan<Vector2> positions,
        Vector2 lockedAim, int gapStart, int gapCount, float warning)
    {
        int count = Math.Min(positions.Length, MaxNeedleSlots);
        if (count == 0)
            return;
        int start = ((gapStart % count) + count) % count;
        int gap = Math.Clamp(gapCount, 0, count);
        for (int i = 0; i < count; i++)
        {
            if ((i - start + count) % count < gap)
                continue;
            Vector2 heading = lockedAim - positions[i];
            Needle(batch, positions[i], heading.ToRotation(), Phase(warning));
        }
    }

    public static void Ghost(SpriteBatch batch, Vector2 bottom, float phase, bool pressing,
        int direction, float height = 240f)
    {
        float p = Phase(phase);
        Vector2 size = new(height * 0.8f, height);
        // The source baseline is (128,300) on a 256x320 cell.
        Vector2 center = bottom - new Vector2(0f, height * (300f / 320f - 0.5f));
        if (pressing)
        {
            float blend = MathHelper.SmoothStep(0f, 1f, p);
            Draw(batch, "DemonGhost", center, size, 0.55f * (1f - blend) * Decoration,
                0f, direction < 0);
            Draw(batch, "DemonPress", center, size, 0.6f * blend * Decoration,
                0f, direction < 0);
        }
        else
            Draw(batch, "DemonGhost", center, size,
                (0.4f + p * 0.2f) * Decoration, 0f, direction < 0);
    }

    public static void SwordTrail(SpriteBatch batch, Vector2 blade, float angle, float size = 110f)
    {
        Draw(batch, "SwordSpinArc", blade, new Vector2(size), 0.6f * Decoration, angle);
    }

    public static void ChakraImpact(SpriteBatch batch, Vector2 hand, float phase, float size = 120f)
    {
        float p = Phase(phase);
        Draw(batch, "ChakraImpact", hand, new Vector2(size * (0.6f + p)),
            (1f - p) * Decoration);
    }
}
