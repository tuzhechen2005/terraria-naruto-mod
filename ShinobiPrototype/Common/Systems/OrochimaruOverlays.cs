using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Common.Systems;

// Orochimaru's killing intent on screen (user, 2026-10-01): while the player is frozen by it, the edges of the view
// darken to blood red and a pair of huge snake eyes looks down from the top (orochimaru-fx-eyes-v1, FxKillingEyes);
// it all fades when the fear breaks (substitution or time).
public sealed class KillingIntentOverlay : ModSystem
{
    private static float shown;

    public override void PostUpdateEverything()
    {
        if (Main.dedServ)
            return;
        bool afraid = Main.LocalPlayer.active && Main.LocalPlayer.GetModPlayer<JutsuStatusPlayer>().FearTicks > 0;
        shown = MathHelper.Clamp(shown + (afraid ? 0.12f : -0.06f), 0f, 1f);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name == "Vanilla: Interface Logic 1");
        layers.Insert(Math.Max(0, index), new LegacyGameInterfaceLayer("ShinobiPrototype: Killing Intent", () =>
        {
            if (shown > 0.01f)
                Draw(Main.spriteBatch);
            return true;
        }, InterfaceScaleType.UI));
    }

    private static void Draw(SpriteBatch spriteBatch)
    {
        Texture2D pixel = TextureAssets.MagicPixel.Value;
        int w = Main.screenWidth, h = Main.screenHeight;
        // Bands of dark red, thickest at the edges.
        const int bands = 10;
        int depth = Math.Min(w, h) / 5;
        for (int i = 0; i < bands; i++)
        {
            int inset = depth * i / bands, thick = depth / bands + 1;
            Color red = new Color(70, 0, 8) * (shown * 0.75f * (1f - i / (float)bands));
            spriteBatch.Draw(pixel, new Rectangle(inset, inset, w - inset * 2, thick), new Rectangle(0, 0, 1, 1), red);
            spriteBatch.Draw(pixel, new Rectangle(inset, h - inset - thick, w - inset * 2, thick), new Rectangle(0, 0, 1, 1), red);
            spriteBatch.Draw(pixel, new Rectangle(inset, inset, thick, h - inset * 2), new Rectangle(0, 0, 1, 1), red);
            spriteBatch.Draw(pixel, new Rectangle(w - inset - thick, inset, thick, h - inset * 2), new Rectangle(0, 0, 1, 1), red);
        }
        if (FxArt.Has("FxKillingEyes"))
        {
            Texture2D eyes = FxArt.Get("FxKillingEyes");
            float scale = 3f;
            Vector2 at = new((float)Math.Round(w / 2f), (float)Math.Round(h * 0.22f));
            spriteBatch.Draw(eyes, at, null, Color.White * shown, 0f, eyes.Size() / 2f, scale, SpriteEffects.None, 0f);
        }
    }
}

// The Five Elements Seal's mark on a player's body for a moment after it lands (orochimaru-fx-seal-v1, FxSealGlyph).
public sealed class SealGlyphLayer : PlayerDrawLayer
{
    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.FrontAccFront);

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) =>
        drawInfo.drawPlayer.GetModPlayer<JutsuStatusPlayer>().SealGlyphTicks > 0 && FxArt.Has("FxSealGlyph");

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        int ticks = drawInfo.drawPlayer.GetModPlayer<JutsuStatusPlayer>().SealGlyphTicks;
        float fade = Math.Min(1f, ticks / 20f);
        Texture2D glyph = FxArt.Get("FxSealGlyph");
        Vector2 at = drawInfo.drawPlayer.Center - Main.screenPosition + new Vector2(0f, 2f);
        drawInfo.DrawDataCache.Add(new DrawData(glyph, new Vector2((float)Math.Round(at.X), (float)Math.Round(at.Y)), null,
            Color.White * fade, 0f, glyph.Size() / 2f, 1f, SpriteEffects.None));
    }
}
