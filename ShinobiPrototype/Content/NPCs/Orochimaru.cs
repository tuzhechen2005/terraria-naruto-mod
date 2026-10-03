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
// Breakthrough gust, the Five Elements Seal (chakra stops recovering), the long neck, and once the giant snake (Manda)
// sliding across the ground. He sinks away as snakes at half life, once the player has held out long
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
    // Bullets (user, 2026-10-02).
    private const float SwarmWindup = 11f;
    private const float VenomWindup = 12f;
    private const float RainWindup = 13f;
    private const float SwordWindup = 14f;

    private const int RevealTicks = 70;
    private const int EmergeTicks = 60;
    private const int ExitTalkTicks = 70;     // standing while he speaks, then sinking for the rest of the exit
    private const int NeckLaunch = 10;
    private const int NeckReachTicks = 14;

    private int fightTicks;
    private int barrage;
    private float lastMove = -1f;
    private bool secondIntent;
    // The killing intent is a stare (ExamBossRules): warned for a second, a red wedge where he is looking, then it falls.
    private int stareTicks;
    private float stareAim;
    private bool giantSnake;
    private bool? ready;
    private bool rematch;

    protected override Color Tint => new(150, 140, 175);

    // Art: orochimaru-style-v3, orochimaru-moves-v1 (moving, openings, leaving) and -v2 (techniques). A technique
    // without its frames yet stands in with the idle frames; without any, the tinted placeholder is drawn.
    protected override string SpritePrefix => Has("Idle") ? "Orochimaru" : null;

    // Drawn one to one (user, 2026-10-02: shrunk into the 1.5x format the art lost all its detail; orochimaru-base-v11
    // and orochimaru-set-v11a/b): about 104 pixels tall on a 224x136 canvas, feet on row 131.
    private static readonly BossSprites.Canvas DenseCanvas = new(224, 136, 112, 132);

    private protected override BossSprites.Canvas CanvasFor(string action) => DenseCanvas;

    // A little larger than one to one (user, 2026-10-02: "可以再稍微大一些"), about 120 pixels tall.
    protected override float SpriteScale => 1.15f;

    protected override bool Tilts => false;

    private static bool Has(string action) => BossSprites.Has($"Orochimaru_{action}_0");

    private static int IdleFrames => BossSprites.Has("Orochimaru_Idle_5") ? 6 : 4;
    private static int WalkFrames => BossSprites.Has("Orochimaru_Walk_7") ? 8 : 4;

    private (string, int, int, bool) Or((string Action, int Frames, int Ticks, bool Loop) pose)
    {
        if (!Has(pose.Action))
            return ("Idle", IdleFrames, 10, true);
        // A technique opens on its in-between frame (orochimaru-tween-v1) for the first few ticks.
        if (Timer < InTicks && State != Dash && State != Recovery && Has(pose.Action + "In"))
            return (pose.Action + "In", 1, InTicks, true);
        return pose;
    }

    private const int InTicks = 6;
    private const int HoldTicks = 10;
    private const int OutTicks = 8;

    private static int DashFrames => BossSprites.Has("Orochimaru_Dash_3") ? 4 : 2;

    // The technique a state shows, how many frames it has, and whether its last pose is held after it (not the neck:
    // its last frame has no head).
    private static (string Action, int Frames, bool Hold)? TechniqueOf(float state) => state switch
    {
        HandsWindup or SwarmWindup => ("Hands", 1, true),
        DashWindup or Dash => ("Dash", 1, true),
        WindWindup or VenomWindup or SwordWindup => ("Wind", 2, true),
        NeckWindup => ("Neck", 1, false),
        SealWindup => ("Seal", 2, true),
        RainWindup => ("Summon", 2, true),
        _ => null,
    };

    private (string Action, int Frames, bool Hold)? lastTechnique;

    protected override int PoseFirstFrame =>
        State == Recovery && lastTechnique is { Hold: true } held && Timer < HoldTicks && Has(held.Action) ? held.Frames - 1 : 0;

    private int summonPose;

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        // Where he was a moment ago: the after-images of the snake dash.
        NPCID.Sets.TrailCacheLength[Type] = 7;
        NPCID.Sets.TrailingMode[Type] = 0;
    }

    // The snake dash leaves a fading violet trail of after-images (orochimaru effects, user 2026-10-01).
    public override bool PreDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (State == Dash && SpritePrefix != null)
            // Two after-images, well apart (user, 2026-10-02: six close together flickered).
            for (int i = 6; i >= 3; i -= 3)
            {
                if (i >= NPC.oldPos.Length || NPC.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 bottom = NPC.oldPos[i] + new Vector2(NPC.width / 2f, NPC.height);
                float fade = i == 3 ? 0.4f : 0.2f;
                BossSprites.TryDraw(spriteBatch, "Orochimaru", "Dash", 0, 2, DenseCanvas, bottom, NPC.direction,
                    new Color(170, 110, 230) * fade, screenPos, SpriteScale);
            }
        return base.PreDraw(spriteBatch, screenPos, drawColor);
    }

    protected override (string Action, int Frames, int TicksPerFrame, bool Loop) Pose => summonPose > 0
        ? Or(("Summon", 2, 20, false))
        : State switch
        {
            Reveal => Or(("Reveal", 3, RevealTicks / 3, false)),
            Emerge => Or(("Emerge", 3, EmergeTicks / 3, false)),
            // One sleeve held out, open, while the snakes come out of it (the reaching frames looked like pipes).
            HandsWindup => Or(("Hands", 1, 12, true)),
            DashWindup or Dash => Or(("Dash", DashFrames, 8, true)),
            WindWindup => Or(("Wind", 2, 20, false)),
            // The head stays on until the neck starts to stretch (it went missing for a few ticks before).
            NeckWindup when Timer < NeckLaunch && Has("NeckIn") => ("NeckIn", 1, NeckLaunch, true),
            NeckWindup => Or(("Neck", 1, 10, true)),
            SealWindup => Or(("Seal", 2, 12, false)),
            SwarmWindup => Or(("Hands", 1, 12, true)),
            VenomWindup or SwordWindup => Or(("Wind", 2, 12, false)),
            RainWindup => Or(("Summon", 2, 12, false)),
            // After a technique: its last pose held a moment, then one in-between frame back to standing.
            Recovery when lastTechnique is { } held && held.Hold && Timer < HoldTicks => Or((held.Action, 1, HoldTicks, true)),
            Recovery when lastTechnique is { } back && Timer < (back.Hold ? HoldTicks : 0) + OutTicks && Has(back.Action + "Out") =>
                (back.Action + "Out", 1, OutTicks, true),
            Exit when Timer < ExitTalkTicks => ("Idle", IdleFrames, 10, true),
            Exit => Or(("Sink", 3, (ThresholdRetreatRules.ExitInvulnerableTicks - ExitTalkTicks) / 3, false)),
            _ => Has("Walk") ? Moving("Walk", "Idle", WalkFrames, IdleFrames) : ("Idle", IdleFrames, 10, true),
        };

    // Damage, eased for a player not yet strong enough.
    private int Dmg(int damage) => ExamBossRules.OrochimaruDamage(damage, ready ?? true);
    protected override int LifeMax => ExamBossRules.OrochimaruLife;
    protected override int Defense => ExamBossRules.OrochimaruDefense;

    private Vector2 Mark => new(NPC.ai[2], NPC.ai[3]);

    // The long neck: from his collar to the head, which lunges out over NeckReachTicks and stays there to bite.
    // The end of the neck stump in the neck frame (orochimaru-moves-v11d: canvas (126, 57), feet at (112, 132)).
    private Vector2 NeckRoot => NPC.Bottom + new Vector2(NPC.direction * 14f, -75f) * SpriteScale;
    private const int NeckHoldTicks = 12;
    private const int NeckBackTicks = 8;

    // Out over NeckReachTicks, held while it bites, then drawn back in before he straightens.
    private float NeckReach
    {
        get
        {
            if (State != NeckWindup || Timer < NeckLaunch)
                return 0f;
            float t = Timer - NeckLaunch;
            if (t < NeckReachTicks)
                return (t + 1f) / NeckReachTicks;
            if (t < NeckReachTicks + NeckHoldTicks)
                return 1f;
            return Math.Max(0f, 1f - (t - NeckReachTicks - NeckHoldTicks + 1f) / NeckBackTicks);
        }
    }
    public Vector2 NeckHead => Vector2.Lerp(NeckRoot, Mark, NeckReach);

    // Sinking into the ground once he has had his say (ExamBoss draws him that many art pixels lower, cut at the ground).
    protected override float SinkPixels => State == Exit && Timer > ExitTalkTicks
        ? 100f * (Timer - ExitTalkTicks) / (ThresholdRetreatRules.ExitInvulnerableTicks - ExitTalkTicks)
        : 0f;

    protected override void Fight(Player target)
    {
        NPC.color = Tint;
        UpdateStare();
        if (State != Recovery)
            lastTechnique = TechniqueOf(State);
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
                    Speak("……呵呵呵，你的身体，比我想的还要有意思呢。下次见面之前，可别随随便便就死掉哦。");
                    Enter(Exit);
                    break;
                case ExamBossRules.OrochimaruEnd.HeldOut:
                    NPC.dontTakeDamage = true;
                    Speak("……拼命挣扎的样子，也挺可爱的嘛。时机还没到——好好留着这条命，等我来取。");
                    Enter(Exit);
                    break;
            }
        // Once more, a quarter of the way down (user, 2026-10-02).
        if (!secondIntent && State is Approach or Recovery && NPC.life <= NPC.lifeMax * 0.75f)
        {
            secondIntent = true;
            KillingIntent();
        }
        // Between techniques he flicks small snakes at the player as he walks; thicker after Manda.
        if (Deciding && State is Approach or Recovery && ++barrage >= ExamBossRules.BarrageEvery(giantSnake))
        {
            barrage = 0;
            Vector2 aim = NPC.DirectionTo(target.Center);
            int count = ExamBossRules.BarrageCount(giantSnake);
            for (int i = 0; i < count; i++)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 20f, -10f),
                    aim.RotatedBy((i - (count - 1) / 2f) * 0.16f) * 8.5f, ModContent.ProjectileType<SnakeBullet>(),
                    Dmg(ExamBossRules.BarrageDamage), 0f, Main.myPlayer);
        }
        if (Deciding && State is Approach or Recovery &&
            ExamBossRules.GiantSnakeDue(giantSnake, NPC.life, NPC.lifeMax, fightTicks))
        {
            giantSnake = true;
            summonPose = 40;
            Projectile.NewProjectile(NPC.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<GiantSnake>(),
                Dmg(ExamBossRules.GiantSnakeDamage), 9f, Main.myPlayer, Main.rand.NextBool() ? 1f : -1f, target.Bottom.Y);
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
                    // Out of the ground, the killing intent (both openings have it; user 2026-10-02).
                    KillingIntent();
                    Enter(Approach);
                }
                break;

            case Approach:
                RunTo(target.Center.X - Math.Sign(target.Center.X - NPC.Center.X) * 9 * 16, 3.2f, target);
                if (Deciding && Timer > ExamBossRules.OrochimaruDecideTicks)
                    Choose(target);
                break;

            case HandsWindup:
                NPC.velocity.X *= 0.8f;
                Telegraph(DustID.PurpleTorch, 18f);
                // The snakes leave his sleeves at the 24th tick; he holds his arms out until they are back in.
                if ((int)Timer == 24)
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
                }
                if (Timer >= 24 + JutsuHitbox.Lifetime(JutsuKind.SnakeHand))
                    Enter(Recovery);
                break;

            case DashWindup:
                NPC.velocity.X *= 0.8f;
                // The path along the ground, round behind the player.
                if (Main.netMode != NetmodeID.Server)
                    Dust.NewDustPerfect(new Vector2(MathHelper.Lerp(NPC.Center.X, Mark.X, Main.rand.NextFloat()), NPC.Bottom.Y - 2f),
                        DustID.Venom, Vector2.Zero, 0, default, 1.1f).noGravity = true;
                if (Timer >= 20f)
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
                        JutsuHitbox.Spawn(NPC, JutsuKind.Bite, NPC.Center + new Vector2(NPC.direction * 30f, 0f), Vector2.Zero, 48, 48,
                            Dmg(ExamBossRules.SnakeDashDamage));
                    Enter(Recovery);
                }
                break;

            case WindWindup:
                NPC.velocity.X *= 0.8f;
                Telegraph(DustID.Cloud, 34f);
                if (Timer >= 27f)
                {
                    SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                    if (Deciding)
                    {
                        JutsuHitbox.Spawn(NPC, JutsuKind.WindBlast, NPC.Center + new Vector2(NPC.direction * 40f, 0f),
                            new Vector2(NPC.direction * 11f, 0f), 90, 130, Dmg(ExamBossRules.WindBlastDamage), ExamBossRules.WindBlastKnockback);
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
                // A short coil, then the head lunges at where the player is now and bites on contact
                // (user, 2026-10-02: it was slow and its touch did nothing).
                NPC.velocity.X *= 0.8f;
                if (Timer == NeckLaunch)
                {
                    NPC.ai[2] = target.Center.X;
                    NPC.ai[3] = target.Center.Y;
                    NPC.netUpdate = true;
                    SoundEngine.PlaySound(SoundID.Item2, NPC.Center);
                    if (Deciding)
                        JutsuHitbox.Spawn(NPC, JutsuKind.NeckHead, NeckHead, Vector2.Zero, 40, 40, Dmg(ExamBossRules.NeckBiteDamage));
                }
                if (Timer == NeckLaunch + NeckReachTicks && Deciding)
                    JutsuHitbox.Spawn(NPC, JutsuKind.Bite, Mark, Vector2.Zero, 44, 44, Dmg(ExamBossRules.NeckBiteDamage));
                if (Timer >= NeckLaunch + NeckReachTicks + NeckHoldTicks + NeckBackTicks)
                    Enter(Recovery);
                break;

            case SealWindup:
                // He closes in with the seal glowing on his fingertips, then presses it on.
                Face(target.Center.X);
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * 6f, 0.15f);
                if (!FxArt.Has("FxSealFlame_0"))
                    Telegraph(DustID.PurpleTorch, 14f);
                if (Timer >= 23f)
                {
                    SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                    if (Deciding)
                        JutsuHitbox.Spawn(NPC, JutsuKind.FiveSeal, NPC.Center + new Vector2(NPC.direction * 30f, 0f),
                            new Vector2(NPC.direction * 4f, 0f), 48, 52, Dmg(ExamBossRules.FiveSealDamage));
                    Enter(Recovery);
                }
                break;

            case SwarmWindup:
                // A fan of snakes straight at the player.
                NPC.velocity.X *= 0.8f;
                Face(target.Center.X);
                if (Timer >= 20f)
                {
                    SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                    if (Deciding)
                    {
                        Vector2 aim = NPC.DirectionTo(target.Center);
                        int count = ExamBossRules.SwarmCount(giantSnake);
                        for (int i = 0; i < count; i++)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 24f, -8f),
                                aim.RotatedBy((i - (count - 1) / 2f) * 0.15f) * 10f, ModContent.ProjectileType<SnakeBullet>(),
                                Dmg(ExamBossRules.SwarmDamage), 0f, Main.myPlayer);
                    }
                    Enter(Recovery);
                }
                break;

            case VenomWindup:
                // Three globs of venom in arcs around the player; each leaves a pool.
                NPC.velocity.X *= 0.8f;
                Face(target.Center.X);
                if (Timer >= 24f)
                {
                    SoundEngine.PlaySound(SoundID.Item95, NPC.Center);
                    if (Deciding)
                        for (int i = -1; i <= 1; i++)
                        {
                            float dx = target.Center.X + i * 4 * 16f - NPC.Center.X;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Top + new Vector2(NPC.direction * 12f, 14f),
                                new Vector2(MathHelper.Clamp(dx / 42f, -11f, 11f), -8f), ModContent.ProjectileType<VenomGlob>(),
                                Dmg(ExamBossRules.VenomDamage), 0f, Main.myPlayer);
                        }
                    Enter(Recovery);
                }
                break;

            case RainWindup:
                // Snakes hang above the player, glinting, then drop one after another.
                NPC.velocity.X *= 0.8f;
                if (Timer >= 20f)
                {
                    SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                    if (Deciding)
                    {
                        int count = ExamBossRules.SnakeRainCount(giantSnake);
                        for (int i = 0; i < count; i++)
                        {
                            float x = target.Center.X + (i - (count - 1) / 2f) * ExamBossRules.SnakeRainGapPx;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(x, target.Center.Y - 380f), Vector2.Zero,
                                ModContent.ProjectileType<SnakeBullet>(), Dmg(ExamBossRules.SnakeRainDamage), 0f, Main.myPlayer,
                                45 + i * 8, 1f);
                        }
                    }
                    Enter(Recovery);
                }
                break;

            case SwordWindup:
                // The Kusanagi out of his mouth: an aiming line, then the blade along it.
                NPC.velocity.X *= 0.7f;
                if (Timer == 1f)
                {
                    Face(target.Center.X);
                    if (Deciding)
                    {
                        Vector2 mouth = NPC.Top + new Vector2(NPC.direction * 14f, 16f);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), mouth, Vector2.Zero, ModContent.ProjectileType<KusanagiBlade>(),
                            Dmg(ExamBossRules.KusanagiDamage), 6f, Main.myPlayer, (target.Center - mouth).ToRotation());
                    }
                }
                if (Timer >= KusanagiBlade.AimTicks + 16)
                    Enter(Recovery);
                break;

            case Recovery:
                NPC.velocity.X *= 0.85f;
                if (Timer >= ExamBossRules.OrochimaruRecoveryTicks)
                    Enter(Approach);
                break;

            case Exit:
                // He has his say first, then sinks into the ground as snakes (user, 2026-10-02: he just vanished).
                NPC.velocity.X *= 0.8f;
                if (Timer >= ExitTalkTicks && Main.netMode != NetmodeID.Server)
                    for (int i = 0; i < 2; i++)
                        Dust.NewDustPerfect(new Vector2(Main.rand.NextFloat(NPC.Left.X - 16f, NPC.Right.X + 16f), NPC.Bottom.Y - 4f),
                            Main.rand.NextBool() ? DustID.Venom : DustID.Dirt, new Vector2(0f, Main.rand.NextFloat(0.5f, 2f)), 0, default, 1.3f);
                if (Timer >= ThresholdRetreatRules.ExitInvulnerableTicks)
                    Leave();
                break;
        }
    }

    // Nine techniques; never the same one twice running. Far off, the bullets come up more often.
    private void Choose(Player target)
    {
        Face(target.Center.X);
        NPC.ai[2] = target.Center.X;
        NPC.ai[3] = target.Center.Y;
        bool far = Math.Abs(target.Center.X - NPC.Center.X) > 18 * 16;
        float[] options = far
            ? new[] { SwarmWindup, VenomWindup, RainWindup, SwordWindup, HandsWindup, WindWindup, DashWindup, SwarmWindup, RainWindup }
            : new[] { HandsWindup, DashWindup, WindWindup, NeckWindup, SealWindup, SwarmWindup, VenomWindup, RainWindup, SwordWindup };
        float next = options[Main.rand.Next(options.Length)];
        if (next == lastMove)
            next = options[(Array.IndexOf(options, next) + 1) % options.Length];
        lastMove = next;
        if (next == DashWindup)
            // Round behind the player.
            NPC.ai[2] = target.Center.X + Math.Sign(target.Center.X - NPC.Center.X) * 5 * 16;
        Enter(next);
    }

    // A line said over his head and in the chat.
    private void Speak(string line)
    {
        Tell("大蛇丸：“" + line + "”", new Color(190, 150, 230));
        if (Main.netMode != NetmodeID.Server)
            CombatText.NewText(NPC.getRect(), new Color(200, 160, 240), line, true);
    }

    // Everyone near enough freezes (each client for its own player) until they substitute out.
    // His eyes in the idle frame (canvas (119, 38), feet at (112, 132)), at his size.
    private Vector2 Eyes => NPC.Bottom + new Vector2(NPC.direction * 7f, -94f) * SpriteScale;

    // The killing intent starts: he stares at where the player is now (user, 2026-10-03: it used to freeze at once, with
    // no way to avoid it).
    private void KillingIntent()
    {
        if (stareTicks > 0)
            return;
        stareTicks = ExamBossRules.KillingIntentWarnTicks;
        stareAim = (Main.player[NPC.target].Center - Eyes).ToRotation();
        SoundEngine.PlaySound(SoundID.Item8 with { Pitch = -0.7f, Volume = 0.9f }, NPC.Center);
    }

    // It falls (each client for its own player): inside the wedge and in his sight, a log takes it, or the player freezes.
    private void UpdateStare()
    {
        if (stareTicks <= 0 || --stareTicks > 0)
            return;
        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
        Player local = Main.LocalPlayer;
        if (Main.netMode == NetmodeID.Server || !local.active || local.dead)
            return;
        Vector2 d = local.Center - Eyes;
        if (!ExamBossRules.InStare(d.X, d.Y, stareAim) || !Collision.CanHitLine(Eyes, 1, 1, local.position, local.width, local.height))
            return;
        if (local.GetModPlayer<SubstitutionPlayer>().TakeBind(local.Center.X >= NPC.Center.X ? 1 : -1))
            return;
        local.GetModPlayer<JutsuStatusPlayer>().Fear(ExamBossRules.KillingIntentTicks);
        Main.NewText("杀气……身体动不了！", 220, 90, 110);
    }

    // The stare's warning: his eyes burn red and a red wedge fans out where he is looking, brighter as it nears.
    private void DrawStare(Vector2 screenPos)
    {
        if (stareTicks <= 0)
            return;
        float t = 1f - stareTicks / (float)ExamBossRules.KillingIntentWarnTicks;
        bool blink = stareTicks < 15 && stareTicks / 3 % 2 == 0;
        Microsoft.Xna.Framework.Graphics.Texture2D pixel = Terraria.GameContent.TextureAssets.MagicPixel.Value;
        Vector2 from = Eyes - screenPos;
        const int rays = 14;
        for (int k = -rays; k <= rays; k++)
        {
            float angle = stareAim + ExamBossRules.KillingIntentHalfAngle * k / rays;
            bool edge = System.Math.Abs(k) == rays;
            Color c = new Color(230, 30, 50) * ((edge ? 0.55f : 0.12f) + (edge ? 0.35f : 0.1f) * t) * (blink ? 1.6f : 1f);
            Main.EntitySpriteDraw(pixel, from, new Rectangle(0, 0, 1, 1), c, angle, new Vector2(0f, 0.5f),
                new Vector2(ExamBossRules.KillingIntentRangePx, edge ? 2f : 3f), Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
        }
        Main.EntitySpriteDraw(pixel, from - new Vector2(4f, 2f), new Rectangle(0, 0, 1, 1), new Color(255, 60, 60), 0f, Vector2.Zero,
            new Vector2(8f, 3f), Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
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
        }
        NPC.active = false;
        NPC.netUpdate = true;
    }

    public override void PostDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        DrawTechniqueFx(drawColor);
        DrawStare(screenPos);
        DrawNeck(spriteBatch, screenPos, drawColor);
    }

    // The violet flames on his fingertips before the Five Elements Seal, and the summoning formula spread on the ground
    // as he calls Manda (orochimaru-fx-seal-v1).
    private void DrawTechniqueFx(Color drawColor)
    {
        if (State == SealWindup && FxArt.Frame("FxSealFlame", (int)(Main.GameUpdateCount / 6), 3) is { } flame)
            FxArt.Draw(flame, NPC.Bottom + new Vector2(NPC.direction * 30f, -66f) * SpriteScale, Color.White, 0f, ArtScale, NPC.direction);
        if (summonPose > 0 && FxArt.Has("FxSummonCircle"))
        {
            float grow = Math.Min(1f, (40 - summonPose) / 12f);
            FxArt.Draw(FxArt.Get("FxSummonCircle"), NPC.Bottom + new Vector2(0f, -8f), BossSprites.Lit(drawColor, 0.6f), 0f,
                ArtScale * (0.4f + 0.6f * grow));
            if (summonPose == 1 && !Main.dedServ)
                for (int i = 0; i < 40; i++)
                    Dust.NewDustPerfect(NPC.Bottom + Main.rand.NextVector2Circular(90f, 30f), DustID.Smoke,
                        Main.rand.NextVector2Circular(3f, 3f) - new Vector2(0f, 2f), 60, Color.White, 2.2f);
        }
    }

    // Where the neck joins the head in the NeckHead art (orochimaru-moves-v11c: the left edge, rows 19 to 26).
    private static Vector2 NeckAttach(Microsoft.Xna.Framework.Graphics.Texture2D head) => new(0f, 22.5f);

    // The long neck as one smooth tube (user, 2026-10-02: flat pieces stacked like stairs looked odd): pieces close
    // together along an S-curve that is pinned at the collar and the head, each turned along the curve.
    private void DrawNeck(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        const string root = "ShinobiPrototype/Content/NPCs/Orochimaru_";
        if (NeckReach <= 0f || !ModContent.HasAsset(root + "NeckSegment") || !ModContent.HasAsset(root + "NeckHead"))
            return;
        var segment = ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(root + "NeckSegment").Value;
        var head = ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(root + "NeckHead").Value;
        Color light = BossSprites.Lit(drawColor);
        // The head is drawn centred on NeckHead (where it bites), so the neck ends half a head short of it.
        Vector2 from = NeckRoot;
        Vector2 aim = NeckHead - from;
        if (aim.Length() < 2f)
            return;
        Vector2 to = NeckHead - Vector2.Normalize(aim) * Math.Min(aim.Length() - 1f, head.Width / 2f * SpriteScale);
        float length = Vector2.Distance(from, to);
        Vector2 dir = (to - from) / length;
        Vector2 side = new(-dir.Y, dir.X);
        float sway = Math.Min(14f * SpriteScale, length * 0.12f);
        Vector2 At(float t) => from + dir * length * t +
            side * (float)(Math.Sin(t * MathHelper.TwoPi * 1.5f + Main.GameUpdateCount * 0.18f) * Math.Sin(t * MathHelper.Pi)) * sway;
        float step = segment.Width * SpriteScale * 0.35f;
        int steps = Math.Max(2, (int)(length / step));
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 at = At(t);
            Vector2 along = At(Math.Min(1f, t + 0.02f)) - At(Math.Max(0f, t - 0.02f));
            spriteBatch.Draw(segment, at - screenPos, null, light, (float)Math.Atan2(along.Y, along.X), segment.Size() / 2f, SpriteScale,
                Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
        }
        Vector2 end = At(1f) - At(0.97f);
        float angle = (float)Math.Atan2(end.Y, end.X);
        bool left = end.X < 0f;
        Vector2 attach = NeckAttach(head);
        if (left)
            attach.X = head.Width - attach.X;
        spriteBatch.Draw(head, to - screenPos, null, light, left ? angle - MathHelper.Pi : angle, attach, SpriteScale,
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
