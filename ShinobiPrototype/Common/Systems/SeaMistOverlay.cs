using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Systems;

// The sea mist over the whole view around the Wave Country bridge, in the grey-white of Zabuza's phase-two mist, but
// made of two layers of a seamless wispy texture (Assets/SeaMist.png, from scripts/make_mist_texture.py) rolling
// with the wind at different speeds and scales, so it is patchy and moving rather than a flat veil (user,
// 2026-09-29). Thickest over the sea and the deck, lighter higher up but never gone. It takes the sky's brightness,
// so it is dim at night, and it is drawn on the ForegroundWater overlay layer so it covers the sea too.
// Thickness follows SeaMistSystem.Density.
public sealed class SeaMistOverlay : Overlay
{
    private const int StripTexels = 4;
    private const int RowsAboveDeck = 30;
    private const float HighMist = 0.45f;
    private const float DeepMist = 0.3f;
    private static readonly Color Tint = new(200, 214, 224); // Zabuza's phase-two mist

    // Two layers for depth: a nearer, larger and faster one over a farther, smaller and slower one.
    private static readonly (float Scale, float Speed, float Alpha, float OffsetY)[] Layers =
    {
        (2.4f, 0.35f, 0.5f, 0f),
        (3.8f, 0.7f, 0.42f, 97f),
    };

    private readonly float[] scroll = new float[Layers.Length];
    private Asset<Texture2D> texture;

    public SeaMistOverlay() : base(EffectPriority.VeryLow, RenderLayers.ForegroundWater)
    {
    }

    public override bool IsVisible() => SeaMistSystem.Density > 0.01f && !Main.gameMenu && WaveBridgeWorld.Site.HasValue;

    public override void Activate(Vector2 position, params object[] args) => Mode = OverlayMode.Active;

    public override void Deactivate(params object[] args) => Mode = OverlayMode.Inactive;

    public override void Update(GameTime gameTime)
    {
    }

    internal void Step()
    {
        float wind = Main.WindForVisuals;
        float direction = wind >= 0f ? 1f : -1f;
        for (int i = 0; i < Layers.Length; i++)
            scroll[i] += Layers[i].Speed * (0.4f + System.Math.Abs(wind)) * direction;
    }

    // How much mist a world row carries: full from just above the deck down to the sea, easing to a lighter mist
    // higher up (never gone) and a little lighter below the sea surface.
    private static float Profile(float worldY, BridgeSite site)
    {
        float top = (site.DeckY - RowsAboveDeck) * 16f;
        float full = (site.DeckY - 3) * 16f;
        float surface = (site.WaterY + 2) * 16f;
        if (worldY <= top)
            return HighMist;
        if (worldY < full)
        {
            float t = (worldY - top) / (full - top);
            return HighMist + (1f - HighMist) * t * t;
        }
        if (worldY <= surface)
            return 1f;
        return System.Math.Max(DeepMist, 1f - (worldY - surface) / (6 * 16f));
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (WaveBridgeWorld.Site is not BridgeSite site)
            return;
        texture ??= ModContent.Request<Texture2D>("ShinobiPrototype/Assets/SeaMist");
        if (!texture.IsLoaded)
            return;

        float density = System.Math.Clamp(SeaMistSystem.Density / 0.9f, 0f, 1.1f);
        Color sky = Main.ColorOfTheSkies.MultiplyRGB(Tint);
        Vector2 view = new Vector2(Main.screenWidth, Main.screenHeight) / Main.GameViewMatrix.Zoom;
        float viewLeft = Main.screenPosition.X + (Main.screenWidth - view.X) / 2f - 32f;
        float viewTop = Main.screenPosition.Y + (Main.screenHeight - view.Y) / 2f - 32f;
        float viewWidth = view.X + 64f;
        float viewBottom = viewTop + view.Y + 64f;

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap,
            DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        Texture2D mist = texture.Value;
        for (int layer = 0; layer < Layers.Length; layer++)
        {
            (float scale, _, float layerAlpha, float offsetY) = Layers[layer];
            float stripHeight = StripTexels * scale;
            float u = (viewLeft + scroll[layer]) / scale;
            int sourceX = (int)System.Math.Floor(u);
            float shift = (u - sourceX) * scale;
            int sourceWidth = (int)(viewWidth / scale) + 3;
            float firstStrip = (float)System.Math.Floor(viewTop / stripHeight) * stripHeight;
            for (float y = firstStrip; y < viewBottom; y += stripHeight)
            {
                float alpha = Profile(y + stripHeight / 2f, site) * layerAlpha * density;
                if (alpha < 0.005f)
                    continue;
                int sourceY = (int)((y + offsetY) / scale);
                Rectangle source = new(sourceX, sourceY, sourceWidth, StripTexels);
                Vector2 position = new(viewLeft - shift - Main.screenPosition.X, y - Main.screenPosition.Y);
                spriteBatch.Draw(mist, position, source, sky * alpha, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState,
            DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
