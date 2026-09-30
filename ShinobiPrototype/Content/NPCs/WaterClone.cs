using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;

namespace ShinobiPrototype.Content.NPCs;

// Zabuza's water clone at the lake ambush: his shape in translucent blue. Runs at the player (on water too) and
// every few seconds winds up and dashes through with the blade. Breaks into water when hit hard enough; the water
// prison (ai[3]) forms a new one after a while and dissolves them all when it bursts.
public sealed class WaterClone : ModNPC
{
    private const int Chase = 0;
    private const int Windup = 1;
    private const int Dash = 2;
    private const int WindupTicks = 30;
    private const int DashTicks = 20;
    private const int DashEvery = 150;
    private const float RunSpeed = 3.2f;
    private const float DashSpeed = 11f;

    private static readonly Color Tint = new(110, 170, 235);

    private ref float State => ref NPC.ai[0];
    private ref float Timer => ref NPC.ai[1];

    public override string Texture => "ShinobiPrototype/Content/NPCs/ZabuzaIdleV2";

    public override void SetStaticDefaults()
    {
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers { Hide = true });
    }

    public override void SetDefaults()
    {
        NPC.width = ZabuzaCombatRules.BodyWidth;
        NPC.height = ZabuzaCombatRules.BodyHeight;
        NPC.aiStyle = -1;
        NPC.lifeMax = StoryRules.CloneLife;
        NPC.damage = StoryRules.CloneContactDamage;
        NPC.defense = 4;
        NPC.knockBackResist = 0.3f;
        NPC.HitSound = SoundID.Splash;
        NPC.DeathSound = SoundID.Splash;
        NPC.value = 0f;
        NPC.npcSlots = 0f;
    }

    public override bool CheckActive() => false;

    public override void AI()
    {
        int owner = (int)NPC.ai[3];
        if (owner < 0 || owner >= Main.maxNPCs || !Main.npc[owner].active ||
            Main.npc[owner].type != ModContent.NPCType<WaterPrison>())
        {
            Burst();
            NPC.active = false;
            return;
        }

        NPC.TargetClosest();
        Player target = Main.player[NPC.target];
        NPC.noGravity = false;
        Timer++;
        switch ((int)State)
        {
            case Chase:
                NPC.damage = StoryRules.CloneContactDamage;
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * RunSpeed, 0.1f);
                if (NPC.collideX && NPC.velocity.Y == 0f)
                    NPC.velocity.Y = -7f;
                if (Timer >= DashEvery + NPC.whoAmI % 3 * 20 && Math.Abs(target.Center.X - NPC.Center.X) < 360f)
                    Enter(Windup);
                break;
            case Windup:
                NPC.velocity.X *= 0.8f;
                if (Timer >= WindupTicks)
                {
                    Enter(Dash);
                    NPC.velocity.X = NPC.direction * DashSpeed;
                }
                break;
            case Dash:
                NPC.damage = StoryRules.CloneSlashDamage;
                NPC.direction = Math.Sign(NPC.velocity.X) is 0 ? NPC.direction : Math.Sign(NPC.velocity.X);
                if (Main.rand.NextBool(2))
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Water, -NPC.velocity.X * 0.2f, 0f);
                if (Timer >= DashTicks)
                    Enter(Chase);
                break;
        }
        NPC.spriteDirection = NPC.direction;
    }

    private void Enter(int state)
    {
        State = state;
        Timer = 0f;
        NPC.netUpdate = true;
    }

    // Stands on water, like Zabuza himself (see ZabuzaBoss.PostAI).
    public override void PostAI()
    {
        if (NPC.velocity.Y < 0f)
            return;
        int x = (int)(NPC.Center.X / 16f);
        int y = (int)((NPC.Bottom.Y + 1f) / 16f);
        if (!WorldGen.InWorld(x, y, 2))
            return;
        Tile feet = Main.tile[x, y];
        if (feet.LiquidAmount == 0 || feet.LiquidType != LiquidID.Water || feet.HasTile && Main.tileSolid[feet.TileType])
            return;
        int top = y;
        while (top > 1 && Main.tile[x, top - 1].LiquidAmount > 0 && Main.tile[x, top - 1].LiquidType == LiquidID.Water)
            top--;
        float surface = top * 16f + (255 - Main.tile[x, top].LiquidAmount) / 255f * 16f;
        if (NPC.Bottom.Y < surface - 2f)
            return;
        NPC.position.Y = surface - NPC.height;
        NPC.velocity.Y = 0f;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        for (int i = 0; i < 6; i++)
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Water, hit.HitDirection * 2f, -1f);
        if (NPC.life <= 0)
            Burst();
    }

    private void Burst()
    {
        if (Main.dedServ)
            return;
        for (int i = 0; i < 40; i++)
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Water, Main.rand.NextFloat(-3f, 3f),
                Main.rand.NextFloat(-4f, 0f), 0, default, 1.4f);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Color color = BossSprites.Lit(Tint.MultiplyRGB(drawColor), 0.4f) * 0.7f;
        (string action, int count, int frame) = (int)State switch
        {
            Windup => ("Windup", 3, BossSprites.Progress(Timer, WindupTicks, 3)),
            Dash => ("Dash", 2, BossSprites.Progress(Timer, DashTicks, 2)),
            _ when Math.Abs(NPC.velocity.X) > 0.5f => ("Run", 6, BossSprites.Loop(6f, 6)),
            _ => ("Idle", 4, BossSprites.Loop(8f, 4)),
        };
        BossSprites.TryDraw(spriteBatch, "Zabuza", action, frame, count, BossSprites.Zabuza, NPC.Bottom, NPC.direction,
            color, screenPos);
        return false;
    }
}
