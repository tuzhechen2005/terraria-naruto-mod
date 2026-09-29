using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.NPCs;

// ai[0] state, ai[1] state timer, ai[2] locked facing, ai[3] encounter stage:
// 0 phase one, 1 transition started, 2 Haku present (or transition finished), 3 Haku fell.
[AutoloadBossHead]
public sealed class ZabuzaBoss : ModNPC
{
    private int demonHeadSlot = -1;
    private int hitFlashTicks;
    private bool frenzyStarted;
    private int comboSlashes;
    private bool openingChecked;
    private int dashTicks;
    private int dashCount;
    private int stuckTicks;
    private float lastX;

    // Summoned this close to the Wave Country bridge, he opens with the bridge line.
    private const float BridgeOpeningRangeTiles = 60f;
    public bool InMistPhase => ZabuzaCombatRules.InMistPhase(NPC.life, NPC.lifeMax);
    public bool LastStand => NPC.ai[3] == 3f;

    public override void Load() =>
        demonHeadSlot = Mod.AddBossHeadTexture(BossHeadTexture + "_SecondStage", -1);

    public override void BossHeadSlot(ref int index)
    {
        if (InMistPhase && demonHeadSlot >= 0)
            index = demonHeadSlot;
    }

    public override void ModifyTypeName(ref string typeName)
    {
        if (InMistPhase)
            typeName = Language.GetTextValue("Mods.ShinobiPrototype.NPCs.ZabuzaBoss.DemonName");
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 1;
        NPCID.Sets.TrailCacheLength[Type] = 6;
        NPCID.Sets.TrailingMode[Type] = 0;
    }

    public override void SetDefaults()
    {
        NPC.width = ZabuzaCombatRules.BodyWidth;
        NPC.height = ZabuzaCombatRules.BodyHeight;
        NPC.damage = 0; // Only the sword, charges and water projectiles deal damage.
        NPC.defense = 7;
        NPC.lifeMax = ZabuzaCombatRules.BossMaxLife;
        NPC.knockBackResist = 0.05f;
        NPC.boss = true;
        NPC.BossBar = ModContent.GetInstance<ZabuzaBossBar>();
        NPC.noGravity = false;
        NPC.noTileCollide = false;
        NPC.value = 0f;
        NPC.aiStyle = -1;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        Music = MusicID.Boss1;
    }

    public override void AI()
    {
        if (!openingChecked)
        {
            openingChecked = true;
            if (WaveBridgeWorld.DistanceToBridgeTiles(NPC.Center) <= BridgeOpeningRangeTiles)
                BossLines.Say(NPC, "BridgeOpening", new Color(160, 200, 230));
        }

        if (NPC.target < 0 || NPC.target >= Main.maxPlayers ||
            !Main.player[NPC.target].active || Main.player[NPC.target].dead)
            NPC.TargetClosest();
        Player target = Main.player[NPC.target];
        if (!target.active || target.dead)
        {
            NPC.EncourageDespawn(10);
            return;
        }
        if (hitFlashTicks > 0)
            hitFlashTicks--;

        if (InMistPhase && NPC.ai[3] == 0f)
        {
            NPC.ai[3] = 1f;
            NPC.localAI[1] = 0f;
            NPC.localAI[0] = ZabuzaCombatRules.TransitionOpeningAttack;
            comboSlashes = 0;
            Enter(ZabuzaCombatRules.MistTransition);
            SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
        }
        if (LastStand && !frenzyStarted && (int)NPC.ai[0] != ZabuzaCombatRules.MistTransition)
        {
            frenzyStarted = true;
            comboSlashes = 0;
            Enter(ZabuzaCombatRules.FrenzyAwaken);
        }

        NPC.ai[1]++;
        int state = (int)NPC.ai[0];
        NPC.noGravity = state is ZabuzaCombatRules.DashActive or ZabuzaCombatRules.DashChainActive or
            ZabuzaCombatRules.KunaiDash;
        // Dashes go straight through terrain, like the Eye of Cthulhu's; EndDash puts him back on open ground.
        NPC.noTileCollide = state is ZabuzaCombatRules.DashActive or ZabuzaCombatRules.DashChainActive;
        NPC.damage = state == ZabuzaCombatRules.KunaiDash
            ? WaveDuoRules.SoftenedDamage(WaveDuoRules.KunaiDashDamage)
            : NPC.noGravity ? WaveDuoRules.SoftenedDamage(InMistPhase ? 48 : 32) : 0;
        Lighting.AddLight(NPC.Center, InMistPhase ? (LastStand ? 0.6f : 0.46f) : 0.16f,
            InMistPhase ? (LastStand ? 0.18f : 0.12f) : 0.21f,
            InMistPhase ? (LastStand ? 0.78f : 0.64f) : 0.26f);
        NPC.dontTakeDamage = state is ZabuzaCombatRules.MistTransition or ZabuzaCombatRules.FrenzyAwaken ||
            state == ZabuzaCombatRules.BodyFlicker && NPC.ai[1] >= ZabuzaCombatRules.FlickerVanishTick &&
            NPC.ai[1] < ZabuzaCombatRules.FlickerReappearTick;
        NPC.alpha = 0;
        if (HandleMirrorCage())
            return;
        switch (state)
        {
            case ZabuzaCombatRules.Approach:
                MoveToward(target);
                if (CheckStuck(target))
                    break;
                if (NPC.ai[1] >= ZabuzaCombatRules.ApproachTicks(InMistPhase,
                    Math.Abs(target.Center.X - NPC.Center.X)))
                {
                    float gap = Math.Abs(target.Center.X - NPC.Center.X);
                    int next = LastStand
                        ? ZabuzaCombatRules.ChooseFrenzyAttack((int)NPC.localAI[0], gap)
                        : ZabuzaCombatRules.ChooseAttack(InMistPhase, (int)NPC.localAI[0], gap,
                            NPC.life / (float)NPC.lifeMax);
                    NPC.ai[2] = NPC.direction;
                    Enter(next);
                    if (next == ZabuzaCombatRules.RainWindup)
                    {
                        NPC.localAI[2] = target.Center.X;
                        NPC.localAI[3] = target.Center.Y;
                        NPC.netUpdate = true;
                    }
                    SoundEngine.PlaySound(next is ZabuzaCombatRules.SlashWindup or
                        ZabuzaCombatRules.SwordThrowWindup ? SoundID.Item71 : SoundID.Item8, NPC.Center);
                }
                break;

            case ZabuzaCombatRules.SlashWindup:
                NPC.direction = NPC.spriteDirection = NPC.ai[2] >= 0f ? 1 : -1;
                NPC.velocity.X *= 0.75f;
                if (InMistPhase && NPC.ai[1] > SlashWindupTicks - 12)
                    NPC.velocity.X = NPC.ai[2] * 2.2f;
                if (NPC.ai[1] >= SlashWindupTicks)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero,
                            ModContent.ProjectileType<ZabuzaSlash>(),
                            WaveDuoRules.SoftenedDamage(32), 0f,
                            Main.myPlayer, NPC.whoAmI, NPC.ai[2]);
                    SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                    NPC.localAI[0]++;
                    Enter(ZabuzaCombatRules.SlashRecovery);
                }
                break;

            case ZabuzaCombatRules.SlashRecovery:
                NPC.velocity.X = ZabuzaCombatRules.IsSlashActive((int)NPC.ai[0], NPC.ai[1])
                    ? NPC.ai[2] * (InMistPhase ? 4.2f : 3.4f)
                    : NPC.velocity.X * 0.7f;
                if (LastStand && comboSlashes < ZabuzaCombatRules.FrenzyComboSlashes - 1 &&
                    NPC.ai[1] >= ZabuzaCombatRules.SlashActiveTicks + 6)
                {
                    comboSlashes++;
                    NPC.ai[2] = target.Center.X >= NPC.Center.X ? 1f : -1f;
                    NPC.localAI[0]--; // A combo counts as one attack for pattern selection.
                    Enter(ZabuzaCombatRules.SlashWindup);
                    SoundEngine.PlaySound(SoundID.Item71, NPC.Center);
                    break;
                }
                if (NPC.ai[1] >= ZabuzaCombatRules.RecoveryTicks(ZabuzaCombatRules.SlashRecovery, InMistPhase))
                {
                    comboSlashes = 0;
                    Enter(ZabuzaCombatRules.Approach);
                }
                break;

            case ZabuzaCombatRules.WaterWindup:
                NPC.direction = NPC.spriteDirection = NPC.ai[2] >= 0f ? 1 : -1;
                NPC.velocity.X *= 0.7f;
                ShowWaterSeal();
                if (NPC.ai[1] >= ZabuzaCombatRules.WaterWindupTicks)
                {
                    Vector2 spawn = NPC.Center + new Vector2(NPC.ai[2] * 28f, -12f);
                    float slope = ZabuzaCombatRules.AimSlope(target.Center.X - spawn.X,
                        target.Center.Y - spawn.Y);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawn,
                            new Vector2(NPC.ai[2] * (InMistPhase ? 12f : 10f),
                                slope * (InMistPhase ? 12f : 10f)),
                            ModContent.ProjectileType<ZabuzaWaterWave>(),
                            WaveDuoRules.SoftenedDamage(26), 0f, Main.myPlayer,
                            InMistPhase ? 1f : 0f);
                    SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                    NPC.localAI[0]++;
                    Enter(ZabuzaCombatRules.WaterRecovery);
                }
                break;

            case ZabuzaCombatRules.WaterRecovery:
                NPC.velocity.X = InMistPhase && NPC.ai[1] <= 12f
                    ? -NPC.ai[2] * 2.5f
                    : NPC.velocity.X * 0.7f;
                if (NPC.ai[1] >= ZabuzaCombatRules.RecoveryTicks(ZabuzaCombatRules.WaterRecovery, InMistPhase))
                    Enter(ZabuzaCombatRules.Approach);
                break;

            case ZabuzaCombatRules.MistTransition:
                UpdateTransition(target);
                break;

            case ZabuzaCombatRules.FrenzyAwaken:
                NPC.velocity.X *= 0.8f;
                if (NPC.ai[1] == 1f)
                {
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                    BossLines.Say(NPC, "ZabuzaFrenzy", new Color(215, 125, 255));
                }
                ShowDemonAura();
                if (NPC.ai[1] >= ZabuzaCombatRules.FrenzyAwakenTicks)
                    Enter(ZabuzaCombatRules.Approach);
                break;

            case ZabuzaCombatRules.MistStep:
                NPC.direction = NPC.spriteDirection = NPC.ai[2] >= 0f ? 1 : -1;
                NPC.alpha = (int)Math.Min(145f, NPC.ai[1] * 9f);
                float stepSpeed = NPC.ai[1] < 20f ? 7.4f : 4.2f;
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.ai[2] * stepSpeed, 0.2f);
                if (NPC.ai[1] == 3f && NPC.collideY)
                {
                    NPC.velocity.Y = -9.4f;
                    NPC.netUpdate = true;
                }
                if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(2))
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Cloud,
                        newColor: new Color(190, 225, 235));
                if (NPC.ai[1] >= ZabuzaCombatRules.MistStepTicks)
                {
                    NPC.ai[2] = target.Center.X >= NPC.Center.X ? 1f : -1f;
                    Enter(Math.Abs(target.Center.X - NPC.Center.X) <= 145f
                        ? ZabuzaCombatRules.SlashWindup : ZabuzaCombatRules.WaterWindup);
                }
                break;

            case ZabuzaCombatRules.DragonWindup:
                NPC.direction = NPC.spriteDirection = NPC.ai[2] >= 0f ? 1 : -1;
                NPC.velocity.X *= 0.7f;
                ShowDragonWindup(target);
                if (NPC.ai[1] >= ZabuzaCombatRules.DragonWindupTicks)
                {
                    Vector2 spawn = NPC.Center + new Vector2(NPC.ai[2] * 32f, -12f);
                    Vector2 aim = new(NPC.ai[2],
                        ZabuzaCombatRules.AimSlope(target.Center.X - spawn.X, target.Center.Y - spawn.Y));
                    aim.Normalize();
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawn, aim * 9f,
                            ModContent.ProjectileType<ZabuzaWaterDragon>(),
                            WaveDuoRules.SoftenedDamage(34), 0f, Main.myPlayer,
                            InMistPhase ? 1f : 0f);
                    SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                    NPC.localAI[0]++;
                    Enter(ZabuzaCombatRules.DragonRecovery);
                }
                break;

            case ZabuzaCombatRules.DragonRecovery:
                NPC.velocity.X *= 0.7f;
                if (NPC.ai[1] >= ZabuzaCombatRules.RecoveryTicks(ZabuzaCombatRules.DragonRecovery, InMistPhase))
                    Enter(ZabuzaCombatRules.Approach);
                break;

            case ZabuzaCombatRules.FanWindup:
                NPC.direction = NPC.spriteDirection = NPC.ai[2] >= 0f ? 1 : -1;
                NPC.velocity.X *= 0.78f;
                ShowWaterSeal();
                if (NPC.ai[1] >= ZabuzaCombatRules.FanWindupTicks)
                {
                    FireFan(target);
                    SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                    NPC.localAI[0]++;
                    Enter(ZabuzaCombatRules.FanRecovery);
                }
                break;

            case ZabuzaCombatRules.FanRecovery:
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, -NPC.ai[2] * 2.8f, 0.12f);
                if (NPC.ai[1] >= ZabuzaCombatRules.RecoveryTicks(ZabuzaCombatRules.FanRecovery, InMistPhase))
                    Enter(ZabuzaCombatRules.Approach);
                break;

            case ZabuzaCombatRules.RainWindup:
                NPC.velocity.X *= 0.75f;
                ShowRainTelegraph();
                if (ZabuzaCombatRules.IsRainVolley(NPC.ai[1]))
                {
                    FireRainVolley();
                    SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                }
                if (NPC.ai[1] >= ZabuzaCombatRules.RainWindupTicks)
                {
                    NPC.localAI[0]++;
                    Enter(ZabuzaCombatRules.RainRecovery);
                }
                break;

            case ZabuzaCombatRules.RainRecovery:
                NPC.velocity.X *= 0.82f;
                if (NPC.ai[1] >= ZabuzaCombatRules.RecoveryTicks(ZabuzaCombatRules.RainRecovery, InMistPhase))
                    Enter(ZabuzaCombatRules.Approach);
                break;

            case ZabuzaCombatRules.SpiralWindup:
                NPC.velocity.X *= 0.76f;
                ShowSpiralTelegraph();
                if (ZabuzaCombatRules.IsSpiralVolley(NPC.ai[1]))
                    FireSpiralVolley();
                if (NPC.ai[1] >= ZabuzaCombatRules.SpiralLastVolleyTick + 8)
                {
                    NPC.localAI[0]++;
                    Enter(ZabuzaCombatRules.SpiralRecovery);
                }
                break;

            case ZabuzaCombatRules.SpiralRecovery:
                NPC.velocity.X *= 0.8f;
                if (NPC.ai[1] >= ZabuzaCombatRules.RecoveryTicks(ZabuzaCombatRules.SpiralRecovery, InMistPhase))
                    Enter(ZabuzaCombatRules.Approach);
                break;

            case ZabuzaCombatRules.DashWindup:
                NPC.direction = NPC.spriteDirection = NPC.ai[2] >= 0f ? 1 : -1;
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, -NPC.ai[2] * 2.2f, 0.11f);
                ShowDashCharge();
                if (NPC.ai[1] >= ZabuzaCombatRules.DashWindupTicks(InMistPhase))
                    StartDash(target, ZabuzaCombatRules.DashActive);
                break;

            case ZabuzaCombatRules.DashActive:
            case ZabuzaCombatRules.DashChainActive:
                if (NPC.ai[1] >= dashTicks)
                {
                    int chain = LastStand ? ZabuzaCombatRules.FrenzyDashChain : InMistPhase ? 2 : 1;
                    if (dashCount < chain)
                        Enter(ZabuzaCombatRules.DashReaim);
                    else
                        EndDash();
                }
                break;

            case ZabuzaCombatRules.DashReaim:
                if (NPC.ai[1] == 1f)
                    PushOutOfTiles();
                NPC.velocity *= 0.78f;
                NPC.ai[2] = target.Center.X >= NPC.Center.X ? 1f : -1f;
                NPC.direction = NPC.spriteDirection = (int)NPC.ai[2];
                ShowDashCharge();
                if (NPC.ai[1] >= ZabuzaCombatRules.DashReaimTicks)
                    StartDash(target, ZabuzaCombatRules.DashChainActive);
                break;

            case ZabuzaCombatRules.BodyFlicker:
                UpdateBodyFlicker(target);
                break;

            case ZabuzaCombatRules.DashRecovery:
                NPC.velocity.X *= 0.82f;
                if (NPC.ai[1] >= ZabuzaCombatRules.RecoveryTicks(ZabuzaCombatRules.DashRecovery, InMistPhase))
                    Enter(ZabuzaCombatRules.Approach);
                break;

            case ZabuzaCombatRules.SwordThrowWindup:
                NPC.direction = NPC.spriteDirection = NPC.ai[2] >= 0f ? 1 : -1;
                NPC.velocity.X *= 0.72f;
                ShowDashCharge();
                if (NPC.ai[1] >= ZabuzaCombatRules.SwordThrowWindupTicks)
                {
                    NPC.ai[2] = target.Center.X >= NPC.Center.X ? 1f : -1f;
                    NPC.direction = NPC.spriteDirection = (int)NPC.ai[2];
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 spawn = NPC.Center + new Vector2(NPC.ai[2] * 30f, -8f);
                        float slope = ZabuzaCombatRules.AimSlope(target.Center.X - spawn.X,
                            target.Center.Y - spawn.Y) * 0.5f;
                        Vector2 velocity = Vector2.Normalize(new Vector2(NPC.ai[2], slope)) *
                            ZabuzaCombatRules.SwordThrowSpeed;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawn, velocity,
                            ModContent.ProjectileType<ZabuzaThrownSword>(),
                            WaveDuoRules.SoftenedDamage(WaveDuoRules.ThrownSwordDamage), 0f,
                            Main.myPlayer, NPC.whoAmI);
                    }
                    SoundEngine.PlaySound(SoundID.Item7, NPC.Center);
                    NPC.localAI[0]++;
                    Enter(ZabuzaCombatRules.SwordThrowRelease);
                }
                break;

            case ZabuzaCombatRules.SwordThrowRelease:
                NPC.velocity.X *= 0.8f;
                if (NPC.ai[1] >= ZabuzaCombatRules.SwordThrowReleaseTicks)
                    Enter(ZabuzaCombatRules.KunaiWindup);
                break;

            case ZabuzaCombatRules.KunaiWindup:
                NPC.ai[2] = target.Center.X >= NPC.Center.X ? 1f : -1f;
                NPC.direction = NPC.spriteDirection = (int)NPC.ai[2];
                NPC.velocity.X *= 0.7f;
                if (!SwordInFlight() && NPC.ai[1] > 2f)
                {
                    Enter(ZabuzaCombatRules.SwordCatch);
                    break;
                }
                if (NPC.ai[1] >= ZabuzaCombatRules.KunaiWindupTicks)
                {
                    float slope = MathHelper.Clamp((target.Center.Y - NPC.Center.Y) /
                        Math.Max(Math.Abs(target.Center.X - NPC.Center.X), 90f), -0.25f, 0.25f);
                    NPC.velocity = Vector2.Normalize(new Vector2(NPC.ai[2], slope)) *
                        ZabuzaCombatRules.KunaiDashSpeed;
                    if (NPC.collideY && NPC.velocity.Y > -1.5f)
                        NPC.velocity.Y = -1.5f;
                    NPC.netUpdate = true;
                    SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                    Enter(ZabuzaCombatRules.KunaiDash);
                }
                break;

            case ZabuzaCombatRules.KunaiDash:
                if (ZabuzaCombatRules.DashHitWall(NPC.ai[1], NPC.collideX, NPC.collideY) ||
                    NPC.ai[1] >= ZabuzaCombatRules.KunaiDashTicks)
                {
                    NPC.velocity *= 0.35f;
                    Enter(SwordInFlight() ? ZabuzaCombatRules.KunaiWindup : ZabuzaCombatRules.SwordCatch);
                }
                break;

            case ZabuzaCombatRules.SwordCatch:
                NPC.velocity.X *= 0.75f;
                if (NPC.ai[1] == 1f)
                    SoundEngine.PlaySound(SoundID.Item37, NPC.Center);
                if (NPC.ai[1] >= ZabuzaCombatRules.SwordCatchTicks)
                    Enter(ZabuzaCombatRules.Approach);
                break;
        }

        if (Math.Abs(NPC.velocity.X) > 0.8f && Math.Abs(NPC.velocity.Y) < 0.8f)
            NPC.frameCounter = (NPC.frameCounter + Math.Abs(NPC.velocity.X)) % 88d;

        if (InMistPhase && (int)NPC.ai[0] != ZabuzaCombatRules.MistTransition)
            ShowDemonAura();
    }

    // Zabuza stands on water, as in the original fight by the bridge, so he never sinks into the sea.
    // AI sets noGravity again every tick, so this only holds him up for the current movement step.
    public override void PostAI()
    {
        if (NPC.velocity.Y < 0f)
            return;
        int x = (int)(NPC.Center.X / 16f);
        int y = (int)((NPC.Bottom.Y + 1f) / 16f);
        if (!WorldGen.InWorld(x, y, 2))
            return;
        Tile feet = Main.tile[x, y];
        if (feet.LiquidAmount == 0 || feet.LiquidType != LiquidID.Water ||
            feet.HasTile && Main.tileSolid[feet.TileType])
            return;

        int top = y;
        while (top > 1 && Main.tile[x, top - 1].LiquidAmount > 0 && Main.tile[x, top - 1].LiquidType == LiquidID.Water)
            top--;
        float surface = top * 16f + (255 - Main.tile[x, top].LiquidAmount) / 255f * 16f;
        if (NPC.Bottom.Y < surface - 2f)
            return;

        NPC.position.Y = surface - NPC.height;
        NPC.velocity.Y = 0f;
        NPC.noGravity = true;
    }

    private int SlashWindupTicks => LastStand
        ? ZabuzaCombatRules.FrenzySlashWindupTicks : ZabuzaCombatRules.SlashWindupTicks;

    // Kneel in the mist, Haku steps out of a mirror, then the demon roars.
    private void UpdateTransition(Player target)
    {
        float tick = NPC.ai[1];
        if (tick == 1f)
        {
            float away = NPC.Center.X >= target.Center.X ? 1f : -1f;
            NPC.velocity = new Vector2(away * 6f, -4f);
            NPC.direction = NPC.spriteDirection = -(int)away;
            NPC.netUpdate = true;
        }
        else if (tick < ZabuzaCombatRules.TransitionBurstTick)
            NPC.velocity.X *= NPC.collideY ? 0.82f : 0.97f;
        else
            NPC.velocity.X *= 0.64f;
        ShowDemonTransition();

        if (tick >= ZabuzaCombatRules.TransitionHakuTick && NPC.ai[3] == 1f &&
            Main.netMode != NetmodeID.MultiplayerClient)
        {
            if (SpawnHaku() || tick >= ZabuzaCombatRules.TransitionHakuDeadline)
            {
                // Without a free NPC slot the fight continues solo rather than stalling.
                NPC.ai[3] = 2f;
                NPC.netUpdate = true;
            }
        }
        if (tick == ZabuzaCombatRules.TransitionBurstTick)
        {
            if (NPC.collideY && NPC.velocity.Y == 0f)
                NPC.velocity.Y = -5.5f;
            NPC.direction = NPC.spriteDirection = target.Center.X >= NPC.Center.X ? 1 : -1;
            NPC.netUpdate = true;
            SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
        }
        if (tick >= ZabuzaCombatRules.MistTransitionTicks && NPC.ai[3] >= 2f)
        {
            NPC.dontTakeDamage = false;
            Enter(ZabuzaCombatRules.Approach);
        }
    }

    public override bool CheckDead()
    {
        if (ZabuzaCombatRules.MayDie(NPC.ai[3]))
            return true;
        // A single huge hit cannot skip the transition or Haku's entrance.
        NPC.life = ZabuzaCombatRules.PhaseOneFloor(NPC.lifeMax);
        NPC.dontTakeDamage = true;
        NPC.netUpdate = true;
        return false;
    }

    public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
    {
        if (LastStand)
            modifiers.FinalDamage *= WaveDuoRules.ZabuzaFrenzyDamageTakenMultiplier;
    }

    private bool SwordInFlight()
    {
        int type = ModContent.ProjectileType<ZabuzaThrownSword>();
        foreach (Projectile projectile in Main.ActiveProjectiles)
            if (projectile.type == type && (int)projectile.ai[0] == NPC.whoAmI)
                return true;
        return false;
    }

    private void ShowDemonAura()
    {
        if (Main.netMode == NetmodeID.Server || (!LastStand && !Main.rand.NextBool(2)))
            return;
        for (int i = 0; i < (LastStand ? 4 : 2); i++)
        {
            float side = i % 2 == 0 ? -1f : 1f;
            Vector2 point = NPC.Bottom + new Vector2(
                side * (NPC.width * 0.5f + Main.rand.NextFloat(3f, 12f)),
                -Main.rand.NextFloat(10f, NPC.height * 0.9f));
            Vector2 velocity = new(-NPC.velocity.X * 0.12f + side * 0.3f,
                -Main.rand.NextFloat(1.3f, 2.5f));
            Dust.NewDustPerfect(point, DustID.Shadowflame, velocity, 45,
                LastStand ? new Color(235, 120, 255) : new Color(170, 80, 255),
                Main.rand.NextFloat(LastStand ? 1.4f : 1.05f,
                    LastStand ? 1.85f : 1.4f)).noGravity = true;
        }
    }

    private void ShowDemonTransition()
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        float tick = NPC.ai[1];
        if (tick < ZabuzaCombatRules.TransitionBurstTick)
        {
            // Mist rolling in around the kneeling swordsman, a thin purple ember inside it.
            if ((int)tick % 2 == 0)
                Dust.NewDustPerfect(NPC.Bottom + new Vector2(Main.rand.NextFloat(-90f, 90f), -4f),
                    DustID.Cloud, new Vector2(Main.rand.NextFloat(-0.6f, 0.6f), -0.4f), 120,
                    new Color(200, 220, 230), 1.6f).noGravity = true;
            if (tick > ZabuzaCombatRules.TransitionHakuTick && (int)tick % 3 == 0)
            {
                float radius = MathHelper.Lerp(70f, 26f,
                    (tick - ZabuzaCombatRules.TransitionHakuTick) /
                    (ZabuzaCombatRules.TransitionBurstTick - ZabuzaCombatRules.TransitionHakuTick));
                Vector2 rim = NPC.Center + Main.rand.NextVector2CircularEdge(radius, radius * 0.8f);
                Dust.NewDustPerfect(rim, DustID.Shadowflame, Vector2.Normalize(NPC.Center - rim) * 2.2f,
                    40, new Color(170, 80, 255), 1.2f).noGravity = true;
            }
            return;
        }
        if (tick == ZabuzaCombatRules.TransitionBurstTick)
        {
            for (int i = 0; i < 36; i++)
            {
                Vector2 velocity = (MathHelper.TwoPi * i / 36f).ToRotationVector2() * 5.6f;
                Dust.NewDustPerfect(NPC.Center, DustID.Shadowflame, velocity, 20,
                    new Color(215, 125, 255), 1.8f).noGravity = true;
            }
        }
        else if ((int)tick % 3 == 0)
        {
            Vector2 point = NPC.Center + Main.rand.NextVector2Circular(34f, 40f);
            Dust.NewDustPerfect(point, DustID.Shadowflame,
                new Vector2(0f, -2f), 30, new Color(170, 80, 255), 1.35f).noGravity = true;
        }
    }

    private void ShowWaterSeal()
    {
        if (Main.netMode == NetmodeID.Server || !Main.rand.NextBool(2))
            return;
        Vector2 hands = NPC.Center + new Vector2(NPC.direction * 10f, -12f);
        Dust.NewDustPerfect(hands + Main.rand.NextVector2Circular(9f, 9f),
            DustID.Water, Vector2.Zero, 80, new Color(110, 210, 240), 1.1f).noGravity = true;
    }

    private void ShowDragonWindup(Player target)
    {
        ShowWaterSeal();
        if (Main.netMode == NetmodeID.Server || NPC.ai[1] < 28f || NPC.ai[1] % 4f != 0f)
            return;
        float slope = ZabuzaCombatRules.AimSlope(target.Center.X - NPC.Center.X,
            target.Center.Y - NPC.Center.Y);
        float distance = 32f + (NPC.ai[1] - 28f) * 1.4f;
        Vector2 gathering = NPC.Center + new Vector2(NPC.ai[2] * distance,
            -12f + slope * distance);
        for (int i = 0; i < 3; i++)
            Dust.NewDustPerfect(gathering + Main.rand.NextVector2Circular(8f, 8f),
                DustID.Water, Vector2.Zero, 40, new Color(110, 210, 240), 1.4f).noGravity = true;
    }

    private void FireFan(Player target)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;
        Vector2 spawn = NPC.Center + new Vector2(NPC.ai[2] * 34f, -16f);
        float slope = ZabuzaCombatRules.AimSlope(target.Center.X - spawn.X,
            target.Center.Y - spawn.Y);
        Vector2 aim = new(NPC.ai[2], slope);
        aim.Normalize();
        float baseAngle = aim.ToRotation();
        for (int i = -2; i <= 2; i++)
        {
            Vector2 velocity = (baseAngle + i * 0.17f).ToRotationVector2() * 8.5f;
            Projectile.NewProjectile(NPC.GetSource_FromAI(), spawn, velocity,
                ModContent.ProjectileType<ZabuzaWaterNeedle>(),
                WaveDuoRules.SoftenedDamage(20), 0f, Main.myPlayer,
                InMistPhase ? 1f : 0f);
        }
    }

    private void ShowRainTelegraph()
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        float elapsed = NPC.ai[1];
        for (int volley = 0; volley < ZabuzaCombatRules.RainVolleyCount; volley++)
        {
            int fireTick = ZabuzaCombatRules.RainFirstVolleyTick +
                volley * ZabuzaCombatRules.RainIntervalTicks;
            if (elapsed < fireTick - 24 || elapsed >= fireTick || elapsed % 4f != 0f)
                continue;
            for (int column = 0; column < 4; column++)
            {
                float x = NPC.localAI[2] + ZabuzaCombatRules.RainColumnOffset(column) +
                    ZabuzaCombatRules.RainVolleyShift(volley);
                Vector2 point = FindRainSpawn(x, NPC.localAI[3]);
                if (point == Vector2.Zero)
                    continue;
                for (int dot = 0; dot < 3; dot++)
                    Dust.NewDustPerfect(point + new Vector2(0f, dot * 25f), DustID.Water,
                        new Vector2(0f, 1f), 50, new Color(95, 225, 255), 1.25f).noGravity = true;
            }
        }
    }

    private void FireRainVolley()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;
        int volley = ((int)NPC.ai[1] - ZabuzaCombatRules.RainFirstVolleyTick) /
            ZabuzaCombatRules.RainIntervalTicks;
        for (int column = 0; column < 4; column++)
        {
            float x = NPC.localAI[2] + ZabuzaCombatRules.RainColumnOffset(column) +
                ZabuzaCombatRules.RainVolleyShift(volley);
            Vector2 spawn = FindRainSpawn(x, NPC.localAI[3]);
            if (spawn == Vector2.Zero)
                continue;
            Projectile.NewProjectile(NPC.GetSource_FromAI(), spawn,
                new Vector2((volley - 1) * 0.35f, 9.5f),
                ModContent.ProjectileType<ZabuzaWaterNeedle>(),
                WaveDuoRules.SoftenedDamage(22), 0f, Main.myPlayer,
                InMistPhase ? 1f : 0f);
        }
    }

    private static Vector2 FindRainSpawn(float x, float targetY)
    {
        if (x < 32f || x > Main.maxTilesX * 16f - 32f ||
            targetY < 128f || targetY > Main.maxTilesY * 16f - 32f)
            return Vector2.Zero;
        Vector2 result = Vector2.Zero;
        // Work upward from the combat space, stopping below a ceiling.
        for (int offset = 72; offset <= 232; offset += 16)
        {
            Vector2 candidate = new(x, targetY - offset);
            if (Collision.SolidCollision(candidate - new Vector2(6f, 3f), 12, 6))
                break;
            result = candidate;
        }
        return result;
    }

    private void ShowSpiralTelegraph()
    {
        if (Main.netMode == NetmodeID.Server || NPC.ai[1] >= ZabuzaCombatRules.SpiralFirstVolleyTick ||
            NPC.ai[1] % 3f != 0f)
            return;
        for (int i = 0; i < 3; i++)
        {
            float angle = NPC.ai[1] * 0.08f + i * MathHelper.TwoPi / 3f;
            Vector2 point = NPC.Center + angle.ToRotationVector2() * 44f;
            Dust.NewDustPerfect(point, DustID.Water, angle.ToRotationVector2() * 0.6f,
                70, new Color(95, 225, 255), 1.25f).noGravity = true;
        }
    }

    private void FireSpiralVolley()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;
        int volley = ((int)NPC.ai[1] - ZabuzaCombatRules.SpiralFirstVolleyTick) /
            ZabuzaCombatRules.SpiralIntervalTicks;
        for (int arm = 0; arm < 3; arm++)
        {
            float angle = NPC.ai[2] >= 0f ? 0f : MathHelper.Pi;
            angle += volley * 0.23f + arm * MathHelper.TwoPi / 3f;
            Vector2 direction = angle.ToRotationVector2();
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + direction * 42f,
                direction * 7.2f, ModContent.ProjectileType<ZabuzaWaterNeedle>(),
                WaveDuoRules.SoftenedDamage(19), 0f, Main.myPlayer,
                InMistPhase ? 1f : 0f);
        }
        if (volley % 2 == 0)
            SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
    }

    // Aims where the player will be, in any direction up to about 60 degrees off level, and runs long enough to
    // reach them and overshoot.
    private void StartDash(Player target, int activeState)
    {
        if (activeState == ZabuzaCombatRules.DashActive)
            dashCount = 0;
        dashCount++;
        float speed = ZabuzaCombatRules.DashSpeed(InMistPhase, NPC.life / (float)NPC.lifeMax) *
                      (LastStand ? WaveDuoRules.ZabuzaFrenzyDashMultiplier : 1f);
        Vector2 toTarget = target.Center - NPC.Center;
        (float x, float y) = ZabuzaCombatRules.DashAim(toTarget.X, toTarget.Y, target.velocity.X, target.velocity.Y, speed);
        NPC.velocity = new Vector2(x, y) * speed;
        dashTicks = ZabuzaCombatRules.DashActiveTicksFor(toTarget.Length(), speed);
        NPC.ai[2] = x >= 0f ? 1f : -1f;
        NPC.direction = NPC.spriteDirection = (int)NPC.ai[2];
        NPC.noGravity = true;
        NPC.netUpdate = true;
        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
        Enter(activeState);
    }

    private void EndDash()
    {
        NPC.noGravity = false;
        NPC.noTileCollide = false;
        PushOutOfTiles();
        NPC.damage = 0;
        NPC.velocity *= 0.4f;
        NPC.localAI[0]++;
        Enter(ZabuzaCombatRules.DashRecovery);
    }

    // After passing through terrain, step out to the nearest spot where the body fits.
    private void PushOutOfTiles()
    {
        if (!Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
            return;
        for (int radius = 1; radius <= 14; radius++)
            foreach (Vector2 step in new[] { new Vector2(0, -1), new Vector2(-1, 0), new Vector2(1, 0),
                         new Vector2(-1, -1), new Vector2(1, -1), new Vector2(0, 1) })
            {
                Vector2 candidate = NPC.position + step * radius * 16f;
                if (!Collision.SolidCollision(candidate, NPC.width, NPC.height))
                {
                    NPC.position = candidate;
                    NPC.netUpdate = true;
                    return;
                }
            }
    }

    // Stuck against terrain (or left far behind): Body Flicker. Only the server (or single player) decides.
    private bool CheckStuck(Player target)
    {
        bool wantsToMove = Math.Abs(target.Center.X - NPC.Center.X) > 90f;
        bool moved = Math.Abs(NPC.position.X - lastX) >= ZabuzaCombatRules.StuckProgressPerTick;
        lastX = NPC.position.X;
        stuckTicks = wantsToMove && !moved ? stuckTicks + 1 : Math.Max(0, stuckTicks - 2);
        if (Main.netMode == NetmodeID.MultiplayerClient ||
            !ZabuzaCombatRules.ShouldFlicker(stuckTicks, NPC.Distance(target.Center) / 16f))
            return false;
        stuckTicks = 0;
        Enter(ZabuzaCombatRules.BodyFlicker);
        return true;
    }

    // Body Flicker (瞬身): dissolve into mist, reappear on open ground six to ten tiles behind the player, fade in.
    private void UpdateBodyFlicker(Player target)
    {
        float tick = NPC.ai[1];
        NPC.velocity.X *= 0.6f;
        if (tick < ZabuzaCombatRules.FlickerVanishTick)
            NPC.alpha = (int)(255 * tick / ZabuzaCombatRules.FlickerVanishTick);
        else if (tick < ZabuzaCombatRules.FlickerReappearTick)
            NPC.alpha = 255;
        else
            NPC.alpha = (int)(255 * (1f - (tick - ZabuzaCombatRules.FlickerReappearTick) /
                (ZabuzaCombatRules.BodyFlickerTicks - ZabuzaCombatRules.FlickerReappearTick)));

        if (tick == 1f)
        {
            SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
            FlickerMist();
        }
        if (tick == ZabuzaCombatRules.FlickerVanishTick && Main.netMode != NetmodeID.MultiplayerClient &&
            FindFlickerSpot(target, out Vector2 spot))
        {
            NPC.position = spot;
            NPC.velocity = Vector2.Zero;
            NPC.direction = NPC.spriteDirection = target.Center.X >= NPC.Center.X ? 1 : -1;
            NPC.netUpdate = true;
        }
        if (tick == ZabuzaCombatRules.FlickerReappearTick)
        {
            SoundEngine.PlaySound(SoundID.Splash, NPC.Center);
            FlickerMist();
        }
        if (tick >= ZabuzaCombatRules.BodyFlickerTicks)
            Enter(ZabuzaCombatRules.Approach);
    }

    private void FlickerMist()
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        for (int i = 0; i < 26; i++)
            Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(NPC.width, NPC.height / 2f),
                DustID.Smoke, Main.rand.NextVector2Circular(2f, 2f), 90, new Color(190, 215, 230), 2f).noGravity = true;
        for (int i = 0; i < 12; i++)
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Water);
    }

    // Open ground behind the player (the side they are not facing), falling back to in front of them.
    private bool FindFlickerSpot(Player target, out Vector2 spot)
    {
        int behind = -target.direction;
        foreach (int side in new[] { behind, -behind })
            foreach (int tiles in new[] { 8, 6, 10, 7, 9 })
            {
                float x = target.Center.X + side * tiles * 16f - NPC.width / 2f;
                for (int rows = -6; rows <= 12; rows++)
                {
                    Vector2 candidate = new(x, target.Bottom.Y + rows * 16f - NPC.height);
                    if (Collision.SolidCollision(candidate, NPC.width, NPC.height) ||
                        !Collision.SolidCollision(candidate + new Vector2(0f, NPC.height), NPC.width, 8))
                        continue;
                    spot = candidate;
                    return true;
                }
            }
        spot = NPC.position;
        return false;
    }

    private void ShowDashCharge()
    {
        if (Main.netMode == NetmodeID.Server || NPC.ai[1] % 3f != 0f)
            return;
        Color color = InMistPhase ? new Color(170, 80, 255) : new Color(145, 220, 255);
        for (int i = 0; i < 3; i++)
            Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(30f, 36f),
                InMistPhase ? DustID.Shadowflame : DustID.Water,
                new Vector2(-NPC.ai[2] * 1.4f, -0.6f), 60, color, 1.2f).noGravity = true;
    }

    private int FindLinkedHaku()
    {
        int hakuType = ModContent.NPCType<HakuBoss>();
        for (int i = 0; i < Main.maxNPCs; i++)
            if (Main.npc[i].active && Main.npc[i].type == hakuType &&
                (int)Main.npc[i].ai[2] == NPC.whoAmI + 1)
                return i;
        return -1;
    }

    // While Haku's mirror cage holds the player, Zabuza waits outside, translucent.
    private bool HandleMirrorCage()
    {
        int index = NPC.ai[3] == 2f ? FindLinkedHaku() : -1;
        if (index >= 0 && Main.npc[index].ModNPC is HakuBoss haku && haku.InCage)
        {
            NPC.localAI[1] = 1f;
            NPC.damage = 0;
            NPC.alpha = 150;
            NPC.dontTakeDamage = true;
            Vector2 fromCenter = NPC.Center - haku.MirrorCenter;
            float outside = WaveDuoRules.CageRadius + 70f;
            if (fromCenter.Length() < outside)
            {
                float away = fromCenter.X >= 0f ? 1f : -1f;
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, away * 5f, 0.2f);
                NPC.direction = NPC.spriteDirection = -(int)away;
                if (NPC.collideX && NPC.velocity.Y == 0f)
                    NPC.velocity.Y = -8f;
            }
            else
                NPC.velocity.X *= 0.8f;
            return true;
        }
        if (NPC.localAI[1] != 0f)
        {
            NPC.localAI[1] = 0f;
            NPC.alpha = 0;
            NPC.dontTakeDamage = false;
            Enter(ZabuzaCombatRules.Approach);
        }
        return false;
    }

    private bool SpawnHaku()
    {
        if (FindLinkedHaku() >= 0)
            return true;
        Player target = Main.player[NPC.target];
        var candidates = WaveDuoRules.HakuSpawnCandidates(NPC.direction);
        for (int i = 0; i < candidates.Length; i++)
        {
            var (x, y, relativeToPlayer) = candidates[i];
            Vector2 center = (relativeToPlayer ? target.Center : NPC.Center) + new Vector2(x, y);
            bool lastResort = i == candidates.Length - 1;
            if (!lastResort && Collision.SolidCollision(center - new Vector2(
                WaveDuoRules.HakuBodyWidth * 0.5f, WaveDuoRules.HakuBodyHeight * 0.5f),
                WaveDuoRules.HakuBodyWidth, WaveDuoRules.HakuBodyHeight))
                continue;
            int index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)center.X,
                (int)(center.Y + WaveDuoRules.HakuBodyHeight * 0.5f),
                ModContent.NPCType<HakuBoss>(), ai0: HakuBoss.EmergeState, ai2: NPC.whoAmI + 1);
            if (index >= 0 && index < Main.maxNPCs)
            {
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, number: index);
                return true;
            }
            return false; // No free NPC slot; retry next tick.
        }
        return false;
    }

    private void MoveToward(Player target)
    {
        float distance = target.Center.X - NPC.Center.X;
        NPC.direction = distance >= 0f ? 1 : -1;
        NPC.spriteDirection = NPC.direction;
        float gap = Math.Abs(distance);
        float side = NPC.Center.X < target.Center.X ? -1f : 1f;
        float preferredGap = LastStand ? 120f : InMistPhase ? 190f : 170f;
        float desiredX = target.Center.X + side * preferredGap;
        float maxSpeed = gap > 400f ? (InMistPhase ? 9f : 7f) :
            (InMistPhase ? 6.6f : 5.2f);
        if (LastStand)
            maxSpeed *= 1.15f;
        float desiredSpeed = MathHelper.Clamp((desiredX - NPC.Center.X) * 0.07f,
            -maxSpeed, maxSpeed);
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, desiredSpeed,
            InMistPhase ? 0.2f : 0.16f);

        if (ZabuzaCombatRules.ShouldLeap((int)NPC.ai[0], NPC.ai[1],
            NPC.collideY && NPC.velocity.Y == 0f, gap, InMistPhase) ||
            (NPC.collideY && NPC.velocity.Y == 0f && target.Center.Y < NPC.Center.Y - 64f &&
             NPC.ai[1] % 20f == 0f))
        {
            if (Main.netMode != NetmodeID.Server)
                for (int i = 0; i < 7; i++)
                    Dust.NewDust(NPC.BottomLeft + new Vector2(0f, -7f), NPC.width, 8,
                        DustID.Cloud, newColor: new Color(175, 195, 200));
            NPC.velocity.Y = InMistPhase ? -11.2f : -9.6f;
            NPC.velocity.X = NPC.direction * (InMistPhase ? 5.8f : 4.8f);
            NPC.netUpdate = true;
        }

        if (NPC.collideX && NPC.velocity.Y == 0f)
        {
            NPC.velocity.Y = -8f;
            NPC.netUpdate = true;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        int state = (int)NPC.ai[0];
        int pose = ZabuzaCombatRules.PoseForState(state, NPC.ai[1],
            Math.Abs(NPC.velocity.X) > 1.1f, Math.Abs(NPC.velocity.Y) > 0.8f,
            (float)NPC.frameCounter);
        float aura = state == ZabuzaCombatRules.MistTransition
            ? ZabuzaCombatRules.TransitionAura(NPC.ai[1]) : 1f;
        Color readable = BossSprites.Lit(drawColor);
        if (InMistPhase)
            readable = Color.Lerp(readable, new Color(222, 196, 255), 0.3f * aura);
        if (LastStand)
            readable = Color.Lerp(readable, new Color(214, 170, 255), 0.3f);
        if (hitFlashTicks > 0)
            readable = Color.Lerp(readable, Color.White, hitFlashTicks / 7f);

        if (ZabuzaCombatRules.DrawBodyAfterimage(InMistPhase, state, NPC.ai[1],
            NPC.velocity.X, NPC.velocity.Y))
            for (int i = 4; i >= 2; i -= 2)
                if (NPC.oldPos[i] != Vector2.Zero)
                    DrawBody(spriteBatch, screenPos, pose, new Color(95, 175, 210) * 0.2f,
                        NPC.Bottom + NPC.oldPos[i] - NPC.position);
        DrawDemonAura(spriteBatch, screenPos);
        DrawBody(spriteBatch, screenPos, pose, readable, NPC.Bottom);
        return false;
    }

    // Red demon flames rising from the body behind the sprite once the demon awakens.
    private void DrawDemonAura(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        if (!InMistPhase)
            return;
        int state = (int)NPC.ai[0];
        float t = NPC.ai[1];
        float burstStart = state switch
        {
            ZabuzaCombatRules.MistTransition => ZabuzaCombatRules.TransitionBurstTick,
            ZabuzaCombatRules.FrenzyAwaken => 0f,
            _ => -1f
        };
        if (state == ZabuzaCombatRules.MistTransition && t < burstStart)
            return;
        Color color = Color.White * (NPC.Opacity * (LastStand ? 0.95f : 0.82f));
        if (burstStart >= 0f && t - burstStart < 18f &&
            BossSprites.TryDraw(spriteBatch, "Zabuza", "AuraBurst",
                BossSprites.Progress(t - burstStart, 18f, 3), 3, BossSprites.ZabuzaAura,
                NPC.Bottom, NPC.spriteDirection, color, screenPos))
            return;
        BossSprites.TryDraw(spriteBatch, "Zabuza", "Aura", BossSprites.Loop(LastStand ? 4f : 5f, 6), 6,
            BossSprites.ZabuzaAura, NPC.Bottom, NPC.spriteDirection, color, screenPos);
    }

    private void DrawBody(SpriteBatch spriteBatch, Vector2 screenPos, int pose, Color color,
        Vector2 bottom)
    {
        (string action, int count, int frame) = FrameFor(pose);
        // In the frenzy his bandages are gone: prefer the unmasked variant of each action.
        if (LastStand && BossSprites.TryDraw(spriteBatch, "Zabuza", "Frenzy" + action, frame, count,
            BossSprites.Zabuza, bottom, NPC.spriteDirection, color * NPC.Opacity, screenPos))
            return;
        if (BossSprites.TryDraw(spriteBatch, "Zabuza", action, frame, count, BossSprites.Zabuza,
            bottom, NPC.spriteDirection, color * NPC.Opacity, screenPos))
            return;
        (string legacy, float anchor) = LegacyPose(pose);
        DrawPose(spriteBatch, screenPos, NPC, legacy, anchor, color, 0f,
            ZabuzaCombatRules.BossDrawScale, bottom);
    }

    private (string Action, int Count, int Frame) FrameFor(int pose)
    {
        float t = NPC.ai[1];
        return pose switch
        {
            ZabuzaCombatRules.WindupPose => ("Windup", 3, BossSprites.Progress(t,
                (int)NPC.ai[0] switch
                {
                    ZabuzaCombatRules.DashWindup => ZabuzaCombatRules.DashWindupTicks(InMistPhase),
                    ZabuzaCombatRules.DashReaim => ZabuzaCombatRules.DashReaimTicks,
                    ZabuzaCombatRules.SwordThrowWindup => ZabuzaCombatRules.SwordThrowWindupTicks,
                    _ => SlashWindupTicks
                }, 3)),
            ZabuzaCombatRules.SlashPose => ("Slash", 3,
                BossSprites.Progress(t, ZabuzaCombatRules.SlashActiveTicks, 3)),
            ZabuzaCombatRules.SealPose => ("Seal", 3, BossSprites.Progress(t, 18f, 3)),
            ZabuzaCombatRules.RunPose or ZabuzaCombatRules.RunMidPose or ZabuzaCombatRules.RunAltPose =>
                ("Run", 6, (int)(NPC.frameCounter / (88d / 6d))),
            ZabuzaCombatRules.LeapPose => ("Leap", 2, NPC.velocity.Y < 0f ? 0 : 1),
            ZabuzaCombatRules.DashPose => ("Dash", 2, (int)(t / 4f)),
            ZabuzaCombatRules.KneelPose => ("Kneel", 2, t < 12f ? 0 : 1),
            ZabuzaCombatRules.RoarPose => ("Roar", 3, BossSprites.Progress(
                (int)NPC.ai[0] == ZabuzaCombatRules.FrenzyAwaken
                    ? t : t - ZabuzaCombatRules.TransitionBurstTick, 18f, 3)),
            ZabuzaCombatRules.ThrowPose => ("Throw", 3,
                BossSprites.Progress(t, ZabuzaCombatRules.SwordThrowReleaseTicks, 3)),
            ZabuzaCombatRules.UnarmedPose => ("Unarmed", 6, (int)(NPC.frameCounter / (88d / 6d))),
            ZabuzaCombatRules.CatchPose => ("Catch", 2, t < 8f ? 0 : 1),
            _ => ("Idle", 4, BossSprites.Loop(10f, 4))
        };
    }

    // Single-pose textures used until the matching animation frames are delivered.
    private static (string Texture, float Anchor) LegacyPose(int pose) => pose switch
    {
        ZabuzaCombatRules.WindupPose or ZabuzaCombatRules.KneelPose => ("ZabuzaWindupV2", 0.46f),
        ZabuzaCombatRules.SlashPose or ZabuzaCombatRules.RoarPose or ZabuzaCombatRules.ThrowPose =>
            ("ZabuzaSlashV2", 0.32f),
        ZabuzaCombatRules.SealPose => ("ZabuzaSealV2", 0.59f),
        ZabuzaCombatRules.RunPose or ZabuzaCombatRules.UnarmedPose => ("ZabuzaRunV2", 0.69f),
        ZabuzaCombatRules.RunMidPose => ("ZabuzaRunMidV4", 0.63f),
        ZabuzaCombatRules.RunAltPose => ("ZabuzaRunAltV4", 0.69f),
        ZabuzaCombatRules.LeapPose or ZabuzaCombatRules.DashPose => ("ZabuzaLeapV2", 0.75f),
        _ => ("ZabuzaIdleV2", 0.69f)
    };

    internal static bool DrawPose(SpriteBatch spriteBatch, Vector2 screenPos, NPC npc,
        string pose, float anchorFraction, Color drawColor, float rotation = 0f, float scale = 1f,
        Vector2? worldBottom = null)
    {
        Texture2D texture = ModContent.Request<Texture2D>($"ShinobiPrototype/Content/NPCs/{pose}").Value;
        bool facingRight = npc.spriteDirection >= 0;
        float anchorX = texture.Width * anchorFraction;
        Vector2 position = (worldBottom ?? npc.Bottom) - screenPos;
        position.X = (float)Math.Round(position.X);
        position.Y = (float)Math.Round(position.Y);
        spriteBatch.Draw(texture, position,
            null, drawColor * npc.Opacity, facingRight ? rotation : -rotation,
            new Vector2(facingRight ? anchorX : texture.Width - anchorX, texture.Height - 3), scale,
            facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        return false;
    }

    private void Enter(int state)
    {
        NPC.ai[0] = state;
        NPC.ai[1] = 0f;
        NPC.netUpdate = true;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write((int)NPC.localAI[0]);
        writer.Write(NPC.localAI[2]);
        writer.Write(NPC.localAI[3]);
        writer.Write(frenzyStarted);
        writer.Write((byte)comboSlashes);
        writer.Write((byte)dashTicks);
        writer.Write((byte)dashCount);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        NPC.localAI[0] = reader.ReadInt32();
        NPC.localAI[2] = reader.ReadSingle();
        NPC.localAI[3] = reader.ReadSingle();
        frenzyStarted = reader.ReadBoolean();
        comboSlashes = reader.ReadByte();
        dashTicks = reader.ReadByte();
        dashCount = reader.ReadByte();
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        hitFlashTicks = 6;
        int count = NPC.life <= 0 ? 28 : 10;
        for (int i = 0; i < count; i++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(3.8f, 3.8f);
            Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(14f, 24f),
                InMistPhase ? DustID.Shadowflame : DustID.Water, velocity, 30,
                InMistPhase ? new Color(200, 110, 255) : new Color(155, 235, 255),
                NPC.life <= 0 ? 1.55f : 1.25f).noGravity = true;
        }
    }

    public override void OnKill()
    {
        int haku = FindLinkedHaku();
        if (haku >= 0)
        {
            Main.npc[haku].ai[3] = 1f;
            Main.npc[haku].netUpdate = true;
            return;
        }
        // Haku already fell (or never arrived): this kill completes the encounter.
        CompleteEncounter(NPC);
    }

    internal static void CompleteEncounter(NPC lastBoss)
    {
        StoryWorld.CompleteWave();
        if (Main.netMode != NetmodeID.MultiplayerClient)
            Item.NewItem(lastBoss.GetSource_Loot(), lastBoss.getRect(),
                ModContent.ItemType<WaveCountryMedal>());
        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.WorldData);
        const string message = "再不斩与白已被击败。波之国主线完成，中忍考试现已开放。";
        if (Main.netMode == NetmodeID.Server)
            ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(message), new Color(100, 220, 160));
        else
            Main.NewText(message, 100, 220, 160);
    }
}
