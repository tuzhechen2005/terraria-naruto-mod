using ShinobiPrototype.Common;

static void Check(bool actual, bool expected, string name)
{
    if (actual != expected)
        throw new Exception($"{name}: expected {expected}, got {actual}");
    Console.WriteLine($"PASS {name}");
}

Check(ChallengeRules.CanUseStoryZabuzaScroll(false), true, "Duo scroll has no prior Haku gate");
Check(ChallengeRules.CanUseStoryZabuzaScroll(true), false, "Story scroll cannot double-spawn");
Check(ChallengeRules.CanUseHakuScroll(false), true, "Legacy Haku scroll can start the duo battle");
Check(ChallengeRules.CanUseHakuScroll(true), false, "Legacy Haku scroll cannot double-spawn");
Check(ChallengeRules.CanUseM0ZabuzaScroll(false), true, "M0 scroll bypasses story gate");
Check(ChallengeRules.CanUseM0ZabuzaScroll(true), false, "M0 scroll cannot double-spawn");
