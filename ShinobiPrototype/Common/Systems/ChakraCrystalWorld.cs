using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using ShinobiPrototype.Content.Tiles;

namespace ShinobiPrototype.Common.Systems;

// Scatters Chakra Crystals through the cavern layer, the way Life Crystals are. Worlds made before
// this system existed get the same placement once, on the first update after loading.
public sealed class ChakraCrystalWorld : ModSystem
{
    private static bool crystalsPlaced;

    public override void ClearWorld() => crystalsPlaced = false;

    public override void SaveWorldData(TagCompound tag)
    {
        if (crystalsPlaced)
            tag["chakraCrystalsPlaced"] = true;
    }

    public override void LoadWorldData(TagCompound tag) => crystalsPlaced = tag.GetBool("chakraCrystalsPlaced");

    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        int index = tasks.FindIndex(pass => pass.Name == "Life Crystals");
        tasks.Insert(index >= 0 ? index + 1 : tasks.Count, new PassLegacy("Shinobi: Chakra Crystals", Generate));
    }

    private static void Generate(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = Loc.Get("WorldGen.ChakraCrystals");
        PlaceCrystals(sync: false);
        crystalsPlaced = true;
    }

    public override void PostUpdateWorld()
    {
        if (crystalsPlaced || Main.netMode == NetmodeID.MultiplayerClient)
            return;

        PlaceCrystals(sync: Main.netMode == NetmodeID.Server);
        crystalsPlaced = true;
    }

    private static void PlaceCrystals(bool sync)
    {
        int target = Math.Max(8, Main.maxTilesX / 350);
        int placed = 0;
        for (int attempt = 0; attempt < target * 500 && placed < target; attempt++)
        {
            int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
            int y = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 250);
            if (TryPlace(x, y, sync))
                placed++;
        }
    }

    // Drops down from (x, y) to the first cave floor with a clear, dry 2x2 space and places a crystal there.
    private static bool TryPlace(int x, int y, bool sync)
    {
        int type = ModContent.TileType<ChakraCrystalTile>();
        for (int floor = y; floor < y + 40 && floor < Main.maxTilesY - 210; floor++)
        {
            if (!WorldGen.SolidTile(x, floor + 1) || !WorldGen.SolidTile(x + 1, floor + 1))
                continue;
            for (int dx = 0; dx < 2; dx++)
                for (int dy = 0; dy < 2; dy++)
                {
                    Tile tile = Main.tile[x + dx, floor - dy];
                    if (tile.HasTile || tile.LiquidAmount > 0)
                        return false;
                }

            if (!WorldGen.PlaceObject(x, floor, type, mute: true) || Main.tile[x, floor].TileType != type)
                return false;
            if (sync)
                NetMessage.SendTileSquare(-1, x, floor - 1, 2, 2);
            return true;
        }
        return false;
    }
}
