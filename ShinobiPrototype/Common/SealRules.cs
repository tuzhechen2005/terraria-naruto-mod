using System;

namespace ShinobiPrototype.Common;

// Hand seals (specs/装备与忍术系统.spec.md): hold the seal key and the seals form one by one, a quarter second each,
// up to six; let go and the jutsu of the slot reached goes off (2, 4 or 6 seals, each slot holding one scroll).
// Kept free of Terraria types so the rule tests can run them.
public static class SealRules
{
    public const int TicksPerSeal = 15;
    public const int MaxSeals = 6;
    public const float WeaveSpeed = 0.4f;   // of the normal running speed

    // Seals formed after holding the key this long: the first after a quarter second, six after a second and a half.
    public static int SealsAfter(int heldTicks) => Math.Clamp(heldTicks / TicksPerSeal, 0, MaxSeals);

    // The slot that goes off when the key is let go: the highest reached that holds a scroll (0 for none).
    public static int Tier(int seals, bool has2, bool has4, bool has6) =>
        seals >= 6 && has6 ? 6 : seals >= 4 && has4 ? 4 : seals >= 2 && has2 ? 2 : 0;

    // The slot being worked towards, for the seals shown overhead.
    public static int Toward(int seals) => seals <= 2 ? 2 : seals <= 4 ? 4 : 6;

    public static int SlotIndex(int tier) => tier switch { 2 => 0, 4 => 1, 6 => 2, _ => -1 };

    // The great techniques (user, 2026-10-03: big, flashy, and not to be fired back to back).
    // Fire Style: Fireball Jutsu — swells from a mouthful to a ball thirteen tiles across, rolls slowly through walls for
    // two seconds burning what it touches and the ground below, then bursts.
    public const int FireballCost = 40;
    public const int FireballCooldownTicks = 360;
    public const int FireballLifeTicks = 120;
    public const int FireballGrowTicks = 30;
    public const float FireballStartPx = 40f;
    public const float FireballFullPx = 208f;
    public const float FireballSpeed = 4.5f;
    public const int FireballHitCooldownTicks = 15;
    public const int GroundFireTicks = 180;
    public const float GroundFireDamageShare = 0.25f;
    public const float FireballBurstDamageShare = 1.5f;

    // The fireball's width after `age` ticks: it swells, then holds.
    public static float FireballSize(int age) =>
        FireballStartPx + (FireballFullPx - FireballStartPx) * Math.Clamp(age / (float)FireballGrowTicks, 0f, 1f);

    // Chidori — a third of a second of lightning gathering in the hand, then a charge of forty tiles that stops at a wall,
    // through everything small and into the first boss, leaving lightning chakra along the way.
    public const int ChidoriCost = 60;
    public const int ChidoriCooldownTicks = 600;
    public const int ChidoriWindupTicks = 18;
    public const float ChidoriReachPx = 640f;
    public const float ChidoriSpeed = 26f;
    public const float ChidoriBossMultiplier = 2.5f;
    public const int LightningTrailTicks = 180;
    public const int LightningTrailHitTicks = 15;
    public const float LightningTrailDamageShare = 0.1f;

    public static int ChidoriChargeTicks => (int)Math.Ceiling(ChidoriReachPx / ChidoriSpeed);
}
