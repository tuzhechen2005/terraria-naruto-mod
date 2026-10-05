namespace ShinobiPrototype.Common;

// The world's ninja enemies (specs/空档衔接与火影小兵.spec.md): numbers and decisions kept free of Terraria types for
// the rule tests. Damage numbers are in EnemyDamageRules.
public static class WorldNinjaRules
{
    // Share of surface spawns (a weight beside vanilla's pool, where the usual night enemies weigh about 1).
    public const float RoninNightWeight = 0.25f;
    public const float RogueGeninWeight = 0.06f;

    public const int RoninLife = 70;
    public const int RoninDefense = 6;
    public const int RogueGeninLife = 90;
    public const int RogueGeninDefense = 8;

    public const float RoninSpeed = 2.2f;
    public const float RogueGeninSpeed = 3f;
    public const int MeleeReachTiles = 3;
    public const int RogueKeepAwayTiles = 7;
    public const int RogueThrowEvery = 90;
    public const int MeleeCooldown = 50;
    public const int StrikeShowTicks = 12;

    // A rogue genin's substitution (a mirror of the player's logs): the first hit worth at least a tenth of its life
    // turns into a log and it reappears beside the player; a hit of a third or more (a stealth throw) breaks through.
    public static bool Substitutes(int damage, int lifeMax, bool used) =>
        !used && damage * 10 >= lifeMax && damage * 3 < lifeMax;

    // Below this share of life, a rogue genin throws a smoke bomb and runs for a while before coming back.
    public const float FleeBelow = 0.3f;
    public const int FleeTicks = 4 * 60;
}
