using System;

namespace ShinobiPrototype.Common;

// Sea mist and mist-preview timing around the Wave Country bridge (M1 spec, stage 4); the bridge's shape lives in
// BridgeDesign. Kept free of Terraria types so the rule tests can run them.
public static class BridgeRules
{
    // Mist, relative to the phase-two boss mist (the overlay's 0.3 level).
    public const float PhaseTwoFog = 0.3f;
    public const float SeaFogOnBridge = 0.6f;   // twice phase two (2026-09-29: "雾太淡")
    public const float NightOrRainFogMultiplier = 1.5f;
    public const float FogFullWithinTiles = 40f;
    public const float FogReachTiles = 300f;
    public const float ScoutSpawnMultiplierInFog = 2f;

    // Mist preview timeline, in ticks from the moment a player nears the broken end (about 14 seconds).
    public const int PreviewLength = 840;
    public const int PreviewZabuzaAppear = 30;
    public const int PreviewZabuzaLine1 = 100;
    public const int PreviewZabuzaLine2 = 270;
    public const int PreviewHakuAppear = 420;
    public const int PreviewHakuLine = 480;
    public const int PreviewVanish = 760;
    public const int PreviewZabuzaOffset = BridgeDesign.HalfBuiltPier + 10;
    public const int PreviewHakuOffset = BridgeDesign.HalfBuiltPier + 14;
    public const float PreviewFog = 1.2f;

    // The preview starts when a player walks out near the broken end (on the deck or the scaffold walkway).
    public const int PreviewTriggerFrom = BridgeDesign.UnfinishedEnd - 14;
    public const int PreviewTriggerTo = BridgeDesign.HalfBuiltPier + 4;

    public static bool NearBrokenEnd(int offset, int rowsAboveDeck) =>
        offset >= PreviewTriggerFrom && offset <= PreviewTriggerTo && rowsAboveDeck >= 0 && rowsAboveDeck <= 5;

    // How thick the sea mist is for a player this far (in tiles) from the bridge, before the overlay's own scale.
    public static float SeaFog(float distanceTiles, bool nightOrRain, float setting)
    {
        if (distanceTiles >= FogReachTiles || setting <= 0f)
            return 0f;
        float closeness = distanceTiles <= FogFullWithinTiles
            ? 1f
            : 1f - (distanceTiles - FogFullWithinTiles) / (FogReachTiles - FogFullWithinTiles);
        return SeaFogOnBridge * closeness * (nightOrRain ? NightOrRainFogMultiplier : 1f) * Math.Clamp(setting, 0f, 1f);
    }

    // Extra fog during the preview: rises quickly, holds, and fades after the pair vanishes.
    public static float PreviewFogBoost(int tick)
    {
        if (tick < 0 || tick >= PreviewLength)
            return 0f;
        if (tick < PreviewZabuzaAppear)
            return PreviewFog * tick / PreviewZabuzaAppear;
        if (tick < PreviewVanish)
            return PreviewFog;
        return PreviewFog * (PreviewLength - tick) / (PreviewLength - PreviewVanish);
    }
}
