using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Common.Systems;

// The village's story NPCs are always in their places in a world with the village: the Third Hokage in his office,
// Morino Ibiki in the Academy, Mitarashi Anko at the forest gate and Gekkō Hayate in the tower hall (they cannot be hurt, so this only ever places them once per session). Run by the server
// or single player.
public sealed class HiruzenSpawnSystem : ModSystem
{
    public override void PostUpdateWorld()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || Main.GameUpdateCount % 120 != 0)
            return;
        Place(KonohaWorld.HokageFeet, ModContent.NPCType<Hiruzen>());
        Place(KonohaWorld.IbikiFeet, ModContent.NPCType<Ibiki>());
        Place(ExamSiteWorld.AnkoFeet, ModContent.NPCType<Anko>());
        Place(ExamSiteWorld.HayateFeet, ModContent.NPCType<Hayate>());
    }

    private static void Place(Vector2? feet, int type)
    {
        if (feet is not Vector2 at || NPC.AnyNPCs(type))
            return;
        int index = NPC.NewNPC(new Terraria.DataStructures.EntitySource_WorldEvent(), (int)at.X, (int)at.Y, type);
        if (index < Main.maxNPCs && Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.SyncNPC, number: index);
    }
}
