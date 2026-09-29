using ShinobiPrototype.Common;

static void Check(bool actual, string name)
{
    if (!actual)
        throw new Exception(name);
    Console.WriteLine($"PASS {name}");
}

Check(ZabuzaCombatRules.SlashWidth == 60 && ZabuzaCombatRules.SlashHeight == 16 &&
    ZabuzaCombatRules.SlashBladeOffsetY > 0f,
    "Blade collision is narrow and sweeps at the player's height");
Check(ZabuzaCombatRules.BodyHeight is >= 80 and <= 88 && ZabuzaCombatRules.BodyWidth <= 40,
    "Body is about twice the player's height; the sword is not part of the body hitbox");
Check(ZabuzaCombatRules.BossDrawScale == 1f,
    "Pixel art is drawn at 1x with no fractional scaling");
Check(!ZabuzaCombatRules.DrawBodyAfterimage(true, ZabuzaCombatRules.DashActive, 8f, 12f, 0f) &&
    !ZabuzaCombatRules.DrawBodyAfterimage(true, ZabuzaCombatRules.MistStep, 5f, 6f, -2f) &&
    ZabuzaCombatRules.DrawBodyAfterimage(false, ZabuzaCombatRules.DashActive, 8f, 9f, 0f),
    "Demon form never draws a second full-body silhouette while moving");
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
Check(ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.WaterWindup, 10) == ZabuzaCombatRules.SealPose,
    "Water techniques use the hand-sign pose");
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
var aimFlat = ZabuzaCombatRules.DashAim(300f, 0f, 0f, 0f, 11.5f);
var aimUp = ZabuzaCombatRules.DashAim(200f, -180f, 0f, 0f, 11.5f);
var aimLead = ZabuzaCombatRules.DashAim(300f, 0f, 0f, -6f, 11.5f);
var aimOverhead = ZabuzaCombatRules.DashAim(10f, -400f, 0f, 0f, 11.5f);
Check(Math.Abs(aimFlat.X - 1f) < 1e-3 && aimUp.Y < -0.6f && aimLead.Y < -0.2f &&
    aimOverhead.Y / Math.Abs(aimOverhead.X) >= -ZabuzaCombatRules.DashMaxSlope - 1e-3,
    "Dash aims at the player in any direction up to about 60 degrees and leads their movement");
Check(ZabuzaCombatRules.DashActiveTicksFor(300f, 11.5f) * 11.5f > 300f &&
    ZabuzaCombatRules.DashActiveTicksFor(2000f, 11.5f) == ZabuzaCombatRules.DashMaxActiveTicks &&
    ZabuzaCombatRules.DashActiveTicksFor(0f, 11.5f) == ZabuzaCombatRules.DashMinActiveTicks,
    "Dash lasts long enough to reach the player and overshoot, within limits");
Check(ZabuzaCombatRules.DashSpeed(false) > 9.2f && ZabuzaCombatRules.DashSpeed(true) > 13.2f,
    "Dash is faster than before the rework");
Check(!ZabuzaCombatRules.ShouldFlicker(60, 20f) && ZabuzaCombatRules.ShouldFlicker(ZabuzaCombatRules.StuckTicksBeforeFlicker, 5f) &&
    ZabuzaCombatRules.ShouldFlicker(0, ZabuzaCombatRules.FlickerFarTiles),
    "Body Flicker when stuck for two seconds or far from the player");
Check(ZabuzaCombatRules.FlickerVanishTick < ZabuzaCombatRules.FlickerReappearTick &&
    ZabuzaCombatRules.FlickerReappearTick < ZabuzaCombatRules.BodyFlickerTicks,
    "Body Flicker fades out, moves, then fades in");
Check(!ZabuzaCombatRules.DashHitWall(1, false, true) &&
    ZabuzaCombatRules.DashHitWall(4, true, false) &&
    ZabuzaCombatRules.DashHitWall(4, false, true) &&
    !ZabuzaCombatRules.DashHitWall(4, false, false),
    "Dash ignores stale takeoff collision then stops at real walls");
Check(ZabuzaCombatRules.MistTransitionTicks is >= 170 and <= 190 &&
    ZabuzaCombatRules.TransitionHakuTick < ZabuzaCombatRules.TransitionBurstTick &&
    ZabuzaCombatRules.TransitionBurstTick < ZabuzaCombatRules.MistTransitionTicks &&
    ZabuzaCombatRules.TransitionHakuDeadline > ZabuzaCombatRules.MistTransitionTicks,
    "Transition runs kneel, Haku's entrance, then the roar, with a spawn deadline after it");
Check(ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.MistTransition, 10) == ZabuzaCombatRules.KneelPose &&
    ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.MistTransition, ZabuzaCombatRules.TransitionHakuTick + 5) == ZabuzaCombatRules.KneelPose &&
    ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.MistTransition, ZabuzaCombatRules.TransitionBurstTick) == ZabuzaCombatRules.RoarPose,
    "Zabuza kneels until the roar");
Check(ZabuzaCombatRules.TransitionAura(1) < ZabuzaCombatRules.TransitionAura(ZabuzaCombatRules.TransitionHakuTick) &&
    ZabuzaCombatRules.TransitionAura(ZabuzaCombatRules.TransitionBurstTick) == 1f,
    "Purple aura builds and bursts with the roar");
Check(ZabuzaCombatRules.TransitionFog(0) == 0f &&
    ZabuzaCombatRules.TransitionFog(ZabuzaCombatRules.TransitionHakuTick) == 1f &&
    ZabuzaCombatRules.TransitionFog(ZabuzaCombatRules.MistTransitionTicks) < 0.5f,
    "Mist thickens while kneeling and thins after the roar");
Check(!ZabuzaCombatRules.MayDie(0f) && !ZabuzaCombatRules.MayDie(1f) &&
    ZabuzaCombatRules.MayDie(2f) && ZabuzaCombatRules.MayDie(3f) &&
    ZabuzaCombatRules.PhaseOneFloor(1800) == 900,
    "A lethal hit before the transition leaves Zabuza at half health");
Check(ZabuzaCombatRules.ChooseFrenzyAttack(1, 200f) == ZabuzaCombatRules.SwordThrowWindup &&
    ZabuzaCombatRules.ChooseFrenzyAttack(0, 60f) == ZabuzaCombatRules.SlashWindup &&
    ZabuzaCombatRules.ChooseFrenzyAttack(2, 220f) == ZabuzaCombatRules.DashWindup,
    "Frenzy rotates slashes, charges and the thrown blade");
Check(Enumerable.Range(0, 30).All(i =>
        !ZabuzaCombatRules.UsesWater(ZabuzaCombatRules.ChooseFrenzyAttack(i, 60f + i * 12f))),
    "Frenzy never uses water techniques");
Check(!ZabuzaCombatRules.SwordShouldReturn(100f, 10) &&
    ZabuzaCombatRules.SwordShouldReturn(ZabuzaCombatRules.SwordThrowRange, 10) &&
    ZabuzaCombatRules.SwordShouldReturn(0f, ZabuzaCombatRules.SwordThrowMaxTicks / 2) &&
    ZabuzaCombatRules.SwordThrowRange is >= 380f and <= 420f,
    "Thrown blade flies about 25 tiles, then returns");
Check(ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.KunaiDash, 3) == ZabuzaCombatRules.UnarmedPose &&
    ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.SwordCatch, 3) == ZabuzaCombatRules.CatchPose &&
    ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.SwordThrowRelease, 3) == ZabuzaCombatRules.ThrowPose,
    "Throw, kunai charge and catch each have their own pose");
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
