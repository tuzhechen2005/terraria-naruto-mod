using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.Buffs;

// Keeps Pakkun with the player while any is summoned.
public sealed class PakkunBuff : ModBuff
{
    public override string Texture => "ShinobiPrototype/Content/Items/Weapons/NinkenScroll";

    public override void SetStaticDefaults()
    {
        Main.buffNoSave[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (player.ownedProjectileCounts[ModContent.ProjectileType<PakkunMinion>()] > 0)
            player.buffTime[buffIndex] = 18000;
        else
        {
            player.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}

// Pakkun's scent on an enemy: every minion hit on it deals 3 more, like a whip's tag.
public sealed class PakkunScent : ModBuff
{
    public const int Ticks = 5 * 60;
    public const int MinionBonus = 3;

    public override string Texture => $"Terraria/Images/Buff_{BuffID.Hunter}";

    public override void SetStaticDefaults() => Main.debuff[Type] = true;
}

public sealed class PakkunScentNPC : GlobalNPC
{
    public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        if (npc.HasBuff<PakkunScent>() && (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type]))
            modifiers.FlatBonusDamage += PakkunScent.MinionBonus;
    }
}
