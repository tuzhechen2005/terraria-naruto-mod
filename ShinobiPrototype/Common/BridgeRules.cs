using System;

namespace ShinobiPrototype.Common;

// Wave Country bridge layout, sea mist and mist-preview timing (M1 spec, stage 4). Kept free of Terraria types
// so the rule tests can run them. Layout offsets are in tiles along the bridge, measured from the shoreline
// column (the last land column before the sea) and multiplied by the bridge direction (+1 = sea to the right).
public static class BridgeRules
{
    public const int DeckLandOverlap = 3;
    public const int DeckLength = 70;
    public const int JaggedTiles = 4;
    public const int GapLength = 20;
    public const int IslandWidth = 25;
    public const int IslandFlatHalfWidth = 8;
    public const int DeckAboveWater = 5;
    public const int Clearance = 25;
    public const int PillarSpacing = 10;
    public const int MaxStairSteps = 16;
    public const int HutWidth = 11;
    public const int HutHeight = 7;
    public const int HutGapFromStairs = 2;
    public const int SeaMargin = 20;
    public const int IslandDockLength = 4;

    public const int DeckStart = -DeckLandOverlap;
    public const int IslandStart = DeckLength + GapLength + 1;
    public const int IslandEnd = IslandStart + IslandWidth - 1;
    public const int IslandCenter = IslandStart + IslandWidth / 2;
    public const int TotalReach = IslandEnd + IslandDockLength + SeaMargin;

    // Tiles filled when the bridge is finished: the jagged end plus the whole gap.
    public const int FinishFrom = DeckLength - JaggedTiles + 1;
    public const int FinishTo = IslandStart - 1;

    // Mist, relative to the phase-two boss mist (the overlay's 0.3 level).
    public const float PhaseTwoFog = 0.3f;
    public const float NightOrRainFogMultiplier = 1.5f;
    public const float FogFullWithinTiles = 10f;
    public const float FogReachTiles = 150f;
    public const float ScoutSpawnMultiplierInFog = 2f;

    // Mist preview timeline, in ticks from the moment a player first steps onto the deck.
    public const int PreviewLength = 480;
    public const int PreviewZabuzaAppear = 30;
    public const int PreviewZabuzaLine1 = 70;
    public const int PreviewZabuzaLine2 = 170;
    public const int PreviewHakuAppear = 250;
    public const int PreviewHakuLine = 280;
    public const int PreviewVanish = 400;
    public const int PreviewZabuzaOffset = DeckLength + 6;
    public const int PreviewHakuOffset = DeckLength + 10;
    public const float PreviewFog = 0.8f;

    public static int DeckY(int waterY) => waterY - DeckAboveWater;

    // The unfinished deck leaves holes in its jagged end; the finished one is continuous up to the island.
    public static bool HasDeckTile(int offset, bool finished)
    {
        if (offset < DeckStart)
            return false;
        if (finished)
            return offset <= FinishTo;
        if (offset > DeckLength)
            return false;
        int intoJagged = offset - (DeckLength - JaggedTiles);
        return intoJagged <= 0 || intoJagged % 2 == 1;
    }

    public static bool IsPillar(int offset, bool finished) =>
        offset > 0 && offset % PillarSpacing == 0 && (offset <= DeckLength || finished && offset <= FinishTo);

    // Island surface as rows below the deck: flat in the middle, sloping down to the water at both ends.
    public static int IslandSurfaceBelowDeck(int offset)
    {
        int fromCenter = Math.Abs(offset - IslandCenter);
        return 1 + Math.Max(0, fromCenter - IslandFlatHalfWidth);
    }

    public static bool OnDeck(int offset, int rowsAboveDeck) =>
        offset >= 0 && offset <= DeckLength && rowsAboveDeck >= 0 && rowsAboveDeck <= 4;

    // How thick the sea mist is for a player this far (in tiles) from the bridge, before the overlay's own scale.
    public static float SeaFog(float distanceTiles, bool nightOrRain, float setting)
    {
        if (distanceTiles >= FogReachTiles || setting <= 0f)
            return 0f;
        float closeness = distanceTiles <= FogFullWithinTiles
            ? 1f
            : 1f - (distanceTiles - FogFullWithinTiles) / (FogReachTiles - FogFullWithinTiles);
        return PhaseTwoFog * closeness * (nightOrRain ? NightOrRainFogMultiplier : 1f) * Math.Clamp(setting, 0f, 1f);
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
