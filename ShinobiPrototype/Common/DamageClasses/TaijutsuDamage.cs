using Terraria.ModLoader;

namespace ShinobiPrototype.Common.DamageClasses;

// Taijutsu (体术): fists and kicks (specs/流派系统.spec.md, "体术武器"). Counts as melee for every melee bonus and
// accessory; the Eight Gates add their own bonus on top.
public sealed class TaijutsuDamage : DamageClass
{
    public override StatInheritanceData GetModifierInheritance(DamageClass damageClass) =>
        damageClass == Melee || damageClass == Generic ? StatInheritanceData.Full : StatInheritanceData.None;

    public override bool GetEffectInheritance(DamageClass damageClass) => damageClass == Melee;
}
