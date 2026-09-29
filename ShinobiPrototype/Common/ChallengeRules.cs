namespace ShinobiPrototype.Common;

public static class ChallengeRules
{
    public static bool CanUseStoryZabuzaScroll(bool anotherBossActive) =>
        !anotherBossActive;

    public static bool CanUseHakuScroll(bool anotherBossActive) =>
        !anotherBossActive;

    public static bool CanUseM0ZabuzaScroll(bool anotherBossActive) =>
        !anotherBossActive;
}
