namespace ShinobiPrototype.Common;

// Timeline of the epilogue after both Zabuza and Haku have fallen (ticks). Two versions: Haku fell first (Zabuza
// speaks last and crawls to her), or Zabuza fell first (Haku speaks last). Kept free of Terraria types for tests.
public static class WaveEpilogueRules
{
    public readonly record struct Beat(int Tick, string Key);

    public static readonly Beat[] HakuFellFirst =
    {
        new(60, "EpilogueZabuza1"),
        new(220, "EpilogueZabuza2"),
        new(400, "EpilogueZabuza3"),
        new(580, "EpilogueZabuza4"),
        new(820, "EpilogueZabuza5"),
        new(1020, "EpilogueSnow"),
        new(1260, "EpilogueRest"),
    };

    public static readonly Beat[] ZabuzaFellFirst =
    {
        new(60, "EpilogueHaku1"),
        new(260, "EpilogueHaku2"),
        new(460, "EpilogueHaku3"),
        new(660, "EpilogueSnow"),
        new(900, "EpilogueRest"),
    };

    public const int CrawlFrom = 580;
    public const int CrawlTo = 800;
    public const int SnowFromHakuFirst = 960;
    public const int SnowFromZabuzaFirst = 600;
    public const int FadeTicks = 120;
    public const int MusicTicks = 186 * 60; // Sadness and Sorrow runs about three minutes
    public const float MusicLeaveTiles = 200f;

    public static Beat[] Beats(bool hakuFellFirst) => hakuFellFirst ? HakuFellFirst : ZabuzaFellFirst;

    public static int Length(bool hakuFellFirst) => Beats(hakuFellFirst)[^1].Tick + 240 + FadeTicks;

    public static int SnowFrom(bool hakuFellFirst) => hakuFellFirst ? SnowFromHakuFirst : SnowFromZabuzaFirst;
}
