using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Items;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Common.Systems;

// The Demon Brothers' first ambush, which the mission waits on (see DemonBrotherRules.AmbushDue): a warning in chat,
// then the pair closes in from both sides. Run by the server (or single player).
public sealed class DemonBrotherAmbushSystem : ModSystem
{
    private static int cooldown;
    private static int warnTicks = -1;
    private static int target = -1;

    public override void OnWorldLoad() => Reset();

    public override void OnWorldUnload() => Reset();

    private static void Reset()
    {
        cooldown = 0;
        warnTicks = -1;
        target = -1;
    }

    public override void PostUpdateWorld()
    {
        if (cooldown > 0)
            cooldown--;
        if (warnTicks >= 0)
        {
            if (--warnTicks < 0)
                Spawn();
            return;
        }
        if (Main.GameUpdateCount % 30 != 0)
            return;

        bool alive = NPC.AnyNPCs(ModContent.NPCType<DemonBrotherGozu>()) || NPC.AnyNPCs(ModContent.NPCType<DemonBrotherMeizu>());
        foreach (Player player in Main.ActivePlayers)
        {
            if (player.dead)
                continue;
            // After meeting Tazuna, the Mist ninja come for whoever walks the sea mist (the Mist scouts that used to
            // hand out the first insignia are gone, user 2026-10-01).
            bool found = StoryWorld.MetTazuna || player.HasItem(ModContent.ItemType<MistInsignia>());
            bool inMist = WaveBridgeWorld.MistActive &&
                          WaveBridgeWorld.DistanceToBridgeTiles(player.Center) < BridgeRules.FogReachTiles;
            if (!DemonBrotherRules.AmbushDue(StoryWorld.DownedDemonBrothers, found, inMist, player.ZoneOverworldHeight,
                    StoryWorld.WaveComplete, alive, cooldown))
                continue;
            target = player.whoAmI;
            warnTicks = DemonBrotherRules.AmbushWarnTicks;
            cooldown = DemonBrotherRules.AmbushRetryTicks;
            Tell(player, Language.GetTextValue("Mods.ShinobiPrototype.Dialogue.DemonBrothersWarn"));
            return;
        }
    }

    // Gōzu on the player's left; he brings Meizu on the mirrored right side himself.
    private static void Spawn()
    {
        Player player = target >= 0 ? Main.player[target] : null;
        target = -1;
        if (player is not { active: true, dead: false })
        {
            cooldown = 0;
            return;
        }
        NPC.NewNPC(new Terraria.DataStructures.EntitySource_WorldEvent(), (int)player.Center.X - 20 * 16,
            (int)player.Bottom.Y, ModContent.NPCType<DemonBrotherGozu>());
    }

    private static void Tell(Player player, string text)
    {
        Color color = new(255, 190, 90);
        if (Main.netMode == NetmodeID.Server)
            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(text), color, player.whoAmI);
        else
            Main.NewText(text, color);
    }
}
