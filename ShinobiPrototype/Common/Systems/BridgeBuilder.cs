using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.Items;

namespace ShinobiPrototype.Common.Systems;

// Where a bridge stands. The hut position is decided by the terrain when it is built, so it is stored too.
public readonly record struct BridgeSite(int ShoreX, int Dir, int WaterY, int HutMidX = 0, int HutFloorY = 0, int HutDoorX = 0)
{
    public int DeckY => BridgeRules.DeckY(WaterY);
    public int X(int offset) => ShoreX + Dir * offset;
    public int OffsetOf(int tileX) => (tileX - ShoreX) * Dir;
}

// Builds the Wave Country bridge from BridgeRules' layout using vanilla tiles only.
internal static class BridgeBuilder
{
    private const int DeckTile = TileID.WoodBlock;
    private const int SyncChunk = 30;

    // World generation: the beach on the side without the Dungeon.
    public static bool TryFindWorldSite(out BridgeSite site)
    {
        int dir = Main.dungeonX < Main.maxTilesX / 2 ? 1 : -1;
        int edgeX = dir == 1 ? Main.maxTilesX - 60 : 59;
        for (int x = edgeX; x != Main.maxTilesX / 2; x -= dir)
        {
            if (!TryColumnTop(x, (int)(Main.worldSurface * 0.35), out int _, out bool isSea) || isSea)
                continue;
            return TryMakeSite(x, dir, out site);
        }
        site = default;
        return false;
    }

    // Blueprint: the player stands on a beach facing the sea; the shoreline is the last land column ahead.
    public static bool TryPlanFromPlayer(Player player, out BridgeSite site, out string reason)
    {
        site = default;
        if (!player.ZoneBeach)
        {
            reason = "要站在海滩上、面朝大海使用施工图。";
            return false;
        }

        int dir = player.direction >= 0 ? 1 : -1;
        int startX = (int)(player.Center.X / 16f);
        int fromY = (int)(player.Center.Y / 16f) - 30;
        for (int x = startX; Math.Abs(x - startX) <= 60; x += dir)
        {
            if (!TryColumnTop(x, fromY, out _, out bool isSea) || !isSea)
                continue;
            if (!TryMakeSite(x - dir, dir, out site))
            {
                reason = "这里离世界边缘太近，或者海太浅，放不下大桥。";
                return false;
            }
            reason = "";
            return true;
        }
        reason = "前方 60 格内没有找到海。请面朝大海使用。";
        return false;
    }

    private static bool TryMakeSite(int shoreX, int dir, out BridgeSite site)
    {
        site = default;
        int far = shoreX + dir * BridgeRules.TotalReach;
        if (far < 45 || far > Main.maxTilesX - 45)
            return false;
        int seaX = shoreX + dir * 8;
        if (!TryColumnTop(seaX, (int)(Main.worldSurface * 0.35), out int waterY, out bool isSea) || !isSea)
            return false;
        site = new BridgeSite(shoreX, dir, waterY);
        return true;
    }

    // First tile or liquid from the top of a column; isSea when the liquid is reached first.
    private static bool TryColumnTop(int x, int fromY, out int topY, out bool isSea)
    {
        fromY = Math.Max(10, fromY);
        for (int y = fromY; y < Main.worldSurface + 50 && y < Main.maxTilesY - 10; y++)
        {
            Tile tile = Main.tile[x, y];
            if (tile.LiquidAmount > 0)
            {
                topY = y;
                isSea = true;
                return true;
            }
            if (tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
            {
                topY = y;
                isSea = false;
                return true;
            }
        }
        topY = 0;
        isSea = false;
        return false;
    }

    private static int GroundBelow(int x, int fromY)
    {
        for (int y = fromY; y < Main.maxTilesY - 10; y++)
        {
            Tile tile = Main.tile[x, y];
            if (tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
                return y;
        }
        return Main.maxTilesY - 10;
    }

    // Everything the bridge will overwrite, including the land-side stairs and hut at their widest.
    private static IEnumerable<(int X, int Y)> Footprint(BridgeSite site)
    {
        int deckY = site.DeckY;
        int landReach = BridgeRules.DeckLandOverlap + BridgeRules.MaxStairSteps + BridgeRules.HutGapFromStairs +
                        BridgeRules.HutWidth + 1;
        for (int offset = -landReach; offset <= BridgeRules.IslandEnd + BridgeRules.IslandDockLength; offset++)
        {
            int x = site.X(offset);
            int bottom = offset <= 0 ? deckY + BridgeRules.MaxStairSteps : deckY;
            for (int y = deckY - BridgeRules.Clearance; y <= bottom; y++)
                yield return (x, y);
        }
    }

    // Natural terrain may be cleared; anything a player could have placed blocks the build.
    public static bool CheckObstacles(BridgeSite site, out string reason)
    {
        foreach ((int x, int y) in Footprint(site))
        {
            if (!WorldGen.InWorld(x, y, 10))
            {
                reason = "大桥会超出世界边界。";
                return false;
            }
            Tile tile = Main.tile[x, y];
            if (tile.WallType != WallID.None && Main.wallHouse[tile.WallType] ||
                tile.HasTile && !IsNatural(tile.TileType))
            {
                reason = $"建桥范围内有玩家放置的方块或墙（约在 {x}, {y}），请先清理或换个地方。";
                return false;
            }
        }
        reason = "";
        return true;
    }

    private static bool IsNatural(int type) =>
        Main.tileCut[type] || TileID.Sets.Ore[type] || type is TileID.Dirt or TileID.Stone or TileID.Grass or
            TileID.Sand or TileID.ClayBlock or TileID.Mud or TileID.JungleGrass or TileID.HardenedSand or
            TileID.Sandstone or TileID.Silt or TileID.Slush or TileID.SnowBlock or
            TileID.IceBlock or TileID.Cactus or TileID.PalmTree or TileID.Trees or TileID.Saplings or
            TileID.BeachPiles or TileID.Pots or TileID.SmallPiles or TileID.LargePiles or TileID.LargePiles2 or
            TileID.Sunflower or TileID.DyePlants or TileID.Coral;

    public static BridgeSite Build(BridgeSite site, bool finished, bool sync)
    {
        int deckY = site.DeckY;
        foreach ((int x, int y) in Footprint(site))
        {
            if (y > deckY)
                continue;
            Tile tile = Main.tile[x, y];
            tile.ClearEverything();
        }

        for (int offset = BridgeRules.DeckStart; offset <= BridgeRules.FinishTo; offset++)
            if (BridgeRules.HasDeckTile(offset, finished))
                PlaceDeck(site, offset);
        for (int offset = 1; offset <= BridgeRules.FinishTo; offset++)
            if (BridgeRules.IsPillar(offset, finished))
                PlacePillar(site.X(offset), deckY + 1);
        if (!finished)
            for (int y = deckY + 1; y <= deckY + 4; y++)
                WorldGen.PlaceTile(site.X(BridgeRules.DeckLength - 1), y, TileID.Rope, mute: true);

        BuildIsland(site);
        int stairsEnd = BuildStairs(site);
        site = BuildHut(site, stairsEnd, finished);
        if (finished)
            PlaceBridgeSign(site);

        Refresh(site, sync);
        return site;
    }

    // Completion after the first victory: fill only empty tiles in the jagged end and the gap.
    public static void Finish(BridgeSite site, bool sync)
    {
        int deckY = site.DeckY;
        for (int offset = BridgeRules.FinishFrom; offset <= BridgeRules.FinishTo; offset++)
        {
            if (!Main.tile[site.X(offset), deckY].HasTile)
                PlaceDeck(site, offset);
            if (BridgeRules.IsPillar(offset, true) && !Main.tile[site.X(offset), deckY + 1].HasTile)
                PlacePillar(site.X(offset), deckY + 1);
        }
        PlaceBridgeSign(site);
        FinishHut(site);
        Refresh(site, sync);
    }

    private static void PlaceDeck(BridgeSite site, int offset)
    {
        int x = site.X(offset);
        int deckY = site.DeckY;
        Tile deck = Main.tile[x, deckY];
        deck.ClearEverything();
        WorldGen.PlaceTile(x, deckY, DeckTile, mute: true, forced: true);
        for (int y = deckY - 2; y <= deckY - 1; y++)
            if (Main.tile[x, y].WallType == WallID.None)
                WorldGen.PlaceWall(x, y, WallID.WoodenFence, mute: true);
    }

    // A wooden beam from under the deck down to the sea floor.
    private static void PlacePillar(int x, int fromY)
    {
        for (int y = fromY; y < Main.maxTilesY - 10; y++)
        {
            Tile tile = Main.tile[x, y];
            if (tile.HasTile && Main.tileSolid[tile.TileType])
                break;
            WorldGen.PlaceTile(x, y, TileID.WoodenBeam, mute: true, forced: true);
        }
    }

    private static void BuildIsland(BridgeSite site)
    {
        int deckY = site.DeckY;
        for (int offset = BridgeRules.IslandStart; offset <= BridgeRules.IslandEnd; offset++)
        {
            int x = site.X(offset);
            int top = deckY + BridgeRules.IslandSurfaceBelowDeck(offset);
            FillUp(x, top, GroundBelow(x, top) - 1, TileID.Sand);
        }

        int surface = deckY + 1;
        int signX = site.X(BridgeRules.IslandCenter - 4);
        if (WorldGen.PlaceTile(signX, surface - 1, TileID.Signs, mute: true))
            WriteSign(signX, surface - 1, "波之国");

        int chest = WorldGen.PlaceChest(site.X(BridgeRules.IslandCenter + 3), surface - 1, TileID.Containers,
            notNearOtherChests: false, style: 0);
        if (chest >= 0)
        {
            Item[] items = Main.chest[chest].item;
            items[0].SetDefaults(ItemID.Rope);
            items[0].stack = 30;
            items[1].SetDefaults(ItemID.Torch);
            items[1].stack = 15;
            items[2].SetDefaults(ModContent.ItemType<ChakraPill>());
            items[2].stack = 3;
        }

        for (int offset = BridgeRules.IslandEnd + 1; offset <= BridgeRules.IslandEnd + BridgeRules.IslandDockLength; offset++)
            WorldGen.PlaceTile(site.X(offset), site.WaterY - 1, TileID.Platforms, mute: true, forced: true);
    }

    // Fills a column from the bottom up, so sand always rests on something as it is placed.
    private static void FillUp(int x, int top, int bottom, int type)
    {
        for (int y = bottom; y >= top; y--)
        {
            Tile tile = Main.tile[x, y];
            tile.ClearEverything();
            WorldGen.PlaceTile(x, y, type, mute: true, forced: true);
        }
    }

    // One-tile steps from the start of the deck down to the ground inland; returns the offset of the last step.
    private static int BuildStairs(BridgeSite site)
    {
        int deckY = site.DeckY;
        int offset = BridgeRules.DeckStart;
        for (int step = 1; step <= BridgeRules.MaxStairSteps; step++)
        {
            int x = site.X(BridgeRules.DeckStart - step);
            int stepY = deckY + step;
            int ground = GroundBelow(x, deckY - BridgeRules.Clearance);
            if (ground <= stepY)
                break;
            offset = BridgeRules.DeckStart - step;
            FillUp(x, stepY, ground - 1, DeckTile);
        }
        return offset;
    }

    // A stilt-free hut on the beach just inland of the stairs. Unfinished it has no door and no light,
    // so no town NPC can claim it; Finish adds both.
    private static BridgeSite BuildHut(BridgeSite site, int stairsEnd, bool finished)
    {
        int nearOffset = stairsEnd - BridgeRules.HutGapFromStairs;
        int farOffset = nearOffset - (BridgeRules.HutWidth - 1);
        int midX = site.X((nearOffset + farOffset) / 2);
        int floorY = GroundBelow(midX, site.DeckY - BridgeRules.Clearance) - 1;

        for (int offset = farOffset; offset <= nearOffset; offset++)
        {
            int x = site.X(offset);
            for (int y = floorY - BridgeRules.HutHeight; y <= floorY; y++)
            {
                Tile tile = Main.tile[x, y];
                tile.ClearEverything();
            }
            FillUp(x, floorY + 1, GroundBelow(x, floorY + 1) - 1, TileID.Sand);

            bool edge = offset == farOffset || offset == nearOffset;
            WorldGen.PlaceTile(x, floorY, DeckTile, mute: true, forced: true);
            WorldGen.PlaceTile(x, floorY - (BridgeRules.HutHeight - 1), DeckTile, mute: true, forced: true);
            for (int y = floorY - (BridgeRules.HutHeight - 2); y <= floorY - 1; y++)
            {
                if (!edge)
                    WorldGen.PlaceWall(x, y, WallID.Wood, mute: true);
                // The sea-side wall keeps a three-tile doorway.
                bool doorway = offset == nearOffset && y >= floorY - 3;
                if (edge && !doorway)
                    WorldGen.PlaceTile(x, y, DeckTile, mute: true, forced: true);
            }
        }

        WorldGen.PlaceObject(site.X(farOffset + 3), floorY - 1, TileID.Tables, mute: true);
        WorldGen.PlaceObject(site.X(farOffset + 5), floorY - 1, TileID.Chairs, mute: true, direction: -site.Dir);

        site = site with { HutMidX = midX, HutFloorY = floorY, HutDoorX = site.X(nearOffset) };
        if (finished)
            FinishHut(site);
        return site;
    }

    private static void FinishHut(BridgeSite site)
    {
        if (site.HutFloorY <= 0)
            return;
        if (!Main.tile[site.HutDoorX, site.HutFloorY - 2].HasTile)
            WorldGen.PlaceTile(site.HutDoorX, site.HutFloorY - 2, TileID.ClosedDoor, mute: true);
        WorldGen.PlaceTile(site.HutMidX, site.HutFloorY - 4, TileID.Torches, mute: true);
    }

    private static void PlaceBridgeSign(BridgeSite site)
    {
        int x = site.X(1);
        int y = site.DeckY - 1;
        if (!Main.tile[x, y].HasTile && WorldGen.PlaceTile(x, y, TileID.Signs, mute: true))
            WriteSign(x, y, "鸣人大桥\n——以及所有守护这座桥的忍者");
    }

    private static void WriteSign(int x, int y, string text)
    {
        int sign = Sign.ReadSign(x, y, true);
        if (sign < 0)
            return;
        Sign.TextSign(sign, text);
        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.ReadSign, -1, -1, null, sign, 0f, 1f);
    }

    private static void Refresh(BridgeSite site, bool sync)
    {
        int landReach = BridgeRules.DeckLandOverlap + BridgeRules.MaxStairSteps + BridgeRules.HutGapFromStairs +
                        BridgeRules.HutWidth + 2;
        int x0 = Math.Min(site.X(-landReach), site.X(BridgeRules.TotalReach));
        int x1 = Math.Max(site.X(-landReach), site.X(BridgeRules.TotalReach));
        int y0 = site.DeckY - BridgeRules.Clearance - 1;
        int y1 = Math.Max(site.HutFloorY, site.WaterY) + 60;
        WorldGen.RangeFrame(x0, y0, x1, y1);
        if (!sync || Main.netMode != NetmodeID.Server)
            return;
        for (int x = x0; x <= x1; x += SyncChunk)
            for (int y = y0; y <= y1; y += SyncChunk)
                NetMessage.SendTileSquare(-1, x, y, Math.Min(SyncChunk, x1 - x + 1), Math.Min(SyncChunk, y1 - y + 1));
    }
}
