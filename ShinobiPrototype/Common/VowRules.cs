namespace ShinobiPrototype.Common;

// The vow (立志, specs/流派系统.spec.md section 6): a character names one school as their own, which opens that
// school's chapter. Style cores stay free to wear either way. Kept free of Terraria types so the rule tests can run it.
public enum VowOutcome : byte
{
    NoCore,          // wearing no style core: nothing to vow to yet
    AlreadyVowed,    // wearing the school already vowed to
    Vow,             // first vow: free
    NeedsFee,        // changing school: say what it costs first
    CannotPay,       // changing school without the fee
    Change,          // changing school, fee paid
}

public static class VowRules
{
    // Changing school costs 10 gold (in copper).
    public const int ChangeFee = 10 * 100 * 100;

    // `confirmed` is the second request in the same conversation, after the fee was named.
    public static VowOutcome Evaluate(StyleSchool vowed, StyleSchool worn, bool confirmed, long coins)
    {
        if (worn == StyleSchool.None)
            return VowOutcome.NoCore;
        if (worn == vowed)
            return VowOutcome.AlreadyVowed;
        if (vowed == StyleSchool.None)
            return VowOutcome.Vow;
        if (!confirmed)
            return VowOutcome.NeedsFee;
        return coins >= ChangeFee ? VowOutcome.Change : VowOutcome.CannotPay;
    }

    // A chapter's rewards only work while its school is the character's own.
    public static bool ChapterActive(StyleSchool vowed, StyleSchool chapterSchool) =>
        vowed != StyleSchool.None && vowed == chapterSchool;

    // Text keys (Loc) for a school's name and its own chapter.
    public static string SchoolKey(StyleSchool school) => "Vow.School." + (school == StyleSchool.None ? "None" : school.ToString());

    public static string ChapterKey(StyleSchool school) => "Vow.Chapter." + (school == StyleSchool.None ? "None" : school.ToString());
}
