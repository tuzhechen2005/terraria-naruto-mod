using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Players;

namespace ShinobiPrototype.Common.Systems;

// After the mist preview and before the win: at night or in rain, near the broken end, the player sometimes glimpses
// one of the pair far out in the fog for a few seconds, now and then with a whisper. At most once per night (or
// per spell of rain). Purely local visuals; each client rolls its own. Also shows the brief glimpse of the masked
// figure behind the senbon warning.
public sealed class MistSightingSystem : ModSystem
{
    private static readonly string[] Whispers = { "Whisper1", "Whisper2", "Whisper3", "Whisper4" };

    private static int tick = -1;
    private static int length;
    private static MistFigure who;
    private static Vector2 bottom;
    private static int facing;
    private static bool usedThisSpell;
    private static bool whisper;

    public override void OnWorldUnload()
    {
        tick = -1;
        usedThisSpell = false;
    }

    internal static void Glimpse(MistFigure figure, Vector2 at, int facingDir, int ticks, bool withWhisper)
    {
        who = figure;
        bottom = at;
        facing = facingDir;
        length = ticks;
        whisper = withWhisper;
        tick = 0;
        MistFigures.Puff(at);
    }

    public override void PostUpdateEverything()
    {
        if (Main.dedServ || WaveBridgeWorld.Site is not BridgeSite site || Main.LocalPlayer is not { active: true } player)
            return;

        if (tick >= 0)
        {
            MistFigures.Veil(bottom);
            if (whisper && tick == BridgeRules.SightingWhisperTick)
                MistFigures.Say(bottom, Main.rand.Next(Whispers));
            if (++tick >= length)
            {
                MistFigures.Puff(bottom);
                tick = -1;
            }
            return;
        }

        bool nightOrRain = !Main.dayTime || Main.raining;
        if (!nightOrRain)
            usedThisSpell = false;
        float toBreak = player.Distance(new Vector2(site.X(BridgeDesign.UnfinishedEnd) * 16f, site.DeckY * 16f)) / 16f;
        if (MistPreviewSystem.Playing ||
            !BridgeRules.SightingEligible(WaveBridgeWorld.MistActive, player.GetModPlayer<MistEncounterPlayer>().SawPreview, nightOrRain,
                usedThisSpell, toBreak) ||
            !Main.rand.NextBool(BridgeRules.SightingChanceOneIn))
            return;

        usedThisSpell = true;
        MistFigure figure = Main.rand.NextBool() ? MistFigure.Zabuza : MistFigure.Haku;
        int offset = Main.rand.Next(BridgeRules.SightingNearOffset, BridgeRules.SightingFarOffset + 1);
        Vector2 at = new(site.X(offset) * 16f + 8f, site.WaterY * 16f - (figure == MistFigure.Haku ? 24f : 0f));
        Glimpse(figure, at, -site.Dir, BridgeRules.SightingLength, withWhisper: Main.rand.NextBool());
    }

    internal static void DrawFigures(SpriteBatch spriteBatch)
    {
        if (tick >= 0)
            MistFigures.Draw(spriteBatch, who, bottom, facing, BridgeRules.FigureVisibility(tick, length, 40));
    }
}
