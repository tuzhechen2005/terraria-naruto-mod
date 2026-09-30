using ShinobiPrototype.Common;

static void Check(bool condition, string name)
{
    if (!condition)
        throw new Exception(name);
    Console.WriteLine($"PASS {name}");
}

Check(WaveDuoRules.HakuMaxLife is >= 850 and <= 950,
    "Haku life raised after play (2026-09-30), about half of Zabuza's");
Check(WaveDuoRules.HakuBodyHeight is >= 64 and <= 72 &&
    WaveDuoRules.HakuBodyHeight < ZabuzaCombatRules.BodyHeight,
    "Haku is about 1.6x the player's height and clearly smaller than Zabuza");
Check(!WaveDuoRules.HakuMayAttack(ZabuzaCombatRules.MistTransition) &&
    WaveDuoRules.HakuMayAttack(ZabuzaCombatRules.Approach),
    "Haku enters during the transition but waits until it ends");
Check(!WaveDuoRules.HakuMayUseStrongAttack(ZabuzaCombatRules.DashActive) &&
    !WaveDuoRules.HakuMayUseStrongAttack(ZabuzaCombatRules.RainWindup) &&
    !WaveDuoRules.HakuMayUseStrongAttack(ZabuzaCombatRules.SpiralWindup) &&
    WaveDuoRules.HakuMayUseStrongAttack(ZabuzaCombatRules.WaterRecovery),
    "Haku strong actions do not overlap Zabuza's dense attacks");
Check(WaveDuoRules.HakuEmergeLineTick < WaveDuoRules.HakuEmergeTicks &&
    ZabuzaCombatRules.TransitionHakuTick + WaveDuoRules.HakuEmergeTicks <= ZabuzaCombatRules.TransitionBurstTick,
    "Haku finishes stepping out of the mirror before Zabuza roars");

var spawn = WaveDuoRules.HakuSpawnCandidates(1);
Check(spawn.Length >= 6 && spawn.Any(c => c.RelativeToPlayer) &&
    spawn[^1] == (0f, 0f, false),
    "Haku's spawn tries several spots near both sides and finally Zabuza himself");

Check(!WaveDuoRules.EncounterComplete(true, false) &&
    !WaveDuoRules.EncounterComplete(false, true) &&
    WaveDuoRules.EncounterComplete(true, true),
    "Both partners must die in the same encounter");
Check(WaveDuoRules.NormalizeLegacyWaveFlags(true, true) == 3 &&
    WaveDuoRules.NormalizeLegacyWaveFlags(true, false) == 0,
    "Old complete saves survive while partial clears reset");

// Mirror cage.
Check(!WaveDuoRules.CageMayStart(true, false, ZabuzaCombatRules.Approach, 5000) &&
    !WaveDuoRules.CageMayStart(true, true, ZabuzaCombatRules.RainWindup, 5000) &&
    !WaveDuoRules.CageMayStart(true, true, ZabuzaCombatRules.Approach, WaveDuoRules.CageCooldownTicks - 1) &&
    WaveDuoRules.CageMayStart(true, true, ZabuzaCombatRules.Approach, WaveDuoRules.CageCooldownTicks),
    "Cage starts only with both alive, Zabuza between attacks, off cooldown");
Check(WaveDuoRules.CageCooldownTicks is >= 840 and <= 960 &&
    WaveDuoRules.CageMirrorCount == 8 && WaveDuoRules.CageRadius is >= 300f and <= 340f,
    "Cage: about every 15 s, eight mirrors, about 20 tiles radius");
Check(!WaveDuoRules.CageShouldEnd(WaveDuoRules.CageHopTicks - 1, 3) &&
    WaveDuoRules.CageShouldEnd(WaveDuoRules.CageHopTicks, 0) &&
    WaveDuoRules.CageShouldEnd(10, WaveDuoRules.CageMirrorsToBreak) &&
    WaveDuoRules.CageBrokenEarly(4) && !WaveDuoRules.CageBrokenEarly(3),
    "Cage lasts about 8 s or until four mirrors are shattered");
Check(WaveDuoRules.PushInsideCage(400f, 0f, 320f, out float px, out float py) &&
    px < 320f && px > 250f && py == 0f &&
    !WaveDuoRules.PushInsideCage(100f, 50f, 320f, out _, out _),
    "Crossing the cage edge pushes the player back inside; the interior is free");
Check(WaveDuoRules.CageBoundaryDamage is >= 8 and <= 12 &&
    WaveDuoRules.CageBoundaryHurtCooldown >= 20,
    "The edge hurts lightly and not every frame");
Check(WaveDuoRules.PhaseInHop(0, false) == WaveDuoRules.HopPhase.Warn &&
    WaveDuoRules.PhaseInHop(WaveDuoRules.CageHopWarnTicks, false) == WaveDuoRules.HopPhase.Exposed &&
    WaveDuoRules.PhaseInHop(WaveDuoRules.CageHopWarnTicks + WaveDuoRules.CageHopExposedTicks, false) ==
        WaveDuoRules.HopPhase.Fade &&
    WaveDuoRules.CageHopCycleTicks == 60 && WaveDuoRules.CageHopExposedTicks == 30,
    "Each hop warns, then leaves Haku hittable for half a second");
Check(WaveDuoRules.HopThrowTick(false) == WaveDuoRules.CageHopWarnTicks &&
    WaveDuoRules.CageHopWarnTicks >= 15,
    "Needles fly only after the mirror has glowed");

bool[] all = Enumerable.Repeat(true, 8).ToArray();
int first = WaveDuoRules.NextMirror(-1, all, 0);
int second = WaveDuoRules.NextMirror(first, all, 1);
Check(first >= 0 && second >= 0 && second != first,
    "Haku always moves to a different mirror");
bool[] one = new bool[8];
one[5] = true;
Check(WaveDuoRules.NextMirror(2, one, 3) == 5 && WaveDuoRules.NextMirror(5, one, 4) == 5 &&
    WaveDuoRules.NextMirror(1, new bool[8], 0) == -1,
    "Broken mirrors are skipped; no mirrors means no hop");
Check(Math.Abs(WaveDuoRules.MirrorAngle(0, 8) + MathF.PI / 2f) < 0.001f &&
    Math.Abs(WaveDuoRules.MirrorAngle(4, 8) - MathF.PI / 2f) < 0.001f,
    "Mirror slots are spread evenly around the ring");

// Haku frenzy.
Check(WaveDuoRules.FrenzyMirrorCount == 6 && WaveDuoRules.FrenzyMirrorRespawnTicks is >= 300 and <= 420 &&
    WaveDuoRules.FrenzyHopCycleTicks < WaveDuoRules.CageHopCycleTicks &&
    WaveDuoRules.FrenzyFanNeedles > WaveDuoRules.CageFanNeedles &&
    WaveDuoRules.FrenzyHopWarnTicks >= 12,
    "Frenzy hops faster with more needles, but still warns; broken mirrors return");
Check(!WaveDuoRules.ThousandNeedleReady(0.31f, 5000) &&
    !WaveDuoRules.ThousandNeedleReady(0.2f, WaveDuoRules.ThousandNeedleCooldownTicks - 1) &&
    WaveDuoRules.ThousandNeedleReady(0.2f, WaveDuoRules.ThousandNeedleCooldownTicks) &&
    WaveDuoRules.ThousandNeedleWarnTicks is >= 44 and <= 52,
    "Thousand Needles unlocks below 30% life, every ~12 s, with ~0.8 s warning");
for (int cast = 0; cast < 10; cast++)
{
    int gap = WaveDuoRules.ThousandNeedleGapStart(cast);
    int skipped = Enumerable.Range(0, WaveDuoRules.ThousandNeedleCount)
        .Count(i => WaveDuoRules.ThousandNeedleSkipped(i, gap));
    Check(skipped == WaveDuoRules.ThousandNeedleGapSize &&
        WaveDuoRules.ThousandNeedleSkipped(gap, gap) &&
        WaveDuoRules.ThousandNeedleSkipped((gap + 1) % WaveDuoRules.ThousandNeedleCount, gap),
        $"Thousand Needles cast {cast} leaves a two-needle escape lane");
}

// Zabuza frenzy.
Check(WaveDuoRules.ZabuzaFrenzyDamageTakenMultiplier is >= 1.2f and <= 1.3f,
    "Frenzied Zabuza drops his guard and takes about 25% more damage");
Check(WaveDuoRules.SoftenedDamage(48) is >= 42 and <= 44 &&
    WaveDuoRules.SoftenedDamage(20) is >= 17 and <= 19,
    "Boss damage is reduced gently rather than gutted");
