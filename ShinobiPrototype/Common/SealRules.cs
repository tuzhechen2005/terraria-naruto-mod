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
}
