namespace ShinobiPrototype.Common;

public static class ExamRules
{
    public const int NotRegistered = 0;
    public const int ForestTrial = 1;
    public const int ArenaReady = 2;

    public static bool WaveComplete(bool downedHaku, bool downedZabuza) =>
        downedHaku && downedZabuza;

    public static bool CanRegister(bool waveComplete, int examStage) =>
        waveComplete && examStage == NotRegistered;

    public static bool HasBothScrolls(int heavenCount, int earthCount) =>
        heavenCount > 0 && earthCount > 0;

    public static bool CanSpawnTrialEnemy(int examStage, bool examCleared,
        bool inJungle, bool correctDepth) =>
        examStage is ForestTrial or ArenaReady && !examCleared && inJungle && correctDepth;

    public static bool CanChallenge(int examStage, bool waveComplete) =>
        waveComplete && examStage >= ArenaReady;
}
