using ShinobiPrototype.Common;

static void Check(bool condition, string name)
{
    if (!condition)
        throw new Exception(name);
    Console.WriteLine($"PASS {name}");
}

Check(WaveDuoRules.HakuMaxLife is >= 500 and <= 600,
    "Haku life matches the agreed early-game duration");
Check(!WaveDuoRules.HakuMayAttack(ZabuzaCombatRules.MistTransition) &&
    WaveDuoRules.HakuMayAttack(ZabuzaCombatRules.Approach),
    "Haku enters during the transition but waits until it ends");
Check(!WaveDuoRules.HakuMayUseStrongAttack(ZabuzaCombatRules.DashActive) &&
    !WaveDuoRules.HakuMayUseStrongAttack(ZabuzaCombatRules.RainWindup) &&
    !WaveDuoRules.HakuMayUseStrongAttack(ZabuzaCombatRules.SpiralWindup) &&
    WaveDuoRules.HakuMayUseStrongAttack(ZabuzaCombatRules.WaterRecovery),
    "Haku strong actions do not overlap Zabuza's dense attacks");
Check(WaveDuoRules.HakuNeedleCount(false) == 1 &&
    WaveDuoRules.HakuNeedleCount(true) <= 2 &&
    WaveDuoRules.HakuNeedleWindupTicks >= 30,
    "Haku's empowered state avoids a projectile flood");
Check(WaveDuoRules.HakuDashSpeed(true) > WaveDuoRules.HakuDashSpeed(false) &&
    WaveDuoRules.HakuDashWindupTicks(true) >= 25 &&
    WaveDuoRules.ZabuzaLastStandDashMultiplier is > 1f and <= 1.2f,
    "Survivor pressure comes from movement with readable warning");
Check(!WaveDuoRules.EncounterComplete(false, false) &&
    !WaveDuoRules.EncounterComplete(true, false) &&
    !WaveDuoRules.EncounterComplete(false, true) &&
    WaveDuoRules.EncounterComplete(true, true),
    "Both partners must die in the same encounter");
Check(WaveDuoRules.NormalizeLegacyWaveFlags(true, true) == 3 &&
    WaveDuoRules.NormalizeLegacyWaveFlags(true, false) == 0 &&
    WaveDuoRules.NormalizeLegacyWaveFlags(false, true) == 0,
    "Old complete saves survive while partial clears reset");
Check(WaveDuoRules.PrismMayStart(true, false, ZabuzaCombatRules.Approach, 1000) == false &&
    WaveDuoRules.PrismMayStart(false, true, ZabuzaCombatRules.Approach, 1000) == false &&
    WaveDuoRules.PrismMayStart(true, true, ZabuzaCombatRules.RainWindup, 1000) == false &&
    WaveDuoRules.PrismMayStart(true, true, ZabuzaCombatRules.Approach,
        WaveDuoRules.PrismCooldownTicks - 1) == false &&
    WaveDuoRules.PrismMayStart(true, true, ZabuzaCombatRules.Approach,
        WaveDuoRules.PrismCooldownTicks),
    "Mirror domain starts only with both partners alive and a quiet Zabuza beat");
Check(WaveDuoRules.PrismVolleyCount == 4 &&
    WaveDuoRules.PrismFirstVolleyTick >= 18 &&
    WaveDuoRules.PrismVolleySpacingTicks >= WaveDuoRules.PrismWarningTicks + 10 &&
    WaveDuoRules.PrismHiddenTicks >= WaveDuoRules.PrismVolleyTick(3) +
        WaveDuoRules.PrismWarningTicks,
    "Four directions are spaced and visibly warned before impact");
Check(WaveDuoRules.IsPrismVolley(WaveDuoRules.PrismVolleyTick(0)) &&
    WaveDuoRules.IsPrismVolley(WaveDuoRules.PrismVolleyTick(3)) &&
    !WaveDuoRules.IsPrismVolley(WaveDuoRules.PrismVolleyTick(3) + 1) &&
    !WaveDuoRules.IsPrismVolley(WaveDuoRules.PrismHiddenTicks - 1),
    "Mirror domain fires exactly four deliberate volleys");
Check(WaveDuoRules.SoftenedDamage(48) is >= 42 and <= 44 &&
    WaveDuoRules.SoftenedDamage(20) is >= 17 and <= 19,
    "Boss damage is reduced gently rather than gutted");
