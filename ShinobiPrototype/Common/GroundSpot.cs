using Microsoft.Xna.Framework;
using Terraria;

namespace ShinobiPrototype.Common;

// Open ground for a body of a given size: no solid tiles where it stands, solid ground (or a platform) under its feet,
// no lava. Used to place the mod's enemies (user, 2026-10-01: bosses and grunts alike were spawning inside walls or
// small holes and could not get out). Positions are bottom centres, like NPC.Bottom.
public static class GroundSpot
{
    public static bool Fits(Vector2 bottom, int width, int height)
    {
        Vector2 topLeft = new(bottom.X - width / 2f, bottom.Y - height);
        return !Collision.SolidCollision(topLeft, width, height) &&
               Collision.SolidCollision(new Vector2(topLeft.X, bottom.Y), width, 8, true) &&
               !Collision.LavaCollision(topLeft, width, height);
    }

    // The nearest open ground to `near`: columns outward from it, and in each column the rows nearest its height.
    public static bool TryNear(Vector2 near, int width, int height, out Vector2 bottom, int tilesAcross = 20,
        int tilesUp = 12, int tilesDown = 16)
    {
        for (int dx = 0; dx <= tilesAcross; dx++)
            foreach (int side in dx == 0 ? new[] { 1 } : new[] { 1, -1 })
            {
                float x = near.X + side * dx * 16f;
                for (int step = 0; step <= System.Math.Max(tilesUp, tilesDown); step++)
                    foreach (int dir in step == 0 ? new[] { 1 } : new[] { -1, 1 })
                    {
                        if (dir < 0 && step > tilesUp || dir > 0 && step > tilesDown)
                            continue;
                        // Snap the feet to the top of a tile row.
                        float y = ((int)(near.Y / 16f) + dir * step) * 16f;
                        Vector2 candidate = new(x, y);
                        if (InWorld(candidate) && Fits(candidate, width, height))
                        {
                            bottom = candidate;
                            return true;
                        }
                    }
            }
        bottom = near;
        return false;
    }

    // Open ground beside a player, `tiles` away: behind them first (the side they are not facing), then in front.
    public static bool TryBeside(Player player, int width, int height, int[] tiles, out Vector2 bottom)
    {
        int behind = player.direction == 0 ? 1 : -player.direction;
        foreach (int side in new[] { behind, -behind })
            foreach (int distance in tiles)
                if (TryNear(player.Bottom + new Vector2(side * distance * 16f, 0f), width, height, out bottom, 2, 6, 12))
                    return true;
        bottom = player.Bottom;
        return false;
    }

    public static bool InWorld(Vector2 bottom) =>
        WorldGen.InWorld((int)(bottom.X / 16f), (int)(bottom.Y / 16f), 10);
}
