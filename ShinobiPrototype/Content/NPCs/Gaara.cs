using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.NPCs;

// Gaara of the Sand, the finals (specs/M2_中忍考试篇.spec.md 3.4), in three phases by life (ExamBossRules):
// 1. The Absolute Defence: hits from the side he faces only chip the sand, except while he recovers from an attack;
//    sand shuriken; the Sand Coffin (a quicksand ring under the player; caught, substitute out before the Burial).
// 2. The sand armour cracks: faster, and a sand wave rolls along the ground.
// 3. Partial transformation: a great sand arm sweeps in front of him, and the Drilling Air Bullet.
// He turns to face the player only when he starts something, so there is time to get behind him.
[AutoloadBossHead]
public sealed class Gaara : ExamBoss
{
    private const float Approach = 0f;
    private const float ShurikenWindup = 1f;
    private const float CoffinWarn = 2f;
    private const float CoffinHold = 3f;
    private const float WaveWindup = 4f;
    private const float ArmWindup = 5f;
    private const float BulletWindup = 6f;
    private const float Recovery = 7f;

    private int shownPhase = 1;

    protected override Color Tint => Phase == 3 ? new Color(200, 150, 90) : new Color(210, 120, 90);
    protected override int LifeMax => ExamBossRules.GaaraLife;
    protected override int Defense => ExamBossRules.GaaraDefense;

    private int Phase => ExamBossRules.GaaraPhase(NPC.life, NPC.lifeMax);
    private float Tempo => ExamBossRules.GaaraTempo(Phase);
    private Vector2 CoffinAt => new(NPC.ai[2], NPC.ai[3]);

    protected override string SpritePrefix => "Gaara";

    private static readonly BossSprites.Canvas BeastCanvas = new(176, 104, 64, 100);

    private protected override BossSprites.Canvas CanvasFor(string action) =>
        action.StartsWith("Beast") ? BeastCanvas : PersonCanvas;

    private int shieldTicks;

    protected override (string Action, int Frames, int TicksPerFrame, bool Loop) Pose => Phase == 3
        ? State switch
        {
            ArmWindup => ("Beast_Arm", 3, 12, false),
            BulletWindup => ("Beast_Bullet", 3, 11, false),
            _ => ("Beast_Idle", 4, 10, true),
        }
        : State switch
        {
            ShurikenWindup or CoffinWarn => ("Cast", 3, 10, false),
            CoffinHold => ("Cast", 1, 10, true),
            WaveWindup => ("Wave", 3, 13, false),
            _ when shieldTicks > 0 => ("Shield", 2, 6, false),
            _ => Phase == 2 ? ("Cracked_Idle", 4, 10, true) : Moving("Walk", "Idle"),
        };

    protected override void Fight(Player target)
    {
        NPC.color = Tint;
        NPC.scale = Phase == 3 ? 1.3f : 1.15f;
        if (shieldTicks > 0)
            shieldTicks--;
        if (Phase != shownPhase)
        {
            shownPhase = Phase;
            SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
        }

        switch (State)
        {
            case Approach:
                // Walks slowly; the sand does the work.
                if ((int)Timer % 40 == 1)
                    Face(target.Center.X);
                float want = Math.Abs(target.Center.X - NPC.Center.X) > 14 * 16 ? NPC.direction * 1.4f * Tempo : 0f;
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, want, 0.08f);
                if (Deciding && Timer > 60f / Tempo)
                    Choose(target);
                break;

            case ShurikenWindup:
                NPC.velocity.X *= 0.8f;
                Telegraph(DustID.Sand, 24f);
                if (Timer >= 30f / Tempo)
                {
                    SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
                    if (Deciding)
                    {
                        Vector2 aim = NPC.DirectionTo(target.Center);
                        for (int i = 0; i < ExamBossRules.ShurikenCount; i++)
                            JutsuHitbox.Spawn(NPC, JutsuKind.SandShuriken, NPC.Center,
                                aim.RotatedBy((i - ExamBossRules.ShurikenCount / 2) * 0.16f) * 9f, 16, 16, ExamBossRules.ShurikenDamage);
                    }
                    Enter(Recovery);
                }
                break;

            case CoffinWarn:
                NPC.velocity.X *= 0.8f;
                // The quicksand ring under where the player stood.
                if (Main.netMode != NetmodeID.Server)
                    for (int i = 0; i < 3; i++)
                        Dust.NewDustPerfect(CoffinAt + new Vector2(Main.rand.NextFloat(-1f, 1f) * ExamBossRules.CoffinRadiusPx, -4f),
                            DustID.Sand, new Vector2(0f, -1.5f), 60, default, 1.4f).noGravity = true;
                if (Timer >= ExamBossRules.CoffinWarnTicks / Tempo)
                {
                    Player local = Main.LocalPlayer;
                    if (Main.netMode != NetmodeID.Server && local.active && !local.dead &&
                        Math.Abs(local.Center.X - CoffinAt.X) < ExamBossRules.CoffinRadiusPx && Math.Abs(local.Bottom.Y - CoffinAt.Y) < 64f)
                    {
                        local.GetModPlayer<JutsuStatusPlayer>().Bind(ExamBossRules.CoffinHoldTicks);
                        Main.NewText($"被沙子裹住了！按【{ShinobiKeybinds.SubstitutionKeyName()}】用替身术脱身！", 255, 200, 120);
                    }
                    Enter(CoffinHold);
                }
                break;

            case CoffinHold:
                NPC.velocity.X *= 0.8f;
                if (Timer >= ExamBossRules.CoffinHoldTicks)
                {
                    SoundEngine.PlaySound(SoundID.Item62, CoffinAt);
                    if (Deciding)
                        JutsuHitbox.Spawn(NPC, JutsuKind.SandBurial, CoffinAt - new Vector2(0f, 48f), Vector2.Zero, 96, 110,
                            ExamBossRules.BurialDamage);
                    Enter(Recovery);
                }
                break;

            case WaveWindup:
                NPC.velocity.X *= 0.8f;
                Telegraph(DustID.Sand, 36f);
                if (Timer >= 40f / Tempo)
                {
                    SoundEngine.PlaySound(SoundID.Item69, NPC.Center);
                    if (Deciding)
                        JutsuHitbox.Spawn(NPC, JutsuKind.SandWave, new Vector2(NPC.Center.X + NPC.direction * 40f, NPC.Bottom.Y - 34f),
                            new Vector2(NPC.direction * 6.5f, 0f), 52, 68, ExamBossRules.SandWaveDamage, 7f);
                    Enter(Recovery);
                }
                break;

            case ArmWindup:
                NPC.velocity.X *= 0.8f;
                Telegraph(DustID.Sand, 50f);
                if (Timer >= 36f / Tempo)
                {
                    SoundEngine.PlaySound(SoundID.Item71, NPC.Center);
                    if (Deciding)
                        JutsuHitbox.Spawn(NPC, JutsuKind.SandArm, NPC.Center + new Vector2(NPC.direction * 120f, -10f), Vector2.Zero,
                            220, 110, ExamBossRules.SandArmDamage, 9f);
                    Enter(Recovery);
                }
                break;

            case BulletWindup:
                NPC.velocity.X *= 0.8f;
                Telegraph(DustID.Cloud, 40f);
                if (Timer >= 34f / Tempo)
                {
                    SoundEngine.PlaySound(SoundID.Item45, NPC.Center);
                    if (Deciding)
                    {
                        Vector2 aim = NPC.DirectionTo(target.Center);
                        for (int i = -1; i <= 1; i++)
                            JutsuHitbox.Spawn(NPC, JutsuKind.AirBullet, NPC.Center, aim.RotatedBy(i * 0.25f) * 4.5f, 56, 56,
                                ExamBossRules.AirBulletDamage, 8f);
                    }
                    Enter(Recovery);
                }
                break;

            case Recovery:
                NPC.velocity.X *= 0.85f;
                if (Deciding && Timer >= 55f / Tempo)
                    Enter(Approach);
                break;
        }
    }

    private void Choose(Player target)
    {
        Face(target.Center.X);
        int phase = Phase;
        int roll = Main.rand.Next(phase == 1 ? 2 : phase == 2 ? 3 : 5);
        switch (roll)
        {
            case 0:
                Enter(ShurikenWindup);
                break;
            case 1:
                NPC.ai[2] = target.Center.X;
                NPC.ai[3] = target.Bottom.Y;
                Enter(CoffinWarn);
                break;
            case 2:
                Enter(WaveWindup);
                break;
            case 3:
                Enter(ArmWindup);
                break;
            default:
                Enter(BulletWindup);
                break;
        }
    }

    // The Absolute Defence.
    private void Shield(float fromX, ref NPC.HitModifiers modifiers)
    {
        int side = fromX >= NPC.Center.X ? 1 : -1;
        if (!ExamBossRules.ShieldBlocks(Phase, NPC.direction, side, State == Recovery))
            return;
        modifiers.FinalDamage *= ExamBossRules.ShieldDamageMultiplier;
        shieldTicks = 20;
        if (Main.netMode != NetmodeID.Server)
        {
            for (int i = 0; i < 8; i++)
                Dust.NewDust(NPC.Center + new Vector2(side * 16f, -20f), 8, 40, DustID.Sand, side * 2f, 0f);
            if (Main.rand.NextBool(4))
                CombatText.NewText(NPC.getRect(), new Color(230, 200, 130), "沙之盾");
        }
    }

    public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers) =>
        Shield(player.Center.X, ref modifiers);

    public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers) =>
        Shield(projectile.Center.X - projectile.velocity.X * 4f, ref modifiers);

    // Everyone who fought him passes the exams (the Chūnin headband); a character's first win always brings the Eight
    // Gates core and Lee's leg weights, later wins by chance (ModifyNPCLoot).
    protected override void LocalVictory(Player player)
    {
        var source = player.GetSource_Misc("ChuninExam");
        if (!player.HasItem(ModContent.ItemType<ChuninHeadband>()))
            player.QuickSpawnItem(source, ModContent.ItemType<ChuninHeadband>());
        ChuninExamPlayer exam = player.GetModPlayer<ChuninExamPlayer>();
        if (exam.GaaraFirstWin)
            return;
        exam.ClaimGaaraFirstWin();
        player.QuickSpawnItem(source, ModContent.ItemType<Items.StyleCores.EightGatesCore>());
        player.QuickSpawnItem(source, ModContent.ItemType<Items.Taijutsu.LeeLegWeights>());
        Main.NewText("首次击败我爱罗：得到八门遁甲之卷（流派核心）与小李的负重护腿。", new Color(255, 215, 120));
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(Terraria.GameContent.ItemDropRules.ItemDropRule.Common(ModContent.ItemType<Items.StyleCores.EightGatesCore>(), 4));
        npcLoot.Add(Terraria.GameContent.ItemDropRules.ItemDropRule.Common(ModContent.ItemType<Items.Taijutsu.LeeLegWeights>(), 3));
    }

    public override void OnKill()
    {
        StoryWorld.DownedGaara = true;
        Tell("我爱罗倒下了……可他身上涌出的查克拉，已经不属于人类。会场上空，羽毛般的幻术落了下来——木叶崩溃开始了。（M3 开发中）",
            new Color(255, 170, 120));
        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.WorldData);
    }
}
