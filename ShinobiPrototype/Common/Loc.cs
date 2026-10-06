using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;

namespace ShinobiPrototype.Common;

// Text shown to players, from the localization files (Localization/*.hjson, under Mods.ShinobiPrototype.Text), so the
// game's language picks Chinese or English. Arguments fill {0}, {1}... in the text. A key starting "Mods." is used as
// it is (the older Mods.ShinobiPrototype.Dialogue lines).
public static class Loc
{
    public const string Prefix = "Mods.ShinobiPrototype.Text.";

    private static string Full(string key) => key.StartsWith("Mods.") ? key : Prefix + key;

    public static string Get(string key, params object[] args) => Language.GetTextValue(Full(key), args);

    public static LocalizedText Text(string key) => Language.GetText(Full(key));

    // One of a numbered set at random: Pick("Tazuna.Line", 9) reads Tazuna.Line1..Tazuna.Line9.
    public static string Pick(string prefix, int count) => Get(prefix + Main.rand.Next(1, count + 1));

    // A chat line for everyone. A server sends the key, so each player reads it in their own language; in single
    // player it goes to the chat; a multiplayer client shows nothing (the server speaks for it).
    public static void Broadcast(Color color, string key, params object[] args)
    {
        if (Main.netMode == NetmodeID.Server)
            ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Full(key), args), color);
        else if (Main.netMode == NetmodeID.SinglePlayer)
            Main.NewText(Get(key, args), color);
    }

    // A chat line for one player: from a server, sent to them; otherwise shown here.
    public static void SendTo(Player player, Color color, string key, params object[] args)
    {
        if (Main.netMode == NetmodeID.Server)
            ChatHelper.SendChatMessageToClient(NetworkText.FromKey(Full(key), args), color, player.whoAmI);
        else
            Main.NewText(Get(key, args), color);
    }
}
