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
    // Frenzy (Haku has fallen).
    public const int FrenzyAwaken = 20;
    public const int SwordThrowWindup = 21;
    public const int SwordThrowRelease = 22;
    public const int KunaiWindup = 23;
    public const int KunaiDash = 24;
    public const int SwordCatch = 25;
    // Body Flicker: when stuck or far away he dissolves into mist and reappears behind the player.
    public const int BodyFlicker = 26;
    public const int IdlePose = 0;
    public const int WindupPose = 1;
    public const int SlashPose = 2;
    public const int SealPose = 3;
    public const int RunPose = 4;
    public const int LeapPose = 5;
    public const int RunMidPose = 6;
    public const int RunAltPose = 7;
    public const int KneelPose = 8;
    public const int RoarPose = 9;
    public const int ThrowPose = 10;
    public const int UnarmedPose = 11;
    public const int CatchPose = 12;
    public const int DashPose = 13;
    // About twice the player's height; the sword is a separate hitbox.
    public const int BodyWidth = 36;
    public const int BodyHeight = 84;
    public const int BossMaxLife = 1800;

    public const int SlashWidth = 60;
    public const int SlashHeight = 16;
    public const float SlashBladeOffsetX = 52f;
    public const float SlashBladeOffsetY = 20f;
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
    // Transition: kneel in the mist, Haku steps out of a mirror, then the demon roars.
    public const int MistTransitionTicks = 180;
    public const int TransitionHakuTick = 60;
    public const int TransitionBurstTick = 120;
    public const int TransitionHakuDeadline = 300;
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
    // Sprites are drawn at 1x; pixel art is never scaled by a fractional factor.
    public const float BossDrawScale = 1f;
    public const int PortraitSize = 44;
    public const float BossBarIconScale = 28f / PortraitSize;
    public const int DashActiveTicks = 18;
    // At least the big-attack warning between chained dashes (specs/敌方伤害标准.spec.md; was 18).
    public const int DashReaimTicks = EnemyDamageRules.BigTelegraphTicks;
    public const int DashRecoveryTicks = 44;

    public const int FrenzyAwakenTicks = 60;
    public const int FrenzySlashWindupTicks = 26;
    public const int FrenzyComboSlashes = 2;
    public const int SwordThrowWindupTicks = 34;
    public const int SwordThrowReleaseTicks = 14;
    public const float SwordThrowRange = 400f;
    public const float SwordThrowSpeed = 13f;
    public const int SwordThrowMaxTicks = 150;
    public const int SwordWidth = 56;
    public const int SwordHeight = 56;
    public const int KunaiWindupTicks = 16;
    public const int KunaiDashTicks = 16;
    public const float KunaiDashSpeed = 12.5f;
    public const int SwordCatchTicks = 22;

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
        < TransitionHakuTick => 0.1f + 0.2f * elapsedTicks / TransitionHakuTick,
        < TransitionBurstTick => 0.3f + 0.3f * (elapsedTicks - TransitionHakuTick) /
            (TransitionBurstTick - TransitionHakuTick),
        _ => 1f
    };

    // Screen mist during the transition: rises while kneeling, clears after the roar.
    public static float TransitionFog(float elapsedTicks) => elapsedTicks switch
    {
        <= 0f => 0f,
        < TransitionHakuTick => elapsedTicks / TransitionHakuTick,
        < TransitionBurstTick => 1f,
        < MistTransitionTicks => 1f - 0.7f * (elapsedTicks - TransitionBurstTick) /
            (MistTransitionTicks - TransitionBurstTick),
        _ => 0.3f
    };

    // Before the transition has run, a lethal hit leaves Zabuza at half health instead.
    public static bool MayDie(float transitionStage) => transitionStage >= 2f;
    public static int PhaseOneFloor(int lifeMax) => Math.Max(1, lifeMax / 2);
    // Dash, rebuilt after "the dash never hits" (user, 2026-09-29): aimed in any direction up to about 60 degrees
    // off level, leading the player's movement, passing through terrain like the Eye of Cthulhu, and lasting long
    // enough to reach the player and overshoot. (Before: nearly flat, stopped by the floor after three ticks, 18
    // ticks long.)
    // The demon-phase dash was 20 ticks; a big attack is warned at least 24 (specs/敌方伤害标准.spec.md).
    public static int DashWindupTicks(bool demonPhase) => demonPhase ? EnemyDamageRules.BigTelegraphTicks : 26;
    // Second pass (user, 2026-09-29: "too short, even in the first form"): longer and faster at every stage and
    // growing through the fight. Stage 0 = first form, 1 = demon (mist) phase, 2 = frenzy after Haku falls
    // (frenzy also multiplies speed by WaveDuoRules.ZabuzaFrenzyDashMultiplier).
    public static float DashSpeed(bool demonPhase, float lifeRatio = 1f) =>
        demonPhase ? (lifeRatio <= 0.25f ? 17f : 15.5f) : 12.5f;
    private static readonly int[] OvershootByStage = { 16, 22, 28 };
    private static readonly int[] MinTicksByStage = { 20, 24, 28 };
    private static readonly int[] MaxTicksByStage = { 40, 48, 56 };
    public const int DashMinActiveTicks = 20;
    public const int DashMaxActiveTicks = 56;
    public const float DashLeadMaxTicks = 18f;
    public const float DashMaxSlope = 1.7f; // tan of about 60 degrees
    public const int FrenzyDashChain = 3;

    public static int DashStage(bool demonPhase, bool frenzy) => frenzy ? 2 : demonPhase ? 1 : 0;

    public static int DashActiveTicksFor(float distance, float speed, int stage = 0)
    {
        stage = Math.Clamp(stage, 0, 2);
        return Math.Clamp((int)(distance / Math.Max(1f, speed)) + OvershootByStage[stage],
            MinTicksByStage[stage], MaxTicksByStage[stage]);
    }

    // Unit direction towards where the player will be when the dash arrives.
    public static (float X, float Y) DashAim(float dx, float dy, float targetVelocityX, float targetVelocityY, float speed)
    {
        float distance = MathF.Sqrt(dx * dx + dy * dy);
        float lead = Math.Clamp(distance / Math.Max(1f, speed), 0f, DashLeadMaxTicks);
        float x = dx + targetVelocityX * lead;
        float y = dy + targetVelocityY * lead;
        if (Math.Abs(x) < 1f)
            x = x >= 0f ? 1f : -1f;
        y = Math.Clamp(y, -Math.Abs(x) * DashMaxSlope, Math.Abs(x) * DashMaxSlope);
        float length = MathF.Sqrt(x * x + y * y);
        return (x / length, y / length);
    }

    public const int BodyFlickerTicks = 50;
    public const int FlickerVanishTick = 20;
    public const int FlickerReappearTick = 30;
    public const int StuckTicksBeforeFlicker = 120;
    public const float StuckProgressPerTick = 0.6f;
    public const float FlickerFarTiles = 60f;

    public static bool ShouldFlicker(int stuckTicks, float distanceTiles) =>
        stuckTicks >= StuckTicksBeforeFlicker || distanceTiles >= FlickerFarTiles;
    public static bool DashHitWall(float elapsedTicks, bool collideX, bool collideY) =>
        elapsedTicks > 3f && (collideX || collideY);
    public static bool IsSlashActive(int state, float elapsedTicks) =>
        state == SlashRecovery && elapsedTicks <= SlashActiveTicks;
    public static bool DrawBodyAfterimage(bool demonPhase, int state, float elapsedTicks,
        float velocityX, float velocityY) =>
        (!demonPhase || state == KunaiDash) && (IsSlashActive(state, elapsedTicks) ||
            state == KunaiDash ||
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
        DashActive or DashChainActive => DashPose,
        MistTransition when elapsedTicks < TransitionBurstTick => KneelPose,
        MistTransition => RoarPose,
        FrenzyAwaken => RoarPose,
        SwordThrowWindup => WindupPose,
        SwordThrowRelease => ThrowPose,
        KunaiWindup or KunaiDash => UnarmedPose,
        SwordCatch => CatchPose,
        WaterWindup or DragonWindup or FanWindup or RainWindup or SpiralWindup => SealPose,
        WaterRecovery when elapsedTicks <= 10f => SealPose,
        DragonRecovery or FanRecovery or RainRecovery or SpiralRecovery when elapsedTicks <= 10f => SealPose,
        MistStep => LeapPose,
        BodyFlicker => SealPose,
        _ when airborne => LeapPose,
        _ when moving => RunFrame(distanceTraveled),
        _ => IdlePose
    };

    public static float AimSlope(float horizontalDistance, float verticalDistance) =>
        Math.Clamp(verticalDistance / Math.Max(Math.Abs(horizontalDistance), 40f), -0.35f, 0.35f);

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
    // clearShot: nothing solid between Zabuza and the player. Water jutsu stop at blocks, so from behind cover he
    // dashes (the dash passes through terrain) or closes in with the blade instead (user, 2026-09-29: hiding behind
    // a rock let him spam blocked jutsu forever).
    public static int ChooseAttack(bool mistPhase, int attacksCompleted, float horizontalDistance,
        float lifeRatio = 1f, bool clearShot = true)
    {
        if (horizontalDistance < 85f)
            return SlashWindup;
        if (!clearShot)
            return horizontalDistance >= 150f ? DashWindup : SlashWindup;
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

    // Frenzy: no water techniques; chained slashes, charges and the thrown blade.
    public static int ChooseFrenzyAttack(int attacksCompleted, float horizontalDistance, bool clearShot = true)
    {
        if (horizontalDistance < 100f)
            return SlashWindup;
        if (!clearShot)
            return horizontalDistance >= 150f ? DashWindup : SlashWindup;
        if (attacksCompleted % 3 == 1 && horizontalDistance >= 120f)
            return SwordThrowWindup;
        return horizontalDistance >= 150f ? DashWindup : SlashWindup;
    }

    public static bool UsesWater(int state) => state is WaterWindup or DragonWindup or FanWindup or
        RainWindup or SpiralWindup or MistStep;

    // The thrown blade flies out until its range or time is spent, then homes back.
    public static bool SwordShouldReturn(float traveled, int elapsedTicks) =>
        traveled >= SwordThrowRange || elapsedTicks >= SwordThrowMaxTicks / 2;
}
