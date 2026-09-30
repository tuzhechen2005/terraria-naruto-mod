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

    private static void Write(string path)
    {
        StringBuilder sb = new();
        if (KonohaWorld.Site is not KonohaSite site)
        {
            File.WriteAllText(path, "{\"k\":\"error\",\"msg\":\"no village\"}\n");
            return;
        }
        int half = KonohaDesign.HalfWidth + KonohaDesign.Blend + 10;
        int top = site.GroundY - 60, bottom = site.GroundY + 20;
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

        KonohaDesign design = KonohaDesign.Create();
        foreach (KRoom room in design.Rooms)
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
        File.WriteAllText(path, sb.ToString());
    }
}
