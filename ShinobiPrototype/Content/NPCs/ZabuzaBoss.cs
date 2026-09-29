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

[AutoloadBossHead]
public sealed class ZabuzaBoss : ModNPC
{
    private int demonHeadSlot = -1;
    private int hitFlashTicks;
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
        NPC.damage = 0; // Only the sword and water projectile deal damage.
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
            Enter(ZabuzaCombatRules.MistTransition);
            SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
        }

        NPC.ai[1]++;
        NPC.noGravity = (int)NPC.ai[0] is ZabuzaCombatRules.DashActive or ZabuzaCombatRules.DashChainActive;
        NPC.damage = NPC.noGravity
            ? WaveDuoRules.SoftenedDamage(InMistPhase ? 48 : 32) : 0;
        Lighting.AddLight(NPC.Center, InMistPhase ? (LastStand ? 0.58f : 0.44f) : 0.16f,
            InMistPhase ? (LastStand ? 0.2f : 0.13f) : 0.21f,
            InMistPhase ? (LastStand ? 0.76f : 0.63f) : 0.26f);
        NPC.dontTakeDamage = (int)NPC.ai[0] == ZabuzaCombatRules.MistTransition;
        NPC.alpha = 0;
        if (HandleMirrorDomain())
            return;
        switch ((int)NPC.ai[0])
        {
            case ZabuzaCombatRules.Approach:
                MoveToward(target);
                if (NPC.ai[1] >= ZabuzaCombatRules.ApproachTicks(InMistPhase,
                    Math.Abs(target.Center.X - NPC.Center.X)))
                {
                    int next = ZabuzaCombatRules.ChooseAttack(InMistPhase, (int)NPC.localAI[0],
                        Math.Abs(target.Center.X - NPC.Center.X), NPC.life / (float)NPC.lifeMax);
                    NPC.ai[2] = NPC.direction;
                    Enter(next);
                    if (next == ZabuzaCombatRules.RainWindup)
                    {
                        NPC.localAI[2] = target.Center.X;
                        NPC.localAI[3] = target.Center.Y;
                        NPC.netUpdate = true;
                    }
                    SoundEngine.PlaySound(next == ZabuzaCombatRules.SlashWindup ? SoundID.Item71 : SoundID.Item8, NPC.Center);
                }
                break;

            case ZabuzaCombatRules.SlashWindup:
                NPC.direction = NPC.spriteDirection = NPC.ai[2] >= 0f ? 1 : -1;
                NPC.velocity.X *= 0.75f;
                if (InMistPhase && NPC.ai[1] > ZabuzaCombatRules.SlashWindupTicks - 12)
                    NPC.velocity.X = NPC.ai[2] * 2.2f;
                if (NPC.ai[1] >= ZabuzaCombatRules.SlashWindupTicks)
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
                if (NPC.ai[1] >= ZabuzaCombatRules.RecoveryTicks(ZabuzaCombatRules.SlashRecovery, InMistPhase))
                    Enter(ZabuzaCombatRules.Approach);
                break;

            case ZabuzaCombatRules.WaterWindup:
                NPC.direction = NPC.spriteDirection = NPC.ai[2] >= 0f ? 1 : -1;
                NPC.velocity.X *= 0.7f;
                ShowWaterSeal();
                if (NPC.ai[1] >= ZabuzaCombatRules.WaterWindupTicks)
                {
                    Vector2 spawn = NPC.Center + new Vector2(NPC.ai[2] * 28f, -18f);
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
                NPC.velocity.X *= 0.64f;
                ShowDemonTransition();
                if (NPC.ai[1] == ZabuzaCombatRules.TransitionBurstTick)
                {
                    if (NPC.collideY && NPC.velocity.Y == 0f)
                        NPC.velocity.Y = -5.5f;
                    NPC.netUpdate = true;
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                }
                if (NPC.ai[1] >= ZabuzaCombatRules.TransitionCloneTick &&
                    NPC.ai[3] == 1f && (int)NPC.ai[1] % 30 == 6 &&
                    Main.netMode != NetmodeID.MultiplayerClient && SpawnHaku())
                {
                    NPC.ai[3] = 2f;
                    NPC.netUpdate = true;
                }
                if (NPC.ai[1] >= ZabuzaCombatRules.MistTransitionTicks && NPC.ai[3] >= 2f)
                {
                    NPC.dontTakeDamage = false;
                    Enter(ZabuzaCombatRules.Approach);
                }
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
                    Vector2 spawn = NPC.Center + new Vector2(NPC.ai[2] * 32f, -16f);
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
                if (ZabuzaCombatRules.DashHitWall(NPC.ai[1], NPC.collideX, NPC.collideY))
                    EndDash();
                else if (NPC.ai[1] >= ZabuzaCombatRules.DashActiveTicks)
                {
                    if (InMistPhase)
                        Enter(ZabuzaCombatRules.DashReaim);
                    else
                        EndDash();
                }
                break;

            case ZabuzaCombatRules.DashReaim:
                NPC.velocity *= 0.78f;
                NPC.ai[2] = target.Center.X >= NPC.Center.X ? 1f : -1f;
                NPC.direction = NPC.spriteDirection = (int)NPC.ai[2];
                ShowDashCharge();
                if (NPC.ai[1] >= ZabuzaCombatRules.DashReaimTicks)
                    StartDash(target, ZabuzaCombatRules.DashChainActive);
                break;

            case ZabuzaCombatRules.DashChainActive:
                if (ZabuzaCombatRules.DashHitWall(NPC.ai[1], NPC.collideX, NPC.collideY) ||
                    NPC.ai[1] >= ZabuzaCombatRules.DashActiveTicks)
                    EndDash();
                break;

            case ZabuzaCombatRules.DashRecovery:
                NPC.velocity.X *= 0.82f;
                if (NPC.ai[1] >= ZabuzaCombatRules.RecoveryTicks(ZabuzaCombatRules.DashRecovery, InMistPhase))
                    Enter(ZabuzaCombatRules.Approach);
                break;
        }

        if (Math.Abs(NPC.velocity.X) > 0.8f && Math.Abs(NPC.velocity.Y) < 0.8f)
            NPC.frameCounter = (NPC.frameCounter + Math.Abs(NPC.velocity.X)) % 88d;

        if (InMistPhase && (int)NPC.ai[0] != ZabuzaCombatRules.MistTransition)
            ShowDemonAura();
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
                LastStand ? new Color(255, 110, 240) : new Color(205, 95, 255),
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
            if ((int)tick % 2 != 0)
                return;
            float radius = MathHelper.Lerp(82f, 30f,
                tick / ZabuzaCombatRules.TransitionBurstTick);
            for (int i = 0; i < 3; i++)
            {
                Vector2 rim = NPC.Center + Main.rand.NextVector2CircularEdge(radius, radius * 0.8f);
                Vector2 velocity = Vector2.Normalize(NPC.Center - rim) * 2.4f;
                Dust.NewDustPerfect(rim, DustID.Shadowflame, velocity, 40,
                    new Color(205, 95, 255), 1.25f).noGravity = true;
            }
            return;
        }
        if (tick == ZabuzaCombatRules.TransitionBurstTick)
        {
            for (int i = 0; i < 32; i++)
            {
                Vector2 velocity = (MathHelper.TwoPi * i / 32f).ToRotationVector2() * 5.2f;
                Dust.NewDustPerfect(NPC.Center, DustID.Shadowflame, velocity, 20,
                    new Color(215, 125, 255), 1.7f).noGravity = true;
            }
        }
        else if ((int)tick % 3 == 0)
        {
            Vector2 point = NPC.Center + Main.rand.NextVector2Circular(40f, 44f);
            Dust.NewDustPerfect(point, DustID.Shadowflame,
                new Vector2(0f, -2f), 30, new Color(205, 95, 255), 1.35f).noGravity = true;
        }
    }

    private void ShowWaterSeal()
    {
        if (Main.netMode == NetmodeID.Server || !Main.rand.NextBool(2))
            return;
        Vector2 hands = NPC.Center + new Vector2(NPC.direction * 10f, -15f);
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
            -16f + slope * distance);
        for (int i = 0; i < 3; i++)
            Dust.NewDustPerfect(gathering + Main.rand.NextVector2Circular(8f, 8f),
                DustID.Water, Vector2.Zero, 40, new Color(110, 210, 240), 1.4f).noGravity = true;
    }

    private void FireFan(Player target)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;
        Vector2 spawn = NPC.Center + new Vector2(NPC.ai[2] * 34f, -22f);
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

    private void StartDash(Player target, int activeState)
    {
        float horizontal = target.Center.X - NPC.Center.X;
        NPC.ai[2] = horizontal >= 0f ? 1f : -1f;
        float slope = MathHelper.Clamp((target.Center.Y - NPC.Center.Y) /
            Math.Max(Math.Abs(horizontal), 90f), -0.28f, 0.28f);
        Vector2 direction = new(NPC.ai[2], slope);
        direction.Normalize();
        NPC.velocity = direction * ZabuzaCombatRules.DashSpeed(InMistPhase,
            NPC.life / (float)NPC.lifeMax) *
            (LastStand ? WaveDuoRules.ZabuzaLastStandDashMultiplier : 1f);
        if (NPC.collideY && NPC.velocity.Y > -1.5f)
            NPC.velocity.Y = -1.5f;
        NPC.direction = NPC.spriteDirection = (int)NPC.ai[2];
        NPC.noGravity = true;
        NPC.netUpdate = true;
        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
        Enter(activeState);
    }

    private void EndDash()
    {
        NPC.noGravity = false;
        NPC.damage = 0;
        NPC.velocity *= 0.4f;
        NPC.localAI[0]++;
        Enter(ZabuzaCombatRules.DashRecovery);
    }

    private void ShowDashCharge()
    {
        if (Main.netMode == NetmodeID.Server || NPC.ai[1] % 3f != 0f)
            return;
        Color color = InMistPhase ? new Color(190, 85, 255) : new Color(145, 220, 255);
        for (int i = 0; i < 3; i++)
            Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(36f, 42f),
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

    private bool HandleMirrorDomain()
    {
        int index = NPC.ai[3] >= 2f ? FindLinkedHaku() : -1;
        if (index >= 0 && Main.npc[index].ModNPC is HakuBoss haku && haku.InMirrorDomain)
        {
            NPC.localAI[1] = 1f;
            NPC.velocity *= 0.72f;
            NPC.damage = 0;
            NPC.alpha = haku.MirrorDomainHidden ? 255 : 0;
            NPC.dontTakeDamage = haku.MirrorDomainHidden;
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
        Vector2[] offsets = { new(-NPC.direction * 260f, -70f),
            new(NPC.direction * 260f, -70f), new(0f, -100f) };
        foreach (Vector2 offset in offsets)
        {
            Vector2 center = NPC.Center + offset;
            if (Collision.SolidCollision(center - new Vector2(
                20f, 38f), 40, 76))
                continue;
            int index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)center.X, (int)center.Y,
                ModContent.NPCType<HakuBoss>(), ai2: NPC.whoAmI + 1);
            if (index >= 0 && index < Main.maxNPCs)
            {
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, number: index);
                return true;
            }
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
        float preferredGap = InMistPhase ? 190f : 170f;
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
        int pose = ZabuzaCombatRules.PoseForState((int)NPC.ai[0], NPC.ai[1],
            Math.Abs(NPC.velocity.X) > 1.1f, Math.Abs(NPC.velocity.Y) > 0.8f,
            (float)NPC.frameCounter);
        string texture = pose switch
        {
            ZabuzaCombatRules.WindupPose => "ZabuzaWindupV2",
            ZabuzaCombatRules.SlashPose => "ZabuzaSlashV2",
            ZabuzaCombatRules.SealPose => "ZabuzaSealV2",
            ZabuzaCombatRules.RunPose => "ZabuzaRunV2",
            ZabuzaCombatRules.RunMidPose => "ZabuzaRunMidV4",
            ZabuzaCombatRules.RunAltPose => "ZabuzaRunAltV4",
            ZabuzaCombatRules.LeapPose => "ZabuzaLeapV2",
            _ => "ZabuzaIdleV2"
        };
        float anchor = pose switch
        {
            ZabuzaCombatRules.WindupPose => 0.46f,
            ZabuzaCombatRules.SlashPose => 0.32f,
            ZabuzaCombatRules.SealPose => 0.59f,
            ZabuzaCombatRules.RunPose => 0.69f,
            ZabuzaCombatRules.RunMidPose => 0.63f,
            ZabuzaCombatRules.RunAltPose => 0.69f,
            ZabuzaCombatRules.LeapPose => 0.75f,
            _ => 0.69f
        };
        float motion = NPC.velocity.Y != 0f ? -0.06f :
            (float)Math.Sin(Main.GlobalTimeWrappedHourly * 7f) * 0.015f;
        motion += MathHelper.Clamp(NPC.velocity.X * NPC.spriteDirection * 0.009f,
            -0.055f, 0.055f);
        float scale = ZabuzaCombatRules.BossDrawScale * (pose == ZabuzaCombatRules.WindupPose
            ? 1f + Math.Min(NPC.ai[1] / ZabuzaCombatRules.SlashWindupTicks, 1f) * 0.04f
            : 1f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5f) * 0.008f);
        float aura = (int)NPC.ai[0] == ZabuzaCombatRules.MistTransition
            ? ZabuzaCombatRules.TransitionAura(NPC.ai[1]) : 1f;
        if (InMistPhase)
            scale *= MathHelper.Lerp(1f, ZabuzaCombatRules.DemonDrawMultiplier, aura);
        if (LastStand)
            scale *= 1.075f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 10f) * 0.012f;
        if (ZabuzaCombatRules.DrawBodyAfterimage(InMistPhase, (int)NPC.ai[0], NPC.ai[1],
            NPC.velocity.X, NPC.velocity.Y))
        {
            for (int i = 4; i >= 2; i -= 2)
                if (NPC.oldPos[i] != Vector2.Zero)
                    DrawPose(spriteBatch, screenPos, NPC, texture, anchor,
                        new Color(95, 175, 210) * 0.2f, motion, scale,
                        NPC.Bottom + NPC.oldPos[i] - NPC.position);
        }
        Color readable = Color.Lerp(drawColor, Color.White, 0.5f);
        if (InMistPhase)
            readable = Color.Lerp(readable, new Color(235, 195, 255), 0.42f * aura);
        if (LastStand)
            readable = Color.Lerp(readable, new Color(255, 150, 245), 0.48f);
        if (hitFlashTicks > 0)
            readable = Color.Lerp(readable, Color.White, hitFlashTicks / 7f);
        return DrawPose(spriteBatch, screenPos, NPC, texture, anchor, readable, motion, scale);
    }

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
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        NPC.localAI[0] = reader.ReadInt32();
        NPC.localAI[2] = reader.ReadSingle();
        NPC.localAI[3] = reader.ReadSingle();
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
            Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(18f, 30f),
                InMistPhase ? DustID.Shadowflame : DustID.Water, velocity, 30,
                InMistPhase ? new Color(210, 105, 255) : new Color(155, 235, 255),
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
        if (LastStand)
            CompleteEncounter();
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

    private void CompleteEncounter() => CompleteEncounter(NPC);
}
