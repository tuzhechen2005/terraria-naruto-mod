using System;

namespace ShinobiPrototype.Common;

// Chakra and Substitution Jutsu numbers (M1 spec). Kept free of Terraria types so the rule tests can run them.
public static class ChakraRules
{
    public const int BaseMaxChakra = 100;
    public const int CrystalBonus = 20;
    public const int PreHardmodeMaxChakra = 200;
    public const int MaxCrystals = (PreHardmodeMaxChakra - BaseMaxChakra) / CrystalBonus;

    public const float SafeRegenPerSecond = 10f;
    public const float CombatRegenPerSecond = 3f;
    public const int RegenDelayTicks = 300;

    // Any hit (weapon, projectile or minion) restores a little; all hits share one per-second budget.
    public const int HitRegenPerHit = 2;
    public const int HitRegenPerSecondCap = 5;
    public const int HitRegenWindowTicks = 60;

    public const int PillRestore = 40;
    public const int PillSicknessTicks = 600;

    public const int SubstitutionCost = 20;
    public const int SubstitutionWindowTicks = 24;
    public const int SubstitutionCooldownTicks = 240;
    public const int SubstitutionImmuneTicks = 60;
    public const int SubstitutionMaxHints = 3;
    public const int SubstitutionHintSpacingTicks = 1800;

    // Kakashi's drill: he throws kunai at the player one at a time, after an unpredictable aim. During the drill the
    // jutsu is free and recovers quickly, and he never throws while it is cooling down, so only timing is practised.
    public const int PracticeThrows = 8;
    public const int PracticeCooldownTicks = 48;
    public const int PracticeGapMinTicks = 60;
    public const int PracticeGapMaxTicks = 150;
    public const int PracticeWindupMinTicks = 18;
    public const int PracticeWindupMaxTicks = 60;
    public const int PracticeMinRangeTiles = 10;
    public const int PracticeMaxRangeTiles = 25;
    public const float PracticeKunaiSpeed = 6.5f;
    public const int PracticeEarlyWindowTicks = 90;
    public const int PracticeLeashTiles = 50;

    public enum Activation { Ready, CoolingDown, NotEnoughChakra }

    public enum PracticeOutcome { Substituted, TooEarly, TooLate, Evaded }

    public enum PracticeReadiness { Ready, TooClose, TooFar, CoolingDown }

    public static int MaxChakra(int crystals) =>
        BaseMaxChakra + CrystalBonus * Math.Clamp(crystals, 0, MaxCrystals);

    public static bool CanUseCrystal(int crystals) => crystals < MaxCrystals;

    public static float RegenPerTick(int recoveryDelay) =>
        (recoveryDelay > 0 ? CombatRegenPerSecond : SafeRegenPerSecond) / 60f;

    public static int HitRegen(int restoredThisWindow) =>
        Math.Clamp(HitRegenPerSecondCap - restoredThisWindow, 0, HitRegenPerHit);

    public static Activation CheckSubstitution(float chakra, int cooldown, int cost = SubstitutionCost) =>
        cooldown > 0 ? Activation.CoolingDown :
        chakra < cost ? Activation.NotEnoughChakra :
        Activation.Ready;

    public static int SubstitutionCostFor(bool practising) => practising ? 0 : SubstitutionCost;

    public static int SubstitutionCooldownFor(bool practising) =>
        practising ? PracticeCooldownTicks : SubstitutionCooldownTicks;

    // Whether Kakashi may start the next throw: the player must be in range and the jutsu off cooldown.
    public static PracticeReadiness CheckPracticeThrow(float distanceTiles, int cooldown) =>
        distanceTiles < PracticeMinRangeTiles ? PracticeReadiness.TooClose :
        distanceTiles > PracticeMaxRangeTiles ? PracticeReadiness.TooFar :
        cooldown > 0 ? PracticeReadiness.CoolingDown :
        PracticeReadiness.Ready;

    // A kunai already aimed is only released once the jutsu is ready, so an early press is never punished twice.
    public static bool MayReleasePracticeKunai(int aimedTicks, int windupTicks, int cooldown) =>
        aimedTicks >= windupTicks && cooldown == 0;

    // A drill kunai reached the player: a standby dodges it; a press shortly before means the timing was early.
    public static PracticeOutcome JudgePracticeHit(bool standingBy, int ticksSinceActivation) =>
        standingBy ? PracticeOutcome.Substituted :
        ticksSinceActivation <= PracticeEarlyWindowTicks ? PracticeOutcome.TooEarly :
        PracticeOutcome.TooLate;

    public static string PracticeVerdict(int substituted, int total) =>
        substituted * 8 >= total * 7 ? "已经很熟练了。实战里也要这么冷静。" :
        substituted * 2 >= total ? "还行。再练几次，身体自己就会记住时机。" :
        "看苦无，不要看我。再来一次吧。";

    public static bool ShouldShowHint(bool mastered, int hintsShown, int ticksSinceLastHint, float chakra, int cooldown) =>
        !mastered && hintsShown < SubstitutionMaxHints && ticksSinceLastHint >= SubstitutionHintSpacingTicks &&
        CheckSubstitution(chakra, cooldown) == Activation.Ready;

    // Landing offsets in tiles, best first: away from the attacker, then toward it; each distance tries
    // level ground, then stepping up, then down. The caller rejects spots that are solid, lava or out of sight.
    public static (int X, int Y)[] LandingOffsets(int awayDirection)
    {
        int away = awayDirection >= 0 ? 1 : -1;
        int[] distances = { 8, 6, 10, 4 };
        int[] rises = { 0, -1, -2, -3, 1, 2 };
        var offsets = new (int X, int Y)[2 * distances.Length * rises.Length];
        int i = 0;
        foreach (int side in new[] { away, -away })
            foreach (int distance in distances)
                foreach (int rise in rises)
                    offsets[i++] = (side * distance, rise);
        return offsets;
    }
}
