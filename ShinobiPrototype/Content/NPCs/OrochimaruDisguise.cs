using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.Projectiles;

namespace ShinobiPrototype.Content.NPCs;

// Orochimaru in the guise of a Grass candidate, halfway through the Forest of Death (user, 2026-10-01). Something is
// off about him, without it being said: he comes alone (candidates come three to a squad), unarmed, never attacks,
// only walks slowly at the player, and his eyes are a snake's. He can be hit:
// - struck down, the body splits like a shed skin and Orochimaru rises out of the ground;
// - left alone, he reaches the player, peels his face away and lets the killing intent loose.
// Either way the same fight follows. Art: orochimaru-moves-v1 (DisguiseWalk); the candidate's sheet stands in without it.
public sealed class OrochimaruDisguise : ModNPC
{
    private const float RevealTiles = 3.5f;
    private const float WalkSpeed = 1.3f;

    public override string Texture => "ShinobiPrototype/Content/NPCs/ForestCanopyCandidate";

    // His walk (orochimaru-moves-v1: the long-haired Grass candidate, as in the reveal); standing, its first frame.
    private static bool OwnArt => BossSprites.Has("Orochimaru_DisguiseWalk_0");

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 7;
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers { Hide = true });
    }

    public override void SetDefaults()
    {
        // As big as any candidate (user, 2026-10-01): nothing about his size gives him away.
        NPC.width = 28;
        NPC.height = 56;
        NPC.lifeMax = 180;
        NPC.damage = 0;
        NPC.defense = 0;
        NPC.knockBackResist = 0.2f;
        NPC.aiStyle = -1;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.Item111;
        NPC.npcSlots = 0f;
    }

    public override bool CheckActive() => false;

    public override void AI()
    {
        NPC.TargetClosest();
        Player target = Main.player[NPC.target];
        if (!target.active || target.dead || NPC.Distance(target.Center) > 160 * 16)
        {
            // Nobody left to meet: gone back into the trees, to wait for the next one.
            if (Main.netMode != NetmodeID.MultiplayerClient && ++NPC.ai[1] > 300)
            {
                NPC.active = false;
                NPC.netUpdate = true;
            }
            return;
        }
        NPC.ai[1] = 0f;
        NPC.direction = NPC.spriteDirection = target.Center.X >= NPC.Center.X ? 1 : -1;
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * WalkSpeed, 0.05f);
        if (NPC.collideX && NPC.velocity.Y == 0f)
            NPC.velocity.Y = -7f;
        if (Main.netMode != NetmodeID.MultiplayerClient && NPC.Distance(target.Center) < RevealTiles * 16f)
            Become(Orochimaru.Reveal);
    }

    // Struck down: the shed skin stays where he fell, and he comes up out of the ground.
    public override bool CheckDead()
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<ShedSkin>(), 0, 0f,
                Main.myPlayer, NPC.spriteDirection);
            Become(Orochimaru.Emerge);
        }
        return false;
    }

    private void Become(float state)
    {
        int index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Bottom.Y, ModContent.NPCType<Orochimaru>(),
            0, state);
        if (index < Main.maxNPCs)
            Main.npc[index].netUpdate = true;
        NPC.life = 0;
        NPC.active = false;
        NPC.netUpdate = true;
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.spriteDirection = NPC.direction;
        NPC.frameCounter += System.Math.Abs(NPC.velocity.X);
        NPC.frame.Y = (System.Math.Abs(NPC.velocity.X) > 0.3f ? 2 + (int)(NPC.frameCounter / 10.0) % 4 : 0) * frameHeight;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (!OwnArt)
            return true;
        bool walking = System.Math.Abs(NPC.velocity.X) > 0.3f;
        BossSprites.TryDraw(spriteBatch, "Orochimaru", "DisguiseWalk", walking ? BossSprites.Loop(10f, 4) : 0, 4,
            ExamBoss.PersonCanvas, NPC.Bottom, NPC.direction, BossSprites.Lit(drawColor), screenPos);
        return false;
    }
}
