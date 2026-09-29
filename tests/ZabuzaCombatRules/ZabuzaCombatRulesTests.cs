using ShinobiPrototype.Common;

static void Check(bool actual, string name)
{
    if (!actual)
        throw new Exception(name);
    Console.WriteLine($"PASS {name}");
}

Check(ZabuzaCombatRules.SlashWidth == 50 && ZabuzaCombatRules.SlashHeight == 14,
    "Blade collision is narrow and follows the illustrated blade");
Check(ZabuzaCombatRules.BodyWidth >= 48 && ZabuzaCombatRules.BodyHeight >= 104,
    "The enlarged boss body is hittable without treating its sword as a body hitbox");
Check(ZabuzaCombatRules.BossDrawScale >= 1.35f &&
    ZabuzaCombatRules.DemonDrawMultiplier >= 1.08f,
    "Both forms are larger and demon form has extra silhouette weight");
Check(!ZabuzaCombatRules.DrawBodyAfterimage(true, ZabuzaCombatRules.DashActive, 8f, 12f, 0f) &&
    !ZabuzaCombatRules.DrawBodyAfterimage(true, ZabuzaCombatRules.MistStep, 5f, 6f, -2f) &&
    ZabuzaCombatRules.DrawBodyAfterimage(false, ZabuzaCombatRules.DashActive, 8f, 9f, 0f),
    "Demon form never draws a second full-body silhouette while moving");
Check(ZabuzaCombatRules.CloneBodyWidth < ZabuzaCombatRules.BodyWidth &&
    ZabuzaCombatRules.CloneBodyHeight < ZabuzaCombatRules.BodyHeight,
    "Water clone retains a smaller independent hitbox");
Check(ZabuzaCombatRules.BossMaxLife >= 1600,
    "Boss survives long enough to expose its multi-pattern phase");
Check(ZabuzaCombatRules.SlashActiveTicks == 10,
    "Blade collision ends with the release pose");
Check(ZabuzaCombatRules.IsSlashActive(ZabuzaCombatRules.SlashRecovery, 10) &&
    !ZabuzaCombatRules.IsSlashActive(ZabuzaCombatRules.SlashRecovery, 11) &&
    !ZabuzaCombatRules.IsSlashActive(ZabuzaCombatRules.WaterRecovery, 1),
    "Release pose only during damaging slash frames");
Check(ZabuzaCombatRules.WaterWidth >= 58 && ZabuzaCombatRules.WaterHeight >= 26,
    "Larger wave art has a correspondingly larger hitbox");
Check(ZabuzaCombatRules.DragonWidth >= 76 && ZabuzaCombatRules.DragonHeight >= 30 &&
    ZabuzaCombatRules.WaterWaveDrawScale > 1.25f &&
    ZabuzaCombatRules.WaterDragonDrawScale > 1.2f,
    "Wave and dragon are visibly larger while their collision remains deliberate");
Check(ZabuzaCombatRules.NeedleWidth <= 12 && ZabuzaCombatRules.NeedleHeight <= 6,
    "Water needle collision remains smaller than its bright sprite");
Check(ZabuzaCombatRules.SlashWindupTicks >= 30 && ZabuzaCombatRules.WaterWindupTicks >= 40,
    "Both attacks have readable windups");
Check(ZabuzaCombatRules.SlashRecoveryTicks >= 30 && ZabuzaCombatRules.WaterRecoveryTicks >= 30,
    "Both attacks leave retaliation windows");
Check(ZabuzaCombatRules.InMistPhase(325, 650) && !ZabuzaCombatRules.InMistPhase(326, 650),
    "Phase switches at half health");
Check(ZabuzaCombatRules.ChooseAttack(false, 0, 80) == ZabuzaCombatRules.SlashWindup,
    "Close range slash");
Check(ZabuzaCombatRules.ChooseAttack(false, 0, 250) == ZabuzaCombatRules.WaterWindup,
    "Long range water wave");
Check(ZabuzaCombatRules.ChooseAttack(true, 0, 125) != ZabuzaCombatRules.ChooseAttack(true, 1, 125),
    "Mist phase alternates at mid range");
Check(ZabuzaCombatRules.ChooseAttack(true, 0, 250) == ZabuzaCombatRules.WaterWindup,
    "Mist phase does not swing outside blade range");
Check(ZabuzaCombatRules.ChooseAttack(true, 1, 45) == ZabuzaCombatRules.SlashWindup,
    "Mist phase does not spawn a wave inside the player");
Check(ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.Approach, 0) == ZabuzaCombatRules.IdlePose,
    "Approach uses new idle silhouette");
Check(ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.SlashWindup, 10) == ZabuzaCombatRules.WindupPose,
    "Sword telegraph uses raised cleaver pose");
Check(ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.SlashRecovery, 5) == ZabuzaCombatRules.SlashPose &&
    ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.SlashRecovery, 11) == ZabuzaCombatRules.IdlePose,
    "Visible swing and collision end together");
Check(ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.WaterWindup, 10) == ZabuzaCombatRules.SealPose &&
    ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.MistTransition, ZabuzaCombatRules.TransitionGatherTick + 1) == ZabuzaCombatRules.SealPose,
    "Water and mist-gathering use hand-sign pose");
Check(ZabuzaCombatRules.ShouldLeap(ZabuzaCombatRules.Approach, 12, true, 145f) &&
    !ZabuzaCombatRules.ShouldLeap(ZabuzaCombatRules.Approach, 13, true, 145f) &&
    !ZabuzaCombatRules.ShouldLeap(ZabuzaCombatRules.Approach, 12, false, 145f) &&
    !ZabuzaCombatRules.ShouldLeap(ZabuzaCombatRules.SlashWindup, 12, true, 145f),
    "Leap only fires once while approaching from the ground");
Check(ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.Approach, 4, true, false) == ZabuzaCombatRules.RunPose &&
    ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.Approach, 4, true, true) == ZabuzaCombatRules.LeapPose,
    "Chase and leap have distinct animation poses");
Check(ZabuzaCombatRules.ChooseAttack(true, 2, 160f) == ZabuzaCombatRules.MistStep,
    "Mist phase adds a repositioning beat");
Check(ZabuzaCombatRules.ChooseAttack(true, 3, 230f) == ZabuzaCombatRules.DragonWindup,
    "Mist phase unlocks water dragon at range");
Check(ZabuzaCombatRules.ChooseAttack(true, 3, 45f) == ZabuzaCombatRules.SlashWindup,
    "Water dragon is never selected at point blank range");
Check(ZabuzaCombatRules.IsWindup(ZabuzaCombatRules.DragonWindup) &&
    ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.DragonWindup, 20) == ZabuzaCombatRules.SealPose,
    "Water dragon has a readable seal windup");
Check(ZabuzaCombatRules.AimSlope(100f, 200f) <= 0.4f &&
    ZabuzaCombatRules.AimSlope(100f, -200f) >= -0.4f,
    "Water aim cannot fire steeply through floors");
Check(!ZabuzaCombatRules.ShouldRespawnClone(1199, false) &&
    !ZabuzaCombatRules.ShouldRespawnClone(1200, true) &&
    ZabuzaCombatRules.ShouldRespawnClone(1200, false),
    "Water clone has cooldown and one-clone cap");
Check(ZabuzaCombatRules.NextCloneCooldown(1300, true) == 0f &&
    ZabuzaCombatRules.NextCloneCooldown(0, false) == 1f,
    "Clone respawn cooldown starts after the active clone dies");
Check(ZabuzaCombatRules.ChooseAttack(false, 1, 240) == ZabuzaCombatRules.FanWindup &&
    ZabuzaCombatRules.ChooseAttack(false, 1, 50) == ZabuzaCombatRules.SlashWindup,
    "Phase one adds a safe-range fan volley");
Check(ZabuzaCombatRules.ChooseAttack(true, 4, 240) == ZabuzaCombatRules.RainWindup &&
    ZabuzaCombatRules.ChooseAttack(true, 5, 240) == ZabuzaCombatRules.SpiralWindup,
    "Mist phase adds rain and spiral patterns");
Check(ZabuzaCombatRules.ChooseAttack(true, 5, 70) == ZabuzaCombatRules.SlashWindup &&
    ZabuzaCombatRules.ChooseAttack(true, 5, 150) != ZabuzaCombatRules.SpiralWindup,
    "Dense patterns are gated away from close range");
Check(ZabuzaCombatRules.IsWindup(ZabuzaCombatRules.FanWindup) &&
    ZabuzaCombatRules.IsWindup(ZabuzaCombatRules.RainWindup) &&
    ZabuzaCombatRules.IsWindup(ZabuzaCombatRules.SpiralWindup) &&
    ZabuzaCombatRules.RainFirstVolleyTick >= 30 &&
    ZabuzaCombatRules.SpiralFirstVolleyTick >= 25,
    "Bullet patterns have readable lead time");
Check(ZabuzaCombatRules.FanRecoveryTicks >= 40 &&
    ZabuzaCombatRules.RainRecoveryTicks >= 40 &&
    ZabuzaCombatRules.SpiralRecoveryTicks >= 40 &&
    ZabuzaCombatRules.IsRecovery(ZabuzaCombatRules.RainRecovery),
    "Dense patterns leave explicit counterattack windows");
Check(ZabuzaCombatRules.RainColumnOffset(0) < -100 &&
    ZabuzaCombatRules.RainColumnOffset(1) < -50 &&
    ZabuzaCombatRules.RainColumnOffset(2) > 50 &&
    ZabuzaCombatRules.RainColumnOffset(3) > 100,
    "Rain leaves a central dodge lane");
Check(ZabuzaCombatRules.RainVolleyShift(0) < 0 &&
    ZabuzaCombatRules.RainVolleyShift(1) == 0 &&
    ZabuzaCombatRules.RainVolleyShift(2) > 0,
    "Rain lanes sweep across the locked target instead of repeating in place");
Check(ZabuzaCombatRules.IsRainVolley(ZabuzaCombatRules.RainFirstVolleyTick) &&
    ZabuzaCombatRules.IsRainVolley(ZabuzaCombatRules.RainFirstVolleyTick + ZabuzaCombatRules.RainIntervalTicks) &&
    !ZabuzaCombatRules.IsRainVolley(ZabuzaCombatRules.RainFirstVolleyTick + 1) &&
    !ZabuzaCombatRules.IsRainVolley(ZabuzaCombatRules.RainFirstVolleyTick + 3 * ZabuzaCombatRules.RainIntervalTicks),
    "Rain fires exactly three spaced volleys");
Check(ZabuzaCombatRules.ShouldLeap(ZabuzaCombatRules.Approach, 24, true, 170, true) &&
    !ZabuzaCombatRules.ShouldLeap(ZabuzaCombatRules.Approach, 24, true, 170, false),
    "Mist phase has a second short-leap opportunity");
Check(ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.Approach, 10, true, false, 0) == ZabuzaCombatRules.RunPose &&
    ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.Approach, 10, true, false, 23) == ZabuzaCombatRules.RunMidPose &&
    ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.Approach, 10, true, false, 46) == ZabuzaCombatRules.RunAltPose &&
    ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.Approach, 10, true, true, 23) == ZabuzaCombatRules.LeapPose,
    "Ground run uses displacement-driven stride frames without idle flashes");
Check(ZabuzaCombatRules.ChooseAttack(false, 2, 230) == ZabuzaCombatRules.DashWindup &&
    ZabuzaCombatRules.ChooseAttack(true, 6, 230) == ZabuzaCombatRules.DashWindup &&
    ZabuzaCombatRules.ChooseAttack(true, 6, 50) == ZabuzaCombatRules.SlashWindup,
    "Dash joins both phases but never starts point blank");
Check(ZabuzaCombatRules.DashSpeed(true) > ZabuzaCombatRules.DashSpeed(false) &&
    ZabuzaCombatRules.DashWindupTicks(true) >= 20 &&
    ZabuzaCombatRules.DashWindupTicks(false) >= 25 &&
    ZabuzaCombatRules.DashReaimTicks >= 15 &&
    ZabuzaCombatRules.DashRecoveryTicks >= 35,
    "Demon chain dash is faster but has telegraph and recovery");
Check(ZabuzaCombatRules.DashSpeed(true, 0.2f) > ZabuzaCombatRules.DashSpeed(true, 0.5f) &&
    ZabuzaCombatRules.ChooseAttack(true, 9, 230, 0.2f) == ZabuzaCombatRules.DashWindup,
    "Low-life demon form increases charge pressure");
Check(!ZabuzaCombatRules.DashHitWall(1, false, true) &&
    ZabuzaCombatRules.DashHitWall(4, true, false) &&
    ZabuzaCombatRules.DashHitWall(4, false, true) &&
    !ZabuzaCombatRules.DashHitWall(4, false, false),
    "Dash ignores stale takeoff collision then stops at real walls");
Check(ZabuzaCombatRules.CloneDesiredOffset(100, 200) > 0 &&
    ZabuzaCombatRules.CloneDesiredOffset(300, 200) < 0 &&
    Math.Abs(ZabuzaCombatRules.CloneDesiredOffset(100, 200)) >= 200,
    "Clone positions on the far side of its target");
Check(ZabuzaCombatRules.CloneCanFire(ZabuzaCombatRules.Approach) &&
    !ZabuzaCombatRules.CloneCanFire(ZabuzaCombatRules.RainWindup) &&
    !ZabuzaCombatRules.CloneCanFire(ZabuzaCombatRules.SpiralWindup) &&
    !ZabuzaCombatRules.CloneCanFire(ZabuzaCombatRules.DashActive),
    "Clone does not stack needle volleys with major boss patterns");
Check(ZabuzaCombatRules.CloneAttackPeriodTicks >= 150 &&
    ZabuzaCombatRules.CloneWindupTicks >= 25,
    "Clone crossfire has a cooldown and visible preparation");
Check(ZabuzaCombatRules.MistTransitionTicks >= 100 &&
    ZabuzaCombatRules.TransitionGatherTick > 20 &&
    ZabuzaCombatRules.TransitionBurstTick > ZabuzaCombatRules.TransitionGatherTick + 20 &&
    ZabuzaCombatRules.TransitionCloneTick > ZabuzaCombatRules.TransitionBurstTick,
    "Demon transformation has separated gather, burst and clone beats");
Check(ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.MistTransition, 10) == ZabuzaCombatRules.WindupPose &&
    ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.MistTransition, ZabuzaCombatRules.TransitionGatherTick + 1) == ZabuzaCombatRules.SealPose &&
    ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.MistTransition, ZabuzaCombatRules.TransitionBurstTick + 1) == ZabuzaCombatRules.SlashPose,
    "Transformation changes pose at each visual beat");
Check(ZabuzaCombatRules.TransitionAura(1) < ZabuzaCombatRules.TransitionAura(ZabuzaCombatRules.TransitionGatherTick) &&
    ZabuzaCombatRules.TransitionAura(ZabuzaCombatRules.TransitionBurstTick) > 0.9f &&
    ZabuzaCombatRules.TransitionAura(ZabuzaCombatRules.MistTransitionTicks) == 1f,
    "Purple aura builds rather than appearing at full strength instantly");
Check(ZabuzaCombatRules.TransitionOpeningAttack == 6 &&
    ZabuzaCombatRules.ChooseAttack(true, ZabuzaCombatRules.TransitionOpeningAttack, 230f) == ZabuzaCombatRules.DashWindup,
    "Demon form opens with its signature charge at range");
Check(ZabuzaCombatRules.RecoveryTicks(ZabuzaCombatRules.DashRecovery, true) < ZabuzaCombatRules.RecoveryTicks(ZabuzaCombatRules.DashRecovery, false) &&
    ZabuzaCombatRules.RecoveryTicks(ZabuzaCombatRules.RainRecovery, true) >= 30 &&
    ZabuzaCombatRules.RecoveryTicks(ZabuzaCombatRules.SlashRecovery, false) == ZabuzaCombatRules.SlashRecoveryTicks,
    "Demon phase presses faster without eliminating retaliation windows");
Check(Math.Abs(ZabuzaCombatRules.BossBarIconScale * ZabuzaCombatRules.PortraitSize - 28f) < 0.01f,
    "Large portrait fits the vanilla boss-bar icon slot");
Check(ZabuzaCombatRules.WaterWaveLifetimeTicks >= 170 &&
    ZabuzaCombatRules.WaterNeedleLifetimeTicks >= 170 &&
    ZabuzaCombatRules.WaterDragonLifetimeTicks >= 170,
    "Ranged projectiles stay alive across the combat screen");
Check(ZabuzaCombatRules.ApproachTicks(false, 240f) < ZabuzaCombatRules.ApproachTicks(false, 80f) &&
    ZabuzaCombatRules.ApproachTicks(true, 240f) < ZabuzaCombatRules.ApproachTicks(true, 80f) &&
    ZabuzaCombatRules.ApproachTicks(false, 80f) == 44,
    "Long-range attacks arrive more often without accelerating point-blank slashes");
Check(ZabuzaCombatRules.WaterWindupTicks >= 40 &&
    ZabuzaCombatRules.DragonWindupTicks >= 55 &&
    ZabuzaCombatRules.FanWindupTicks >= 30 &&
    ZabuzaCombatRules.WaterRecoveryTicks >= 38 &&
    ZabuzaCombatRules.FanRecoveryTicks >= 38,
    "Higher ranged cadence retains visible telegraphs and retaliation time");
Check(ZabuzaCombatRules.CloneAttackPeriodTicks >= 150 &&
    ZabuzaCombatRules.CloneWindupTicks >= 25 &&
    !ZabuzaCombatRules.CloneCanFire(ZabuzaCombatRules.SpiralWindup),
    "More frequent clone shots still respect dense-pattern exclusion");
