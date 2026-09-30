namespace ShinobiPrototype.Common;

// The four schools of the master spec (流派): a core accessory carries a school at a tier; only one core may be worn
// (see specs/流派系统.spec.md). The per-school passives and techniques wait on the user's decisions; this is only
// the shared frame. Kept free of Terraria types so the rule tests can run it.
public enum StyleSchool : byte { None, Sharingan, EightGates, Byakugan, Sage }

public static class StyleCoreRules
{
    // A second core cannot join the accessories while one is worn; swapping one core for another in the same slot is
    // fine (the slot's current item is the one being replaced).
    public static bool CanEquip(bool anotherCoreWorn) => !anotherCoreWorn;

    // The technique fires only with a core worn, off cooldown and with the chakra for it.
    public static bool CanUseTechnique(StyleSchool worn, int cooldownTicks, int chakra, int cost) =>
        worn != StyleSchool.None && cooldownTicks <= 0 && chakra >= cost;

    // A higher tier of the same school includes everything below it.
    public static bool Includes(StyleSchool school, int tier, StyleSchool askedSchool, int askedTier) =>
        school == askedSchool && tier >= askedTier;
}
