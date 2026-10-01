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

// The Forest of Death (specs/M2_中忍考试篇.spec.md 3.2), run by the server (or single player) from each player's
// synced progress:
// - candidate squads: three candidates who come together for someone hunting on the jungle surface who still lacks a
//   scroll, one squad at a time (user, 2026-10-01: they used to spawn one by one like any monster);
// - the Rain genin's ambush: once a character still short of a scroll has beaten a squad and nears the central tower,
//   a warning, then three Rain genin close in. If the character escapes or falls, they leave and try again later.
// Everyone is placed on open ground (GroundSpot), never inside the hillside.
public sealed class DeathForestSystem : ModSystem
{
    private static int cooldown;
    private static int warnTicks = -1;
    private static int target = -1;
    private static int nextSquad = 1;
    private static readonly int[] squadWait = new int[Main.maxPlayers];

    public override void OnWorldLoad() => Reset();

    public override void OnWorldUnload() => Reset();

    private static void Reset()
    {
        cooldown = 0;
        warnTicks = -1;
        target = -1;
        nextSquad = 1;
        System.Array.Clear(squadWait);
        ForestExamCandidate.ForgetSquads();
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
        UpdateSquads();
        if (cooldown > 0)
            return;

        bool rainAlive = NPC.AnyNPCs(ModContent.NPCType<RainGenin>());
        foreach (Player player in Main.ActivePlayers)
        {
            if (player.dead)
                continue;
            ChuninExamPlayer exam = player.GetModPlayer<ChuninExamPlayer>();
            float fromTower = ExamSiteWorld.Tower is ExamSite tower ? tower.DistanceTiles(player.Center) : float.MaxValue;
            if (!ChuninExamRules.RainAmbushDue(exam.Stage, exam.RainAmbushDone, exam.SquadsBeaten,
                    player.ZoneJungle && player.ZoneOverworldHeight, fromTower, rainAlive, exam.HasBothScrolls))
                continue;
            target = player.whoAmI;
            warnTicks = ChuninExamRules.RainAmbushWarnTicks;
            cooldown = ChuninExamRules.RainAmbushRetryTicks;
            Tell(player, "林间传来细碎的雨声……头顶的树枝上，有三把伞。");
            return;
        }
    }

    // One on each side, one dropping from the trees.
    private static void Spawn()
    {
        Player player = target >= 0 ? Main.player[target] : null;
        target = -1;
        if (player is not { active: true, dead: false })
        {
            cooldown = 0;
            return;
        }
        int type = ModContent.NPCType<RainGenin>();
        int squad = nextSquad++;
        SpawnMember(type, player.Bottom + new Vector2(-22 * 16f, 0f), squad, 0);
        SpawnMember(type, player.Bottom + new Vector2(22 * 16f, 0f), squad, 0);
        // From the branches above: dropped in mid-air it falls to the ground; inside the hillside, it is moved out.
        int third = NPC.NewNPC(new EntitySource_WorldEvent(), (int)player.Center.X + 6 * 16, (int)player.Bottom.Y - 14 * 16,
            type, 0, 0f, 0f, 0f, squad);
        if (third < Main.maxNPCs && Collision.SolidCollision(Main.npc[third].position, Main.npc[third].width, Main.npc[third].height))
            SpawnMember(type, player.Bottom + new Vector2(6 * 16f, 0f), squad, 0, Main.npc[third]);
        Tell(player, "雨隐的考生：“你的卷轴，我们收下了。”");
    }

    // Squads: each player still short of a scroll on the jungle surface waits a while, then one squad comes from just
    // off screen. Waiting starts over while a squad is about.
    private static void UpdateSquads()
    {
        int type = ModContent.NPCType<ForestCanopyCandidate>();
        foreach (Player player in Main.ActivePlayers)
        {
            ChuninExamPlayer exam = player.GetModPlayer<ChuninExamPlayer>();
            bool onSurface = player.ZoneJungle && player.ZoneOverworldHeight && !player.dead;
            bool near = false;
            foreach (NPC npc in Main.ActiveNPCs)
                if (npc.type == type && npc.Distance(player.Center) < ChuninExamRules.SquadNearTiles * 16f)
                    near = true;
            if (near)
                squadWait[player.whoAmI] = 0;
            else if (onSurface)
                squadWait[player.whoAmI] += 30;
            if (!ChuninExamRules.SquadDue(exam.Stage, onSurface, exam.HasBothScrolls, near, squadWait[player.whoAmI],
                    exam.SquadsBeaten))
                continue;
            squadWait[player.whoAmI] = 0;
            SendSquad(player, type);
        }
    }

    internal static void SendSquad(Player player, int type)
    {
        int side = Main.rand.NextBool() ? 1 : -1;
        Vector2 at = player.Bottom + new Vector2(side * ChuninExamRules.SquadSpawnTiles * 16f, 0f);
        if (!GroundSpot.TryNear(at, 28, 56, out Vector2 leader, 14, 20, 24))
            return;
        int squad = nextSquad++;
        int name = Main.rand.Next(ForestExamCandidate.SquadNames.Length);
        for (int i = 0; i < ChuninExamRules.SquadSize; i++)
            SpawnMember(ModContent.NPCType<ForestCanopyCandidate>(), leader + new Vector2(side * i * 30f, 0f), squad, name);
        Tell(player, $"树丛里有动静……{ForestExamCandidate.SquadNames[name]}盯上了你的卷轴。");
    }

    // A member on open ground near `at` (members of one squad stand side by side); moves `existing` there if given.
    private static void SpawnMember(int type, Vector2 at, int squad, int name, NPC existing = null)
    {
        if (!GroundSpot.TryNear(at, 28, 56, out Vector2 bottom, 10, 16, 20))
            bottom = at;
        if (existing != null)
        {
            existing.Bottom = bottom;
            existing.netUpdate = true;
            return;
        }
        NPC.NewNPC(new EntitySource_WorldEvent(), (int)bottom.X, (int)bottom.Y, type, 0, name, 0f, 0f, squad);
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
