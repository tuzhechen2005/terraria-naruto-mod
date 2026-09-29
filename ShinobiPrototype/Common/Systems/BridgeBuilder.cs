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
    public int DeckY => BridgeDesign.DeckY(WaterY);
    public int DeckRow(int offset) => BridgeDesign.DeckRow(WaterY, offset);
    public int X(int offset) => ShoreX + Dir * offset;
    public int OffsetOf(int tileX) => (tileX - ShoreX) * Dir;
    public int HutCeilingY => HutFloorY - BridgeDesign.HutRoomHeight - 1;
}

// Turns a BridgeDesign into vanilla tiles, walls and furniture, for world generation, the blueprint and completion.
internal static class BridgeBuilder
{
    private const int SyncChunk = 30;
    private const byte RedPaint = 1;

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
        int far = shoreX + dir * BridgeDesign.TotalReach;
        if (far < 45 || far > Main.maxTilesX - 45)
            return false;
        if (!TryColumnTop(shoreX + dir * 8, (int)(Main.worldSurface * 0.35), out int waterY, out bool isSea) || !isSea)
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
            if (IsSolidGround(tile))
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

    private static bool IsSolidGround(Tile tile) =>
        tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType];

    // The design for a site, measured against the terrain as it is right now. When the bridge already stands,
    // its own materials are looked through so the measurements still find the natural ground and sea floor.
    public static BridgeDesign Design(BridgeSite site, bool finished, bool lookThroughBridge = false)
    {
        int top = site.DeckY - BridgeDesign.Clearance;
        Dictionary<int, int> solid = new();
        int FirstSolid(int offset)
        {
            if (solid.TryGetValue(offset, out int y))
                return y;
            int x = site.X(offset);
            for (y = top; y < Main.maxTilesY - 10; y++)
            {
                Tile tile = Main.tile[x, y];
                if (IsSolidGround(tile) && !(lookThroughBridge && IsBridgeMaterial(tile.TileType)))
                    break;
            }
            return solid[offset] = y;
        }
        return BridgeDesign.Create(site.WaterY, FirstSolid, FirstSolid, finished);
    }

    // Natural terrain may be cleared; anything a player could have placed blocks the build.
    public static bool CheckObstacles(BridgeSite site, out string reason)
    {
        BridgeDesign design = Design(site, StoryWorld.WaveComplete);
        foreach ((int x, int y) in Footprint(site, design))
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

    private static IEnumerable<(int X, int Y)> Footprint(BridgeSite site, BridgeDesign design)
    {
        foreach ((int from, int to, int top, int bottom) in design.Clear)
            for (int o = from; o <= to; o++)
                for (int y = top; y <= bottom; y++)
                    yield return (site.X(o), y);
        foreach (Cell cell in design.Cells)
            yield return (site.X(cell.Offset), cell.Y);
    }

    private static bool IsBridgeMaterial(int type) =>
        type is TileID.GrayBrick or TileID.StoneSlab or TileID.WoodBlock or TileID.DynastyWood or
            TileID.BlueDynastyShingles;

    private static bool IsNatural(int type) =>
        Main.tileCut[type] || TileID.Sets.Ore[type] || type is TileID.Dirt or TileID.Stone or TileID.Grass or
            TileID.Sand or TileID.ClayBlock or TileID.Mud or TileID.JungleGrass or TileID.HardenedSand or
            TileID.Sandstone or TileID.Silt or TileID.Slush or TileID.SnowBlock or TileID.IceBlock or
            TileID.Cactus or TileID.PalmTree or TileID.Trees or TileID.Saplings or TileID.BeachPiles or
            TileID.Pots or TileID.SmallPiles or TileID.LargePiles or TileID.LargePiles2 or TileID.Sunflower or
            TileID.DyePlants or TileID.Coral;

    public static BridgeSite Build(BridgeSite site, bool finished, bool sync)
    {
        BridgeDesign design = Design(site, finished);
        foreach ((int from, int to, int top, int bottom) in design.Clear)
            for (int o = from; o <= to; o++)
                for (int y = top; y <= bottom; y++)
                {
                    Tile tile = Main.tile[site.X(o), y];
                    tile.ClearEverything();
                }

        foreach (Cell cell in design.Cells)
            PlaceCell(site, cell, onlyIfEmpty: false);
        foreach (Wall wall in design.Walls)
            PlaceWall(site, wall);

        site = site with
        {
            HutMidX = site.X(design.HutMidOffset),
            HutFloorY = design.HutFloorY,
            HutDoorX = site.X(design.HutDoorOffset),
        };
        foreach (Placement placement in design.Placements)
            PlaceFixture(site, placement);

        Refresh(site, design, sync);
        return site;
    }

    // Completion after the first victory: take down the scaffolding and crane, then fill only empty tiles over
    // the sea with the finished arches, deck, railings and lamps. The hut gets its lantern, which makes it a home.
    public static void Finish(BridgeSite site, bool sync)
    {
        BridgeDesign done = Design(site, finished: true, lookThroughBridge: true);
        BridgeDesign before = Design(site, finished: false, lookThroughBridge: true);
        foreach (Cell cell in before.Cells)
        {
            if (!cell.Scaffold || done.Has(cell.Offset, cell.Y))
                continue;
            Tile tile = Main.tile[site.X(cell.Offset), cell.Y];
            if (tile.HasTile && tile.TileType == TileFor(cell.Part))
                WorldGen.KillTile(site.X(cell.Offset), cell.Y, noItem: true);
        }

        foreach (Cell cell in done.Cells)
            if (cell.Offset > 0 && cell.Offset < BridgeDesign.IslandStart)
                PlaceCell(site, cell, onlyIfEmpty: true);
        foreach (Wall wall in done.Walls)
            if (wall.Offset > 0 && wall.Offset < BridgeDesign.IslandStart)
                PlaceWall(site, wall);
        foreach (Placement placement in done.Placements)
            if (placement.Fixture is Fixture.LampPost or Fixture.BridgeSign &&
                placement.Offset >= 0 && placement.Offset < BridgeDesign.IslandStart)
                PlaceFixture(site, placement);

        if (site.HutFloorY > 0)
            WorldGen.PlaceObject(site.HutMidX, site.HutCeilingY + 1, TileID.HangingLanterns, mute: true);
        Refresh(site, done, sync);
    }

    private static ushort TileFor(Part part) => part switch
    {
        Part.Brick => TileID.GrayBrick,
        Part.Slab => TileID.StoneSlab,
        Part.Stone => TileID.Stone,
        Part.Sand => TileID.Sand,
        Part.Dirt => TileID.Dirt,
        Part.Grass => TileID.Grass,
        Part.Wood or Part.RedWood => TileID.WoodBlock,
        Part.DynastyWood => TileID.DynastyWood,
        Part.Shingle => TileID.BlueDynastyShingles,
        Part.Beam or Part.RedBeam => TileID.WoodenBeam,
        Part.Platform => TileID.Platforms,
        _ => TileID.Rope,
    };

    // Bridge-relative shapes to Terraria slopes. SlopeDownRight keeps the bottom and right sides solid (the top
    // climbs to the right); SlopeUpLeft keeps the top and left (the underside climbs to the right).
    private static SlopeType SlopeFor(Shape shape, int dir)
    {
        bool seaRight = dir >= 0;
        return shape switch
        {
            Shape.RisesSeaward => seaRight ? SlopeType.SlopeDownRight : SlopeType.SlopeDownLeft,
            Shape.RisesLandward => seaRight ? SlopeType.SlopeDownLeft : SlopeType.SlopeDownRight,
            Shape.CeilingRisesSeaward => seaRight ? SlopeType.SlopeUpLeft : SlopeType.SlopeUpRight,
            Shape.CeilingRisesLandward => seaRight ? SlopeType.SlopeUpRight : SlopeType.SlopeUpLeft,
            _ => SlopeType.Solid,
        };
    }

    private static void PlaceCell(BridgeSite site, Cell cell, bool onlyIfEmpty)
    {
        int x = site.X(cell.Offset);
        Tile tile = Main.tile[x, cell.Y];
        if (onlyIfEmpty && tile.HasTile)
            return;
        ushort type = TileFor(cell.Part);
        tile.ClearTile();
        if (Main.tileSolid[type] && !Main.tileSolidTop[type])
            tile.LiquidAmount = 0;
        WorldGen.PlaceTile(x, cell.Y, type, mute: true, forced: true);
        tile = Main.tile[x, cell.Y];
        if (!tile.HasTile)
            return;
        if (cell.Shape == Shape.Half)
            tile.IsHalfBlock = true;
        else if (cell.Shape != Shape.Full)
            tile.Slope = SlopeFor(cell.Shape, site.Dir);
        if (cell.Part is Part.RedBeam or Part.RedWood)
            tile.TileColor = RedPaint;
    }

    private static void PlaceWall(BridgeSite site, Wall wall)
    {
        int x = site.X(wall.Offset);
        if (Main.tile[x, wall.Y].WallType != WallID.None)
            return;
        ushort type = wall.Backdrop switch
        {
            Backdrop.StoneWall => WallID.GrayBrick,
            Backdrop.Railing => WallID.StoneSlab,
            Backdrop.ShojiWall => WallID.WhiteDynasty,
            Backdrop.Glass => WallID.Glass,
            _ => WallID.Wood,
        };
        WorldGen.PlaceWall(x, wall.Y, type, mute: true);
    }

    private static void PlaceFixture(BridgeSite site, Placement placement)
    {
        int x = site.X(placement.Offset);
        int y = placement.Y;
        switch (placement.Fixture)
        {
            case Fixture.LampPost:
                WorldGen.PlaceObject(x, y, TileID.Lampposts, mute: true);
                break;
            case Fixture.Lantern:
                WorldGen.PlaceObject(x, y, TileID.HangingLanterns, mute: true);
                break;
            case Fixture.Door:
                PlaceDoor(x, y);
                break;
            case Fixture.Table:
                WorldGen.PlaceObject(x, y, TileID.Tables, mute: true);
                break;
            case Fixture.Chair:
                WorldGen.PlaceObject(x, y, TileID.Chairs, mute: true, direction: -site.Dir);
                break;
            case Fixture.IslandSign:
                PlaceSign(Math.Min(x, x + site.Dir), y, "波之国");
                break;
            case Fixture.BridgeSign:
                PlaceSign(Math.Min(x, x + site.Dir), y, "鸣人大桥\n——以及所有守护这座桥的忍者");
                break;
            case Fixture.Chest:
                FillChest(WorldGen.PlaceChest(Math.Min(x, x + site.Dir), y, TileID.Containers,
                    notNearOtherChests: false, style: 0));
                break;
            case Fixture.Palm:
                WorldGen.PlaceTile(x, y - 1, TileID.Saplings, mute: true, style: 0);
                WorldGen.GrowPalmTree(x, y - 1);
                break;
            case Fixture.Shell:
                WorldGen.PlaceTile(x, y, TileID.BeachPiles, mute: true, style: WorldGen.genRand.Next(3));
                break;
        }
    }

    // A three-tall door whose bottom tile is `bottomY`. Which row the placement call anchors on differs between
    // placement paths, so each candidate is tried until the door is confirmed standing in the doorway; a
    // misaligned attempt overlaps the floor or the wall above and simply fails.
    private static void PlaceDoor(int x, int bottomY)
    {
        foreach (int anchor in new[] { bottomY - 1, bottomY - 2, bottomY })
        {
            if (IsDoorInPlace(x, bottomY))
                return;
            WorldGen.PlaceTile(x, anchor, TileID.ClosedDoor, mute: true);
        }
        if (!IsDoorInPlace(x, bottomY))
            WorldGen.PlaceObject(x, bottomY - 2, TileID.ClosedDoor, mute: true);
    }

    private static bool IsDoorInPlace(int x, int bottomY)
    {
        for (int y = bottomY - 2; y <= bottomY; y++)
            if (!Main.tile[x, y].HasTile || Main.tile[x, y].TileType != TileID.ClosedDoor)
                return false;
        return true;
    }

    private static void PlaceSign(int x, int bottomY, string text)
    {
        if (!WorldGen.PlaceObject(x, bottomY, TileID.Signs, mute: true))
            return;
        int sign = Sign.ReadSign(x, bottomY, true);
        if (sign < 0)
            return;
        Sign.TextSign(sign, text);
        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.ReadSign, -1, -1, null, sign, 0f, 1f);
    }

    private static void FillChest(int chest)
    {
        if (chest < 0)
            return;
        Item[] items = Main.chest[chest].item;
        items[0].SetDefaults(ItemID.Rope);
        items[0].stack = 30;
        items[1].SetDefaults(ItemID.Torch);
        items[1].stack = 15;
        items[2].SetDefaults(ModContent.ItemType<ChakraPill>());
        items[2].stack = 3;
    }

    private static void Refresh(BridgeSite site, BridgeDesign design, bool sync)
    {
        int x0 = Math.Min(site.X(design.LandmostOffset - 2), site.X(BridgeDesign.TotalReach));
        int x1 = Math.Max(site.X(design.LandmostOffset - 2), site.X(BridgeDesign.TotalReach));
        int y0 = site.DeckY - BridgeDesign.Clearance - 2;
        int y1 = site.WaterY + 60;
        WorldGen.RangeFrame(x0, y0, x1, y1);
        if (!sync || Main.netMode != NetmodeID.Server)
            return;
        for (int x = x0; x <= x1; x += SyncChunk)
            for (int y = y0; y <= y1; y += SyncChunk)
                NetMessage.SendTileSquare(-1, x, y, Math.Min(SyncChunk, x1 - x + 1), Math.Min(SyncChunk, y1 - y + 1));
    }
}
