using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.Buffs;

// Stealth after a blink (specs/装备与忍术系统.spec.md): the next hit is always critical and harder (StealthPlayer
// spends it). Shown with the other buffs so its time left is plain (user, 2026-10-03). Art: stealth-buff-icon-v1;
// until it is in, vanilla's Invisibility icon stands in.
public sealed class StealthBuff : ModBuff
{
    public override string Texture => ModContent.HasAsset("ShinobiPrototype/Content/Buffs/StealthBuff")
        ? "ShinobiPrototype/Content/Buffs/StealthBuff"
        : $"Terraria/Images/Buff_{BuffID.Invisibility}";

    public override void SetStaticDefaults()
    {
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        Main.buffNoSave[Type] = true;
    }
}
