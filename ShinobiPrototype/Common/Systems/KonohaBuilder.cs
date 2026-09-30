using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace ShinobiPrototype.Common.Systems;

// Turns a KonohaDesign into vanilla tiles at world generation: levels the land around the spawn point, lays the
// village and moves the spawn (and the Guide) to the gate.
internal static class KonohaBuilder
{
    public static KonohaSite Build(int centerX)
    {
        int ground = GroundLevel(centerX);
        KonohaSite site = new(centerX, ground);
        KonohaDesign design = KonohaDesign.Create();

        Level(site);
        foreach (KCell cell in design.Cells)
            PlaceCell(site, cell);
        foreach (KWallCell wall in design.Walls)
            WorldGen.PlaceWall(site.X(wall.Dx), site.Y(wall.Dy), WallFor(wall.Wall), mute: true);
        // Furniture after all tiles and walls, doors first so rooms are closed before anything else is placed.
        foreach (KPlace place in design.Places.OrderBy(p => p.Fix == KFix.Door ? 0 : 1))
            PlaceFixture(site, place);

        int margin = KonohaDesign.HalfWidth + KonohaDesign.Blend + 2;
        WorldGen.RangeFrame(centerX - margin, ground - KonohaDesign.ClearHeight - 2, centerX + margin,
            ground + KonohaDesign.FoundationDepth + 2);

        Main.spawnTileX = centerX;
        Main.spawnTileY = ground;
        foreach (NPC npc in Main.ActiveNPCs)
            if (npc.townNPC)
            {
                npc.position.X = centerX * 16f + 8f - npc.width / 2f + npc.whoAmI * 20f;
                npc.position.Y = ground * 16f - npc.height;
                npc.homeless = true;
            }
        return site;
    }

    // The ground around the spawn: the median surface over the plaza, so one odd column does not decide it.
    private static int GroundLevel(int centerX)
    {
        List<int> rows = new();
        for (int x = centerX - KonohaDesign.PlazaHalf; x <= centerX + KonohaDesign.PlazaHalf; x++)
            rows.Add(Surface(x));
        rows.Sort();
        return rows[rows.Count / 2];
    }

    private static int Surface(int x)
    {
        for (int y = (int)(Main.worldSurface * 0.3); y < Main.worldSurface + 30; y++)
        {
            Tile tile = Main.tile[x, y];
            if (tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType] &&
                tile.TileType is not (TileID.Trees or TileID.LivingWood or TileID.LeafBlock) || tile.LiquidAmount > 0)
                return y;
        }
        return (int)Main.worldSurface;
    }

    // Clear the sky over the village and slope back to the natural surface past the walls; solid dirt beneath.
    private static void Level(KonohaSite site)
    {
        int half = KonohaDesign.HalfWidth, blend = KonohaDesign.Blend;
        for (int dx = -half - blend; dx <= half + blend; dx++)
        {
            int x = site.X(dx);
            if (!WorldGen.InWorld(x, site.GroundY, 20))
                continue;
            int target = site.GroundY;
            if (Math.Abs(dx) > half)
            {
                float t = (Math.Abs(dx) - half) / (float)blend;
                target = (int)Math.Round(site.GroundY + (Surface(x) - site.GroundY) * t * t * (3f - 2f * t));
            }
            for (int y = site.GroundY - KonohaDesign.ClearHeight; y < target; y++)
            {
                Tile tile = Main.tile[x, y];
                tile.ClearEverything();
            }
            // Solid dirt at least FoundationDepth deep, and on down to the old ground where the land was lower, so the
            // raised ground never overhangs a hollow.
            for (int y = target; y <= target + 80; y++)
            {
                Tile tile = Main.tile[x, y];
                bool solid = tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType];
                if (y > target + KonohaDesign.FoundationDepth && solid)
                    break;
                if (y == target)
                {
                    tile.ClearEverything();
                    tile.ResetToType(TileID.Grass);
                }
                else if (!solid)
                {
                    tile.ClearEverything();
                    tile.ResetToType(TileID.Dirt);
                }
                tile.LiquidAmount = 0;
            }
        }
    }

    private static void PlaceCell(KonohaSite site, KCell cell)
    {
        int x = site.X(cell.Dx), y = site.Y(cell.Dy);
        Tile tile = Main.tile[x, y];
        tile.ClearTile();
        tile.LiquidAmount = 0;
        tile.ResetToType(TileFor(cell.Mat));
        if (cell.Shape == KShape.Half)
            tile.IsHalfBlock = true;
        else if (cell.Shape == KShape.TopRisesEast)
            tile.Slope = SlopeType.SlopeDownRight;
        else if (cell.Shape == KShape.TopRisesWest)
            tile.Slope = SlopeType.SlopeDownLeft;
    }

    private static ushort TileFor(KMat mat) => mat switch
    {
        KMat.Grass => TileID.Grass,
        KMat.Dirt => TileID.Dirt,
        KMat.Slab => TileID.StoneSlab,
        KMat.Brick => TileID.GrayBrick,
        KMat.RedBrick => TileID.RedBrick,
        KMat.Stucco => TileID.YellowStucco,
        KMat.Marble => TileID.MarbleBlock,
        KMat.DynastyWood => TileID.DynastyWood,
        KMat.Wood => TileID.WoodBlock,
        KMat.RedShingle => TileID.RedDynastyShingles,
        KMat.BlueShingle => TileID.BlueDynastyShingles,
        KMat.Plating => TileID.MarbleBlock,
        KMat.Beam => TileID.WoodenBeam,
        KMat.Platform => TileID.Platforms,
        _ => TileID.LivingWood,
    };

    private static ushort WallFor(KWall wall) => wall switch
    {
        KWall.Planks => WallID.Planked,
        KWall.Stucco => WallID.YellowStucco,
        KWall.Shoji => WallID.WhiteDynasty,
        KWall.Marble => WallID.MarbleBlock,
        KWall.RedBrick => WallID.RedBrick,
        KWall.Brick => WallID.GrayBrick,
        KWall.Palm => WallID.PalmWood,
        KWall.RedStucco => WallID.RedStucco,
        KWall.Fence => WallID.WoodenFence,
        KWall.DoorLeaf => WallID.RichMaogany,
        _ => WallID.Wood,
    };

    private static void PlaceFixture(KonohaSite site, KPlace place)
    {
        int x = site.X(place.Dx), y = site.Y(place.Dy);
        switch (place.Fix)
        {
            case KFix.Door:
                PlaceDoor(x, y);
                break;
            case KFix.Table:
                WorldGen.PlaceObject(x, y, TileID.Tables, mute: true);
                break;
            case KFix.Chair:
                WorldGen.PlaceObject(x, y, TileID.Chairs, mute: true, direction: 1);
                break;
            case KFix.Lantern:
                WorldGen.PlaceObject(x, y, TileID.ChineseLanterns, mute: true);
                break;
            case KFix.LampPost:
                WorldGen.PlaceObject(x, y, TileID.Lampposts, mute: true);
                break;
            case KFix.Banner:
                WorldGen.PlaceObject(x, y, TileID.Banners, mute: true, style: place.Style);
                break;
            case KFix.Bookcase:
                WorldGen.PlaceObject(x, y, TileID.Bookcases, mute: true);
                break;
            case KFix.Bed:
                WorldGen.PlaceObject(x, y, TileID.Beds, mute: true, direction: 1);
                break;
            case KFix.Sign:
                PlaceSign(x, y, place.Text);
                break;
            case KFix.Tree:
                WorldGen.PlaceTile(x, y, TileID.Saplings, mute: true);
                WorldGen.GrowTree(x, y);
                break;
        }
    }

    // Three-tall door with its bottom at `bottomY` (see BridgeBuilder.PlaceDoor for why several anchors are tried).
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
        if (sign >= 0)
            Sign.TextSign(sign, text);
    }
}
