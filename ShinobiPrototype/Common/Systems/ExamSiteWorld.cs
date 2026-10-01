using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace ShinobiPrototype.Common.Systems;

// Where one of the Chūnin Exam landmarks stands (ExamSiteDesign): its centre and ground row, and which way the forest
// runs from the gate to the tower (+1 east, as the designs are drawn; -1 west, mirrored).
public readonly record struct ExamSite(ExamSiteKind Kind, int CenterX, int GroundY, int Dir = 1)
{
    // The builder's placement helpers take the village's origin type.
    public KonohaSite Origin => new(CenterX, GroundY);

    public float DistanceTiles(Vector2 worldPosition) =>
        Vector2.Distance(worldPosition / 16f, new Vector2(CenterX + 0.5f, GroundY - 2f));

    public Rectangle ArenaWorld()
    {
        KRoom room = ExamSiteWorld.Design(Kind, Dir).Arena;
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
    public static ExamSite? HollowTree { get; private set; }
    private static ExamSite? oldCamp;
    public static int BuiltVersion { get; private set; }

    private static readonly Dictionary<(ExamSiteKind, int), ExamSiteDesign> designs = new();

    public static ExamSiteDesign Design(ExamSiteKind kind, int dir = 1)
    {
        dir = dir < 0 ? -1 : 1;
        if (!designs.TryGetValue((kind, dir), out ExamSiteDesign design))
            designs[(kind, dir)] = design = dir < 0 ? ExamSiteDesign.Create(kind).Mirrored() : ExamSiteDesign.Create(kind);
        return design;
    }

    // Where the story people stand and the forest's fixed encounters happen (specs/M2_中忍考试篇.spec.md 3.2).
    public static Vector2? AnkoFeet => Gate is ExamSite g ? Feet(g, ExamSiteDesign.AnkoDx) : null;
    public static Vector2? HayateFeet => Tower is ExamSite t ? Feet(t, ExamSiteDesign.HayateDx) : null;
    // The first squad lies in wait a little way past the gate; the Rain genin in the clearing before the tower.
    public static Vector2? GateAmbushSpot => Gate is ExamSite g ? Feet(g, ChuninExamRules.GateAmbushTiles) : null;
    public static Vector2? RainClearing => Tower is ExamSite t
        ? Feet(t, -(ExamSiteDesign.TowerHalf + ChuninExamRules.RainClearingTiles)) : null;

    private static Vector2 Feet(ExamSite site, int dx) => new((site.CenterX + site.Dir * dx + 0.5f) * 16f, site.GroundY * 16f);

    public static bool InArena(ExamSite? site, Vector2 worldPosition) =>
        site is ExamSite s && s.ArenaWorld().Contains(worldPosition.ToPoint());

    public static IEnumerable<ExamSite> All()
    {
        foreach (ExamSite? site in new[] { Gate, Tower, Stadium, HollowTree })
            if (site is ExamSite s)
                yield return s;
    }

    // "（在东边）" for objective texts: a rough direction, as vanilla would give it (master spec, "可玩性与引导");
    // the exact distance is the quest tracker's, when the player turns it on.
    public static string Hint(ExamSite? site, Player player)
    {
        if (site is not ExamSite s)
            return "";
        int dx = s.CenterX - (int)(player.Center.X / 16f);
        return Math.Abs(dx) < 20 ? "（就在附近）" : $"（在{(dx < 0 ? "西" : "东")}边）";
    }

    public static Vector2? Where(ExamSite? site) =>
        site is ExamSite s ? new Vector2((s.CenterX + 0.5f) * 16f, s.GroundY * 16f) : null;

    public static string GateHint(Player player) => Hint(Gate, player);
    public static string TowerHint(Player player) => Hint(Tower, player);
    public static string StadiumHint(Player player) => Hint(Stadium, player);

    public override void ClearWorld()
    {
        Gate = Tower = Stadium = HollowTree = null;
        oldCamp = null;
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
            bool eastOfVillage = (west + east) / 2 > village.CenterX;
            BuildForest(west, east, eastOfVillage ? 1 : -1);
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

    // The forest of Training Ground 44 (v2, user 2026-10-01): the gate on the jungle's side facing the village, the
    // tower as deep in as the jungle allows and well clear of the gate, and on the way between them the giant tree and
    // warning signs pointing on to the tower.
    internal static void BuildForest(int west, int east, int dir)
    {
        int gateInset = Design(ExamSiteKind.Gate).HalfWidth + 6;
        int towerInset = Design(ExamSiteKind.Tower).HalfWidth + 6;
        int gateX = dir > 0 ? west + gateInset : east - gateInset;
        int towerX = dir > 0 ? east - towerInset : west + towerInset;
        int least = Design(ExamSiteKind.Gate).HalfWidth + Design(ExamSiteKind.Tower).HalfWidth + 60;
        if ((towerX - gateX) * dir < least)
            towerX = gateX + dir * least;
        Gate = ExamSiteBuilder.Build(ExamSiteKind.Gate, gateX, dir);
        Tower = ExamSiteBuilder.Build(ExamSiteKind.Tower, towerX, dir);
        int length = (towerX - gateX) * dir;
        HollowTree = length >= 140 ? ExamSiteBuilder.Build(ExamSiteKind.HollowTree, gateX + dir * length * 3 / 10, dir) : null;
        // Warning signs every so often, clear of the other landmarks and the Rain genin's clearing.
        int rain = towerX - dir * (ExamSiteDesign.TowerHalf + ChuninExamRules.RainClearingTiles);
        for (int x = gateX + dir * (Design(ExamSiteKind.Gate).HalfWidth + 12); (rain - x) * dir > 12; x += dir * 30)
        {
            bool clear = true;
            foreach (ExamSite? s in new[] { HollowTree })
                if (s is ExamSite site && Math.Abs(site.CenterX - x) < Design(site.Kind).HalfWidth + 14)
                    clear = false;
            if (clear)
                ExamSiteBuilder.Build(ExamSiteKind.Marker, x, dir);
        }
    }

    // /m0 exam rebuild: knock down this world's forest landmarks (as far as the v1 or v2 designs reached) and build
    // them again in the current design.
    internal static bool RebuildForest()
    {
        if (ExamSiteBuilder.JungleSurface() is not (int west, int east) || KonohaWorld.Site is not KonohaSite village)
            return false;
        foreach (ExamSite? site in new[] { Gate, Tower, HollowTree, oldCamp })
            if (site is ExamSite s)
                ExamSiteBuilder.Erase(s, 50, 70);
        oldCamp = null;
        BuildForest(west, east, (west + east) / 2 > village.CenterX ? 1 : -1);
        BuiltVersion = ExamSiteDesign.Version;
        return true;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        foreach (ExamSite site in All())
            tag[$"examSite{site.Kind}"] = new[] { site.CenterX, site.GroundY, site.Dir };
        if (BuiltVersion > 0)
            tag["examSiteVersion"] = BuiltVersion;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        Gate = Load(tag, ExamSiteKind.Gate);
        Tower = Load(tag, ExamSiteKind.Tower);
        Stadium = Load(tag, ExamSiteKind.Stadium);
        // The rest point was dropped (user, 2026-10-01: too crude); remembered only so a rebuild clears it away.
        oldCamp = tag.GetIntArray("examSiteCamp") is { Length: 3 } c ? new ExamSite(ExamSiteKind.Marker, c[0], c[1], c[2]) : null;
        HollowTree = Load(tag, ExamSiteKind.HollowTree);
        BuiltVersion = tag.GetInt("examSiteVersion");
    }

    private static ExamSite? Load(TagCompound tag, ExamSiteKind kind) => tag.GetIntArray($"examSite{kind}") switch
    {
        { Length: 3 } v => new ExamSite(kind, v[0], v[1], v[2]),
        { Length: 2 } v => new ExamSite(kind, v[0], v[1]),
        _ => null,
    };

    public override void NetSend(BinaryWriter writer)
    {
        foreach (ExamSite? site in new[] { Gate, Tower, Stadium, HollowTree })
        {
            writer.Write(site.HasValue);
            if (site is ExamSite s)
            {
                writer.Write(s.CenterX);
                writer.Write(s.GroundY);
                writer.Write((sbyte)s.Dir);
            }
        }
    }

    public override void NetReceive(BinaryReader reader)
    {
        Gate = Read(reader, ExamSiteKind.Gate);
        Tower = Read(reader, ExamSiteKind.Tower);
        Stadium = Read(reader, ExamSiteKind.Stadium);
        HollowTree = Read(reader, ExamSiteKind.HollowTree);
    }

    private static ExamSite? Read(BinaryReader reader, ExamSiteKind kind) =>
        reader.ReadBoolean() ? new ExamSite(kind, reader.ReadInt32(), reader.ReadInt32(), reader.ReadSByte()) : null;
}
