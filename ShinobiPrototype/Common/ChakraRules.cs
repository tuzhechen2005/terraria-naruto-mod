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

    public enum Activation { Ready, CoolingDown, NotEnoughChakra }

    public static int MaxChakra(int crystals) =>
        BaseMaxChakra + CrystalBonus * Math.Clamp(crystals, 0, MaxCrystals);

    public static bool CanUseCrystal(int crystals) => crystals < MaxCrystals;

    public static float RegenPerTick(int recoveryDelay) =>
        (recoveryDelay > 0 ? CombatRegenPerSecond : SafeRegenPerSecond) / 60f;

    public static int HitRegen(int restoredThisWindow) =>
        Math.Clamp(HitRegenPerSecondCap - restoredThisWindow, 0, HitRegenPerHit);

    public static Activation CheckSubstitution(float chakra, int cooldown) =>
        cooldown > 0 ? Activation.CoolingDown :
        chakra < SubstitutionCost ? Activation.NotEnoughChakra :
        Activation.Ready;

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
