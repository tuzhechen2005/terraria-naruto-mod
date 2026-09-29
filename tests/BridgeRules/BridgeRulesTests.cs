using ShinobiPrototype.Common;
using static ShinobiPrototype.Common.BridgeRules;

static void Check(bool condition, string name)
{
    if (!condition)
        throw new Exception(name);
    Console.WriteLine($"PASS {name}");
}

const int waterY = 100;
int Ground(int o) => o >= 0 ? waterY : waterY - Math.Min(6, (-o + 2) / 4);
int Seabed(int o) => o <= 0 ? Ground(o) : waterY + Math.Min(20, 2 + o / 3);
BridgeDesign unfinished = BridgeDesign.Create(waterY, Ground, Seabed, finished: false);
BridgeDesign finished = BridgeDesign.Create(waterY, Ground, Seabed, finished: true);
int Deck(int o) => BridgeDesign.DeckRow(waterY, o);

// Shape of the bridge.
int cageTiles = (int)Math.Ceiling(WaveDuoRules.CageRadius / 16f);
Check(BridgeDesign.Clearance > cageTiles, "Open air above the deck is taller than the ice-mirror cage radius");
Check(BridgeDesign.UnfinishedEnd > 2 * cageTiles, "Unfinished deck is longer than the cage is wide");
Check(Deck(BridgeDesign.IslandStart / 2) < Deck(1) && Deck(0) == BridgeDesign.DeckY(waterY) &&
      Deck(BridgeDesign.IslandStart) == BridgeDesign.DeckY(waterY), "Deck is cambered: highest mid-span, level at both ends");
Check(Enumerable.Range(1, BridgeDesign.IslandStart - 1).All(o => Math.Abs(Deck(o) - Deck(o - 1)) <= 1),
    "Camber never steps more than one tile");
Check(finished.Cells.Any(c => c.Shape is Shape.CeilingRisesSeaward or Shape.CeilingRisesLandward),
    "Arches have hammered undersides");
Check(Enumerable.Range(0, BridgeDesign.IslandStart).All(o => finished.Has(o, Deck(o))),
    "Finished deck is continuous from the shore to the island");
Check(Enumerable.Range(BridgeDesign.UnfinishedEnd + 1, 5).All(o => !unfinished.Has(o, Deck(o)) ||
      unfinished.At(o, Deck(o))!.Value.Scaffold), "Before completion the stone deck stops at the break");
Check(finished.Has(BridgeDesign.IslandStart, Deck(BridgeDesign.IslandStart)), "Island top is level with the deck");
Check(unfinished.Cells.Any(c => c.Scaffold) && !finished.Cells.Any(c => c.Scaffold),
    "Scaffolding and crane only before completion");
Check(Enumerable.Range(1, 4).Select(i => i * BridgeDesign.PierSpacing).All(p => finished.Has(p, waterY + 1)),
    "Piers stand in the water every 14 tiles");

// Tazuna's hut.
bool HasFixture(BridgeDesign d, Fixture f) => d.Placements.Any(p => p.Fixture == f);
Check(HasFixture(unfinished, Fixture.Door) && HasFixture(unfinished, Fixture.Table) && HasFixture(unfinished, Fixture.Chair),
    "Hut has a door, table and chair from the start");
Check(!HasFixture(unfinished, Fixture.Lantern) && HasFixture(finished, Fixture.Lantern),
    "Hut has no light until the bridge is finished, so no NPC can claim it early");
Check(unfinished.HutFloorY < Ground(unfinished.HutMidOffset), "Hut stands on stilts above the beach");
Check(unfinished.Placements.Count(p => p.Fixture == Fixture.Door) == 2, "Hut has a door on both sides");
// Walking up from either side: every column from the ground to the floor climbs at most one tile at a time.
int TopAt(BridgeDesign d, int o) => d.Cells.Where(c => c.Offset == o && c.Part is Part.Wood or Part.Platform)
    .Select(c => c.Y).DefaultIfEmpty(d.GroundAfterBuild(o)).Min();
bool Climbable(BridgeDesign d, int from, int to, int step)
{
    for (int o = from; o != to; o += step)
        if (TopAt(d, o + step) < TopAt(d, o) - 1)
            return false;
    return true;
}
void CheckHutReachable(BridgeDesign d, string terrain)
{
    int doorFar = d.Placements.Where(p => p.Fixture == Fixture.Door).Min(p => p.Offset);
    int doorNear = d.HutDoorOffset;
    Check(d.GroundAfterBuild(d.HutMidOffset) - d.HutFloorY == BridgeDesign.HutStilts + 1,
        $"Hut floor sits four tiles above the levelled beach ({terrain})");
    Check(Climbable(d, d.LandmostOffset, doorFar, +1), $"Hut can be climbed from inland without jumping ({terrain})");
    Check(Climbable(d, doorNear + BridgeDesign.PorchLength + 6, doorNear, -1),
        $"Hut can be climbed from the beach side ({terrain})");
}
CheckHutReachable(unfinished, "gentle beach");
// The screenshot case: a sand mound under the sea-side stairs and a dip inland of the hut.
int Bumpy(int o) => Ground(o) - (o is >= -16 and <= -8 ? 6 : 0) + (o is >= -40 and <= -30 ? 4 : 0);
CheckHutReachable(BridgeDesign.Create(waterY, Bumpy, Seabed, finished: false), "mound and dip");
Check(unfinished.Cells.Any(c => c.Part == Part.Shingle), "Hut has a tiled roof");
Check(!HasFixture(unfinished, Fixture.BridgeSign) && HasFixture(finished, Fixture.BridgeSign),
    "Naruto Bridge sign only once finished");

// Island.
Check(finished.Cells.Any(c => c.Part == Part.RedBeam) && HasFixture(finished, Fixture.IslandSign) &&
      HasFixture(finished, Fixture.Chest), "Island has a torii gate, the Land of Waves sign and a chest");

// Mist.
Check(Math.Abs(SeaFog(0, false, 1f) - SeaFogOnBridge) < 1e-4 && SeaFogOnBridge >= 3 * PhaseTwoFog,
    "On the bridge by day the mist is over three times the phase-two mist");
Check(SeaFog(0, true, 1f) > SeaFog(0, false, 1f), "Night or rain makes it thicker");
Check(Math.Abs(SeaFog(0, false, 2f) - 2 * SeaFogOnBridge) < 1e-4 && SeaFog(0, false, 5f) == SeaFog(0, false, 2f),
    "Setting can double the mist, no further");
Check(SeaFog(FogFullWithinTiles, false, 1f) == SeaFog(0, false, 1f), "Full mist over the ramp and hut");
Check(SeaFog(50, false, 1f) > 0f && SeaFog(FogReachTiles, true, 1f) == 0f && FogReachTiles <= 80f,
    "Mist is gone a little past the beach");
Check(SeaFog(30, false, 1f) > SeaFog(60, false, 1f), "Mist thins with distance");
Check(SeaFog(0, false, 0f) == 0f && Math.Abs(SeaFog(0, false, 0.5f) - SeaFogOnBridge / 2) < 1e-4,
    "Setting scales or disables the mist");

// Mist preview.
Check(PreviewZabuzaAppear < PreviewZabuzaLine1 && PreviewZabuzaLine1 < PreviewZabuzaLine2 &&
      PreviewZabuzaLine2 < PreviewHakuAppear && PreviewHakuAppear < PreviewHakuLine &&
      PreviewHakuLine < PreviewVanish && PreviewVanish < PreviewLength, "Preview beats happen in order");
Check(PreviewLength / 60f is >= 12f and <= 16f, "Preview lasts about fourteen seconds");
Check(PreviewFogBoost(0) == 0f && PreviewFogBoost(300) == PreviewFog && PreviewFogBoost(PreviewLength) == 0f,
    "Preview mist rises, holds and clears");
Check(!NearBrokenEnd(10, 1) && NearBrokenEnd(BridgeDesign.UnfinishedEnd, 1) && NearBrokenEnd(PreviewTriggerTo, 1),
    "Preview starts only near the broken end");
Check(PreviewZabuzaOffset > PreviewTriggerTo + 10 && PreviewHakuOffset < BridgeDesign.IslandStart,
    "The pair appear over the water a little beyond the scaffolding, short of the island");

// Figures in the mist.
float peak = Enumerable.Range(0, 2000).Max(t => FigureVisibility(t, 2000, 60));
Check(peak <= FigureMaxVisibility + 1e-4 && peak > FigureMaxVisibility - 0.05f, "A mist figure is never fully clear");
Check(Enumerable.Range(100, 1800).All(t => FigureVisibility(t, 2000, 60) >= FigureMinVisibility - 1e-4),
    "Once faded in, a figure never quite disappears until it leaves");
Check(FigureVisibility(0, 200, 30) == 0f && FigureVisibility(200, 200, 30) == 0f, "Figures fade in and out");
Check(FigureDetail is > 0f and < 0.2f, "Only a trace of detail shows through the silhouette");

Check(SightingEligible(true, true, true, false, 20) && !SightingEligible(true, false, true, false, 20) &&
      !SightingEligible(true, true, false, false, 20) && !SightingEligible(true, true, true, true, 20) &&
      !SightingEligible(false, true, true, false, 20) && !SightingEligible(true, true, true, false, 60),
    "Sightings: after the preview, before the win, at night or in rain, near the break, once per night");
Check(SightingNearOffset > PreviewTriggerTo && SightingFarOffset < BridgeDesign.IslandStart,
    "Sightings stand on the water past the scaffolding, short of the island");

Check(SenbonWarningDue(false, true, true, 3, true) && !SenbonWarningDue(true, true, true, 3, true) &&
      !SenbonWarningDue(false, true, true, 2, true) && !SenbonWarningDue(false, true, true, 3, false) &&
      !SenbonWarningDue(false, false, true, 3, true), "Senbon warning: once, with three insignia, near the break");
