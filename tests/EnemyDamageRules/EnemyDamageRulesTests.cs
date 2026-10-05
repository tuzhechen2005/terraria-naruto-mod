using ShinobiPrototype.Common;
using static ShinobiPrototype.Common.EnemyDamageRules;

// The enemy damage table (specs/敌方伤害标准.spec.md): every attack within its cap, every big attack warned in time.

static void Check(bool condition, string name)
{
    if (!condition)
        throw new Exception(name);
    Console.WriteLine($"PASS {name}");
}

Check(ProjectileDamage(60) * 2 == 60 && ProjectileDamage(45) * 2 == 44 && ProjectileDamage(1) == 1,
    "A projectile is spawned with half (vanilla doubles it on the player), never above the table's number");
Check(Cap(AttackKind.Normal, 200) == 30 && Cap(AttackKind.Big, 200) == 60 && Cap(AttackKind.Lingering, 300) == 45,
    "Caps: 15% of the baseline for normal attacks and hazards, 30% for big ones");
Check(BigWindup(20f) == BigTelegraphTicks && BigWindup(60f, 1.5f) == 40 && BigWindup(30f, 0.7f) == 43,
    "A phase's tempo shortens a big attack's windup, never below 24 ticks");

var table = EnemyDamageTable.Table;
Check(table.Count >= 50 && table.Select(a => a.Name).Distinct().Count() == table.Count, "Every attack has one row");
foreach (EnemyAttack attack in table)
{
    Check(attack.Damage > 0 && attack.Damage <= Cap(attack.Kind, attack.Baseline),
        $"{attack.Name}: {attack.Damage} within the {attack.Kind} cap of {Cap(attack.Kind, attack.Baseline)} (baseline {attack.Baseline})");
    if (attack.Kind == AttackKind.Big)
        Check(attack.TelegraphTicks >= BigTelegraphTicks, $"{attack.Name}: a big attack warned {attack.TelegraphTicks} ticks ahead (>= 24)");
}
Check(Genin.CandidateStrikeWindupTicks >= InstantMeleeTicks && Neji.FirstPalmTick >= InstantMeleeTicks,
    "A strike that appears beside the player shows its swing for at least 15 ticks");
Check(new[] { Brothers.Baseline, WaterClone.Baseline, Zabuza.Baseline, Genin.Baseline, Orochimaru.Baseline, Neji.Baseline }
        .SequenceEqual(new[] { 100, 200, 200, 300, 300, 400 }),
    "Baselines follow the story gates: 100, 200 for Zabuza, 300 for the forest, 400 for the finals");
Check(ExamBossRules.OrochimaruDamage(Orochimaru.Kusanagi, false) < Orochimaru.Kusanagi,
    "Orochimaru still eases his damage for a player not yet strong enough");

// The world's ninja enemies (specs/空档衔接与火影小兵.spec.md).
Check(WorldNinjaRules.Substitutes(9, 90, false) && !WorldNinjaRules.Substitutes(8, 90, false),
    "A rogue genin substitutes a hit worth a tenth of its life");
Check(!WorldNinjaRules.Substitutes(30, 90, false) && !WorldNinjaRules.Substitutes(20, 90, true),
    "A hit of a third or more (a stealth throw) breaks through, and it substitutes only once");
Check(WorldNinjaRules.RoninNightWeight > WorldNinjaRules.RogueGeninWeight, "Ronin are common at night, rogue genin rare");
