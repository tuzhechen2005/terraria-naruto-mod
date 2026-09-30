using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.NPCs;

// Genin in the Forest of Death (specs/M2_中忍考试篇.spec.md 3.2): they run the player down and throw senbon. Placeholder
// art: the old candidate sprite, tinted per group. Who beat them is judged on each player's own client (a player who
// landed a hit takes part), since the exam progress lives there.
public abstract class ForestExamCandidate : ModNPC
{
    public override string Texture => "ShinobiPrototype/Content/NPCs/ForestExamCandidate";

    protected abstract Color Tint { get; }
    protected virtual float Speed => 2.6f;
    protected virtual int ThrowInterval => 150;

    // The local player hit this one (client side only).
    private bool hitByLocalPlayer;

    public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 1;

    public override void SetDefaults()
    {
        NPC.width = 28;
        NPC.height = 46;
        NPC.damage = 26;
        NPC.defense = 8;
        NPC.lifeMax = 260;
        NPC.knockBackResist = 0.35f;
        NPC.aiStyle = -1;
        NPC.value = Item.buyPrice(silver: 2);
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.color = Tint;
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
            Throw(target);
        }
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
    protected override Color Tint => new(180, 225, 170);

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
    protected override Color Tint => new(150, 165, 200);
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

    protected override void Beaten(ChuninExamPlayer exam)
    {
        foreach (NPC other in Main.ActiveNPCs)
            if (other.type == Type && other.whoAmI != NPC.whoAmI && other.life > 0)
                return;
        exam.CreditRainTrio();
    }
}
