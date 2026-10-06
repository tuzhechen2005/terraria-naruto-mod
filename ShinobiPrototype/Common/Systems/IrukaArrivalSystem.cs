using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Common.Systems;

// Iruka reports to the mission desk the first time a world is played (new or old), at the foot of the Hokage's
// Residence (the world spawn if the village is missing), like Kakashi (KakashiSpawnSystem); without this he would
// wait for a free house, which an old world may never have. After that he comes and goes like any town NPC.
public sealed class IrukaArrivalSystem : ModSystem
{
    private static bool arrived;

    public override void ClearWorld() => arrived = false;

    public override void SaveWorldData(TagCompound tag)
    {
        if (arrived)
            tag["irukaArrived"] = true;
    }

    public override void LoadWorldData(TagCompound tag) => arrived = tag.GetBool("irukaArrived");

    public override void PostUpdateWorld()
    {
        if (arrived || Main.netMode == NetmodeID.MultiplayerClient)
            return;
        int type = ModContent.NPCType<Iruka>();
        arrived = true;
        if (NPC.AnyNPCs(type))
            return;

        Vector2 at = KonohaWorld.BuildingWhere(KonohaBuildings.HokageTower) ??
                     new Vector2(Main.spawnTileX * 16 + 8, Main.spawnTileY * 16);
        int index = NPC.NewNPC(new Terraria.DataStructures.EntitySource_SpawnNPC(), (int)at.X, (int)at.Y, type);
        if (index >= Main.maxNPCs)
            return;
        NPC iruka = Main.npc[index];
        iruka.homeless = true;
        iruka.homeTileX = (int)(at.X / 16f);
        iruka.homeTileY = (int)(at.Y / 16f);
        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.SyncNPC, number: index);
    }
}
