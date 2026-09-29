using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Buffs;

// Soldier-pill cooldown, like Mana Sickness: blocks eating another pill until it wears off.
public sealed class ChakraSickness : ModBuff
{
    public override string Texture => $"Terraria/Images/Buff_{BuffID.ManaSickness}";

    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
    }
}
