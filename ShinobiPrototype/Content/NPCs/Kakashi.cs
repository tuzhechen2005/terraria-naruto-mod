using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items;

namespace ShinobiPrototype.Content.NPCs;

// Kakashi, the Leaf mentor: the only Naruto town NPC that turns up without a house (see KakashiSpawnSystem).
// His opening line carries the current objective; the buttons give tips and a substitution drill.
[AutoloadHead]
public sealed class Kakashi : ModNPC
{
    // Sheet layout, one frame per row, facing left like vanilla town NPCs.
    private const int IdleFrame = 0;
    private const int WalkFirst = 1;
    private const int WalkFrames = 6;
    private const int JumpFrame = 7;
    private const int SitFrame = 8;
    private const int ThrowFirst = 9;
    private const int ThrowFrames = 3;
    private const int FrameCount = 12;

    // Vanilla town AI states used for animation.
    private const float SittingState = 5f;
    private const float ThrowingState = 10f;

    private static int nextTip;

    // Substitution drill (local client only): the player being drilled, whether a kunai is being aimed,
    // and how long the release pose still shows.
    public int DrillTarget { get; set; } = -1;
    public bool DrillAiming { get; set; }
    public int DrillReleaseTicks { get; set; }

    public Vector2 DrillHand => NPC.Center + new Vector2(NPC.direction * 12f, -6f);

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = FrameCount;
        NPCID.Sets.DangerDetectRange[Type] = 500;
        NPCID.Sets.AttackType[Type] = 0;
        NPCID.Sets.AttackTime[Type] = 24;
        NPCID.Sets.AttackAverageChance[Type] = 20;
        NPCID.Sets.HatOffsetY[Type] = 4;
        SetHappiness();
    }

    public override void SetDefaults()
    {
        NPC.townNPC = true;
        NPC.friendly = true;
        NPC.width = 18;
        NPC.height = 40;
        NPC.aiStyle = NPCAIStyleID.Passive;
        NPC.damage = 10;
        NPC.defense = 15;
        NPC.lifeMax = 250;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.5f;
        NPC.scale = NpcSheet.KakashiScale;
    }

    public override bool CanTownNPCSpawn(int numTownNPCs) => true;

    // Likes the forest (like the Leaf), dislikes the desert; likes Tazuna, finds the Party Girl a bit loud.
    private void SetHappiness()
    {
        NPC.Happiness
            .SetBiomeAffection<Terraria.GameContent.Personalities.ForestBiome>(Terraria.GameContent.Personalities.AffectionLevel.Like)
            .SetBiomeAffection<Terraria.GameContent.Personalities.DesertBiome>(Terraria.GameContent.Personalities.AffectionLevel.Dislike)
            .SetNPCAffection<Tazuna>(Terraria.GameContent.Personalities.AffectionLevel.Like)
            .SetNPCAffection(NPCID.PartyGirl, Terraria.GameContent.Personalities.AffectionLevel.Dislike);
    }

    public override List<string> SetNPCNameList() => new() { this.GetLocalizedValue("GivenName") };

    public override string GetChat()
    {
        Player player = Main.LocalPlayer;
        string objective = player.GetModPlayer<StoryPlayer>().CurrentObjective();

        // The first time after the player met Orochimaru in the forest.
        ChuninExamPlayer exam = player.GetModPlayer<ChuninExamPlayer>();
        if (exam.OrochimaruDone && !exam.KakashiHeardOrochimaru)
        {
            exam.KakashiHeardOrochimaru = true;
            return Loc.Get("Kakashi.HeardOrochimaru");
        }

        // Characters made before the handbook existed (or who lost it) get one from him.
        int handbook = ModContent.ItemType<NinjaHandbook>();
        if (!player.HasItem(handbook))
        {
            player.QuickSpawnItem(NPC.GetSource_FromThis(), handbook);
            return Loc.Get("Kakashi.Handbook") + "\n\n" + objective;
        }

        // Worlds made before the bridge existed: hand over Tazuna's blueprint (again, if it was lost).
        int blueprint = ModContent.ItemType<BridgeBlueprint>();
        if (!WaveBridgeWorld.Site.HasValue && !player.HasItem(blueprint))
        {
            player.QuickSpawnItem(NPC.GetSource_FromThis(), blueprint);
            return Loc.Get("Kakashi.Blueprint") + "\n\n" + objective;
        }

        // Wave Country done: back to the Leaf, and the recommendation for the Chūnin Exams (M2 spec, section 2).
        int recommendation = ModContent.ItemType<ExamAdmissionScroll>();
        if (exam.Stage == ExamStage.Recommend)
        {
            exam.Recommend();
            player.QuickSpawnItem(NPC.GetSource_FromThis(), recommendation);
            return RecommendationLine + "\n\n" + player.GetModPlayer<StoryPlayer>().CurrentObjective();
        }
        if (exam.Stage == ExamStage.Written && !player.HasItem(recommendation))
        {
            player.QuickSpawnItem(NPC.GetSource_FromThis(), recommendation);
            return Loc.Get("Kakashi.LostRecommendation") + "\n\n" + objective;
        }
        int challenge = ModContent.ItemType<NejiChallengeScroll>();
        if (exam.Stage is ExamStage.Finals or ExamStage.Done && !player.HasItem(challenge))
        {
            player.QuickSpawnItem(NPC.GetSource_FromThis(), challenge);
            return Loc.Get("Kakashi.NejiChallenge") + "\n\n" + objective;
        }
        if (exam.Stage == ExamStage.NoVillage)
            return Loc.Get("Kakashi.NoVillage") + "\n\n" + objective;

        return Loc.Get($"Kakashi.Greeting{Main.rand.Next(1, GreetingCount + 1)}") + "\n\n" + objective;
    }

    private const int GreetingCount = 3;   // Kakashi.Greeting1..3

    public static string RecommendationLine => Loc.Get("Kakashi.Recommendation");

    // Once the Wave epilogue ends: the server (or single player) flickers him in beside the player nearest the bridge
    // who has not had the recommendation yet, wherever they have got to; each such player gets the line and the
    // recommendation on their client. (It used to wait for players within 200 tiles, and a player who flew off during
    // the epilogue never saw him; user, 2026-09-30.)
    public static void ArriveAfterEpilogue(Vector2 where)
    {
        int type = ModContent.NPCType<Kakashi>();
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            Player closest = null;
            foreach (Player player in Main.ActivePlayers)
                if (!player.dead && player.GetModPlayer<ChuninExamPlayer>().Stage == ExamStage.Recommend &&
                    (closest == null || player.Distance(where) < closest.Distance(where)))
                    closest = player;
            if (closest != null)
            {
                // On the ground under the player (who may be in the air), a step behind them.
                Vector2 beside = GroundBelow(closest.Bottom + new Vector2(-closest.direction * 48f, 0f));
                NPC kakashi = null;
                foreach (NPC npc in Main.ActiveNPCs)
                    if (npc.type == type)
                    {
                        kakashi = npc;
                        break;
                    }
                // Not in the world (fell in a fight, say): he comes anyway.
                if (kakashi == null)
                {
                    int index = NPC.NewNPC(new Terraria.DataStructures.EntitySource_WorldEvent(), (int)beside.X, (int)beside.Y, type);
                    kakashi = index < Main.maxNPCs ? Main.npc[index] : null;
                }
                if (kakashi != null)
                {
                    kakashi.Bottom = beside;
                    kakashi.velocity = Vector2.Zero;
                    kakashi.direction = kakashi.spriteDirection = closest.direction;
                    kakashi.netUpdate = true;
                }
            }
        }
        if (Main.dedServ)
            return;
        Player local = Main.LocalPlayer;
        ChuninExamPlayer exam = local.GetModPlayer<ChuninExamPlayer>();
        if (!local.active || exam.Stage != ExamStage.Recommend)
            return;
        exam.Recommend();
        local.QuickSpawnItem(local.GetSource_Misc("KakashiRecommendation"), ModContent.ItemType<ExamAdmissionScroll>());
        Main.NewText(Loc.Get("Kakashi.Says", RecommendationLine), new Color(200, 210, 230));
        // He opens the conversation himself once he stands beside the player: the line, with the ride home on a button
        // (user, 2026-09-30). Opening it the same tick he was moved broke the chat drawing.
        pendingTalkTicks = 20 * 60;
    }

    private static Vector2 GroundBelow(Vector2 at)
    {
        int x = (int)(at.X / 16f);
        for (int y = (int)(at.Y / 16f); y < Main.maxTilesY - 10 && y < at.Y / 16f + 60; y++)
            if (WorldGen.SolidOrSlopedTile(x, y))
                return new Vector2(at.X, y * 16f);
        return at;
    }

    private static int pendingTalkTicks;

    // Client side, each tick (KakashiSpawnSystem): open the pending talk once the player stands on the ground beside
    // him (a player in the air drifted out of range and the talk closed at once; user, 2026-09-30).
    internal static void UpdatePendingTalk()
    {
        if (pendingTalkTicks <= 0 || Main.dedServ)
            return;
        pendingTalkTicks--;
        Player local = Main.LocalPlayer;
        if (local.dead || local.velocity.Y != 0f)
            return;
        int type = ModContent.NPCType<Kakashi>();
        foreach (NPC npc in Main.ActiveNPCs)
            if (npc.type == type && npc.Distance(local.Center) < 8 * 16)
            {
                for (int i = 0; i < 20; i++)
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Smoke, 0f, -1f, 100, default, 1.4f);
                Main.playerInventory = false;
                Main.npcChatCornerItem = 0;
                local.SetTalkNPC(npc.whoAmI);
                Main.npcChatText = RecommendationLine;
                pendingTalkTicks = 0;
                return;
            }
    }

    public override void SetChatButtons(ref string button, ref string button2)
    {
        button = Loc.Get("Kakashi.ButtonTips");
        button2 = RideButton(Main.LocalPlayer) ?? LessonButton(Main.LocalPlayer) ?? Loc.Get("Kakashi.ButtonDrill");
    }

    // From the recommendation until the written test is passed he takes the player where the test is, in one Body
    // Flicker: home to the gate from afar (the bridge is a long walk from the village), or to the Academy from inside
    // the village (user, 2026-09-30: the button must still be there when asked again).
    private static string RideButton(Player player)
    {
        if (KonohaWorld.Site is not KonohaSite site || player.GetModPlayer<ChuninExamPlayer>().Stage != ExamStage.Written)
            return null;
        return Loc.Get(FarFromVillage(player, site) ? "Kakashi.ButtonHome" : "Kakashi.ButtonAcademy");
    }

    private static bool OffersRideHome(Player player) => RideButton(player) != null;

    // His three lessons (KakashiLessonPlayer), once Tazuna has come clean; the substitution drill once they are done.
    private static string LessonButton(Player player)
    {
        KakashiLessonPlayer lessons = player.GetModPlayer<KakashiLessonPlayer>();
        return KakashiLessonPlayer.Open && lessons.Next != KakashiLesson.Done
            ? Loc.Get("Kakashi.ButtonLesson", Loc.Get($"Lesson.{lessons.Next}.Name"))
            : null;
    }

    private static bool FarFromVillage(Player player, KonohaSite site) =>
        System.Math.Abs(player.Center.X / 16f - site.CenterX) > KonohaDesign.HalfWidth + 60;

    private void RideHome()
    {
        if (KonohaWorld.Site is not KonohaSite site)
            return;
        Player player = Main.LocalPlayer;
        NpcChatCloser.CloseNextTick();
        bool far = FarFromVillage(Main.LocalPlayer, site);
        // From afar: the gate. Inside the village: the street just west of the Academy, by its door.
        int academy = KonohaWorld.Design.Buildings.FindIndex(b => b.Name == KonohaBuildings.Academy);
        Vector2 gate = far || academy < 0
            ? new(site.CenterX * 16f + 8f, site.GroundY * 16f)
            : new((site.X(KonohaWorld.Design.Buildings[academy].X0) - 2 + 0.5f) * 16f, site.GroundY * 16f);
        for (int i = 0; i < 30; i++)
            Dust.NewDust(player.position, player.width, player.height, DustID.Smoke, 0f, -1f, 100, default, 1.6f);
        player.Teleport(gate - new Vector2(player.width / 2f, player.height), TeleportationStyleID.RecallPotion);
        if (Main.netMode == NetmodeID.MultiplayerClient)
            NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, gate.X - player.width / 2f,
                gate.Y - player.height, TeleportationStyleID.RecallPotion);
        else
        {
            NPC.Bottom = gate + new Vector2(-player.direction * 48f, 0f);
            NPC.velocity = Vector2.Zero;
        }
        Main.NewText(Loc.Get(far ? "Kakashi.RideHome" : "Kakashi.RideAcademy"), new Color(200, 210, 230));
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
        if (firstButton)
        {
            string[] tips = Tips();
            Main.npcChatText = tips[nextTip];
            nextTip = (nextTip + 1) % tips.Length;
            return;
        }

        if (OffersRideHome(Main.LocalPlayer))
            RideHome();
        else if (LessonButton(Main.LocalPlayer) != null)
            Main.npcChatText = Main.LocalPlayer.GetModPlayer<KakashiLessonPlayer>().Ask();
        else
            StartPractice();
    }

    private const int TipCount = 7;   // Kakashi.Tip1..7; the first names the substitution key

    private static string[] Tips()
    {
        string[] tips = new string[TipCount];
        for (int i = 0; i < TipCount; i++)
            tips[i] = Loc.Get($"Kakashi.Tip{i + 1}", ShinobiKeybinds.SubstitutionKeyName());
        return tips;
    }

    private void StartPractice()
    {
        Player player = Main.LocalPlayer;
        NpcChatCloser.CloseNextTick();
        CombatText.NewText(NPC.getRect(), Color.White, Loc.Get("Kakashi.DrillStart"));
        player.GetModPlayer<SubstitutionDrillPlayer>().Start(NPC.whoAmI);
    }

    // During a drill he stops wandering and faces the player; gravity and collision still run outside the AI.
    public override bool PreAI()
    {
        // The lake ambush: he stands by the player, is held in the water prison, then stays for the ending.
        if (WaterPrison.KakashiScene() is WaterPrison scene)
        {
            NPC.velocity = Vector2.Zero;
            NPC.direction = scene.KakashiFacing;
            NPC.dontTakeDamage = scene.KakashiHold(NPC).HasValue;
            if (scene.KakashiHold(NPC) is Vector2 feet)
                NPC.Bottom = feet;
            return false;
        }
        NPC.dontTakeDamage = false;

        if (DrillReleaseTicks > 0)
            DrillReleaseTicks--;
        if (DrillTarget < 0 || !Main.player[DrillTarget].active)
            return true;

        NPC.velocity.X *= 0.8f;
        NPC.direction = Main.player[DrillTarget].Center.X >= NPC.Center.X ? 1 : -1;
        return false;
    }

    // Inside the prison he is drawn by the sphere, in his trapped pose.
    public override bool PreDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) =>
        WaterPrison.KakashiScene()?.KakashiHold(NPC) is null;

    public override void TownNPCAttackStrength(ref int damage, ref float knockback)
    {
        damage = 14;
        knockback = 3f;
    }

    public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
    {
        cooldown = 20;
        randExtraCooldown = 10;
    }

    public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
    {
        projType = ProjectileID.ThrowingKnife;
        attackDelay = 8;
    }

    public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
    {
        multiplier = 11f;
        gravityCorrection = 0f;
        randomOffset = 0.5f;
    }

    public override void FindFrame(int frameHeight)
    {
        // Overriding FindFrame skips vanilla's town-NPC framing, which is also what turns the sprite to face
        // the way he walks; without this he always faces left and walks backwards when heading right.
        NPC.spriteDirection = NPC.direction;

        int frame;
        if (DrillAiming)
            frame = ThrowFirst;
        else if (DrillReleaseTicks > 0)
            frame = ThrowFirst + (DrillReleaseTicks > 8 ? 1 : 2);
        else if (NPC.ai[0] == SittingState)
            frame = SitFrame;
        else if (NPC.ai[0] == ThrowingState)
        {
            float progress = 1f - NPC.ai[1] / Math.Max(1, NPCID.Sets.AttackTime[Type]);
            frame = ThrowFirst + Math.Clamp((int)(progress * ThrowFrames), 0, ThrowFrames - 1);
        }
        else if (NPC.velocity.Y != 0f)
            frame = JumpFrame;
        else if (Math.Abs(NPC.velocity.X) > 0.1f)
        {
            NPC.frameCounter += Math.Abs(NPC.velocity.X);
            frame = WalkFirst + (int)(NPC.frameCounter / 8.0) % WalkFrames;
        }
        else
        {
            NPC.frameCounter = 0;
            frame = IdleFrame;
        }

        NPC.frame.Y = frame * frameHeight;
    }
}
