using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.NPCs;

// Genin in the Forest of Death (specs/M2_中忍考试篇.spec.md 3.2): they run the player down and throw senbon. Who beat
// them is judged on each player's own client (a player who landed a hit takes part), since the exam progress lives
// there. Art: exam-genin-style-v1 and exam-genin-full-v1, one sheet each (<ClassName>.png, 112x88 frames facing left):
// standing, the throw (shown for a moment after each one), a four-frame run and the jump.
public abstract class ForestExamCandidate : ModNPC
{
    private const int ThrowPoseTicks = 20;
    private const int ThrowFrame = 1;
    private const int RunFirst = 2;
    private const int RunFrames = 4;
    private const int JumpFrame = RunFirst + RunFrames;

    protected virtual float Speed => 2.6f;
    protected virtual int ThrowInterval => 150;

    // The local player hit this one (client side only).
    private bool hitByLocalPlayer;

    public override void SetStaticDefaults() => Main.npcFrameCount[Type] = JumpFrame + 1;

    public override void SetDefaults()
    {
        NPC.width = 28;
        // The new art stands about 62 pixels tall.
        NPC.height = 56;
        NPC.damage = 26;
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
        // Keep a little distance to throw from, close in when the player is far.
        float want = System.Math.Abs(distance) > 12 * 16 ? NPC.direction * Speed : NPC.direction * Speed * 0.35f;
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, want, 0.07f);
        if (NPC.collideX && NPC.velocity.Y == 0f)
            NPC.velocity.Y = -7.5f;
        if (NPC.velocity.Y == 0f && target.Bottom.Y < NPC.Top.Y - 32f && Main.rand.NextBool(90))
            NPC.velocity.Y = -9f;

        if (Main.netMode != NetmodeID.MultiplayerClient && ++NPC.ai[1] >= ThrowInterval &&
            Collision.CanHitLine(NPC.Center, 1, 1, target.Center, 1, 1))
        {
            NPC.ai[1] = Main.rand.Next(-30, 30);
            NPC.ai[2] = ThrowPoseTicks;
            NPC.netUpdate = true;
            Throw(target);
        }
        if (NPC.ai[2] > 0f)
            NPC.ai[2]--;
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.spriteDirection = NPC.direction;
        int frame;
        if (NPC.ai[2] > 0f)
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

    protected virtual void Throw(Player target)
    {
        Vector2 aim = NPC.DirectionTo(target.Center) * 9f;
        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, aim, ModContent.ProjectileType<ExamSenbon>(),
            NPC.damage / 3, 0f, Main.myPlayer);
    }

    public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
    {
        if (player.whoAmI == Main.myPlayer)
            hitByLocalPlayer = true;
    }

    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        if (projectile.owner == Main.myPlayer && projectile.friendly)
            hitByLocalPlayer = true;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life > 0 || Main.netMode == NetmodeID.Server)
            return;
        for (int i = 0; i < 12; i++)
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, hit.HitDirection * 2f, -1.5f);
        if (hitByLocalPlayer)
            Beaten(Main.LocalPlayer.GetModPlayer<ChuninExamPlayer>());
    }

    protected abstract void Beaten(ChuninExamPlayer exam);
}

// Candidates on the jungle surface, three to a squad, for anyone hunting the other scroll.
public sealed class ForestCanopyCandidate : ForestExamCandidate
{

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        Player player = spawnInfo.Player;
        if (!ChuninExamRules.CandidatesSpawn(player.GetModPlayer<ChuninExamPlayer>().Stage,
                player.ZoneJungle && player.ZoneOverworldHeight))
            return 0f;
        return NPC.CountNPCS(Type) >= ChuninExamRules.SquadSize * 2 ? 0f : 0.35f;
    }

    // A naturally spawned candidate brings the other two of their squad.
    public override void OnSpawn(IEntitySource source)
    {
        if (source is not EntitySource_SpawnNPC || Main.netMode == NetmodeID.MultiplayerClient)
            return;
        for (int i = 1; i < ChuninExamRules.SquadSize; i++)
            NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + (i == 1 ? -40 : 40), (int)NPC.Bottom.Y, Type);
    }

    protected override void Beaten(ChuninExamPlayer exam) => exam.CreditCandidate();
}

// The Rain genin who ambush the player once in the forest; the last of the three carries the missing scroll.
public sealed class RainGenin : ForestExamCandidate
{
    protected override float Speed => 3f;
    protected override int ThrowInterval => 110;

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.lifeMax = 340;
        NPC.damage = 30;
        NPC.defense = 10;
    }

    // A fan of three senbon (the umbrella trick of the anime, kept to the ground).
    protected override void Throw(Player target)
    {
        Vector2 aim = NPC.DirectionTo(target.Center) * 9f;
        for (int i = -1; i <= 1; i++)
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, aim.RotatedBy(i * 0.14f),
                ModContent.ProjectileType<ExamSenbon>(), NPC.damage / 3, 0f, Main.myPlayer);
    }

    public override void AI()
    {
        if (Main.netMode != NetmodeID.Server && NPC.localAI[1] == 0f)
        {
            NPC.localAI[1] = 1f;
            Common.Systems.BossIntroSystem.Show("雨隐三人组", "雨隐村的下忍——冲着你的卷轴来的");
        }
        base.AI();
    }

    protected override void Beaten(ChuninExamPlayer exam)
    {
        foreach (NPC other in Main.ActiveNPCs)
            if (other.type == Type && other.whoAmI != NPC.whoAmI && other.life > 0)
                return;
        exam.CreditRainTrio();
    }
}
