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

    // Haku's side: four lines, the last echoing the snow of the day he found her (user, 2026-09-29).
    public static readonly Beat[] ZabuzaFellFirst =
    {
        new(60, "EpilogueHaku1"),
        new(220, "EpilogueHaku2"),
        new(580, "EpilogueHaku3"),
        new(820, "EpilogueHaku5"),
        new(1020, "EpilogueSnow"),
        new(1260, "EpilogueRest"),
    };

    public const int CrawlFrom = 580;
    public const int CrawlTo = 800;
    public const int SnowFromHakuFirst = 960;
    public const int SnowFromZabuzaFirst = 800;
    public const int FadeTicks = 120;
    public const int MusicTicks = 102 * 60; // the epilogue track (the Need to be Strong file) runs about 1:42
    public const float MusicLeaveTiles = 200f;

    public static Beat[] Beats(bool hakuFellFirst) => hakuFellFirst ? HakuFellFirst : ZabuzaFellFirst;

    public static int Length(bool hakuFellFirst) => Beats(hakuFellFirst)[^1].Tick + 240 + FadeTicks;

    public static int SnowFrom(bool hakuFellFirst) => hakuFellFirst ? SnowFromHakuFirst : SnowFromZabuzaFirst;

    // Snowflakes per tick once it starts: enough to read as snowfall over the whole view.
    public const int SnowPerTick = 4;
}
