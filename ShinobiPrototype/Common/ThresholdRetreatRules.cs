using System;

namespace ShinobiPrototype.Common;

// The "fight you cannot win" pattern from the master spec (总纲 “原作中‘打不赢’的战斗”): the boss is beaten down to a
// threshold, then locks its life there, turns invulnerable and plays its exit. Used by encounter fights such as
// Orochimaru in the Forest of Death (M2 draft). Kept free of Terraria types so the rule tests can run it.
public static class ThresholdRetreatRules
{
    public const float DefaultThreshold = 0.5f;
    // How long the boss stays untouchable while its exit plays.
    public const int ExitInvulnerableTicks = 240;

    // Life the boss is held at once the threshold is reached (rounded up, so it never reaches 0). ClampDamage keeps a
    // single burst hit from carrying it past this.
    public static int LockedLife(int lifeMax, float threshold = DefaultThreshold) =>
        Math.Max(1, (int)Math.Ceiling(lifeMax * threshold));

    public static bool Reached(int life, int lifeMax, float threshold = DefaultThreshold) =>
        life <= LockedLife(lifeMax, threshold);

    // Damage this hit may actually deal before the lock catches it.
    public static int ClampDamage(int life, int lifeMax, int damage, float threshold = DefaultThreshold) =>
        Math.Max(0, Math.Min(damage, life - LockedLife(lifeMax, threshold)));
}
