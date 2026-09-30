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

// Dosu Kinuta of the Sound, the prelims bout in the tower hall (specs/M2_中忍考试篇.spec.md 3.3): the Resonating
// Echo Drill up close (low damage, the ears ring: left and right swap for under two seconds) and a sound wave that
// throws the player back from range.
[AutoloadBossHead]
public sealed class Dosu : ExamBoss
{
    private const float Approach = 0f;
    private const float DrillWindup = 1f;
    private const float DrillRecovery = 2f;
    private const float WaveWindup = 3f;
    private const float WaveRecovery = 4f;

    protected override Color Tint => new(120, 115, 105);
    protected override int LifeMax => ExamBossRules.DosuLife;
    protected override int Defense => ExamBossRules.DosuDefense;
    protected override (string Name, string Title) Intro => ("音忍·多斯", "音隐村的下忍，大蛇丸的部下");

    protected override string SpritePrefix => "Dosu";

    protected override (string Action, int Frames, int TicksPerFrame, bool Loop) Pose => State switch
    {
        DrillWindup => ("DrillWindup", 2, 12, true),
        DrillRecovery when Timer < 14f => ("Drill", 2, 7, false),
        WaveWindup => ("Wave", 1, 10, true),
        WaveRecovery when Timer < 24f => ("Wave", 3, 8, false),
        _ => Moving("Walk", "Idle"),
    };

    protected override void Fight(Player target)
    {
        float distance = System.Math.Abs(target.Center.X - NPC.Center.X);
        NPC.damage = State == DrillRecovery && Timer < 14f ? ExamBossRules.DosuContactDamage : 0;
        switch (State)
        {
            case Approach:
                RunTo(target.Center.X - System.Math.Sign(target.Center.X - NPC.Center.X) * 7 * 16, 3.4f, target);
                if (Deciding && Timer > 70f)
                    Enter(distance < 12 * 16 && Main.rand.NextBool(3, 5) ? DrillWindup : WaveWindup);
                break;

            case DrillWindup:
                NPC.velocity.X *= 0.8f;
                Face(target.Center.X);
                Telegraph(DustID.Smoke, 26f);
                if (Timer == 1f)
                    Say("响鸣穿！", new Color(220, 220, 255));
                if (Timer >= ExamBossRules.DosuDrillWindupTicks)
                {
                    NPC.velocity.X = NPC.direction * 10f;
                    SoundEngine.PlaySound(SoundID.Item103, NPC.Center);
                    if (Deciding)
                        JutsuHitbox.Spawn(NPC, JutsuKind.EchoDrill, NPC.Center + new Vector2(NPC.direction * 24f, 0f),
                            new Vector2(NPC.direction * 10f, 0f), 40, 40, ExamBossRules.DosuDrillDamage);
                    Enter(DrillRecovery);
                }
                break;

            case DrillRecovery:
                if (Timer > 14f)
                    NPC.velocity.X *= 0.85f;
                if (Deciding && Timer >= 40f)
                    Enter(Approach);
                break;

            case WaveWindup:
                NPC.velocity.X *= 0.8f;
                Face(target.Center.X);
                Telegraph(DustID.Cloud, 40f);
                if (Timer >= ExamBossRules.DosuWaveWindupTicks)
                {
                    SoundEngine.PlaySound(SoundID.Item74, NPC.Center);
                    if (Deciding)
                        JutsuHitbox.Spawn(NPC, JutsuKind.SoundWave, NPC.Center + new Vector2(NPC.direction * 30f, 0f),
                            new Vector2(NPC.direction * 7f, 0f), 60, 90, ExamBossRules.DosuWaveDamage, ExamBossRules.DosuWaveKnockback);
                    Enter(WaveRecovery);
                }
                break;

            case WaveRecovery:
                NPC.velocity.X *= 0.85f;
                if (Deciding && Timer >= 45f)
                    Enter(Approach);
                break;
        }
    }

    protected override void LocalVictory(Player player) => player.GetModPlayer<ChuninExamPlayer>().PassPrelims();

    public override void OnKill()
    {
        StoryWorld.DownedDosu = true;
        Tell("胜者——" + "木叶的考生！预选赛合格。正式赛在一个月后，于木叶的考试会场举行。", new Color(255, 220, 120));
        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.WorldData);
    }
}
