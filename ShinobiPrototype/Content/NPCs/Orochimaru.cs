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

    protected override float SpriteScale => 1f;

    private static bool Has(string action) => BossSprites.Has($"Orochimaru_{action}_0");

    private static int IdleFrames => BossSprites.Has("Orochimaru_Idle_5") ? 6 : 4;
    private static int WalkFrames => BossSprites.Has("Orochimaru_Walk_7") ? 8 : 4;

    private (string, int, int, bool) Or((string Action, int Frames, int Ticks, bool Loop) pose)
    {
        if (!Has(pose.Action))
            return ("Idle", IdleFrames, 10, true);
        // A technique opens on its in-between frame (orochimaru-tween-v1) for the first few ticks.
        if (Timer < InTicks && Has(pose.Action + "In"))
            return (pose.Action + "In", 1, InTicks, true);
        return pose;
    }

    private const int InTicks = 6;

    private int summonPose;

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        // Where he was a moment ago: the after-images of the snake dash.
        NPCID.Sets.TrailCacheLength[Type] = 6;
        NPCID.Sets.TrailingMode[Type] = 0;
    }

    // The snake dash leaves a fading violet trail of after-images (orochimaru effects, user 2026-10-01).
    public override bool PreDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (State == Dash && SpritePrefix != null)
            for (int i = NPC.oldPos.Length - 1; i >= 1; i--)
            {
                if (NPC.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 bottom = NPC.oldPos[i] + new Vector2(NPC.width / 2f, NPC.height);
                float fade = 0.5f * (1f - i / (float)NPC.oldPos.Length);
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
            HandsWindup => Or(("Hands", 3, 14, false)),
            DashWindup or Dash => Or(("Dash", 2, 6, true)),
            WindWindup => Or(("Wind", 2, 20, false)),
            NeckWindup => Or(("Neck", 1, 10, true)),
            SealWindup => Or(("Seal", 2, 12, false)),
            SwarmWindup => Or(("Hands", 3, 7, false)),
            VenomWindup or SwordWindup => Or(("Wind", 2, 12, false)),
            RainWindup => Or(("Summon", 2, 12, false)),
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
    // The collar of the neck frame (orochimaru-set-v11b: canvas (120, 46), feet at (112, 132), drawn one to one).
    private Vector2 NeckRoot => NPC.Bottom + new Vector2(NPC.direction * 8f, -86f);
    private float NeckReach => State != NeckWindup || Timer < NeckLaunch ? 0f : Math.Min(1f, (Timer - NeckLaunch) / NeckReachTicks);
    public Vector2 NeckHead => Vector2.Lerp(NeckRoot, Mark, NeckReach);

    // Sinking into the ground once he has had his say (ExamBoss draws him that many art pixels lower, cut at the ground).
    protected override float SinkPixels => State == Exit && Timer > ExitTalkTicks
        ? 100f * (Timer - ExitTalkTicks) / (ThresholdRetreatRules.ExitInvulnerableTicks - ExitTalkTicks)
        : 0f;

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
                if (Timer >= 24f)
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
                if (Timer >= NeckLaunch + NeckReachTicks + 12)
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
    private void KillingIntent()
    {
        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
        Player local = Main.LocalPlayer;
        if (Main.netMode == NetmodeID.Server || !local.active || local.dead || Vector2.Distance(local.Center, NPC.Center) > 60 * 16)
            return;
        local.GetModPlayer<JutsuStatusPlayer>().Fear(ExamBossRules.KillingIntentTicks);
        Main.NewText($"杀气……身体动不了！按【{ShinobiKeybinds.SubstitutionKeyName()}】用替身术挣脱！", 220, 90, 110);
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
        DrawNeck(spriteBatch, screenPos, drawColor);
    }

    // The violet flames on his fingertips before the Five Elements Seal, and the summoning formula spread on the ground
    // as he calls Manda (orochimaru-fx-seal-v1).
    private void DrawTechniqueFx(Color drawColor)
    {
        if (State == SealWindup && FxArt.Frame("FxSealFlame", (int)(Main.GameUpdateCount / 6), 3) is { } flame)
            FxArt.Draw(flame, NPC.Bottom + new Vector2(NPC.direction * 48f, -76f), Color.White, 0f, ArtScale, NPC.direction);
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

    private void DrawNeck(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        const string root = "ShinobiPrototype/Content/NPCs/Orochimaru_";
        if (NeckReach <= 0f || !ModContent.HasAsset(root + "NeckSegment") || !ModContent.HasAsset(root + "NeckHead"))
            return;
        var segment = ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(root + "NeckSegment").Value;
        var head = ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(root + "NeckHead").Value;
        Color light = BossSprites.Lit(drawColor);
        // From the collar out towards where the player stood, snaking a little on the way.
        Vector2 from = NeckRoot;
        Vector2 to = NeckHead;
        float length = Vector2.Distance(from, to);
        Vector2 dir = length > 0f ? (to - from) / length : Vector2.UnitX;
        Vector2 side = new(-dir.Y, dir.X);
        int steps = (int)(length / 9f);
        for (int i = 0; i < steps; i++)
        {
            float t = i / (float)Math.Max(1, steps);
            Vector2 at = from + dir * length * t + side * (float)Math.Sin(t * MathHelper.TwoPi * 1.5f + Main.GameUpdateCount * 0.2f) * 15f * t;
            spriteBatch.Draw(segment, at - screenPos, null, light, 0f, segment.Size() / 2f, SpriteScale,
                Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
        }
        float angle = (float)Math.Atan2(dir.Y, dir.X);
        bool left = dir.X < 0f;
        spriteBatch.Draw(head, to - screenPos, null, light, left ? angle - MathHelper.Pi : angle, head.Size() / 2f, SpriteScale,
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
