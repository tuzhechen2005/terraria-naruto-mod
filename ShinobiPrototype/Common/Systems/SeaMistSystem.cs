using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Systems;

// The sea mist around the Wave Country bridge: vanilla's graveyard fog clouds, spawned the way vanilla's
// AmbientWindSystem spawns them in a graveyard, but around the bridge instead (no graveyard biome, so no darkening,
// graveyard spawns or housing effects). Floor clouds drift along the ground and the deck; airborne clouds drift in
// chains through open air, including low over the sea. Client-side visuals only.
public sealed class SeaMistSystem : ModSystem
{
    // Vanilla's rates in a graveyard: 1 in 120 per ground tile per tick, 1 in 120000 per air tile per tick.
    private const int VanillaFloorChance = 120;
    private const int VanillaAirChance = 120000;
    private const int SeaSurfaceChance = 360;
    private const int WorkWidth = 120;
    private const int WorkHeight = 30;

    private static readonly List<Point> airSpots = new();
    private static int updates;
    private static IEntitySource source;

    // Fog density 0..1 near the player right now; the preview thickens it.
    public static float Density { get; private set; }

    public override void OnWorldUnload()
    {
        airSpots.Clear();
        Density = 0f;
    }

    public override void PostUpdateEverything()
    {
        if (Main.dedServ || Main.LocalPlayer is not { active: true } player)
            return;

        float target = 0f;
        if (WaveBridgeWorld.MistActive)
            target = BridgeRules.SeaMist(WaveBridgeWorld.DistanceToBridgeTiles(player.Center),
                !Main.dayTime || Main.raining, ShinobiClientConfig.Instance.SeaFogStrength / 100f);
        target = System.Math.Max(target, MistPreviewSystem.MistBoost);
        Density = MathHelper.Lerp(Density, target, 0.05f);
        float rate = BridgeRules.FogSpawnRate(Density);
        if (rate <= 0.01f)
            return;

        source ??= new EntitySource_Misc("ShinobiSeaMist");
        updates++;
        Point center = player.Center.ToTileCoordinates();
        int floorChance = System.Math.Max(1, (int)(VanillaFloorChance / rate));
        int airChance = System.Math.Max(1, (int)(VanillaAirChance / rate));
        int seaChance = System.Math.Max(1, (int)(SeaSurfaceChance / rate));
        for (int x = center.X - WorkWidth / 2; x < center.X + WorkWidth / 2; x++)
            for (int y = center.Y - WorkHeight / 2; y < center.Y + WorkHeight / 2; y++)
            {
                if (!WorldGen.InWorld(x, y, 10))
                    continue;
                if (Main.rand.NextBool(airChance) && AllowsWind(x, y))
                    airSpots.Add(new Point(x, y));
                Tile tile = Main.tile[x, y];
                Tile above = Main.tile[x, y - 1];
                if (tile.HasTile && Main.tileSolid[tile.TileType] && tile.Slope == SlopeType.Solid && !tile.IsHalfBlock &&
                    !WorldGen.SolidTile(above) && Main.rand.NextBool(floorChance))
                {
                    SpawnFloorCloud(x, y);
                    if (Main.rand.NextBool(3))
                        SpawnFloorCloud(x, y - 1);
                }
                else if (tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Water && above.LiquidAmount == 0 &&
                         !WorldGen.SolidTile(above) && Main.rand.NextBool(seaChance))
                {
                    SpawnAirborneChain(x, y - 1, Main.rand.Next(1, 4));
                }
            }

        if (updates % 30 == 0)
        {
            foreach (Point spot in airSpots)
                SpawnAirborneChain(spot.X, spot.Y, Main.rand.Next(2, 6));
            airSpots.Clear();
        }
    }

    private static bool AllowsWind(int x, int y)
    {
        for (int i = -2; i <= 2; i++)
        {
            if (i == 0)
                continue;
            if (Blocks(Main.tile[x + i, y]) || Blocks(Main.tile[x, y + i]))
                return false;
        }
        return true;
    }

    private static bool Blocks(Tile tile) => tile.HasTile && Main.tileSolid[tile.TileType];

    // Same as vanilla's AmbientWindSystem.SpawnFloorCloud.
    private static void SpawnFloorCloud(int x, int y)
    {
        Vector2 position = new Point(x, y - 1).ToWorldCoordinates();
        int type = Main.rand.Next(GoreID.AmbientFloorCloud1, GoreID.AmbientFloorCloud4);
        float lift = 16f * Main.rand.NextFloat();
        position.Y -= lift;
        if (lift < 4f)
            type = GoreID.AmbientFloorCloud4;
        float scale = 0.8f + Main.rand.NextFloat() * 0.2f;
        Gore.NewGorePerfect(source, position, Vector2.UnitX * 0.4f * Main.WindForVisuals, type, scale);
    }

    // A drifting chain of airborne clouds, as in vanilla's AmbientWindSystem.SpawnAirborneCloud.
    private static void SpawnAirborneChain(int x, int y, int count)
    {
        float heading = 0.0236f * Main.rand.NextFloatDirection();
        float turn = 0.0236f * Main.rand.NextFloatDirection();
        while (turn > -0.0118f && turn < 0.0118f)
            turn = 0.0236f * Main.rand.NextFloatDirection();
        float minScale = 1.1f;
        float extraScale = 2.2f;
        if (Main.rand.NextBool(4))
            extraScale = 1.2f;
        Vector2 at = new Point(x, y).ToWorldCoordinates();
        heading -= turn * count * 0.5f;
        for (int i = 0; i < count; i++)
        {
            if (Main.rand.NextBool(10))
                turn *= Main.rand.NextFloatDirection();
            int type = GoreID.AmbientAirborneCloud1 + Main.rand.Next(2) * 2;
            float scale = minScale + Main.rand.NextFloat() * extraScale;
            heading += turn;
            Vector2 velocity = Vector2.UnitX.RotatedBy(heading) * 1.4f;
            Gore.NewGorePerfect(source, at + Main.rand.NextVector2Circular(4f, 4f) + new Vector2(10f, 0f),
                velocity * Main.WindForVisuals, type, scale);
            at += velocity * 6.5f * scale;
        }
    }
}
