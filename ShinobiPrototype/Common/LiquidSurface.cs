using System;
using Terraria;

namespace ShinobiPrototype.Common;

// Where the water's surface really is, in world pixels. The top water tile is rarely full: its liquid sits at the
// bottom of the tile, so the tile's top edge is above the water by up to 16 pixels (user, 2026-10-01: Zabuza and Haku
// floated above the lake and the sea). Figures standing on water use this, sunk a pixel or two so they look planted.
public static class LiquidSurface
{
    public const float StandDepth = 2f;

    // The surface under worldX, searched a few tiles around nearY (a tile-top estimate); nearY itself if no water.
    public static float At(float worldX, float nearY)
    {
        int x = (int)(worldX / 16f);
        int from = (int)(nearY / 16f) - 4;
        if (x < 0 || x >= Main.maxTilesX)
            return nearY;
        for (int y = Math.Max(from, 0); y < Math.Min(from + 12, Main.maxTilesY); y++)
        {
            Tile tile = Main.tile[x, y];
            if (tile.LiquidAmount > 0)
                return y * 16f + 16f - tile.LiquidAmount / 16f;
            if (tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType] && y > from + 4)
                break;
        }
        return nearY;
    }

    // A foot position standing on that surface.
    public static float StandY(float worldX, float nearY) => At(worldX, nearY) + StandDepth;
}
