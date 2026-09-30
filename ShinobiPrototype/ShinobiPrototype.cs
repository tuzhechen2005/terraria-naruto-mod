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
        WaveEpilogue,     // server -> clients: both bosses have fallen (haku fell first, where)
        TazunaTalk,       // client -> server: a player talked to Tazuna (whether he confessed)
        ExamSync,         // client -> server: a player's Chunin Exam progress (ChuninExamPlayer)
    }

    // Boss Checklist (optional mod): list Zabuza and Haku just after the Eye of Cthulhu (2.0), and the Chūnin Exam
    // fights where they fall among vanilla's bosses (Eater/Brain 3.0, Skeletron 5.0).
    public override void PostSetupContent()
    {
        if (!ModLoader.TryGetMod("BossChecklist", out Mod checklist))
            return;
        LogExamBoss(checklist, "MiniBoss", "Orochimaru", 3.2f, () => StoryWorld.OrochimaruMet,
            ModContent.NPCType<Content.NPCs.Orochimaru>(), ModContent.ItemType<Content.Items.SnakeSkin>(),
            ModContent.ItemType<Content.Items.StyleCores.SharinganCore1>());
        LogExamBoss(checklist, "MiniBoss", "Dosu", 3.5f, () => StoryWorld.DownedDosu,
            ModContent.NPCType<Content.NPCs.Dosu>(), ModContent.ItemType<Content.Items.SoundNinjaToken>());
        LogExamBoss(checklist, "Boss", "Gaara", 5.2f, () => StoryWorld.DownedGaara,
            ModContent.NPCType<Content.NPCs.Gaara>(), ModContent.ItemType<Content.Items.SandGourd>(),
            ModContent.ItemType<Content.Items.StyleCores.EightGatesCore>(), ModContent.ItemType<Content.Items.Taijutsu.LeeLegWeights>(),
            ModContent.ItemType<Content.Items.ChuninHeadband>());
        LogExamBoss(checklist, "Boss", "Neji", 5.3f, () => StoryWorld.DownedNeji,
            ModContent.NPCType<Content.NPCs.Neji>(), ModContent.ItemType<Content.Items.NejiChallengeScroll>(),
            ModContent.ItemType<Content.Items.StyleCores.ByakuganCore>());
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

    private void LogExamBoss(Mod checklist, string kind, string key, float progression, Func<bool> downed, int npc, int spawnItem,
        params int[] collectibles)
    {
        checklist.Call($"Log{kind}", this, key, progression, downed, npc, new Dictionary<string, object>
        {
            ["displayName"] = Language.GetText($"Mods.ShinobiPrototype.BossChecklist.{key}.DisplayName"),
            ["spawnInfo"] = Language.GetText($"Mods.ShinobiPrototype.BossChecklist.{key}.SpawnInfo"),
            ["spawnItems"] = spawnItem,
            ["collectibles"] = new List<int>(collectibles),
        });
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        switch ((Packet)reader.ReadByte())
        {
            case Packet.WaveEpilogue:
                bool hakuFirst = reader.ReadBoolean();
                Microsoft.Xna.Framework.Vector2 where = reader.ReadVector2();
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    WaveEpilogueSystem.Start(hakuFirst, where);
                break;
            case Packet.WaveFirstWin:
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    Main.LocalPlayer.GetModPlayer<Common.Players.WaveRewardPlayer>().ReceiveFirstWin();
                break;
            case Packet.TazunaTalk:
                bool confessed = reader.ReadBoolean();
                if (Main.netMode == NetmodeID.Server)
                    StoryWorld.ApplyTazunaTalk(confessed);
                break;
            case Packet.ExamSync:
                byte who = reader.ReadByte();
                Main.player[who].GetModPlayer<Common.Players.ChuninExamPlayer>().Read(reader);
                break;
            case Packet.BuildBridge:
                BridgeSite site = new(reader.ReadInt32(), reader.ReadSByte(), reader.ReadInt32());
                if (Main.netMode == NetmodeID.Server)
                    BridgeBlueprintNet.BuildOnServer(site, whoAmI);
                break;
        }
    }
}
