using System;
using System.Collections.Generic;

namespace ShinobiPrototype.Common;

// The Chūnin Exam landmarks (specs/M2_中忍考试篇.spec.md, section 4), built like the Hidden Leaf (KonohaDesign) and
// independent of Terraria so they can be tested and rendered: the gate of Training Ground 44 at the jungle's edge,
// the central tower deep in the jungle, the landmarks on the way between them, and the finals stadium outside the
// village wall. Positions are
// (dx, dy) from the site's centre on its ground row (0 = the ground row, negative = above).
public enum ExamSiteKind : byte { Gate, Tower, Stadium, HollowTree, Marker }

public sealed class ExamSiteDesign
{
    public const int Version = 2;

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
        ExamSiteKind.HollowTree => HollowTree(),
        ExamSiteKind.Marker => Marker(),
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

    // Training Ground 44 (v2, user 2026-10-01: the gate was too small). Drawn with the forest to the east (+x): a tall
    // timber gate over the path, a chain-link fence running some forty tiles off each way, and on the village side
    // the proctor's hut with the rules board, where Mitarashi Anko hands out the scrolls. Mirrored when the forest
    // lies to the west.
    public const int GatePostInner = 5;
    public const int GateHeight = 20;
    public const int FenceHalf = 44;
    public const int HutX0 = -31, HutX1 = -21;
    public const int AnkoDx = -18;

    private static ExamSiteDesign Gate()
    {
        ExamSiteDesign d = new(ExamSiteKind.Gate, FenceHalf + 2, 16, GateHeight + 8);
        d.Ground(KMat.JungleGrass, KMat.Mud);
        foreach (int side in new[] { -1, 1 })
        {
            for (int i = 0; i < 2; i++)
                for (int y = -1; y >= -GateHeight; y--)
                    d.Set(side * (GatePostInner + i), y, KMat.Beam);
            // The fence, from just outside the posts outward; a post every six tiles.
            for (int x = GatePostInner + 2; x <= FenceHalf; x++)
                for (int y = -1; y >= -10; y--)
                    d.SetWall(side * x, y, KWall.MetalFence);
            for (int x = GatePostInner + 5; x <= FenceHalf; x += 6)
                for (int y = -1; y >= -11; y--)
                    d.SetWall(side * x, y, KWall.Mahogany);
        }
        // Two crossbeams, the upper one wider, under a tiled roof.
        int lower = GatePostInner + 2, upper = GatePostInner + 4;
        for (int x = -lower; x <= lower; x++)
            d.Set(x, -GateHeight + 3, KMat.RichMahogany);
        for (int x = -upper; x <= upper; x++)
            d.Set(x, -GateHeight - 1, KMat.RichMahogany);
        d.Roof(-upper - 1, upper + 1, -GateHeight - 2, KMat.RedShingle, 3);
        // Banners hang from the lower crossbeam.
        d.Places.Add(new KPlace(-3, -GateHeight + 4, KFix.Banner, Style: 0));
        d.Places.Add(new KPlace(2, -GateHeight + 4, KFix.Banner, Style: 0));
        d.Places.Add(new KPlace(-GatePostInner - 4, -1, KFix.Sign, "第四十四演习场\n——死亡森林"));
        d.Places.Add(new KPlace(GatePostInner + 3, -1, KFix.Sign, "危险·禁止入内\n中忍考试第二试进行中"));
        // The proctor's hut, door facing the gate.
        for (int y = -1; y >= -6; y--)
        {
            d.Set(HutX0, y, KMat.Wood);
            d.Set(HutX1, y, KMat.Wood);
            for (int x = HutX0 + 1; x < HutX1; x++)
                d.SetWall(x, y, KWall.Planks);
        }
        for (int x = HutX0; x <= HutX1; x++)
            d.Set(x, -7, KMat.Wood);
        d.Roof(HutX0 - 1, HutX1 + 1, -8, KMat.RedShingle, 3);
        // Doors both ways, so the path along the ground stays open.
        d.Door(HutX1, 0);
        d.Door(HutX0, 0);
        d.Places.Add(new KPlace(HutX0 + 3, -1, KFix.Table));
        d.Places.Add(new KPlace(HutX0 + 5, -1, KFix.Chair));
        d.Places.Add(new KPlace(HutX0 + 6, -6, KFix.Lantern));
        d.Places.Add(new KPlace(HutX1 + 6, -1, KFix.Sign,
            "第二试　生存演习\n一、领取天之卷或地之卷，夺取另一卷\n二、带齐两卷进入中央塔\n三、不限时，生死自负"));
        d.Buildings.Add(new KBuilding("第四十四演习场入口", -upper - 1, upper + 1, -GateHeight - 4));
        d.Buildings.Add(new KBuilding("监考小屋", HutX0, HutX1, -10));
        return d;
    }

    // The central tower (v2, user 2026-10-01: too small), after the anime: a great stone hall on the ground floor,
    // where the prelims are fought, with watching galleries along both walls and the giant hands of the Ram seal
    // standing at its middle; two gallery floors above it, a narrower keep above those, and a roofed lookout on top.
    // Drawn with the gate to the west (-x): Gekkō Hayate waits on that side of the hall.
    public const int TowerHalf = 29;           // outer faces of the tower walls
    public const int HallHeight = 22;          // rows of open hall above its floor
    public const int KeepHalf = 18;
    public const int HayateDx = -TowerHalf + 9;

    private static ExamSiteDesign Tower()
    {
        ExamSiteDesign d = new(ExamSiteKind.Tower, TowerHalf + 6, 24, 66);
        d.Ground(KMat.JungleGrass, KMat.Mud);
        int x0 = -TowerHalf, x1 = TowerHalf;
        for (int x = x0 + 1; x < x1; x++)
            d.Set(x, 0, KMat.Slab);
        int hallCeiling = -HallHeight - 1;
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
        d.Places.RemoveAll(p => p.Fix == KFix.Door && (p.Dx == x0 + 1 || p.Dx == x1 - 1));
        d.Arena = new KRoom(x0 + 2, x1 - 2, hallCeiling + 1, -1, "中央塔大厅");

        // Galleries along both walls, railed, with steps up to them.
        const int gallery = -12;
        foreach (int side in new[] { -1, 1 })
        {
            int wall = side < 0 ? x0 + 2 : x1 - 2;
            for (int i = 0; i < 8; i++)
            {
                d.Set(wall - side * i, gallery, KMat.Platform);
                d.SetWall(wall - side * i, gallery - 1, KWall.MetalFence);
                d.SetWall(wall - side * i, gallery - 2, KWall.MetalFence);
            }
            foreach ((int row, int width) in new[] { (-4, 3), (-8, 3) })
                for (int i = 0; i < width; i++)
                    d.Set(wall - side * (i + (row == -8 ? 4 : 0)), row, KMat.Platform);
            // A banner over each gallery, hanging from the hall ceiling.
            d.Places.Add(new KPlace(wall - side * 4, -HallHeight, KFix.Banner, Style: side < 0 ? 0 : 1));
        }
        // The Ram seal: two great white stone hands, palms together, index and middle fingers raised, on a dark plinth,
        // in the background (marble against the hall's grey slabs, so it stands out).
        foreach (int side in new[] { -1, 1 })
        {
            for (int y = -4; y >= -13; y--)
                for (int i = 1; i <= 5; i++)
                    d.SetWall(side * i, y, i == 5 || y == -4 ? KWall.Brick : KWall.Marble);
            for (int y = -14; y >= -21; y--)
                for (int i = 1; i <= 3; i++)
                    d.SetWall(side * i, y, i == 3 || y == -21 ? KWall.Brick : KWall.Marble);
            // The folded fingers across the back of the hand.
            for (int i = 1; i <= 5; i++)
                d.SetWall(side * i, -10, KWall.Brick);
        }
        for (int y = -4; y >= -21; y--)
            d.SetWall(0, y, KWall.Brick);
        for (int x = -8; x <= 8; x++)
            for (int y = -1; y >= -3; y--)
                d.SetWall(x, y, y == -3 || System.Math.Abs(x) == 8 ? KWall.Brick : KWall.RedBrick);
        foreach (int x in new[] { -18, -6, 6, 18 })
            d.Places.Add(new KPlace(x, hallCeiling + 1, KFix.Lantern));

        // Two gallery floors above the hall, then the keep.
        int floor = hallCeiling;
        for (int s = 0; s < 4; s++)
        {
            int half = s < 2 ? TowerHalf - 1 : KeepHalf;
            int ceiling = floor - KonohaDesign.StoryHeight;
            for (int x = -half; x <= half; x++)
                d.Set(x, ceiling, KMat.Brick);
            for (int y = floor - 1; y > ceiling; y--)
            {
                d.Set(-half, y, KMat.Brick);
                d.Set(half, y, KMat.Brick);
                for (int x = -half + 1; x <= half - 1; x++)
                    d.SetWall(x, y, KWall.Brick);
            }
            // Ways up: platforms through each floor, alternating sides.
            int up = s % 2 == 0 ? -half + 2 : half - 4;
            for (int i = 0; i < 3; i++)
            {
                d.Set(up + i, floor, KMat.Platform);
                if (s < 3)
                    d.Set(up + i, ceiling, KMat.Platform);
            }
            d.Places.Add(new KPlace(0, ceiling + 1, KFix.Lantern));
            if (s == 1)
                // Battlements on the gallery roof outside the keep.
                for (int x = -TowerHalf + 1; x <= TowerHalf - 1; x += 2)
                    if (Math.Abs(x) > KeepHalf)
                        d.Set(x, ceiling - 1, KMat.Brick);
            floor = ceiling;
        }
        for (int x = -KeepHalf; x <= KeepHalf; x += 2)
            d.Set(x, floor - 1, KMat.Brick);
        for (int y = floor - 1; y >= floor - 4; y--)
        {
            d.Set(-6, y, KMat.Beam);
            d.Set(6, y, KMat.Beam);
        }
        d.Roof(-8, 8, floor - 5, KMat.RedShingle, 4);
        d.Places.Add(new KPlace(x0 + 4, -1, KFix.Sign,
            "天无智慧，则当求知以备之；\n地无体力，则当奔走以求之。\n天地双开，则险道亦成正道。"));
        d.Places.Add(new KPlace(x0 - 4, -1, KFix.Sign, "中央塔\n——带齐天、地两卷入内"));
        d.Buildings.Add(new KBuilding("中央塔", x0, x1, floor - 9));
        return d;
    }

    // A giant hollow tree: walk in through either side; a genin's stash in the hollow.
    private static ExamSiteDesign HollowTree()
    {
        ExamSiteDesign d = new(ExamSiteKind.HollowTree, 12, 10, 44);
        d.Ground(KMat.JungleGrass, KMat.Mud);
        const int trunkHalf = 5, top = -30;
        for (int y = -1; y >= top; y--)
            for (int x = -trunkHalf; x <= trunkHalf; x++)
            {
                bool hollow = Math.Abs(x) <= trunkHalf - 2 && y >= -7;
                bool doorway = Math.Abs(x) >= trunkHalf - 1 && y >= -3;
                if (hollow || doorway)
                    d.SetWall(x, y, KWall.LivingWood);
                else
                    d.Set(x, y, KMat.LivingWood);
            }
        // Roots spreading out at the foot, behind the path.
        foreach (int side in new[] { -1, 1 })
            for (int i = 1; i <= 3; i++)
                for (int y = -1; y >= -4 + i; y--)
                    d.SetWall(side * (trunkHalf + i), y, KWall.LivingWood);
        // The crown.
        for (int y = top - 1; y >= top - 9; y--)
        {
            int half = 11 - Math.Abs(y - (top - 5)) * 2;
            for (int x = -half; x <= half; x++)
                d.Set(x, y, KMat.Leaf);
        }
        d.Places.Add(new KPlace(0, -1, KFix.Chest));
        d.Buildings.Add(new KBuilding("空心巨树", -trunkHalf, trunkHalf, top - 10));
        return d;
    }

    // A warning sign by the way to the tower.
    private static ExamSiteDesign Marker()
    {
        ExamSiteDesign d = new(ExamSiteKind.Marker, 1, 3, 6);
        d.Ground(KMat.JungleGrass, KMat.Mud);
        d.Places.Add(new KPlace(0, -1, KFix.Sign, "危险\n禁止入内——第四十四演习场"));
        return d;
    }

    // The design flipped east to west (the forest beyond the gate lies west of it).
    public ExamSiteDesign Mirrored()
    {
        ExamSiteDesign m = new(Kind, HalfWidth, Blend, ClearHeight);
        foreach (KCell c in Cells)
            m.Set(-c.Dx, c.Dy, c.Mat, c.Shape switch
            {
                KShape.TopRisesEast => KShape.TopRisesWest,
                KShape.TopRisesWest => KShape.TopRisesEast,
                _ => c.Shape,
            });
        foreach (KWallCell w in Walls)
            m.SetWall(-w.Dx, w.Dy, w.Wall);
        foreach (KPlace place in Places)
            m.Places.Add(place with { Dx = -place.Dx - FixtureWidth(place.Fix) + 1 });
        foreach (KBuilding b in Buildings)
            m.Buildings.Add(b with { X0 = -b.X1, X1 = -b.X0 });
        m.Arena = Arena with { X0 = -Arena.X1, X1 = -Arena.X0 };
        return m;
    }

    // Signs and chests are placed by their west column, so a mirrored one shifts by its width to keep its footprint;
    // tables, benches and campfires are anchored at their middle, everything else is one tile wide.
    private static int FixtureWidth(KFix fix) => fix is KFix.Sign or KFix.Chest ? 2 : 1;

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
