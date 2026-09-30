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

// Orochimaru in the Forest of Death (specs/M2_中忍考试篇.spec.md section 5), a fight that cannot be won: he comes as a
// Grass candidate, drops the disguise and freezes the player with killing intent (substitution breaks it), then Hidden
// Shadow Snake Hands, a snake dash round behind the player, the Great Breakthrough gust, the Five Elements Seal (chakra
// stops recovering), summoned snakes and the long neck. At half life he stops taking damage, says his line and sinks away as snakes (ThresholdRetreatRules), leaving
// his shed skin to call him back. The Sharingan vial waits on the style spec.
public sealed class Orochimaru : ExamBoss
{
    private const float Disguise = 0f;
    private const float Approach = 1f;
    private const float HandsWindup = 2f;
    private const float DashWindup = 3f;
    private const float Dash = 4f;
    private const float WindWindup = 5f;
    private const float NeckWindup = 6f;
    private const float Recovery = 7f;
    private const float Exit = 8f;
    private const float SealWindup = 9f;

    private const int DisguiseTicks = 110;
    private const int IntentTick = 80;

    private int snakeSummons;

    protected override Color Tint => State == Disguise ? new Color(180, 225, 170) : new Color(150, 140, 175);
    protected override int LifeMax => ExamBossRules.OrochimaruLife;
    protected override int Defense => ExamBossRules.OrochimaruDefense;

    private Vector2 Mark => new(NPC.ai[2], NPC.ai[3]);

    protected override void Fight(Player target)
    {
        NPC.color = Tint;
        NPC.damage = State == Dash ? ExamBossRules.SnakeDashDamage : 0;
        if (State != Exit && ThresholdRetreatRules.Reached(NPC.life, NPC.lifeMax))
        {
            NPC.life = ThresholdRetreatRules.LockedLife(NPC.lifeMax);
            NPC.dontTakeDamage = true;
            Say("……真有意思。这个孩子，将来会是个好容器。", new Color(190, 150, 230));
            Enter(Exit);
        }
        if (Deciding && State is Approach or Recovery &&
            ExamBossRules.SummonSnakes(NPC.life / (float)NPC.lifeMax, snakeSummons))
        {
            snakeSummons++;
            SummonSnakes();
        }

        switch (State)
        {
            case Disguise:
                RunTo(target.Center.X, 2.2f, target);
                if (Timer == 30f)
                    Say("……你的卷轴，是天之卷？还是地之卷？", new Color(200, 230, 190));
                if (Timer == IntentTick)
                {
                    Smoke();
                    Say("变身术——解。", new Color(190, 150, 230));
                    KillingIntent();
                }
                if (Timer >= DisguiseTicks)
                    Enter(Approach);
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
                    Say("潜影蛇手！", new Color(190, 150, 230));
                    SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                    if (Deciding)
                    {
                        Vector2 aim = NPC.DirectionTo(Mark);
                        float speed = ExamBossRules.SnakeHandReachPx / (JutsuHitbox.Lifetime(JutsuKind.SnakeHand) / 2f);
                        for (int i = -1; i <= 1; i++)
                            JutsuHitbox.Spawn(NPC, JutsuKind.SnakeHand, NPC.Center, aim.RotatedBy(i * 0.12f) * speed, 22, 22,
                                ExamBossRules.SnakeHandDamage);
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
                            ExamBossRules.SnakeDashDamage);
                    Enter(Recovery);
                }
                break;

            case WindWindup:
                NPC.velocity.X *= 0.8f;
                Telegraph(DustID.Cloud, 34f);
                if (Timer >= 40f)
                {
                    Say("风遁·大突破！", new Color(190, 150, 230));
                    SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                    if (Deciding)
                    {
                        JutsuHitbox.Spawn(NPC, JutsuKind.WindBlast, NPC.Center + new Vector2(NPC.direction * 40f, 0f),
                            new Vector2(NPC.direction * 9f, 0f), 90, 130, ExamBossRules.WindBlastDamage, ExamBossRules.WindBlastKnockback);
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
                        JutsuHitbox.Spawn(NPC, JutsuKind.Strike, Mark, Vector2.Zero, 40, 40, ExamBossRules.NeckBiteDamage);
                    Enter(Recovery);
                }
                break;

            case SealWindup:
                // He closes in with the seal glowing on his fingertips, then presses it on.
                Face(target.Center.X);
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * 6f, 0.15f);
                Telegraph(DustID.PurpleTorch, 14f);
                if (Timer == 1f)
                    Say("五行封印！", new Color(190, 150, 230));
                if (Timer >= 34f)
                {
                    SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                    if (Deciding)
                        JutsuHitbox.Spawn(NPC, JutsuKind.FiveSeal, NPC.Center + new Vector2(NPC.direction * 30f, 0f),
                            new Vector2(NPC.direction * 4f, 0f), 48, 52, ExamBossRules.FiveSealDamage);
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
        Say("通灵之术！", new Color(190, 150, 230));
        for (int side = -1; side <= 1; side += 2)
            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + side * 5 * 16, (int)NPC.Bottom.Y, ModContent.NPCType<SummonedSnake>());
    }

    private void Leave()
    {
        Smoke();
        if (Deciding)
        {
            StoryWorld.OrochimaruMet = true;
            Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<SnakeSkin>());
            Tell("大蛇丸化作一群蛇，钻进了土里。地上只留下一张蛇蜕。", new Color(190, 150, 230));
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

// A snake summoned by Orochimaru: crawls at the player along the ground. Placeholder art: a green body of segments.
public sealed class SummonedSnake : ModNPC
{
    public override string Texture => "ShinobiPrototype/Content/NPCs/ForestExamCandidate";

    private readonly Vector2[] trail = new Vector2[10];

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
        for (int i = trail.Length - 1; i > 0; i--)
            trail[i] = trail[i - 1];
        trail[0] = NPC.Center;
    }

    public override bool PreDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        var pixel = Terraria.GameContent.TextureAssets.MagicPixel.Value;
        for (int i = trail.Length - 1; i >= 0; i--)
        {
            Vector2 at = trail[i] == Vector2.Zero ? NPC.Center : trail[i];
            at.Y += (float)Math.Sin((Main.GameUpdateCount + i * 6) * 0.2f) * 3f;
            float size = i == 0 ? 16f : 13f - i * 0.6f;
            Color color = (i == 0 ? new Color(90, 150, 70) : new Color(70, 120, 60)).MultiplyRGB(drawColor);
            spriteBatch.Draw(pixel, at - screenPos, new Rectangle(0, 0, 1, 1), color, 0f, new Vector2(0.5f), size,
                Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
        }
        return false;
    }
}
