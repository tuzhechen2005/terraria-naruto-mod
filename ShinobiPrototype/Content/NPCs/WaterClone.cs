using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.NPCs;

public sealed class WaterClone : ModNPC
{
    public override string Texture => "ShinobiPrototype/Content/NPCs/ZabuzaBoss";

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 1;
    }

    public override void SetDefaults()
    {
        NPC.width = ZabuzaCombatRules.CloneBodyWidth;
        NPC.height = ZabuzaCombatRules.CloneBodyHeight;
        NPC.damage = 0;
        NPC.defense = 0;
        NPC.lifeMax = 65;
        NPC.knockBackResist = 0.5f;
        NPC.aiStyle = -1;
        NPC.color = new Color(90, 170, 230);
        NPC.alpha = 85;
        NPC.HitSound = SoundID.Splash;
        NPC.DeathSound = SoundID.Splash;
    }

    public override void AI()
    {
        int parentIndex = (int)NPC.ai[0];
        if (parentIndex < 0 || parentIndex >= Main.maxNPCs || !Main.npc[parentIndex].active ||
            Main.npc[parentIndex].type != ModContent.NPCType<ZabuzaBoss>())
        {
            NPC.active = false;
            return;
        }

        if (NPC.target < 0 || NPC.target >= Main.maxPlayers ||
            !Main.player[NPC.target].active || Main.player[NPC.target].dead)
            NPC.TargetClosest();
        Player target = Main.player[NPC.target];
        if (!target.active || target.dead)
            return;

        float distance = target.Center.X - NPC.Center.X;
        NPC.direction = distance >= 0f ? 1 : -1;
        NPC.spriteDirection = NPC.direction;
        NPC.ai[1]++;
        if (NPC.ai[3] > 0f)
            NPC.ai[3]--;

        if (NPC.ai[1] < ZabuzaCombatRules.CloneAttackPeriodTicks -
            ZabuzaCombatRules.CloneWindupTicks)
        {
            float flankX = target.Center.X + NPC.ai[2] * 240f;
            float desiredSpeed = MathHelper.Clamp((flankX - NPC.Center.X) * 0.07f, -4.6f, 4.6f);
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, desiredSpeed, 0.16f);
            if (NPC.collideX && NPC.velocity.Y == 0f)
            {
                NPC.velocity.Y = -8f;
                NPC.netUpdate = true;
            }
            if (NPC.collideY && NPC.velocity.Y == 0f &&
                target.Center.Y < NPC.Center.Y - 72f && NPC.ai[1] % 30f == 0f)
            {
                NPC.velocity.Y = -9f;
                NPC.netUpdate = true;
            }
        }
        else
        {
            NPC.velocity.X *= 0.82f;
            if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(3))
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Water);
            if (NPC.ai[1] >= ZabuzaCombatRules.CloneAttackPeriodTicks)
            {
                NPC parent = Main.npc[parentIndex];
                if (!ZabuzaCombatRules.CloneCanFire((int)parent.ai[0]))
                    NPC.ai[1] = ZabuzaCombatRules.CloneAttackPeriodTicks - 20;
                else
                {
                    if (Math.Abs(distance) < 115f)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero,
                                ModContent.ProjectileType<ZabuzaSlash>(), 16, 0f,
                                Main.myPlayer, NPC.whoAmI, NPC.direction);
                        SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                        NPC.ai[3] = ZabuzaCombatRules.SlashActiveTicks;
                    }
                    else
                    {
                        FireNeedles(target);
                        SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                    }
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                }
            }
        }
        if (Math.Abs(NPC.velocity.X) > 0.8f && Math.Abs(NPC.velocity.Y) < 0.8f)
            NPC.frameCounter = (NPC.frameCounter + Math.Abs(NPC.velocity.X)) % 88d;
    }

    private void FireNeedles(Player target)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;
        Vector2 spawn = NPC.Center + new Vector2(NPC.direction * 26f, -12f);
        Vector2 aim = target.Center - spawn;
        if (aim.LengthSquared() < 1f)
            aim = new Vector2(NPC.direction, 0f);
        aim.Normalize();
        for (int i = -1; i <= 1; i++)
        {
            Vector2 velocity = aim.RotatedBy(i * 0.13f) * 7.4f;
            Projectile.NewProjectile(NPC.GetSource_FromAI(), spawn, velocity,
                ModContent.ProjectileType<ZabuzaWaterNeedle>(), 15, 0f, Main.myPlayer, 1f);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Color watery = new Color(120, 210, 245) * 0.68f;
        if (NPC.ai[3] > 0f)
            return ZabuzaBoss.DrawPose(spriteBatch, screenPos, NPC, "ZabuzaSlashV2", 0.32f, watery);
        if (NPC.ai[1] >= ZabuzaCombatRules.CloneAttackPeriodTicks -
            ZabuzaCombatRules.CloneWindupTicks)
            return ZabuzaBoss.DrawPose(spriteBatch, screenPos, NPC, "ZabuzaSealV2", 0.59f, watery);
        int pose = ZabuzaCombatRules.PoseForState(ZabuzaCombatRules.Approach, NPC.ai[1],
            Math.Abs(NPC.velocity.X) > 1.1f, Math.Abs(NPC.velocity.Y) > 0.8f,
            (float)NPC.frameCounter);
        string texture = pose switch
        {
            ZabuzaCombatRules.RunPose => "ZabuzaRunV2",
            ZabuzaCombatRules.RunMidPose => "ZabuzaRunMidV4",
            ZabuzaCombatRules.RunAltPose => "ZabuzaRunAltV4",
            ZabuzaCombatRules.LeapPose => "ZabuzaLeapV2",
            _ => "ZabuzaIdleV2"
        };
        float anchor = pose switch
        {
            ZabuzaCombatRules.RunMidPose => 0.63f,
            ZabuzaCombatRules.LeapPose => 0.75f,
            _ => 0.69f
        };
        return ZabuzaBoss.DrawPose(spriteBatch, screenPos, NPC, texture, anchor, watery);
    }

    public override void OnKill()
    {
        for (int i = 0; i < 14; i++)
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Water);
    }

}
