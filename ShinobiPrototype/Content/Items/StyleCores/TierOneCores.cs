using Terraria;
using Terraria.ID;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;

namespace ShinobiPrototype.Content.Items.StyleCores;

// The tier-one cores obtainable in the Chūnin Exams (specs/流派系统.spec.md section 3). Icons from m2-style-items-v1;
// the Byakugan keeps a vanilla placeholder until byakugan-icon-v2.

// The Sharingan in a vial (one tomoe), from Orochimaru in the Forest of Death: a wider substitution window, and
// Foresight, which takes the next hit with a free substitution.
public sealed class SharinganCore1 : StyleCore
{
    public override StyleSchool School => StyleSchool.Sharingan;
    public override int Tier => 1;
    public override int TechniqueCooldown => StyleCoreRules.ForesightCooldownTicks;
    public override int TechniqueCost(Player player) => StyleCoreRules.ForesightCost;

    public override void ApplyPassive(Player player) =>
        player.GetModPlayer<StyleCorePlayer>().SharinganTier = System.Math.Max(player.GetModPlayer<StyleCorePlayer>().SharinganTier, Tier);

    public override void UseTechnique(Player player) =>
        player.GetModPlayer<StyleCorePlayer>().StartForesight(StyleCoreRules.ForesightTicks(Tier));
}

// The Eight Inner Gates (first three), from Gaara: taijutsu strikes faster, and each press opens another gate.
public sealed class EightGatesCore : StyleCore
{
    public override StyleSchool School => StyleSchool.EightGates;
    public override int Tier => 1;
    public override int TechniqueCooldown => 30;
    public override int TechniqueCost(Player player) =>
        StyleCoreRules.GatePressCost(player.GetModPlayer<StyleCorePlayer>().Gates);

    public override void ApplyPassive(Player player)
    {
        player.GetModPlayer<StyleCorePlayer>().GatesCore = true;
        player.GetAttackSpeed<Common.DamageClasses.TaijutsuDamage>() += StyleCoreRules.TaijutsuAttackSpeed;
    }

    public override void UseTechnique(Player player) => player.GetModPlayer<StyleCorePlayer>().PressGates();
}

// The Byakugan, from Neji: enemies seen through walls, melee hits seal their points, and the Rotation.
public sealed class ByakuganCore : StyleCore
{
    public override string Texture => $"Terraria/Images/Item_{ItemID.Lens}";
    public override StyleSchool School => StyleSchool.Byakugan;
    public override int Tier => 1;
    public override int TechniqueCooldown => StyleCoreRules.RotationCooldownTicks;
    public override int TechniqueCost(Player player) => StyleCoreRules.RotationCost;

    public override void ApplyPassive(Player player) => player.GetModPlayer<StyleCorePlayer>().Byakugan = true;

    public override void UseTechnique(Player player) => player.GetModPlayer<StyleCorePlayer>().StartRotation();
}
