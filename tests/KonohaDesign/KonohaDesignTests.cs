using ShinobiPrototype.Common;

static void Check(bool condition, string name)
{
    if (!condition)
        throw new Exception(name);
    Console.WriteLine($"PASS {name}");
}

KonohaDesign d = KonohaDesign.Create();

// Optional dump for the preview renderer: dotnet run -- <out.jsonl>
if (args.Length > 0)
{
    using StreamWriter w = new(args[0]);
    foreach (KCell c in d.Cells)
        w.WriteLine($"{{\"k\":\"c\",\"x\":{c.Dx},\"y\":{c.Dy},\"m\":\"{c.Mat}\",\"s\":\"{c.Shape}\"}}");
    foreach (KWallCell c in d.Walls)
        w.WriteLine($"{{\"k\":\"w\",\"x\":{c.Dx},\"y\":{c.Dy},\"m\":\"{c.Wall}\"}}");
    foreach (KPlace p in d.Places)
        w.WriteLine($"{{\"k\":\"p\",\"x\":{p.Dx},\"y\":{p.Dy},\"m\":\"{p.Fix}\",\"t\":\"{p.Text.Replace("\n", " ")}\"}}");
    foreach (KRoom r in d.Rooms)
        w.WriteLine($"{{\"k\":\"r\",\"x0\":{r.X0},\"x1\":{r.X1},\"top\":{r.Top},\"bottom\":{r.Bottom}}}");
}

bool Solid(int x, int y) => d.CellAt(x, y) is KCell c && c.Mat is not (KMat.Platform or KMat.Beam);
bool Platform(int x, int y) => d.CellAt(x, y) is KCell { Mat: KMat.Platform };
bool DoorAt(int x, int y) => d.Places.Any(p => p.Fix == KFix.Door && p.Dx == x && y <= p.Dy && y >= p.Dy - 2);

Check(d.Rooms.Count >= 45, $"At least 45 homes (has {d.Rooms.Count})");

foreach (KRoom r in d.Rooms)
{
    string at = $"{r.Building} [{r.X0}..{r.X1}, {r.Top}..{r.Bottom}]";
    bool enclosed = true;
    bool entrance = false;
    for (int x = r.X0 - 1; x <= r.X1 + 1; x++)
        foreach (int y in new[] { r.Top - 1, r.Bottom + 1 })
        {
            if (Platform(x, y)) entrance = true;
            else if (!Solid(x, y) && x >= r.X0 && x <= r.X1) enclosed = false;
        }
    for (int y = r.Top; y <= r.Bottom; y++)
        foreach (int x in new[] { r.X0 - 1, r.X1 + 1 })
        {
            if (DoorAt(x, y)) entrance = true;
            else if (!Solid(x, y)) enclosed = false;
        }
    Check(enclosed, $"{at} is closed in by solid tiles, doors and platforms");
    Check(entrance, $"{at} has a door or a platform");
    int area = (r.Width + 2) * (r.Height + 2);
    Check(area >= 60 && r.Width * r.Height <= 750, $"{at} meets the vanilla size rule ({area} with frame)");
    bool walled = true;
    for (int x = r.X0; x <= r.X1; x++)
        for (int y = r.Top; y <= r.Bottom; y++)
            if (d.WallAt(x, y) is null) walled = false;
    Check(walled, $"{at} has background wall everywhere");
    bool In(KPlace p) => p.Dx >= r.X0 && p.Dx <= r.X1 && p.Dy >= r.Top - 1 && p.Dy <= r.Bottom;
    Check(d.Places.Any(p => p.Fix == KFix.Table && In(p)) && d.Places.Any(p => p.Fix == KFix.Chair && In(p)) &&
          d.Places.Any(p => p.Fix == KFix.Lantern && In(p)), $"{at} has a table, a chair and a lantern");
    bool lanternHangs = d.Places.Where(p => p.Fix == KFix.Lantern && In(p))
        .All(p => Solid(p.Dx, p.Dy - 1) && Solid(p.Dx - 1, p.Dy - 1));
    Check(lanternHangs, $"{at} lantern hangs from solid ceiling on both halves");
    bool clearInside = true;
    for (int x = r.X0; x <= r.X1; x++)
        for (int y = r.Top; y <= r.Bottom; y++)
            if (d.Has(x, y)) clearInside = false;
    Check(clearInside, $"{at} has no tiles inside");
}

// Footprints of fixtures (x0, y0, w, h), by how each is anchored; none may overlap another or a tile.
(int X, int Y, int W, int H) Footprint(KPlace p) => p.Fix switch
{
    KFix.Door => (p.Dx, p.Dy - 2, 1, 3),
    KFix.Table => (p.Dx - 1, p.Dy - 1, 3, 2),
    KFix.Chair => (p.Dx, p.Dy - 1, 1, 2),
    KFix.Lantern => (p.Dx - 1, p.Dy, 2, 2),
    KFix.LampPost => (p.Dx, p.Dy - 5, 1, 6),
    KFix.Sign => (p.Dx, p.Dy - 1, 2, 2),
    KFix.Banner => (p.Dx, p.Dy, 1, 3),
    KFix.Bookcase => (p.Dx - 1, p.Dy - 3, 3, 4),
    KFix.Bed => (p.Dx - 2, p.Dy - 1, 4, 2),
    KFix.Painting or KFix.WeaponRack => (p.Dx - 1, p.Dy - 1, 3, 3),
    KFix.Bench => (p.Dx - 1, p.Dy - 1, 3, 2),
    KFix.PottedPlant => (p.Dx, p.Dy - 1, 1, 2),
    _ => (p.Dx, p.Dy, 1, 1),
};
Dictionary<(int, int), KPlace> used = new();
bool noClash = true;
foreach (KPlace p in d.Places.Where(p => p.Fix != KFix.Tree))
{
    var (fx, fy, fw, fh) = Footprint(p);
    bool hangs = p.Fix is KFix.Painting or KFix.WeaponRack;
    for (int x = fx; x < fx + fw; x++)
        for (int y = fy; y < fy + fh; y++)
        {
            if (used.TryGetValue((x, y), out KPlace other) && !(hangs ^ (other.Fix is KFix.Painting or KFix.WeaponRack)))
            {
                noClash = false;
                Console.WriteLine($"  {p.Fix} at {p.Dx},{p.Dy} overlaps {other.Fix} at {other.Dx},{other.Dy}");
            }
            if (p.Fix != KFix.Door && Solid(x, y))
            {
                noClash = false;
                Console.WriteLine($"  {p.Fix} at {p.Dx},{p.Dy} sits in a tile at {x},{y}");
            }
            used[(x, y)] = p;
        }
}
Check(noClash, "Fixtures do not overlap each other or tiles");

var sorted = d.Buildings.OrderBy(b => b.X0).ToList();
bool apart = true;
for (int i = 1; i < sorted.Count; i++)
    if (sorted[i].X0 <= sorted[i - 1].X1)
    {
        apart = false;
        Console.WriteLine($"  overlap: {sorted[i - 1].Name} {sorted[i - 1].X0}..{sorted[i - 1].X1} and {sorted[i].Name} {sorted[i].X0}..{sorted[i].X1}");
    }
Check(apart, "Buildings do not overlap");
Check(d.Buildings.All(b => b.X0 >= -KonohaDesign.HalfWidth - 1 && b.X1 <= KonohaDesign.HalfWidth + 1), "Everything stands inside the walls");
Check(d.Cells.All(c => Math.Abs(c.Dx) <= KonohaDesign.HalfWidth + 1), "No tiles beyond the walls");
Check(!Solid(0, -1) && !Solid(0, -2) && !Solid(0, -3) && Solid(0, 0), "The spawn under the gate is open ground");
bool passable = Enumerable.Range(-KonohaDesign.HalfWidth, 2 * KonohaDesign.HalfWidth + 1)
    .All(x => !Solid(x, -1) || d.Buildings.Any(b => b.Name is not ("西墙" or "东墙" or "阿吽大门") && x >= b.X0 && x <= b.X1));
Check(passable, "The main street is walkable end to end outside buildings (gate and wall passages open)");
foreach (var p in d.Places.Where(p => p.Fix == KFix.LampPost)) foreach (var b in d.Buildings.Where(b => p.Dx >= b.X0 - 1 && p.Dx <= b.X1 + 1)) Console.WriteLine($"  lamp {p.Dx} in {b.Name} {b.X0}..{b.X1}");
Check(d.Places.Where(p => p.Fix == KFix.LampPost).All(p => !d.Buildings.Any(b => p.Dx >= b.X0 - 1 && p.Dx <= b.X1 + 1)),
    "Lamp posts stand in the open");

// The street runs straight through the village: along the ground rows nothing blocks from wall to wall except doors,
// which open (user, 2026-09-30: every building had only one way in and the village could not be crossed).
{
    KonohaDesign street = KonohaDesign.Create();
    HashSet<(int, int)> doorCells = new();
    foreach (KPlace place in street.Places)
        if (place.Fix == KFix.Door)
            for (int dy = 0; dy < 3; dy++)
                doorCells.Add((place.Dx, place.Dy - dy));
    List<int> blocked = new();
    for (int x = -KonohaDesign.HalfWidth; x <= KonohaDesign.HalfWidth; x++)
        for (int y = -3; y <= -1; y++)
            // The training posts (living wood, three tall) are meant to be jumped.
            if (street.CellAt(x, y) is KCell c && c.Mat is not (KMat.Beam or KMat.Platform or KMat.LivingWood) && !doorCells.Contains((x, y)))
                blocked.Add(x);
    Check(blocked.Count == 0, $"The street can be walked from wall to wall (blocked at {string.Join(",", blocked.Distinct().Take(8))})");
    int partitionDoors = street.Places.Count(p => p.Fix == KFix.Door);
    Check(partitionDoors > 30, "Doors through the partitions as well as the outer walls");
}
