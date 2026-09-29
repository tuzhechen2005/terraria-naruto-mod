namespace ShinobiPrototype.Common;

public static class TrainingRules
{
    public const int StyleCount = 3;
    public const int NatureCount = 5;

    public static int Next(int current, int count) => current >= count ? 1 : current + 1;

    public static bool CanGraduate(int hits, int style, int nature) =>
        hits >= 3 && style >= 1 && style <= StyleCount && nature >= 1 && nature <= NatureCount;

    public static int LearningStarCost(int affinity, int school) =>
        affinity == school ? 1 : 3;

    public static string StyleName(int style) => style switch
    {
        1 => "体术", 2 => "忍术", 3 => "均衡", _ => "未选择"
    };

    public static string NatureName(int nature) => nature switch
    {
        1 => "火", 2 => "水", 3 => "风", 4 => "土", 5 => "雷", _ => "未选择"
    };
}
