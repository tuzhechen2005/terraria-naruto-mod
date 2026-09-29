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

Check(CheckSubstitution(SubstitutionCost, 0) == Activation.Ready, "Substitution ready with 20 chakra");
Check(CheckSubstitution(SubstitutionCost - 1, 0) == Activation.NotEnoughChakra, "Substitution needs 20 chakra");
Check(CheckSubstitution(100, 1) == Activation.CoolingDown, "Substitution blocked while cooling down");
Check(SubstitutionWindowTicks == 24 && SubstitutionCooldownTicks == 240, "0.4 s standby, 4 s cooldown");
Check(SubstitutionCooldownTicks > SubstitutionWindowTicks, "Cooldown outlasts the standby window");

Check(ShouldShowHint(false, 0, SubstitutionHintSpacingTicks, 100, 0), "First hit shows the hint");
Check(!ShouldShowHint(true, 0, SubstitutionHintSpacingTicks, 100, 0), "No hint once the jutsu has succeeded");
Check(!ShouldShowHint(false, SubstitutionMaxHints, SubstitutionHintSpacingTicks, 100, 0), "At most three hints");
Check(!ShouldShowHint(false, 1, SubstitutionHintSpacingTicks - 1, 100, 0), "Hints are spaced apart");
Check(!ShouldShowHint(false, 0, SubstitutionHintSpacingTicks, 5, 0), "No hint when the jutsu could not be used");

var offsets = LandingOffsets(1);
Check(offsets[0] == (8, 0), "Landing tries level ground away from the attacker first");
Check(offsets.Take(offsets.Length / 2).All(o => o.X > 0) && offsets.Skip(offsets.Length / 2).All(o => o.X < 0),
    "Away side is tried before the attacker's side");
Check(LandingOffsets(-1)[0] == (-8, 0), "Direction flips with the hit");
