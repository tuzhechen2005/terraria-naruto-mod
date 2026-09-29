using ShinobiPrototype.Common;

static void Check(bool actual, bool expected, string name)
{
    if (actual != expected)
        throw new Exception($"{name}: expected {expected}, got {actual}");
    Console.WriteLine($"PASS {name}");
}

Check(ExamRules.WaveComplete(false, false), false, "Neither boss defeated");
Check(ExamRules.WaveComplete(true, false), false, "Haku alone is not complete");
Check(ExamRules.WaveComplete(false, true), false, "Zabuza alone is not complete");
Check(ExamRules.WaveComplete(true, true), true, "Both partners defeated completes Wave Country");
Check(ExamRules.CanRegister(false, ExamRules.NotRegistered), false, "Cannot register before Wave Country");
Check(ExamRules.CanRegister(true, ExamRules.NotRegistered), true, "Can register after Wave Country");
Check(ExamRules.CanRegister(true, ExamRules.ForestTrial), false, "Cannot register twice");
Check(ExamRules.HasBothScrolls(1, 0), false, "Heaven alone is insufficient");
Check(ExamRules.HasBothScrolls(0, 1), false, "Earth alone is insufficient");
Check(ExamRules.HasBothScrolls(1, 1), true, "Both scrolls unlock arena preparation");
Check(ExamRules.CanSpawnTrialEnemy(ExamRules.ForestTrial, false, true, true), true, "Enemy spawns during trial");
Check(ExamRules.CanSpawnTrialEnemy(ExamRules.ArenaReady, false, true, true), true, "Enemy remains for recovery");
Check(ExamRules.CanSpawnTrialEnemy(ExamRules.ForestTrial, true, true, true), false, "Enemy stops after completion");
Check(ExamRules.CanSpawnTrialEnemy(ExamRules.ForestTrial, false, false, true), false, "No spawn outside jungle");
Check(ExamRules.CanSpawnTrialEnemy(ExamRules.ForestTrial, false, true, false), false, "Wrong depth does not spawn");
Check(ExamRules.CanChallenge(ExamRules.ForestTrial, true), false, "Arena requires both scrolls");
Check(ExamRules.CanChallenge(ExamRules.ArenaReady, true), true, "Arena ready");
Check(ExamRules.CanChallenge(ExamRules.ArenaReady, false), false, "Arena still requires Wave Country");
