using System;

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

    // The one who fell last gets up and walks to the other (user, 2026-09-30: sliding there lying down looked eerie):
    // lies still through the first lines, pushes up and stands, staggers over, and collapses at the other's side
    // just before the last line.
    public enum WalkerPhase { Lying, Rising, Walking, Collapsing, Down }

    public const int RiseFrom = 240;
    public const int RiseTicks = 90;
    public const int CollapseFrom = 780;
    public const int CollapseTicks = 45;
    public const float MinWalkSpeed = 0.15f;
    public const float MaxWalkSpeed = 1.1f;

    public static WalkerPhase Phase(int tick) =>
        tick < RiseFrom ? WalkerPhase.Lying
        : tick < RiseFrom + RiseTicks ? WalkerPhase.Rising
        : tick < CollapseFrom ? WalkerPhase.Walking
        : tick < CollapseFrom + CollapseTicks ? WalkerPhase.Collapsing
        : WalkerPhase.Down;

    // Pixels a tick, so the walk across `gap` pixels ends as the collapse begins; slow either way.
    public static float WalkSpeed(float gap, int tick) =>
        Math.Clamp(Math.Abs(gap) / Math.Max(1, CollapseFrom - tick), MinWalkSpeed, MaxWalkSpeed);
    public const int SnowFromHakuFirst = 960;
    public const int SnowFromZabuzaFirst = 800;
    public const int FadeTicks = 120;
    public const int MusicTicks = 102 * 60; // the epilogue track (the Need to be Strong file) runs about 1:42
    public const float MusicLeaveTiles = 200f;

    public static Beat[] Beats(bool hakuFellFirst) => hakuFellFirst ? HakuFellFirst : ZabuzaFellFirst;

    public static int Length(bool hakuFellFirst) => Beats(hakuFellFirst)[^1].Tick + 240 + FadeTicks;

    public static int SnowFrom(bool hakuFellFirst) => hakuFellFirst ? SnowFromHakuFirst : SnowFromZabuzaFirst;

    // Snowflakes per tick at full fall: enough to read as snowfall over the whole view. It thickens gradually from the
    // first flakes (user, 2026-09-30: all at once felt abrupt) and thins out again before the scene ends.
    public const int SnowPerTick = 4;
    public const int SnowFadeInTicks = 360;
    public const int SnowFadeOutTicks = 180;

    public static float SnowRate(int ticksSinceSnow, int ticksLeft) =>
        ticksSinceSnow < 0 ? 0f
            : SnowPerTick * Math.Min(1f, ticksSinceSnow / (float)SnowFadeInTicks) *
              Math.Clamp(ticksLeft / (float)SnowFadeOutTicks, 0f, 1f);
}
