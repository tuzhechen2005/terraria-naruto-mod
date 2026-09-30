using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace ShinobiPrototype.Common.Systems;

// Where one of the Chūnin Exam landmarks stands (ExamSiteDesign): its centre and ground row.
public readonly record struct ExamSite(ExamSiteKind Kind, int CenterX, int GroundY)
{
    // The builder's placement helpers take the village's origin type.
    public KonohaSite Origin => new(CenterX, GroundY);

    public float DistanceTiles(Vector2 worldPosition) =>
        Vector2.Distance(worldPosition / 16f, new Vector2(CenterX + 0.5f, GroundY - 2f));

    public Rectangle ArenaWorld()
    {
        KRoom room = ExamSiteWorld.Design(Kind).Arena;
        return new Rectangle((CenterX + room.X0) * 16, (GroundY + room.Top) * 16, room.Width * 16, room.Height * 16);
    }
}

// The exam landmarks of a new world: the Training Ground 44 gate and the tower in the jungle, the finals stadium
// outside the Leaf's wall. Built at world generation after the village; worlds without the village have none.
public sealed class ExamSiteWorld : ModSystem
{
    public static ExamSite? Gate { get; private set; }
    public static ExamSite? Tower { get; private set; }
    public static ExamSite? Stadium { get; private set; }
    public static int BuiltVersion { get; private set; }

    private static readonly Dictionary<ExamSiteKind, ExamSiteDesign> designs = new();

    public static ExamSiteDesign Design(ExamSiteKind kind)
    {
        if (!designs.TryGetValue(kind, out ExamSiteDesign design))
            designs[kind] = design = ExamSiteDesign.Create(kind);
        return design;
    }

    public static bool InArena(ExamSite? site, Vector2 worldPosition) =>
        site is ExamSite s && s.ArenaWorld().Contains(worldPosition.ToPoint());

    public static IEnumerable<ExamSite> All()
    {
        foreach (ExamSite? site in new[] { Gate, Tower, Stadium })
            if (site is ExamSite s)
                yield return s;
    }

    // "(东边约 350 格)" for objective texts.
    public static string Hint(ExamSite? site, Player player)
    {
        if (site is not ExamSite s)
            return "";
        int dx = s.CenterX - (int)(player.Center.X / 16f);
        if (Math.Abs(dx) < 20)
            return "（就在附近）";
        return $"（{(dx < 0 ? "西" : "东")}边约 {Math.Abs(dx) / 10 * 10} 格）";
    }

    public static string GateHint(Player player) => Hint(Gate, player);
    public static string TowerHint(Player player) => Hint(Tower, player);
    public static string StadiumHint(Player player) => Hint(Stadium, player);

    public override void ClearWorld()
    {
        Gate = Tower = Stadium = null;
        BuiltVersion = 0;
    }

    // Called by the village's world generation pass once the village stands (the stadium is placed beside it).
    internal static void Generate(GenerationProgress progress)
    {
        if (KonohaWorld.Site is not KonohaSite village)
            return;
        progress.Message = "布置中忍考试的场地";
        (int jungleWest, int jungleEast)? jungle = ExamSiteBuilder.JungleSurface();
        if (jungle is (int west, int east))
        {
            // The gate on the jungle's side facing the village, the tower in its middle.
            bool eastOfVillage = (west + east) / 2 > village.CenterX;
            int inset = Design(ExamSiteKind.Gate).HalfWidth + 6;
            int gateX = eastOfVillage ? west + inset : east - inset;
            int towerX = (west + east) / 2;
            if (Math.Abs(towerX - gateX) < 60)
                towerX = gateX + (eastOfVillage ? 60 : -60);
            Gate = ExamSiteBuilder.Build(ExamSiteKind.Gate, gateX);
            Tower = ExamSiteBuilder.Build(ExamSiteKind.Tower, towerX);
            // The stadium on the village's other side, clear of the forest.
            int side = eastOfVillage ? -1 : 1;
            ExamSiteDesign stadium = Design(ExamSiteKind.Stadium);
            int offset = KonohaDesign.HalfWidth + KonohaDesign.Blend + stadium.Blend + stadium.HalfWidth + 4;
            Stadium = ExamSiteBuilder.Build(ExamSiteKind.Stadium, village.CenterX + side * offset);
        }
        else
        {
            int offset = KonohaDesign.HalfWidth + KonohaDesign.Blend + Design(ExamSiteKind.Stadium).Blend +
                         Design(ExamSiteKind.Stadium).HalfWidth + 4;
            Stadium = ExamSiteBuilder.Build(ExamSiteKind.Stadium, village.CenterX + offset);
        }
        BuiltVersion = ExamSiteDesign.Version;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        foreach (ExamSite site in All())
            tag[$"examSite{site.Kind}"] = new[] { site.CenterX, site.GroundY };
        if (BuiltVersion > 0)
            tag["examSiteVersion"] = BuiltVersion;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        Gate = Load(tag, ExamSiteKind.Gate);
        Tower = Load(tag, ExamSiteKind.Tower);
        Stadium = Load(tag, ExamSiteKind.Stadium);
        BuiltVersion = tag.GetInt("examSiteVersion");
    }

    private static ExamSite? Load(TagCompound tag, ExamSiteKind kind) =>
        tag.GetIntArray($"examSite{kind}") is { Length: 2 } xy ? new ExamSite(kind, xy[0], xy[1]) : null;

    public override void NetSend(BinaryWriter writer)
    {
        foreach (ExamSite? site in new[] { Gate, Tower, Stadium })
        {
            writer.Write(site.HasValue);
            if (site is ExamSite s)
            {
                writer.Write(s.CenterX);
                writer.Write(s.GroundY);
            }
        }
    }

    public override void NetReceive(BinaryReader reader)
    {
        Gate = Read(reader, ExamSiteKind.Gate);
        Tower = Read(reader, ExamSiteKind.Tower);
        Stadium = Read(reader, ExamSiteKind.Stadium);
    }

    private static ExamSite? Read(BinaryReader reader, ExamSiteKind kind) =>
        reader.ReadBoolean() ? new ExamSite(kind, reader.ReadInt32(), reader.ReadInt32()) : null;
}
