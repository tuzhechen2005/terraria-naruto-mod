using System;

namespace ShinobiPrototype.Common;

// Timeline of the epilogue after both Zabuza and Haku have fallen (ticks). Two versions: Haku fell first (Zabuza
// speaks last and walks to her), or Zabuza fell first (Haku speaks last). The one who fell last says every line lying
// where they fell, then gets up and staggers over at a slow, fixed pace, and collapses at the other's side; only then
// the snow and the closing narration (user, 2026-09-30). How long the walk takes depends on how far apart they lie,
// so the later beats follow it. Kept free of Terraria types for tests.
public static class WaveEpilogueRules
{
    public readonly record struct Beat(int Tick, string Key);

    public static readonly Beat[] ZabuzaLines =
    {
        new(60, "EpilogueZabuza1"),
        new(220, "EpilogueZabuza2"),
        new(400, "EpilogueZabuza3"),
        new(580, "EpilogueZabuza4"),
        new(820, "EpilogueZabuza5"),
    };

    // Haku's side: four lines, the last echoing the snow of the day he found her (user, 2026-09-29).
    public static readonly Beat[] HakuLines =
    {
        new(60, "EpilogueHaku1"),
        new(220, "EpilogueHaku2"),
        new(580, "EpilogueHaku3"),
        new(820, "EpilogueHaku5"),
    };

    public static Beat[] Lines(bool hakuFellFirst) => hakuFellFirst ? ZabuzaLines : HakuLines;

    public enum WalkerPhase { Lying, Rising, Walking, Collapsing, Down }

    public const int RiseAfterLastLine = 150;
    public const int RiseTicks = 90;
    // A slow, fixed pace: about two tiles a second. If the other is too far to reach in twenty seconds, the walker
    // falls on the way.
    public const float WalkSpeed = 0.6f;
    public const int MaxWalkTicks = 20 * 60;
    public const int CollapseTicks = 45;
    public const int SnowLineAfterCollapse = 90;
    public const int RestLineAfterSnow = 240;

    public static int RiseFrom(bool hakuFellFirst) => Lines(hakuFellFirst)[^1].Tick + RiseAfterLastLine;

    // Ticks to cover `gap` pixels at the walking pace.
    public static int WalkTicks(float gap) => (int)Math.Clamp(Math.Max(0f, gap) / WalkSpeed, 0f, MaxWalkTicks);

    public static int CollapseFrom(bool hakuFellFirst, int walkTicks) => RiseFrom(hakuFellFirst) + RiseTicks + walkTicks;

    public static int SnowLine(bool hakuFellFirst, int walkTicks) =>
        CollapseFrom(hakuFellFirst, walkTicks) + CollapseTicks + SnowLineAfterCollapse;

    public static int RestLine(bool hakuFellFirst, int walkTicks) => SnowLine(hakuFellFirst, walkTicks) + RestLineAfterSnow;

    public static Beat[] Beats(bool hakuFellFirst, int walkTicks)
    {
        Beat[] lines = Lines(hakuFellFirst);
        Beat[] all = new Beat[lines.Length + 2];
        lines.CopyTo(all, 0);
        all[^2] = new Beat(SnowLine(hakuFellFirst, walkTicks), "EpilogueSnow");
        all[^1] = new Beat(RestLine(hakuFellFirst, walkTicks), "EpilogueRest");
        return all;
    }

    public static WalkerPhase Phase(bool hakuFellFirst, int walkTicks, int tick)
    {
        int rise = RiseFrom(hakuFellFirst);
        int collapse = CollapseFrom(hakuFellFirst, walkTicks);
        return tick < rise ? WalkerPhase.Lying
            : tick < rise + RiseTicks ? WalkerPhase.Rising
            : tick < collapse ? WalkerPhase.Walking
            : tick < collapse + CollapseTicks ? WalkerPhase.Collapsing
            : WalkerPhase.Down;
    }

    public const int FadeTicks = 120;
    public const int MusicTicks = 102 * 60; // the epilogue track (the Need to be Strong file) runs about 1:42
    public const float MusicLeaveTiles = 200f;

    public static int Length(bool hakuFellFirst, int walkTicks) => RestLine(hakuFellFirst, walkTicks) + 240 + FadeTicks;

    // The snow starts a little before its narration line.
    public static int SnowFrom(bool hakuFellFirst, int walkTicks) => SnowLine(hakuFellFirst, walkTicks) - 60;

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
