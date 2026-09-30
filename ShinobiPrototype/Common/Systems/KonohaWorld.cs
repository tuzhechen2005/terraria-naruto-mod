using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;

namespace ShinobiPrototype.Common.Systems;

// Where the Hidden Leaf Village stands: the A-Un gate at the spawn point (M2a spec). New worlds only.
public readonly record struct KonohaSite(int CenterX, int GroundY)
{
    public int X(int dx) => CenterX + dx;
    public int Y(int dy) => GroundY + dy;
    public bool Contains(int tileX, int tileY) =>
        Math.Abs(tileX - CenterX) <= KonohaDesign.HalfWidth &&
        tileY >= GroundY - KonohaDesign.ClearHeight && tileY <= GroundY + KonohaDesign.FoundationDepth;
}

public sealed class KonohaWorld : ModSystem
{
    public static KonohaSite? Site { get; private set; }
    public static int BuiltVersion { get; private set; }

    public static bool InKonoha(Vector2 worldPosition) =>
        Site is KonohaSite site && site.Contains((int)(worldPosition.X / 16f), (int)(worldPosition.Y / 16f));

    public override void ClearWorld()
    {
        Site = null;
        BuiltVersion = 0;
    }

    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        // Last of all but the final cleanup, so vanilla's trees, pots and grass are already there to clear away.
        int index = tasks.FindIndex(pass => pass.Name == "Final Cleanup");
        tasks.Insert(index >= 0 ? index : tasks.Count, new PassLegacy("Shinobi: Hidden Leaf Village", Generate));
    }

    private static void Generate(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "建造木叶隐村";
        Site = KonohaBuilder.Build(Main.spawnTileX);
        BuiltVersion = KonohaDesign.Version;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        if (Site is not KonohaSite site)
            return;
        tag["konohaX"] = site.CenterX;
        tag["konohaY"] = site.GroundY;
        tag["konohaVersion"] = BuiltVersion;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        if (tag.ContainsKey("konohaX"))
            Site = new KonohaSite(tag.GetInt("konohaX"), tag.GetInt("konohaY"));
        BuiltVersion = tag.GetInt("konohaVersion");
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write(Site.HasValue);
        if (Site is KonohaSite site)
        {
            writer.Write(site.CenterX);
            writer.Write(site.GroundY);
        }
    }

    public override void NetReceive(BinaryReader reader)
    {
        Site = reader.ReadBoolean() ? new KonohaSite(reader.ReadInt32(), reader.ReadInt32()) : null;
    }
}
