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

// Orochimaru halfway through the Forest of Death (specs/M2_中忍考试篇.spec.md section 5; redesigned with the user
// 2026-10-01), a fight that cannot be won. He comes as a Grass candidate (OrochimaruDisguise) and starts here either
// revealed (he reached the player: the killing intent, broken by substitution) or emerging from the ground (the
// disguise was struck down). Then Hidden Shadow Snake Hands, a snake dash round behind the player, the Great
// Breakthrough gust, the Five Elements Seal (chakra stops recovering), summoned snakes, the long neck, and once the
// giant snake writhing across the ground. He sinks away as snakes at half life, once the player has held out long
// enough, or when the player falls (ExamBossRules.OrochimaruEnds); a player not yet strong enough takes less and holds
// out less. The first meeting always leaves the Sharingan in a vial (ChuninExamPlayer.CreditOrochimaru); the shed skin
// he leaves calls him back for another try at it (then by chance).
[AutoloadBossHead]
public sealed class Orochimaru : ExamBoss
{
    public const float Reveal = 0f;    // also how the shed skin calls him: he is simply there
    public const float Emerge = 10f;
    private const float Approach = 1f;
    private const float HandsWindup = 2f;
    private const float DashWindup = 3f;
    private const float Dash = 4f;
    private const float WindWindup = 5f;
    private const float NeckWindup = 6f;
    private const float Recovery = 7f;
    private const float Exit = 8f;
    private const float SealWindup = 9f;

    private const int RevealTicks = 70;
    private const int EmergeTicks = 60;

    private int snakeSummons;
    private int fightTicks;
    private bool giantSnake;
    private bool? ready;
    private bool rematch;

    protected override Color Tint => new(150, 140, 175);

    // Art: orochimaru-style-v3, orochimaru-moves-v1 (moving, openings, leaving) and -v2 (techniques). A technique
    // without its frames yet stands in with the idle frames; without any, the tinted placeholder is drawn.
    protected override string SpritePrefix => Has("Idle") ? "Orochimaru" : null;

    private static bool Has(string action) => BossSprites.Has($"Orochimaru_{action}_0");

    private static (string, int, int, bool) Or((string Action, int Frames, int Ticks, bool Loop) pose) =>
        Has(pose.Action) ? pose : ("Idle", 4, 12, true);

    private int summonPose;

    protected override (string Action, int Frames, int TicksPerFrame, bool Loop) Pose => summonPose > 0
        ? Or(("Summon", 2, 20, false))
        : State switch
        {
            Reveal => Or(("Reveal", 3, RevealTicks / 3, false)),
            Emerge => Or(("Emerge", 3, EmergeTicks / 3, false)),
            HandsWindup => Or(("Hands", 3, 14, false)),
            DashWindup or Dash => Or(("Dash", 2, 6, true)),
            WindWindup => Or(("Wind", 2, 20, false)),
            NeckWindup => Or(("Neck", 1, 10, true)),
            SealWindup => Or(("Seal", 2, 17, false)),
            Exit => Or(("Sink", 3, ThresholdRetreatRules.ExitInvulnerableTicks / 3, false)),
            _ => Has("Walk") ? Moving("Walk", "Idle") : ("Idle", 4, 12, true),
        };

    // Damage, eased for a player not yet strong enough.
    private int Dmg(int damage) => ExamBossRules.OrochimaruDamage(damage, ready ?? true);
    protected override int LifeMax => ExamBossRules.OrochimaruLife;
    protected override int Defense => ExamBossRules.OrochimaruDefense;

    private Vector2 Mark => new(NPC.ai[2], NPC.ai[3]);

    protected override void Fight(Player target)
    {
        NPC.color = Tint;
        if (summonPose > 0)
            summonPose--;
        if (ready == null)
        {
            ready = ChuninExamRules.ReadyForOrochimaru(NPC.downedBoss2, target.statLifeMax);
            rematch = StoryWorld.OrochimaruMet;
        }
        NPC.damage = State == Dash ? Dmg(ExamBossRules.SnakeDashDamage) : 0;
        if (State is not (Reveal or Emerge or Exit))
            fightTicks++;
        if (State != Exit)
            switch (ExamBossRules.OrochimaruEnds(NPC.life, NPC.lifeMax, fightTicks, ready ?? true, false))
            {
                case ExamBossRules.OrochimaruEnd.HalfLife:
                    NPC.life = ThresholdRetreatRules.LockedLife(NPC.lifeMax);
                    NPC.dontTakeDamage = true;
                    Enter(Exit);
                    break;
                case ExamBossRules.OrochimaruEnd.HeldOut:
                    NPC.dontTakeDamage = true;
                    Enter(Exit);
                    break;
            }
        if (Deciding && State is Approach or Recovery &&
            ExamBossRules.GiantSnakeDue(giantSnake, NPC.life, NPC.lifeMax, fightTicks))
        {
            giantSnake = true;
            summonPose = 40;
            Projectile.NewProjectile(NPC.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<GiantSnake>(),
                Dmg(ExamBossRules.GiantSnakeDamage), 9f, Main.myPlayer, Main.rand.NextBool() ? 1f : -1f, target.Bottom.Y);
        }
        if (Deciding && State is Approach or Recovery &&
            ExamBossRules.SummonSnakes(NPC.life / (float)NPC.lifeMax, snakeSummons))
        {
            snakeSummons++;
            SummonSnakes();
        }

        switch (State)
        {
            case Reveal:
                // He peels the candidate's face away; the killing intent follows.
                NPC.velocity.X *= 0.8f;
                Face(target.Center.X);
                if (Timer == 1f)
                {
                    Smoke();
                    KillingIntent();
                }
                if (Timer >= RevealTicks)
                    Enter(Approach);
                break;

            case Emerge:
                // Up out of the ground where the shed skin lies.
                NPC.velocity.X = 0f;
                NPC.alpha = (int)(255 * (1f - System.Math.Min(1f, Timer / EmergeTicks)));
                if (Main.netMode != NetmodeID.Server)
                    Dust.NewDust(NPC.BottomLeft + new Vector2(0f, -6f), NPC.width, 6, DustID.Dirt, 0f, -2f);
                if (Timer == 1f)
                    Tell("大蛇丸：“……真着急啊。”", new Color(190, 150, 230));
                if (Timer >= EmergeTicks)
                {
                    NPC.alpha = 0;
                    Face(target.Center.X);
                    Enter(Approach);
                }
                break;

            case Approach:
                RunTo(target.Center.X - Math.Sign(target.Center.X - NPC.Center.X) * 9 * 16, 3.2f, target);
                if (Deciding && Timer > 55f)
                    Choose(target);
                break;

            case HandsWindup:
                NPC.velocity.X *= 0.8f;
                Telegraph(DustID.PurpleTorch, 18f);
                if (Timer >= 36f)
                {
                    SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                    if (Deciding)
                    {
                        Vector2 aim = NPC.DirectionTo(Mark);
                        float speed = ExamBossRules.SnakeHandReachPx / (JutsuHitbox.Lifetime(JutsuKind.SnakeHand) / 2f);
                        for (int i = -1; i <= 1; i++)
                            JutsuHitbox.Spawn(NPC, JutsuKind.SnakeHand, NPC.Center, aim.RotatedBy(i * 0.12f) * speed, 22, 22,
                                Dmg(ExamBossRules.SnakeHandDamage));
                    }
                    Enter(Recovery);
                }
                break;

            case DashWindup:
                NPC.velocity.X *= 0.8f;
                // The path along the ground, round behind the player.
                if (Main.netMode != NetmodeID.Server)
                    Dust.NewDustPerfect(new Vector2(MathHelper.Lerp(NPC.Center.X, Mark.X, Main.rand.NextFloat()), NPC.Bottom.Y - 2f),
                        DustID.Venom, Vector2.Zero, 0, default, 1.1f).noGravity = true;
                if (Timer >= 30f)
                {
                    SoundEngine.PlaySound(SoundID.Item18, NPC.Center);
                    Enter(Dash);
                }
                break;

            case Dash:
                NPC.noTileCollide = true;
                NPC.velocity = new Vector2(Math.Sign(Mark.X - NPC.Center.X) * 16f, 0f);
                if (Math.Abs(Mark.X - NPC.Center.X) < 20f || Timer > 40f)
                {
                    NPC.noTileCollide = false;
                    NPC.velocity.X = 0f;
                    Face(target.Center.X);
                    if (Deciding)
                        JutsuHitbox.Spawn(NPC, JutsuKind.Strike, NPC.Center + new Vector2(NPC.direction * 26f, 0f), Vector2.Zero, 44, 44,
                            Dmg(ExamBossRules.SnakeDashDamage));
                    Enter(Recovery);
                }
                break;

            case WindWindup:
                NPC.velocity.X *= 0.8f;
                Telegraph(DustID.Cloud, 34f);
                if (Timer >= 40f)
                {
                    SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                    if (Deciding)
                    {
                        JutsuHitbox.Spawn(NPC, JutsuKind.WindBlast, NPC.Center + new Vector2(NPC.direction * 40f, 0f),
                            new Vector2(NPC.direction * 9f, 0f), 90, 130, Dmg(ExamBossRules.WindBlastDamage), ExamBossRules.WindBlastKnockback);
                        // Blown back, then the snakes follow.
                        NPC.ai[2] = target.Center.X + NPC.direction * 10 * 16;
                        NPC.ai[3] = target.Center.Y;
                    }
                    State = HandsWindup;
                    Timer = 14f;
                    NPC.netUpdate = true;
                }
                break;

            case NeckWindup:
                NPC.velocity.X *= 0.8f;
                // The long neck snakes out to where the player stood.
                if (Main.netMode != NetmodeID.Server)
                {
                    Vector2 along = Vector2.Lerp(NPC.Top, Mark, Main.rand.NextFloat() * Math.Min(1f, Timer / 40f));
                    Dust.NewDustPerfect(along, DustID.PurpleTorch, Vector2.Zero, 0, default, 1f).noGravity = true;
                }
                if (Timer >= 48f)
                {
                    SoundEngine.PlaySound(SoundID.Item2, Mark);
                    if (Deciding)
                        JutsuHitbox.Spawn(NPC, JutsuKind.Strike, Mark, Vector2.Zero, 40, 40, Dmg(ExamBossRules.NeckBiteDamage));
                    Enter(Recovery);
                }
                break;

            case SealWindup:
                // He closes in with the seal glowing on his fingertips, then presses it on.
                Face(target.Center.X);
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * 6f, 0.15f);
                Telegraph(DustID.PurpleTorch, 14f);
                if (Timer >= 34f)
                {
                    SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                    if (Deciding)
                        JutsuHitbox.Spawn(NPC, JutsuKind.FiveSeal, NPC.Center + new Vector2(NPC.direction * 30f, 0f),
                            new Vector2(NPC.direction * 4f, 0f), 48, 52, Dmg(ExamBossRules.FiveSealDamage));
                    Enter(Recovery);
                }
                break;

            case Recovery:
                NPC.velocity.X *= 0.85f;
                if (Timer >= 40f)
                    Enter(Approach);
                break;

            case Exit:
                NPC.velocity.X *= 0.8f;
                if (Main.netMode != NetmodeID.Server)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Venom, 0f, 2f, 0, default, 1.3f);
                if (Timer >= ThresholdRetreatRules.ExitInvulnerableTicks)
                    Leave();
                break;
        }
    }

    private void Choose(Player target)
    {
        Face(target.Center.X);
        NPC.ai[2] = target.Center.X;
        NPC.ai[3] = target.Center.Y;
        switch (Main.rand.Next(5))
        {
            case 4:
                Enter(SealWindup);
                break;
            case 0:
                Enter(HandsWindup);
                break;
            case 1:
                // Round behind the player.
                NPC.ai[2] = target.Center.X + Math.Sign(target.Center.X - NPC.Center.X) * 5 * 16;
                Enter(DashWindup);
                break;
            case 2:
                Enter(WindWindup);
                break;
            default:
                Enter(NeckWindup);
                break;
        }
    }

    // Everyone near enough freezes (each client for its own player) until they substitute out.
    private void KillingIntent()
    {
        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
        Player local = Main.LocalPlayer;
        if (Main.netMode == NetmodeID.Server || !local.active || local.dead || Vector2.Distance(local.Center, NPC.Center) > 60 * 16)
            return;
        local.GetModPlayer<JutsuStatusPlayer>().Fear(ExamBossRules.KillingIntentTicks);
        Main.NewText($"杀气……身体动不了！按【{ShinobiKeybinds.SubstitutionKeyName()}】用替身术挣脱！", 220, 90, 110);
    }

    private void SummonSnakes()
    {
        summonPose = 40;
        for (int side = -1; side <= 1; side += 2)
            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + side * 5 * 16, (int)NPC.Bottom.Y, ModContent.NPCType<SummonedSnake>());
    }

    // The player fell: he spares them (the encounter still counts).
    protected override bool TargetGone(Player target)
    {
        if (!target.dead || State == Exit)
            return false;
        Tell("大蛇丸：“……还不是时候。”", new Color(190, 150, 230));
        Leave();
        return true;
    }

    private void Leave()
    {
        Smoke();
        // Each client credits its own player if they were there (the first meeting leaves the Sharingan in a vial).
        if (Main.netMode != NetmodeID.Server && Main.LocalPlayer.active && Main.LocalPlayer.Distance(NPC.Center) < 150 * 16)
            Main.LocalPlayer.GetModPlayer<ChuninExamPlayer>().CreditOrochimaru();
        if (Deciding)
        {
            StoryWorld.OrochimaruMet = true;
            Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<SnakeSkin>());
            // Called back with the shed skin: the eye only by chance.
            if (rematch && Main.rand.NextFloat() < ExamBossRules.SharinganVialChance)
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<Items.StyleCores.SharinganCore1>());
            Tell("大蛇丸化作一群蛇，钻进了土里。地上只留下一张蛇蜕——在丛林里用它，还能把他引出来。", new Color(190, 150, 230));
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
            foreach (NPC snake in Main.ActiveNPCs)
                if (snake.type == ModContent.NPCType<SummonedSnake>())
                {
                    snake.active = false;
                    snake.netUpdate = true;
                }
        }
        NPC.active = false;
        NPC.netUpdate = true;
    }

    public override void PostDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        const string root = "ShinobiPrototype/Content/NPCs/Orochimaru_";
        if (State != NeckWindup || !ModContent.HasAsset(root + "NeckSegment") || !ModContent.HasAsset(root + "NeckHead"))
            return;
        var segment = ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(root + "NeckSegment").Value;
        var head = ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(root + "NeckHead").Value;
        Color light = BossSprites.Lit(drawColor);
        // From the collar out towards where the player stood, snaking a little on the way.
        Vector2 from = NPC.Top + new Vector2(NPC.direction * 6f, 14f);
        Vector2 to = Vector2.Lerp(from, Mark, Math.Min(1f, Timer / 40f));
        float length = Vector2.Distance(from, to);
        Vector2 dir = length > 0f ? (to - from) / length : Vector2.UnitX;
        Vector2 side = new(-dir.Y, dir.X);
        int steps = (int)(length / 6f);
        for (int i = 0; i < steps; i++)
        {
            float t = i / (float)Math.Max(1, steps);
            Vector2 at = from + dir * length * t + side * (float)Math.Sin(t * MathHelper.TwoPi * 1.5f + Main.GameUpdateCount * 0.2f) * 10f * t;
            spriteBatch.Draw(segment, at - screenPos, null, light, 0f, segment.Size() / 2f, 1f,
                Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
        }
        float angle = (float)Math.Atan2(dir.Y, dir.X);
        bool left = dir.X < 0f;
        spriteBatch.Draw(head, to - screenPos, null, light, left ? angle - MathHelper.Pi : angle, head.Size() / 2f, 1f,
            left ? Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally : Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
    }

    // One burst hit cannot carry him past half.
    public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers) => Cap(ref modifiers);

    public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers) => Cap(ref modifiers);

    private void Cap(ref NPC.HitModifiers modifiers) =>
        modifiers.SetMaxDamage(Math.Max(1, NPC.life - ThresholdRetreatRules.LockedLife(NPC.lifeMax)));

    public override bool CheckDead()
    {
        NPC.life = ThresholdRetreatRules.LockedLife(NPC.lifeMax);
        NPC.dontTakeDamage = true;
        return false;
    }
}

// A snake summoned by Orochimaru: crawls at the player along the ground, writhing (drawn in code, SnakeBody).
public sealed class SummonedSnake : ModNPC
{
    public override string Texture => "ShinobiPrototype/Content/NPCs/ForestExamCandidate";

    public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 1;

    public override void SetDefaults()
    {
        NPC.width = 36;
        NPC.height = 18;
        NPC.lifeMax = ExamBossRules.SnakeLife;
        NPC.damage = ExamBossRules.SnakeDamage;
        NPC.defense = 6;
        NPC.knockBackResist = 0.2f;
        NPC.aiStyle = -1;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
    }

    public override void AI()
    {
        NPC.TargetClosest();
        Player target = Main.player[NPC.target];
        if (!target.active || target.dead)
        {
            NPC.EncourageDespawn(30);
            return;
        }
        NPC.direction = NPC.spriteDirection = target.Center.X >= NPC.Center.X ? 1 : -1;
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * 4f, 0.06f);
        if (NPC.collideX && NPC.velocity.Y == 0f)
            NPC.velocity.Y = -6.5f;
    }

    // A green snake writhing along the ground behind its head (SnakeBody).
    public override bool PreDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        float time = Main.GameUpdateCount + NPC.whoAmI * 13;
        SnakeBody.Draw(spriteBatch, NPC.Bottom + new Vector2(NPC.direction * 12f, 0f), NPC.direction, 9, 16f, time,
            SnakeBody.Green, BossSprites.Lit(drawColor, 0.35f));
        return false;
    }
}
