using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.NPCs;

// Genin in the Forest of Death (specs/M2_中忍考试篇.spec.md 3.2). DeathForestSystem sends them three at a time: a squad
// shares a number (ai[3]) and ai[0] holds its kind and name (Kind * 10 + name). Who beat a squad is judged on each
// player's own client (whoever hit any of the three takes part when the last one falls), since the exam progress
// lives there. Art: exam-genin-style-v1 and exam-genin-full-v1, one sheet each (<ClassName>.png, 112x88 frames facing
// left): standing, the throw (shown for a moment after each attack), a four-frame run and the jump.
public abstract class ForestExamCandidate : ModNPC
{
    public const int KindRoaming = 0;   // an optional fight anywhere in the forest
    public const int KindGate = 1;      // the squad lying in wait past the gate
    public const int KindRain = 2;      // the Rain genin before the tower

    public static readonly string[] SquadNames = { "Genin.SquadGrass", "Genin.SquadWaterfall", "Genin.SquadLeaf" };   // text keys

    private const int PoseTicks = 20;
    private const int ThrowFrame = 1;
    private const int RunFirst = 2;
    private const int RunFrames = 4;
    private const int JumpFrame = RunFirst + RunFrames;

    // Squads the local player has hit (client side).
    private static readonly System.Collections.Generic.HashSet<int> squadsHitHere = new();

    public static void ForgetSquads() => squadsHitHere.Clear();

    protected int Squad => (int)NPC.ai[3];
    protected int Kind => (int)NPC.ai[0] / 10;
    protected string SquadName => Loc.Get(SquadNames[System.Math.Clamp((int)NPC.ai[0] % 10, 0, SquadNames.Length - 1)]);

    // ai[1] counts up to the next attack, ai[2] counts down the attack pose.
    protected ref float AttackTimer => ref NPC.ai[1];
    protected ref float PoseTimer => ref NPC.ai[2];

    protected virtual float Speed => 2.6f;
    protected virtual float KeepAway => 9 * 16f;

    public override void SetStaticDefaults() => Main.npcFrameCount[Type] = JumpFrame + 1;

    public override void SetDefaults()
    {
        NPC.width = 28;
        // The art stands about 62 pixels tall.
        NPC.height = 56;
        NPC.damage = EnemyDamageRules.Genin.CandidateContact;
        NPC.defense = 8;
        NPC.lifeMax = 260;
        NPC.knockBackResist = 0.35f;
        NPC.aiStyle = -1;
        NPC.value = Item.buyPrice(silver: 2);
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
    }

    public override void AI()
    {
        NPC.TargetClosest();
        Player target = Main.player[NPC.target];
        if (!target.active || target.dead)
        {
            NPC.velocity.X *= 0.95f;
            NPC.EncourageDespawn(60);
            return;
        }

        float distance = target.Center.X - NPC.Center.X;
        NPC.direction = distance >= 0f ? 1 : -1;
        NPC.spriteDirection = NPC.direction;
        float want = System.Math.Abs(distance) > KeepAway ? NPC.direction * Speed : NPC.direction * Speed * 0.35f;
        if (PoseTimer > 0f)
            want = 0f;
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, want, 0.08f);
        if (NPC.collideX && NPC.velocity.Y == 0f)
            NPC.velocity.Y = -7.5f;
        if (NPC.velocity.Y == 0f && target.Bottom.Y < NPC.Top.Y - 32f && Main.rand.NextBool(90))
            NPC.velocity.Y = -9f;
        if (PoseTimer > 0f)
            PoseTimer--;

        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;
        AttackTimer++;
        if (Attack(target, System.Math.Abs(distance)))
        {
            PoseTimer = PoseTicks;
            NPC.netUpdate = true;
        }
    }

    // Server side: start an attack when it is due; true if one was made (the pose follows).
    protected abstract bool Attack(Player target, float distance);

    protected bool ClearShot(Player target) => Collision.CanHitLine(NPC.Center, 1, 1, target.Center, 1, 1);

    public override void FindFrame(int frameHeight)
    {
        NPC.spriteDirection = NPC.direction;
        int frame;
        if (PoseTimer > 0f)
            frame = ThrowFrame;
        else if (NPC.velocity.Y != 0f)
            frame = JumpFrame;
        else if (System.Math.Abs(NPC.velocity.X) > 0.3f)
        {
            NPC.frameCounter += System.Math.Abs(NPC.velocity.X);
            frame = RunFirst + (int)(NPC.frameCounter / 10.0) % RunFrames;
        }
        else
        {
            NPC.frameCounter = 0;
            frame = 0;
        }
        NPC.frame.Y = frame * frameHeight;
    }

    public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
    {
        if (player.whoAmI == Main.myPlayer)
            squadsHitHere.Add(Squad);
    }

    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        if (projectile.owner == Main.myPlayer && projectile.friendly)
            squadsHitHere.Add(Squad);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life > 0 || Main.netMode == NetmodeID.Server)
            return;
        for (int i = 0; i < 12; i++)
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, hit.HitDirection * 2f, -1.5f);
        foreach (NPC other in Main.ActiveNPCs)
            if (other.type == Type && other.whoAmI != NPC.whoAmI && other.life > 0 && (int)other.ai[3] == Squad)
                return;
        if (squadsHitHere.Remove(Squad))
            Beaten(Main.LocalPlayer.GetModPlayer<ChuninExamPlayer>());
    }

    protected abstract void Beaten(ChuninExamPlayer exam);

    // A poof of smoke and a few tiles away, beside the target (the smoke bomb; the Rain genin's clone trick).
    protected void SmokeTo(Player target, int[] tiles)
    {
        Smoke(NPC.Center);
        if (GroundSpot.TryBeside(target, NPC.width, NPC.height, tiles, out Vector2 bottom))
        {
            NPC.Bottom = bottom;
            NPC.velocity = Vector2.Zero;
            NPC.netUpdate = true;
        }
        Smoke(NPC.Center);
    }

    protected static void Smoke(Vector2 at)
    {
        if (Main.dedServ)
            return;
        for (int i = 0; i < 20; i++)
            Dust.NewDustPerfect(at + Main.rand.NextVector2Circular(24f, 30f), DustID.Smoke, Main.rand.NextVector2Circular(2f, 2f),
                100, default, 1.7f);
    }
}

// Candidates on the jungle surface: a kunai up close, shuriken from a little way off, and now and then a smoke bomb to
// slip round behind the player (user, 2026-10-01: they all just threw senbon).
public sealed class ForestCanopyCandidate : ForestExamCandidate
{
    private const int SlashEvery = 55;
    private const int ThrowEvery = 140;

    // Server side: ticks until the kunai strike lands. The swing (the throw pose) shows first, so a strike that
    // appears right beside the player can be seen coming (specs/敌方伤害标准.spec.md: at least 15 ticks).
    private int strikeIn;

    protected override bool Attack(Player target, float distance)
    {
        if (strikeIn > 0)
        {
            if (--strikeIn == 0)
            {
                JutsuHitbox.Spawn(NPC, JutsuKind.Strike, NPC.Center + new Vector2(NPC.direction * 22f, 0f), Vector2.Zero, 44, 50,
                    EnemyDamage.Projectile(EnemyDamageRules.Genin.CandidateStrike), 4f);
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
            }
            return false;
        }
        if (distance < 4 * 16f && AttackTimer >= SlashEvery)
        {
            AttackTimer = 0f;
            strikeIn = EnemyDamageRules.Genin.CandidateStrikeWindupTicks;
            return true;
        }
        if (distance >= 6 * 16f && distance < 26 * 16f && AttackTimer >= ThrowEvery && ClearShot(target))
        {
            AttackTimer = Main.rand.Next(-30, 20);
            Vector2 aim = new(NPC.direction * 9.5f, 0f);
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, aim, ModContent.ProjectileType<ExamShuriken>(),
                EnemyDamage.Projectile(EnemyDamageRules.Genin.CandidateShuriken), 0f, Main.myPlayer);
            return true;
        }
        // The smoke bomb, once in a while when the player keeps close.
        if (distance < 7 * 16f && AttackTimer >= 40f && Main.rand.NextBool(240))
        {
            AttackTimer = 0f;
            SmokeTo(target, new[] { 6, 8, 10 });
        }
        return false;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.Shuriken, 2, 15, 30));
        npcLoot.Add(ItemDropRule.Common(ItemID.ThrowingKnife, 3, 10, 20));
        npcLoot.Add(ItemDropRule.Common(ItemID.LesserHealingPotion, 5));
    }

    protected override void Beaten(ChuninExamPlayer exam)
    {
        if (Kind == KindGate)
            exam.CreditGateSquad();
        else
            exam.CreditSquad();
    }
}

// The Rain genin who lie in wait in the clearing before the tower: the second test's small boss (user, 2026-10-01).
// They keep their distance and take turns: a fan of three senbon, the umbrella rain (needles thrown up out of the
// umbrella come down on the player, a glint first marks each), and an illusion clone that bursts into smoke when hit.
// The last of the three carries the missing scroll.
public sealed class RainGenin : ForestExamCandidate
{
    private const int AttackEvery = 100;

    protected override float Speed => 3f;
    protected override float KeepAway => 12 * 16f;

    private int Turn => (int)NPC.localAI[1];

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.lifeMax = 420;
        NPC.damage = EnemyDamageRules.Genin.RainContact;
        NPC.defense = 10;
    }

    protected override bool Attack(Player target, float distance)
    {
        if (AttackTimer < AttackEvery || !ClearShot(target))
            return false;
        AttackTimer = Main.rand.Next(-25, 15);
        NPC.localAI[1]++;
        switch (Turn % 3)
        {
            case 0:
            {
                Vector2 aim = NPC.DirectionTo(target.Center) * 9f;
                for (int i = -1; i <= 1; i++)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, aim.RotatedBy(i * 0.14f),
                        ModContent.ProjectileType<ExamSenbon>(), EnemyDamage.Projectile(EnemyDamageRules.Genin.RainSenbon), 0f, Main.myPlayer);
                break;
            }
            case 1:
                // The umbrella rain: a spread of needles above the player, falling one after another.
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item7, NPC.Center);
                for (int i = 0; i < 6; i++)
                {
                    Vector2 at = target.Center + new Vector2((i - 2.5f) * 46f + Main.rand.NextFloat(-10f, 10f), -360f);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), at, Vector2.Zero, ModContent.ProjectileType<ExamSenbon>(),
                        EnemyDamage.Projectile(EnemyDamageRules.Genin.RainUmbrella), 0f, Main.myPlayer, EnemyDamageRules.Genin.RainUmbrellaHangTicks + i * 6);
                }
                break;
            default:
                // The clone: the genin slips aside in smoke and an illusion stays behind in its place.
                if (NPC.CountNPCS(ModContent.NPCType<RainClone>()) < 3)
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Bottom.Y, ModContent.NPCType<RainClone>());
                SmokeTo(target, new[] { 12, 10, 14 });
                break;
        }
        return true;
    }

    protected override void Beaten(ChuninExamPlayer exam) => exam.CreditRainTrio();
}

// An illusion left by a Rain genin: looks the same, walks at the player, harmless, gone in smoke at the first hit or
// after a while.
public sealed class RainClone : ModNPC
{
    private const int LifeTicks = 600;

    public override string Texture => "ShinobiPrototype/Content/NPCs/RainGenin";

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 7;
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers { Hide = true });
    }

    public override void SetDefaults()
    {
        NPC.width = 28;
        NPC.height = 56;
        NPC.lifeMax = 1;
        NPC.damage = 0;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0f;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.Item8;
        NPC.npcSlots = 0f;
    }

    public override void AI()
    {
        NPC.TargetClosest();
        Player target = Main.player[NPC.target];
        NPC.direction = NPC.spriteDirection = target.Center.X >= NPC.Center.X ? 1 : -1;
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * 2.2f, 0.06f);
        if (NPC.collideX && NPC.velocity.Y == 0f)
            NPC.velocity.Y = -7f;
        if (++NPC.ai[0] > LifeTicks && Main.netMode != NetmodeID.MultiplayerClient)
        {
            NPC.active = false;
            NPC.netUpdate = true;
        }
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.spriteDirection = NPC.direction;
        NPC.frameCounter += System.Math.Abs(NPC.velocity.X);
        NPC.frame.Y = (System.Math.Abs(NPC.velocity.X) > 0.3f ? 2 + (int)(NPC.frameCounter / 10.0) % 4 : 0) * frameHeight;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life > 0 || Main.dedServ)
            return;
        for (int i = 0; i < 24; i++)
            Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(20f, 28f), DustID.Smoke,
                Main.rand.NextVector2Circular(2f, 2f), 100, default, 1.6f);
    }

    public override bool CheckActive() => true;
}
