using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.NPCs;

public sealed class ArenaRivalBoss : ModNPC
{
    private const int Approach = 0;
    private const int SandWindup = 1;
    private const int SandRecovery = 2;
    private const int RushWindup = 3;
    private const int Rushing = 4;
    private const int RushRecovery = 5;

    public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 1;

    public override void SetDefaults()
    {
        NPC.width = 30;
        NPC.height = 50;
        NPC.damage = 0;
        NPC.defense = 4;
        NPC.lifeMax = 760;
        NPC.knockBackResist = 0.1f;
        NPC.boss = true;
        NPC.aiStyle = -1;
        Music = MusicID.Boss2;
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

        NPC.ai[1]++;
        float distance = target.Center.X - NPC.Center.X;
        switch ((int)NPC.ai[0])
        {
            case Approach:
                NPC.direction = distance >= 0f ? 1 : -1;
                NPC.spriteDirection = NPC.direction;
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X,
                    Math.Abs(distance) > 140f ? NPC.direction * 3f : 0f, 0.07f);
                if (NPC.collideX && NPC.velocity.Y == 0f)
                    NPC.velocity.Y = -7f;
                if (NPC.ai[1] >= 40f)
                {
                    if (Math.Abs(distance) < 180f)
                        Enter(RushWindup);
                    else
                    {
                        NPC.ai[2] = (target.Center - NPC.Center).ToRotation();
                        Enter(SandWindup);
                    }
                }
                break;

            case SandWindup:
                NPC.velocity.X *= 0.7f;
                ShowSandWarning();
                if (NPC.ai[1] >= 45f)
                {
                    FireSand();
                    SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                    Enter(SandRecovery);
                }
                break;

            case SandRecovery:
                NPC.velocity.X *= 0.7f;
                if (NPC.ai[1] >= 44f)
                    Enter(Approach);
                break;

            case RushWindup:
                NPC.velocity.X *= 0.7f;
                ShowRushWarning();
                if (NPC.ai[1] >= 36f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero,
                            ModContent.ProjectileType<ArenaRushHitbox>(), 30, 0f,
                            Main.myPlayer, NPC.whoAmI, NPC.direction);
                    SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                    Enter(Rushing);
                }
                break;

            case Rushing:
                NPC.velocity.X = NPC.direction * 8f;
                if (NPC.ai[1] >= 20f || NPC.collideX)
                    Enter(RushRecovery);
                break;

            case RushRecovery:
                NPC.velocity.X *= 0.7f;
                if (NPC.ai[1] >= 50f)
                    Enter(Approach);
                break;
        }
    }

    private void ShowSandWarning()
    {
        Vector2 direction = new((float)Math.Cos(NPC.ai[2]), (float)Math.Sin(NPC.ai[2]));
        Vector2 marker = NPC.Center + direction * (25f + NPC.ai[1] * 3f);
        Dust.NewDustPerfect(marker, DustID.Sand, Vector2.Zero).noGravity = true;
    }

    private void ShowRushWarning()
    {
        Vector2 marker = NPC.Center + new Vector2(NPC.direction * (25f + NPC.ai[1] * 3f), 6f);
        Dust.NewDustPerfect(marker, DustID.Sand, Vector2.Zero).noGravity = true;
    }

    private void FireSand()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;
        Vector2 velocity = new((float)Math.Cos(NPC.ai[2]), (float)Math.Sin(NPC.ai[2]));
        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity * 9f,
            ModContent.ProjectileType<ArenaSandBolt>(), 25, 0f, Main.myPlayer);
    }

    private void Enter(int state)
    {
        NPC.ai[0] = state;
        NPC.ai[1] = 0f;
        NPC.netUpdate = true;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) =>
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ChuninHeadband>()));

    public override void OnKill()
    {
        StoryWorld.DownedArenaRival = true;
        if (Main.netMode != NetmodeID.Server)
            Main.NewText("中忍考试预选赛完成！你已晋升中忍，并获得中忍护额。", 100, 220, 160);
    }
}
