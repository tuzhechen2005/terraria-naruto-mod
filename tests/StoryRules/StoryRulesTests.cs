using ShinobiPrototype.Common;
using static ShinobiPrototype.Common.StoryRules;

static void Check(bool condition, string name)
{
    if (!condition)
        throw new Exception(name);
    Console.WriteLine($"PASS {name}");
}

Check(Stage(false, false, false, false, false, false, false) == WaveStage.FindTazuna, "A new world starts by finding Tazuna");
Check(Stage(false, true, false, false, false, false, false) == WaveStage.Scout, "After meeting him, scout for Mist ninja");
Check(Stage(false, false, true, false, false, false, false) == WaveStage.ReportToTazuna &&
      Stage(false, true, true, false, false, false, false) == WaveStage.ReportToTazuna,
    "The brothers down sends the player back to Tazuna, met or not");
Check(Stage(false, true, true, true, false, false, false) == WaveStage.GetStronger &&
      Stage(false, true, true, true, false, false, true) == WaveStage.Lake, "The lake waits until the player is ready");
Check(Stage(false, true, true, true, true, false, false) == WaveStage.Bridge &&
      Stage(false, true, true, true, true, true, false) == WaveStage.Showdown, "After the lake: the bridge, then the fight");
Check(Stage(true, false, false, false, false, false, false) == WaveStage.Done, "A finished Wave Country is done");
Check(Stage(false, false, false, false, true, true, false) == WaveStage.Showdown,
    "The lake done moves on even if earlier flags are missing");

Check(ReadyForLake(true, 100) && ReadyForLake(false, 200) && !ReadyForLake(false, 180),
    "Ready: the Eye down or 200 max life");
Check(!PreviewAllowed(false, false) && PreviewAllowed(true, false), "The mist preview waits for the lake");
Check(!PreviewAllowed(true, true) && !TeasersAllowed(true) && TeasersAllowed(false),
    "Once Zabuza has been fought, no more figures in the mist");
Check(!ScrollCraftable(false, false, true, false) && !ScrollCraftable(false, true, false, false) &&
      ScrollCraftable(false, true, true, false) && ScrollCraftable(true, false, false, false),
    "Scroll recipe: after the lake and the preview, always once done");
Check(ScrollCraftable(false, true, false, true), "A character who missed the preview can still craft once Zabuza has been fought");

Check(IsLake(2000, 4200, 20, 5), "A wide pond inland is a lake");
Check(!IsLake(200, 4200, 60, 30) && !IsLake(4000, 4200, 60, 30), "The oceans are not lakes");
Check(!IsLake(2000, 4200, 8, 5) && !IsLake(2000, 4200, 20, 2), "Puddles are not lakes");
Check(Direction(-5) == "Dir.West" && Direction(5) == "Dir.East", "Direction text keys");

Check(IntroZabuzaAppear < IntroKakashiArrive && IntroKakashiArrive < PrisonFormed && PrisonFormed < ClonesFormed,
    "The opening runs Zabuza, Kakashi, prison, clones");
Check(PrisonFormed < IntroKakashiTrapped && IntroKakashiTrapped < IntroClonesCall, "Kakashi speaks from inside the prison");
Check(OutroSenbon < OutroZabuzaFalls && OutroZabuzaFalls < OutroHakuAppear && OutroVanish < OutroKakashiDeduce1 &&
      OutroKakashiDeduce2 < OutroLength, "The ending runs senbon, fall, Haku, vanish, Kakashi's deduction");
Check(RetryCooldownTicks >= 60 * 60, "A failed ambush waits at least a minute");

Check(WaveEpilogueRules.SnowRate(-1, 500) == 0f && WaveEpilogueRules.SnowRate(0, 500) == 0f,
    "No snow before it starts");
Check(WaveEpilogueRules.SnowRate(WaveEpilogueRules.SnowFadeInTicks / 2, 1000) < WaveEpilogueRules.SnowPerTick &&
      WaveEpilogueRules.SnowRate(WaveEpilogueRules.SnowFadeInTicks, 1000) == WaveEpilogueRules.SnowPerTick,
    "Snow thickens gradually to full fall");
Check(WaveEpilogueRules.SnowRate(2000, WaveEpilogueRules.SnowFadeOutTicks / 2) < WaveEpilogueRules.SnowPerTick &&
      WaveEpilogueRules.SnowRate(2000, 0) == 0f, "Snow thins out before the scene ends");

// The epilogue walk (user, 2026-09-30): fall, rise, speak while staggering over at a slow fixed pace, stand if early,
// collapse after the last words, and only then the snow and the closing narration.
{
    int near = WaveEpilogueRules.WalkTicks(120f), far = WaveEpilogueRules.WalkTicks(900f);
    Check(WaveEpilogueRules.ZabuzaLines[0].Tick >= WaveEpilogueRules.WalkFrom && WaveEpilogueRules.HakuLines[0].Tick >= WaveEpilogueRules.WalkFrom,
        "The lines start once the walker is on their feet, so they are spoken walking");
    Check(WaveEpilogueRules.Phase(true, near, 0) == WaveEpilogueRules.WalkerPhase.Lying &&
          WaveEpilogueRules.Phase(true, near, WaveEpilogueRules.RiseFrom) == WaveEpilogueRules.WalkerPhase.Rising &&
          WaveEpilogueRules.Phase(true, near, WaveEpilogueRules.WalkFrom) == WaveEpilogueRules.WalkerPhase.Walking &&
          WaveEpilogueRules.Phase(true, near, WaveEpilogueRules.ArriveAt(near)) == WaveEpilogueRules.WalkerPhase.Standing,
        "Near: lying, rising, walking, then standing beside the other until the words are said");
    Check(WaveEpilogueRules.CollapseFrom(true, near) == WaveEpilogueRules.ZabuzaLines[^1].Tick + WaveEpilogueRules.CollapseAfterLastLine,
        "Near: the collapse comes after the last words");
    Check(WaveEpilogueRules.CollapseFrom(true, far) == WaveEpilogueRules.ArriveAt(far) &&
          WaveEpilogueRules.Phase(true, far, WaveEpilogueRules.ArriveAt(far) - 1) == WaveEpilogueRules.WalkerPhase.Walking,
        "Far: still walking after the words, collapsing on arrival");
    Check(Math.Abs(near * WaveEpilogueRules.WalkSpeed - 120f) < 1f && WaveEpilogueRules.WalkSpeed <= 0.8f,
        "One slow pace, as long as the distance takes");
    Check(WaveEpilogueRules.WalkTicks(100000f) == WaveEpilogueRules.MaxWalkTicks && WaveEpilogueRules.WalkTicks(-5f) == 0,
        "Too far to reach: the walker falls on the way");
    WaveEpilogueRules.Beat[] beats = WaveEpilogueRules.Beats(true, far);
    Check(beats[^2].Key == "EpilogueSnow" && beats[^2].Tick > WaveEpilogueRules.CollapseFrom(true, far) + WaveEpilogueRules.CollapseTicks &&
          WaveEpilogueRules.Length(true, far) > beats[^1].Tick, "The snow and the narration follow the collapse");
}
