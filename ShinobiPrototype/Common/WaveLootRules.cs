using System;
using System.Collections.Generic;

namespace ShinobiPrototype.Common;

public enum WaveLoot { Kubikiribocho, Senbon, WaterDragon, IceMirror, ZabuzaHeadband, HakuMask }

// What one win over Zabuza and Haku drops (M1 spec, "波之国 Boss 奖励"): one of the four class weapons for certain,
// about a 20% chance of a second, different one, and each boss mask at the vanilla boss-mask rate of 1 in 7. The
// same roll fills the expert treasure bag. The chakra crystal and medal are per-character first-win rewards and are
// handled separately. Kept free of Terraria types so the rule tests can run it.
public static class WaveLootRules
{
    public static readonly WaveLoot[] Weapons =
        { WaveLoot.Kubikiribocho, WaveLoot.Senbon, WaveLoot.WaterDragon, WaveLoot.IceMirror };

    public const double ExtraWeaponChance = 0.2;
    public const int MaskOneIn = 7;
    // Trophies drop from the boss itself in every mode (not in the bag), at vanilla's rate.
    public const int TrophyOneIn = 10;

    // nextInt(n) returns 0..n-1; nextDouble() returns [0, 1).
    public static List<WaveLoot> Roll(Func<int, int> nextInt, Func<double> nextDouble)
    {
        List<WaveLoot> loot = new();
        int first = nextInt(Weapons.Length);
        loot.Add(Weapons[first]);
        if (nextDouble() < ExtraWeaponChance)
        {
            int second = nextInt(Weapons.Length - 1);
            if (second >= first)
                second++;
            loot.Add(Weapons[second]);
        }
        if (nextInt(MaskOneIn) == 0)
            loot.Add(WaveLoot.ZabuzaHeadband);
        if (nextInt(MaskOneIn) == 0)
            loot.Add(WaveLoot.HakuMask);
        return loot;
    }
}
