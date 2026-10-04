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
// 1. Sand: fans of sand shuriken, quicksand (marked spots, then pillars), pellets of sand flicked as he walks, and the
//    Sand Coffin (a quicksand ring under the player; caught, substitute out before the Burial). Every 8 to 10 seconds,
//    or sooner after heavy hits from the front, the Sand Guard: a wall of sand in front of him for three seconds (front
//    hits lose 90%), then two seconds winded (user, 2026-10-02: time to get behind him).
// 2. The sand armour cracks: faster, the bullets thicker, and a sand wave rolls along the ground.
// 3. Partial transformation: a great sand arm sweeps in front of him, and the Drilling Air Bullet; no more guard.
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
    private const float Guard = 8f;
    private const float Winded = 9f;
    private const float QuicksandWindup = 10f;

    private int shownPhase = 1;
    private int sinceGuard;
    private int guardInterval = ExamBossRules.GuardInterval(0.5f);
    private int frontalDamage;
    private float lastMove = -1f;

    protected override Color Tint => Phase == 3 ? new Color(200, 150, 90) : new Color(210, 120, 90);
    protected override int LifeMax => ExamBossRules.GaaraLife;
    protected override int Defense => ExamBossRules.GaaraDefense;

    private int Phase => ExamBossRules.GaaraPhase(NPC.life, NPC.lifeMax);
    private float Tempo => ExamBossRules.GaaraTempo(Phase);
    private Vector2 CoffinAt => new(NPC.ai[2], NPC.ai[3]);

    // Every frame derived from one base drawn to Tazuna's standard (gaara-base-v2, gaara-set-v2, gaara-beast-v2).
    protected override string SpritePrefix => "Gaara";

    private static readonly BossSprites.Canvas BeastCanvas = new(176, 104, 64, 100);

    private protected override BossSprites.Canvas CanvasFor(string action) =>
        action.StartsWith("Beast") ? BeastCanvas : PersonCanvas;

    private bool Guarding => State == Guard;

    protected override (string Action, int Frames, int TicksPerFrame, bool Loop) Pose => Phase == 3
        ? State switch
        {
            ArmWindup => WithLeadIn(("Beast_Arm", 3, 12, false)),
            BulletWindup => WithLeadIn(("Beast_Bullet", 3, 11, false)),
            _ => ("Beast_Idle", 4, 10, true),
        }
        : State switch
        {
            ShurikenWindup or CoffinWarn or QuicksandWindup => WithLeadIn(("Cast", 3, 10, false)),
            CoffinHold => ("Cast", 1, 10, true),
            WaveWindup => WithLeadIn(("Wave", 3, 13, false)),
            Guard => WithLeadIn(("Shield", 2, 6, false)),
            Winded => ("Hurt", 1, 10, true),
            _ => Phase == 2 ? ("Cracked_Idle", 4, 10, true) : Moving("Walk", "Idle", ArtWalkFrames, ArtIdleFrames),
        };

    protected override void Fight(Player target)
    {
        NPC.color = Tint;
        NPC.scale = Phase == 3 ? 1.3f : 1.15f;
        if (Phase != shownPhase)
        {
            shownPhase = Phase;
            SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
            // Transformed, he has no more use for the wall.
            if (Guarding && Phase == 3)
                Enter(Approach);
        }
        if (!Guarding && State != Winded)
            sinceGuard++;
        if (Deciding && State is Approach or Recovery &&
            ExamBossRules.GuardDue(Phase, sinceGuard, guardInterval, frontalDamage))
        {
            Face(target.Center.X);
            sinceGuard = frontalDamage = 0;
            guardInterval = ExamBossRules.GuardInterval(Main.rand.NextFloat());
            SoundEngine.PlaySound(SoundID.Item74 with { Pitch = -0.4f }, NPC.Center);
            Say("沙之守护", new Color(230, 200, 130));
            Enter(Guard);
        }

        switch (State)
        {
            case Approach:
                // Walks slowly; the sand does the work.
                if ((int)Timer % 40 == 1)
                    Face(target.Center.X);
                float want = Math.Abs(target.Center.X - NPC.Center.X) > 14 * 16 ? NPC.direction * 1.4f * Tempo : 0f;
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, want, 0.08f);
                // Pellets of sand from the gourd as he walks.
                int every = ExamBossRules.PelletEvery(Phase);
                if (Deciding && Timer > 20f && (int)Timer % every == every - 1)
                {
                    float far = Math.Abs(target.Center.X - NPC.Center.X);
                    Vector2 aim = NPC.DirectionTo(target.Center - new Vector2(0f, far * 0.12f));
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 14f, -24f), aim * 8f,
                        ModContent.ProjectileType<SandPellet>(), EnemyDamage.Projectile(EnemyDamageRules.Gaara.Pellet), 1f, Main.myPlayer);
                }
                if (Deciding && Timer > 60f / Tempo)
                    Choose(target);
                break;

            case Guard:
                // Rooted behind the wall.
                NPC.velocity.X *= 0.6f;
                if (Main.netMode != NetmodeID.Server && !FxArt.Has("FxSandWall_0"))
                    for (int i = 0; i < 2; i++)
                        Dust.NewDustPerfect(WallAt + new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-56f, 56f)), DustID.Sand,
                            new Vector2(0f, -0.6f), 60, default, 1.3f).noGravity = true;
                if (Timer >= ExamBossRules.GuardTicks)
                {
                    // The wall falls, and so does his guard.
                    SoundEngine.PlaySound(SoundID.Item51, NPC.Center);
                    if (Main.netMode != NetmodeID.Server)
                        for (int i = 0; i < 30; i++)
                            Dust.NewDust(WallAt - new Vector2(16f, 56f), 32, 112, DustID.Sand, NPC.direction * 1.5f, 2f, 60, default, 1.4f);
                    Enter(Winded);
                }
                break;

            case Winded:
                NPC.velocity.X *= 0.6f;
                if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(5))
                    Dust.NewDustPerfect(NPC.Top + new Vector2(Main.rand.NextFloat(-12f, 12f), 6f), DustID.Sand, new Vector2(0f, 1.5f), 60,
                        default, 1f);
                if (Deciding && Timer >= ExamBossRules.GuardStaggerTicks)
                    Enter(Approach);
                break;

            case QuicksandWindup:
                NPC.velocity.X *= 0.8f;
                Telegraph(DustID.Sand, 30f);
                if (Timer >= 26f / Tempo)
                {
                    SoundEngine.PlaySound(SoundID.Item20, target.Center);
                    if (Deciding)
                    {
                        // One spot under the player, the rest either side of it, far enough apart to stand between.
                        int spots = ExamBossRules.QuicksandSpots(Phase);
                        for (int i = 0; i < spots; i++)
                        {
                            float x = target.Center.X + (i - spots / 2) * ExamBossRules.QuicksandGapPx + (i == spots / 2 ? 0f : Main.rand.NextFloat(-14f, 14f));
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(x, target.Bottom.Y - 8f), Vector2.Zero,
                                ModContent.ProjectileType<Quicksand>(), EnemyDamage.Projectile(EnemyDamageRules.Gaara.Quicksand), 4f, Main.myPlayer,
                                ExamBossRules.QuicksandWarnTicks + Math.Abs(i - spots / 2) * 6);
                        }
                    }
                    Enter(Recovery);
                }
                break;

            case ShurikenWindup:
                NPC.velocity.X *= 0.8f;
                Telegraph(DustID.Sand, 24f);
                if (Timer >= EnemyDamageRules.BigWindup(EnemyDamageRules.Gaara.ShurikenWindupTicks, Tempo))
                {
                    SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
                    if (Deciding)
                    {
                        Vector2 aim = NPC.DirectionTo(target.Center);
                        int fan = ExamBossRules.ShurikenFan(Phase);
                        for (int i = 0; i < fan; i++)
                            JutsuHitbox.Spawn(NPC, JutsuKind.SandShuriken, NPC.Center,
                                aim.RotatedBy((i - fan / 2) * 0.16f) * 9f, 18, 18, EnemyDamage.Projectile(EnemyDamageRules.Gaara.Shuriken));
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
                if (Timer >= EnemyDamageRules.BigWindup(ExamBossRules.CoffinWarnTicks, Tempo))
                {
                    Player local = Main.LocalPlayer;
                    // Still on the quicksand when it closes: a log takes it, or the sand does (user, 2026-10-03).
                    if (Main.netMode != NetmodeID.Server && local.active && !local.dead &&
                        Math.Abs(local.Center.X - CoffinAt.X) < ExamBossRules.CoffinRadiusPx && Math.Abs(local.Bottom.Y - CoffinAt.Y) < 64f &&
                        !local.GetModPlayer<SubstitutionPlayer>().TakeBind(local.Center.X >= CoffinAt.X ? 1 : -1))
                    {
                        local.GetModPlayer<JutsuStatusPlayer>().Bind(ExamBossRules.CoffinHoldTicks);
                        Main.NewText("被沙子裹住了！", 255, 200, 120);
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
                            EnemyDamage.Projectile(EnemyDamageRules.Gaara.Burial));
                    Enter(Recovery);
                }
                break;

            case WaveWindup:
                NPC.velocity.X *= 0.8f;
                Telegraph(DustID.Sand, 36f);
                if (Timer >= EnemyDamageRules.BigWindup(EnemyDamageRules.Gaara.SandWaveWindupTicks, Tempo))
                {
                    SoundEngine.PlaySound(SoundID.Item69, NPC.Center);
                    if (Deciding)
                        JutsuHitbox.Spawn(NPC, JutsuKind.SandWave, new Vector2(NPC.Center.X + NPC.direction * 40f, NPC.Bottom.Y - 34f),
                            new Vector2(NPC.direction * 6.5f, 0f), 52, 68, EnemyDamage.Projectile(EnemyDamageRules.Gaara.SandWave), 7f);
                    Enter(Recovery);
                }
                break;

            case ArmWindup:
                NPC.velocity.X *= 0.8f;
                Telegraph(DustID.Sand, 50f);
                if (Timer >= EnemyDamageRules.BigWindup(EnemyDamageRules.Gaara.SandArmWindupTicks, Tempo))
                {
                    SoundEngine.PlaySound(SoundID.Item71, NPC.Center);
                    if (Deciding)
                        JutsuHitbox.Spawn(NPC, JutsuKind.SandArm, NPC.Center + new Vector2(NPC.direction * 120f, -10f), Vector2.Zero,
                            220, 110, EnemyDamage.Projectile(EnemyDamageRules.Gaara.SandArm), 9f);
                    Enter(Recovery);
                }
                break;

            case BulletWindup:
                NPC.velocity.X *= 0.8f;
                Telegraph(DustID.Cloud, 40f);
                if (Timer >= EnemyDamageRules.BigWindup(EnemyDamageRules.Gaara.AirBulletWindupTicks, Tempo))
                {
                    SoundEngine.PlaySound(SoundID.Item45, NPC.Center);
                    if (Deciding)
                    {
                        Vector2 aim = NPC.DirectionTo(target.Center);
                        for (int i = -1; i <= 1; i++)
                            JutsuHitbox.Spawn(NPC, JutsuKind.AirBullet, NPC.Center, aim.RotatedBy(i * 0.25f) * 4.5f, 56, 56,
                                EnemyDamage.Projectile(EnemyDamageRules.Gaara.AirBullet), 8f);
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

    // Sand shuriken, quicksand or the coffin; the sand wave once the armour cracks; the arm and the air bullet once he
    // transforms. Never the same twice running.
    private void Choose(Player target)
    {
        Face(target.Center.X);
        float[] moves = Phase switch
        {
            1 => new[] { ShurikenWindup, QuicksandWindup, CoffinWarn },
            2 => new[] { ShurikenWindup, QuicksandWindup, CoffinWarn, WaveWindup },
            _ => new[] { ShurikenWindup, QuicksandWindup, CoffinWarn, WaveWindup, ArmWindup, BulletWindup, ArmWindup, BulletWindup },
        };
        float move;
        do
            move = moves[Main.rand.Next(moves.Length)];
        while (move == lastMove);
        lastMove = move;
        if (move == CoffinWarn)
        {
            NPC.ai[2] = target.Center.X;
            NPC.ai[3] = target.Bottom.Y;
        }
        Enter(move);
    }

    // Where the wall of sand stands: just in front of him.
    private Vector2 WallAt => new(NPC.Center.X + NPC.direction * 40f, NPC.Bottom.Y - 56f);

    // The Sand Guard: hits from the front lose 90% while the wall is up; heavy hits from the front bring it up sooner.
    private void Guarded(float fromX, ref NPC.HitModifiers modifiers)
    {
        int side = fromX >= NPC.Center.X ? 1 : -1;
        if (!ExamBossRules.GuardBlocks(Guarding, NPC.direction, side))
            return;
        modifiers.FinalDamage *= ExamBossRules.GuardDamageMultiplier;
        if (Main.netMode != NetmodeID.Server)
            for (int i = 0; i < 8; i++)
                Dust.NewDust(WallAt - new Vector2(8f, 30f), 16, 60, DustID.Sand, side * 2f, 0f);
    }

    private void CountFrontal(float fromX, int damage)
    {
        int side = fromX >= NPC.Center.X ? 1 : -1;
        if (side == NPC.direction && !Guarding && State != Winded)
            frontalDamage += damage;
    }

    public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers) =>
        Guarded(player.Center.X, ref modifiers);

    public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers) =>
        Guarded(projectile.Center.X - projectile.velocity.X * 4f, ref modifiers);

    public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitByItem(player, item, hit, damageDone);
        CountFrontal(player.Center.X, damageDone);
    }

    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitByProjectile(projectile, hit, damageDone);
        CountFrontal(projectile.Center.X - projectile.velocity.X * 4f, damageDone);
    }

    // The wall of sand in front of him (gaara-fx-v1): frame 0 is the sand rising, then 1 and 2 churn in turn.
    public override void PostDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        int frame = Timer < 10f ? 0 : 1 + (int)(Main.GameUpdateCount / 8) % 2;
        if (!Guarding || FxArt.Frame("FxSandWall", frame, 3) is not { } wall)
            return;
        float rise = Math.Min(1f, 0.4f + Timer / 10f);
        Vector2 foot = new(WallAt.X, NPC.Bottom.Y);
        Main.EntitySpriteDraw(wall, new Vector2(MathF.Round(foot.X - screenPos.X), MathF.Round(foot.Y - screenPos.Y)), null,
            BossSprites.Lit(drawColor, 0.6f), 0f, new Vector2(wall.Width / 2f, wall.Height), new Vector2(ArtScale, ArtScale * rise),
            NPC.direction < 0 ? Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally : Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
    }

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
