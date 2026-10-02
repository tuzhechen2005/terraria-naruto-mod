using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.NPCs;

// Dosu Kinuta of the Sound, the prelims bout in the tower hall (specs/M2_中忍考试篇.spec.md 3.3). Five techniques
// (user, 2026-10-01), chosen by distance (ExamBossRules.ChooseDosuMove), one phase; under half life every windup is
// shorter and the sound wave comes twice:
// - Resonating Echo Drill, two thrusts (the second turns to follow the player; the ears ring: left and right swap);
// - the sound wave, a cone that throws the player back;
// - the floor quake: a punch into the floor sends sound crawling along it both ways (jump it);
// - the resonance ring: sound spreading out from him in a ring (only the ring itself hurts; get clear or substitute);
// - the leap: up high and down on the player with the drill, the landing spot marked first.
// Art: dosu-base-v1 (Idle, Walk, DrillWindup, Drill, Wave, Hurt) and dosu-moves-v1 (Slam, Leap); until the latter
// arrive the wave and drill frames stand in.
[AutoloadBossHead]
public sealed class Dosu : ExamBoss
{
    private const float Approach = 0f;
    private const float DrillWindup = 1f;
    private const float DrillDash = 2f;
    private const float DrillTurn = 3f;
    private const float DrillRecovery = 4f;
    private const float WaveWindup = 5f;
    private const float WaveRecovery = 6f;
    private const float QuakeWindup = 7f;
    private const float QuakeRecovery = 8f;
    private const float RingWindup = 9f;
    private const float RingRecovery = 10f;
    private const float LeapWindup = 11f;
    private const float LeapAir = 12f;
    private const float LeapRecovery = 13f;

    private const int SecondThrustTicks = 22;

    private DosuMove last;

    protected override Color Tint => new(120, 115, 105);
    protected override int LifeMax => ExamBossRules.DosuLife;
    protected override int Defense => ExamBossRules.DosuDefense;

    protected override string SpritePrefix => "Dosu";

    private static bool HasSlam => BossSprites.Has("Dosu_Slam_0");
    private static bool HasLeap => BossSprites.Has("Dosu_Leap_0");

    private float Scale => ExamBossRules.DosuWindupScale(NPC.life, NPC.lifeMax);

    // Only while those frames are the pose (user, 2026-10-02: late in a recovery the walk took the offset and asked for
    // walk frames that do not exist, and the old placeholder figure showed).
    protected override int PoseFirstFrame => State switch
    {
        QuakeRecovery when HasSlam && Timer < 30f => 1,
        LeapAir when HasLeap => 1,
        LeapRecovery when HasLeap && Timer < 20f => 2,
        _ => 0,
    };

    protected override (string Action, int Frames, int TicksPerFrame, bool Loop) Pose => State switch
    {
        DrillWindup => WithLeadIn(("DrillWindup", 2, 12, true)),
        DrillDash or DrillTurn => ("Drill", 2, 7, false),
        DrillRecovery when Timer < 14f => ("Drill", 2, 7, false),
        WaveWindup or RingWindup => WithLeadIn(("Wave", 1, 10, true)),
        WaveRecovery or RingRecovery when Timer < 24f => ("Wave", 3, 8, false),
        QuakeWindup => HasSlam ? WithLeadIn(("Slam", 1, 10, true)) : ("Wave", 1, 10, true),
        QuakeRecovery when Timer < 30f => HasSlam ? ("Slam", 2, 15, false) : ("Wave", 3, 8, false),
        LeapWindup => HasLeap ? WithLeadIn(("Leap", 1, 10, true)) : ("DrillWindup", 2, 12, true),
        LeapAir => HasLeap ? ("Leap", 1, 10, true) : ("Drill", 1, 10, true),
        LeapRecovery when Timer < 20f => HasLeap ? ("Leap", 1, 10, true) : ("Drill", 2, 7, false),
        _ => Moving("Walk", "Idle", ArtWalkFrames, ArtIdleFrames),
    };

    protected override void Fight(Player target)
    {
        float distance = System.Math.Abs(target.Center.X - NPC.Center.X);
        NPC.damage = State is DrillDash or DrillTurn && Timer < 14f ? ExamBossRules.DosuContactDamage
            : State == LeapAir && NPC.velocity.Y > 0f ? ExamBossRules.DosuLeapDamage
            : 0;
        switch (State)
        {
            case Approach:
                RunTo(target.Center.X - System.Math.Sign(target.Center.X - NPC.Center.X) * 7 * 16, 3.6f, target);
                if (Deciding && Timer > 55f * Scale)
                {
                    last = ExamBossRules.ChooseDosuMove(distance / 16f, last, Main.rand.NextFloat());
                    Enter(last switch
                    {
                        DosuMove.Drill => DrillWindup,
                        DosuMove.Wave => WaveWindup,
                        DosuMove.Quake => QuakeWindup,
                        DosuMove.Ring => RingWindup,
                        _ => LeapWindup,
                    });
                }
                break;

            // --- Resonating Echo Drill: two thrusts.
            case DrillWindup:
                NPC.velocity.X *= 0.8f;
                Face(target.Center.X);
                Telegraph(DustID.Smoke, 26f);
                if (Timer >= ExamBossRules.DosuDrillWindupTicks * Scale)
                {
                    Thrust();
                    Enter(DrillDash);
                }
                break;
            case DrillDash:
                if (Timer > 14f)
                    NPC.velocity.X *= 0.8f;
                if (Timer >= SecondThrustTicks * Scale)
                {
                    Face(target.Center.X);
                    Thrust();
                    Enter(DrillTurn);
                }
                break;
            case DrillTurn:
                if (Timer > 14f)
                    NPC.velocity.X *= 0.85f;
                if (Deciding && Timer >= 18f)
                    Enter(DrillRecovery);
                break;
            case DrillRecovery:
                NPC.velocity.X *= 0.85f;
                if (Deciding && Timer >= 36f)
                    Enter(Approach);
                break;

            // --- The sound wave (twice under half life).
            case WaveWindup:
                NPC.velocity.X *= 0.8f;
                Face(target.Center.X);
                Telegraph(DustID.Cloud, 40f);
                if (Timer >= ExamBossRules.DosuWaveWindupTicks * Scale)
                {
                    Wave();
                    Enter(WaveRecovery);
                }
                break;
            case WaveRecovery:
                NPC.velocity.X *= 0.85f;
                if (Timer == 20f && ExamBossRules.DosuDoubleWave(NPC.life, NPC.lifeMax))
                {
                    Face(target.Center.X);
                    Wave();
                }
                if (Deciding && Timer >= 45f)
                    Enter(Approach);
                break;

            // --- The floor quake: sound crawling along the floor both ways.
            case QuakeWindup:
                NPC.velocity.X *= 0.7f;
                Face(target.Center.X);
                Telegraph(DustID.Smoke, 34f);
                if (Timer >= ExamBossRules.DosuQuakeWindupTicks * Scale)
                {
                    SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                    if (Deciding)
                        foreach (int side in new[] { -1, 1 })
                            JutsuHitbox.Spawn(NPC, JutsuKind.GroundQuake, NPC.Bottom + new Vector2(side * 30f, -22f),
                                new Vector2(side * 6.5f, 0f), 36, 44, ExamBossRules.DosuQuakeDamage, 5f);
                    Enter(QuakeRecovery);
                }
                break;
            case QuakeRecovery:
                NPC.velocity.X *= 0.8f;
                if (Deciding && Timer >= 50f)
                    Enter(Approach);
                break;

            // --- The resonance ring.
            case RingWindup:
                NPC.velocity.X *= 0.75f;
                if (Main.netMode != NetmodeID.Server && Timer % 6f == 0f)
                    for (int i = 0; i < 16; i++)
                        Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2CircularEdge(1f, 1f) *
                            ExamBossRules.DosuRingRadiusTiles * 16f * (Timer / (ExamBossRules.DosuRingWindupTicks * Scale)),
                            DustID.Smoke, Vector2.Zero, 160, new Color(220, 220, 255), 1f).noGravity = true;
                if (Timer >= ExamBossRules.DosuRingWindupTicks * Scale)
                {
                    SoundEngine.PlaySound(SoundID.Item74, NPC.Center);
                    if (Deciding)
                    {
                        int size = ExamBossRules.DosuRingRadiusTiles * 32;
                        JutsuHitbox.Spawn(NPC, JutsuKind.ResonanceRing, NPC.Center, Vector2.Zero, size, size,
                            ExamBossRules.DosuRingDamage, 8f);
                    }
                    Enter(RingRecovery);
                }
                break;
            case RingRecovery:
                NPC.velocity.X *= 0.85f;
                if (Deciding && Timer >= 50f)
                    Enter(Approach);
                break;

            // --- The leap: up high and down on the player.
            case LeapWindup:
                NPC.velocity.X *= 0.7f;
                Face(target.Center.X);
                Telegraph(DustID.Smoke, 20f);
                if (Timer >= ExamBossRules.DosuLeapWindupTicks * Scale)
                {
                    SoundEngine.PlaySound(SoundID.Item24, NPC.Center);
                    NPC.velocity = new Vector2(MathHelper.Clamp((target.Center.X - NPC.Center.X) / 52f, -9f, 9f), -12f);
                    NPC.ai[2] = target.Center.X;
                    Enter(LeapAir);
                }
                break;
            case LeapAir:
                // The landing spot glints on the ground below where he aims.
                if (Main.netMode != NetmodeID.Server && Timer % 4f == 0f)
                    Dust.NewDustPerfect(new Vector2(NPC.ai[2] + Main.rand.NextFloat(-40f, 40f), target.Bottom.Y - 4f), DustID.Smoke,
                        new Vector2(0f, -1f), 120, new Color(220, 220, 255), 1.2f).noGravity = true;
                if (Timer > 8f && NPC.velocity.Y == 0f)
                {
                    SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                    if (Deciding)
                    {
                        JutsuHitbox.Spawn(NPC, JutsuKind.ImpactSlam, NPC.Bottom + new Vector2(0f, -20f), Vector2.Zero, 130, 40,
                            ExamBossRules.DosuLeapDamage, 7f);
                        JutsuHitbox.Spawn(NPC, JutsuKind.EchoDrill, NPC.Center + new Vector2(NPC.direction * 20f, 10f), Vector2.Zero,
                            40, 40, ExamBossRules.DosuDrillDamage);
                    }
                    Enter(LeapRecovery);
                }
                else if (Timer > 150f)
                    Enter(LeapRecovery);
                break;
            case LeapRecovery:
                NPC.velocity.X *= 0.8f;
                if (Deciding && Timer >= 45f)
                    Enter(Approach);
                break;
        }
    }

    private void Thrust()
    {
        NPC.velocity.X = NPC.direction * 10f;
        SoundEngine.PlaySound(SoundID.Item103, NPC.Center);
        if (Deciding)
            JutsuHitbox.Spawn(NPC, JutsuKind.EchoDrill, NPC.Center + new Vector2(NPC.direction * 24f, 0f),
                new Vector2(NPC.direction * 10f, 0f), 40, 40, ExamBossRules.DosuDrillDamage);
    }

    private void Wave()
    {
        SoundEngine.PlaySound(SoundID.Item74, NPC.Center);
        if (Deciding)
            JutsuHitbox.Spawn(NPC, JutsuKind.SoundWave, NPC.Center + new Vector2(NPC.direction * 30f, 0f),
                new Vector2(NPC.direction * 7f, 0f), 60, 90, ExamBossRules.DosuWaveDamage, ExamBossRules.DosuWaveKnockback);
    }

    protected override void LocalVictory(Player player) => player.GetModPlayer<ChuninExamPlayer>().PassPrelims();

    public override void OnKill()
    {
        StoryWorld.DownedDosu = true;
        Tell("月光疾风：“（咳）……胜者，木叶的考生。预选赛合格。”正式赛在一个月后，于木叶的考试会场举行。", new Color(255, 220, 120));
        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.WorldData);
    }
}
