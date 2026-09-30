using ShinobiPrototype.Common;
using static ShinobiPrototype.Common.StyleCoreRules;
using static ShinobiPrototype.Common.ThresholdRetreatRules;

static void Check(bool condition, string name)
{
    if (!condition)
        throw new Exception(name);
    Console.WriteLine($"PASS {name}");
}

Check(CanEquip(0, false) && !CanEquip(1, false), "Only one style core at a time before the second slot");
Check(CanEquip(1, true) && !CanEquip(2, true), "Two cores once the second slot is open (after Pain), never three");
Check(CanUseTechnique(StyleSchool.Sharingan, 0, 30, 30), "Technique ready with a core, no cooldown and enough chakra");
Check(!CanUseTechnique(StyleSchool.None, 0, 100, 30), "No technique without a core");
Check(!CanUseTechnique(StyleSchool.Byakugan, 5, 100, 30), "No technique during cooldown");
Check(!CanUseTechnique(StyleSchool.Sage, 0, 29, 30), "No technique without the chakra");
Check(Includes(StyleSchool.Sharingan, 3, StyleSchool.Sharingan, 1) && !Includes(StyleSchool.Sharingan, 1, StyleSchool.Sharingan, 2),
    "A higher tier includes the lower ones");
Check(!Includes(StyleSchool.EightGates, 3, StyleSchool.Sharingan, 1), "Tiers only count within a school");

Check(LockedLife(1000) == 500 && LockedLife(1001) == 501 && LockedLife(1, 0.5f) == 1, "Life locks at the threshold, never at 0");
Check(!Reached(501, 1000) && Reached(500, 1000), "The threshold is reached at exactly half");
Check(ClampDamage(600, 1000, 300) == 100 && ClampDamage(500, 1000, 50) == 0 && ClampDamage(900, 1000, 50) == 50,
    "A burst hit cannot carry the boss past the threshold");
Check(ExitInvulnerableTicks >= 120, "The exit plays out with the boss untouchable");

// The tier-one designs (specs/流派系统.spec.md section 3).
Check(SubstitutionWindowTicks(0) == 24 && SubstitutionWindowTicks(1) == 36 && SubstitutionWindowTicks(2) == 42 &&
      SubstitutionWindowTicks(3) == 48, "The Sharingan widens the substitution window: 0.4 / 0.6 / 0.7 / 0.8 s");
Check(ForesightTicks(1) == 240 && ForesightTicks(2) == 300, "Foresight lasts four seconds, five from two tomoe");
Check(NextGates(0) == 1 && NextGates(2) == 3 && NextGates(3) == 0, "Each press opens a gate; the press after the third closes");
Check(GatePressCost(0) == GateCost && GatePressCost(3) == 0, "Opening a gate costs chakra, closing is free");
Check(GateLifeLossPerSecond(1) == 2 && GateLifeLossPerSecond(3) == 6 && GateLifeLossPerSecond(0) == 0,
    "The gates burn 2 / 4 / 6 life a second");
Check(GatesForcedShut(2, 19, 100) && !GatesForcedShut(2, 20, 100) && !GatesForcedShut(0, 1, 100),
    "The gates shut themselves below a fifth of life");
Check(PressureScale(3) > PressureScale(1) && PressureScale(0) == 1f, "Taijutsu pressure grows with the gates");
Check(AddPoint(0) == 1 && AddPoint(3) == PointMaxStacks, "Gentle Fist points stack to three");
