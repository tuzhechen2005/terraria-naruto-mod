namespace ShinobiPrototype.Common;

// The mission desk's D-rank missions (specs/空档衔接与火影小兵.spec.md; user 2026-10-04: from the start, taken one
// after another, so each pays little). Kept free of Terraria types for the rule tests.
public enum MissionKind : byte { None, Tora, Letter, Gather, Patrol }

public static class MissionRules
{
    public static readonly MissionKind[] DRank = { MissionKind.Tora, MissionKind.Letter, MissionKind.Gather, MissionKind.Patrol };

    // A mission other than the last one; roll is in [0, 1).
    public static MissionKind Next(MissionKind last, float roll)
    {
        int count = DRank.Length - (last == MissionKind.None ? 0 : 1);
        int pick = System.Math.Min(count - 1, (int)(roll * count));
        foreach (MissionKind kind in DRank)
        {
            if (kind == last)
                continue;
            if (pick-- == 0)
                return kind;
        }
        return DRank[0];
    }

    // Silver and tokens paid at the desk; a letter to someone far away pays one token more.
    public static (int Silver, int Tokens) Reward(MissionKind kind, bool far = false) => kind switch
    {
        MissionKind.Tora => (40, 2),
        MissionKind.Letter => (30, far ? 2 : 1),
        MissionKind.Gather => (30, 1),
        MissionKind.Patrol => (60, 3),
        _ => (0, 0),
    };

    public const int FarLetterTiles = 150;
    public const int PatrolRonin = 3;
    public const int ToraMinTiles = 150;
    public const int ToraMaxTiles = 300;
    public const float ToraFleeTiles = 12f;
    public const float ToraCatchPx = 26f;
    public const float ToraSpeed = 2.6f;
    public const int ToraRunTicks = 3 * 60;
    public const int ToraRestTicks = 70;
}
