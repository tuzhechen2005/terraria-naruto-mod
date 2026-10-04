using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Items.Jutsu;

namespace ShinobiPrototype.Common.Systems;

// While the local player forms hand seals: the zodiac signs of the scroll over their head, lit as each seal forms, and
// its name under them.
public sealed class SealOverlay : ModSystem
{
    private static readonly string[] Fallback = { "Rat", "Ox", "Tiger", "Hare", "Dragon", "Snake" };

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name == "Vanilla: Entity Health Bars");
        layers.Insert(index < 0 ? 0 : index, new LegacyGameInterfaceLayer("ShinobiPrototype: Hand Seals", Draw, InterfaceScaleType.Game));
    }

    private const string Ui = "ShinobiPrototype/Assets/UI/";
    // Where the fill sits inside the frame art (chidori-bar-v1).
    private static readonly Vector2 FillOffset = new(6f, 5f);

    // The Chidori charge bar (chidori-bar-v1): the frame, the fill showing from the left as it gathers, a spark at its
    // head, and a white flash as it fills. Without the art, plain bars.
    private static void DrawGatherBar(Vector2 centre, float progress)
    {
        var sb = Main.spriteBatch;
        progress = MathHelper.Clamp(progress, 0f, 1f);
        if (ModContent.HasAsset(Ui + "ChidoriBarFrame") && ModContent.HasAsset(Ui + "ChidoriBarFill"))
        {
            var frame = ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(Ui + "ChidoriBarFrame").Value;
            var fill = ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(Ui + "ChidoriBarFill").Value;
            Vector2 at = new((float)System.Math.Round(centre.X - frame.Width / 2f), (float)System.Math.Round(centre.Y - frame.Height / 2f));
            sb.Draw(frame, at, Color.White);
            int shown = (int)(fill.Width * progress);
            if (shown > 0)
                sb.Draw(fill, at + FillOffset, new Rectangle(0, 0, shown, fill.Height), Color.White);
            Vector2 head = at + FillOffset + new Vector2(shown, fill.Height / 2f);
            string spark = $"ChidoriBarSpark_{Main.GameUpdateCount / 3 % 3}";
            if (shown > 0 && ModContent.HasAsset(Ui + spark))
            {
                var t = ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(Ui + spark).Value;
                sb.Draw(t, head - t.Size() / 2f, Color.White);
            }
            if (progress > 0.93f)
                sb.Draw(fill, at + FillOffset, null, new Color(255, 255, 255, 0) * ((progress - 0.93f) / 0.07f) * 0.8f);
            return;
        }
        var pixel = TextureAssets.MagicPixel.Value;
        Vector2 corner = centre - new Vector2(30f, 3f);
        sb.Draw(pixel, new Rectangle((int)corner.X - 2, (int)corner.Y - 2, 64, 10), Color.Black * 0.75f);
        sb.Draw(pixel, new Rectangle((int)corner.X, (int)corner.Y, (int)(60 * progress), 6),
            Color.Lerp(new Color(90, 150, 255), new Color(220, 245, 255), progress));
    }

    private static bool Draw()
    {
        Player player = Main.LocalPlayer;
        if (Main.gameMenu || player is not { active: true } || player.dead)
            return true;
        SealPlayer seals = player.GetModPlayer<SealPlayer>();
        // Chidori gathering: a bar over the head, filling.
        if (Content.Projectiles.ChidoriCharge.GatherProgress(player) is float gathered)
        {
            DrawGatherBar(player.Top - Main.screenPosition - new Vector2(0f, 22f), gathered);
            return true;
        }
        if (!seals.Weaving)
            return true;
        int formed = seals.Seals;
        SealScroll forming = seals.Forming;
        string[] signs = forming?.Signs ?? Fallback;
        Vector2 top = player.Top - Main.screenPosition - new Vector2(0f, 40f);
        // Each sign by its name in the player's language (one character in Chinese, a word in English), spaced by width.
        var font = FontAssets.MouseText.Value;
        string[] names = new string[signs.Length];
        float[] widths = new float[signs.Length];
        float total = 0f;
        const float gap = 8f;
        for (int i = 0; i < signs.Length; i++)
        {
            names[i] = Loc.Get("Seal.Sign." + signs[i]);
            widths[i] = System.Math.Max(14f, font.MeasureString(names[i]).X);
            total += widths[i] + (i > 0 ? gap : 0f);
        }
        float x = top.X - total / 2f;
        for (int i = 0; i < signs.Length; i++)
        {
            bool done = i < formed;
            Utils.DrawBorderString(Main.spriteBatch, names[i], new Vector2(x + widths[i] / 2f, top.Y),
                done ? new Color(140, 210, 255) : new Color(90, 100, 120) * 0.7f, done ? 1f : 0.85f, 0.5f, 0.5f);
            x += widths[i] + gap;
        }
        if (forming != null)
            Utils.DrawBorderString(Main.spriteBatch, forming.Item.Name, top + new Vector2(0f, 22f), new Color(255, 225, 150), 0.75f, 0.5f, 0.5f);
        return true;
    }
}
