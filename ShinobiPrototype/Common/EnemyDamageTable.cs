using System;
using System.Collections.Generic;

namespace ShinobiPrototype.Common;

// The damage table as rows, for the rule tests: every attack with its damage, kind, baseline and the shortest warning it
// ever gets (specs/敌方伤害标准.spec.md). Apart from EnemyDamageRules so that the rule files it reads can use those
// constants without each test project needing all of them.
public static class EnemyDamageTable
{
    // The whole table, for the rule tests (the same rows as the spec). Telegraphs are the shortest the attack ever gets:
    // the fastest phase, the enraged Dosu, Gaara at his quickest tempo.
    public static IReadOnlyList<EnemyAttack> Table
    {
        get
        {
            float enraged = ExamBossRules.DosuEnragedWindup;
            float tempo = ExamBossRules.GaaraTempo(3);
            return new EnemyAttack[]
            {
                new("Brothers.Contact", EnemyDamageRules.Brothers.Contact, AttackKind.Normal, EnemyDamageRules.Brothers.Baseline, 0),
                new("Brothers.Swipe", EnemyDamageRules.Brothers.Swipe, AttackKind.Big, EnemyDamageRules.Brothers.Baseline, DemonBrotherRules.SwipeWindupTicks),
                new("Brothers.Chain", EnemyDamageRules.Brothers.Chain, AttackKind.Big, EnemyDamageRules.Brothers.Baseline, DemonBrotherRules.ChainWarnTicks),
                new("WaterClone.Contact", EnemyDamageRules.WaterClone.Contact, AttackKind.Normal, EnemyDamageRules.WaterClone.Baseline, 0),
                new("WaterClone.Slash", EnemyDamageRules.WaterClone.Slash, AttackKind.Big, EnemyDamageRules.WaterClone.Baseline, EnemyDamageRules.WaterClone.SlashWindupTicks),
                new("MirrorCage.Boundary", EnemyDamageRules.MirrorCage.Boundary, AttackKind.Normal, EnemyDamageRules.MirrorCage.Baseline, 0),

                new("Zabuza.Slash", EnemyDamageRules.Zabuza.Slash, AttackKind.Big, EnemyDamageRules.Zabuza.Baseline,
                    Math.Min(ZabuzaCombatRules.SlashWindupTicks, ZabuzaCombatRules.FrenzySlashWindupTicks)),
                new("Zabuza.WaterWave", EnemyDamageRules.Zabuza.WaterWave, AttackKind.Big, EnemyDamageRules.Zabuza.Baseline, ZabuzaCombatRules.WaterWindupTicks),
                new("Zabuza.WaterDragon", EnemyDamageRules.Zabuza.WaterDragon, AttackKind.Big, EnemyDamageRules.Zabuza.Baseline, ZabuzaCombatRules.DragonWindupTicks),
                new("Zabuza.NeedleFan", EnemyDamageRules.Zabuza.NeedleFan, AttackKind.Big, EnemyDamageRules.Zabuza.Baseline, ZabuzaCombatRules.FanWindupTicks),
                new("Zabuza.NeedleRain", EnemyDamageRules.Zabuza.NeedleRain, AttackKind.Big, EnemyDamageRules.Zabuza.Baseline, ZabuzaCombatRules.RainIntervalTicks),
                new("Zabuza.NeedleSpiral", EnemyDamageRules.Zabuza.NeedleSpiral, AttackKind.Big, EnemyDamageRules.Zabuza.Baseline, ZabuzaCombatRules.SpiralFirstVolleyTick),
                new("Zabuza.Dash", EnemyDamageRules.Zabuza.Dash, AttackKind.Big, EnemyDamageRules.Zabuza.Baseline,
                    Math.Min(ZabuzaCombatRules.DashWindupTicks(false), ZabuzaCombatRules.DashReaimTicks)),
                new("Zabuza.DemonDash", EnemyDamageRules.Zabuza.DemonDash, AttackKind.Big, EnemyDamageRules.Zabuza.Baseline,
                    Math.Min(ZabuzaCombatRules.DashWindupTicks(true), ZabuzaCombatRules.DashReaimTicks)),
                new("Zabuza.ThrownSword", EnemyDamageRules.Zabuza.ThrownSword, AttackKind.Big, EnemyDamageRules.Zabuza.Baseline, ZabuzaCombatRules.SwordThrowWindupTicks),
                new("Zabuza.KunaiDash", EnemyDamageRules.Zabuza.KunaiDash, AttackKind.Normal, EnemyDamageRules.Zabuza.Baseline, ZabuzaCombatRules.KunaiWindupTicks),

                new("Haku.Needle", EnemyDamageRules.Haku.Needle, AttackKind.Big, EnemyDamageRules.Haku.Baseline, WaveDuoRules.HakuNeedleWindupTicks),
                new("Haku.MirrorDash", EnemyDamageRules.Haku.MirrorDash, AttackKind.Big, EnemyDamageRules.Haku.Baseline,
                    Math.Min(WaveDuoRules.HakuDashWindupTicks(false), WaveDuoRules.HakuDashWindupTicks(true))),
                new("Haku.CageFan", EnemyDamageRules.Haku.CageFan, AttackKind.Normal, EnemyDamageRules.Haku.Baseline, WaveDuoRules.CageHopWarnTicks),
                new("Haku.FrenzyFan", EnemyDamageRules.Haku.FrenzyFan, AttackKind.Normal, EnemyDamageRules.Haku.Baseline, WaveDuoRules.FrenzyHopWarnTicks),
                new("Haku.ExposedNeedle", EnemyDamageRules.Haku.ExposedNeedle, AttackKind.Normal, EnemyDamageRules.Haku.Baseline, 0),
                new("Haku.ThousandNeedles", EnemyDamageRules.Haku.ThousandNeedles, AttackKind.Big, EnemyDamageRules.Haku.Baseline, WaveDuoRules.ThousandNeedleWarnTicks),

                new("Genin.CandidateContact", EnemyDamageRules.Genin.CandidateContact, AttackKind.Normal, EnemyDamageRules.Genin.Baseline, 0),
                new("Genin.CandidateStrike", EnemyDamageRules.Genin.CandidateStrike, AttackKind.Normal, EnemyDamageRules.Genin.Baseline, EnemyDamageRules.Genin.CandidateStrikeWindupTicks),
                new("Genin.CandidateShuriken", EnemyDamageRules.Genin.CandidateShuriken, AttackKind.Normal, EnemyDamageRules.Genin.Baseline, 0),
                new("Genin.RainContact", EnemyDamageRules.Genin.RainContact, AttackKind.Normal, EnemyDamageRules.Genin.Baseline, 0),
                new("Genin.RainSenbon", EnemyDamageRules.Genin.RainSenbon, AttackKind.Normal, EnemyDamageRules.Genin.Baseline, 0),
                new("Genin.RainUmbrella", EnemyDamageRules.Genin.RainUmbrella, AttackKind.Big, EnemyDamageRules.Genin.Baseline, EnemyDamageRules.Genin.RainUmbrellaHangTicks),

                new("Dosu.Drill", EnemyDamageRules.Dosu.Drill, AttackKind.Big, EnemyDamageRules.Dosu.Baseline,
                    Math.Min(EnemyDamageRules.BigWindup(ExamBossRules.DosuDrillWindupTicks * enraged), EnemyDamageRules.Dosu.SecondThrustTicks)),
                new("Dosu.Wave", EnemyDamageRules.Dosu.Wave, AttackKind.Big, EnemyDamageRules.Dosu.Baseline, EnemyDamageRules.BigWindup(ExamBossRules.DosuWaveWindupTicks * enraged)),
                new("Dosu.Quake", EnemyDamageRules.Dosu.Quake, AttackKind.Big, EnemyDamageRules.Dosu.Baseline, EnemyDamageRules.BigWindup(ExamBossRules.DosuQuakeWindupTicks * enraged)),
                new("Dosu.Ring", EnemyDamageRules.Dosu.Ring, AttackKind.Big, EnemyDamageRules.Dosu.Baseline, EnemyDamageRules.BigWindup(ExamBossRules.DosuRingWindupTicks * enraged)),
                new("Dosu.Leap", EnemyDamageRules.Dosu.Leap, AttackKind.Big, EnemyDamageRules.Dosu.Baseline, EnemyDamageRules.BigWindup(ExamBossRules.DosuLeapWindupTicks * enraged)),

                new("Orochimaru.Barrage", EnemyDamageRules.Orochimaru.Barrage, AttackKind.Normal, EnemyDamageRules.Orochimaru.Baseline, 0),
                new("Orochimaru.GiantSnake", EnemyDamageRules.Orochimaru.GiantSnake, AttackKind.Big, EnemyDamageRules.Orochimaru.Baseline, ExamBossRules.GiantSnakeWarnTicks),
                new("Orochimaru.SnakeHands", EnemyDamageRules.Orochimaru.SnakeHands, AttackKind.Big, EnemyDamageRules.Orochimaru.Baseline, EnemyDamageRules.Orochimaru.HandsWindupTicks),
                new("Orochimaru.WindBlast", EnemyDamageRules.Orochimaru.WindBlast, AttackKind.Big, EnemyDamageRules.Orochimaru.Baseline, EnemyDamageRules.Orochimaru.WindWindupTicks),
                new("Orochimaru.DashBite", EnemyDamageRules.Orochimaru.DashBite, AttackKind.Big, EnemyDamageRules.Orochimaru.Baseline, EnemyDamageRules.Orochimaru.DashWindupTicks),
                new("Orochimaru.NeckBite", EnemyDamageRules.Orochimaru.NeckBite, AttackKind.Big, EnemyDamageRules.Orochimaru.Baseline, EnemyDamageRules.Orochimaru.NeckBiteTicks),
                new("Orochimaru.FiveSeal", EnemyDamageRules.Orochimaru.FiveSeal, AttackKind.Big, EnemyDamageRules.Orochimaru.Baseline, EnemyDamageRules.Orochimaru.FiveSealWindupTicks),
                new("Orochimaru.Swarm", EnemyDamageRules.Orochimaru.Swarm, AttackKind.Normal, EnemyDamageRules.Orochimaru.Baseline, EnemyDamageRules.Orochimaru.SwarmWindupTicks),
                new("Orochimaru.Venom", EnemyDamageRules.Orochimaru.Venom, AttackKind.Big, EnemyDamageRules.Orochimaru.Baseline, EnemyDamageRules.Orochimaru.VenomWindupTicks),
                new("Orochimaru.VenomPool", EnemyDamageRules.Orochimaru.VenomPool, AttackKind.Lingering, EnemyDamageRules.Orochimaru.Baseline, 0),
                new("Orochimaru.SnakeRain", EnemyDamageRules.Orochimaru.SnakeRain, AttackKind.Big, EnemyDamageRules.Orochimaru.Baseline,
                    EnemyDamageRules.Orochimaru.SnakeRainWindupTicks + EnemyDamageRules.Orochimaru.SnakeRainHangTicks),
                new("Orochimaru.Kusanagi", EnemyDamageRules.Orochimaru.Kusanagi, AttackKind.Big, EnemyDamageRules.Orochimaru.Baseline, EnemyDamageRules.Orochimaru.KusanagiAimTicks),

                new("Neji.Palm", EnemyDamageRules.Neji.Palm, AttackKind.Normal, EnemyDamageRules.Neji.Baseline, EnemyDamageRules.Neji.FirstPalmTick),
                new("Neji.Rotation", EnemyDamageRules.Neji.Rotation, AttackKind.Big, EnemyDamageRules.Neji.Baseline, EnemyDamageRules.Neji.RotationWindupTicks),
                new("Neji.SixtyFour", EnemyDamageRules.Neji.SixtyFour, AttackKind.Big, EnemyDamageRules.Neji.Baseline, ExamBossRules.SixtyFourWarnTicks),

                new("Gaara.Pellet", EnemyDamageRules.Gaara.Pellet, AttackKind.Normal, EnemyDamageRules.Gaara.Baseline, 0),
                new("Gaara.Quicksand", EnemyDamageRules.Gaara.Quicksand, AttackKind.Big, EnemyDamageRules.Gaara.Baseline, ExamBossRules.QuicksandWarnTicks),
                new("Gaara.Shuriken", EnemyDamageRules.Gaara.Shuriken, AttackKind.Big, EnemyDamageRules.Gaara.Baseline, EnemyDamageRules.BigWindup(EnemyDamageRules.Gaara.ShurikenWindupTicks, tempo)),
                new("Gaara.Burial", EnemyDamageRules.Gaara.Burial, AttackKind.Big, EnemyDamageRules.Gaara.Baseline, EnemyDamageRules.BigWindup(ExamBossRules.CoffinWarnTicks, tempo)),
                new("Gaara.SandWave", EnemyDamageRules.Gaara.SandWave, AttackKind.Big, EnemyDamageRules.Gaara.Baseline, EnemyDamageRules.BigWindup(EnemyDamageRules.Gaara.SandWaveWindupTicks, tempo)),
                new("Gaara.SandArm", EnemyDamageRules.Gaara.SandArm, AttackKind.Big, EnemyDamageRules.Gaara.Baseline, EnemyDamageRules.BigWindup(EnemyDamageRules.Gaara.SandArmWindupTicks, tempo)),
                new("Gaara.AirBullet", EnemyDamageRules.Gaara.AirBullet, AttackKind.Big, EnemyDamageRules.Gaara.Baseline, EnemyDamageRules.BigWindup(EnemyDamageRules.Gaara.AirBulletWindupTicks, tempo)),
            };
        }
    }
}
