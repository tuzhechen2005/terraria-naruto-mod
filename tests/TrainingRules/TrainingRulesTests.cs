using ShinobiPrototype.Common;

static void Equal<T>(T expected, T actual, string name)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
        throw new Exception($"{name}: expected {expected}, got {actual}");
    Console.WriteLine($"PASS {name}");
}

Equal(1, TrainingRules.Next(0, TrainingRules.StyleCount), "First style");
Equal(1, TrainingRules.Next(TrainingRules.StyleCount, TrainingRules.StyleCount), "Style wraps");
Equal(1, TrainingRules.Next(TrainingRules.NatureCount, TrainingRules.NatureCount), "Nature wraps");
Equal(false, TrainingRules.CanGraduate(3, 0, 1), "Style required");
Equal(false, TrainingRules.CanGraduate(3, 1, 0), "Nature required");
Equal(false, TrainingRules.CanGraduate(2, 1, 1), "Kunai training required");
Equal(true, TrainingRules.CanGraduate(3, 1, 1), "Training complete");
Equal(1, TrainingRules.LearningStarCost(2, 2), "Home school cost");
Equal(3, TrainingRules.LearningStarCost(2, 5), "Cross school cost");
