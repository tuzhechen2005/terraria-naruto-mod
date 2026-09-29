using System;

namespace ShinobiPrototype.Common;

public static class ZabuzaCombatRules
{
    public const int Approach = 0;
    public const int SlashWindup = 1;
    public const int SlashRecovery = 2;
    public const int WaterWindup = 3;
    public const int WaterRecovery = 4;
    public const int MistTransition = 5;
    public const int MistStep = 6;
    public const int DragonWindup = 7;
    public const int DragonRecovery = 8;
    public const int FanWindup = 9;
    public const int FanRecovery = 10;
    public const int RainWindup = 11;
    public const int RainRecovery = 12;
    public const int SpiralWindup = 13;
    public const int SpiralRecovery = 14;
    public const int DashWindup = 15;
    public const int DashActive = 16;
    public const int DashReaim = 17;
    public const int DashChainActive = 18;
    public const int DashRecovery = 19;
    public const int IdlePose = 0;
    public const int WindupPose = 1;
    public const int SlashPose = 2;
    public const int SealPose = 3;
    public const int RunPose = 4;
    public const int LeapPose = 5;
    public const int RunMidPose = 6;
    public const int RunAltPose = 7;
    public const int BodyWidth = 50;
    public const int BodyHeight = 108;
    public const int CloneBodyWidth = 36;
    public const int CloneBodyHeight = 68;
    public const int BossMaxLife = 1800;

    public const int SlashWidth = 50;
    public const int SlashHeight = 14;
    public const float SlashBladeOffsetY = -18f;
    public const int SlashActiveTicks = 10;
    public const int WaterWidth = 60;
    public const int WaterHeight = 28;
    public const int DragonWidth = 78;
    public const int DragonHeight = 32;
    public const int NeedleWidth = 12;
    public const int NeedleHeight = 6;
    public const float WaterWaveDrawScale = 1.5f;
    public const float WaterDragonDrawScale = 1.3f;
    public const float WaterNeedleDrawScale = 1.35f;
    public const int WaterWaveLifetimeTicks = 180;
    public const int WaterNeedleLifetimeTicks = 190;
    public const int WaterDragonLifetimeTicks = 180;
    public const int SlashWindupTicks = 38;
    public const int WaterWindupTicks = 44;
    public const int SlashRecoveryTicks = 45;
    public const int WaterRecoveryTicks = 42;
    public const int MistTransitionTicks = 120;
    public const int TransitionGatherTick = 35;
    public const int TransitionBurstTick = 78;
    public const int TransitionCloneTick = 96;
    public const int TransitionOpeningAttack = 6;
    public const int MistStepTicks = 30;
    public const int DragonWindupTicks = 62;
    public const int DragonRecoveryTicks = 54;
    public const int FanWindupTicks = 34;
    public const int FanRecoveryTicks = 40;
    public const int RainFirstVolleyTick = 36;
    public const int RainIntervalTicks = 24;
    public const int RainVolleyCount = 3;
    public const int RainWindupTicks = 96;
    public const int RainRecoveryTicks = 56;
    public const int SpiralFirstVolleyTick = 32;
    public const int SpiralIntervalTicks = 8;
    public const int SpiralLastVolleyTick = 88;
    public const int SpiralRecoveryTicks = 58;
    public const float BossDrawScale = 1.35f;
    public const float DemonDrawMultiplier = 1.1f;
    public const int PortraitSize = 44;
    public const float BossBarIconScale = 28f / PortraitSize;
    public const int CloneRespawnTicks = 1200;
    public const int DashActiveTicks = 18;
    public const int DashReaimTicks = 18;
    public const int DashRecoveryTicks = 44;
    public const int CloneAttackPeriodTicks = 150;
    public const int CloneWindupTicks = 30;

    public static bool InMistPhase(int life, int lifeMax) => life <= lifeMax / 2;
    public static bool IsWindup(int state) => state is SlashWindup or WaterWindup or DragonWindup or FanWindup or RainWindup or SpiralWindup or DashWindup or DashReaim;
    public static bool IsRecovery(int state) => state is SlashRecovery or WaterRecovery or DragonRecovery or FanRecovery or RainRecovery or SpiralRecovery or DashRecovery;
    public static int ApproachTicks(bool demonPhase, float horizontalDistance) =>
        horizontalDistance >= 170f ? (demonPhase ? 18 : 30) : (demonPhase ? 26 : 44);
    public static int RecoveryTicks(int state, bool demonPhase)
    {
        int baseTicks = state switch
        {
            SlashRecovery => SlashRecoveryTicks,
            WaterRecovery => WaterRecoveryTicks,
            DragonRecovery => DragonRecoveryTicks,
            FanRecovery => FanRecoveryTicks,
            RainRecovery => RainRecoveryTicks,
            SpiralRecovery => SpiralRecoveryTicks,
            DashRecovery => DashRecoveryTicks,
            _ => throw new ArgumentOutOfRangeException(nameof(state))
        };
        return demonPhase ? Math.Max(30, (int)Math.Round(baseTicks * 0.72f)) : baseTicks;
    }
    public static float TransitionAura(float elapsedTicks) => elapsedTicks switch
    {
        <= 0f => 0.1f,
        < TransitionGatherTick => 0.1f + 0.5f * elapsedTicks / TransitionGatherTick,
        < TransitionBurstTick => 0.6f + 0.4f * (elapsedTicks - TransitionGatherTick) /
            (TransitionBurstTick - TransitionGatherTick),
        _ => 1f
    };
    public static int DashWindupTicks(bool demonPhase) => demonPhase ? 23 : 31;
    public static float DashSpeed(bool demonPhase, float lifeRatio = 1f) =>
        demonPhase ? (lifeRatio <= 0.25f ? 15f : 13.2f) : 9.2f;
    public static bool DashHitWall(float elapsedTicks, bool collideX, bool collideY) =>
        elapsedTicks > 3f && (collideX || collideY);
    public static float CloneDesiredOffset(float parentX, float targetX) => parentX < targetX ? 240f : -240f;
    public static bool CloneCanFire(int bossState) => bossState is not
        (RainWindup or SpiralWindup or DashWindup or DashActive or DashReaim or DashChainActive or MistTransition);
    public static bool IsSlashActive(int state, float elapsedTicks) =>
        state == SlashRecovery && elapsedTicks <= SlashActiveTicks;
    public static bool DrawBodyAfterimage(bool demonPhase, int state, float elapsedTicks,
        float velocityX, float velocityY) =>
        !demonPhase && (IsSlashActive(state, elapsedTicks) ||
            state is DashActive or DashChainActive ||
            (Math.Abs(velocityX) > 4.5f && Math.Abs(velocityY) > 1f));

    public static int RunFrame(float distanceTraveled) =>
        ((int)(Math.Max(0f, distanceTraveled) / 22f) % 4) switch
        {
            1 or 3 => RunMidPose,
            2 => RunAltPose,
            _ => RunPose
        };

    public static int PoseForState(int state, float elapsedTicks,
        bool moving = false, bool airborne = false, float distanceTraveled = 0f) => state switch
    {
        SlashWindup => WindupPose,
        SlashRecovery when elapsedTicks <= SlashActiveTicks => SlashPose,
        DashWindup or DashReaim => WindupPose,
        DashActive or DashChainActive => LeapPose,
        MistTransition when elapsedTicks < TransitionGatherTick => WindupPose,
        MistTransition when elapsedTicks < TransitionBurstTick => SealPose,
        MistTransition => SlashPose,
        WaterWindup or DragonWindup or FanWindup or RainWindup or SpiralWindup => SealPose,
        WaterRecovery when elapsedTicks <= 10f => SealPose,
        DragonRecovery or FanRecovery or RainRecovery or SpiralRecovery when elapsedTicks <= 10f => SealPose,
        MistStep => LeapPose,
        _ when airborne => LeapPose,
        _ when moving => RunFrame(distanceTraveled),
        _ => IdlePose
    };

    public static float AimSlope(float horizontalDistance, float verticalDistance) =>
        Math.Clamp(verticalDistance / Math.Max(Math.Abs(horizontalDistance), 40f), -0.35f, 0.35f);

    public static bool ShouldRespawnClone(float elapsedTicks, bool cloneActive) =>
        elapsedTicks >= CloneRespawnTicks && !cloneActive;

    public static float NextCloneCooldown(float elapsedTicks, bool cloneActive) =>
        cloneActive ? 0f : elapsedTicks + 1f;

    public static bool ShouldLeap(int state, float elapsedTicks, bool grounded,
        float horizontalDistance, bool mistPhase = false) =>
        state == Approach && (elapsedTicks == 12f || (mistPhase && elapsedTicks == 24f)) && grounded &&
        horizontalDistance >= 85f && horizontalDistance <= 260f;

    public static int RainColumnOffset(int column) => column switch
    {
        0 => -156,
        1 => -82,
        2 => 82,
        3 => 156,
        _ => throw new ArgumentOutOfRangeException(nameof(column))
    };

    public static int RainVolleyShift(int volley) => (volley - 1) * 76;

    public static bool IsRainVolley(float elapsedTicks) =>
        elapsedTicks >= RainFirstVolleyTick &&
        elapsedTicks < RainFirstVolleyTick + RainVolleyCount * RainIntervalTicks &&
        (elapsedTicks - RainFirstVolleyTick) % RainIntervalTicks == 0f;

    public static bool IsSpiralVolley(float elapsedTicks) =>
        elapsedTicks >= SpiralFirstVolleyTick && elapsedTicks <= SpiralLastVolleyTick &&
        (elapsedTicks - SpiralFirstVolleyTick) % SpiralIntervalTicks == 0f;

    // The boss cycles attacks by phase and spacing; no dense or ranged attack is
    // selected point blank. Low-life demon form increases charge frequency.
    public static int ChooseAttack(bool mistPhase, int attacksCompleted, float horizontalDistance,
        float lifeRatio = 1f)
    {
        if (horizontalDistance < 85f)
            return SlashWindup;
        if (mistPhase)
        {
            if (lifeRatio <= 0.25f && attacksCompleted > 0 && attacksCompleted % 3 == 0 &&
                horizontalDistance >= 150f)
                return DashWindup;
            if (attacksCompleted > 0 && attacksCompleted % 8 == 6 && horizontalDistance >= 150f)
                return DashWindup;
            if (attacksCompleted > 0 && attacksCompleted % 8 == 2 &&
                horizontalDistance >= 100f && horizontalDistance <= 300f)
                return MistStep;
            if (attacksCompleted > 0 && attacksCompleted % 8 == 3 && horizontalDistance >= 170f)
                return DragonWindup;
            if (attacksCompleted > 0 && attacksCompleted % 8 == 4 && horizontalDistance >= 145f)
                return RainWindup;
            if (attacksCompleted > 0 && attacksCompleted % 8 == 5 && horizontalDistance >= 200f)
                return SpiralWindup;
            if (attacksCompleted > 0 && attacksCompleted % 8 == 5 && horizontalDistance >= 170f)
                return FanWindup;
            if (attacksCompleted > 0 && attacksCompleted % 8 == 1 && horizontalDistance >= 170f)
                return FanWindup;
            if (attacksCompleted > 0 && attacksCompleted % 8 == 7 && horizontalDistance >= 170f)
                return FanWindup;
            if (horizontalDistance >= 170f)
                return WaterWindup;
            return attacksCompleted % 2 == 0 ? SlashWindup : WaterWindup;
        }
        if (horizontalDistance < 145f)
            return SlashWindup;
        if (attacksCompleted % 4 == 2 && horizontalDistance >= 170f)
            return DashWindup;
        return attacksCompleted % 3 == 1 && horizontalDistance >= 170f
            ? FanWindup : WaterWindup;
    }
}
