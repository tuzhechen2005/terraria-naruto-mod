using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Common.Systems;

// Starts the exam bouts where they are held (specs/M2_中忍考试篇.spec.md): the prelims against Dosu when a character
// due for them stands in the tower hall, the finals against Gaara on the stadium field, and Orochimaru's encounter the
// first time a character strong enough goes hunting a scroll on the jungle surface (rarely again after that). Run by
// the server (or single player) from each player's synced exam progress.
public sealed class ExamBoutSystem : ModSystem
{
    // Five seconds between the announcement and the opponent (user, 2026-09-30).
    private const int AnnounceTicks = 300;
    private const int RetryTicks = 60 * 20;

    private static int cooldown;
    private static int announce = -1;
    private static int pendingType;
    private static Vector2 pendingAt;
    private static int pendingNear = -1;

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
            {
                // Someone who walks up to the player comes from wherever the player is by then.
                if (pendingNear >= 0 && Main.player[pendingNear] is { active: true, dead: false } near)
                    pendingAt = new Vector2(near.Center.X + (Main.rand.NextBool() ? -1 : 1) * 30 * 16, near.Bottom.Y);
                NPC.NewNPC(new Terraria.DataStructures.EntitySource_WorldEvent(), (int)pendingAt.X, (int)pendingAt.Y, pendingType);
                pendingNear = -1;
            }
            return;
        }
        if (Main.GameUpdateCount % 30 != 0 || cooldown > 0 || AnyBout())
            return;

        foreach (Player player in Main.ActivePlayers)
        {
            if (player.dead)
                continue;
            ExamStage stage = player.GetModPlayer<ChuninExamPlayer>().Stage;
            if (stage == ExamStage.Prelims && ExamSiteWorld.Tower is ExamSite tower && ExamSiteWorld.InArena(tower, player.Center))
            {
                Rectangle hall = tower.ArenaWorld();
                Schedule(ModContent.NPCType<Dosu>(), new Vector2(FarSide(hall, player), hall.Bottom),
                    $"监考官：“预选赛开始。{player.name} 对 音忍·多斯。——开始！”");
                return;
            }
            if (stage == ExamStage.Finals && ExamSiteWorld.Stadium is ExamSite stadium && ExamSiteWorld.InArena(stadium, player.Center))
            {
                Rectangle field = stadium.ArenaWorld();
                Schedule(ModContent.NPCType<Gaara>(), new Vector2(FarSide(field, player), field.Bottom),
                    $"主考官：“中忍考试正式赛，第一场——{player.name} 对 砂隐的我爱罗！”");
                return;
            }
            bool forest = stage == ExamStage.ForestHunt && player.ZoneJungle && player.ZoneOverworldHeight;
            if (forest && ChuninExamRules.ReadyForOrochimaru(NPC.downedBoss2, player.statLifeMax) &&
                (!StoryWorld.OrochimaruMet || Main.rand.NextFloat() < ExamBossRules.ForestRematchChancePerCheck))
            {
                // He walks up as a Grass candidate.
                pendingNear = player.whoAmI;
                Schedule(ModContent.NPCType<Orochimaru>(), player.Bottom,
                    "林子里忽然安静了下来……一个草隐的考生从树后走出来，笑得让人发毛。");
                cooldown = RetryTicks * 3;
                return;
            }
        }
    }

    private static bool AnyBout() =>
        NPC.AnyNPCs(ModContent.NPCType<Dosu>()) || NPC.AnyNPCs(ModContent.NPCType<Gaara>()) ||
        NPC.AnyNPCs(ModContent.NPCType<Orochimaru>()) || NPC.AnyNPCs(ModContent.NPCType<Neji>());

    // The opponent comes in from the far end of the arena.
    private static float FarSide(Rectangle arena, Player player) =>
        player.Center.X < arena.Center.X ? arena.Right - 64 : arena.Left + 64;

    private static void Schedule(int type, Vector2 at, string line)
    {
        pendingType = type;
        pendingAt = at;
        announce = AnnounceTicks;
        cooldown = RetryTicks;
        ExamBoss.Tell(line, new Color(255, 220, 120));
    }
}
