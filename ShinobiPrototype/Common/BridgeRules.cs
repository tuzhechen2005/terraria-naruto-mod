using System;

namespace ShinobiPrototype.Common;

// Sea mist and mist-preview timing around the Wave Country bridge (M1 spec, stage 4); the bridge's shape lives in
// BridgeDesign. Kept free of Terraria types so the rule tests can run them.
public static class BridgeRules
{
    // Mist, relative to the phase-two boss mist (the overlay's 0.3 level).
    public const float PhaseTwoFog = 0.3f;
    // About 3.3 times phase two (user, 2026-09-29: still "雾太淡" at twice). The overlay draws 0.42 x this.
    public const float SeaFogOnBridge = 1.0f;
    public const float NightOrRainFogMultiplier = 1.4f;
    public const float MaxFogSetting = 2f;
    // Measured from the shoreline-to-island span: thick over the ramp and hut, gone just past the beach
    // (user, 2026-09-29: 300 tiles reached far too far inland).
    public const float FogFullWithinTiles = 20f;
    public const float FogReachTiles = 70f;
    public const float ScoutSpawnMultiplierInFog = 2f;

    // Mist preview timeline, in ticks from the moment a player nears the broken end (about 14 seconds).
    public const int PreviewLength = 840;
    public const int PreviewZabuzaAppear = 30;
    public const int PreviewZabuzaLine1 = 100;
    public const int PreviewZabuzaLine2 = 270;
    public const int PreviewHakuAppear = 420;
    public const int PreviewHakuLine = 480;
    public const int PreviewVanish = 760;
    // A little farther out than the scaffolding's end, but close enough to make out a shape.
    public const int PreviewZabuzaOffset = BridgeDesign.HalfBuiltPier + 16;
    public const int PreviewHakuOffset = BridgeDesign.HalfBuiltPier + 20;
    public const float PreviewFog = 1.2f;

    // The preview starts when a player walks out near the broken end (on the deck or the scaffold walkway).
    public const int PreviewTriggerFrom = BridgeDesign.UnfinishedEnd - 14;
    public const int PreviewTriggerTo = BridgeDesign.HalfBuiltPier + 4;

    public static bool NearBrokenEnd(int offset, int rowsAboveDeck) =>
        offset >= PreviewTriggerFrom && offset <= PreviewTriggerTo && rowsAboveDeck >= 0 && rowsAboveDeck <= 5;

    // Figures in the mist: a dark silhouette whose strength drifts between these, with only a trace of detail.
    public const float FigureMinVisibility = 0.2f;
    public const float FigureMaxVisibility = 0.6f;
    public const float FigureDetail = 0.13f;
    public const int FigurePulseTicks = 150;

    // Sightings after the preview: at night or in rain, near the break, at most once per night (or rain).
    public const int SightingLength = 240;
    public const int SightingWhisperTick = 60;
    public const int SightingChanceOneIn = 600;       // per tick while eligible: about ten seconds on average
    public const float SightingRangeTiles = 40f;
    public const int SightingNearOffset = BridgeDesign.HalfBuiltPier + 10;
    public const int SightingFarOffset = BridgeDesign.HalfBuiltPier + 22;

    // The senbon warning once a player carries three Mist insignia.
    public const int SenbonWarningInsignia = 3;
    public const int SenbonStuckTicks = 180;
    public const float SenbonSpeed = 14f;
    public const int SenbonThrowerOffset = BridgeDesign.HalfBuiltPier + 14;

    // How visible a mist figure is `tick` ticks into an appearance lasting `length`, fading in and out over `fade`.
    public static float FigureVisibility(int tick, int length, int fade)
    {
        if (tick < 0 || tick >= length)
            return 0f;
        float envelope = Math.Min(1f, Math.Min(tick, length - tick) / (float)Math.Max(1, fade));
        float pulse = 0.5f + 0.5f * (float)Math.Sin(tick * 2 * Math.PI / FigurePulseTicks);
        return envelope * (FigureMinVisibility + (FigureMaxVisibility - FigureMinVisibility) * pulse);
    }

    public static bool SightingEligible(bool mistActive, bool previewDone, bool nightOrRain, bool usedThisSpell,
        float distanceToBreakTiles) =>
        mistActive && previewDone && nightOrRain && !usedThisSpell && distanceToBreakTiles <= SightingRangeTiles;

    public static bool SenbonWarningDue(bool alreadyWarned, bool previewDone, bool mistActive, int insignia,
        bool nearBreak) =>
        !alreadyWarned && previewDone && mistActive && insignia >= SenbonWarningInsignia && nearBreak;

    // How thick the sea mist is for a player this far (in tiles) from the bridge, before the overlay's own scale.
    public static float SeaFog(float distanceTiles, bool nightOrRain, float setting)
    {
        if (distanceTiles >= FogReachTiles || setting <= 0f)
            return 0f;
        float closeness = distanceTiles <= FogFullWithinTiles
            ? 1f
            : 1f - (distanceTiles - FogFullWithinTiles) / (FogReachTiles - FogFullWithinTiles);
        return SeaFogOnBridge * closeness * (nightOrRain ? NightOrRainFogMultiplier : 1f) * Math.Clamp(setting, 0f, MaxFogSetting);
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
