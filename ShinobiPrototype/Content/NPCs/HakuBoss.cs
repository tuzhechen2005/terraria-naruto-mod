using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.NPCs;

// ai[0] state, ai[1] state timer, ai[2] linked Zabuza whoAmI + 1, ai[3] 1 once Zabuza fell.
[AutoloadBossHead]
public sealed class HakuBoss : ModNPC
{
    private const int Approach = 0;
    private const int NeedleWindup = 1;
    private const int NeedleRecovery = 2;
    private const int MirrorWindup = 3;
    private const int MirrorDash = 4;
    private const int MirrorRecovery = 5;
    private const int FrenzyPause = 6;
    private const int CageForm = 7;
    private const int CageHop = 8;
    private const int CageStagger = 9;
    private const int CageEnd = 10;
    public const int EmergeState = 11;
    private const int FrenzyHop = 12;
    private const int FrenzyExposed = 13;
    private const int ThousandCast = 14;

    private Vector2 mirrorCenter;
    private int hopMirror = -1;
    private int hopCount;
    private int cageCooldown;
    private int thousandCooldown;
    private int thousandCasts;
    private bool frenzyStarted;
    private readonly int[] respawnTimers = new int[WaveDuoRules.FrenzyMirrorCount];

    public override string Texture => "ShinobiPrototype/Content/NPCs/HakuPoseAtlasV2";
    public override string BossHeadTexture => "ShinobiPrototype/Content/NPCs/HakuBoss_Head_Boss";
    public bool LastStand => NPC.ai[3] == 1f;
    private int State => (int)NPC.ai[0];
    private bool Frenzy => State is FrenzyHop or FrenzyExposed or ThousandCast;

    public Vector2 MirrorCenter => mirrorCenter;
    public bool InCage => State is CageForm or CageHop or CageStagger;
    public bool CageBoundaryActive => State is CageForm or CageHop;
    public bool MirrorsActive => State is CageForm or CageHop || Frenzy;
    private int MirrorCount => Frenzy ? WaveDuoRules.FrenzyMirrorCount : WaveDuoRules.CageMirrorCount;
    private float MirrorRadius => Frenzy ? WaveDuoRules.FrenzyRadius : WaveDuoRules.CageRadius;

    public Vector2 MirrorPosition(int slot)
    {
        float angle = WaveDuoRules.MirrorAngle(slot, MirrorCount);
        return mirrorCenter + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * MirrorRadius;
    }

    public bool MirrorGlowing(int slot) => slot == hopMirror && (State is CageHop or FrenzyHop) &&
        WaveDuoRules.PhaseInHop((int)NPC.ai[1] % WaveDuoRules.HopCycle(Frenzy), Frenzy) ==
            WaveDuoRules.HopPhase.Warn;

    public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 1;

    public override void SetDefaults()
    {
        NPC.width = WaveDuoRules.HakuBodyWidth;
        NPC.height = WaveDuoRules.HakuBodyHeight;
        NPC.damage = 0;
        NPC.defense = 4;
        NPC.lifeMax = WaveDuoRules.HakuMaxLife;
        NPC.knockBackResist = 0.12f;
        NPC.boss = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true; // Flies freely like vanilla flying bosses.
        NPC.aiStyle = -1;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.BossBar = ModContent.GetInstance<HakuBossBar>();
        Music = MusicID.Boss1;
    }

    public override void AI()
    {
        if (NPC.target < 0 || NPC.target >= Main.maxPlayers ||
            !Main.player[NPC.target].active || Main.player[NPC.target].dead)
            NPC.TargetClosest();
        Player target = Main.player[NPC.target];
        if (!target.active || target.dead)
        {
            NPC.EncourageDespawn(10);
            return;
        }

        NPC parent = LinkedZabuza();
        if (parent == null && !LastStand)
        {
            NPC.active = false;
            return;
        }
        NPC.ai[1]++;
        if (parent != null && !InCage)
            cageCooldown = Math.Min(WaveDuoRules.CageCooldownTicks, cageCooldown + 1);
        if (Math.Abs(NPC.velocity.X) > 2.2f)
            NPC.localAI[2] = 1f;
        else if (Math.Abs(NPC.velocity.X) < 0.8f)
            NPC.localAI[2] = 0f;
        NPC.frameCounter += Math.Abs(NPC.velocity.X) * 0.55f;
        NPC.direction = NPC.spriteDirection = target.Center.X >= NPC.Center.X ? 1 : -1;
        NPC.dontTakeDamage = false;
        NPC.alpha = 0;
        Lighting.AddLight(NPC.Center, LastStand ? 0.25f : 0.14f, 0.4f, 0.55f);

        if (State == EmergeState)
        {
            UpdateEmerge();
            return;
        }
        if (LastStand && !frenzyStarted)
        {
            frenzyStarted = true;
            Enter(FrenzyPause);
        }
        if (parent != null && !WaveDuoRules.HakuMayAttack((int)parent.ai[0]))
        {
            NPC.velocity *= 0.84f;
            ShowIceAura(2);
            return;
        }

        switch (State)
        {
            case Approach:
                MoveToFlank(target, parent);
                if (Main.netMode != NetmodeID.MultiplayerClient && parent != null &&
                    WaveDuoRules.CageMayStart(true, true, (int)parent.ai[0], cageCooldown))
                {
                    StartMirrors(target, WaveDuoRules.CageMirrorCount);
                    NPC.velocity *= 0.3f;
                    Enter(CageForm);
                    break;
                }
                if (NPC.ai[1] >= 52f &&
                    (parent == null || WaveDuoRules.HakuMayUseStrongAttack((int)parent.ai[0])))
                {
                    bool dash = (int)NPC.localAI[0] % 2 == 1 &&
                        Math.Abs(target.Center.X - NPC.Center.X) < 330f;
                    Enter(dash ? MirrorWindup : NeedleWindup);
                }
                break;

            case NeedleWindup:
                NPC.velocity *= 0.8f;
                ShowIceAura(2);
                if (NPC.ai[1] >= WaveDuoRules.HakuNeedleWindupTicks)
                {
                    if (parent != null && !WaveDuoRules.HakuMayUseStrongAttack((int)parent.ai[0]))
                    {
                        NPC.ai[1] = WaveDuoRules.HakuNeedleWindupTicks - 1;
                        break;
                    }
                    FireFan(target, 1, 9f, 18);
                    SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                    NPC.localAI[0]++;
                    Enter(NeedleRecovery);
                }
                break;

            case NeedleRecovery:
                NPC.velocity *= 0.82f;
                if (NPC.ai[1] >= WaveDuoRules.HakuNeedleRecoveryTicks)
                    Enter(Approach);
                break;

            case MirrorWindup:
                NPC.velocity *= 0.78f;
                ShowIceAura(3);
                if (NPC.ai[1] >= WaveDuoRules.HakuDashWindupTicks(false))
                {
                    if (parent != null && !WaveDuoRules.HakuMayUseStrongAttack((int)parent.ai[0]))
                    {
                        NPC.ai[1] = WaveDuoRules.HakuDashWindupTicks(false) - 1;
                        break;
                    }
                    StartMirrorDash(target);
                }
                break;

            case MirrorDash:
                ShowIceAura(1);
                if (NPC.ai[1] >= WaveDuoRules.HakuDashActiveTicks)
                {
                    NPC.velocity *= 0.3f;
                    NPC.localAI[0]++;
                    Enter(MirrorRecovery);
                }
                break;

            case MirrorRecovery:
                NPC.velocity *= 0.8f;
                if (NPC.ai[1] >= WaveDuoRules.HakuDashRecoveryTicks)
                    Enter(Approach);
                break;

            case CageForm:
                NPC.velocity *= 0.8f;
                NPC.dontTakeDamage = true;
                NPC.alpha = (int)MathHelper.Clamp(NPC.ai[1] / WaveDuoRules.CageFormTicks * 255f, 0f, 255f);
                ShowIceAura(3);
                if (NPC.ai[1] >= WaveDuoRules.CageFormTicks)
                {
                    hopMirror = -1;
                    hopCount = 0;
                    Enter(CageHop);
                }
                break;

            case CageHop:
            {
                bool[] alive = AliveMirrors();
                int broken = WaveDuoRules.CageMirrorCount - WaveDuoRules.AliveCount(alive);
                if (WaveDuoRules.CageBrokenEarly(broken))
                {
                    SoundEngine.PlaySound(SoundID.Shatter, NPC.Center);
                    Enter(CageStagger);
                    break;
                }
                if (WaveDuoRules.CageShouldEnd((int)NPC.ai[1], broken) || !UpdateHop(target, alive, false))
                    Enter(CageEnd);
                break;
            }

            case CageStagger:
                // Breaking the cage leaves Haku dazed and open to punishment.
                NPC.velocity = new Vector2(0f, (float)Math.Sin(NPC.ai[1] * 0.2f) * 0.4f);
                if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(3))
                    Dust.NewDustPerfect(NPC.Top + new Vector2(Main.rand.NextFloat(-10f, 10f), -4f),
                        DustID.IceTorch, new Vector2(0f, -1f), 60, Color.White, 1.1f).noGravity = true;
                if (NPC.ai[1] >= WaveDuoRules.CageStaggerTicks)
                    Enter(CageEnd);
                break;

            case CageEnd:
                NPC.velocity *= 0.85f;
                if (NPC.ai[1] >= WaveDuoRules.CageEndTicks)
                {
                    cageCooldown = 0;
                    hopMirror = -1;
                    Enter(Approach);
                }
                break;

            case FrenzyPause:
                NPC.velocity *= 0.75f;
                NPC.dontTakeDamage = true;
                if (NPC.ai[1] == 1f)
                {
                    SoundEngine.PlaySound(SoundID.Item27, NPC.Center);
                    BossLines.Say(NPC, "HakuFrenzy", new Color(175, 240, 255));
                    if (Main.netMode != NetmodeID.Server)
                        WaveOverlaySystem.Flash();
                }
                ShowIceAura(4);
                if (NPC.ai[1] >= WaveDuoRules.FrenzyPauseTicks)
                {
                    Array.Clear(respawnTimers);
                    thousandCooldown = 0;
                    hopMirror = -1;
                    hopCount = 0;
                    StartMirrors(target, WaveDuoRules.FrenzyMirrorCount, frenzy: true);
                    Enter(FrenzyHop);
                }
                break;

            case FrenzyHop:
            {
                FollowPlayerRing(target);
                bool[] alive = AliveMirrors();
                RespawnFrenzyMirrors(alive);
                thousandCooldown++;
                int cycleTick = (int)NPC.ai[1] % WaveDuoRules.FrenzyHopCycleTicks;
                if (cycleTick == 0 && WaveDuoRules.ThousandNeedleReady(NPC.life / (float)NPC.lifeMax,
                    thousandCooldown))
                {
                    Enter(ThousandCast);
                    break;
                }
                if (!UpdateHop(target, alive, true))
                    Enter(FrenzyExposed);
                break;
            }

            case FrenzyExposed:
            {
                // Every mirror is broken: Haku fights in the open until one reforms.
                FollowPlayerRing(target);
                MoveToFlank(target, null);
                bool[] alive = AliveMirrors();
                RespawnFrenzyMirrors(alive);
                thousandCooldown++;
                if ((int)NPC.ai[1] % 40 == 20)
                {
                    FireFan(target, 1, 10.5f, 21);
                    SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                }
                if (WaveDuoRules.AliveCount(alive) > 0 && (int)NPC.ai[1] % 40 == 39)
                {
                    hopMirror = -1;
                    Enter(FrenzyHop);
                }
                break;
            }

            case ThousandCast:
            {
                FollowPlayerRing(target);
                RespawnFrenzyMirrors(AliveMirrors());
                NPC.velocity *= 0.8f;
                ShowIceAura(3);
                if (NPC.ai[1] == 1f)
                    CastThousandNeedles(target);
                if (NPC.ai[1] >= WaveDuoRules.ThousandNeedleCastTicks)
                {
                    thousandCooldown = 0;
                    thousandCasts++;
                    hopMirror = -1;
                    Enter(FrenzyHop);
                }
                break;
            }
        }
    }

    // Emerging from a mirror beside Zabuza during his transition; untouchable until out.
    private void UpdateEmerge()
    {
        NPC.velocity = Vector2.Zero;
        NPC.dontTakeDamage = true;
        float t = NPC.ai[1];
        NPC.alpha = (int)MathHelper.Clamp(255f * (1f - (t - 20f) / 30f), 0f, 255f);
        if (t == 1f)
        {
            mirrorCenter = NPC.Center;
            SoundEngine.PlaySound(SoundID.Item27, NPC.Center);
        }
        if (t == WaveDuoRules.HakuEmergeLineTick)
            BossLines.Say(NPC, "HakuArrive", new Color(175, 240, 255));
        ShowIceAura(2);
        if (t >= WaveDuoRules.HakuEmergeTicks)
            Enter(Approach);
    }

    // Returns false when no mirror is left to hop into.
    private bool UpdateHop(Player target, bool[] alive, bool frenzy)
    {
        int cycle = WaveDuoRules.HopCycle(frenzy);
        int cycleTick = (int)NPC.ai[1] % cycle;
        // Pick a new mirror each cycle, or at once if the current one was shattered.
        if (cycleTick == 0 || hopMirror < 0 || !alive[hopMirror])
        {
            hopMirror = WaveDuoRules.NextMirror(hopMirror, alive, hopCount++);
            if (hopMirror < 0)
                return false;
        }
        // Stand just inside the chosen mirror, toward the ring center.
        Vector2 slot = MirrorPosition(hopMirror);
        NPC.Center = slot + Vector2.Normalize(mirrorCenter - slot + new Vector2(0.01f, 0f)) * 26f;
        NPC.velocity = Vector2.Zero;
        switch (WaveDuoRules.PhaseInHop(cycleTick, frenzy))
        {
            case WaveDuoRules.HopPhase.Warn:
                NPC.alpha = 255;
                NPC.dontTakeDamage = true;
                break;
            case WaveDuoRules.HopPhase.Exposed:
                if (cycleTick == WaveDuoRules.HopThrowTick(frenzy))
                {
                    FireFan(target, frenzy ? WaveDuoRules.FrenzyFanNeedles : WaveDuoRules.CageFanNeedles,
                        frenzy ? 10.5f : 9f, frenzy ? 21 : 18);
                    SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                }
                break;
            default:
                NPC.alpha = 200;
                NPC.dontTakeDamage = true;
                break;
        }
        return true;
    }

    private void StartMirrors(Player target, int count, bool frenzy = false)
    {
        mirrorCenter = target.Center;
        NPC.netUpdate = true;
        SoundEngine.PlaySound(SoundID.Item27, target.Center);
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;
        for (int slot = 0; slot < count; slot++)
            SpawnMirror(slot, frenzy);
    }

    private void SpawnMirror(int slot, bool frenzy)
    {
        float angle = WaveDuoRules.MirrorAngle(slot, frenzy ? WaveDuoRules.FrenzyMirrorCount
            : WaveDuoRules.CageMirrorCount);
        float radius = frenzy ? WaveDuoRules.FrenzyRadius : WaveDuoRules.CageRadius;
        Vector2 at = mirrorCenter + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
        int index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)at.X, (int)at.Y + 32,
            ModContent.NPCType<HakuIceMirror>(), ai0: NPC.whoAmI + 1, ai1: slot);
        if (Main.netMode == NetmodeID.Server && index >= 0 && index < Main.maxNPCs)
            NetMessage.SendData(MessageID.SyncNPC, number: index);
    }

    private bool[] AliveMirrors()
    {
        bool[] alive = new bool[MirrorCount];
        int type = ModContent.NPCType<HakuIceMirror>();
        foreach (NPC npc in Main.ActiveNPCs)
            if (npc.type == type && (int)npc.ai[0] == NPC.whoAmI + 1)
            {
                int slot = (int)npc.ai[1];
                if (slot >= 0 && slot < alive.Length)
                    alive[slot] = true;
            }
        return alive;
    }

    private void RespawnFrenzyMirrors(bool[] alive)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;
        for (int slot = 0; slot < alive.Length && slot < respawnTimers.Length; slot++)
        {
            if (alive[slot])
            {
                respawnTimers[slot] = 0;
                continue;
            }
            if (++respawnTimers[slot] >= WaveDuoRules.FrenzyMirrorRespawnTicks)
            {
                respawnTimers[slot] = 0;
                SpawnMirror(slot, true);
            }
        }
    }

    private void FollowPlayerRing(Player target)
    {
        mirrorCenter = Vector2.Lerp(mirrorCenter, target.Center, 0.03f);
        if ((int)NPC.ai[1] % 30 == 0)
            NPC.netUpdate = true;
    }

    private void CastThousandNeedles(Player target)
    {
        SoundEngine.PlaySound(SoundID.Item28, target.Center);
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;
        int gap = WaveDuoRules.ThousandNeedleGapStart(thousandCasts);
        for (int i = 0; i < WaveDuoRules.ThousandNeedleCount; i++)
        {
            if (WaveDuoRules.ThousandNeedleSkipped(i, gap))
                continue;
            float angle = MathHelper.TwoPi * i / WaveDuoRules.ThousandNeedleCount;
            Vector2 offset = angle.ToRotationVector2() * WaveDuoRules.ThousandNeedleRadius;
            Projectile.NewProjectile(NPC.GetSource_FromAI(), target.Center + offset,
                -Vector2.Normalize(offset) * 8f, ModContent.ProjectileType<HakuPrismShard>(),
                WaveDuoRules.SoftenedDamage(21), 0f, Main.myPlayer, 0f, NPC.whoAmI + 1,
                WaveDuoRules.ThousandNeedleWarnTicks);
        }
    }

    private NPC LinkedZabuza()
    {
        int index = (int)NPC.ai[2] - 1;
        if (index < 0 || index >= Main.maxNPCs)
            return null;
        NPC other = Main.npc[index];
        return other.active && other.type == ModContent.NPCType<ZabuzaBoss>() ? other : null;
    }

    private void MoveToFlank(Player target, NPC parent)
    {
        float side = parent != null
            ? (parent.Center.X < target.Center.X ? 1f : -1f)
            : (NPC.Center.X < target.Center.X ? -1f : 1f);
        Vector2 desired = target.Center + new Vector2(side * (LastStand ? 180f : 225f), -105f);
        Vector2 delta = desired - NPC.Center;
        float maxSpeed = LastStand ? 7.6f : 5.9f;
        Vector2 velocity = new(MathHelper.Clamp(delta.X * 0.055f, -maxSpeed, maxSpeed),
            MathHelper.Clamp(delta.Y * 0.045f, -maxSpeed * 0.8f, maxSpeed * 0.8f));
        NPC.velocity = Vector2.Lerp(NPC.velocity, velocity, 0.15f);
    }

    private void StartMirrorDash(Player target)
    {
        Vector2 direction = target.Center - NPC.Center;
        if (direction.LengthSquared() < 1f)
            direction = new Vector2(NPC.direction, 0f);
        direction.Normalize();
        NPC.velocity = direction * WaveDuoRules.HakuDashSpeed(false);
        if (Main.netMode != NetmodeID.MultiplayerClient)
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero,
                ModContent.ProjectileType<HakuDashHitbox>(),
                WaveDuoRules.SoftenedDamage(23), 0f,
                Main.myPlayer, NPC.whoAmI, NPC.direction);
        SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
        Enter(MirrorDash);
    }

    private void FireFan(Player target, int count, float speed, int damage)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;
        Vector2 direction = target.Center - NPC.Center;
        if (direction.LengthSquared() < 1f)
            direction = new Vector2(NPC.direction, 0f);
        direction.Normalize();
        for (int i = 0; i < count; i++)
        {
            float angle = (i - (count - 1) * 0.5f) * 0.14f;
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + direction * 18f,
                direction.RotatedBy(angle) * speed, ModContent.ProjectileType<HakuNeedle>(),
                WaveDuoRules.SoftenedDamage(damage), 0f, Main.myPlayer);
        }
    }

    private void ShowIceAura(int count)
    {
        if (Main.netMode == NetmodeID.Server || NPC.alpha >= 200)
            return;
        for (int i = 0; i < count; i++)
        {
            Vector2 point = NPC.Center + Main.rand.NextVector2Circular(24f, 34f);
            Dust.NewDustPerfect(point, DustID.IceTorch, new Vector2(0f, -1.2f), 45,
                LastStand ? new Color(225, 250, 255) : new Color(140, 230, 255),
                LastStand ? 1.4f : 1.1f).noGravity = true;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        bool emergeArt = State == EmergeState && BossSprites.Has("Haku_Emerge_0");
        if (State == EmergeState && !emergeArt)
            DrawEmergeMirror(spriteBatch, screenPos);
        if (State is MirrorWindup or MirrorDash)
            DrawIceMirrors(spriteBatch, screenPos);
        Color color = Color.Lerp(drawColor, Color.White, 0.52f);
        if (LastStand)
            color = Color.Lerp(color, new Color(185, 245, 255), 0.45f);
        if (State == CageStagger)
            color = Color.Lerp(color, new Color(255, 255, 255), 0.3f);
        color *= NPC.Opacity;

        (string action, int count, int frame) = FrameFor();
        if (BossSprites.TryDraw(spriteBatch, "Haku", action, frame, count,
            emergeArt ? BossSprites.HakuEmerge : BossSprites.Haku,
            NPC.Bottom, NPC.spriteDirection, color, screenPos))
            return false;

        // Legacy high-resolution atlas until the pixel frames are delivered.
        Texture2D atlas = ModContent.Request<Texture2D>(Texture).Value;
        int cell = atlas.Width / 2;
        int pose = action switch { "Throw" or "Hop" => 2, "Dash" => 3, "Move" => 1, _ => 0 };
        Rectangle source = new((pose % 2) * cell, (pose / 2) * cell, cell, cell);
        float footAnchor = pose switch { 0 => 614f, 1 => 600f, 2 => 574f, _ => 540f };
        spriteBatch.Draw(atlas, NPC.Bottom - screenPos, source, color,
            0f, new Vector2(cell * 0.5f, footAnchor), 0.125f,
            NPC.spriteDirection >= 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        return false;
    }

    private (string Action, int Count, int Frame) FrameFor()
    {
        float t = NPC.ai[1];
        return State switch
        {
            NeedleWindup => ("Throw", 4, BossSprites.Progress(t, WaveDuoRules.HakuNeedleWindupTicks + 6, 4)),
            MirrorWindup => ("Dash", 3, 0),
            MirrorDash => ("Dash", 3, 1 + BossSprites.Progress(t, WaveDuoRules.HakuDashActiveTicks, 2)),
            CageHop or FrenzyHop when HopExposed() => ("Throw", 4, BossSprites.Progress(
                t % WaveDuoRules.HopCycle(Frenzy) - WaveDuoRules.HopThrowTick(Frenzy) + 8, 16f, 4)),
            EmergeState => ("Emerge", 4, BossSprites.Progress(t, WaveDuoRules.HakuEmergeTicks, 4)),
            ThousandCast => ("Throw", 4, 3),
            _ when NPC.localAI[2] == 1f => ("Move", 4, BossSprites.Loop(6f, 4)),
            _ => ("Idle", 4, BossSprites.Loop(8f, 4))
        };
    }

    private bool HopExposed() => WaveDuoRules.PhaseInHop((int)NPC.ai[1] % WaveDuoRules.HopCycle(Frenzy),
        Frenzy) == WaveDuoRules.HopPhase.Exposed;

    private void DrawEmergeMirror(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        float t = NPC.ai[1];
        float fade = t < 20f ? t / 20f : t > 35f ? Math.Max(0f, 1f - (t - 35f) / 15f) : 1f;
        DrawMirrorAt(spriteBatch, mirrorCenter == Vector2.Zero ? NPC.Center : mirrorCenter,
            screenPos, new Color(160, 235, 255) * (0.85f * fade));
    }

    private void DrawIceMirrors(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        float pulse = 0.85f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 8f) * 0.08f;
        for (int side = -1; side <= 1; side += 2)
            DrawMirrorAt(spriteBatch, NPC.Center + new Vector2(side * 70f, -4f), screenPos,
                new Color(130, 220, 255) * (0.8f * pulse));
    }

    private static void DrawMirrorAt(SpriteBatch spriteBatch, Vector2 world, Vector2 screenPos, Color color)
    {
        Vector2 center = world - screenPos;
        center = new Vector2((float)Math.Round(center.X), (float)Math.Round(center.Y));
        if (ModContent.HasAsset("ShinobiPrototype/Content/NPCs/IceMirror_0"))
        {
            Texture2D art = ModContent.Request<Texture2D>("ShinobiPrototype/Content/NPCs/IceMirror_0").Value;
            spriteBatch.Draw(art, center, null, color, 0f, art.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            return;
        }
        Texture2D mirror = ModContent.Request<Texture2D>("ShinobiPrototype/Content/NPCs/HakuIceMirrorV2").Value;
        spriteBatch.Draw(mirror, center, null, color, 0f, mirror.Size() * 0.5f,
            64f / mirror.Height, SpriteEffects.None, 0f);
    }

    private void Enter(int state)
    {
        NPC.ai[0] = state;
        NPC.ai[1] = 0f;
        NPC.netUpdate = true;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(mirrorCenter.X);
        writer.Write(mirrorCenter.Y);
        writer.Write((sbyte)hopMirror);
        writer.Write(hopCount);
        writer.Write(cageCooldown);
        writer.Write(thousandCooldown);
        writer.Write(thousandCasts);
        writer.Write(frenzyStarted);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        mirrorCenter = new Vector2(reader.ReadSingle(), reader.ReadSingle());
        hopMirror = reader.ReadSByte();
        hopCount = reader.ReadInt32();
        cageCooldown = reader.ReadInt32();
        thousandCooldown = reader.ReadInt32();
        thousandCasts = reader.ReadInt32();
        frenzyStarted = reader.ReadBoolean();
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        for (int i = 0; i < (NPC.life <= 0 ? 24 : 7); i++)
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.IceTorch);
    }

    public override void OnKill()
    {
        NPC parent = LinkedZabuza();
        if (parent != null)
        {
            parent.ai[3] = 3f;
            parent.netUpdate = true;
        }
        else
            ZabuzaBoss.CompleteEncounter(NPC);
    }
}
