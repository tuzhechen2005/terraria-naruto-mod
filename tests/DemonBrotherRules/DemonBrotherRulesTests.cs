using ShinobiPrototype.Common;
using static ShinobiPrototype.Common.DemonBrotherRules;

static void Check(bool condition, string name)
{
    if (!condition)
        throw new Exception(name);
    Console.WriteLine($"PASS {name}");
}

Check(SpawnAllowed(true, true, false, false, false, true) && SpawnAllowed(true, false, true, false, false, true),
    "They ambush in rain or in the sea mist");
Check(!SpawnAllowed(false, true, true, false, false, true), "Not before the player has found a Mist insignia");
Check(!SpawnAllowed(true, false, false, false, false, true), "Not in clear weather away from the mist");
Check(!SpawnAllowed(true, true, true, true, false, true) && SpawnAllowed(true, true, false, true, false, true, onBeach: true) &&
      !SpawnAllowed(true, false, true, true, false, true, onBeach: true),
    "Once Wave Country is done, only in the rain on the beach");
Check(!SpawnAllowed(true, true, true, false, true, true), "Only one pair at a time");
Check(!SpawnAllowed(true, true, true, false, false, false), "Only on the surface");

Check(FlankX(1000f, -1) < 1000f && FlankX(1000f, 1) > 1000f, "The brothers take opposite sides of the player");
Check(ChainSweepReady(0, 300f, true) && !ChainSweepReady(10, 300f, true) && !ChainSweepReady(0, 300f, false) &&
      !ChainSweepReady(0, ChainMaxLength + 1f, true) && !ChainSweepReady(0, 20f, true),
    "Chain sweep needs the cooldown over, the player between them and a sensible gap");
Check(ChainWarnTicks >= 30, "The chain sweep is telegraphed for at least half a second");
Check(ChainHurts(true, 200f) && !ChainHurts(false, 200f), "A slack chain does not hurt");
Check(ChainRushSpeed * ChainRushTicks > ChainMaxLength / 2f, "The rush carries each brother past the middle");

Check(AmbushDue(false, true, true, true, false, false, 0), "The first ambush is certain in the bridge mist once an insignia is found");
Check(!AmbushDue(true, true, true, true, false, false, 0), "After the first win they only come at random");
Check(!AmbushDue(false, false, true, true, false, false, 0) && !AmbushDue(false, true, false, true, false, false, 0),
    "Needs an insignia and the sea mist");
Check(!AmbushDue(false, true, true, true, false, true, 0) && !AmbushDue(false, true, true, true, false, false, 10),
    "Not while a pair is out or during the retry wait");
Check(AmbushRetryTicks >= 60 * 60 && AmbushWarnTicks >= 60, "A warning first, and a minute before trying again");
