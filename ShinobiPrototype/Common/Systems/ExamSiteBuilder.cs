using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace ShinobiPrototype.Common.Systems;

// Turns an ExamSiteDesign into vanilla tiles at world generation, the way KonohaBuilder lays the village: level the
// land under the site, then place blocks, walls and fixtures.
internal static class ExamSiteBuilder
{
    // Fewer than this many jungle columns in a row is not the jungle (a patch of mud, or a floating island's edge).
    private const int MinJungleWidth = 80;
    private const int JungleGapTolerance = 15;
    private const int OceanMargin = 380;

    // The widest stretch of jungle surface, as (west, east) tile columns.
    public static (int West, int East)? JungleSurface()
    {
        (int, int)? best = null;
        int runStart = -1, lastJungle = -1;
        for (int x = OceanMargin; x < Main.maxTilesX - OceanMargin; x++)
        {
            if (!IsJungle(x))
                continue;
            if (runStart < 0 || x - lastJungle > JungleGapTolerance)
                runStart = x;
            lastJungle = x;
            if (best is not (int w, int e) || lastJungle - runStart > e - w)
                best = (runStart, lastJungle);
        }
        return best is (int west, int east) && east - west >= MinJungleWidth ? best : null;
    }

    private static bool IsJungle(int x)
    {
        int y = KonohaBuilder.Surface(x);
        Tile tile = Main.tile[x, y];
        return tile.HasTile && tile.TileType is TileID.JungleGrass or TileID.Mud;
    }

    public static ExamSite Build(ExamSiteKind kind, int centerX, int dir = 1)
    {
        ExamSiteDesign design = ExamSiteWorld.Design(kind, dir);
        centerX = Math.Clamp(centerX, OceanMargin, Main.maxTilesX - OceanMargin);
        ExamSite site = new(kind, centerX, GroundLevel(centerX, design), dir < 0 ? -1 : 1);
        Level(site, design);
        KonohaSite origin = site.Origin;
        foreach (KCell cell in design.Cells)
            KonohaBuilder.PlaceCell(origin, cell);
        foreach (KWallCell wall in design.Walls)
            WorldGen.PlaceWall(origin.X(wall.Dx), origin.Y(wall.Dy), KonohaBuilder.WallFor(wall.Wall), mute: true);
        foreach (KPlace place in design.Places.OrderBy(p => p.Fix == KFix.Door ? 0 : 1))
            KonohaBuilder.PlaceFixture(origin, place);
        int margin = design.HalfWidth + design.Blend + 2;
        WorldGen.RangeFrame(centerX - margin, site.GroundY - design.ClearHeight - 2, centerX + margin,
            site.GroundY + design.FoundationDepth + 2);
        return site;
    }

    // Clears what stands on a site (tiles, walls, liquids) `half` tiles each way and `height` rows up from its ground.
    public static void Erase(ExamSite site, int half, int height)
    {
        for (int x = site.CenterX - half; x <= site.CenterX + half; x++)
            for (int y = site.GroundY - height; y < site.GroundY; y++)
                if (WorldGen.InWorld(x, y, 10))
                    Main.tile[x, y].ClearEverything();
        WorldGen.RangeFrame(site.CenterX - half, site.GroundY - height, site.CenterX + half, site.GroundY);
    }

    // The median surface across the site, so one odd column (a pond, a tree stump) does not decide it.
    private static int GroundLevel(int centerX, ExamSiteDesign design)
    {
        List<int> rows = new();
        for (int x = centerX - design.HalfWidth; x <= centerX + design.HalfWidth; x++)
            rows.Add(KonohaBuilder.Surface(x));
        rows.Sort();
        return rows[rows.Count / 2];
    }

    // Clear the sky over the site and slope back to the natural surface past its edges; solid ground beneath.
    private static void Level(ExamSite site, ExamSiteDesign design)
    {
        ushort top = design.Jungle ? TileID.JungleGrass : TileID.Grass;
        ushort under = design.Jungle ? TileID.Mud : TileID.Dirt;
        int half = design.HalfWidth, blend = design.Blend;
        for (int dx = -half - blend; dx <= half + blend; dx++)
        {
            int x = site.CenterX + dx;
            if (!WorldGen.InWorld(x, site.GroundY, 20))
                continue;
            int target = site.GroundY;
            if (Math.Abs(dx) > half)
            {
                float t = (Math.Abs(dx) - half) / (float)blend;
                target = (int)Math.Round(site.GroundY + (KonohaBuilder.BlendSurface(x) - site.GroundY) * t * t * (3f - 2f * t));
            }
            for (int y = site.GroundY - design.ClearHeight; y < target; y++)
                Main.tile[x, y].ClearEverything();
            for (int y = target; y <= target + 60; y++)
            {
                Tile tile = Main.tile[x, y];
                bool solid = tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType];
                if (y > target + design.FoundationDepth && solid)
                    break;
                if (y == target)
                {
                    tile.ClearEverything();
                    tile.ResetToType(top);
                }
                else if (!solid)
                {
                    tile.ClearEverything();
                    tile.ResetToType(under);
                }
                tile.LiquidAmount = 0;
            }
        }
    }
}
