using System;

namespace ShinobiPrototype.Common;

// Where a character stands in the Chūnin Exams (specs/M2_中忍考试篇.spec.md). The exam is a personal qualification,
// so its steps are kept per character; boss kills are the world's, as in vanilla.
public enum ExamStage : byte
{
    Locked,       // Wave Country not finished
    NoVillage,    // a world without the Hidden Leaf: the exams need a new world
    Recommend,    // talk to Kakashi for the recommendation
    Written,      // the written test at the Academy
    ForestGate,   // walk to the Area 44 gate for a scroll
    ForestHunt,   // take the other scroll, then the tower
    Prelims,      // the preliminary bout in the tower
    Training,     // the month before the finals: grow strong enough
    Finals,       // the finals at the stadium
    Done,         // Gaara has fallen: the Konoha Crush (M3)
}

public enum ExamScroll : byte { None, Heaven, Earth }

// Everything a character has done in the exams, as saved.
public readonly record struct ExamProgress(bool Recommended, bool WrittenPassed, ExamScroll Issued, bool TowerReached,
    bool PrelimsPassed);

// Kept free of Terraria types so the rule tests can run it.
public static class ChuninExamRules
{
    // Gates (the master spec: vanilla bosses are never the only way through).
    public const int OrochimaruLifeThreshold = 300;
    public const int FinalsLifeThreshold = 400;

    public static bool ReadyForOrochimaru(bool downedWorldEvil, int lifeMax) =>
        downedWorldEvil || lifeMax >= OrochimaruLifeThreshold;

    public static bool ReadyForFinals(bool downedSkeletron, int lifeMax) =>
        downedSkeletron || lifeMax >= FinalsLifeThreshold;

    public static ExamStage Stage(bool waveComplete, bool hasVillage, ExamProgress progress, bool readyForFinals,
        bool downedGaara)
    {
        if (downedGaara)
            return ExamStage.Done;
        if (!waveComplete)
            return ExamStage.Locked;
        if (!hasVillage)
            return ExamStage.NoVillage;
        if (!progress.Recommended)
            return ExamStage.Recommend;
        if (!progress.WrittenPassed)
            return ExamStage.Written;
        if (!progress.TowerReached)
            return progress.Issued == ExamScroll.None ? ExamStage.ForestGate : ExamStage.ForestHunt;
        if (!progress.PrelimsPassed)
            return ExamStage.Prelims;
        return readyForFinals ? ExamStage.Finals : ExamStage.Training;
    }

    // The written test: nine questions from the bank, then Ibiki's tenth.
    public const int WrittenQuestions = 9;
    // Paid only on passing, per question answered right: 10 silver (copper coins are counted in copper).
    public const int CopperPerCorrect = 10 * 100;

    public static int WrittenReward(int correct) => Math.Clamp(correct, 0, WrittenQuestions) * CopperPerCorrect;

    // Picks `count` distinct questions from a bank of `bankSize`, driven by `next(n)` (a random number in [0, n)).
    public static int[] Draw(int bankSize, int count, Func<int, int> next)
    {
        int[] order = new int[bankSize];
        for (int i = 0; i < bankSize; i++)
            order[i] = i;
        int take = Math.Min(count, bankSize);
        for (int i = 0; i < take; i++)
        {
            int j = i + next(bankSize - i);
            (order[i], order[j]) = (order[j], order[i]);
        }
        return order[..take];
    }

    // Giving up at the tenth question ends this sitting; the test can be taken again once a new day has dawned.
    public static bool CanSitWritten(bool gaveUp, bool dawnSinceGivingUp) => !gaveUp || dawnSinceGivingUp;

    // The Forest of Death: the scroll handed out at the gate, and the one the character must take.
    public static ExamScroll Issue(int roll) => roll % 2 == 0 ? ExamScroll.Heaven : ExamScroll.Earth;

    public static ExamScroll Other(ExamScroll issued) => issued switch
    {
        ExamScroll.Heaven => ExamScroll.Earth,
        ExamScroll.Earth => ExamScroll.Heaven,
        _ => ExamScroll.None,
    };

    public static bool HasBoth(int heaven, int earth) => heaven > 0 && earth > 0;

    // Candidate squads: three to a squad, coming together, each squad beaten carries the other scroll with this chance,
    // and the fifth always does (user, 2026-10-01: they came one by one and "a squad" was just every third kill).
    public const int SquadSize = 3;
    public const float SquadScrollChance = 0.25f;
    public const int SquadPity = 5;

    public static bool SquadDropsScroll(int squadsBeaten, float roll) =>
        squadsBeaten >= SquadPity || roll < SquadScrollChance;

    // The second test's fixed encounters (user, 2026-10-01: random squads and a hidden trigger felt muddled). A squad
    // lies in wait past the gate (carrying the same scroll), and the Rain genin in the clearing before the tower carry
    // the missing one; both wait at their spot for whoever comes by, and try again if the player runs or falls.
    public const int EncounterReachTiles = 16;
    public const int GateAmbushRetryTicks = 20 * 60;
    public const int RainAmbushRetryTicks = 20 * 60;
    public const int RainAmbushWarnTicks = 120;

    public static bool GateAmbushDue(ExamStage stage, bool done, float tilesFromSpot, bool squadAlive) =>
        stage == ExamStage.ForestHunt && !done && tilesFromSpot <= EncounterReachTiles && !squadAlive;

    public static bool RainAmbushDue(ExamStage stage, bool done, float tilesFromClearing, bool rainAlive, bool hasBoth) =>
        stage == ExamStage.ForestHunt && !done && tilesFromClearing <= EncounterReachTiles && !rainAlive && !hasBoth;

    // A squad comes for someone hunting on the jungle surface who still lacks a scroll, one squad at a time: the first
    // after a short while in the forest, the next a while after the last one was beaten or lost.
    public const int SquadFirstTicks = 15 * 60;
    public const int SquadEveryTicks = 35 * 60;
    public const int SquadSpawnTiles = 48;  // just off screen
    public const int SquadNearTiles = 120;  // a squad this close is still about

    public static bool SquadDue(ExamStage stage, bool onJungleSurface, bool hasBoth, bool squadNear, int ticksWaited,
        int squadsBeaten) =>
        stage == ExamStage.ForestHunt && onJungleSurface && !hasBoth && !squadNear &&
        ticksWaited >= (squadsBeaten == 0 ? SquadFirstTicks : SquadEveryTicks);

    // The forest's fixed encounters (user, 2026-10-01): a squad lies in wait this far past the gate, and the Rain genin
    // in a clearing this far out from the tower's wall on the gate side.
    public const int GateAmbushTiles = 40;
    public const int RainClearingTiles = 40;

    // How close to the Area 44 gate the scroll is handed out, in tiles from its centre.
    public const int GateReachTiles = 14;
}
