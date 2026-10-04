using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ShinobiPrototype.Content.Items;

namespace ShinobiPrototype.Common.Players;

// Per-character first-win rewards for Zabuza and Haku: a chakra crystal and the Wave Country medal, once.
public sealed class WaveRewardPlayer : ModPlayer
{
    public bool FirstWinClaimed { get; private set; }

    public override void Initialize() => FirstWinClaimed = false;

    // Runs on the rewarded player's own client (or in single player).
    public void ReceiveFirstWin()
    {
        if (FirstWinClaimed || Player.whoAmI != Main.myPlayer)
            return;
        FirstWinClaimed = true;
        var source = Player.GetSource_Misc("WaveFirstWin");
        Player.QuickSpawnItem(source, ModContent.ItemType<ChakraCrystal>());
        Player.QuickSpawnItem(source, ModContent.ItemType<WaveCountryMedal>());
        Main.NewText(Loc.Get("Wave.FirstWin"), new Color(255, 215, 120));
    }

    public override void SaveData(TagCompound tag)
    {
        if (FirstWinClaimed)
            tag["waveFirstWin"] = true;
    }

    public override void LoadData(TagCompound tag) => FirstWinClaimed = tag.GetBool("waveFirstWin");
}
