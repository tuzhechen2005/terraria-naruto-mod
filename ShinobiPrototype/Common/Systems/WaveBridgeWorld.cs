using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Common.Systems;

// The Wave Country bridge: generated with the world (or built from Tazuna's blueprint in older worlds), finished
// after the first win over Zabuza and Haku. Also keeps Tazuna at his hut. (The mist preview is per character:
// MistEncounterPlayer.)
public sealed class WaveBridgeWorld : ModSystem
{
    public static BridgeSite? Site { get; private set; }
    public static bool Finished { get; private set; }
    // Which BridgeDesign.Version built this world's bridge (0 = unknown: built before versions were recorded).
    public static int BuiltVersion { get; private set; }
    private static bool tazunaMovedIn;

    // Mist lifts for good once Wave Country is complete, even before the bridge is physically finished.
    public static bool MistActive => Site.HasValue && !Finished && !StoryWorld.WaveComplete;

    public override void ClearWorld()
    {
        Site = null;
        Finished = false;
        BuiltVersion = 0;
        tazunaMovedIn = false;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        if (Site is not BridgeSite site)
            return;
        tag["bridge"] = new[] { site.ShoreX, site.Dir, site.WaterY, site.HutMidX, site.HutFloorY, site.HutDoorX };
        tag["bridgeFinished"] = Finished;
        tag["bridgeVersion"] = BuiltVersion;
        tag["tazunaMovedIn"] = tazunaMovedIn;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        if (tag.GetIntArray("bridge") is { Length: 6 } b)
            Site = new BridgeSite(b[0], b[1], b[2], b[3], b[4], b[5]);
        Finished = tag.GetBool("bridgeFinished");
        BuiltVersion = tag.GetInt("bridgeVersion");
        tazunaMovedIn = tag.GetBool("tazunaMovedIn");
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write(Site.HasValue);
        if (Site is BridgeSite site)
        {
            writer.Write(site.ShoreX);
            writer.Write((sbyte)site.Dir);
            writer.Write(site.WaterY);
            writer.Write(site.HutMidX);
            writer.Write(site.HutFloorY);
            writer.Write(site.HutDoorX);
        }
        writer.Write(Finished);
        writer.Write(BuiltVersion);
    }

    public override void NetReceive(BinaryReader reader)
    {
        Site = reader.ReadBoolean()
            ? new BridgeSite(reader.ReadInt32(), reader.ReadSByte(), reader.ReadInt32(),
                reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32())
            : null;
        Finished = reader.ReadBoolean();
        BuiltVersion = reader.ReadInt32();
    }

    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        int index = tasks.FindIndex(pass => pass.Name == "Final Cleanup");
        tasks.Insert(index >= 0 ? index : tasks.Count, new PassLegacy("Shinobi: Wave Country Bridge", Generate));
    }

    private static void Generate(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "修建波之国大桥";
        if (BridgeBuilder.TryFindWorldSite(out BridgeSite site))
        {
            Site = BridgeBuilder.Build(site, finished: false, sync: false);
            BuiltVersion = BridgeDesign.Version;
        }
    }

    // Blueprint builds run on the server (or in single player); already-won worlds get the finished bridge.
    public static bool TryBuildFromBlueprint(BridgeSite site, out string reason)
    {
        if (Site.HasValue)
        {
            reason = "这个世界已经有大桥了。";
            return false;
        }
        if (!BridgeBuilder.CheckObstacles(site, out reason))
            return false;

        bool won = StoryWorld.WaveComplete;
        Site = BridgeBuilder.Build(site, finished: won, sync: true);
        BuiltVersion = BridgeDesign.Version;
        Finished = won;
        SyncWorld();
        return true;
    }

    public override void PostUpdateWorld()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || Site is not BridgeSite site)
            return;

        if (!Finished && StoryWorld.WaveComplete)
            FinishBridge(site);
        KeepTazuna(site);
    }

    private static void FinishBridge(BridgeSite site)
    {
        BridgeBuilder.Finish(site, sync: true);
        Finished = true;
        SyncWorld();
        Announce("大桥完工了。海上的雾，散去了。", new Color(150, 220, 255));
    }

    // Before the win, the builder stands in his hut (he is not saved with the world, so he is re-placed when
    // missing). After it, he moves into the finished hut once as a town NPC; vanilla housing takes over from there.
    private static void KeepTazuna(BridgeSite site)
    {
        if (site.HutFloorY <= 0)
            return;
        int bridgeType = ModContent.NPCType<TazunaBridge>();
        if (!Finished)
        {
            if (!NPC.AnyNPCs(bridgeType))
                SpawnAtHut(site, bridgeType);
            return;
        }

        foreach (NPC npc in Main.ActiveNPCs)
            if (npc.type == bridgeType)
            {
                npc.active = false;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
            }

        if (tazunaMovedIn)
            return;
        tazunaMovedIn = true;
        int townType = ModContent.NPCType<Tazuna>();
        if (NPC.AnyNPCs(townType))
            return;
        int index = SpawnAtHut(site, townType);
        if (index < 0)
            return;
        NPC tazuna = Main.npc[index];
        tazuna.homeless = false;
        tazuna.homeTileX = site.HutMidX;
        tazuna.homeTileY = site.HutFloorY;
        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.SyncNPC, number: index);
    }

    private static int SpawnAtHut(BridgeSite site, int type)
    {
        int index = NPC.NewNPC(new EntitySource_WorldEvent(), site.HutMidX * 16 + 8, site.HutFloorY * 16, type);
        return index < Main.maxNPCs ? index : -1;
    }

    // Distance in tiles from a world position to the bridge (0 anywhere from the shoreline out to the island).
    public static float DistanceToBridgeTiles(Vector2 worldPosition)
    {
        if (Site is not BridgeSite site)
            return float.MaxValue;
        float tileX = worldPosition.X / 16f;
        float tileY = worldPosition.Y / 16f;
        float a = site.X(0);
        float b = site.X(BridgeDesign.IslandEnd);
        float dx = Math.Max(0f, Math.Max(Math.Min(a, b) - tileX, tileX - Math.Max(a, b)));
        float dy = Math.Max(0f, Math.Abs(tileY - site.DeckY) - 20f);
        return MathF.Sqrt(dx * dx + dy * dy);
    }

    private static void SyncWorld()
    {
        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.WorldData);
    }

    private static void Announce(string text, Color color)
    {
        if (Main.netMode == NetmodeID.Server)
            ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(text), color);
        else
            Main.NewText(text, color);
    }
}
