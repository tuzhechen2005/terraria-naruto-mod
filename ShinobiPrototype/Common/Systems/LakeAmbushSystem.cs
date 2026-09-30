using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.NPCs;

namespace ShinobiPrototype.Common.Systems;

// The lake ambush (M1 spec, "湖边初遇"): once Tazuna has confessed and a player is strong enough, Zabuza traps
// Kakashi in a water prison at the next surface lake that player walks up to. The server (or single player) finds
// lakes and starts the scene; the scene itself is the WaterPrison NPC.
public sealed class LakeAmbushSystem : ModSystem
{
    public readonly record struct Lake(int CenterX, int SurfaceY, int Width);

    private static readonly List<Lake> lakes = new();
    private static bool scanned;
    private static int retryCooldown;

    public static IReadOnlyList<Lake> Lakes => lakes;

    public static void Retreated() => retryCooldown = StoryRules.RetryCooldownTicks;

    public override void OnWorldLoad() => Reset();

    public override void OnWorldUnload() => Reset();

    private static void Reset()
    {
        lakes.Clear();
        scanned = false;
        retryCooldown = 0;
    }

    public override void PostUpdateWorld()
    {
        if (!scanned)
        {
            scanned = true;
            ScanLakes();
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }
        if (retryCooldown > 0)
            retryCooldown--;
        if (Main.GameUpdateCount % 30 != 0 || retryCooldown > 0 || StoryWorld.WaveComplete || StoryWorld.LakeDone ||
            !StoryWorld.TazunaConfessed || NPC.AnyNPCs(ModContent.NPCType<WaterPrison>()))
            return;

        foreach (Player player in Main.ActivePlayers)
        {
            if (player.dead || !player.GetModPlayer<StoryPlayer>().ReadyForLake)
                continue;
            if (FindLakeNear(player) is Lake lake)
            {
                Start(lake, player);
                return;
            }
        }
    }

    public static void Start(Lake lake, Player player)
    {
        int dir = player.Center.X < lake.CenterX * 16f + 8f ? 1 : -1;
        // The sphere floats over the middle of the lake, its bottom just above the water.
        Vector2 center = new(lake.CenterX * 16f + 8f, lake.SurfaceY * 16f - WaterPrison.Radius - 10f);
        NPC.NewNPC(new Terraria.DataStructures.EntitySource_WorldEvent(), (int)center.X, (int)center.Y + WaterPrison.Radius,
            ModContent.NPCType<WaterPrison>(), 0, 0f, 0f, dir, lake.SurfaceY * 16f);
    }

    // A lake the player is standing by: surface water in the columns around them, wide and deep enough, not the sea.
    public static Lake? FindLakeNear(Player player)
    {
        int px = (int)(player.Center.X / 16f);
        int py = (int)(player.Bottom.Y / 16f);
        if (py > Main.worldSurface)
            return null;
        int reach = (int)StoryRules.LakeTriggerTiles;
        for (int d = 0; d <= reach; d++)
        {
            foreach (int x in d == 0 ? new[] { px } : new[] { px - d, px + d })
            {
                if (!WorldGen.InWorld(x, py, 10))
                    continue;
                int? surface = WaterSurface(x, py - 15, py + 25);
                if (surface is int y && Measure(x, y) is Lake lake)
                    return lake;
            }
        }
        return null;
    }

    // The first row from the top of this range that is liquid rather than air, if that liquid is water.
    private static int? WaterSurface(int x, int from, int to)
    {
        for (int y = Math.Max(from, 10); y < Math.Min(to, Main.maxTilesY - 10); y++)
        {
            Tile tile = Main.tile[x, y];
            if (tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
                return null;
            if (tile.LiquidAmount > 0)
                return tile.LiquidType == LiquidID.Water ? y : null;
        }
        return null;
    }

    private static bool IsWater(int x, int y)
    {
        if (!WorldGen.InWorld(x, y, 2))
            return false;
        Tile tile = Main.tile[x, y];
        return tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Water &&
               !(tile.HasTile && Main.tileSolid[tile.TileType]);
    }

    // Follows the water along its surface row (or the one below, for slopes) to find the lake's width and depth.
    private static Lake? Measure(int x, int surfaceY)
    {
        int row = surfaceY + 1;
        if (!IsWater(x, row))
            return null;
        int left = x, right = x;
        while (left - x > -300 && IsWater(left - 1, row))
            left--;
        while (right - x < 300 && IsWater(right + 1, row))
            right++;
        int width = right - left + 1;
        int center = (left + right) / 2;
        int depth = 0;
        while (depth < 40 && IsWater(center, surfaceY + depth))
            depth++;
        return StoryRules.IsLake(center, Main.maxTilesX, width, depth) ? new Lake(center, surfaceY, width) : null;
    }

    // Every surface lake in the world, for the objective's "nearest lake" hint.
    private static void ScanLakes()
    {
        lakes.Clear();
        int top = (int)(Main.worldSurface * 0.3);
        int bottom = (int)Main.worldSurface;
        int lastRight = -1;
        for (int x = StoryRules.OceanMarginTiles; x < Main.maxTilesX - StoryRules.OceanMarginTiles; x++)
        {
            if (x <= lastRight)
                continue;
            if (WaterSurface(x, top, bottom) is int y && Measure(x, y) is Lake lake)
            {
                lakes.Add(lake);
                lastRight = lake.CenterX + lake.Width / 2 + 1;
            }
        }
    }

    public static string NearestLakeHint(Player player)
    {
        int px = (int)(player.Center.X / 16f);
        Lake? nearest = null;
        foreach (Lake lake in lakes)
            if (nearest is not Lake best || Math.Abs(lake.CenterX - px) < Math.Abs(best.CenterX - px))
                nearest = lake;
        if (nearest is not Lake found)
            return "去地表找一片湖（不是海）。";
        int dx = found.CenterX - px;
        return Math.Abs(dx) < 30
            ? "你身边就有一片湖。"
            : $"离你最近的湖在{StoryRules.Direction(dx)}边。";
    }

    // The nearest lake's middle, for the quest tracker.
    public static Microsoft.Xna.Framework.Vector2? NearestLake(Player player)
    {
        int px = (int)(player.Center.X / 16f);
        Lake? nearest = null;
        foreach (Lake lake in lakes)
            if (nearest is not Lake best || Math.Abs(lake.CenterX - px) < Math.Abs(best.CenterX - px))
                nearest = lake;
        return nearest is Lake found ? new Microsoft.Xna.Framework.Vector2(found.CenterX * 16f, found.SurfaceY * 16f) : null;
    }

    // Mist over the lake while the scene plays, for the local player.
    public static float MistBoost()
    {
        int prison = NPC.FindFirstNPC(ModContent.NPCType<WaterPrison>());
        if (prison < 0 || Main.LocalPlayer is not { active: true } player)
            return 0f;
        float tiles = player.Distance(Main.npc[prison].Center) / 16f;
        return tiles < 60f ? 0.45f : tiles < 100f ? 0.45f * (100f - tiles) / 40f : 0f;
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write((short)Math.Min(lakes.Count, 64));
        for (int i = 0; i < Math.Min(lakes.Count, 64); i++)
        {
            writer.Write((short)lakes[i].CenterX);
            writer.Write((short)lakes[i].SurfaceY);
            writer.Write((short)lakes[i].Width);
        }
    }

    public override void NetReceive(BinaryReader reader)
    {
        lakes.Clear();
        int count = reader.ReadInt16();
        for (int i = 0; i < count; i++)
            lakes.Add(new Lake(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16()));
    }
}

// The entrance track while the prison is up (vanilla boss music without the local soundtrack).
public sealed class LakeAmbushMusic : ModSceneEffect
{
    public override int Music => WaveMusic.OrBossMusic(WaveMusic.ZabuzaEntrance);
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

    public override bool IsSceneEffectActive(Player player)
    {
        int prison = NPC.FindFirstNPC(ModContent.NPCType<WaterPrison>());
        return prison >= 0 && player.Distance(Main.npc[prison].Center) < StoryRules.AbortRangeTiles * 16f;
    }
}
