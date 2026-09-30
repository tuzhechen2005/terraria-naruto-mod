using System;
using System.Collections.Generic;

namespace ShinobiPrototype.Common;

// The Chūnin Exam landmarks (specs/M2_中忍考试篇.spec.md, section 4), built like the Hidden Leaf (KonohaDesign) and
// independent of Terraria so they can be tested and rendered: the gate of Training Ground 44 at the jungle's edge,
// the central tower in the middle of the jungle, and the finals stadium outside the village wall. Positions are
// (dx, dy) from the site's centre on its ground row (0 = the ground row, negative = above).
public enum ExamSiteKind : byte { Gate, Tower, Stadium }

public sealed class ExamSiteDesign
{
    public const int Version = 1;

    public ExamSiteKind Kind { get; }
    public int HalfWidth { get; }
    public int Blend { get; }                 // terrain slopes back to nature over this many tiles past HalfWidth
    public int ClearHeight { get; }
    public int FoundationDepth => 8;
    public bool Jungle => Kind != ExamSiteKind.Stadium;

    // The tower's hall and the stadium's field (inclusive): where the prelims and finals are fought, and where
    // arriving with both scrolls counts. Empty for the gate.
    public KRoom Arena { get; private set; }

    private readonly Dictionary<(int, int), KCell> cells = new();
    private readonly Dictionary<(int, int), KWall> walls = new();
    public readonly List<KPlace> Places = new();
    public readonly List<KBuilding> Buildings = new();

    public IEnumerable<KCell> Cells => cells.Values;
    public IEnumerable<KWallCell> Walls
    {
        get
        {
            foreach (var ((x, y), wall) in walls)
                yield return new KWallCell(x, y, wall);
        }
    }

    public KCell? CellAt(int dx, int dy) => cells.TryGetValue((dx, dy), out KCell c) ? c : null;
    public KWall? WallAt(int dx, int dy) => walls.TryGetValue((dx, dy), out KWall w) ? w : null;

    // Solid to walk into: everything but beams (drawn in front of the street) and platforms.
    public bool Blocks(int dx, int dy) => CellAt(dx, dy) is KCell c && c.Mat is not (KMat.Beam or KMat.Platform);

    private ExamSiteDesign(ExamSiteKind kind, int halfWidth, int blend, int clearHeight)
    {
        Kind = kind;
        HalfWidth = halfWidth;
        Blend = blend;
        ClearHeight = clearHeight;
    }

    private void Set(int dx, int dy, KMat mat, KShape shape = KShape.Full) => cells[(dx, dy)] = new KCell(dx, dy, mat, shape);
    private void Clear(int dx, int dy) => cells.Remove((dx, dy));
    private void SetWall(int dx, int dy, KWall wall) => walls[(dx, dy)] = wall;

    public static ExamSiteDesign Create(ExamSiteKind kind) => kind switch
    {
        ExamSiteKind.Gate => Gate(),
        ExamSiteKind.Tower => Tower(),
        _ => Stadium(),
    };

    private void Ground(KMat top, KMat under)
    {
        for (int x = -HalfWidth; x <= HalfWidth; x++)
        {
            Set(x, 0, top);
            for (int y = 1; y <= FoundationDepth; y++)
                Set(x, y, under);
        }
    }

    private void Roof(int x0, int x1, int row, KMat mat, int rows)
    {
        for (int i = 0; i < rows && x0 + i <= x1 - i; i++)
        {
            int y = row - i;
            for (int x = x0 + i; x <= x1 - i; x++)
                Set(x, y, mat);
            Set(x0 + i, y, mat, KShape.TopRisesEast);
            Set(x1 - i, y, mat, KShape.TopRisesWest);
        }
    }

    private void Door(int x, int floorRow)
    {
        for (int y = floorRow - 1; y >= floorRow - 3; y--)
            Clear(x, y);
        Places.Add(new KPlace(x, floorRow - 1, KFix.Door));
    }

    // Training Ground 44: a chain-link fence running off into the forest on both sides, and a timber gate over the
    // path with the warning signs. The path itself stays open (the posts are beams).
    public const int GatePostInner = 4;
    public const int GateHeight = 10;

    private static ExamSiteDesign Gate()
    {
        ExamSiteDesign d = new(ExamSiteKind.Gate, 18, 16, 30);
        d.Ground(KMat.JungleGrass, KMat.Mud);
        foreach (int side in new[] { -1, 1 })
        {
            for (int i = 0; i < 2; i++)
                for (int y = -1; y >= -GateHeight; y--)
                    d.Set(side * (GatePostInner + i), y, KMat.Beam);
            // The fence, from just outside the posts to the edge of the site.
            for (int x = GatePostInner + 2; x <= d.HalfWidth; x++)
                for (int y = -1; y >= -6; y--)
                    d.SetWall(side * x, y, KWall.MetalFence);
            // Fence posts every few tiles, in the background too.
            for (int x = GatePostInner + 4; x <= d.HalfWidth; x += 5)
                for (int y = -1; y >= -7; y--)
                    d.SetWall(side * x, y, KWall.Mahogany);
        }
        // The two gate leaves, swung open behind the path.
        for (int x = -GatePostInner + 2; x <= GatePostInner - 2; x++)
            for (int y = -1; y >= -GateHeight + 2; y--)
                d.SetWall(x, y, KWall.Mahogany);
        int beamHalf = GatePostInner + 3;
        for (int x = -beamHalf; x <= beamHalf; x++)
            d.Set(x, -GateHeight - 1, KMat.RichMahogany);
        d.Roof(-beamHalf - 1, beamHalf + 1, -GateHeight - 2, KMat.RedShingle, 2);
        d.Places.Add(new KPlace(-GatePostInner - 3, -1, KFix.Sign, "第四十四演习场\n——死亡森林"));
        d.Places.Add(new KPlace(GatePostInner + 3, -1, KFix.Sign, "禁止入内\n中忍考试第二试进行中"));
        d.Places.Add(new KPlace(-2, -GateHeight, KFix.Banner, Style: 0));
        d.Places.Add(new KPlace(2, -GateHeight, KFix.Banner, Style: 0));
        d.Buildings.Add(new KBuilding("第四十四演习场入口", -beamHalf - 1, beamHalf + 1, -GateHeight - 3));
        return d;
    }

    // The tower in the middle of the Forest of Death: a tall stone keep. The ground floor is one great hall (the
    // prelims are fought there), with the scroll verse on a sign by the entrance; two floors of rooms above.
    public const int TowerHalf = 17;           // outer faces of the tower walls
    public const int HallHeight = 14;          // rows of open hall above its floor

    private static ExamSiteDesign Tower()
    {
        ExamSiteDesign d = new(ExamSiteKind.Tower, TowerHalf + 6, 20, 50);
        d.Ground(KMat.JungleGrass, KMat.Mud);
        int x0 = -TowerHalf, x1 = TowerHalf;
        for (int x = x0 + 1; x < x1; x++)
            d.Set(x, 0, KMat.Slab);
        int hallCeiling = -HallHeight - 1;
        // Walls of the hall, two tiles thick, doors at the bottom of each side.
        for (int y = -1; y > hallCeiling; y--)
            foreach (int x in new[] { x0, x0 + 1, x1 - 1, x1 })
                d.Set(x, y, KMat.Brick);
        for (int x = x0; x <= x1; x++)
            d.Set(x, hallCeiling, KMat.Brick);
        for (int y = -1; y > hallCeiling; y--)
            for (int x = x0 + 2; x <= x1 - 2; x++)
                d.SetWall(x, y, KWall.Slab);
        foreach (int x in new[] { x0, x0 + 1, x1 - 1, x1 })
            d.Door(x, 0);
        // Only the outer tile of each doorway carries a door: clear the inner one to an open arch.
        d.Places.RemoveAll(p => p.Fix == KFix.Door && (p.Dx == x0 + 1 || p.Dx == x1 - 1));
        d.Arena = new KRoom(x0 + 2, x1 - 2, hallCeiling + 1, -1, "中央塔大厅");
        // Platforms up through the hall ceiling, one stack each side, to the floors above.
        foreach (int x in new[] { x0 + 3, x1 - 5 })
            for (int i = 0; i < 3; i++)
                d.Set(x + i, hallCeiling, KMat.Platform);
        // Two floors above the hall.
        int floor = hallCeiling;
        for (int s = 0; s < 2; s++)
        {
            int ceiling = floor - KonohaDesign.StoryHeight;
            for (int x = x0 + 1; x <= x1 - 1; x++)
                d.Set(x, ceiling, KMat.Brick);
            for (int y = floor - 1; y > ceiling; y--)
            {
                d.Set(x0 + 1, y, KMat.Brick);
                d.Set(x1 - 1, y, KMat.Brick);
                for (int x = x0 + 2; x <= x1 - 2; x++)
                    d.SetWall(x, y, KWall.Brick);
            }
            if (s == 0)
                foreach (int x in new[] { x0 + 3, x1 - 5 })
                    for (int i = 0; i < 3; i++)
                        d.Set(x + i, ceiling, KMat.Platform);
            d.Places.Add(new KPlace(0, ceiling + 1, KFix.Lantern));
            floor = ceiling;
        }
        // Battlements, and a small roofed lookout in the middle.
        for (int x = x0 + 1; x <= x1 - 1; x += 2)
            d.Set(x, floor - 1, KMat.Brick);
        for (int x = -4; x <= 4; x++)
            for (int y = floor - 1; y >= floor - 4; y--)
                if (Math.Abs(x) == 4)
                    d.Set(x, y, KMat.Beam);
        d.Roof(-6, 6, floor - 5, KMat.RedShingle, 3);
        // Lanterns along the hall ceiling, the verse by the entrance, banners.
        foreach (int x in new[] { -10, 0, 10 })
            d.Places.Add(new KPlace(x, hallCeiling + 1, KFix.Lantern));
        d.Places.Add(new KPlace(x0 + 4, -1, KFix.Sign,
            "天无智慧，则当求知以备之；\n地无体力，则当奔走以求之。\n天地双开，则险道亦成正道。"));
        d.Places.Add(new KPlace(-6, hallCeiling + 1, KFix.Banner, Style: 1));
        d.Places.Add(new KPlace(6, hallCeiling + 1, KFix.Banner, Style: 1));
        d.Places.Add(new KPlace(x1 + 3, -1, KFix.Sign, "中央塔\n——带齐天、地两卷入内"));
        d.Buildings.Add(new KBuilding("中央塔", x0, x1, floor - 7));
        return d;
    }

    // The finals stadium: an open sand-coloured field between tiers of stands. The stands are platforms (the field
    // is reached by walking straight in along the ground), the outer shell stone above head height, and the far side
    // of the ring painted behind the field.
    public const int FieldHalf = 24;
    public const int StadiumHalf = 36;
    public const int StandRows = 5;

    private static ExamSiteDesign Stadium()
    {
        ExamSiteDesign d = new(ExamSiteKind.Stadium, StadiumHalf + 2, 20, 45);
        d.Ground(KMat.Grass, KMat.Dirt);
        for (int x = -StadiumHalf; x <= StadiumHalf; x++)
            d.Set(x, 0, Math.Abs(x) <= FieldHalf ? KMat.Stucco : KMat.Slab);
        d.Arena = new KRoom(-FieldHalf, FieldHalf, -d.ClearHeight + 2, -1, "考试会场");
        foreach (int side in new[] { -1, 1 })
        {
            // Stands: each row two tiles higher and two tiles further out.
            for (int r = 0; r < StandRows; r++)
            {
                int row = -4 - 2 * r;
                for (int i = FieldHalf + 1 + 2 * r; i <= StadiumHalf - 1; i++)
                    d.Set(side * i, row, KMat.Platform);
                for (int y = row + 1; y <= -1; y++)
                    for (int i = FieldHalf + 1 + 2 * r; i <= FieldHalf + 2 + 2 * r; i++)
                        d.SetWall(side * i, y, KWall.Stucco);
            }
            // The outer shell: beams at the entrance, stone above head height up to the top of the stands.
            int shell = side * StadiumHalf;
            for (int y = -1; y >= -4; y--)
                d.Set(shell, y, KMat.Beam);
            int top = -4 - 2 * StandRows;
            for (int y = -5; y >= top; y--)
            {
                d.Set(shell, y, KMat.Stucco);
                d.Set(shell + side, y, KMat.Stucco);
            }
            d.Set(shell, top - 1, KMat.RedShingle, side < 0 ? KShape.TopRisesEast : KShape.TopRisesWest);
            d.Places.Add(new KPlace(side * (FieldHalf - 2), -1, KFix.LampPost));
            // A banner at each entrance, hanging from the shell's outer stone above the way in.
            d.Places.Add(new KPlace(shell + side, -4, KFix.Banner, Style: side < 0 ? 0 : 1));
        }
        // The far side of the ring behind the field: a wall of stands in the background.
        for (int x = -FieldHalf; x <= FieldHalf; x++)
            for (int y = -1; y >= -12; y--)
                d.SetWall(x, y, y % 3 == 0 ? KWall.Slab : KWall.Stucco);
        d.Places.Add(new KPlace(-StadiumHalf - 4, -1, KFix.Sign, "中忍考试 · 正式赛会场"));
        d.Buildings.Add(new KBuilding("考试会场", -StadiumHalf - 1, StadiumHalf + 1, -4 - 2 * StandRows - 1));
        return d;
    }
}
