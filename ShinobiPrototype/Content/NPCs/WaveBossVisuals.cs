using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Systems;

namespace ShinobiPrototype.Content.NPCs;

// Frame-strip sprites for the Wave Country bosses. New art is delivered as one PNG per
// frame (`<Prefix>_<Action>_<n>`) on a fixed canvas; until a frame exists callers fall
// back to the older single-pose textures.
internal static class BossSprites
{
    public readonly record struct Canvas(int Width, int Height, int CenterX, int BaselineY);

    public static readonly Canvas Zabuza = new(288, 128, 144, 124);
    public static readonly Canvas ZabuzaAura = new(160, 240, 80, 239); // purple demon apparition
    public static readonly Canvas Haku = new(144, 96, 72, 92);
    public static readonly Canvas HakuEmerge = new(160, 96, 80, 92); // frames include the mirror

    private static string AssetPath(string name) => $"ShinobiPrototype/Content/NPCs/{name}";

    public static bool Has(string name) => ModContent.HasAsset(AssetPath(name));

    public static bool TryDraw(SpriteBatch spriteBatch, string prefix, string action, int frame,
        int frameCount, Canvas canvas, Vector2 worldBottom, int direction, Color color,
        Vector2 screenPos)
    {
        int index = ((frame % frameCount) + frameCount) % frameCount;
        string path = AssetPath($"{prefix}_{action}_{index}");
        if (!ModContent.HasAsset(path))
            return false;
        Texture2D texture = ModContent.Request<Texture2D>(path).Value;
        bool facingRight = direction >= 0;
        Vector2 position = worldBottom - screenPos;
        position = new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));
        Vector2 origin = new(facingRight ? canvas.CenterX : canvas.Width - canvas.CenterX,
            canvas.BaselineY);
        spriteBatch.Draw(texture, position, null, color, 0f, origin, 1f,
            facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        return true;
    }

    // World-lit like vanilla characters, with a floor so a boss never vanishes in the dark.
    public static Color Lit(Color drawColor, float floor = 0.3f)
    {
        int min = (int)(255 * floor);
        return new Color(Math.Max(drawColor.R, min), Math.Max(drawColor.G, min),
            Math.Max(drawColor.B, min), drawColor.A);
    }

    public static int Progress(float elapsed, float duration, int frameCount) =>
        Math.Clamp((int)(elapsed * frameCount / Math.Max(1f, duration)), 0, frameCount - 1);

    public static int Loop(float ticksPerFrame, int frameCount) =>
        (int)(Main.GameUpdateCount / ticksPerFrame) % frameCount;
}

internal static class BossLines
{
    // Each client runs boss AI, so lines are shown locally; nothing is broadcast twice.
    public static void Say(NPC npc, string key, Color color)
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        string text = Language.GetTextValue($"Mods.ShinobiPrototype.Dialogue.{key}");
        int colon = text.IndexOf('：') >= 0 ? text.IndexOf('：') : text.IndexOf(':');
        CombatText.NewText(npc.Hitbox, color, colon >= 0 ? text[(colon + 1)..].Trim() : text,
            dramatic: true);
        Main.NewText(text, color);
    }
}

// Screen mist during Zabuza's transition and mist phase, the sea mist around the Wave Country bridge (same colour,
// same layer) and a brief ice flash when Haku goes berserk.
public sealed class WaveOverlaySystem : ModSystem
{
    private static float fog;
    private static float flash;

    public static void Flash() => flash = 1f;

    public override void PostUpdateEverything()
    {
        if (Main.dedServ)
            return;
        float target = 0f;
        int zabuza = ModContent.NPCType<ZabuzaBoss>();
        foreach (NPC npc in Main.ActiveNPCs)
        {
            if (npc.type != zabuza)
                continue;
            if ((int)npc.ai[0] == ZabuzaCombatRules.MistTransition)
                target = ZabuzaCombatRules.TransitionFog(npc.ai[1]);
            else if (ZabuzaCombatRules.InMistPhase(npc.life, npc.lifeMax) && npc.ai[3] < 3f)
                target = 0.3f;
        }
        target = Math.Max(target, Math.Max(SeaFogTarget(), MistPreviewSystem.FogBoost));
        fog = MathHelper.Lerp(fog, target, 0.05f);
        fog = Math.Min(fog, 2f);
        flash = Math.Max(0f, flash - 1f / 45f);
        // Thicker mist, more drifting puffs.
        if (fog > 0.1f && Main.GameUpdateCount % 2 == 0)
            for (int i = 0; i < 1 + (int)(fog * 3f); i++)
                DriftPuff();
    }

    private static float SeaFogTarget()
    {
        if (!WaveBridgeWorld.MistActive || Main.LocalPlayer is not { active: true } player)
            return 0f;
        return BridgeRules.SeaFog(WaveBridgeWorld.DistanceToBridgeTiles(player.Center),
            !Main.dayTime || Main.raining, ShinobiClientConfig.Instance.SeaFogStrength / 100f);
    }

    // Slow grey-white puffs drifting across the view, so the mist reads as moving air rather than a tint.
    private static void DriftPuff()
    {
        Vector2 at = Main.screenPosition + new Vector2(Main.rand.NextFloat(Main.screenWidth), Main.rand.NextFloat(Main.screenHeight));
        Dust puff = Dust.NewDustPerfect(at, DustID.Smoke, new Vector2(Main.rand.NextFloat(0.2f, 0.6f), 0f),
            140, new Color(200, 214, 224), Main.rand.NextFloat(2.6f, 4f));
        puff.noGravity = true;
        puff.noLight = true;
    }

    public override void OnWorldUnload()
    {
        fog = 0f;
        flash = 0f;
    }

    public override void PostDrawTiles()
    {
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState,
            DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        MistPreviewSystem.DrawFigures(Main.spriteBatch);
        MistSightingSystem.DrawFigures(Main.spriteBatch);
        Main.spriteBatch.End();

        if (fog < 0.01f && flash < 0.01f)
            return;
        Rectangle screen = new(0, 0, Main.screenWidth, Main.screenHeight);
        Main.spriteBatch.Begin();
        if (fog >= 0.01f)
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, screen,
                new Color(200, 214, 224) * (0.42f * fog));
        if (flash >= 0.01f)
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, screen,
                new Color(170, 235, 255) * (0.5f * flash));
        Main.spriteBatch.End();
    }
}
