using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Common.Systems;

// The Third Hokage is always in his office in a world with the village (he cannot be hurt, so this only ever places
// him once per session). Run by the server or single player.
public sealed class HiruzenSpawnSystem : ModSystem
{
    public override void PostUpdateWorld()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || Main.GameUpdateCount % 120 != 0)
            return;
        if (KonohaWorld.HokageFeet is not Vector2 feet || NPC.AnyNPCs(ModContent.NPCType<Hiruzen>()))
            return;
        int index = NPC.NewNPC(new Terraria.DataStructures.EntitySource_WorldEvent(), (int)feet.X, (int)feet.Y,
            ModContent.NPCType<Hiruzen>());
        if (index < Main.maxNPCs && Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.SyncNPC, number: index);
    }
}
