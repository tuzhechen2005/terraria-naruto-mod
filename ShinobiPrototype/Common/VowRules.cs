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

    public static string SchoolName(StyleSchool school) => school switch
    {
        StyleSchool.Sharingan => "写轮眼",
        StyleSchool.EightGates => "八门",
        StyleSchool.Byakugan => "白眼 · 柔拳",
        StyleSchool.Sage => "仙术 · 九尾",
        _ => "无",
    };

    public static string ChapterName(StyleSchool school) => school switch
    {
        StyleSchool.Sharingan => "咒印",
        StyleSchool.EightGates => "凯的修行",
        StyleSchool.Byakugan => "日向宗家与分家",
        StyleSchool.Sage => "妙木山",
        _ => "",
    };
}
