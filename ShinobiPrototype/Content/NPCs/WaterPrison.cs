using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items;

namespace ShinobiPrototype.Content.NPCs;

// The lake ambush (M1 spec, "湖边初遇"), run by the water prison itself: Zabuza rises on the lake, Kakashi steps up
// beside the player and is caught in the prison, water clones harass the player, and breaking the sphere frees him;
// then senbon drop Zabuza and the masked hunter-nin (Haku) carries him off. Zabuza (who cannot be hit), Haku and the
// trapped Kakashi are drawn by this NPC; the town Kakashi is held inside the sphere meanwhile (see Kakashi.PreAI).
// Every client runs the same timeline from the synced phase and tick; the server decides phase changes.
public sealed class WaterPrison : ModNPC
{
    public const int Radius = 44;
    private const int Intro = 0;
    private const int Fight = 1;
    private const int Outro = 2;

    private static readonly Color ZabuzaVoice = new(160, 200, 230);
    private static readonly Color KakashiVoice = new(210, 220, 235);
    private static readonly Color HakuVoice = new(175, 240, 255);

    private ref float Phase => ref NPC.ai[0];
    private ref float Tick => ref NPC.ai[1];
    // Zabuza stands on the far side of the sphere from the player who found the lake.
    private int Side => NPC.ai[2] >= 0f ? 1 : -1;
    private float WaterY => NPC.ai[3];
    private ref float CloneTimer => ref NPC.localAI[0];

    // Everything is drawn in PreDraw; the sphere's first frame stands in as the registered texture.
    public override string Texture => "ShinobiPrototype/Content/NPCs/WaterSphere_0";

    public bool Holding => Phase == Fight || (Phase == Intro && Tick >= StoryRules.PrisonFormed);

    // Standing on the lake's real surface (WaterY is the top water tile's edge).
    private Vector2 ZabuzaBottom => OnWater(NPC.Center.X + Side * (Radius + 46f));
    private Vector2 HakuBottom => OnWater(NPC.Center.X + Side * (Radius + 86f));

    private Vector2 OnWater(float x) => new(x, LiquidSurface.StandY(x, WaterY));

    public override void SetStaticDefaults()
    {
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers { Hide = true });
        NPCID.Sets.MPAllowedEnemies[Type] = true;
    }

    public override void SetDefaults()
    {
        NPC.width = Radius * 2;
        NPC.height = Radius * 2;
        NPC.aiStyle = -1;
        NPC.lifeMax = StoryRules.PrisonLife;
        NPC.defense = StoryRules.PrisonDefense;
        NPC.damage = 0;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.dontTakeDamage = true;
        NPC.boss = true;
        NPC.npcSlots = 5f;
        NPC.HitSound = SoundID.Item21;
        NPC.value = 0f;
    }

    public override bool CheckActive() => false;

    public override bool? CanBeHitByProjectile(Projectile projectile) => Holding ? null : false;

    public override bool? CanBeHitByItem(Player player, Item item) => Holding ? null : false;

    public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
    {
        NPC.velocity = Vector2.Zero;
        NPC.life = NPC.lifeMax;
    }

    // The sphere bursting starts the ending instead of killing the director.
    public override bool CheckDead()
    {
        if (Phase != Fight)
            return false;
        NPC.life = 1;
        NPC.dontTakeDamage = true;
        Phase = Outro;
        Tick = 0f;
        NPC.netUpdate = true;
        return false;
    }

    public override void AI()
    {
        NPC.velocity = Vector2.Zero;
        NPC.dontTakeDamage = !Holding;
        bool server = Main.netMode != NetmodeID.MultiplayerClient;
        int tick = (int)Tick;

        if (server && Phase != Outro && !AnyoneNear())
        {
            Retreat();
            return;
        }

        if (Phase == Intro)
            UpdateIntro(tick, server);
        else if (Phase == Fight)
            UpdateFight(server);
        else
            UpdateOutro(tick, server);

        if (NPC.active)
            Tick++;
    }

    private bool AnyoneNear()
    {
        foreach (Player player in Main.ActivePlayers)
            if (!player.dead && player.Distance(NPC.Center) < StoryRules.AbortRangeTiles * 16f)
                return true;
        return false;
    }

    private void UpdateIntro(int tick, bool server)
    {
        switch (tick)
        {
            case StoryRules.IntroZabuzaAppear:
                Splash(ZabuzaBottom, 30);
                break;
            case StoryRules.IntroZabuzaLine:
                Say(ZabuzaBottom, "LakeZabuza1", ZabuzaVoice);
                break;
            case StoryRules.IntroKakashiArrive:
                if (server)
                    MoveKakashi(BesideNearestPlayer());
                break;
            case StoryRules.IntroKakashiLine:
                Say(KakashiSpot(), "LakeKakashi1", KakashiVoice);
                break;
            case StoryRules.IntroPrisonCall:
                Say(ZabuzaBottom, "LakeZabuza2", ZabuzaVoice);
                break;
            case StoryRules.PrisonFormed:
                Splash(NPC.Center, 50);
                SoundEngine.PlaySound(SoundID.Splash, NPC.Center);
                break;
            case StoryRules.IntroKakashiTrapped:
                Say(NPC.Center, "LakeKakashi2", KakashiVoice);
                break;
            case StoryRules.IntroClonesCall:
                Say(ZabuzaBottom, "LakeZabuza3", ZabuzaVoice);
                break;
            case StoryRules.ClonesFormed:
                if (server)
                {
                    for (int i = 0; i < StoryRules.CloneCount; i++)
                        SpawnClone(i);
                    Phase = Fight;
                    Tick = 0f;
                    NPC.netUpdate = true;
                }
                if (!Main.dedServ)
                    Main.NewText(Language.GetTextValue("Mods.ShinobiPrototype.Dialogue.LakeGoal"), 255, 190, 90);
                break;
        }
    }

    private void UpdateFight(bool server)
    {
        if (!server)
            return;
        int clones = 0;
        foreach (NPC npc in Main.ActiveNPCs)
            if (npc.type == ModContent.NPCType<WaterClone>() && (int)npc.ai[3] == NPC.whoAmI)
                clones++;
        if (clones >= StoryRules.CloneCount)
        {
            CloneTimer = StoryRules.CloneReformTicks;
            return;
        }
        if (--CloneTimer <= 0f)
        {
            SpawnClone(clones);
            CloneTimer = StoryRules.CloneReformTicks;
        }
    }

    private void UpdateOutro(int tick, bool server)
    {
        switch (tick)
        {
            case 1:
                Splash(NPC.Center, 80);
                SoundEngine.PlaySound(SoundID.Splash, NPC.Center);
                SoundEngine.PlaySound(SoundID.Item96, NPC.Center);
                if (server)
                {
                    DissolveClones();
                    MoveKakashi(BesideNearestPlayer());
                }
                break;
            case StoryRules.OutroKakashiLine:
                Say(KakashiSpot(), "LakeKakashi3", KakashiVoice);
                break;
            case StoryRules.OutroZabuzaLine:
                Say(ZabuzaBottom, "LakeZabuza4", ZabuzaVoice);
                break;
            case StoryRules.OutroZabuzaFalls:
                Splash(ZabuzaBottom, 20);
                break;
            case StoryRules.OutroHakuAppear:
                MistPuff(HakuBottom);
                break;
            case StoryRules.OutroHakuLine:
                Say(HakuBottom, "LakeHaku", HakuVoice);
                break;
            case StoryRules.OutroVanish:
                MistPuff(HakuBottom);
                MistPuff(ZabuzaBottom);
                break;
            case StoryRules.OutroKakashiDeduce1:
                Say(KakashiSpot(), "LakeKakashi4", KakashiVoice);
                break;
            case StoryRules.OutroKakashiDeduce2:
                Say(KakashiSpot(), "LakeKakashi5", KakashiVoice);
                break;
        }
        if (tick >= StoryRules.OutroLength && server)
            Finish();
    }

    private void Finish()
    {
        StoryWorld.LakeDone = true;
        Player player = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)];
        var source = NPC.GetSource_FromThis();
        Item.NewItem(source, player.getRect(), ItemID.SilverCoin, 50);
        Item.NewItem(source, player.getRect(), ModContent.ItemType<ChakraPill>(), 3);
        NPC.active = false;
        if (Main.netMode == NetmodeID.Server)
        {
            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
            NetMessage.SendData(MessageID.WorldData);
        }
    }

    // Everyone near is dead or gone: Zabuza melts into the lake and the prison breaks; it can happen again later.
    private void Retreat()
    {
        DissolveClones();
        Broadcast("LakeRetreat", ZabuzaVoice);
        Broadcast("LakeRetreatEnd", new Color(190, 200, 210));
        LakeAmbushSystem.Retreated();
        NPC.active = false;
        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
    }

    private void SpawnClone(int index)
    {
        Vector2 at = ZabuzaBottom + new Vector2(-Side * (index + 1) * 30f, -2f);
        int clone = NPC.NewNPC(NPC.GetSource_FromThis(), (int)at.X, (int)at.Y, ModContent.NPCType<WaterClone>(),
            0, 0f, 0f, 0f, NPC.whoAmI);
        if (clone < Main.maxNPCs)
            Main.npc[clone].netUpdate = true;
    }

    private void DissolveClones()
    {
        foreach (NPC npc in Main.ActiveNPCs)
        {
            if (npc.type != ModContent.NPCType<WaterClone>())
                continue;
            npc.active = false;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
        }
    }

    // --- Kakashi: the town NPC himself steps in, is held in the sphere, and is let out beside the player.

    // The prison running a scene Kakashi is part of: from his arrival beside the player to the end of the ending.
    public static WaterPrison KakashiScene()
    {
        foreach (NPC npc in Main.ActiveNPCs)
            if (npc.ModNPC is WaterPrison prison &&
                (prison.Phase != Intro || prison.Tick >= StoryRules.IntroKakashiArrive))
                return prison;
        return null;
    }

    // While the sphere holds him, Kakashi's feet go at its centre so his body sits inside it.
    public Vector2? KakashiHold(NPC kakashi) => Holding ? NPC.Center + new Vector2(0f, kakashi.height / 2f) : null;

    public int KakashiFacing => Side;

    private Vector2 BesideNearestPlayer()
    {
        Player player = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)];
        return player.Bottom + new Vector2(Side * 28f, 0f);
    }

    private static void MoveKakashi(Vector2 bottom)
    {
        int index = NPC.FindFirstNPC(ModContent.NPCType<Kakashi>());
        if (index < 0)
            return;
        NPC kakashi = Main.npc[index];
        MistPuff(kakashi.Bottom);
        kakashi.Bottom = bottom;
        kakashi.velocity = Vector2.Zero;
        kakashi.netUpdate = true;
        MistPuff(bottom);
    }

    private Vector2 KakashiSpot()
    {
        int index = NPC.FindFirstNPC(ModContent.NPCType<Kakashi>());
        return index >= 0 ? Main.npc[index].Bottom : NPC.Center;
    }

    // --- Lines and effects

    private static void Say(Vector2 bottom, string key, Color color)
    {
        if (Main.dedServ)
            return;
        string text = Language.GetTextValue($"Mods.ShinobiPrototype.Dialogue.{key}");
        int colon = text.IndexOf('：') >= 0 ? text.IndexOf('：') : text.IndexOf(':');
        CombatText.NewText(new Rectangle((int)bottom.X - 20, (int)bottom.Y - 90, 40, 90), color,
            colon >= 0 ? text[(colon + 1)..].Trim() : text, dramatic: true);
        Main.NewText(text, color);
    }

    private static void Broadcast(string key, Color color)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            Main.NewText(Language.GetTextValue($"Mods.ShinobiPrototype.Dialogue.{key}"), color);
        else
            Loc.Broadcast(color, $"Mods.ShinobiPrototype.Dialogue.{key}");
    }

    private static void Splash(Vector2 at, int count)
    {
        if (Main.dedServ)
            return;
        for (int i = 0; i < count; i++)
        {
            Dust drop = Dust.NewDustPerfect(at + Main.rand.NextVector2Circular(Radius, Radius), DustID.Water,
                Main.rand.NextVector2Circular(4f, 4f) - new Vector2(0f, 2f), 0, default, 1.4f);
            drop.noGravity = false;
        }
    }

    private static void MistPuff(Vector2 bottom)
    {
        if (Main.dedServ)
            return;
        for (int i = 0; i < 24; i++)
            Dust.NewDustPerfect(bottom + new Vector2(Main.rand.NextFloat(-20f, 20f), -Main.rand.NextFloat(0f, 60f)),
                DustID.Smoke, Main.rand.NextVector2Circular(1.4f, 1.4f), 100, new Color(210, 220, 230), 1.8f).noGravity = true;
    }

    // --- Drawing: Zabuza (and Haku) on the water, Kakashi inside the sphere, the sphere, the senbon.

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        int tick = (int)Tick;
        int facing = -Side;
        Color zabuzaLight = BossSprites.Lit(Lighting.GetColor((int)(ZabuzaBottom.X / 16f), (int)(ZabuzaBottom.Y / 16f) - 3));

        bool zabuzaShown = Phase == Fight || (Phase == Intro && tick >= StoryRules.IntroZabuzaAppear) ||
                           (Phase == Outro && tick < StoryRules.OutroVanish);
        if (zabuzaShown)
        {
            if (Phase == Outro && tick >= StoryRules.OutroZabuzaFalls)
                DrawLying(spriteBatch, screenPos, zabuzaLight);
            else if (Holding && (BossSprites.TryDraw(spriteBatch, "Zabuza", "Hold", BossSprites.Loop(20f, 2), 2,
                         BossSprites.Zabuza, ZabuzaBottom, facing, zabuzaLight, screenPos) ||
                     BossSprites.TryDraw(spriteBatch, "Zabuza", "Seal", 2, 3, BossSprites.Zabuza, ZabuzaBottom, facing,
                         zabuzaLight, screenPos)))
            {
            }
            else
                BossSprites.TryDraw(spriteBatch, "Zabuza", "Idle", BossSprites.Loop(8f, 4), 4, BossSprites.Zabuza,
                    ZabuzaBottom, facing, zabuzaLight, screenPos);
        }

        if (Phase == Outro && tick >= StoryRules.OutroHakuAppear && tick < StoryRules.OutroVanish)
        {
            Color hakuLight = BossSprites.Lit(Lighting.GetColor((int)(HakuBottom.X / 16f), (int)(HakuBottom.Y / 16f) - 2));
            BossSprites.TryDraw(spriteBatch, "Haku", "Idle", BossSprites.Loop(8f, 4), 4, BossSprites.Haku, HakuBottom,
                facing, hakuLight, screenPos);
        }

        if (Phase == Outro && tick >= StoryRules.OutroSenbon && tick < StoryRules.OutroZabuzaFalls)
            DrawSenbon(spriteBatch, screenPos, (tick - StoryRules.OutroSenbon) /
                (float)(StoryRules.OutroZabuzaFalls - StoryRules.OutroSenbon));

        if (Holding)
        {
            DrawTrappedKakashi(spriteBatch, screenPos);
            DrawSphere(spriteBatch, screenPos);
        }
        return false;
    }

    // Felled by the needles: the epilogue's collapse (wave-epilogue-walk-v2), so his fall matches the bridge's.
    private void DrawLying(SpriteBatch spriteBatch, Vector2 screenPos, Color light)
    {
        int fallen = (int)Tick - StoryRules.OutroZabuzaFalls;
        if (BossSprites.TryDraw(spriteBatch, "Zabuza", "Collapse", Math.Min(2, fallen / 8), 3, BossSprites.Zabuza,
                ZabuzaBottom, -Side, light, screenPos))
            return;
        const string path = "ShinobiPrototype/Content/NPCs/Zabuza_Lying";
        if (!ModContent.HasAsset(path))
            return;
        Texture2D body = ModContent.Request<Texture2D>(path).Value;
        spriteBatch.Draw(body, ZabuzaBottom - screenPos, null, light, 0f, new Vector2(body.Width / 2f, body.Height - 2),
            1f, Side > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
    }

    // Two needles from the treeline on the far side, into Zabuza's neck.
    private void DrawSenbon(SpriteBatch spriteBatch, Vector2 screenPos, float progress)
    {
        Texture2D needle = ModContent.Request<Texture2D>("ShinobiPrototype/Content/Projectiles/HakuSenbon").Value;
        Vector2 neck = ZabuzaBottom + new Vector2(0f, -78f);
        for (int i = 0; i < 2; i++)
        {
            Vector2 from = neck + new Vector2(Side * 420f, -160f - i * 30f);
            Vector2 at = Vector2.Lerp(from, neck + new Vector2(0f, i * 6f), progress);
            float rotation = (neck - from).ToRotation();
            spriteBatch.Draw(needle, at - screenPos, null, Color.White, rotation, needle.Size() / 2f, 1f,
                SpriteEffects.None, 0f);
        }
    }

    private void DrawTrappedKakashi(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Color light = BossSprites.Lit(Lighting.GetColor((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f)), 0.4f);
        Vector2 bob = new(0f, (float)Math.Sin(Main.GameUpdateCount / 20f) * 2f);
        int frame = BossSprites.Loop(24f, 2);
        string trapped = $"ShinobiPrototype/Content/NPCs/Kakashi_Trapped_{frame}";
        SpriteEffects flip = Side > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        if (ModContent.HasAsset(trapped))
        {
            Texture2D tex = ModContent.Request<Texture2D>(trapped).Value;
            spriteBatch.Draw(tex, NPC.Center + bob - screenPos, null, light, 0f, tex.Size() / 2f, NpcSheet.ScaleFor(NpcSheet.KakashiBody), flip, 0f);
            return;
        }
        // Without the trapped frames (kakashi-trapped-v2): his jump frame from the town sheet, tilted.
        Texture2D sheet = TextureAssets.Npc[ModContent.NPCType<Kakashi>()].Value;
        int height = sheet.Height / 12;
        Rectangle source = new(0, 7 * height, sheet.Width, height);
        spriteBatch.Draw(sheet, NPC.Center + bob - screenPos, source, light, 0.25f * -Side, source.Size() / 2f, NpcSheet.ScaleFor(NpcSheet.KakashiBody),
            flip, 0f);
    }

    private void DrawSphere(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        float flash = NPC.justHit ? 1.4f : 1f;
        string path = $"ShinobiPrototype/Content/NPCs/WaterSphere_{BossSprites.Loop(10f, 3)}";
        Color tint = BossSprites.Lit(Lighting.GetColor((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f)), 0.5f);
        if (ModContent.HasAsset(path))
        {
            Texture2D tex = ModContent.Request<Texture2D>(path).Value;
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, tint * flash, 0f, tex.Size() / 2f, 1f,
                SpriteEffects.None, 0f);
            return;
        }
        // Placeholder: vanilla's bubble, stretched to the sphere.
        Main.instance.LoadProjectile(ProjectileID.Bubble);
        Texture2D bubble = TextureAssets.Projectile[ProjectileID.Bubble].Value;
        float scale = Radius * 2f / bubble.Width * (1f + 0.03f * (float)Math.Sin(Main.GameUpdateCount / 12f));
        spriteBatch.Draw(bubble, NPC.Center - screenPos, null, new Color(120, 190, 255) * 0.75f * flash, 0f,
            bubble.Size() / 2f, scale, SpriteEffects.None, 0f);
    }
}
