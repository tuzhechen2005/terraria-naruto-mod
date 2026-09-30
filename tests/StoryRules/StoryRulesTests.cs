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
Check(Direction(-5) == "西" && Direction(5) == "东", "Direction words");

Check(IntroZabuzaAppear < IntroKakashiArrive && IntroKakashiArrive < PrisonFormed && PrisonFormed < ClonesFormed,
    "The opening runs Zabuza, Kakashi, prison, clones");
Check(PrisonFormed < IntroKakashiTrapped && IntroKakashiTrapped < IntroClonesCall, "Kakashi speaks from inside the prison");
Check(OutroSenbon < OutroZabuzaFalls && OutroZabuzaFalls < OutroHakuAppear && OutroVanish < OutroKakashiDeduce1 &&
      OutroKakashiDeduce2 < OutroLength, "The ending runs senbon, fall, Haku, vanish, Kakashi's deduction");
Check(RetryCooldownTicks >= 60 * 60, "A failed ambush waits at least a minute");
