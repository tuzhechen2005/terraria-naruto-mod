namespace ShinobiPrototype.Common;

// The Wave Country mission chain and the lake ambush (M1 spec, "剧情补充"). Kept free of Terraria types so the rule
// tests can run them.
public enum WaveStage
{
    FindTazuna,
    Scout,
    ReportToTazuna,
    GetStronger,
    Lake,
    Bridge,
    Showdown,
    Done,
}

public static class StoryRules
{
    // Strong enough for the lake: the Eye of Cthulhu down, or 200 max life (vanilla bosses are never the only gate).
    public const int LakeLifeThreshold = 200;

    public static bool ReadyForLake(bool downedEye, int lifeMax) => downedEye || lifeMax >= LakeLifeThreshold;

    public static WaveStage Stage(bool waveComplete, bool metTazuna, bool downedBrothers, bool confessed,
        bool lakeDone, bool sawPreview, bool readyForLake)
    {
        if (waveComplete)
            return WaveStage.Done;
        if (lakeDone)
            return sawPreview ? WaveStage.Showdown : WaveStage.Bridge;
        if (confessed)
            return readyForLake ? WaveStage.Lake : WaveStage.GetStronger;
        if (downedBrothers)
            return WaveStage.ReportToTazuna;
        return metTazuna ? WaveStage.Scout : WaveStage.FindTazuna;
    }

    // The mist preview at the broken end waits for the lake: until then the bridge only has fog.
    public static bool PreviewAllowed(bool lakeDone) => lakeDone;

    // The challenge scroll recipe: after the lake and this character's preview; free again once Wave Country is done.
    public static bool ScrollCraftable(bool waveComplete, bool lakeDone, bool sawPreview) =>
        waveComplete || (lakeDone && sawPreview);

    // A lake, not the sea: this far from either world edge (vanilla oceans reach about 380 tiles in).
    public const int OceanMarginTiles = 380;
    public const int LakeMinWidth = 12;
    public const int LakeMinDepth = 3;
    public const float LakeTriggerTiles = 25f;

    public static bool IsLake(int centerX, int worldWidth, int width, int depth) =>
        centerX >= OceanMarginTiles && centerX <= worldWidth - OceanMarginTiles &&
        width >= LakeMinWidth && depth >= LakeMinDepth;

    // Where the nearest lake lies, for the objective text.
    public static string Direction(int dx) => dx < 0 ? "西" : "东";

    // The ambush: numbers are first values for testing in play.
    public const int PrisonLife = 1800;
    public const int PrisonDefense = 8;
    public const int CloneCount = 3;
    public const int CloneLife = 140;
    public const int CloneContactDamage = 22;
    public const int CloneSlashDamage = 26;
    public const int CloneReformTicks = 360;
    public const float AbortRangeTiles = 100f;
    public const int RetryCooldownTicks = 3600;

    // Opening, in ticks from the trigger (about ten seconds); the prison can be hit from PrisonFormed.
    public const int IntroZabuzaAppear = 60;
    public const int IntroZabuzaLine = 90;
    public const int IntroKakashiArrive = 240;
    public const int IntroKakashiLine = 260;
    public const int IntroPrisonCall = 420;
    public const int PrisonFormed = 450;
    public const int IntroKakashiTrapped = 500;
    public const int IntroClonesCall = 620;
    public const int ClonesFormed = 650;

    // After the prison bursts (about twelve seconds).
    public const int OutroKakashiLine = 30;
    public const int OutroZabuzaLine = 150;
    public const int OutroSenbon = 190;
    public const int OutroZabuzaFalls = 220;
    public const int OutroHakuAppear = 280;
    public const int OutroHakuLine = 310;
    public const int OutroVanish = 480;
    public const int OutroKakashiDeduce1 = 540;
    public const int OutroKakashiDeduce2 = 690;
    public const int OutroLength = 780;
}
