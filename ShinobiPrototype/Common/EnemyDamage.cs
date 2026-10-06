using Terraria;
using Terraria.DataStructures;

namespace ShinobiPrototype.Common;

// How an enemy's damage reaches the player, so that the numbers in EnemyDamageRules are what the player takes in
// normal mode before defence (specs/敌方伤害标准.spec.md). Every hostile projectile, contact damage and direct hurt of
// this mod goes through here.
public static class EnemyDamage
{
    // Vanilla doubles a hostile projectile's damage when it hits a player; the same in every mode, as in vanilla.
    public static int Projectile(int actual) => EnemyDamageRules.ProjectileDamage(actual);

    // Contact damage set while an NPC fights (assigning NPC.damage every tick would drop vanilla's Expert and Master
    // scaling, which only applies once at spawn): normal-mode damage times the mode's enemy-damage multiplier.
    public static int Contact(NPC npc, int actual) => actual <= 0 ? 0 : npc.GetAttackDamage_ScaledByStrength(actual);

    // A direct hurt (the mirror cage's edge, the Demon Brothers' chain), scaled like contact damage.
    public static double Hurt(Player player, PlayerDeathReason reason, int actual, int hitDirection, int cooldownCounter = -1) =>
        player.Hurt(reason, (int)(actual * Main.GameModeInfo.EnemyDamageMultiplier), hitDirection, cooldownCounter: cooldownCounter);
}
