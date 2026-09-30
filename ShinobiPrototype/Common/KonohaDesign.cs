using System;
using System.Collections.Generic;

namespace ShinobiPrototype.Common;

// The Hidden Leaf Village around the spawn point (M2a spec), independent of Terraria so it can be tested and rendered
// to a preview. Positions are (dx, dy): dx counts tiles east of the A-Un gate (the spawn point), dy is the row
// relative to the village ground (0 = the ground row itself, negative = above). KonohaBuilder maps the materials to
// vanilla tiles and places everything.
public enum KMat : byte
{
    Grass, Dirt, Slab, Brick, RedBrick, Stucco, Marble, DynastyWood, Wood, RedShingle, BlueShingle, Plating,
    Beam, Platform, LivingWood,
}

public enum KShape : byte { Full, Half, TopRisesEast, TopRisesWest }

public enum KWall : byte { Planks, Stucco, Shoji, Marble, RedBrick, Wood, Brick, Palm, RedStucco, Fence, DoorLeaf }

public enum KFix : byte { Door, Table, Chair, Lantern, LampPost, Sign, Banner, Tree, Bookcase, Bed }

public readonly record struct KCell(int Dx, int Dy, KMat Mat, KShape Shape = KShape.Full);

public readonly record struct KWallCell(int Dx, int Dy, KWall Wall);

// Dy is the bottom row of the fixture (the row resting on the floor) except lanterns and banners, which hang with
// their top at Dy. Text is for signs.
public readonly record struct KPlace(int Dx, int Dy, KFix Fix, string Text = "", int Style = 0);

// A home's interior (inclusive), and the building it belongs to.
public readonly record struct KRoom(int X0, int X1, int Top, int Bottom, string Building)
{
    public int Width => X1 - X0 + 1;
    public int Height => Bottom - Top + 1;
}

public readonly record struct KBuilding(string Name, int X0, int X1, int Top);

public sealed class KonohaDesign
{
    // Bump whenever the layout changes.
    public const int Version = 1;

    public const int HalfWidth = 199;          // outer faces of the village walls
    public const int Blend = 30;               // terrain slopes back to nature over this many tiles past the walls
    public const int ClearHeight = 70;         // open sky cleared above the ground
    public const int FoundationDepth = 12;     // solid dirt guaranteed under the ground row
    public const int StoryHeight = 6;          // floor row to floor row: five rows of room and one of floor
    public const int RoomHeight = 5;
    public const int PlazaHalf = 22;
    public const int GatePillarInner = 8;
    public const int GatePillarWidth = 3;
    public const int GateHeight = 18;
    public const int WallHeight = 22;
    public const int WallPassage = 5;
    public const int WallThickness = 6;

    private readonly Dictionary<(int, int), KCell> cells = new();
    private readonly Dictionary<(int, int), KWall> walls = new();
    public readonly List<KPlace> Places = new();
    public readonly List<KRoom> Rooms = new();
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

    public bool Has(int dx, int dy) => cells.ContainsKey((dx, dy));
    public KCell? CellAt(int dx, int dy) => cells.TryGetValue((dx, dy), out KCell c) ? c : null;
    public KWall? WallAt(int dx, int dy) => walls.TryGetValue((dx, dy), out KWall w) ? w : null;

    private void Set(int dx, int dy, KMat mat, KShape shape = KShape.Full) => cells[(dx, dy)] = new KCell(dx, dy, mat, shape);
    private void Clear(int dx, int dy) => cells.Remove((dx, dy));
    private void SetWall(int dx, int dy, KWall wall) => walls[(dx, dy)] = wall;

    private static int FloorRow(int story) => -StoryHeight * story;

    public static KonohaDesign Create()
    {
        KonohaDesign d = new();
        d.Ground();
        d.Gate();
        d.VillageWall(-HalfWidth, -1);
        d.VillageWall(HalfWidth - WallThickness + 1, 1);

        // West: the Hokage tower by the plaza, the Academy, then homes out to the wall.
        d.HokageTower(-58);
        d.Academy(-94);
        d.Places.Add(new KPlace(-100, -1, KFix.Tree));
        d.Places.Add(new KPlace(-104, -1, KFix.Tree));
        d.Apartment("住宅 A", -133, KMat.Stucco, KMat.RedShingle, KWall.Shoji);
        d.Apartment("住宅 B", -162, KMat.Stucco, KMat.BlueShingle, KWall.Palm);
        d.Apartment("住宅 C", -191, KMat.Stucco, KMat.RedShingle, KWall.Shoji);

        // East: Ichiraku, the shopping street, the hospital, homes, Training Ground 3.
        d.Ichiraku(26);
        d.Shop("商店 1", 43, KMat.RedShingle);
        d.Shop("商店 2", 60, KMat.BlueShingle);
        d.Shop("商店 3", 77, KMat.RedShingle);
        d.Hospital(94);
        d.Apartment("住宅 D", 132, KMat.Stucco, KMat.RedShingle, KWall.Palm);
        d.TrainingGround(162);

        foreach (int x in new[] { -18, 17, -62, -98, -136, -165, 40, 128, 158 })
            d.Places.Add(new KPlace(x, -1, KFix.LampPost));
        d.Places.Add(new KPlace(-3, -1, KFix.Sign, "木叶隐村\n——火之国·阿吽之门"));
        d.Places.Add(new KPlace(161, -1, KFix.Tree));
        return d;
    }

    // Grass over dirt everywhere, stone slabs across the plaza.
    private void Ground()
    {
        for (int x = -HalfWidth; x <= HalfWidth; x++)
        {
            Set(x, 0, Math.Abs(x) <= PlazaHalf ? KMat.Slab : KMat.Grass);
            for (int y = 1; y <= FoundationDepth; y++)
                Set(x, y, KMat.Dirt);
        }
    }

    // The A-Un gate: two tall pillars, a double crossbeam and a tiled roof; the spawn point is under it.
    private void Gate()
    {
        foreach (int side in new[] { -1, 1 })
        {
            int inner = side * GatePillarInner;
            for (int i = 0; i < GatePillarWidth; i++)
                // Beams, not blocks: in a side view the pillars stand in front of the street, so they must not block it.
                for (int y = -1; y >= -GateHeight; y--)
                    Set(inner + side * i, y, KMat.Beam);
            // Hanging from the crossbeam just inside each pillar.
            Places.Add(new KPlace(inner - side * 2, -GateHeight, KFix.Banner, Style: side < 0 ? 0 : 1));
        }
        int beamHalf = GatePillarInner + GatePillarWidth + 2;
        for (int x = -beamHalf; x <= beamHalf; x++)
        {
            Set(x, -GateHeight - 1, KMat.Wood);
            Set(x, -GateHeight - 2, KMat.Wood);
        }
        Roof(-beamHalf - 1, beamHalf + 1, -GateHeight - 3, KMat.RedShingle, 2);
        // A second, smaller tier with a ridge beam, like the gate's stacked roof.
        for (int x = -6; x <= 6; x++)
            Set(x, -GateHeight - 5, KMat.Wood);
        Roof(-8, 8, -GateHeight - 6, KMat.RedShingle, 2);
        // Background, so the street stays open: the two great door leaves swung open beside the pillars, and the
        // village wall running away behind them.
        int outer = GatePillarInner + GatePillarWidth;
        foreach (int side in new[] { -1, 1 })
            for (int i = 0; i < 18; i++)
            {
                int x = side * (outer + i);
                int height = i < 7 ? GateHeight - 3 : 12;
                for (int y = -1; y >= -height; y--)
                    SetWall(x, y, i < 7 ? KWall.DoorLeaf : KWall.Brick);
            }
        Buildings.Add(new KBuilding("阿吽大门", -beamHalf - 1, beamHalf + 1, -GateHeight - 8));
    }

    // A tall stone wall with a gateway at the bottom (brick behind, beam posts at its edges, so it reads as an
    // arch rather than a floating block) and a small tiled cap.
    private void VillageWall(int x0, int side)
    {
        int x1 = x0 + WallThickness - 1;
        for (int x = x0; x <= x1; x++)
        {
            for (int y = -WallPassage - 1; y >= -WallHeight; y--)
                Set(x, y, KMat.Brick);
            for (int y = -1; y >= -WallPassage; y--)
                SetWall(x, y, KWall.Brick);
            // Battlements along the top.
            if ((x - x0) % 2 == 0)
                Set(x, -WallHeight - 1, KMat.Brick);
        }
        for (int y = -1; y >= -WallPassage; y--)
        {
            Set(x0, y, KMat.Beam);
            Set(x1, y, KMat.Beam);
        }
        // The wall running on into the distance behind the street, inside the village.
        for (int i = 1; i <= 8; i++)
        {
            int x = side < 0 ? x1 + i : x0 - i;
            for (int y = -1; y >= -WallHeight + 4; y--)
                SetWall(x, y, KWall.Brick);
        }
        Buildings.Add(new KBuilding(side < 0 ? "西墙" : "东墙", x0, x1, -WallHeight - 1));
    }

    // A block of homes: `rooms` side by side on each of `stories` floors. Ground rooms open through doors in the
    // outer walls; upper rooms through a platform in their floor, stacked so the player can climb straight up.
    // Returns the row of the flat roof.
    private int Block(string name, int x0, int rooms, int interior, int stories, KMat frame, KMat floor, KWall wall,
        bool beds = false, bool topAccess = false)
    {
        int width = rooms * (interior + 1) + 1;
        int x1 = x0 + width - 1;
        for (int s = 0; s < stories; s++)
        {
            int floorRow = FloorRow(s);
            int ceiling = floorRow - StoryHeight;
            for (int x = x0; x <= x1; x++)
            {
                if (s > 0)
                    Set(x, floorRow, floor);
                Set(x, ceiling, floor);
            }
            for (int r = 0; r < rooms; r++)
            {
                int left = x0 + r * (interior + 1);
                int inL = left + 1, inR = left + interior;
                for (int y = floorRow - 1; y > ceiling; y--)
                {
                    Set(left, y, frame);
                    for (int x = inL; x <= inR; x++)
                        SetWall(x, y, wall);
                }
                if (s > 0)
                    for (int x = inL + 1; x <= inL + 3; x++)
                        Set(x, floorRow, KMat.Platform);
                Rooms.Add(new KRoom(inL, inR, ceiling + 1, floorRow - 1, name));
                Furnish(inL, inR, floorRow, ceiling, beds && r % 2 == 0);
            }
            for (int y = floorRow - 1; y > ceiling; y--)
                Set(x1, y, frame);
            if (s == 0)
            {
                Door(x0, floorRow);
                Door(x1, floorRow);
                Awning(x0 - 1, -1);
                Awning(x1 + 1, 1);
            }
        }
        // With a room built on top (the Hokage's office), the top ceiling gets its platforms too.
        int roof = FloorRow(stories);
        for (int r = 0; r < rooms && topAccess; r++)
        {
            int inL = x0 + r * (interior + 1) + 1;
            for (int x = inL + 1; x <= inL + 3; x++)
                Set(x, roof, KMat.Platform);
        }
        Buildings.Add(new KBuilding(name, x0, x1, roof));
        return roof;
    }

    // A little tiled awning over a ground-floor door, high enough to walk under.
    private void Awning(int x, int dir)
    {
        for (int i = 0; i < 2; i++)
            if (!Has(x + dir * i, -4))
                Set(x + dir * i, -4, KMat.RedShingle, KShape.Half);
    }

    // A balcony of platforms two tiles out from the wall at `x`, with a fence behind its railing.
    private void Balcony(int x, int dir, int floorRow)
    {
        for (int i = 0; i < 2; i++)
        {
            int bx = x + dir * i;
            Set(bx, floorRow, KMat.Platform);
            SetWall(bx, floorRow - 1, KWall.Fence);
            SetWall(bx, floorRow - 2, KWall.Fence);
        }
    }

    private void Door(int x, int floorRow)
    {
        for (int y = floorRow - 1; y >= floorRow - 3; y--)
            Clear(x, y);
        Places.Add(new KPlace(x, floorRow - 1, KFix.Door));
    }

    // A table with a chair beside it, and a paper lantern from the ceiling: what every vanilla home needs.
    private void Furnish(int inL, int inR, int floorRow, int ceiling, bool bed)
    {
        int table = inR - 2;
        Places.Add(new KPlace(table, floorRow - 1, KFix.Table));
        Places.Add(new KPlace(table - 2, floorRow - 1, KFix.Chair));
        // Paper lanterns are two tiles wide and hang left of their anchor: keep both halves clear of the floor
        // platforms (inL+1..inL+3) above.
        Places.Add(new KPlace(inL + (inR - inL) / 2 + 1, ceiling + 1, KFix.Lantern));
        if (bed && inR - inL + 1 >= 12)
            Places.Add(new KPlace(inL + 5, floorRow - 1, KFix.Bed));
    }

    // A pitched roof of shingles over [x0, x1] starting at row `row` and stepping in by one tile a row.
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

    private void Apartment(string name, int x0, KMat frame, KMat roofMat, KWall wall)
    {
        const int interior = 10;
        int roof = Block(name, x0, 2, interior, 3, frame, KMat.Wood, wall, beds: false);
        int x1 = x0 + 2 * (interior + 1);
        Roof(x0 - 1, x1 + 1, roof - 1, roofMat, 1);
        // A round water tank on stilts on the roof, as on the Leaf's rooftops.
        int tx = x1 - 6;
        Set(tx, roof - 2, KMat.Beam);
        Set(tx + 3, roof - 2, KMat.Beam);
        for (int y = roof - 3; y >= roof - 6; y--)
            for (int x = tx; x < tx + 4; x++)
                Set(x, y, KMat.Plating);
        Set(tx, roof - 6, KMat.Plating, KShape.TopRisesEast);
        Set(tx + 3, roof - 6, KMat.Plating, KShape.TopRisesWest);
        // Balconies with a fence on the upper floors, facing the street.
        for (int s = 1; s < 3; s++)
            Balcony(x0 - 1, -1, FloorRow(s));
    }

    private void Academy(int x0)
    {
        const int interior = 13;
        int roof = Block("忍者学校", x0, 2, interior, 2, KMat.Stucco, KMat.Wood, KWall.Palm);
        int x1 = x0 + 2 * (interior + 1);
        Roof(x0 - 2, x1 + 2, roof - 1, KMat.RedShingle, 4);
        foreach (KRoom room in Rooms)
            if (room.Building == "忍者学校" && room.Bottom == -1)
                Places.Add(new KPlace(room.X0 + 5, -1, KFix.Bookcase));
        Places.Add(new KPlace(x0 + 3, roof - 5, KFix.Sign, "忍者学校"));
    }

    // Red, with rounded shoulders so the tower reads as a cylinder, and the office crowned by a red dome.
    private void HokageTower(int x0)
    {
        const int interior = 14;
        int roof = Block("火影楼", x0, 2, interior, 3, KMat.RedBrick, KMat.Wood, KWall.RedStucco, topAccess: true);
        int x1 = x0 + 2 * (interior + 1);
        // The office: one wide room on top.
        int floorRow = roof;
        int ceiling = floorRow - StoryHeight - 4;
        for (int x = x0; x <= x1; x++)
            Set(x, ceiling, KMat.RedBrick);
        for (int y = floorRow - 1; y > ceiling; y--)
        {
            Set(x0, y, KMat.RedBrick);
            Set(x1, y, KMat.RedBrick);
            for (int x = x0 + 1; x < x1; x++)
                SetWall(x, y, KWall.Shoji);
        }
        Rooms.Add(new KRoom(x0 + 1, x1 - 1, ceiling + 1, floorRow - 1, "火影楼"));
        // The desk and chair to one side and a lantern at each end, leaving the emblem clear.
        Places.Add(new KPlace(x1 - 4, floorRow - 1, KFix.Table));
        Places.Add(new KPlace(x1 - 6, floorRow - 1, KFix.Chair));
        Places.Add(new KPlace(x0 + 6, ceiling + 1, KFix.Lantern));
        Places.Add(new KPlace(x1 - 3, ceiling + 1, KFix.Lantern));
        Places.Add(new KPlace(x0 + 4, floorRow - 1, KFix.Bookcase));
        Places.Add(new KPlace(x0 + 8, floorRow - 1, KFix.Sign, "火影办公室"));
        // Rounded shoulders down the tower's sides.
        for (int s = 0; s < 4; s++)
        {
            int top = s < 3 ? FloorRow(s + 1) : ceiling;
            Set(x0 - 1, top, KMat.RedBrick, KShape.TopRisesEast);
            Set(x1 + 1, top, KMat.RedBrick, KShape.TopRisesWest);
        }
        // The 火 emblem, big on the office's back wall, as on the tower's roof in the anime.
        string[] fire =
        {
            "....#....",
            "....#....",
            "#...#...#",
            ".#..#..#.",
            "....#....",
            "...#.#...",
            "..#...#..",
            ".#.....#.",
            "#.......#",
        };
        int ex = (x0 + x1) / 2 - 4;
        for (int row = 0; row < fire.Length; row++)
            for (int col = 0; col < fire[row].Length; col++)
                if (fire[row][col] == '#')
                    SetWall(ex + col, ceiling + 1 + row, KWall.RedStucco);
        // Dome, rounded.
        int[] insets = { 0, 1, 3, 5, 8, 12 };
        int domeRows = insets.Length;
        for (int i = 0; i < domeRows; i++)
        {
            int y = ceiling - 1 - i;
            int inset = insets[i];
            int l = x0 - 1 + inset, r = x1 + 1 - inset;
            if (l > r)
                break;
            for (int x = l; x <= r; x++)
                Set(x, y, KMat.RedShingle);
            Set(l, y, KMat.RedShingle, KShape.TopRisesEast);
            Set(r, y, KMat.RedShingle, KShape.TopRisesWest);
        }
        Buildings[^1] = new KBuilding("火影楼", x0 - 1, x1 + 1, ceiling - domeRows);
        Places.Add(new KPlace(x1 + 3, -1, KFix.Sign, "火影楼\n——火"));
    }

    private void Hospital(int x0)
    {
        const int interior = 14;
        int roof = Block("木叶医院", x0, 2, interior, 3, KMat.Marble, KMat.Marble, KWall.Marble, beds: true);
        int x1 = x0 + 2 * (interior + 1);
        for (int x = x0 - 1; x <= x1 + 1; x++)
            Set(x, roof - 1, KMat.Marble);
        // A red cross standing on the roof.
        int cx = (x0 + x1) / 2;
        for (int y = roof - 2; y >= roof - 6; y--)
            Set(cx, y, KMat.RedBrick);
        for (int x = cx - 2; x <= cx + 2; x++)
            Set(x, roof - 4, KMat.RedBrick);
        Buildings[^1] = new KBuilding("木叶医院", x0 - 1, x1 + 1, roof - 6);
        Places.Add(new KPlace(x0 - 3, -1, KFix.Sign, "木叶医院"));
    }

    private void Shop(string name, int x0, KMat roofMat)
    {
        const int interior = 11;
        int roof = Block(name, x0, 1, interior, 2, KMat.DynastyWood, KMat.Wood, KWall.Shoji);
        int x1 = x0 + interior + 1;
        Roof(x0 - 2, x1 + 2, roof - 1, roofMat, 3);
    }

    // The ramen stand: a back room to live in, and an open counter under an awning facing the plaza.
    private void Ichiraku(int x0)
    {
        const int interior = 10;
        int roof = Block("一乐拉面", x0, 1, interior, 1, KMat.DynastyWood, KMat.Wood, KWall.Shoji);
        int x1 = x0 + interior + 1;
        Roof(x0 - 6, x1 + 1, roof - 1, KMat.RedShingle, 2);
        // Awning posts and the counter out front.
        for (int y = -1; y >= roof; y--)
            Set(x0 - 5, y, KMat.Beam);
        for (int x = x0 - 5; x < x0; x++)
            Set(x, roof, KMat.Wood);
        Places.Add(new KPlace(x0 - 3, -1, KFix.Table));
        Places.Add(new KPlace(x0 - 3, roof + 1, KFix.Banner, Style: 3));
        Places.Add(new KPlace(x0 - 4, roof + 1, KFix.Banner, Style: 3));
        Places.Add(new KPlace(x0 - 7, -1, KFix.Sign, "一乐拉面"));
        Buildings[^1] = new KBuilding("一乐拉面", x0 - 6, x1 + 1, roof - 2);
    }

    // Training Ground 3: three wooden posts in the grass.
    private void TrainingGround(int x0)
    {
        foreach (int x in new[] { x0 + 6, x0 + 12, x0 + 18 })
            for (int y = -1; y >= -3; y--)
                Set(x, y, KMat.LivingWood);
        Places.Add(new KPlace(x0 + 2, -1, KFix.Sign, "第三演习场"));
        Buildings.Add(new KBuilding("第三演习场", x0, x0 + 24, -3));
    }
}
