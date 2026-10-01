using Terraria;
using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Systems;

// Closes the NPC chat on the next tick. A chat button handler must not close it itself: vanilla's chat drawing goes on
// reading Main.npc[talkNPC] after OnChatButtonClicked returns, so a talkNPC of -1 there throws IndexOutOfRange (user,
// 2026-10-01: the error when Kakashi flickered the player home).
public sealed class NpcChatCloser : ModSystem
{
    private static bool pending;

    public static void CloseNextTick() => pending = true;

    public override void PostUpdateEverything()
    {
        if (!pending || Main.dedServ)
            return;
        pending = false;
        Main.LocalPlayer.SetTalkNPC(-1);
        Main.npcChatText = "";
    }
}
