using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Systems;

namespace ShinobiPrototype;

public sealed class ShinobiPrototype : Mod
{
    internal enum Packet : byte
    {
        BuildBridge,      // client -> server: shore X, direction, water Y
        WaveFirstWin,     // server -> one client: grant that character's first-win rewards
    }

    // Boss Checklist (optional mod): list Zabuza and Haku just after the Eye of Cthulhu (2.0).
    public override void PostSetupContent()
    {
        if (!ModLoader.TryGetMod("BossChecklist", out Mod checklist))
            return;
        int zabuza = ModContent.NPCType<Content.NPCs.ZabuzaBoss>();
        int haku = ModContent.NPCType<Content.NPCs.HakuBoss>();
        checklist.Call("LogBoss", this, "WaveDuo", 2.1f, (Func<bool>)(() => StoryWorld.WaveComplete),
            new List<int> { zabuza, haku }, new Dictionary<string, object>
            {
                ["displayName"] = Language.GetText("Mods.ShinobiPrototype.BossChecklist.WaveDuo.DisplayName"),
                ["spawnInfo"] = Language.GetText("Mods.ShinobiPrototype.BossChecklist.WaveDuo.SpawnInfo"),
                ["spawnItems"] = ModContent.ItemType<Content.Items.ZabuzaChallengeScroll>(),
                ["collectibles"] = new List<int>
                {
                    ModContent.ItemType<Content.Items.WaveBossBag>(),
                    ModContent.ItemType<Content.Items.WaveCountryMedal>(),
                    ModContent.ItemType<Content.Items.ChakraCrystal>(),
                    ModContent.ItemType<Content.Items.Weapons.Kubikiribocho>(),
                    ModContent.ItemType<Content.Items.Weapons.Senbon>(),
                    ModContent.ItemType<Content.Items.Weapons.WaterDragonJutsu>(),
                    ModContent.ItemType<Content.Items.Weapons.IceMirrorJutsu>(),
                },
            });
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        switch ((Packet)reader.ReadByte())
        {
            case Packet.WaveFirstWin:
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    Main.LocalPlayer.GetModPlayer<Common.Players.WaveRewardPlayer>().ReceiveFirstWin();
                break;
            case Packet.BuildBridge:
                BridgeSite site = new(reader.ReadInt32(), reader.ReadSByte(), reader.ReadInt32());
                if (Main.netMode == NetmodeID.Server)
                    BridgeBlueprintNet.BuildOnServer(site, whoAmI);
                break;
        }
    }
}
