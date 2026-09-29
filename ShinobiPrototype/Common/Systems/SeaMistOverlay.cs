using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;

namespace ShinobiPrototype.Common.Systems;

// The mist veil over the whole view: the sea mist around the Wave Country bridge and Zabuza's transition /
// mist-phase mist (whichever is thicker), in the phase-two grey-white, plus Haku's brief ice flash. Two things keep it from looking wrong at night (user, 2026-09-29): its colour follows the sky's
// brightness (with a floor, so night mist is a dim grey rather than a glowing sheet), and it is drawn on the
// ForegroundWater overlay layer, so the sea is covered instead of showing through as dark blocks.
// Thickness follows SeaMistSystem.Density.
public sealed class SeaMistOverlay : Overlay
{
    private static readonly Color Tint = new(200, 214, 224); // Zabuza's phase-two mist
    private const float NightBrightnessFloor = 0.22f;
    private const float MaxAlpha = 0.9f;

    public SeaMistOverlay() : base(EffectPriority.VeryLow, RenderLayers.ForegroundWater)
    {
    }

    public override bool IsVisible() => !Main.gameMenu && (SeaMistSystem.Density > 0.01f ||
        (Content.NPCs.WaveOverlaySystem.BossMistAlpha > 0.004f || Content.NPCs.WaveOverlaySystem.FlashAlpha > 0.004f));

    public override void Activate(Vector2 position, params object[] args) => Mode = OverlayMode.Active;

    public override void Deactivate(params object[] args) => Mode = OverlayMode.Inactive;

    public override void Update(GameTime gameTime)
    {
    }

    // Brightness of the sky right now, 0..1: full by day, low at night.
    internal static float SkyBrightness()
    {
        Color sky = Main.ColorOfTheSkies;
        return System.Math.Max(NightBrightnessFloor, System.Math.Max(sky.R, System.Math.Max(sky.G, sky.B)) / 255f);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        float alpha = System.Math.Max(System.Math.Min(MaxAlpha, SeaMistSystem.Density),
            Content.NPCs.WaveOverlaySystem.BossMistAlpha);
        Color color = Tint * SkyBrightness();
        color.A = 255;
        Vector2 view = new Vector2(Main.screenWidth, Main.screenHeight) / Main.GameViewMatrix.Zoom;
        Vector2 topLeft = (new Vector2(Main.screenWidth, Main.screenHeight) - view) / 2f;
        Rectangle area = new((int)topLeft.X - 16, (int)topLeft.Y - 16, (int)view.X + 32, (int)view.Y + 32);
        if (alpha > 0.004f)
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, area, color * alpha);
        // The ice flash is a burst of light, so it keeps its brightness at night.
        float flash = Content.NPCs.WaveOverlaySystem.FlashAlpha;
        if (flash > 0.004f)
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, area, new Color(170, 235, 255) * flash);
    }
}
