using ShinobiPrototype.Common;
using static ShinobiPrototype.Common.ChakraRules;

static void Check(bool condition, string name)
{
    if (!condition)
        throw new Exception(name);
    Console.WriteLine($"PASS {name}");
}

Check(MaxChakra(0) == 100 && MaxChakra(1) == 120, "Crystals start at 100 and add 20 each");
Check(MaxChakra(MaxCrystals) == 200 && MaxChakra(MaxCrystals + 3) == 200, "Pre-Hardmode maximum is 200");
Check(CanUseCrystal(MaxCrystals - 1) && !CanUseCrystal(MaxCrystals), "Crystals stop working at the cap");

Check(Math.Abs(RegenPerTick(RegenDelayTicks) * 60f - 3f) < 0.001f &&
      Math.Abs(RegenPerTick(0) * 60f - 10f) < 0.001f, "Natural regen: 3/s after casting, 10/s otherwise");

int restored = 0;
for (int hit = 0; hit < 10; hit++)
    restored += HitRegen(restored);
Check(restored == HitRegenPerSecondCap, "Any number of hits in one second restores at most 5");
Check(HitRegen(0) == HitRegenPerHit && HitRegen(4) == 1, "A single hit restores 2, trimmed at the cap");

Check(CheckSubstitution(SubstitutionCost, 0) == Activation.Ready, "A blink is ready with 15 chakra and a log");
Check(CheckSubstitution(SubstitutionCost - 1, 0) == Activation.NotEnoughChakra, "A blink needs 15 chakra");
Check(CheckSubstitution(100, 0, SubstitutionCost, 0) == Activation.NoLog, "A blink needs a log");
Check(CheckSubstitution(100, 1) == Activation.CoolingDown, "No second blink on the same press");
Check(SubstitutionCooldownTicks <= 30, "The key is never kept waiting: logs, not a cooldown, set the pace");

Check(StartingLogs == 2 && LogRegenTicks == 600 && LogRegenPerHitTicks == 30, "Two logs to start, one back every 10 s, half a second sooner per hit");
Check(AutoSubstitutes(1, false, true) && !AutoSubstitutes(0, false, true) && !AutoSubstitutes(2, true, true) && !AutoSubstitutes(2, false, false),
    "A log takes an enemy's hit by itself, unless none is left or the chakra points are sealed; falls and lava are not dodged");
Check(TickLogs(0, 2, LogRegenTicks - 1, LogRegenTicks) == (1, 0), "A log comes back after 10 s");
Check(TickLogs(1, 2, 0, LogRegenTicks, LogRegenPerHitTicks) == (1, 1 + LogRegenPerHitTicks), "Landing a hit brings the next log sooner");
Check(TickLogs(2, 2, 300, LogRegenTicks) == (2, 0), "At the cap nothing builds up");
Check(StealthTicks == 120 && StealthDamageBonus > 0f, "Two seconds of stealth after a substitution, the next hit stronger");

Check(ShouldShowHint(false, 0, SubstitutionHintSpacingTicks), "The first log taken explains the key");
Check(!ShouldShowHint(true, 0, SubstitutionHintSpacingTicks), "No hint once the player has used the key");
Check(!ShouldShowHint(false, SubstitutionMaxHints, SubstitutionHintSpacingTicks), "At most three hints");
Check(!ShouldShowHint(false, 1, SubstitutionHintSpacingTicks - 1), "Hints are spaced apart");

var offsets = LandingOffsets(1);
Check(offsets[0] == (8, 0), "Landing tries level ground away from the attacker first");
Check(offsets.Take(offsets.Length / 2).All(o => o.X > 0) && offsets.Skip(offsets.Length / 2).All(o => o.X < 0),
    "Away side is tried before the attacker's side");
Check(LandingOffsets(-1)[0] == (-8, 0), "Direction flips with the hit");

Check(SubstitutionCostFor(true) == 0 && SubstitutionCostFor(false) == SubstitutionCost, "Drill substitutions are free");
Check(SubstitutionCooldownFor(true) < PracticeGapMinTicks, "Jutsu is ready again before the next drill kunai");
Check(CheckSubstitution(0, 0, SubstitutionCostFor(true)) == Activation.Ready, "Drill works with empty chakra");
Check(PracticeWindupMinTicks < PracticeWindupMaxTicks && PracticeGapMinTicks < PracticeGapMaxTicks,
    "Drill timing varies between throws");
float flightTicks = PracticeMinRangeTiles * 16f / PracticeKunaiSpeed;
Check(flightTicks > SubstitutionWindowTicks, "Closest kunai is in the air longer than the standby window, so it can be read");
Check(CheckPracticeThrow(PracticeMinRangeTiles - 1, 0) == PracticeReadiness.TooClose, "Kakashi waits when the player is too close");
Check(CheckPracticeThrow(PracticeMaxRangeTiles + 1, 0) == PracticeReadiness.TooFar, "Kakashi waits when the player is too far");
Check(CheckPracticeThrow(15, 1) == PracticeReadiness.CoolingDown, "No new throw while substitution cools down");
Check(CheckPracticeThrow(15, 0) == PracticeReadiness.Ready, "Throws when in range and ready");
Check(!MayReleasePracticeKunai(40, 30, 5) && MayReleasePracticeKunai(40, 30, 0) && !MayReleasePracticeKunai(10, 30, 0),
    "An aimed kunai waits for both its windup and the cooldown");
Check(PracticeMaxRangeTiles < PracticeLeashTiles, "Too-far waiting happens before the drill is abandoned");
Check(JudgePracticeHit(true, 5) == PracticeOutcome.Substituted, "Standby at impact is a success");
Check(JudgePracticeHit(false, 30) == PracticeOutcome.TooEarly, "Pressed recently but expired: too early");
Check(JudgePracticeHit(false, PracticeEarlyWindowTicks + 1) == PracticeOutcome.TooLate, "No recent press: too late");
Check(PracticeVerdict(8, 8) != PracticeVerdict(4, 8) && PracticeVerdict(4, 8) != PracticeVerdict(1, 8),
    "Drill summary depends on the score");
