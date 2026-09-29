using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Common.Systems;

internal enum MistFigure { Zabuza, Haku }

// Zabuza and Haku as they appear before the fight: a dark silhouette in the mist with only a trace of detail,
// never fully clear, veiled by extra fog. Names stay hidden ("？？？") until the boss bar reveals them.
internal static class MistFigures
{
    private static readonly Color Silhouette = new(28, 34, 46);
    public static readonly Color VoiceColor = new(175, 195, 215);

    public static void Draw(SpriteBatch spriteBatch, MistFigure who, Vector2 bottom, int facing, float visibility)
    {
        if (visibility <= 0.01f)
            return;
        (string prefix, BossSprites.Canvas canvas) = who == MistFigure.Zabuza
            ? ("Zabuza", BossSprites.Zabuza)
            : ("Haku", BossSprites.Haku);
        int frame = BossSprites.Loop(8f, 4);
        BossSprites.TryDraw(spriteBatch, prefix, "Idle", frame, 4, canvas, bottom, facing,
            Silhouette * visibility, Main.screenPosition);
        Color lit = BossSprites.Lit(Lighting.GetColor((int)(bottom.X / 16f), (int)(bottom.Y / 16f) - 2), 0.5f);
        float detail = BridgeRules.FigureDetail * visibility / BridgeRules.FigureMaxVisibility;
        BossSprites.TryDraw(spriteBatch, prefix, "Idle", frame, 4, canvas, bottom, facing,
            lit * detail, Main.screenPosition);
    }

    // Fog drifting across the figure's body, drawn over it by the dust layer.
    public static void Veil(Vector2 bottom)
    {
        if (Main.GameUpdateCount % 5 != 0)
            return;
        Vector2 at = bottom + new Vector2(Main.rand.NextFloat(-30f, 30f), -Main.rand.NextFloat(0f, 90f));
        Dust puff = Dust.NewDustPerfect(at, DustID.Smoke, new Vector2(Main.rand.NextFloat(-0.3f, 0.3f), -0.1f),
            120, new Color(200, 214, 224), Main.rand.NextFloat(2.2f, 3.2f));
        puff.noGravity = true;
        puff.noLight = true;
    }

    public static void Puff(Vector2 bottom)
    {
        for (int i = 0; i < 30; i++)
            Dust.NewDustPerfect(bottom + new Vector2(Main.rand.NextFloat(-24f, 24f), -Main.rand.NextFloat(0f, 90f)),
                DustID.Smoke, Main.rand.NextVector2Circular(1.5f, 1.5f), 100, new Color(210, 220, 230), 2f).noGravity = true;
    }

    // A line from the mist: floating text by the figure and the full line (with its "？？？：" speaker) in chat.
    public static void Say(Vector2 bottom, string key)
    {
        string text = Language.GetTextValue($"Mods.ShinobiPrototype.Dialogue.{key}");
        int colon = text.IndexOf('：');
        Rectangle area = new((int)bottom.X - 20, (int)bottom.Y - 90, 40, 90);
        CombatText.NewText(area, VoiceColor, colon >= 0 ? text[(colon + 1)..] : text, dramatic: true);
        Main.NewText(text, VoiceColor);
    }
}
