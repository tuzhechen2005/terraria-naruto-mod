using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Items;
using ShinobiPrototype.Content.Items.Weapons;

namespace ShinobiPrototype.Common.Systems;

// Rewards for beating Zabuza and Haku, settled once when the second of them falls (server or single player).
internal static class WaveRewards
{
    private const float FirstWinRangeTiles = 150f;

    public static int ItemFor(WaveLoot loot) => loot switch
    {
        WaveLoot.ZabuzaHeadband => ModContent.ItemType<ZabuzaHeadband>(),
        WaveLoot.HakuMask => ModContent.ItemType<HakuMask>(),
        WaveLoot.Kubikiribocho => ModContent.ItemType<Kubikiribocho>(),
        WaveLoot.Senbon => ModContent.ItemType<Senbon>(),
        WaveLoot.WaterDragon => ModContent.ItemType<WaterDragonJutsu>(),
        WaveLoot.IceMirror => ModContent.ItemType<IceMirrorJutsu>(),
        _ => 0,
    };

    public static void Settle(NPC lastBoss)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;

        if (Main.expertMode)
            lastBoss.DropItemInstanced(lastBoss.position, lastBoss.Size, ModContent.ItemType<WaveBossBag>());
        else
            foreach (WaveLoot loot in WaveLootRules.Roll(Main.rand.Next, Main.rand.NextDouble))
            {
                int type = ItemFor(loot);
                if (type > 0)
                    Item.NewItem(lastBoss.GetSource_Loot(), lastBoss.getRect(), type);
            }

        // First-win rewards go to each character who was there, once per character.
        foreach (Player player in Main.ActivePlayers)
        {
            if (player.Distance(lastBoss.Center) > FirstWinRangeTiles * 16f)
                continue;
            if (Main.netMode == NetmodeID.SinglePlayer)
                player.GetModPlayer<WaveRewardPlayer>().ReceiveFirstWin();
            else
            {
                ModPacket packet = ModContent.GetInstance<ShinobiPrototype>().GetPacket();
                packet.Write((byte)ShinobiPrototype.Packet.WaveFirstWin);
                packet.Send(toClient: player.whoAmI);
            }
        }
    }
}
