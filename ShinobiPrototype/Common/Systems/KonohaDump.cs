using System;
using System.Globalization;
using System.IO;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Systems;

// Development check, off unless the environment variable SHINOBI_KONOHA_DUMP names an output file: once a dedicated
// server has generated a world, write every tile of the village (type, frame, slope, wall) plus vanilla's
// own housing verdict for each designed home, then quit. scripts/konoha-worldgen-check.sh runs it and
// scripts/render_konoha_world.py draws the result with the vanilla textures.
public sealed class KonohaDump : ModSystem
{
    // Right after generation: a dedicated server with nobody connected does not run world updates.
    public override void PostWorldGen()
    {
        string path = Environment.GetEnvironmentVariable("SHINOBI_KONOHA_DUMP");
        if (string.IsNullOrEmpty(path) || !Main.dedServ)
            return;
        if (KonohaWorld.Site is KonohaSite site)
        {
            int half = KonohaDesign.HalfWidth + KonohaDesign.Blend + 10;
            WorldGen.RangeFrame(site.CenterX - half, site.GroundY - 62, site.CenterX + half, site.GroundY + 22);
        }
        Write(path);
        Environment.Exit(0);
    }

    private static int? ExpectedTile(KFix fix) => fix switch
    {
        KFix.Door => TileID.ClosedDoor,
        KFix.Table => TileID.Tables,
        KFix.Chair => TileID.Chairs,
        KFix.Lantern => TileID.ChineseLanterns,
        KFix.LampPost => TileID.Lampposts,
        KFix.Sign => TileID.Signs,
        KFix.Banner => TileID.Banners,
        KFix.Bookcase => TileID.Bookcases,
        KFix.Bed => TileID.Beds,
        KFix.Painting => TileID.Painting3X3,
        KFix.WeaponRack => TileID.WeaponsRack,
        KFix.Bench => TileID.Benches,
        KFix.PottedPlant => TileID.ClayPot,
        KFix.Tree => TileID.Trees,
        _ => null,
    };

    private static void Write(string path)
    {
        if (KonohaWorld.Site is not KonohaSite site)
        {
            File.WriteAllText(path, "{\"k\":\"error\",\"msg\":\"no village\"}\n");
            return;
        }
        KonohaDesign design = KonohaDesign.Create();
        WriteRegion(path, site, KonohaDesign.HalfWidth + KonohaDesign.Blend + 10, 60, 20, design.Rooms, design.Places);
        // The Chūnin Exam landmarks, each in its own file next to the village's.
        foreach (ExamSite exam in ExamSiteWorld.All())
        {
            ExamSiteDesign d = ExamSiteDesign.Create(exam.Kind);
            int half = d.HalfWidth + d.Blend + 6;
            WorldGen.RangeFrame(exam.CenterX - half, exam.GroundY - d.ClearHeight - 2, exam.CenterX + half, exam.GroundY + 22);
            WriteRegion(path.Replace(".jsonl", $"-{exam.Kind}.jsonl"), exam.Origin, half, d.ClearHeight, 20,
                System.Array.Empty<KRoom>(), d.Places);
        }
    }

    private static void WriteRegion(string path, KonohaSite site, int half, int above, int below,
        System.Collections.Generic.IEnumerable<KRoom> rooms, System.Collections.Generic.IEnumerable<KPlace> places)
    {
        StringBuilder sb = new();
        int top = site.GroundY - above, bottom = site.GroundY + below;
        sb.Append(CultureInfo.InvariantCulture,
            $"{{\"k\":\"meta\",\"cx\":{site.CenterX},\"gy\":{site.GroundY},\"x0\":{site.CenterX - half},\"x1\":{site.CenterX + half},\"y0\":{top},\"y1\":{bottom},\"spawnX\":{Main.spawnTileX},\"spawnY\":{Main.spawnTileY},\"w\":{Main.maxTilesX},\"h\":{Main.maxTilesY}}}\n");
        for (int x = site.CenterX - half; x <= site.CenterX + half; x++)
            for (int y = top; y <= bottom; y++)
            {
                Tile t = Main.tile[x, y];
                if (!t.HasTile && t.WallType == 0 && t.LiquidAmount == 0)
                    continue;
                sb.Append($"{{\"x\":{x},\"y\":{y}");
                if (t.HasTile)
                    sb.Append($",\"t\":{t.TileType},\"fx\":{t.TileFrameX},\"fy\":{t.TileFrameY},\"sl\":{(int)t.Slope},\"hb\":{(t.IsHalfBlock ? 1 : 0)},\"tc\":{t.TileColor}");
                if (t.WallType != 0)
                    sb.Append($",\"w\":{t.WallType},\"wfx\":{t.WallFrameX},\"wfy\":{t.WallFrameY}");
                if (t.LiquidAmount > 0)
                    sb.Append($",\"l\":{t.LiquidAmount},\"lt\":{t.LiquidType}");
                sb.Append("}\n");
            }

        foreach (KRoom room in rooms)
        {
            int x = site.X((room.X0 + room.X1) / 2), y = site.Y(room.Bottom);
            bool check = WorldGen.StartRoomCheck(x, y);
            bool needs = check && WorldGen.RoomNeeds(NPCID.Guide);
            int score = -1;
            if (needs)
            {
                WorldGen.ScoreRoom();
                score = WorldGen.hiScore;
            }
            sb.Append($"{{\"k\":\"room\",\"b\":\"{room.Building}\",\"x0\":{site.X(room.X0)},\"x1\":{site.X(room.X1)},\"top\":{site.Y(room.Top)},\"bottom\":{site.Y(room.Bottom)},\"check\":{(check ? 1 : 0)},\"needs\":{(needs ? 1 : 0)},\"score\":{score}}}\n");
        }
        // Did every designed fixture actually go in? Placement fails silently when something is in the way.
        foreach (KPlace place in places)
        {
            int x = site.X(place.Dx), y = site.Y(place.Dy);
            int? expected = ExpectedTile(place.Fix);
            if (expected is not int type)
                continue;
            bool found = false;
            for (int dx = -2; dx <= 2 && !found; dx++)
                for (int dy = -3; dy <= 3 && !found; dy++)
                {
                    Tile t = Main.tile[x + dx, y + dy];
                    found = t.HasTile && t.TileType == type;
                }
            if (!found)
                sb.Append($"{{\"k\":\"missing\",\"fix\":\"{place.Fix}\",\"dx\":{place.Dx},\"dy\":{place.Dy}}}\n");
        }
        File.WriteAllText(path, sb.ToString());
    }
}
