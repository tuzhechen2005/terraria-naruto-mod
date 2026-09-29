using ShinobiPrototype.Common;
using static ShinobiPrototype.Common.BridgeRules;

static void Check(bool condition, string name)
{
    if (!condition)
        throw new Exception(name);
    Console.WriteLine($"PASS {name}");
}

int cageTiles = (int)Math.Ceiling(WaveDuoRules.CageRadius / 16f);
Check(Clearance > cageTiles, "Open air above the deck is taller than the ice-mirror cage radius");
Check(DeckLength > 2 * cageTiles, "Deck is longer than the cage is wide");
Check(DeckY(100) == 95, "Deck sits five tiles above the water");

Check(HasDeckTile(DeckStart, false) && !HasDeckTile(DeckStart - 1, false), "Deck starts three tiles inland");
Check(HasDeckTile(DeckLength - JaggedTiles, false), "Deck is continuous up to the jagged end");
int jagged = Enumerable.Range(FinishFrom, JaggedTiles).Count(o => HasDeckTile(o, false));
Check(jagged > 0 && jagged < JaggedTiles, "Unfinished end is jagged: some tiles, some holes");
Check(Enumerable.Range(DeckLength + 1, GapLength).All(o => !HasDeckTile(o, false)), "Gap is open before completion");
Check(Enumerable.Range(DeckStart, FinishTo - DeckStart + 1).All(o => HasDeckTile(o, true)),
    "Finished deck is continuous from the shore to the island");
Check(FinishTo + 1 == IslandStart, "Finished deck lands on the island");

Check(IsPillar(10, false) && !IsPillar(0, false) && !IsPillar(80, false) && IsPillar(80, true),
    "Pillars every ten tiles; gap pillars only once finished");
Check(IslandSurfaceBelowDeck(IslandCenter) == 1 && IslandSurfaceBelowDeck(IslandStart) == DeckAboveWater,
    "Island is flat at deck height in the middle and meets the water at its ends");

Check(OnDeck(10, 0) && OnDeck(DeckLength, 3) && !OnDeck(-2, 0) && !OnDeck(10, 6), "Preview trigger zone is the deck over the sea");

Check(Math.Abs(SeaFog(0, false, 1f) - PhaseTwoFog) < 1e-4, "On the bridge by day the mist matches phase two");
Check(Math.Abs(SeaFog(0, true, 1f) - PhaseTwoFog * 1.5f) < 1e-4, "Night or rain makes it 1.5 times thicker");
Check(SeaFog(FogReachTiles, true, 1f) == 0f, "No mist beyond 150 tiles");
Check(SeaFog(40, false, 1f) > SeaFog(100, false, 1f), "Mist thins with distance");
Check(SeaFog(0, false, 0f) == 0f && Math.Abs(SeaFog(0, false, 0.5f) - PhaseTwoFog / 2) < 1e-4, "Setting scales or disables the mist");

Check(PreviewZabuzaAppear < PreviewZabuzaLine1 && PreviewZabuzaLine1 < PreviewZabuzaLine2 &&
      PreviewZabuzaLine2 < PreviewHakuAppear && PreviewHakuAppear < PreviewHakuLine &&
      PreviewHakuLine < PreviewVanish && PreviewVanish < PreviewLength, "Preview beats happen in order");
Check(PreviewLength / 60f is >= 7f and <= 9f, "Preview lasts about eight seconds");
Check(PreviewFogBoost(0) == 0f && PreviewFogBoost(200) == PreviewFog && PreviewFogBoost(PreviewLength) == 0f,
    "Preview mist rises, holds and clears");
Check(PreviewZabuzaOffset > DeckLength && PreviewZabuzaOffset < IslandStart, "Zabuza appears over the water in the gap");
