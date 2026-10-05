using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items.Tier1;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.NPCs;

// The world's ninja enemies, one rig for all (specs/空档衔接与火影小兵.spec.md; user 2026-10-04: one ninja base, a costume
// per kind). Frames, 112x88 like the exam candidates: Idle, Wind (Slash_0), Strike (Slash_1), Walk x4, Jump, Throw.
// The AI runs on the server; every hit comes from a hitbox or projectile through EnemyDamage.
public abstract class WorldNinja : ModNPC
{
    protected const int FrameIdle = 0, FrameWind = 1, FrameStrike = 2, FrameWalk = 3, FrameJump = 7, FrameThrow = 8;
    protected const float Chase = 0f, Windup = 1f, Strike = 2f, Throw = 3f, Flee = 4f;

    protected ref float State => ref NPC.ai[0];
    protected ref float Timer => ref NPC.ai[1];
    protected ref float Cooldown => ref NPC.ai[2];

    protected abstract float Speed { get; }
    protected abstract int WindupTicks { get; }
    protected abstract int StrikeDamage { get; }

    public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 9;

    public override void SetDefaults()
    {
        NPC.width = 28;
        NPC.height = 56;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0.4f;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
    }

    // Never inside the Leaf, on the surface only.
    protected static bool OutsideTheLeaf(NPCSpawnInfo info) =>
        info.Player.ZoneOverworldHeight && !KonohaWorld.InKonoha(info.Player.Center) && !info.PlayerInTown;

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) =>
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
        {
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
            new FlavorTextBestiaryInfoElement($"Mods.ShinobiPrototype.NPCs.{Name}.Bestiary"),
        });

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
        float dx = target.Center.X - NPC.Center.X;
        Timer++;
        if (Cooldown > 0f)
            Cooldown--;
        Think(target, dx);
        if (NPC.collideX && NPC.velocity.Y == 0f)
            NPC.velocity.Y = -7.5f;
        if (NPC.velocity.Y == 0f && target.Bottom.Y < NPC.Top.Y - 32f && Main.rand.NextBool(90))
            NPC.velocity.Y = -9f;
    }

    protected virtual void Think(Player target, float dx)
    {
        switch (State)
        {
            case Windup:
                NPC.velocity.X *= 0.8f;
                if (Timer >= WindupTicks)
                    Enter(Strike);
                break;
            case Strike:
                NPC.velocity.X *= 0.8f;
                if (Timer == 1f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    JutsuHitbox.Spawn(NPC, JutsuKind.Strike, NPC.Center + new Vector2(NPC.direction * 24f, 0f), Vector2.Zero, 48, 52,
                        EnemyDamage.Projectile(StrikeDamage), 4f);
                    SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                }
                if (Timer >= WorldNinjaRules.StrikeShowTicks)
                {
                    Cooldown = WorldNinjaRules.MeleeCooldown;
                    Enter(Chase);
                }
                break;
            default:
                Run(target, dx);
                break;
        }
    }

    protected void Run(Player target, float dx, float keepAway = 0f)
    {
        NPC.direction = NPC.spriteDirection = dx >= 0f ? 1 : -1;
        float want = System.Math.Abs(dx) > keepAway ? NPC.direction * Speed : 0f;
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, want, 0.08f);
        if (Main.netMode != NetmodeID.MultiplayerClient && Cooldown <= 0f &&
            System.Math.Abs(dx) < WorldNinjaRules.MeleeReachTiles * 16f && System.Math.Abs(target.Center.Y - NPC.Center.Y) < 48f)
            Enter(Windup);
    }

    protected void Enter(float state)
    {
        State = state;
        Timer = 0f;
        NPC.netUpdate = true;
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.spriteDirection = NPC.direction;
        int frame = State switch
        {
            Windup => FrameWind,
            Strike => FrameStrike,
            Throw => FrameThrow,
            _ when NPC.velocity.Y != 0f => FrameJump,
            _ when System.Math.Abs(NPC.velocity.X) > 0.3f => FrameWalk + (int)(++NPC.frameCounter / 8 % 4),
            _ => FrameIdle,
        };
        NPC.frame.Y = frame * frameHeight;
    }
}

// 卡多的浪人: Gato's hired thugs, not ninja. They roam the surface at night outside the Leaf and cut with an old sword.
public sealed class Ronin : WorldNinja
{
    protected override float Speed => WorldNinjaRules.RoninSpeed;
    protected override int WindupTicks => EnemyDamageRules.Ronin.SlashWindupTicks;
    protected override int StrikeDamage => EnemyDamageRules.Ronin.Slash;

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.lifeMax = WorldNinjaRules.RoninLife;
        NPC.defense = WorldNinjaRules.RoninDefense;
        NPC.damage = EnemyDamageRules.Ronin.Contact;
        NPC.knockBackResist = 0.5f;
        NPC.value = Item.buyPrice(silver: 1, copper: 50);
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo) =>
        !Main.dayTime && OutsideTheLeaf(spawnInfo) ? WorldNinjaRules.RoninNightWeight : 0f;

    public override void ModifyNPCLoot(NPCLoot npcLoot) =>
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RoughCloth>(), 2, 1, 2));
}

// 叛忍·下忍: a young rogue from some village. Keeps a few tiles off and throws kunai, stabs up close, turns the first
// solid hit into a log (WorldNinjaRules.Substitutes; a stealth throw breaks through), and below a third of its life
// throws a smoke bomb and runs for a while.
public sealed class RogueGenin : WorldNinja
{
    private bool substituted;
    private bool fled;
    private int lastLife;

    protected override float Speed => WorldNinjaRules.RogueGeninSpeed;
    protected override int WindupTicks => EnemyDamageRules.RogueGenin.StabWindupTicks;
    protected override int StrikeDamage => EnemyDamageRules.RogueGenin.Stab;

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.lifeMax = WorldNinjaRules.RogueGeninLife;
        NPC.defense = WorldNinjaRules.RogueGeninDefense;
        NPC.damage = EnemyDamageRules.RogueGenin.Contact;
        NPC.knockBackResist = 0.35f;
        NPC.value = Item.buyPrice(silver: 3);
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo) => OutsideTheLeaf(spawnInfo) ? WorldNinjaRules.RogueGeninWeight : 0f;

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RogueHeadband>(), 4));
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ToolBlueprint>(), 12));
    }

    protected override void Think(Player target, float dx)
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
            Defend(target);
        switch (State)
        {
            case Throw:
                NPC.velocity.X *= 0.8f;
                if (Timer == 6f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 aim = NPC.DirectionTo(target.Center) * 10f;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, aim, ModContent.ProjectileType<HostileKunai>(),
                        EnemyDamage.Projectile(EnemyDamageRules.RogueGenin.Kunai), 1f, Main.myPlayer);
                    SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                }
                if (Timer >= 20f)
                {
                    Cooldown = 20f;
                    Enter(Chase);
                }
                return;
            case Flee:
                NPC.direction = NPC.spriteDirection = dx >= 0f ? -1 : 1;
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * Speed * 1.3f, 0.1f);
                if (Timer >= WorldNinjaRules.FleeTicks)
                    Enter(Chase);
                return;
            case Chase:
                Run(target, dx, WorldNinjaRules.RogueKeepAwayTiles * 16f);
                if (State == Chase && Main.netMode != NetmodeID.MultiplayerClient && Cooldown <= 0f &&
                    Timer % WorldNinjaRules.RogueThrowEvery == WorldNinjaRules.RogueThrowEvery - 1 &&
                    System.Math.Abs(dx) > WorldNinjaRules.MeleeReachTiles * 16f &&
                    Collision.CanHitLine(NPC.Center, 1, 1, target.Center, 1, 1))
                    Enter(Throw);
                return;
            default:
                base.Think(target, dx);
                return;
        }
    }

    // Server side: the substitution and the smoke bomb, read from the life it lost since the last tick.
    private void Defend(Player target)
    {
        if (lastLife == 0)
            lastLife = NPC.life;
        int lost = lastLife - NPC.life;
        if (lost > 0 && WorldNinjaRules.Substitutes(lost, NPC.lifeMax, substituted))
        {
            substituted = true;
            NPC.life = lastLife;
            Smoke();
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom - new Vector2(0f, 16f), Vector2.Zero,
                ModContent.ProjectileType<SubstitutionLog>(), 0, 0f, Main.myPlayer);
            if (GroundSpot.TryBeside(target, NPC.width, NPC.height, new[] { 6, 8, 5 }, out Vector2 bottom))
            {
                NPC.Bottom = bottom;
                NPC.velocity = Vector2.Zero;
            }
            Smoke();
            NPC.netUpdate = true;
        }
        else if (!fled && NPC.life < NPC.lifeMax * WorldNinjaRules.FleeBelow)
        {
            fled = true;
            Smoke();
            Enter(Flee);
        }
        lastLife = NPC.life;
    }

    private void Smoke()
    {
        SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
        if (Main.dedServ)
            return;
        for (int i = 0; i < 20; i++)
            Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(24f, 30f), DustID.Smoke, Main.rand.NextVector2Circular(2f, 2f),
                100, default, 1.7f);
    }
}
