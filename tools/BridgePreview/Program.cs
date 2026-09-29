// Dumps the Wave Country bridge design on a synthetic beach as JSON lines, for scripts/render_bridge_preview.py.
// Usage: dotnet run --project tools/BridgePreview -- <finished:true|false> <out.jsonl>
using System.Text.Json;
using ShinobiPrototype.Common;

bool finished = args.Length > 0 && bool.Parse(args[0]);
string outPath = args.Length > 1 ? args[1] : "bridge.jsonl";
const int waterY = 100;
// Beach rising gently inland from the waterline; sea floor sloping down to 20 tiles deep.
int Ground(int offset) => offset >= 0 ? waterY : waterY - Math.Min(6, (-offset + 2) / 4);
int Seabed(int offset) => offset <= 0 ? Ground(offset) : waterY + Math.Min(20, 2 + offset / 3);

BridgeDesign design = BridgeDesign.Create(waterY, Ground, Seabed, finished);
using StreamWriter w = new(outPath);
w.WriteLine(JsonSerializer.Serialize(new { kind = "meta", waterY, deckY = BridgeDesign.DeckY(waterY),
    hutFloor = design.HutFloorY, landmost = design.LandmostOffset, reach = BridgeDesign.TotalReach }));
for (int o = design.LandmostOffset - 2; o <= BridgeDesign.TotalReach; o++)
    w.WriteLine(JsonSerializer.Serialize(new { kind = "terrain", o, ground = o <= 0 ? Ground(o) : Seabed(o) }));
foreach (Cell c in design.Cells)
    w.WriteLine(JsonSerializer.Serialize(new { kind = "cell", o = c.Offset, y = c.Y, part = c.Part.ToString(), shape = c.Shape.ToString() }));
foreach (Wall wall in design.Walls)
    w.WriteLine(JsonSerializer.Serialize(new { kind = "wall", o = wall.Offset, y = wall.Y, b = wall.Backdrop.ToString() }));
foreach (Placement p in design.Placements)
    w.WriteLine(JsonSerializer.Serialize(new { kind = "fixture", o = p.Offset, y = p.Y, f = p.Fixture.ToString() }));
