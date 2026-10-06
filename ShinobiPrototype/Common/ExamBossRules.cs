using System;

namespace ShinobiPrototype.Common;

public enum DosuMove : byte { None, Drill, Wave, Quake, Ring, Leap }

// The Chūnin Exam fights (specs/M2_中忍考试篇.spec.md sections 3.3–3.5 and 5): Dosu in the prelims, Gaara in the
// finals, Neji after them, and Orochimaru's encounter in the Forest of Death. Numbers are first values for play.
// Kept free of Terraria types so the rule tests can run it.
public static class ExamBossRules
{
    // --- Dosu (prelims): about seven tenths of Zabuza (1800 life after the Eye), a little later in the game.
    public const int DosuLife = 2200;
    public const int DosuDefense = 8;
    public const int DosuDrillWindupTicks = 45;
    public const int DosuWaveWindupTicks = 40;
    public const float DosuWaveKnockback = 11f;
    // Five techniques (user, 2026-10-01: two were far too few beside Zabuza), one phase; under half life every windup
    // is shorter and the sound wave comes twice.
    public const int DosuQuakeWindupTicks = 40;
    public const int DosuRingWindupTicks = 60;
    public const int DosuLeapWindupTicks = 30;
    public const int DosuRingRadiusTiles = 10;
    public const float DosuEnragedWindup = 0.7f;

    public static float DosuWindupScale(int life, int lifeMax) => life * 2 <= lifeMax ? DosuEnragedWindup : 1f;

    public static bool DosuDoubleWave(int life, int lifeMax) => life * 2 <= lifeMax;

    // What he does next, by how far away the player is; never the same technique twice running. roll is in [0, 1).
    public static DosuMove ChooseDosuMove(float tilesAway, DosuMove last, float roll)
    {
        DosuMove[] options = tilesAway < 5f ? new[] { DosuMove.Drill, DosuMove.Ring, DosuMove.Drill, DosuMove.Quake }
            : tilesAway < 14f ? new[] { DosuMove.Quake, DosuMove.Wave, DosuMove.Leap, DosuMove.Drill }
            : new[] { DosuMove.Leap, DosuMove.Wave, DosuMove.Quake };
        int pick = (int)(roll * options.Length) % options.Length;
        for (int i = 0; i < options.Length; i++)
        {
            DosuMove move = options[(pick + i) % options.Length];
            if (move != last)
                return move;
        }
        return options[pick];
    }
    // The Resonating Echo Drill's ringing ears: left and right swap, never longer than two seconds (user, 2026-09-30).
    public const int TinnitusTicks = 100;
    public const int TinnitusMaxTicks = 120;

    public static int Tinnitus(int current, int added) => Math.Min(TinnitusMaxTicks, Math.Max(current, added));

    // --- Gaara (finals, required): three phases by life.
    public const int GaaraLife = 5200;
    public const int GaaraDefense = 14;
    public const float ShieldPhaseTwo = 0.5f;     // the sand armour cracks
    public const float ShieldPhaseThree = 0.25f;  // the partial transformation
    public const int CoffinWarnTicks = 60;
    public const int CoffinRadiusPx = 56;
    public const int CoffinHoldTicks = 90;     // caught: this long to substitute out before the Sand Burial

    public static int GaaraPhase(int life, int lifeMax)
    {
        float share = lifeMax <= 0 ? 0f : life / (float)lifeMax;
        return share <= ShieldPhaseThree ? 3 : share <= ShieldPhaseTwo ? 2 : 1;
    }

    // The Sand Guard (user, 2026-10-02; replaces the old shield that took every hit from the front): every 8 to 10
    // seconds, or sooner once he has taken a lot from the front, a wall of sand rises in front of him and he stands
    // behind it for three seconds. Hits from the front lose 90%, hits from behind land in full. Then two seconds
    // winded, open from every side. Gone once he transforms.
    public const int GuardTicks = 180;
    public const int GuardStaggerTicks = 120;
    public const int GuardIntervalMin = 480;
    public const int GuardIntervalMax = 600;
    public const int GuardEarlyDamage = 260;
    public const float GuardDamageMultiplier = 0.1f;

    public static int GuardInterval(float roll) =>
        GuardIntervalMin + (int)(Math.Clamp(roll, 0f, 1f) * (GuardIntervalMax - GuardIntervalMin));

    public static bool GuardDue(int phase, int ticksSinceGuard, int interval, int frontalDamage) =>
        phase < 3 && (ticksSinceGuard >= interval || frontalDamage >= GuardEarlyDamage);

    public static bool GuardBlocks(bool guarding, int facing, int hitFromSide) => guarding && hitFromSide == facing;

    // His bullets, thicker once the armour cracks: the sand shuriken fan, the quicksand (marked spots on the ground,
    // then pillars of sand) and the pellets of sand he flicks while walking.
    public static int ShurikenFan(int phase) => phase >= 2 ? 7 : 5;
    public const int QuicksandWarnTicks = 50;
    public const int QuicksandGapPx = 96;
    public static int QuicksandSpots(int phase) => phase >= 2 ? 5 : 3;
    public static int PelletEvery(int phase) => phase >= 2 ? 40 : 65;

    // Faster once the armour cracks, faster again when transformed.
    public static float GaaraTempo(int phase) => phase switch { 1 => 1f, 2 => 1.3f, _ => 1.5f };

    // --- Neji (optional sparring).
    public const int NejiLife = 4000;
    public const int NejiDefense = 12;
    public const int RotationRadiusPx = 90;
    public const int RotationTicks = 60;
    public const int SixtyFourWarnTicks = 70;
    public const int SixtyFourRadiusPx = 120;
    // Chakra point seals: each takes this much maximum chakra; at full stacks substitution is sealed for a while.
    public const int SealChakraPerStack = 20;
    public const int SealMaxStacks = 3;
    public const int SealTicks = 600;

    public static int SealedChakra(int stacks) => Math.Clamp(stacks, 0, SealMaxStacks) * SealChakraPerStack;

    public static bool SubstitutionSealed(int stacks) => stacks >= SealMaxStacks;

    public static int AddSeal(int stacks) => Math.Min(SealMaxStacks, stacks + 1);

    // --- Orochimaru's encounter in the Forest of Death: a fight that cannot be won, on the way to the tower (user,
    // 2026-10-01). It ends when he is down to half life (ThresholdRetreatRules), when the player has held out long
    // enough, or when the player falls (he spares them); a player not yet strong enough takes less and holds out less.
    public const int OrochimaruLife = 5000;
    public const int OrochimaruDefense = 12;
    public const int KillingIntentTicks = 120;   // frozen by fear when caught without a log
    // The killing intent is a stare (user, 2026-10-03: it could not be avoided): his eyes glow and a red wedge shows
    // where he is looking for a second; whoever is still inside it, in his sight, when it falls is caught.
    public const int KillingIntentWarnTicks = 60;
    public const float KillingIntentRangePx = 640f;
    public const float KillingIntentHalfAngle = 0.4f;   // about 23 degrees either side

    // Whether a point (dx, dy from his eyes) is inside the stare aimed at `aim` radians.
    public static bool InStare(float dx, float dy, float aim)
    {
        if (dx * dx + dy * dy > KillingIntentRangePx * KillingIntentRangePx)
            return false;
        float diff = MathF.Atan2(dy, dx) - aim;
        diff = MathF.IEEERemainder(diff, MathF.PI * 2f);
        return MathF.Abs(diff) <= KillingIntentHalfAngle;
    }
    public const int SnakeHandReachPx = 560;
    public const float WindBlastKnockback = 14f;
    // The Five Elements Seal: the chakra stops coming back for a while (natural and on-hit recovery; pills still work).
    public const int FiveSealTicks = 480;
    // The shed skin calls him back for another try at the eye (the first meeting always leaves it).
    public const float SharinganVialChance = 0.25f;

    public const int OrochimaruHoldOutTicks = 90 * 60;
    public const int OrochimaruHoldOutWeakTicks = 60 * 60;
    public const float OrochimaruWeakDamage = 0.7f;
    // The giant snake: summoned once, when he is down to three quarters or a while into the fight.
    public const float GiantSnakeAt = 0.75f;
    public const int GiantSnakeAfterTicks = 35 * 60;
    public const int GiantSnakeWarnTicks = 90;

    // Bullets (user, 2026-10-02: Terraria bosses fight with projectiles; more of them, more often). The second half of
    // the fight (after Manda) is denser. Every damage number is in EnemyDamageRules (specs/敌方伤害标准.spec.md).
    public const int VenomPoolTicks = 5 * 60;
    public const int KusanagiReachPx = 760;
    public const int OrochimaruDecideTicks = 35;   // between techniques: a short walk (eased a little, user 2026-10-02)
    public const int OrochimaruRecoveryTicks = 28;

    public static int SwarmCount(bool secondHalf) => secondHalf ? 7 : 5;
    public static int SnakeRainCount(bool secondHalf) => secondHalf ? 7 : 5;
    // Wide gaps between the falling snakes to slip through (user, 2026-10-02).
    public const float SnakeRainGapPx = 88f;
    // While he walks between techniques he flicks small snakes at the player.
    public static int BarrageEvery(bool secondHalf) => secondHalf ? 85 : 120;
    public static int BarrageCount(bool secondHalf) => secondHalf ? 3 : 2;

        public enum OrochimaruEnd { None, HalfLife, HeldOut, PlayerFell }

    public static OrochimaruEnd OrochimaruEnds(int life, int lifeMax, int fightTicks, bool ready, bool playerFell)
    {
        if (playerFell)
            return OrochimaruEnd.PlayerFell;
        if (ThresholdRetreatRules.Reached(life, lifeMax))
            return OrochimaruEnd.HalfLife;
        return fightTicks >= (ready ? OrochimaruHoldOutTicks : OrochimaruHoldOutWeakTicks) ? OrochimaruEnd.HeldOut : OrochimaruEnd.None;
    }

    public static int OrochimaruDamage(int damage, bool ready) => ready ? damage : (int)(damage * OrochimaruWeakDamage);

    public static bool GiantSnakeDue(bool summoned, int life, int lifeMax, int fightTicks) =>
        !summoned && (life <= lifeMax * GiantSnakeAt || fightTicks >= GiantSnakeAfterTicks);

    // Movement locks (sand coffin, killing intent) break the moment the player substitutes.
    public static bool BreaksOnSubstitution(int bindTicks, int fearTicks) => bindTicks > 0 || fearTicks > 0;
}
