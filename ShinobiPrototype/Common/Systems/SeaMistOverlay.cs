using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Systems;

// Full-screen sea mist: large, slow puffs using vanilla's graveyard fog-cloud textures, spread over the whole view
// (sky, sea, bridge) and drifting with the wind. Each puff takes the world light where it floats, so the mist is
// dim at night instead of glowing. Drawn on the ForegroundWater overlay layer, so it covers the sea as well.
// Thickness follows SeaMistSystem.Density (distance to the bridge, night or rain, the setting, the preview).
public sealed class SeaMistOverlay : Overlay
{
    private const int PuffCount = 60;
    private const float Margin = 260f;
    private const int FadeInTicks = 90;
    private static readonly Color Tint = new(222, 228, 234);
    private static readonly int[] Textures =
    {
        GoreID.AmbientAirborneCloud1, GoreID.AmbientAirborneCloud3, GoreID.AmbientFloorCloud1,
        GoreID.AmbientFloorCloud2, GoreID.AmbientFloorCloud3,
    };

    private struct Puff
    {
        public Vector2 Position;
        public float Speed;
        public float Scale;
        public float Alpha;
        public float Phase;
        public int Texture;
        public int Age;
        public bool Flip;
    }

    private readonly List<Puff> puffs = new();

    public SeaMistOverlay() : base(EffectPriority.VeryLow, RenderLayers.ForegroundWater)
    {
    }

    public override bool IsVisible() => SeaMistSystem.Density > 0.01f && !Main.gameMenu;

    public override void Activate(Vector2 position, params object[] args) => Mode = OverlayMode.Active;

    public override void Deactivate(params object[] args) => Mode = OverlayMode.Inactive;

    public override void Update(GameTime gameTime)
    {
    }

    private static Rectangle ViewArea()
    {
        Vector2 size = new Vector2(Main.screenWidth, Main.screenHeight) / Main.GameViewMatrix.Zoom;
        Vector2 topLeft = Main.screenPosition + (new Vector2(Main.screenWidth, Main.screenHeight) - size) / 2f;
        return new Rectangle((int)(topLeft.X - Margin), (int)(topLeft.Y - Margin),
            (int)(size.X + 2 * Margin), (int)(size.Y + 2 * Margin));
    }

    // Moves the puffs with the wind and replaces any that leave the view, entering from the upwind side.
    internal void Step()
    {
        Rectangle area = ViewArea();
        float wind = Main.WindForVisuals;
        int windDir = wind >= 0f ? 1 : -1;
        while (puffs.Count < PuffCount)
            puffs.Add(NewPuff(area, anywhere: true, windDir));

        for (int i = 0; i < puffs.Count; i++)
        {
            Puff puff = puffs[i];
            puff.Age++;
            puff.Position.X += puff.Speed * (0.35f + System.Math.Abs(wind)) * windDir;
            puff.Position.Y += 0.12f * (float)System.Math.Sin((puff.Age + puff.Phase) * 0.01f);
            if (!area.Contains(puff.Position.ToPoint()))
                puff = NewPuff(area, anywhere: false, windDir);
            puffs[i] = puff;
        }
    }

    private static Puff NewPuff(Rectangle area, bool anywhere, int windDir)
    {
        float x = anywhere ? Main.rand.NextFloat(area.Left, area.Right)
            : windDir > 0 ? area.Left + Main.rand.NextFloat(40f) : area.Right - Main.rand.NextFloat(40f);
        return new Puff
        {
            Position = new Vector2(x, Main.rand.NextFloat(area.Top, area.Bottom)),
            Speed = Main.rand.NextFloat(0.25f, 0.6f),
            Scale = Main.rand.NextFloat(2.6f, 5.2f),
            Alpha = Main.rand.NextFloat(0.16f, 0.3f),
            Phase = Main.rand.NextFloat(1000f),
            Texture = Main.rand.Next(Textures),
            Age = anywhere ? FadeInTicks : 0,
            Flip = Main.rand.NextBool(),
        };
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        float density = System.Math.Clamp(SeaMistSystem.Density / 0.9f, 0f, 1.1f);
        foreach (Puff puff in puffs)
        {
            Main.instance.LoadGore(puff.Texture);
            Texture2D texture = TextureAssets.Gore[puff.Texture].Value;
            Point tile = puff.Position.ToTileCoordinates();
            Color light = WorldGen.InWorld(tile.X, tile.Y) ? Lighting.GetColor(tile.X, tile.Y) : Color.White;
            float fade = System.Math.Min(1f, puff.Age / (float)FadeInTicks);
            Color color = light.MultiplyRGB(Tint) * (puff.Alpha * density * fade);
            spriteBatch.Draw(texture, puff.Position - Main.screenPosition, null, color, 0f, texture.Size() / 2f,
                puff.Scale, puff.Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }
    }
}
