using System;

namespace ShinobiPrototype.Common;

// The four schools of the master spec (流派): a core accessory carries a school at a tier (specs/流派系统.spec.md).
// Cores are free to wear and swap; one at a time until the second core slot opens (after Pain's assault on the
// Leaf). Kept free of Terraria types so the rule tests can run it.
public enum StyleSchool : byte { None, Sharingan, EightGates, Byakugan, Sage }

public static class StyleCoreRules
{
    // A core joins the accessories only while fewer than the allowed number of other cores are worn.
    public static int MaxCores(bool secondSlot) => secondSlot ? 2 : 1;

    public static bool CanEquip(int otherCoresWorn, bool secondSlot) => otherCoresWorn < MaxCores(secondSlot);

    // The technique fires only with a core worn, off cooldown and with the chakra for it.
    public static bool CanUseTechnique(StyleSchool worn, int cooldownTicks, int chakra, int cost) =>
        worn != StyleSchool.None && cooldownTicks <= 0 && chakra >= cost;

    // A higher tier of the same school includes everything below it.
    public static bool Includes(StyleSchool school, int tier, StyleSchool askedSchool, int askedTier) =>
        school == askedSchool && tier >= askedTier;

    // --- Sharingan: one tomoe dodges, two see, three counter.
    public static int SubstitutionWindowTicks(int sharinganTier) => sharinganTier switch
    {
        <= 0 => ChakraRules.SubstitutionWindowTicks,   // 0.4 s
        1 => 36,                                       // 0.6 s
        2 => 42,                                       // 0.7 s
        _ => 48,                                       // 0.8 s
    };

    public const int ForesightCost = 30;
    public const int ForesightCooldownTicks = 20 * 60;
    public static int ForesightTicks(int sharinganTier) => sharinganTier >= 2 ? 5 * 60 : 4 * 60;
    public const float ForesightDamageBonus = 0.15f;
    public const int ForesightBonusTicks = 3 * 60;
    public const int SeenTargetTicks = 5 * 60;

    // --- Eight Gates: each press opens one more gate, the press after the third closes them.
    public const int MaxGates = 3;
    public const int GateCost = 20;
    public const float DamagePerGate = 0.1f;
    public const float SpeedPerGate = 0.1f;
    public const float KnockbackPerGate = 0.15f;
    public const float CloseBelowLife = 0.2f;
    public const int FatigueTicks = 10 * 60;
    public const float FatigueDamage = -0.1f;
    public const float TaijutsuAttackSpeed = 0.1f;

    public static int NextGates(int gates) => gates >= MaxGates ? 0 : gates + 1;

    // Opening costs chakra; closing is free.
    public static int GatePressCost(int gates) => gates >= MaxGates ? 0 : GateCost;

    // Life lost each second with this many gates open: 2, 4, 6.
    public static int GateLifeLossPerSecond(int gates) => 2 * Math.Clamp(gates, 0, MaxGates);

    public static bool GatesForcedShut(int gates, int life, int lifeMax) =>
        gates > 0 && life < lifeMax * CloseBelowLife;

    // Taijutsu air pressure grows with the gates: bigger and further.
    public static float PressureScale(int gates) => 1f + 0.25f * Math.Clamp(gates, 0, MaxGates);

    // --- Byakugan / Gentle Fist: melee hits seal points (defence down; bosses take more).
    public const int PointDefensePerStack = 4;
    public const int PointMaxStacks = 3;
    public const int PointTicks = 5 * 60;
    public const float PointBossDamagePerStack = 0.05f;
    public const int RotationCost = 40;
    public const int RotationCooldownTicks = 15 * 60;
    public const int RotationTicks = 60;
    public const float RotationRadiusTiles = 5f;
    public const int RotationDamage = 20;

    public static int AddPoint(int stacks) => Math.Min(PointMaxStacks, stacks + 1);

    // --- Sage (the Rasengan), acquired in M3; numbers kept here for when it arrives.
    public const int RasenganCost = 40;
    public const int RasenganCooldownTicks = 8 * 60;
}
