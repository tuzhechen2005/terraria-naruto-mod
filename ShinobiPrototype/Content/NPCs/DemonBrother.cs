using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items;

namespace ShinobiPrototype.Content.NPCs;

// The Demon Brothers (鬼之兄弟), an elite pair joined by a chain (rules in DemonBrotherRules). Gōzu keeps to the
// player's left and Meizu to the right; they swipe with their claw gauntlets up close, and Gōzu calls the chain sweep:
// the chain flashes, then both rush past each other so it cuts through the player between them. When one falls the
// other goes berserk. Frames: Idle, Run x6, Swipe x3, Hurt (64x56, facing left).
public abstract class DemonBrother : ModNPC
{
    private const int Chase = 0;
    private const int SwipeWindup = 1;
    private const int Swipe = 2;
    private const int SwipeRecovery = 3;
    private const int ChainWarn = 4;
    private const int ChainRush = 5;

    private const int FrameIdle = 0;
    private const int FrameRunFirst = 1;
    private const int FrameSwipeFirst = 7;
    private const int FrameHurt = 10;

    private int hurtTicks;

    protected abstract int Side { get; }
    protected abstract int PartnerType { get; }
    protected bool Leads => Side < 0;

    private ref float State => ref NPC.ai[0];
    private ref float Timer => ref NPC.ai[1];
    private ref float PartnerIndex => ref NPC.ai[2];
    private ref float ChainCooldown => ref NPC.ai[3];

    public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 11;

    public override void SetDefaults()
    {
        NPC.width = 26;
        NPC.height = 46;
        NPC.lifeMax = DemonBrotherRules.LifeMax;
        NPC.damage = DemonBrotherRules.ContactDamage;
        NPC.defense = 6;
        NPC.knockBackResist = 0.25f;
        NPC.aiStyle = -1;
        NPC.value = 1500f;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath2;
    }

    protected NPC Partner
    {
        get
        {
            int i = (int)PartnerIndex;
            return i >= 0 && i < Main.maxNPCs && Main.npc[i].active && Main.npc[i].type == PartnerType ? Main.npc[i] : null;
        }
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (!Leads)
            return 0f;
        Player player = spawnInfo.Player;
        bool found = player.HasItem(ModContent.ItemType<MistInsignia>()) ||
                     player.GetModPlayer<StoryPlayer>().InsigniaNoticeShown;
        bool inMist = WaveBridgeWorld.MistActive &&
                      WaveBridgeWorld.DistanceToBridgeTiles(player.Center) < BridgeRules.FogReachTiles;
        bool alive = NPC.AnyNPCs(Type) || NPC.AnyNPCs(PartnerType);
        return DemonBrotherRules.SpawnAllowed(found, Main.raining, inMist, StoryWorld.WaveComplete, alive,
            player.ZoneOverworldHeight) ? DemonBrotherRules.SpawnChance : 0f;
    }

    // Gōzu brings Meizu with him, on the far side of the player: the pincer.
    public override void OnSpawn(IEntitySource source)
    {
        // NewNPC fills ai[] after SetDefaults, so starting values are set here.
        ChainCooldown = DemonBrotherRules.ChainCooldownTicks / 2f;
        if (!Leads || Main.netMode == NetmodeID.MultiplayerClient)
            return;
        Player player = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)];
        float mirrorX = player.Center.X + (player.Center.X - NPC.Center.X);
        int partner = NPC.NewNPC(source, (int)mirrorX, (int)NPC.Bottom.Y, PartnerType);
        if (partner >= Main.maxNPCs)
            return;
        PartnerIndex = partner;
        Main.npc[partner].ai[2] = NPC.whoAmI;
        NPC.netUpdate = true;
        Main.npc[partner].netUpdate = true;
    }

    public override void AI()
    {
        NPC.TargetClosest();
        Player target = Main.player[NPC.target];
        if (!target.active || target.dead)
        {
            NPC.velocity.X *= 0.9f;
            return;
        }

        NPC partner = Partner;
        bool berserk = partner == null;
        float speed = DemonBrotherRules.RunSpeed * (berserk ? DemonBrotherRules.BerserkSpeedMultiplier : 1f);
        if (ChainCooldown > 0f)
            ChainCooldown--;
        if (hurtTicks > 0)
            hurtTicks--;
        Timer++;
        NPC.damage = State == ChainWarn ? 0 :
            State == Swipe ? (int)(DemonBrotherRules.ContactDamage * 1.5f) : DemonBrotherRules.ContactDamage;

        switch ((int)State)
        {
            case Chase:
                RunTo(berserk ? target.Center.X : DemonBrotherRules.FlankX(target.Center.X, Side), speed, target);
                if (Leads && partner != null && TryStartChainSweep(target, partner))
                    break;
                if (Math.Abs(target.Center.X - NPC.Center.X) < DemonBrotherRules.SwipeRange &&
                    Math.Abs(target.Center.Y - NPC.Center.Y) < 60f)
                    Enter(SwipeWindup);
                break;

            case SwipeWindup:
                NPC.velocity.X *= 0.8f;
                Face(target.Center.X);
                if (Timer >= DemonBrotherRules.SwipeWindupTicks)
                {
                    NPC.velocity.X = NPC.direction * 6f;
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                    Enter(Swipe);
                }
                break;

            case Swipe:
                if (Timer >= DemonBrotherRules.SwipeActiveTicks)
                    Enter(SwipeRecovery);
                break;

            case SwipeRecovery:
                NPC.velocity.X *= 0.85f;
                if (Timer >= DemonBrotherRules.SwipeRecoveryTicks)
                    Enter(Chase);
                break;

            case ChainWarn:
                NPC.velocity.X *= 0.7f;
                if (partner == null)
                {
                    Enter(Chase);
                    break;
                }
                Face(partner.Center.X);
                if (Timer >= DemonBrotherRules.ChainWarnTicks)
                {
                    NPC.velocity.X = Math.Sign(partner.Center.X - NPC.Center.X) * DemonBrotherRules.ChainRushSpeed;
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item71, NPC.Center);
                    Enter(ChainRush);
                }
                break;

            case ChainRush:
                if (NPC.collideX && NPC.velocity.Y == 0f)
                    NPC.velocity.Y = -7f;
                if (Leads && partner != null)
                    HurtPlayerOnChain(partner);
                if (Timer >= DemonBrotherRules.ChainRushTicks)
                {
                    ChainCooldown = DemonBrotherRules.ChainCooldownTicks;
                    Enter(SwipeRecovery);
                }
                break;
        }
    }

    private void RunTo(float x, float speed, Player target)
    {
        float gap = x - NPC.Center.X;
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Math.Clamp(gap * 0.05f, -speed, speed), 0.1f);
        Face(Math.Abs(gap) > 24f ? x : target.Center.X);
        if (NPC.collideX && NPC.velocity.Y == 0f)
            NPC.velocity.Y = -7f;
        if (target.Center.Y < NPC.Center.Y - 90f && NPC.velocity.Y == 0f && Timer % 40f == 0f)
            NPC.velocity.Y = -8.5f;
    }

    private void Face(float x)
    {
        NPC.direction = x >= NPC.Center.X ? 1 : -1;
        NPC.spriteDirection = NPC.direction;
    }

    // Gōzu decides; only the server (or single player) starts it, and both brothers switch together.
    private bool TryStartChainSweep(Player target, NPC partner)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return false;
        float left = Math.Min(NPC.Center.X, partner.Center.X);
        float right = Math.Max(NPC.Center.X, partner.Center.X);
        bool between = target.Center.X > left && target.Center.X < right;
        if (!DemonBrotherRules.ChainSweepReady((int)ChainCooldown, right - left, between))
            return false;
        Enter(ChainWarn);
        partner.ai[0] = ChainWarn;
        partner.ai[1] = 0f;
        partner.netUpdate = true;
        return true;
    }

    private Vector2 Hand => NPC.Center + new Vector2(NPC.direction * 12f, -2f);

    // Each client checks its own player against the swept chain (hurting a player happens on their client).
    private void HurtPlayerOnChain(NPC partner)
    {
        Player player = Main.LocalPlayer;
        Vector2 a = Hand;
        Vector2 b = ((DemonBrother)partner.ModNPC).Hand;
        if (!player.active || player.dead || player.immune ||
            !DemonBrotherRules.ChainHurts(true, Vector2.Distance(a, b)) ||
            !Collision.CheckAABBvLineCollision(player.position, player.Size, a, b))
            return;
        player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), DemonBrotherRules.ChainDamage,
            player.Center.X >= NPC.Center.X ? 1 : -1);
    }

    private void Enter(int state)
    {
        State = state;
        Timer = 0f;
        NPC.netUpdate = true;
    }

    public override void HitEffect(NPC.HitInfo hit) => hurtTicks = 10;

    public override void FindFrame(int frameHeight)
    {
        int frame;
        if (hurtTicks > 0)
            frame = FrameHurt;
        else if (State is SwipeWindup or ChainWarn)
            frame = FrameSwipeFirst;
        else if (State == Swipe)
            frame = FrameSwipeFirst + 1;
        else if (State == SwipeRecovery && Timer < 10f)
            frame = FrameSwipeFirst + 2;
        else if (NPC.velocity.Y != 0f)
            frame = FrameRunFirst + 2;
        else if (Math.Abs(NPC.velocity.X) > 0.3f)
        {
            NPC.frameCounter += Math.Abs(NPC.velocity.X);
            frame = FrameRunFirst + (int)(NPC.frameCounter / 7.0) % 6;
        }
        else
            frame = FrameIdle;
        NPC.frame.Y = frame * frameHeight;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) =>
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MistInsignia>()));

    // The second brother down raises the mission to A rank (once per world).
    public override void OnKill()
    {
        if (Partner != null || StoryWorld.DownedDemonBrothers)
            return;
        StoryWorld.DownedDemonBrothers = true;
        string text = Language.GetTextValue("Mods.ShinobiPrototype.Dialogue.DemonBrothersDown");
        if (Main.netMode == NetmodeID.Server)
        {
            ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(text), new Color(255, 190, 90));
            NetMessage.SendData(MessageID.WorldData);
        }
        else
            Main.NewText(text, 255, 190, 90);
    }

    // Gōzu draws the chain between the two gauntlets, flashing while the sweep is being telegraphed.
    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (!Leads || Partner is not NPC partner)
            return;
        Texture2D link = ModContent.Request<Texture2D>("ShinobiPrototype/Content/NPCs/DemonBrotherChain").Value;
        Vector2 a = Hand;
        Vector2 b = ((DemonBrother)partner.ModNPC).Hand;
        float length = Vector2.Distance(a, b);
        if (length < 4f)
            return;
        float rotation = (b - a).ToRotation();
        bool warn = State == ChainWarn;
        float sag = State == ChainRush || warn ? 0f : Math.Min(24f, length * 0.12f);
        int links = (int)(length / link.Width) + 1;
        for (int i = 0; i < links; i++)
        {
            float t = (i + 0.5f) / links;
            Vector2 at = Vector2.Lerp(a, b, t) + new Vector2(0f, sag * 4f * t * (1f - t));
            Color color = Lighting.GetColor((int)(at.X / 16f), (int)(at.Y / 16f));
            if (warn && (int)(Timer / 4f) % 2 == 0)
                color = Color.Lerp(color, new Color(255, 90, 70), 0.8f);
            spriteBatch.Draw(link, at - screenPos, null, color, rotation, link.Size() / 2f, 1f, SpriteEffects.None, 0f);
        }
    }
}

public sealed class DemonBrotherGozu : DemonBrother
{
    protected override int Side => -1;
    protected override int PartnerType => ModContent.NPCType<DemonBrotherMeizu>();
}

public sealed class DemonBrotherMeizu : DemonBrother
{
    protected override int Side => 1;
    protected override int PartnerType => ModContent.NPCType<DemonBrotherGozu>();
}
