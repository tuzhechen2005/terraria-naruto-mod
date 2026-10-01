using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Common.Systems;

// The mist preview at the bridge, once per character (started by MistEncounterPlayer): Zabuza on the water beyond
// the broken end, then Haku in her hunter mask, then both vanish. Pure client-side visuals and text for the player
// who triggered it, so nothing can be hit and nothing is synced.
public sealed class MistPreviewSystem : ModSystem
{
    private static int tick = -1;

    public static float MistBoost => BridgeRules.PreviewMistBoost(tick);

    public static void Play() => tick = 0;

    public override void OnWorldUnload() => tick = -1;

    public override void PostUpdateEverything()
    {
        if (tick < 0 || Main.dedServ || WaveBridgeWorld.Site is not BridgeSite site)
            return;

        switch (tick)
        {
            case BridgeRules.PreviewZabuzaAppear:
                MistFigures.Puff(ZabuzaBottom(site));
                break;
            case BridgeRules.PreviewZabuzaLine1:
                MistFigures.Say(ZabuzaBottom(site), "PreviewZabuza1");
                break;
            case BridgeRules.PreviewZabuzaLine2:
                MistFigures.Say(ZabuzaBottom(site), "PreviewZabuza2");
                break;
            case BridgeRules.PreviewHakuAppear:
                MistFigures.Puff(HakuBottom(site));
                break;
            case BridgeRules.PreviewHakuLine:
                MistFigures.Say(HakuBottom(site), "PreviewHaku");
                break;
            case BridgeRules.PreviewVanish:
                MistFigures.Puff(ZabuzaBottom(site));
                MistFigures.Puff(HakuBottom(site));
                Main.NewText(Language.GetTextValue("Mods.ShinobiPrototype.Dialogue.PreviewEnd"), 190, 200, 210);
                break;
        }
        if (Visibility(BridgeRules.PreviewZabuzaAppear) > 0f)
            MistFigures.Veil(ZabuzaBottom(site));
        if (Visibility(BridgeRules.PreviewHakuAppear) > 0f)
            MistFigures.Veil(HakuBottom(site));
        if (++tick >= BridgeRules.PreviewLength)
            tick = -1;
    }

    public static bool Playing => tick >= 0;

    // Both stand on the sea's real surface (user, 2026-10-01: they floated above it).
    private static Vector2 ZabuzaBottom(BridgeSite site) => OnWater(site.X(BridgeRules.PreviewZabuzaOffset) * 16f + 8f, site);

    private static Vector2 HakuBottom(BridgeSite site) => OnWater(site.X(BridgeRules.PreviewHakuOffset) * 16f + 8f, site);

    private static Vector2 OnWater(float x, BridgeSite site) => new(x, LiquidSurface.StandY(x, site.WaterY * 16f));

    private static float Visibility(int appear) =>
        BridgeRules.FigureVisibility(tick - appear, BridgeRules.PreviewVanish + 10 - appear, 25);

    // Called by the overlay before it lays the mist on top, so the pair shows through the fog.
    internal static void DrawFigures(SpriteBatch spriteBatch)
    {
        if (tick < 0 || WaveBridgeWorld.Site is not BridgeSite site)
            return;
        MistFigures.Draw(spriteBatch, MistFigure.Zabuza, ZabuzaBottom(site), -site.Dir,
            Visibility(BridgeRules.PreviewZabuzaAppear));
        MistFigures.Draw(spriteBatch, MistFigure.Haku, HakuBottom(site), -site.Dir,
            Visibility(BridgeRules.PreviewHakuAppear));
    }
}
