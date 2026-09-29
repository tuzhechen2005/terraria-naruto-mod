using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Common.Systems;

// Kakashi arrives at the world spawn as soon as a world is played (new or old), and returns there a little
// while after dying, housing or not. He moves into a free house like any town NPC.
public sealed class KakashiSpawnSystem : ModSystem
{
    private const int RespawnDelayTicks = 60 * 60 * 2;

    private static bool arrived;
    private static int absentTicks;

    public override void ClearWorld()
    {
        arrived = false;
        absentTicks = 0;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        if (arrived)
            tag["kakashiArrived"] = true;
    }

    public override void LoadWorldData(TagCompound tag) => arrived = tag.GetBool("kakashiArrived");

    public override void PostUpdateWorld()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;

        int type = ModContent.NPCType<Kakashi>();
        if (NPC.AnyNPCs(type))
        {
            absentTicks = 0;
            arrived = true;
            return;
        }

        if (arrived && ++absentTicks < RespawnDelayTicks)
            return;

        int index = NPC.NewNPC(new Terraria.DataStructures.EntitySource_SpawnNPC(),
            Main.spawnTileX * 16 + 8, Main.spawnTileY * 16, type);
        if (index >= Main.maxNPCs)
            return;

        NPC kakashi = Main.npc[index];
        kakashi.homeless = true;
        kakashi.homeTileX = Main.spawnTileX;
        kakashi.homeTileY = Main.spawnTileY;
        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.SyncNPC, number: index);
        arrived = true;
        absentTicks = 0;
    }
}
