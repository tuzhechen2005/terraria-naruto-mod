using System;
using System.Collections.Generic;

namespace ShinobiPrototype.Common;

// What each tile of the Wave Country bridge scene is made of, independent of Terraria so it can be tested and
// rendered to a preview image. Positions are (offset, y): offset counts tiles along the bridge from the shoreline
// (0) towards the sea, y is the absolute tile row. Shapes are bridge-relative ("seaward"); BridgeBuilder maps them
// to Terraria slopes for the side of the world the bridge is on.
public enum Part : byte
{
    Brick, Slab, Stone, Sand, Dirt, Grass, Wood, DynastyWood, Shingle, Beam, Platform, Rope, RedBeam, RedWood,
}

public enum Shape : byte
{
    Full,
    Half,
    RisesSeaward,       // top surface climbs towards the sea
    RisesLandward,
    CeilingRisesSeaward, // bottom face climbs towards the sea (arch undersides)
    CeilingRisesLandward,
}

public enum Backdrop : byte { StoneWall, Railing, ShojiWall, Glass, WoodWall }

public enum Fixture : byte
{
    LampPost, Lantern, IslandSign, BridgeSign, Chest, Door, Table, Chair, Palm, Shell,
}

public readonly record struct Cell(int Offset, int Y, Part Part, Shape Shape = Shape.Full, bool Scaffold = false);

public readonly record struct Wall(int Offset, int Y, Backdrop Backdrop);

// Y is the bottom row of the fixture (the row resting on the floor); lanterns hang from the row above theirs.
public readonly record struct Placement(int Offset, int Y, Fixture Fixture);

public sealed class BridgeDesign
{
    // Layout, in tiles along the bridge.
    public const int DeckAboveWater = 7;
    public const int Camber = 3;              // the deck rises this much towards the middle of the full span
    public const int PierSpacing = 14;
    public const int PierWidth = 2;
    public const int LastBuiltPier = 56;       // arches stand up to here before completion
    public const int HalfBuiltPier = 70;
    public const int SlabEnd = 60;             // unfinished walkway ends raggedly around here
    public const int BandEnd = 63;
    public const int IslandStart = 98;
    public const int IslandFlatEnd = 118;
    public const int IslandEnd = 126;
    public const int DockLength = 6;
    public const int Clearance = 25;
    public const int HutWidth = 13;
    public const int HutStilts = 3;
    public const int HutRoomHeight = 5;
    public const int PorchLength = 3;
    public const int MaxRampSteps = 16;
    public const int SeaMargin = 20;
    public const int TotalReach = IslandEnd + DockLength + SeaMargin;

    public static int DeckY(int waterY) => waterY - DeckAboveWater;

    // Walking row of the deck at an offset: a gentle arch over the whole span from shore to island.
    public static int DeckRow(int waterY, int offset)
    {
        if (offset <= 0 || offset >= IslandStart)
            return DeckY(waterY);
        float t = (offset - IslandStart / 2f) / (IslandStart / 2f);
        return DeckY(waterY) - (int)Math.Round(Camber * (1f - t * t));
    }
    public const int UnfinishedEnd = BandEnd;
    public static bool IsPier(int offset) => offset >= 0 && offset < IslandStart && offset % PierSpacing < PierWidth;

    public List<Cell> Cells { get; } = new();
    public List<Wall> Walls { get; } = new();
    public List<Placement> Placements { get; } = new();
    // Rectangles (inclusive) of natural terrain to clear before building: (fromOffset, toOffset, topY, bottomY).
    public List<(int From, int To, int Top, int Bottom)> Clear { get; } = new();

    public int HutMidOffset { get; private set; }
    public int HutDoorOffset { get; private set; }
    public int HutFloorY { get; private set; }
    public int LandmostOffset { get; private set; }

    private readonly int waterY;
    private readonly int deckY;
    private readonly Func<int, int> groundY;
    private readonly Func<int, int> seabedY;
    private readonly bool finished;
    private readonly Dictionary<(int, int), int> index = new();

    // groundY: first solid row of natural ground on land (offsets <= 0); seabedY: first solid row under the sea.
    public static BridgeDesign Create(int waterY, Func<int, int> groundY, Func<int, int> seabedY, bool finished)
    {
        BridgeDesign design = new(waterY, groundY, seabedY, finished);
        design.Build();
        return design;
    }

    private BridgeDesign(int waterY, Func<int, int> groundY, Func<int, int> seabedY, bool finished)
    {
        this.waterY = waterY;
        deckY = DeckY(waterY);
        this.groundY = groundY;
        this.seabedY = seabedY;
        this.finished = finished;
    }

    private int Deck(int offset) => DeckRow(waterY, offset);

    private void Set(int offset, int y, Part part, Shape shape = Shape.Full, bool scaffold = false)
    {
        Cell cell = new(offset, y, part, shape, scaffold);
        if (index.TryGetValue((offset, y), out int at))
            Cells[at] = cell;
        else
        {
            index[(offset, y)] = Cells.Count;
            Cells.Add(cell);
        }
    }

    public bool Has(int offset, int y) => index.ContainsKey((offset, y));

    public Cell? At(int offset, int y) => index.TryGetValue((offset, y), out int at) ? Cells[at] : null;

    private void Build()
    {
        int deckTo = finished ? IslandStart - 1 : SlabEnd;
        Clear.Add((-1, IslandEnd + DockLength, deckY - Clearance, deckY - 1));

        BuildArches();
        BuildDeck(deckTo);
        if (!finished)
            BuildConstructionSite();
        int rampEnd = BuildRamp();
        BuildHut(rampEnd);
        BuildIsland();
    }

    // Piers every 14 tiles with semi-elliptical stone arches between them, like a real stone bridge.
    private void BuildArches()
    {
        int lastArch = finished ? IslandStart : LastBuiltPier;
        for (int pier = 0; pier < IslandStart; pier += PierSpacing)
        {
            bool built = pier <= lastArch;
            bool half = !finished && pier == HalfBuiltPier;
            if (!built && !half)
                continue;
            for (int x = pier; x < pier + PierWidth; x++)
                for (int y = half ? waterY - 3 : Deck(x) + 1; y < seabedY(x); y++)
                    Set(x, y, Part.Brick);
            if (pier > 0)
                Footing(pier - 1, Shape.RisesSeaward);
            Footing(pier + PierWidth, Shape.RisesLandward);
        }

        for (int pier = 0; pier + PierSpacing <= lastArch; pier += PierSpacing)
            Arch(pier + PierWidth, pier + PierSpacing - 1);
    }

    private void Footing(int x, Shape topShape)
    {
        for (int y = waterY - 1; y < seabedY(x); y++)
            Set(x, y, Part.Brick, y == waterY - 1 ? topShape : Shape.Full);
    }

    private int Intrados(int x, int from, int to)
    {
        float center = (from + to) / 2f;
        float half = (to - from) / 2f + 0.5f;
        float t = (x - center) / half;
        int crown = Deck(x) + 3;
        int spring = waterY - 1;
        return crown + (int)Math.Round((spring - crown) * t * t);
    }

    private void Arch(int from, int to)
    {
        for (int x = from; x <= to; x++)
        {
            int low = Intrados(x, from, to);
            for (int y = Deck(x) + 1; y < low; y++)
                Set(x, y, Part.Brick);
            // The arch ring is traced in lighter slab so each arch reads clearly.
            int seaward = x < to ? Intrados(x + 1, from, to) : low;
            int landward = x > from ? Intrados(x - 1, from, to) : low;
            Shape ring = seaward < low ? Shape.CeilingRisesSeaward
                : landward < low ? Shape.CeilingRisesLandward
                : Shape.Full;
            Set(x, low, Part.Slab, ring);
        }
    }

    // Stone slab walkway over a slab cornice band, following the camber (hammered slopes where it steps so it
    // walks smoothly), with a low stone railing and lamp posts over each arch crown.
    private void BuildDeck(int deckTo)
    {
        int bandTo = finished ? deckTo : BandEnd;
        for (int x = 0; x <= bandTo; x++)
            Set(x, Deck(x) + 1, Part.Slab);
        for (int x = 0; x <= deckTo; x++)
        {
            if (!finished && x > SlabEnd - 3 && x % 2 == 0)
                continue; // ragged, unfinished paving
            int row = Deck(x);
            Set(x, row, Part.Slab);
            if (x > 0 && Deck(x - 1) > row)
                Set(x - 1, row, Part.Slab, Shape.RisesSeaward);
            else if (x > 0 && Deck(x - 1) < row)
                Set(x, row - 1, Part.Slab, Shape.RisesLandward);
            Walls.Add(new Wall(x, row - 1, Backdrop.Railing));
            Walls.Add(new Wall(x, row - 2, Backdrop.Railing));
            if (x % 7 == 0)
                Walls.Add(new Wall(x, row - 3, Backdrop.StoneWall));
        }
        for (int x = PierSpacing / 2; x <= deckTo; x += PierSpacing)
            Placements.Add(new Placement(x, Deck(x) - 1, Fixture.LampPost));
        if (finished)
            Placements.Add(new Placement(2, Deck(2) - 1, Fixture.BridgeSign));
    }

    // Before completion: timber scaffolding around the unfinished end, a crane lifting a stone block,
    // and stacked building material on the deck.
    private void BuildConstructionSite()
    {
        const int from = LastBuiltPier - 1;
        const int to = HalfBuiltPier + 4;
        for (int x = LastBuiltPier + 2; x <= to; x += 3)
        {
            int bottom = seabedY(x);
            for (int y = deckY - 8; y < bottom; y++)
                if (!Has(x, y))
                    Set(x, y, Part.Beam, scaffold: true);
        }
        foreach (int level in new[] { deckY - 8, deckY - 4, waterY - 2 })
            for (int x = from; x <= to; x++)
                if (!Has(x, level))
                    Set(x, level, Part.Platform, scaffold: true);
        for (int x = BandEnd + 1; x <= to; x++)
            Set(x, Deck(BandEnd), Part.Platform, scaffold: true);
        for (int y = deckY - 7; y < waterY - 2; y++)
            if (!Has(to, y))
                Set(to, y, Part.Rope, scaffold: true);

        // The crane: a mast on the deck, a jib out over the gap, a rope and the stone block being lifted.
        const int mast = LastBuiltPier + 1;
        for (int y = deckY - 16; y < Deck(mast); y++)
            Set(mast, y, Part.Beam, scaffold: true);
        for (int x = mast - 3; x <= HalfBuiltPier - 1; x++)
            Set(x, deckY - 17, Part.Wood, scaffold: true);
        for (int y = deckY - 16; y < deckY - 11; y++)
            Set(HalfBuiltPier - 1, y, Part.Rope, scaffold: true);
        for (int x = HalfBuiltPier - 2; x <= HalfBuiltPier - 1; x++)
            for (int y = deckY - 11; y <= deckY - 10; y++)
                Set(x, y, Part.Brick, scaffold: true);

        Set(LastBuiltPier - 6, Deck(LastBuiltPier - 6) - 1, Part.Brick, scaffold: true);
        Set(LastBuiltPier - 5, Deck(LastBuiltPier - 5) - 1, Part.Brick, scaffold: true);
        Set(LastBuiltPier - 4, Deck(LastBuiltPier - 4) - 1, Part.Slab, Shape.Half, scaffold: true);
    }

    // A stone approach ramp down to the beach, one tile per column, hammered into a smooth slope.
    private int BuildRamp()
    {
        int end = 0;
        for (int step = 1; step <= MaxRampSteps; step++)
        {
            // A slope tile in row r climbs from r + 1 to r, so step n sits in row deckY + n - 1 and the
            // surface runs on without a ledge from the deck down to the ground.
            int x = -step;
            int top = deckY + step - 1;
            int ground = groundY(x);
            if (top >= ground)
                break;
            end = x;
            Clear.Add((x, x, deckY - Clearance, top - 1));
            for (int y = top; y < ground; y++)
                Set(x, y, y == top ? Part.Slab : Part.Brick, y == top ? Shape.RisesSeaward : Shape.Full);
            Walls.Add(new Wall(x, top - 1, Backdrop.Railing));
        }
        return end;
    }

    // Tazuna's house: a fisherman's hut on stilts with a porch and wooden stairs, dynasty-wood walls, paper
    // (shoji) and glass windows, and a blue-tiled gable roof. It has a door, table and chair but no light until
    // the bridge is finished, so no other town NPC can claim it before then.
    private void BuildHut(int rampEnd)
    {
        const int stairs = HutStilts + 1;
        int near = rampEnd - 3 - stairs - PorchLength;
        int far = near - (HutWidth - 1);
        int highestGround = int.MaxValue;
        for (int x = far; x <= near + PorchLength + stairs; x++)
            highestGround = Math.Min(highestGround, groundY(x));
        int floor = highestGround - HutStilts - 1;
        int ceiling = floor - HutRoomHeight - 1;
        HutFloorY = floor;
        HutMidOffset = (near + far) / 2;
        HutDoorOffset = near;
        LandmostOffset = far - 3;

        for (int x = far - 2; x <= near + PorchLength + stairs; x++)
            Clear.Add((x, x, ceiling - 6, groundY(x) - 1));

        for (int x = far; x <= near + PorchLength; x++)
        {
            bool porch = x > near;
            Set(x, floor, porch ? Part.Platform : Part.Wood);
            bool stilt = x == far || x == near || x == near + PorchLength || (x - far) % 4 == 0;
            if (stilt)
                for (int y = floor + 1; y < groundY(x); y++)
                    Set(x, y, Part.Beam);
            if (porch)
                Walls.Add(new Wall(x, floor - 1, Backdrop.Railing));
        }

        for (int y = ceiling + 1; y < floor; y++)
        {
            Set(far, y, Part.DynastyWood);
            if (y < floor - 3)
                Set(near, y, Part.DynastyWood);
            for (int x = far + 1; x < near; x++)
            {
                bool window = y is var r && r >= floor - 4 && r <= floor - 3 &&
                              (x - far) % 5 is 2 or 3;
                Walls.Add(new Wall(x, y, window ? Backdrop.Glass : Backdrop.ShojiWall));
            }
        }
        Placements.Add(new Placement(near, floor - 1, Fixture.Door));
        Placements.Add(new Placement(far + 3, floor - 1, Fixture.Table));
        Placements.Add(new Placement(far + 5, floor - 1, Fixture.Chair));
        if (finished)
            Placements.Add(new Placement(HutMidOffset, ceiling + 1, Fixture.Lantern));

        for (int x = far - 1; x <= near + 1; x++)
            Set(x, ceiling, Part.DynastyWood);
        int left = far - 2;
        int right = near + 2;
        for (int row = 1; left <= right; row++, left += 2, right -= 2)
        {
            int y = ceiling - row;
            for (int x = left; x <= right; x++)
            {
                Shape shape = x == left ? Shape.RisesSeaward : x == right ? Shape.RisesLandward : Shape.Full;
                Set(x, y, Part.Shingle, left == right ? Shape.Half : shape);
            }
        }

        // Wooden stairs from the beach up to the porch.
        for (int step = 1; step <= stairs; step++)
        {
            int x = near + PorchLength + step;
            int top = floor + step - 1;
            if (top >= groundY(x))
                break;
            for (int y = top; y < groundY(x); y++)
                Set(x, y, Part.Wood, y == top ? Shape.RisesLandward : Shape.Full);
        }
    }

    // A small island past the gap: a rocky cliff facing the bridge, a grassy top with a red torii gate, the
    // "Land of Waves" sign and a chest, a sandy beach with palms and shells, and a wooden dock.
    private void BuildIsland()
    {
        int grassFrom = IslandStart + 2;
        int grassTo = IslandFlatEnd - 5;
        for (int x = IslandStart; x <= IslandEnd; x++)
        {
            int top = x <= IslandFlatEnd ? deckY : deckY + (x - IslandFlatEnd - 1);
            top = Math.Min(top, waterY);
            bool beach = x > grassTo;
            bool cliff = x < grassFrom;
            for (int y = top; y < seabedY(x); y++)
            {
                Part part = cliff || y >= waterY + 2 ? Part.Stone
                    : beach ? Part.Sand
                    : y == top ? Part.Grass : Part.Dirt;
                Shape shape = y == top && x > IslandFlatEnd ? Shape.RisesLandward : Shape.Full;
                Set(x, y, part, shape);
            }
        }
        if (!finished)
            for (int y = deckY; y < waterY; y++)
                Set(IslandStart, y, Part.Stone, y == deckY ? Shape.RisesSeaward : Shape.Full);

        // Below the waterline the island spreads out like a reef, one tile per two rows of depth.
        for (int x = IslandStart - 12; x <= IslandEnd + 12; x++)
        {
            if (x >= IslandStart && x <= IslandEnd)
                continue;
            int reach = x < IslandStart ? IslandStart - x : x - IslandEnd;
            for (int y = waterY + 2 * reach; y < seabedY(x); y++)
                if (!Has(x, y))
                    Set(x, y, Part.Stone, y == waterY + 2 * reach
                        ? x < IslandStart ? Shape.RisesSeaward : Shape.RisesLandward
                        : Shape.Full);
        }

        // Torii gate.
        const int toriiLeft = IslandStart + 6;
        const int toriiRight = toriiLeft + 4;
        for (int y = deckY - 6; y < deckY; y++)
        {
            Set(toriiLeft, y, Part.RedBeam);
            Set(toriiRight, y, Part.RedBeam);
        }
        for (int x = toriiLeft - 1; x <= toriiRight + 1; x++)
            Set(x, deckY - 7, Part.RedWood,
                x == toriiLeft - 1 ? Shape.RisesLandward : x == toriiRight + 1 ? Shape.RisesSeaward : Shape.Full);
        for (int x = toriiLeft; x <= toriiRight; x++)
            Set(x, deckY - 5, Part.RedWood, Shape.Half);

        // Palms stand on the ground tile given; everything else rests on the row above the ground.
        Placements.Add(new Placement(IslandStart + 1, deckY - 1, Fixture.LampPost));
        Placements.Add(new Placement(IslandStart + 3, deckY - 1, Fixture.IslandSign));
        Placements.Add(new Placement(IslandStart + 13, deckY - 1, Fixture.Chest));
        Placements.Add(new Placement(grassTo + 1, deckY, Fixture.Palm));
        Placements.Add(new Placement(IslandFlatEnd, deckY, Fixture.Palm));
        Placements.Add(new Placement(grassTo + 3, deckY - 1, Fixture.Shell));

        for (int x = IslandEnd + 1; x <= IslandEnd + DockLength; x++)
        {
            Set(x, waterY - 1, Part.Platform);
            if ((x - IslandEnd) % 3 == 0)
                for (int y = waterY; y < seabedY(x); y++)
                    Set(x, y, Part.Beam);
        }
    }
}
