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

    private const int RevealTicks = 70;
    private const int EmergeTicks = 60;

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
                BossSprites.TryDraw(spriteBatch, "Orochimaru", "Dash", 0, 2, PersonCanvas, bottom, NPC.direction,
                    new Color(170, 110, 230) * fade, screenPos, ArtScale);
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
                        JutsuHitbox.Spawn(NPC, JutsuKind.Bite, NPC.Center + new Vector2(NPC.direction * 30f, 0f), Vector2.Zero, 48, 48,
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
                        JutsuHitbox.Spawn(NPC, JutsuKind.Bite, Mark, Vector2.Zero, 44, 44, Dmg(ExamBossRules.NeckBiteDamage));
                    Enter(Recovery);
                }
                break;

            case SealWindup:
                // He closes in with the seal glowing on his fingertips, then presses it on.
                Face(target.Center.X);
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * 6f, 0.15f);
                if (!FxArt.Has("FxSealFlame_0"))
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
            FxArt.Draw(flame, NPC.Center + new Vector2(NPC.direction * 34f, -18f), Color.White, 0f, ArtScale, NPC.direction);
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
        if (State != NeckWindup || !ModContent.HasAsset(root + "NeckSegment") || !ModContent.HasAsset(root + "NeckHead"))
            return;
        var segment = ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(root + "NeckSegment").Value;
        var head = ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(root + "NeckHead").Value;
        Color light = BossSprites.Lit(drawColor);
        // From the collar out towards where the player stood, snaking a little on the way.
        Vector2 from = NPC.Top + new Vector2(NPC.direction * 9f, 6f);
        Vector2 to = Vector2.Lerp(from, Mark, Math.Min(1f, Timer / 40f));
        float length = Vector2.Distance(from, to);
        Vector2 dir = length > 0f ? (to - from) / length : Vector2.UnitX;
        Vector2 side = new(-dir.Y, dir.X);
        int steps = (int)(length / (6f * ArtScale));
        for (int i = 0; i < steps; i++)
        {
            float t = i / (float)Math.Max(1, steps);
            Vector2 at = from + dir * length * t + side * (float)Math.Sin(t * MathHelper.TwoPi * 1.5f + Main.GameUpdateCount * 0.2f) * 15f * t;
            spriteBatch.Draw(segment, at - screenPos, null, light, 0f, segment.Size() / 2f, ArtScale,
                Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
        }
        float angle = (float)Math.Atan2(dir.Y, dir.X);
        bool left = dir.X < 0f;
        spriteBatch.Draw(head, to - screenPos, null, light, left ? angle - MathHelper.Pi : angle, head.Size() / 2f, ArtScale,
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
