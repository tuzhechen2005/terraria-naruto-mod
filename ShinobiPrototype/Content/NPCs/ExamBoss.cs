using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Systems;

namespace ShinobiPrototype.Content.NPCs;

// Shared frame for the Chūnin Exam fights (Dosu, Gaara, Neji, Orochimaru; ExamBossRules): a person-sized boss with a
// state machine in ai[0] (state) and ai[1] (timer), run by the server, with every hit coming from JutsuHitbox. Who
// took part is judged on each player's own client (a player who landed a hit), since the exam progress lives there.
// Placeholder art: the candidate sprite, tinted per boss, until the real sprites are drawn.
public abstract class ExamBoss : ModNPC
{
    public override string Texture => "ShinobiPrototype/Content/NPCs/ForestExamCandidate";

    // The boss bar at the bottom of the screen needs a head icon (vanilla shows no bar without one); cut from the
    // idle frame, "<Name>_Head_Boss.png".
    public override string BossHeadTexture => $"ShinobiPrototype/Content/NPCs/{Name}_Head_Boss";

    protected abstract Color Tint { get; }
    protected abstract int LifeMax { get; }
    protected abstract int Defense { get; }

    protected ref float State => ref NPC.ai[0];
    protected ref float Timer => ref NPC.ai[1];

    private bool hitByLocalPlayer;
    private int leaveTicks;
    private bool introShown;
    protected int HurtTicks;

    // Frame-by-frame art (BossSprites: "<Prefix>_<Action>_<n>.png" in Content/NPCs). Until the frames are there the
    // tinted placeholder is drawn instead.
    internal static readonly BossSprites.Canvas PersonCanvas = new(112, 88, 56, 84);
    protected virtual string SpritePrefix => null;
    private protected virtual BossSprites.Canvas CanvasFor(string action) => PersonCanvas;
    // The animation to show now: its frame count, ticks per frame, and whether it loops (otherwise it plays once
    // from the start of the current state).
    protected virtual (string Action, int Frames, int TicksPerFrame, bool Loop) Pose => ("Idle", 4, 10, true);

    protected (string, int, int, bool) Moving(string walk, string idle) =>
        System.Math.Abs(NPC.velocity.X) > 0.4f ? (walk, 4, 7, true) : (idle, 4, 10, true);

    // The entrance title (BossIntroSystem): name and who they are, shown once the boss has shown itself.
    protected abstract (string Name, string Title) Intro { get; }
    protected virtual bool Revealed => true;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 1;
        // Summoned from a player's item in multiplayer (NPC.SpawnOnPlayer).
        NPCID.Sets.MPAllowedEnemies[Type] = true;
    }

    public override void SetDefaults()
    {
        NPC.width = 28;
        NPC.height = 46;
        NPC.scale = 1.15f;
        NPC.boss = true;
        NPC.lifeMax = LifeMax;
        NPC.defense = Defense;
        NPC.damage = 0;
        NPC.knockBackResist = 0f;
        NPC.aiStyle = -1;
        NPC.npcSlots = 10f;
        NPC.value = Item.buyPrice(gold: 3);
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.color = Tint;
    }

    public override void AI()
    {
        NPC.TargetClosest();
        Player target = Main.player[NPC.target];
        if (!target.active || target.dead || Vector2.Distance(target.Center, NPC.Center) > 3000f)
        {
            // Everyone fled or fell: leave the field.
            NPC.velocity.X *= 0.9f;
            if (++leaveTicks > 180)
            {
                Smoke();
                NPC.active = false;
                NPC.netUpdate = true;
            }
            return;
        }
        leaveTicks = 0;
        Timer++;
        if (HurtTicks > 0)
            HurtTicks--;
        if (!introShown && Revealed && Main.netMode != NetmodeID.Server)
        {
            introShown = true;
            BossIntroSystem.Show(Intro.Name, Intro.Title);
        }
        Fight(target);
    }

    protected abstract void Fight(Player target);

    protected void Enter(float state)
    {
        State = state;
        Timer = 0f;
        NPC.netUpdate = true;
    }

    protected void Face(float x)
    {
        NPC.direction = x >= NPC.Center.X ? 1 : -1;
        NPC.spriteDirection = NPC.direction;
    }

    protected void RunTo(float x, float speed, Player target)
    {
        float gap = x - NPC.Center.X;
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Math.Clamp(gap * 0.05f, -speed, speed), 0.12f);
        Face(Math.Abs(gap) > 24f ? x : target.Center.X);
        if (NPC.collideX && NPC.velocity.Y == 0f)
            NPC.velocity.Y = -8f;
        if (target.Bottom.Y < NPC.Top.Y - 40f && NPC.velocity.Y == 0f && (int)Timer % 45 == 0)
            NPC.velocity.Y = -9.5f;
    }

    // Only the server (or single player) decides; clients follow the synced state.
    protected static bool Deciding => Main.netMode != NetmodeID.MultiplayerClient;

    protected void Telegraph(int dust, float radius = 30f)
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2CircularEdge(radius, radius), dust, Vector2.Zero, 0, default, 1.2f)
            .noGravity = true;
    }

    protected void Smoke()
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        for (int i = 0; i < 30; i++)
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 1f), 100, default, 1.6f);
    }

    public static void Tell(string text, Color color)
    {
        if (Main.netMode == NetmodeID.Server)
            ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(text), color);
        else if (Main.netMode == NetmodeID.SinglePlayer)
            Main.NewText(text, color);
    }

    // A line said over the boss's head (every client draws it).
    protected void Say(string text, Color color)
    {
        if (Main.netMode != NetmodeID.Server)
            CombatText.NewText(NPC.getRect(), color, text, true);
    }

    public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
    {
        if (player.whoAmI == Main.myPlayer)
            hitByLocalPlayer = true;
    }

    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        if (projectile.owner == Main.myPlayer && projectile.friendly)
            hitByLocalPlayer = true;
    }

    // Whether the local player took part in the fight.
    protected bool LocalPlayerFought => hitByLocalPlayer;

    public override bool PreDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (SpritePrefix == null)
            return true;
        (string action, int frames, int ticksPerFrame, bool loop) = HurtTicks > 0 ? ("Hurt", 1, 10, true) : Pose;
        int frame = loop ? (int)(Main.GameUpdateCount / (uint)ticksPerFrame) : System.Math.Min(frames - 1, (int)(Timer / ticksPerFrame));
        return !BossSprites.TryDraw(spriteBatch, SpritePrefix, action, frame, frames, CanvasFor(action), NPC.Bottom, NPC.direction,
            BossSprites.Lit(drawColor), screenPos);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        HurtTicks = 8;
        if (NPC.life > 0 || Main.netMode == NetmodeID.Server)
            return;
        Smoke();
        if (hitByLocalPlayer)
            LocalVictory(Main.LocalPlayer);
    }

    // The local player took part in beating the boss (runs on that player's client).
    protected virtual void LocalVictory(Player player) { }

    public override bool CheckActive() => false;
}
