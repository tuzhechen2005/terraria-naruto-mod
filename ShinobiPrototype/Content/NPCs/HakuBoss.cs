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

[AutoloadBossHead]
public sealed class HakuBoss : ModNPC
{
    private const int Approach = 0;
    private const int NeedleWindup = 1;
    private const int NeedleRecovery = 2;
    private const int MirrorWindup = 3;
    private const int MirrorDash = 4;
    private const int MirrorRecovery = 5;
    private const int LastStandAwaken = 6;
    private const int PrismWindup = 7;
    private const int PrismHidden = 8;
    private const int PrismRecover = 9;
    private Vector2 mirrorDomainCenter;

    public override string Texture => "ShinobiPrototype/Content/NPCs/HakuPoseAtlasV2";
    public override string BossHeadTexture => "ShinobiPrototype/Content/NPCs/HakuBoss_Head_Boss";
    public bool LastStand => NPC.ai[3] == 1f;
    public bool InMirrorDomain => (int)NPC.ai[0] is PrismWindup or PrismHidden or PrismRecover;
    public bool MirrorDomainHidden => (int)NPC.ai[0] == PrismHidden;

    public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 1;

    public override void SetDefaults()
    {
        NPC.width = 40;
        NPC.height = 76;
        NPC.damage = 0;
        NPC.defense = 4;
        NPC.lifeMax = WaveDuoRules.HakuMaxLife;
        NPC.knockBackResist = 0.12f;
        NPC.boss = true;
        NPC.noGravity = true;
        NPC.noTileCollide = false;
        NPC.aiStyle = -1;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.BossBar = ModContent.GetInstance<HakuBossBar>();
        Music = MusicID.Boss1;
    }

    public override void AI()
    {
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
        if (parent != null && !InMirrorDomain)
            NPC.localAI[3] = Math.Min(WaveDuoRules.PrismCooldownTicks, NPC.localAI[3] + 1f);
        NPC.ai[1]++;
        if (Math.Abs(NPC.velocity.X) > 2.2f)
            NPC.localAI[2] = 1f;
        else if (Math.Abs(NPC.velocity.X) < 0.8f)
            NPC.localAI[2] = 0f;
        NPC.frameCounter += Math.Abs(NPC.velocity.X) * 0.55f;
        NPC.direction = target.Center.X >= NPC.Center.X ? 1 : -1;
        NPC.spriteDirection = NPC.direction;
        NPC.dontTakeDamage = (int)NPC.ai[0] is LastStandAwaken or PrismHidden;
        NPC.alpha = MirrorDomainHidden ? 255 : 0;
        Lighting.AddLight(NPC.Center, LastStand ? 0.25f : 0.14f, 0.4f, 0.55f);

        if (LastStand && NPC.localAI[1] == 0f)
        {
            NPC.localAI[1] = 1f;
            Enter(LastStandAwaken);
            SoundEngine.PlaySound(SoundID.Item27, NPC.Center);
        }
        if (parent != null && !WaveDuoRules.HakuMayAttack((int)parent.ai[0]))
        {
            NPC.velocity *= 0.84f;
            ShowIceAura(2);
            return;
        }

        switch ((int)NPC.ai[0])
        {
            case Approach:
                MoveToFlank(target, parent);
                if (Main.netMode != NetmodeID.MultiplayerClient &&
                    WaveDuoRules.PrismMayStart(NPC.active, parent != null,
                        parent == null ? -1 : (int)parent.ai[0], (int)NPC.localAI[3]))
                {
                    NPC.localAI[3] = 0f;
                    mirrorDomainCenter = target.Center;
                    NPC.velocity *= 0.3f;
                    Enter(PrismWindup);
                    SoundEngine.PlaySound(SoundID.Item27, NPC.Center);
                    break;
                }
                if (NPC.ai[1] >= (LastStand ? 38f : 52f) &&
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
                    FireNeedles(target);
                    SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                    NPC.localAI[0]++;
                    Enter(NeedleRecovery);
                }
                break;

            case NeedleRecovery:
                NPC.velocity *= 0.82f;
                if (NPC.ai[1] >= (LastStand ? 31f : WaveDuoRules.HakuNeedleRecoveryTicks))
                    Enter(Approach);
                break;

            case MirrorWindup:
                NPC.velocity *= 0.78f;
                ShowIceAura(3);
                if (NPC.ai[1] >= WaveDuoRules.HakuDashWindupTicks(LastStand))
                {
                    if (parent != null && !WaveDuoRules.HakuMayUseStrongAttack((int)parent.ai[0]))
                    {
                        NPC.ai[1] = WaveDuoRules.HakuDashWindupTicks(LastStand) - 1;
                        break;
                    }
                    StartMirrorDash(target);
                }
                break;

            case MirrorDash:
                ShowIceAura(1);
                if (NPC.ai[1] >= WaveDuoRules.HakuDashActiveTicks || NPC.collideX || NPC.collideY)
                {
                    NPC.velocity *= 0.3f;
                    NPC.localAI[0]++;
                    Enter(MirrorRecovery);
                }
                break;

            case MirrorRecovery:
                NPC.velocity *= 0.8f;
                if (NPC.ai[1] >= (LastStand ? 34f : WaveDuoRules.HakuDashRecoveryTicks))
                    Enter(Approach);
                break;

            case LastStandAwaken:
                NPC.velocity *= 0.75f;
                ShowIceAura(4);
                if (NPC.ai[1] >= 48f)
                {
                    NPC.dontTakeDamage = false;
                    Enter(Approach);
                }
                break;

            case PrismWindup:
                NPC.velocity *= 0.78f;
                ShowIceAura(4);
                if (NPC.ai[1] >= WaveDuoRules.PrismWindupTicks)
                {
                    SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                    Enter(PrismHidden);
                }
                break;

            case PrismHidden:
                NPC.velocity *= 0.72f;
                if (WaveDuoRules.IsPrismVolley((int)NPC.ai[1]) &&
                    Vector2.DistanceSquared(target.Center, mirrorDomainCenter) <= 600f * 600f)
                {
                    SoundEngine.PlaySound(SoundID.Item17, target.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        FirePrismShard(target, WaveDuoRules.PrismVolleyIndex((int)NPC.ai[1]));
                }
                if (NPC.ai[1] >= WaveDuoRules.PrismHiddenTicks)
                {
                    NPC.alpha = 0;
                    NPC.dontTakeDamage = false;
                    Enter(PrismRecover);
                }
                break;

            case PrismRecover:
                NPC.velocity *= 0.8f;
                if (NPC.ai[1] >= WaveDuoRules.PrismRecoverTicks)
                    Enter(Approach);
                break;
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
        NPC.velocity = direction * WaveDuoRules.HakuDashSpeed(LastStand);
        if (Main.netMode != NetmodeID.MultiplayerClient)
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero,
                ModContent.ProjectileType<HakuDashHitbox>(),
                WaveDuoRules.SoftenedDamage(LastStand ? 28 : 23), 0f,
                Main.myPlayer, NPC.whoAmI, NPC.direction);
        SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
        Enter(MirrorDash);
    }

    private void FireNeedles(Player target)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;
        Vector2 direction = target.Center - NPC.Center;
        if (direction.LengthSquared() < 1f)
            direction = new Vector2(NPC.direction, 0f);
        direction.Normalize();
        int count = WaveDuoRules.HakuNeedleCount(LastStand);
        for (int i = 0; i < count; i++)
        {
            float angle = (i - (count - 1) * 0.5f) * 0.14f;
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + direction * 18f,
                direction.RotatedBy(angle) * (LastStand ? 10.5f : 9f),
                ModContent.ProjectileType<HakuNeedle>(),
                WaveDuoRules.SoftenedDamage(LastStand ? 21 : 18), 0f, Main.myPlayer);
        }
    }

    private void FirePrismShard(Player target, int volley)
    {
        if (Vector2.DistanceSquared(target.Center, mirrorDomainCenter) > 600f * 600f)
            return;
        Vector2[] offsets = { new(-245f, -45f), new(245f, -45f),
            new(0f, -195f), new(0f, 195f) };
        Vector2 spawn = mirrorDomainCenter + offsets[volley];
        if (Collision.SolidCollision(spawn - new Vector2(13f, 8f), 26, 16))
            spawn = mirrorDomainCenter + new Vector2(volley % 2 == 0 ? -215f : 215f, -145f);
        if (Collision.SolidCollision(spawn - new Vector2(13f, 8f), 26, 16))
            return;
        Vector2 direction = target.Center - spawn;
        if (direction.LengthSquared() < 1f)
            return;
        direction.Normalize();
        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawn,
            direction * 8.5f, ModContent.ProjectileType<HakuPrismShard>(),
            WaveDuoRules.SoftenedDamage(21), 0f, Main.myPlayer, 0f, NPC.whoAmI + 1);
    }

    private void ShowIceAura(int count)
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        for (int i = 0; i < count; i++)
        {
            Vector2 point = NPC.Center + Main.rand.NextVector2Circular(35f, 42f);
            Dust.NewDustPerfect(point, DustID.IceTorch, new Vector2(0f, -1.2f), 45,
                LastStand ? new Color(225, 250, 255) : new Color(140, 230, 255),
                LastStand ? 1.4f : 1.1f).noGravity = true;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D atlas = ModContent.Request<Texture2D>(Texture).Value;
        int cell = atlas.Width / 2;
        int pose = (int)NPC.ai[0] switch
        {
            NeedleWindup => 2,
            MirrorWindup or MirrorDash => 3,
            Approach when NPC.localAI[2] == 1f => 1,
            _ => 0
        };
        Rectangle frame = new((pose % 2) * cell, (pose / 2) * cell, cell, cell);
        if (InMirrorDomain)
            DrawMirrorDomain(spriteBatch, screenPos);
        if ((int)NPC.ai[0] is MirrorWindup or MirrorDash or LastStandAwaken)
            DrawIceMirrors(spriteBatch, screenPos);
        Color color = Color.Lerp(drawColor, Color.White, 0.52f);
        if (LastStand)
            color = Color.Lerp(color, new Color(185, 245, 255), 0.45f);
        float footAnchor = pose switch { 0 => 614f, 1 => 600f, 2 => 574f, _ => 540f };
        float strideBob = pose == 1 ? (float)Math.Sin(NPC.frameCounter * 0.32d) * 1.4f : 0f;
        Vector2 drawPosition = NPC.Bottom - screenPos + new Vector2(0f, strideBob);
        spriteBatch.Draw(atlas, drawPosition, frame, color * NPC.Opacity,
            0f, new Vector2(cell * 0.5f, footAnchor), 0.17f,
            NPC.spriteDirection >= 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        return false;
    }

    private void DrawIceMirrors(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Texture2D mirror = ModContent.Request<Texture2D>(
            "ShinobiPrototype/Content/NPCs/HakuIceMirrorV2").Value;
        Vector2 origin = mirror.Size() * 0.5f;
        float pulse = 0.85f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 8f) * 0.08f;
        for (int side = -1; side <= 1; side += 2)
        {
            Vector2 point = NPC.Center + new Vector2(side * 80f, -6f) - screenPos;
            spriteBatch.Draw(mirror, point, null,
                (LastStand ? new Color(215, 250, 255) : new Color(130, 220, 255)) * 0.8f,
                0f, origin, 0.07f * pulse, SpriteEffects.None, 0f);
        }
    }

    private void DrawMirrorDomain(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        if (NPC.target < 0 || NPC.target >= Main.maxPlayers)
            return;
        Texture2D mirror = ModContent.Request<Texture2D>(
            "ShinobiPrototype/Content/NPCs/HakuIceMirrorV2").Value;
        Vector2 center = (mirrorDomainCenter == Vector2.Zero
            ? Main.player[NPC.target].Center : mirrorDomainCenter) - screenPos;
        Vector2[] offsets = { new(-210f, 0f), new(210f, 0f),
            new(0f, -155f), new(0f, 155f) };
        float fade = (int)NPC.ai[0] == PrismWindup
            ? MathHelper.Clamp(NPC.ai[1] / WaveDuoRules.PrismWindupTicks, 0f, 1f)
            : (int)NPC.ai[0] == PrismRecover
                ? 1f - MathHelper.Clamp(NPC.ai[1] / WaveDuoRules.PrismRecoverTicks, 0f, 1f)
                : 1f;
        float pulse = 0.92f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f) * 0.05f;
        for (int i = 0; i < offsets.Length; i++)
            spriteBatch.Draw(mirror, center + offsets[i], null,
                new Color(145, 235, 255) * (0.74f * fade),
                i >= 2 ? MathHelper.PiOver2 : 0f, mirror.Size() * 0.5f,
                0.08f * pulse, SpriteEffects.None, 0f);
    }

    private void Enter(int state)
    {
        NPC.ai[0] = state;
        NPC.ai[1] = 0f;
        NPC.netUpdate = true;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(mirrorDomainCenter.X);
        writer.Write(mirrorDomainCenter.Y);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        mirrorDomainCenter = new Vector2(reader.ReadSingle(), reader.ReadSingle());
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
        else if (LastStand)
            ZabuzaBoss.CompleteEncounter(NPC);
    }
}
