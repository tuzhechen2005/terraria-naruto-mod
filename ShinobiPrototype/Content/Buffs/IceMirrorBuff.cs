using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.Buffs;

// Keeps the ice-mirror minions alive while the player has any.
public sealed class IceMirrorBuff : ModBuff
{
    public override string Texture => $"Terraria/Images/Buff_{BuffID.Frostburn}";

    public override void SetStaticDefaults()
    {
        Main.buffNoSave[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (player.ownedProjectileCounts[ModContent.ProjectileType<IceMirrorMinion>()] > 0)
            player.buffTime[buffIndex] = 18000;
        else
        {
            player.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}
