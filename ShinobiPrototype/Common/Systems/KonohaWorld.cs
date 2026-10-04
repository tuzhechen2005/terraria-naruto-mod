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

    private static KonohaDesign design;

    // The layout the village was built from (the same for every world of this version).
    public static KonohaDesign Design => design ??= KonohaDesign.Create();

    // Inside a building's outline (walls to roof, ground floor up), e.g. the Academy for the written test.
    public static bool InBuilding(string name, Vector2 worldPosition)
    {
        if (Site is not KonohaSite site)
            return false;
        int x = (int)(worldPosition.X / 16f) - site.CenterX, y = (int)(worldPosition.Y / 16f) - site.GroundY;
        foreach (KBuilding building in Design.Buildings)
            if (building.Name == name && x >= building.X0 && x <= building.X1 && y >= building.Top && y < 0)
                return true;
        return false;
    }

    // The middle of a building at street level, for the quest tracker.
    public static Vector2? BuildingWhere(string name)
    {
        if (Site is not KonohaSite site)
            return null;
        foreach (KBuilding building in Design.Buildings)
            if (building.Name == name)
                return new Vector2(site.X((building.X0 + building.X1) / 2) * 16f, site.GroundY * 16f);
        return null;
    }

    public static Vector2? IbikiFeet =>
        Site is KonohaSite site
            ? new Vector2((site.X(Design.IbikiSpot.Dx) + 0.5f) * 16f, (site.Y(Design.IbikiSpot.Dy) + 1) * 16f)
            : null;

    // World position of the Third Hokage's feet in his office.
    public static Vector2? HokageFeet =>
        Site is KonohaSite site
            ? new Vector2((site.X(Design.HokageSpot.Dx) + 0.5f) * 16f, (site.Y(Design.HokageSpot.Dy) + 1) * 16f)
            : null;

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
        progress.Message = Loc.Get("WorldGen.Konoha");
        Site = KonohaBuilder.Build(Main.spawnTileX);
        BuiltVersion = KonohaDesign.Version;
        ExamSiteWorld.Generate(progress);
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
