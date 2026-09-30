namespace ShinobiPrototype.Common;

public static class ChallengeRules
{
    // The story scroll only works around the bridge (user, 2026-09-30): Zabuza waits at the broken bridge.
    public const float StoryScrollRangeTiles = BridgeRules.FogReachTiles;

    public static bool CanUseStoryZabuzaScroll(bool anotherBossActive, float tilesFromBridge) =>
        !anotherBossActive && tilesFromBridge <= StoryScrollRangeTiles;

    public static bool CanUseHakuScroll(bool anotherBossActive) =>
        !anotherBossActive;

    public static bool CanUseM0ZabuzaScroll(bool anotherBossActive) =>
        !anotherBossActive;
}
