using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Common.Systems;

// Starts the exam bouts where they are held (specs/M2_中忍考试篇.spec.md): the prelims against Dosu when a character
// due for them asks Gekkō Hayate in the tower hall (user, 2026-10-01: walking in used to start it at once), and the
// finals against Gaara on the stadium field. Orochimaru no longer comes on his own: only his shed skin calls him, until
// his part in the second test is settled. Run by the server (or single player) from each player's synced progress.
public sealed class ExamBoutSystem : ModSystem
{
    // Five seconds between the announcement and the opponent (user, 2026-09-30).
    private const int AnnounceTicks = 300;
    private const int PrelimsAnnounceTicks = 120;
    private const int RetryTicks = 60 * 20;

    private static int cooldown;
    private static int announce = -1;
    private static int pendingType;
    private static Vector2 pendingAt;

    public override void OnWorldLoad() => Reset();

    public override void OnWorldUnload() => Reset();

    private static void Reset()
    {
        cooldown = 0;
        announce = -1;
        pendingType = 0;
    }

    public override void PostUpdateWorld()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;
        if (cooldown > 0)
            cooldown--;
        if (announce >= 0)
        {
            if (--announce < 0 && !NPC.AnyNPCs(pendingType))
                NPC.NewNPC(new Terraria.DataStructures.EntitySource_WorldEvent(), (int)pendingAt.X, (int)pendingAt.Y, pendingType);
            return;
        }
        if (Main.GameUpdateCount % 30 != 0 || cooldown > 0 || AnyBout())
            return;

        foreach (Player player in Main.ActivePlayers)
        {
            if (player.dead)
                continue;
            ExamStage stage = player.GetModPlayer<ChuninExamPlayer>().Stage;
            if (stage == ExamStage.Finals && ExamSiteWorld.Stadium is ExamSite stadium && ExamSiteWorld.InArena(stadium, player.Center))
            {
                Rectangle field = stadium.ArenaWorld();
                Schedule(ModContent.NPCType<Gaara>(), new Vector2(FarSide(field, player), field.Bottom),
                    $"主考官：“中忍考试正式赛，第一场——{player.name} 对 砂隐的我爱罗！”");
                return;
            }
        }
    }

    // Hayate's button: the client asks, the server (or single player) starts the bout.
    public static void RequestPrelims(Player player)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            ModPacket packet = ModContent.GetInstance<ShinobiPrototype>().GetPacket();
            packet.Write((byte)ShinobiPrototype.Packet.StartPrelims);
            packet.Write((byte)player.whoAmI);
            packet.Send();
            return;
        }
        StartPrelims(player.whoAmI);
    }

    internal static void StartPrelims(int who)
    {
        Player player = Main.player[who];
        if (!player.active || player.dead || announce >= 0 || AnyBout() || ExamSiteWorld.Tower is not ExamSite tower ||
            player.GetModPlayer<ChuninExamPlayer>().Stage != ExamStage.Prelims)
            return;
        Rectangle hall = tower.ArenaWorld();
        Schedule(ModContent.NPCType<Dosu>(), new Vector2(FarSide(hall, player), hall.Bottom),
            $"月光疾风：“（咳）……预选赛，{player.name} 对 音忍·多斯。——开始。”", PrelimsAnnounceTicks);
    }

    public static bool BoutUnderway => announce >= 0 || AnyBout();

    private static bool AnyBout() =>
        NPC.AnyNPCs(ModContent.NPCType<Dosu>()) || NPC.AnyNPCs(ModContent.NPCType<Gaara>()) ||
        NPC.AnyNPCs(ModContent.NPCType<Orochimaru>()) || NPC.AnyNPCs(ModContent.NPCType<Neji>());

    // The opponent comes in from the far end of the arena.
    private static float FarSide(Rectangle arena, Player player) =>
        player.Center.X < arena.Center.X ? arena.Right - 64 : arena.Left + 64;

    private static void Schedule(int type, Vector2 at, string line, int ticks = AnnounceTicks)
    {
        pendingType = type;
        pendingAt = at;
        announce = ticks;
        cooldown = RetryTicks;
        ExamBoss.Tell(line, new Color(255, 220, 120));
    }
}
