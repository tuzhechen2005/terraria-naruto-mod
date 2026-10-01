using System;

namespace ShinobiPrototype.Common;

// Timeline of the epilogue after both Zabuza and Haku have fallen (ticks). Two versions: Haku fell first (Zabuza
// speaks last and walks to her), or Zabuza fell first (Haku speaks last). The one who fell last lies still a moment,
// gets up, and says their lines as they stagger over at a slow, fixed pace; arriving before the last words, they
// stand there until they are said; then they collapse at the other's side, and only then come the snow and the
// closing narration (user, 2026-09-30). How long the walk takes depends on how far apart they lie, so the later beats
// follow it. Kept free of Terraria types for tests.
public static class WaveEpilogueRules
{
    public readonly record struct Beat(int Tick, string Key);

    public static readonly Beat[] ZabuzaLines =
    {
        new(200, "EpilogueZabuza1"),
        new(360, "EpilogueZabuza2"),
        new(540, "EpilogueZabuza3"),
        new(720, "EpilogueZabuza4"),
        new(960, "EpilogueZabuza5"),
    };

    // Haku's side: four lines, the last echoing the snow of the day he found her (user, 2026-09-29).
    public static readonly Beat[] HakuLines =
    {
        new(200, "EpilogueHaku1"),
        new(360, "EpilogueHaku2"),
        new(720, "EpilogueHaku3"),
        new(960, "EpilogueHaku5"),
    };

    public static Beat[] Lines(bool hakuFellFirst) => hakuFellFirst ? ZabuzaLines : HakuLines;

    public enum WalkerPhase { Lying, Rising, Walking, Standing, Collapsing, Down }

    public const int RiseFrom = 90;
    public const int RiseTicks = 90;
    public const int WalkFrom = RiseFrom + RiseTicks;
    // A slow, fixed pace: about two tiles a second. If the other is too far to reach in twenty seconds, the walker
    // falls on the way.
    public const float WalkSpeed = 0.6f;
    public const int MaxWalkTicks = 20 * 60;
    // After the last words, a breath before they fall.
    public const int CollapseAfterLastLine = 120;
    public const int CollapseTicks = 45;
    public const int SnowLineAfterCollapse = 90;
    public const int RestLineAfterSnow = 240;

    // Ticks to cover `gap` pixels at the walking pace.
    public static int WalkTicks(float gap) => (int)Math.Clamp(Math.Max(0f, gap) / WalkSpeed, 0f, MaxWalkTicks);

    public static int ArriveAt(int walkTicks) => WalkFrom + walkTicks;

    public static int CollapseFrom(bool hakuFellFirst, int walkTicks) =>
        Math.Max(ArriveAt(walkTicks), Lines(hakuFellFirst)[^1].Tick + CollapseAfterLastLine);

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
        int collapse = CollapseFrom(hakuFellFirst, walkTicks);
        return tick < RiseFrom ? WalkerPhase.Lying
            : tick < WalkFrom ? WalkerPhase.Rising
            : tick < Math.Min(ArriveAt(walkTicks), collapse) ? WalkerPhase.Walking
            : tick < collapse ? WalkerPhase.Standing
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
