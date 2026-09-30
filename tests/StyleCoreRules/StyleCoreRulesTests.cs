using ShinobiPrototype.Common;
using static ShinobiPrototype.Common.StyleCoreRules;
using static ShinobiPrototype.Common.ThresholdRetreatRules;

static void Check(bool condition, string name)
{
    if (!condition)
        throw new Exception(name);
    Console.WriteLine($"PASS {name}");
}

Check(CanEquip(false) && !CanEquip(true), "Only one style core at a time");
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
