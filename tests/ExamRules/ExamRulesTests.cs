using ShinobiPrototype.Common;
using static ShinobiPrototype.Common.ChuninExamRules;

static void Check(bool condition, string name)
{
    if (!condition)
        throw new Exception(name);
    Console.WriteLine($"PASS {name}");
}

ExamProgress none = default;
ExamProgress recommended = none with { Recommended = true };
ExamProgress written = recommended with { WrittenPassed = true };
ExamProgress issued = written with { Issued = ExamScroll.Earth };
ExamProgress tower = issued with { TowerReached = true };
ExamProgress prelims = tower with { PrelimsPassed = true };

Check(Stage(false, true, prelims, true, false) == ExamStage.Locked, "The exams wait for Wave Country");
Check(Stage(true, false, none, false, false) == ExamStage.NoVillage, "A world without the Leaf cannot host the exams");
Check(Stage(true, true, none, false, false) == ExamStage.Recommend, "First the recommendation from Kakashi");
Check(Stage(true, true, recommended, false, false) == ExamStage.Written, "Then the written test");
Check(Stage(true, true, written, false, false) == ExamStage.ForestGate, "Then the Area 44 gate for a scroll");
Check(Stage(true, true, issued, false, false) == ExamStage.ForestHunt, "Holding a scroll: take the other");
Check(Stage(true, true, tower, false, false) == ExamStage.Prelims, "At the tower with both: the prelims");
Check(Stage(true, true, prelims, false, false) == ExamStage.Training, "Too weak for the finals: train");
Check(Stage(true, true, prelims, true, false) == ExamStage.Finals, "Strong enough: the finals");
Check(Stage(true, true, none, false, true) == ExamStage.Done && Stage(false, false, none, false, true) == ExamStage.Done,
    "Once Gaara has fallen in the world, the exams are over for everyone");

Check(ReadyForOrochimaru(true, 100) && ReadyForOrochimaru(false, 300) && !ReadyForOrochimaru(false, 299),
    "Orochimaru waits for the world's evil boss or 300 life");
Check(ReadyForFinals(true, 100) && ReadyForFinals(false, 400) && !ReadyForFinals(false, 399),
    "The finals wait for Skeletron or 400 life");

Check(WrittenReward(9) == 9 * CopperPerCorrect && WrittenReward(0) == 0 && WrittenReward(12) == 9 * CopperPerCorrect,
    "The written test pays per right answer, nine at most");
Random rng = new(7);
bool distinct = true;
for (int i = 0; i < 200; i++)
{
    int[] drawn = Draw(WrittenExamBank.Questions.Length, WrittenQuestions, rng.Next);
    distinct &= drawn.Length == WrittenQuestions && drawn.Distinct().Count() == WrittenQuestions &&
                drawn.All(q => q >= 0 && q < WrittenExamBank.Questions.Length);
}
Check(distinct, "Nine different questions are drawn every time");
Check(Draw(4, 4, rng.Next).OrderBy(i => i).SequenceEqual(new[] { 0, 1, 2, 3 }), "Shuffling four options keeps all four");
Check(WrittenExamBank.Questions.Length >= 30, "The bank holds at least thirty questions");
Check(WrittenExamBank.Questions.All(q => q.Options.Length == 4 && q.Options.Distinct().Count() == 4 &&
    q.Options.All(o => o.Length > 0) && q.Text.Length > 0), "Every question has four different answers");
Check(WrittenExamBank.Questions.Select(q => q.Text).Distinct().Count() == WrittenExamBank.Questions.Length,
    "No question appears twice");
Check(CanSitWritten(false, false) && !CanSitWritten(true, false) && CanSitWritten(true, true),
    "Giving up at the tenth question waits for the next dawn");

Check(Other(Issue(0)) != Issue(0) && Other(ExamScroll.Heaven) == ExamScroll.Earth && Other(ExamScroll.Earth) == ExamScroll.Heaven,
    "The scroll to take is always the other one");
Check(Issue(0) != Issue(1), "Either scroll can be handed out");
Check(HasBoth(1, 1) && !HasBoth(1, 0) && !HasBoth(0, 2), "Both scrolls are needed");
Check(!SquadDropsScroll(1, 0.9f) && SquadDropsScroll(1, 0.1f) && SquadDropsScroll(SquadPity, 0.99f),
    "A squad carries the scroll by chance, the fifth always");
Check(RainAmbushDue(ExamStage.ForestHunt, false, true, false) && !RainAmbushDue(ExamStage.ForestHunt, true, true, false) &&
      !RainAmbushDue(ExamStage.ForestGate, false, true, false) && !RainAmbushDue(ExamStage.ForestHunt, false, false, false) &&
      !RainAmbushDue(ExamStage.ForestHunt, false, true, true),
    "The Rain genin ambush once, on the jungle surface, while hunting");
Check(CandidatesSpawn(ExamStage.ForestHunt, true) && !CandidatesSpawn(ExamStage.Prelims, true) && !CandidatesSpawn(ExamStage.ForestHunt, false),
    "Candidates only come for someone hunting on the jungle surface");

Check(VowRules.Evaluate(StyleSchool.None, StyleSchool.None, false, 0) == VowOutcome.NoCore, "No core, no vow");
Check(VowRules.Evaluate(StyleSchool.None, StyleSchool.Sharingan, false, 0) == VowOutcome.Vow, "The first vow is free");
Check(VowRules.Evaluate(StyleSchool.Sharingan, StyleSchool.Sharingan, true, 0) == VowOutcome.AlreadyVowed, "Vowing twice changes nothing");
Check(VowRules.Evaluate(StyleSchool.Sharingan, StyleSchool.EightGates, false, 1_000_000) == VowOutcome.NeedsFee,
    "Changing school names the fee first");
Check(VowRules.Evaluate(StyleSchool.Sharingan, StyleSchool.EightGates, true, VowRules.ChangeFee - 1) == VowOutcome.CannotPay &&
      VowRules.Evaluate(StyleSchool.Sharingan, StyleSchool.EightGates, true, VowRules.ChangeFee) == VowOutcome.Change,
    "Changing school costs ten gold");
Check(VowRules.ChapterActive(StyleSchool.Sharingan, StyleSchool.Sharingan) && !VowRules.ChapterActive(StyleSchool.EightGates, StyleSchool.Sharingan) &&
      !VowRules.ChapterActive(StyleSchool.None, StyleSchool.None), "A chapter's rewards work only for one's own school");

foreach (ExamSiteKind kind in Enum.GetValues<ExamSiteKind>())
{
    ExamSiteDesign d = ExamSiteDesign.Create(kind);
    Check(d.Cells.All(c => Math.Abs(c.Dx) <= d.HalfWidth && c.Dy >= -d.ClearHeight && c.Dy <= d.FoundationDepth) &&
          d.Walls.All(w => Math.Abs(w.Dx) <= d.HalfWidth && w.Dy >= -d.ClearHeight) &&
          d.Places.All(p => Math.Abs(p.Dx) <= d.HalfWidth + 4 && p.Dy >= -d.ClearHeight),
        $"{kind}: everything inside the cleared site");
    bool open = true;
    for (int x = -d.HalfWidth; x <= d.HalfWidth; x++)
        for (int y = -3; y <= -1; y++)
            open &= !d.Blocks(x, y);
    Check(open, $"{kind}: a player can walk straight through along the ground");
    Check(Enumerable.Range(-d.HalfWidth, 2 * d.HalfWidth + 1).All(x => d.CellAt(x, 0) is not null),
        $"{kind}: solid ground all the way across");
}
ExamSiteDesign towerSite = ExamSiteDesign.Create(ExamSiteKind.Tower);
KRoom hall = towerSite.Arena;
bool hallClear = true;
for (int x = hall.X0; x <= hall.X1; x++)
    for (int y = hall.Top; y <= hall.Bottom; y++)
        hallClear &= !towerSite.Blocks(x, y);
Check(hallClear && hall.Width >= 25 && hall.Height >= 12, "The tower hall is open and big enough for a bout");
Check(towerSite.Places.Count(p => p.Fix == KFix.Door) == 2, "One door each side of the tower hall");
ExamSiteDesign stadium = ExamSiteDesign.Create(ExamSiteKind.Stadium);
bool sky = true;
for (int x = -ExamSiteDesign.FieldHalf; x <= ExamSiteDesign.FieldHalf; x++)
    for (int y = -stadium.ClearHeight; y <= -1; y++)
        sky &= stadium.CellAt(x, y) is null;
Check(sky && stadium.Arena.Width >= 45, "The stadium field is open to the sky and wide enough for Gaara");
