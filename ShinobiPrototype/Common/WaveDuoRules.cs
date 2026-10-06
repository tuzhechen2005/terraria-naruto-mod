using System;

namespace ShinobiPrototype.Common;

public static class WaveDuoRules
{
    public const int HakuMaxLife = 900; // was 560 (user, 2026-09-30: too low)
    public const int HakuBodyWidth = 28;
    public const int HakuBodyHeight = 68;
    public const int HakuNeedleWindupTicks = 36;
    public const int HakuNeedleRecoveryTicks = 38;
    public const int HakuMirrorWindupTicks = 38;
    public const int HakuDashActiveTicks = 17;
    public const int HakuDashRecoveryTicks = 45;
    public const float HakuNeedleDrawScale = 0.055f;
    public const float PrismShardDrawScale = 0.067f;
    public const int PrismWarningTicks = 28;

    // Haku emerges from a mirror during Zabuza's transition.
    public const int HakuEmergeTicks = 50;
    public const int HakuEmergeLineTick = 20;

    // Mirror cage: both partners alive, the target is trapped in a ring of mirrors.
    public const int CageCooldownTicks = 900;
    public const int CageFormTicks = 40;
    public const int CageHopTicks = 480;
    public const int CageMirrorCount = 8;
    public const float CageRadius = 320f;
    public const int CageMirrorLife = 60;
    public const int CageMirrorsToBreak = 4;
    public const int CageBoundaryHurtCooldown = 30;
    public const int CageStaggerTicks = 90;
    public const int CageEndTicks = 30;
    public const int CageHopCycleTicks = 60;
    public const int CageHopWarnTicks = 20;
    public const int CageHopExposedTicks = 30;
    public const int CageFanNeedles = 3;

    // Haku frenzy after Zabuza falls: mirrors follow the player and respawn.
    public const int FrenzyPauseTicks = 60;
    public const int FrenzyMirrorCount = 6;
    public const float FrenzyRadius = 300f;
    public const int FrenzyMirrorRespawnTicks = 360;
    public const int FrenzyHopCycleTicks = 42;
    public const int FrenzyHopWarnTicks = 14;
    public const int FrenzyHopExposedTicks = 20;
    public const int FrenzyFanNeedles = 5;
    public const float ThousandNeedleLifeRatio = 0.3f;
    public const int ThousandNeedleCooldownTicks = 720;
    public const int ThousandNeedleWarnTicks = 48;
    public const int ThousandNeedleCount = 16;
    public const int ThousandNeedleGapSize = 2;
    public const float ThousandNeedleRadius = 240f;
    public const int ThousandNeedleCastTicks = 70;

    // Zabuza frenzy after Haku falls.
    public const float ZabuzaFrenzyDamageTakenMultiplier = 1.25f;
    public const float ZabuzaFrenzyDashMultiplier = 1.16f;

    public static bool HakuMayAttack(int zabuzaState) =>
        zabuzaState != ZabuzaCombatRules.MistTransition;

    public static bool HakuMayUseStrongAttack(int zabuzaState) => zabuzaState is not
        (ZabuzaCombatRules.MistTransition or ZabuzaCombatRules.RainWindup or
         ZabuzaCombatRules.SpiralWindup or ZabuzaCombatRules.DashWindup or
         ZabuzaCombatRules.DashActive or ZabuzaCombatRules.DashReaim or
         ZabuzaCombatRules.DashChainActive);

    public static float HakuDashSpeed(bool lastStand) => lastStand ? 11.5f : 8.5f;
    public static int HakuDashWindupTicks(bool lastStand) => lastStand ? 26 : 34;

    public static bool CageMayStart(bool hakuAlive, bool zabuzaAlive,
        int zabuzaState, int elapsedCooldown) => hakuAlive && zabuzaAlive &&
        zabuzaState == ZabuzaCombatRules.Approach && elapsedCooldown >= CageCooldownTicks;

    public static bool CageShouldEnd(int hopElapsed, int brokenMirrors) =>
        hopElapsed >= CageHopTicks || brokenMirrors >= CageMirrorsToBreak;

    public static bool CageBrokenEarly(int brokenMirrors) => brokenMirrors >= CageMirrorsToBreak;

    // Angle (radians) of a mirror slot; slot 0 is directly above the center.
    public static float MirrorAngle(int slot, int count) =>
        -MathF.PI / 2f + slot * MathF.Tau / count;

    public enum HopPhase { Warn, Exposed, Fade }

    public static HopPhase PhaseInHop(int cycleTick, bool frenzy)
    {
        int warn = frenzy ? FrenzyHopWarnTicks : CageHopWarnTicks;
        int exposed = frenzy ? FrenzyHopExposedTicks : CageHopExposedTicks;
        if (cycleTick < warn)
            return HopPhase.Warn;
        return cycleTick < warn + exposed ? HopPhase.Exposed : HopPhase.Fade;
    }

    public static int HopCycle(bool frenzy) => frenzy ? FrenzyHopCycleTicks : CageHopCycleTicks;
    public static int HopThrowTick(bool frenzy) => frenzy ? FrenzyHopWarnTicks : CageHopWarnTicks;

    // Deterministic across clients: jump roughly across the ring, skipping broken mirrors.
    public static int NextMirror(int current, bool[] alive, int hopCount)
    {
        int count = alive.Length;
        if (count == 0)
            return -1;
        int start = ((current < 0 ? 0 : current) + count / 2 + (hopCount % 3) - 1) % count;
        for (int k = 0; k < count; k++)
        {
            int index = (start + k) % count;
            if (alive[index] && (index != current || AliveCount(alive) == 1))
                return index;
        }
        return -1;
    }

    public static int AliveCount(bool[] alive)
    {
        int total = 0;
        foreach (bool mirror in alive)
            if (mirror)
                total++;
        return total;
    }

    // Returns true (and the corrected offset) when the player has crossed the cage edge.
    public static bool PushInsideCage(float offsetX, float offsetY, float radius,
        out float pushedX, out float pushedY)
    {
        float distance = MathF.Sqrt(offsetX * offsetX + offsetY * offsetY);
        float limit = radius - 12f;
        if (distance <= limit || distance < 0.001f)
        {
            pushedX = offsetX;
            pushedY = offsetY;
            return false;
        }
        float scale = (radius - 20f) / distance;
        pushedX = offsetX * scale;
        pushedY = offsetY * scale;
        return true;
    }

    public static bool ThousandNeedleReady(float lifeRatio, int cooldownElapsed) =>
        lifeRatio < ThousandNeedleLifeRatio && cooldownElapsed >= ThousandNeedleCooldownTicks;

    // The gap is always ThousandNeedleGapSize consecutive needles, so an escape lane exists.
    public static int ThousandNeedleGapStart(int castIndex) => (castIndex * 5 + 3) % ThousandNeedleCount;

    public static bool ThousandNeedleSkipped(int needle, int gapStart)
    {
        int relative = (needle - gapStart + ThousandNeedleCount) % ThousandNeedleCount;
        return relative < ThousandNeedleGapSize;
    }

    public static bool EncounterComplete(bool hakuDefeated, bool zabuzaDefeated) =>
        hakuDefeated && zabuzaDefeated;
    public static byte NormalizeLegacyWaveFlags(bool hakuDefeated, bool zabuzaDefeated) =>
        EncounterComplete(hakuDefeated, zabuzaDefeated) ? (byte)3 : (byte)0;

    // Candidate spawn offsets for Haku, relative to Zabuza (first) then to the player.
    // The final fallback is Zabuza's own position, so the transition can never stall.
    public static (float X, float Y, bool RelativeToPlayer)[] HakuSpawnCandidates(int zabuzaDirection)
    {
        int back = -zabuzaDirection;
        return new[]
        {
            (back * 70f, -20f, false), (back * 140f, -60f, false),
            (zabuzaDirection * 70f, -20f, false), (0f, -90f, false),
            (back * 200f, -80f, true), (-back * 200f, -80f, true), (0f, -160f, true),
            (0f, 0f, false)
        };
    }
}
