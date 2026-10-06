namespace ShinobiPrototype.Common;

// The written test's questions (specs/M2_中忍考试篇.spec.md 3.1): Naruto and Terraria general knowledge. Each is a set
// of text keys (Loc, WrittenExam.Q<n>): the question and four answers, the right one always A0; the panel shuffles
// the options.
public readonly record struct ExamQuestion(string Text, string[] Options);

public static class WrittenExamBank
{
    public const int Count = 31;

    public static readonly ExamQuestion[] Questions = Build();

    private static ExamQuestion[] Build()
    {
        var questions = new ExamQuestion[Count];
        for (int n = 1; n <= Count; n++)
            questions[n - 1] = new ExamQuestion($"WrittenExam.Q{n}.Text",
                new[] { $"WrittenExam.Q{n}.A0", $"WrittenExam.Q{n}.A1", $"WrittenExam.Q{n}.A2", $"WrittenExam.Q{n}.A3" });
        return questions;
    }

    // The tenth question (Ibiki).
    public const string TenthIntro = "WrittenExam.TenthIntro";
    public const string TenthAccepted = "WrittenExam.TenthAccepted";
    public const string TenthGaveUp = "WrittenExam.TenthGaveUp";
}
