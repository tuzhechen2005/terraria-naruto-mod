using Terraria;

namespace ShinobiPrototype.Content.Projectiles;

// One hit per player for a hostile attack that lingers (specs/敌方伤害标准.spec.md): marked in ModifyHitPlayer, before
// any dodge, so a hit taken by a log counts too and the same cast cannot land again once the player's immunity ends.
// Allocated on first use, never in a template projectile, so no two projectiles share it.
public sealed class PlayerHits
{
    private readonly bool[] hit = new bool[Main.maxPlayers + 1];

    public bool Has(Player player) => hit[player.whoAmI];

    public void Mark(Player player) => hit[player.whoAmI] = true;

    public void Clear() => System.Array.Clear(hit);
}
