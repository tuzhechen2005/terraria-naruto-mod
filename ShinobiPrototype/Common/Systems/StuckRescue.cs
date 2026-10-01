using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Common.Systems;

// Keeps the mod's walking enemies out of walls and holes (user, 2026-10-01: bosses and grunts alike spawned inside
// terrain or dropped into small holes and could not get out). Two parts, decided by the server (or single player):
// - spawning inside solid tiles, or with nothing to stand on, moves them to the nearest open ground (GroundSpot);
// - in a fight, embedded in tiles, or making no headway for a few seconds while pressed against a wall, out of sight
//   of their target or far away from it, they Body Flicker (瞬身) to open ground beside the target.
// Zabuza has his own Body Flicker; fliers, the water clones on the lake and friendly NPCs are left alone.
public sealed class StuckRescue : GlobalNPC
{
    private const int CheckEvery = 30;
    private const int StuckChecks = 6; // three seconds without headway
    private const float HeadwayPixels = 24f;
    private const float FarTiles = 40f;
    private static readonly int[] BesideTiles = { 9, 7, 11, 13 };

    private int timer;
    private float lastX;
    private int stillChecks;

    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC npc, bool lateInstantiation) =>
        lateInstantiation && npc.ModNPC?.Mod is ShinobiPrototype && !npc.friendly && !npc.townNPC && !npc.noGravity &&
        npc.type != ModContent.NPCType<ZabuzaBoss>() && npc.type != ModContent.NPCType<WaterClone>();

    public override void OnSpawn(NPC npc, IEntitySource source)
    {
        lastX = npc.position.X;
        if (Main.netMode == NetmodeID.MultiplayerClient || npc.wet)
            return;
        bool embedded = Collision.SolidCollision(npc.position, npc.width, npc.height);
        bool nothingBelow = !Collision.SolidCollision(npc.BottomLeft, npc.width, 10 * 16, true);
        if ((embedded || nothingBelow) && GroundSpot.TryNear(npc.Bottom, npc.width, npc.height, out Vector2 bottom))
        {
            npc.Bottom = bottom;
            npc.netUpdate = true;
        }
    }

    public override void PostAI(NPC npc)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || ++timer < CheckEvery)
            return;
        timer = 0;
        if (npc.target < 0 || npc.target >= Main.maxPlayers || Main.player[npc.target] is not { active: true, dead: false } target ||
            npc.dontTakeDamage && npc.alpha >= 200)
            return;

        bool embedded = Collision.SolidCollision(npc.position + new Vector2(2f), npc.width - 4, npc.height - 4);
        bool headway = Math.Abs(npc.position.X - lastX) >= HeadwayPixels;
        lastX = npc.position.X;
        bool sight = Collision.CanHitLine(npc.position, npc.width, npc.height, target.position, target.width, target.height);
        bool far = npc.Distance(target.Center) / 16f > FarTiles;
        stillChecks = !headway && (npc.collideX || !sight || far) ? stillChecks + 1 : 0;
        if (!embedded && stillChecks < StuckChecks)
            return;
        stillChecks = 0;

        if (GroundSpot.TryBeside(target, npc.width, npc.height, BesideTiles, out Vector2 bottom) ||
            embedded && GroundSpot.TryNear(npc.Bottom, npc.width, npc.height, out bottom))
            Flicker(npc, bottom);
    }

    private static void Flicker(NPC npc, Vector2 bottom)
    {
        Smoke(npc);
        npc.Bottom = bottom;
        npc.velocity = Vector2.Zero;
        npc.netUpdate = true;
        Smoke(npc);
    }

    private static void Smoke(NPC npc)
    {
        if (Main.dedServ)
            return;
        for (int i = 0; i < 18; i++)
            Dust.NewDust(npc.position, npc.width, npc.height, DustID.Smoke, Main.rand.NextFloat(-2f, 2f),
                Main.rand.NextFloat(-2f, 0.5f), 100, default, 1.5f);
    }
}
