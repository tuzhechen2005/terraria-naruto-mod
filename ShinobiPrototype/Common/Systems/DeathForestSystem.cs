using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Common.Systems;

// The Forest of Death (specs/M2_中忍考试篇.spec.md 3.2; redesigned with the user 2026-10-01), run by the server (or
// single player) from each player's synced progress. Fixed encounters on the way from the gate to the tower:
// - past the gate, a squad lies in wait (carrying the same scroll as the player);
// - halfway, Orochimaru in the guise of a lone Grass candidate (after the squad at the gate);
// - in the clearing before the tower, the Rain genin (carrying the missing one; after Orochimaru).
// Each waits at its spot and tries again if the player runs or falls. Elsewhere in the forest, squads roam as an
// optional fight (no scroll, no announcement) once the first encounter is behind the player. Everyone is placed on
// open ground (GroundSpot).
public sealed class DeathForestSystem : ModSystem
{
    private const int MemberWidth = 28, MemberHeight = 56;

    private static int gateCooldown;
    private static int orochimaruCooldown;
    private static int orochimaruWarn = -1;
    private static int orochimaruTarget = -1;
    private static int rainCooldown;
    private static int rainWarn = -1;
    private static int rainTarget = -1;
    private static int nextSquad = 1;
    private static readonly int[] squadWait = new int[Main.maxPlayers];

    public override void OnWorldLoad() => Reset();

    public override void OnWorldUnload() => Reset();

    private static void Reset()
    {
        gateCooldown = rainCooldown = orochimaruCooldown = 0;
        orochimaruWarn = orochimaruTarget = -1;
        rainWarn = rainTarget = -1;
        nextSquad = 1;
        System.Array.Clear(squadWait);
        ForestExamCandidate.ForgetSquads();
    }

    public override void PostUpdateWorld()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;
        if (gateCooldown > 0)
            gateCooldown--;
        if (rainCooldown > 0)
            rainCooldown--;
        if (orochimaruCooldown > 0)
            orochimaruCooldown--;
        if (orochimaruWarn >= 0 && --orochimaruWarn < 0)
            SendDisguise();
        if (rainWarn >= 0 && --rainWarn < 0)
            SpawnRain();
        if (Main.GameUpdateCount % 30 != 0)
            return;

        int candidate = ModContent.NPCType<ForestCanopyCandidate>();
        int rain = ModContent.NPCType<RainGenin>();
        foreach (Player player in Main.ActivePlayers)
        {
            if (player.dead)
                continue;
            ChuninExamPlayer exam = player.GetModPlayer<ChuninExamPlayer>();
            if (gateCooldown <= 0 && ExamSiteWorld.GateAmbushSpot is Vector2 spot &&
                ChuninExamRules.GateAmbushDue(exam.Stage, exam.GateSquadDone, Vector2.Distance(player.Bottom, spot) / 16f,
                    AnyOfKind(candidate, ForestExamCandidate.KindGate)))
            {
                gateCooldown = ChuninExamRules.GateAmbushRetryTicks;
                SpawnGateSquad(player);
            }
            bool orochimaruAbout = NPC.AnyNPCs(ModContent.NPCType<OrochimaruDisguise>()) || NPC.AnyNPCs(ModContent.NPCType<Orochimaru>());
            if (orochimaruCooldown <= 0 && orochimaruWarn < 0 && ExamSiteWorld.OrochimaruX is float midway &&
                ChuninExamRules.OrochimaruDue(exam.Stage, exam.GateSquadDone, exam.OrochimaruDone,
                    System.Math.Abs(player.Center.X - midway) / 16f, orochimaruAbout))
            {
                orochimaruCooldown = ChuninExamRules.OrochimaruRetryTicks;
                orochimaruWarn = ChuninExamRules.OrochimaruWarnTicks;
                orochimaruTarget = player.whoAmI;
                Tell(player, "林子里忽然没了虫鸣……");
            }
            // The Rain genin wait until Orochimaru has been met.
            if (exam.OrochimaruDone && rainCooldown <= 0 && rainWarn < 0 && ExamSiteWorld.RainClearing is Vector2 clearing &&
                ChuninExamRules.RainAmbushDue(exam.Stage, exam.RainAmbushDone, Vector2.Distance(player.Bottom, clearing) / 16f,
                    NPC.AnyNPCs(rain), exam.HasBothScrolls))
            {
                rainCooldown = ChuninExamRules.RainAmbushRetryTicks;
                rainWarn = ChuninExamRules.RainAmbushWarnTicks;
                rainTarget = player.whoAmI;
                Tell(player, "林间下起了细雨……头顶的树枝上，撑开了三把伞。");
            }
            UpdateRoaming(player, exam, candidate);
        }
    }

    private static bool AnyOfKind(int type, int kind)
    {
        foreach (NPC npc in Main.ActiveNPCs)
            if (npc.type == type && (int)npc.ai[0] / 10 == kind)
                return true;
        return false;
    }

    // Two ahead on the path, one dropping from the branches behind.
    private static void SpawnGateSquad(Player player)
    {
        int dir = ExamSiteWorld.Gate is ExamSite gate ? gate.Dir : 1;
        int type = ModContent.NPCType<ForestCanopyCandidate>();
        int squad = nextSquad++;
        int code = ForestExamCandidate.KindGate * 10 + Main.rand.Next(ForestExamCandidate.SquadNames.Length);
        SpawnMember(type, player.Bottom + new Vector2(dir * 18 * 16f, 0f), squad, code);
        SpawnMember(type, player.Bottom + new Vector2(dir * 21 * 16f, 0f), squad, code);
        SpawnMember(type, player.Bottom + new Vector2(-dir * 10 * 16f, 0f), squad, code);
        Tell(player, "……树上有人。");
    }

    // A lone Grass candidate walks out of the trees ahead, towards the tower.
    private static void SendDisguise()
    {
        Player player = orochimaruTarget >= 0 ? Main.player[orochimaruTarget] : null;
        orochimaruTarget = -1;
        if (player is not { active: true, dead: false })
        {
            orochimaruCooldown = 0;
            return;
        }
        int dir = ExamSiteWorld.Gate is ExamSite gate ? gate.Dir : 1;
        if (!GroundSpot.TryNear(player.Bottom + new Vector2(dir * 34 * 16f, 0f), MemberWidth, MemberHeight, out Vector2 at, 12, 16, 20))
            at = player.Bottom + new Vector2(dir * 34 * 16f, 0f);
        NPC.NewNPC(new EntitySource_WorldEvent(), (int)at.X, (int)at.Y, ModContent.NPCType<OrochimaruDisguise>());
    }

    // One on each side of the clearing, one dropping from the trees above the player.
    private static void SpawnRain()
    {
        Player player = rainTarget >= 0 ? Main.player[rainTarget] : null;
        rainTarget = -1;
        if (player is not { active: true, dead: false } || ExamSiteWorld.RainClearing is not Vector2 clearing)
        {
            rainCooldown = 0;
            return;
        }
        int type = ModContent.NPCType<RainGenin>();
        int squad = nextSquad++;
        int code = ForestExamCandidate.KindRain * 10;
        SpawnMember(type, clearing + new Vector2(-14 * 16f, 0f), squad, code);
        SpawnMember(type, clearing + new Vector2(14 * 16f, 0f), squad, code);
        int third = NPC.NewNPC(new EntitySource_WorldEvent(), (int)player.Center.X + 5 * 16, (int)player.Bottom.Y - 14 * 16,
            type, 0, code, 0f, 0f, squad);
        if (third < Main.maxNPCs && Collision.SolidCollision(Main.npc[third].position, MemberWidth, MemberHeight))
            SpawnMember(type, player.Bottom + new Vector2(5 * 16f, 0f), squad, code, Main.npc[third]);
        Tell(player, "雨隐的考生：“你的卷轴，我们收下了。”");
    }

    // Roaming squads, once the first encounter is behind the player: each player still short of a scroll on the
    // jungle surface waits a while, then one squad comes from just off screen. Waiting starts over while one is about.
    private static void UpdateRoaming(Player player, ChuninExamPlayer exam, int type)
    {
        bool onSurface = player.ZoneJungle && player.ZoneOverworldHeight;
        bool near = false;
        foreach (NPC npc in Main.ActiveNPCs)
            if (npc.type == type && npc.Distance(player.Center) < ChuninExamRules.SquadNearTiles * 16f)
                near = true;
        if (near)
            squadWait[player.whoAmI] = 0;
        else if (onSurface && exam.GateSquadDone)
            squadWait[player.whoAmI] += 30;
        if (!exam.GateSquadDone || !ChuninExamRules.SquadDue(exam.Stage, onSurface, exam.HasBothScrolls, near,
                squadWait[player.whoAmI], exam.SquadsBeaten))
            return;
        squadWait[player.whoAmI] = 0;
        SendSquad(player, type);
    }

    internal static void SendSquad(Player player, int type)
    {
        int side = Main.rand.NextBool() ? 1 : -1;
        Vector2 at = player.Bottom + new Vector2(side * ChuninExamRules.SquadSpawnTiles * 16f, 0f);
        if (!GroundSpot.TryNear(at, MemberWidth, MemberHeight, out Vector2 leader, 14, 20, 24))
            return;
        int squad = nextSquad++;
        int code = ForestExamCandidate.KindRoaming * 10 + Main.rand.Next(ForestExamCandidate.SquadNames.Length);
        for (int i = 0; i < ChuninExamRules.SquadSize; i++)
            SpawnMember(type, leader + new Vector2(side * i * 30f, 0f), squad, code);
    }

    // A member on open ground near `at` (members of one squad stand side by side); moves `existing` there if given.
    private static void SpawnMember(int type, Vector2 at, int squad, int code, NPC existing = null)
    {
        if (!GroundSpot.TryNear(at, MemberWidth, MemberHeight, out Vector2 bottom, 10, 16, 20))
            bottom = at;
        if (existing != null)
        {
            existing.Bottom = bottom;
            existing.netUpdate = true;
            return;
        }
        NPC.NewNPC(new EntitySource_WorldEvent(), (int)bottom.X, (int)bottom.Y, type, 0, code, 0f, 0f, squad);
    }

    private static void Tell(Player player, string text)
    {
        Color color = new(170, 190, 255);
        if (Main.netMode == NetmodeID.Server)
            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(text), color, player.whoAmI);
        else
            Main.NewText(text, color);
    }
}
