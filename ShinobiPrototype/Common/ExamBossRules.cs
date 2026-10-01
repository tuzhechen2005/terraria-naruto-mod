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
    public const int DosuContactDamage = 24;
    public const int DosuDrillDamage = 30;
    public const int DosuWaveDamage = 22;
    public const int DosuDrillWindupTicks = 45;
    public const int DosuWaveWindupTicks = 40;
    public const float DosuWaveKnockback = 11f;
    // Five techniques (user, 2026-10-01: two were far too few beside Zabuza), one phase; under half life every windup
    // is shorter and the sound wave comes twice.
    public const int DosuQuakeDamage = 24;
    public const int DosuRingDamage = 20;
    public const int DosuLeapDamage = 34;
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
    // The Absolute Defence: a hit from the side Gaara faces only chips the sand unless he is recovering from an attack.
    public const float ShieldDamageMultiplier = 0.2f;
    public const int ShurikenDamage = 24;
    public const int ShurikenCount = 5;
    public const int CoffinWarnTicks = 60;
    public const int CoffinRadiusPx = 56;
    public const int CoffinHoldTicks = 90;     // caught: this long to substitute out before the Sand Burial
    public const int BurialDamage = 70;
    public const int SandWaveDamage = 34;
    public const int SandArmDamage = 44;
    public const int AirBulletDamage = 38;

    public static int GaaraPhase(int life, int lifeMax)
    {
        float share = lifeMax <= 0 ? 0f : life / (float)lifeMax;
        return share <= ShieldPhaseThree ? 3 : share <= ShieldPhaseTwo ? 2 : 1;
    }

    // Whether the shield takes the hit: from the front, and Gaara not in an attack's recovery.
    public static bool ShieldBlocks(int phase, int facing, int hitFromSide, bool recovering) =>
        phase == 1 && !recovering && hitFromSide == facing;

    // Faster once the armour cracks, faster again when transformed.
    public static float GaaraTempo(int phase) => phase switch { 1 => 1f, 2 => 1.3f, _ => 1.5f };

    // --- Neji (optional sparring).
    public const int NejiLife = 4000;
    public const int NejiDefense = 12;
    public const int PalmDamage = 30;
    public const int RotationDamage = 26;
    public const int RotationRadiusPx = 90;
    public const int RotationTicks = 60;
    public const int SixtyFourWarnTicks = 70;
    public const int SixtyFourRadiusPx = 120;
    public const int SixtyFourDamage = 64;
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
    public const int KillingIntentTicks = 120;   // frozen by fear unless the player substitutes out
    public const int SnakeHandDamage = 34;
    public const int SnakeHandReachPx = 420;
    public const int SnakeDashDamage = 38;
    public const int WindBlastDamage = 30;
    public const float WindBlastKnockback = 14f;
    public const int NeckBiteDamage = 42;
    // The Five Elements Seal: the chakra stops coming back for a while (natural and on-hit recovery; pills still work).
    public const int FiveSealDamage = 24;
    public const int FiveSealTicks = 480;
    public const int SnakeLife = 220;
    public const int SnakeDamage = 26;
    // Summons: once above half life, once more on the way down (the fight ends at half).
    public const float SnakeSummonAt = 0.8f;
    public const float SecondSnakeSummonAt = 0.62f;

    public static bool SummonSnakes(float lifeShare, int summonsSoFar) =>
        summonsSoFar == 0 && lifeShare <= SnakeSummonAt || summonsSoFar == 1 && lifeShare <= SecondSnakeSummonAt;

    // The shed skin calls him back for another try at the eye (the first meeting always leaves it).
    public const float SharinganVialChance = 0.25f;

    public const int OrochimaruHoldOutTicks = 90 * 60;
    public const int OrochimaruHoldOutWeakTicks = 60 * 60;
    public const float OrochimaruWeakDamage = 0.7f;
    // The giant snake: summoned once, when he is down to three quarters or a while into the fight.
    public const float GiantSnakeAt = 0.75f;
    public const int GiantSnakeAfterTicks = 35 * 60;
    public const int GiantSnakeDamage = 46;
    public const int GiantSnakeWarnTicks = 90;

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
