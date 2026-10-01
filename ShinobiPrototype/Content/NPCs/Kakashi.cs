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
        // A touch larger than vanilla town NPCs, so the mentor stands out; drawing stays anchored at the feet.
        NPC.scale = 1.1f;
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
        string[] greetings =
        {
            "哟。",
            "……（合上手里的橙色小书）哟。",
            "抱歉来晚了，我在人生的道路上迷路了。",
        };
        Player player = Main.LocalPlayer;
        string objective = player.GetModPlayer<StoryPlayer>().CurrentObjective();

        // Characters made before the handbook existed (or who lost it) get one from him.
        int handbook = ModContent.ItemType<NinjaHandbook>();
        if (!player.HasItem(handbook))
        {
            player.QuickSpawnItem(NPC.GetSource_FromThis(), handbook);
            return "哟。你的忍者手册呢？……拿着，别再弄丢了。任务、忍术、查克拉的事都记在里面。\n\n" + objective;
        }

        // Worlds made before the bridge existed: hand over Tazuna's blueprint (again, if it was lost).
        int blueprint = ModContent.ItemType<BridgeBlueprint>();
        if (!WaveBridgeWorld.Site.HasValue && !player.HasItem(blueprint))
        {
            player.QuickSpawnItem(NPC.GetSource_FromThis(), blueprint);
            return "哟。造桥的达兹纳托我把这个交给你——他的施工图。这片海边还没有他的桥。" +
                   "到海滩上面朝大海用一次，看看轮廓；没问题的话原地再用一次，桥就立起来了。\n\n" + objective;
        }

        // Wave Country done: back to the Leaf, and the recommendation for the Chūnin Exams (M2 spec, section 2).
        ChuninExamPlayer exam = player.GetModPlayer<ChuninExamPlayer>();
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
            return "推荐书弄丢了？……拿着，这是补的。别再丢了。\n\n" + objective;
        }
        int challenge = ModContent.ItemType<NejiChallengeScroll>();
        if (exam.Stage is ExamStage.Finals or ExamStage.Done && !player.HasItem(challenge))
        {
            player.QuickSpawnItem(NPC.GetSource_FromThis(), challenge);
            return "日向家的那个孩子——宁次，托我带句话：正式赛开始以后，他想在会场和你切磋一场。" +
                   "去不去随你。\n\n" + objective;
        }
        if (exam.Stage == ExamStage.NoVillage)
            return "辛苦了。……本来该回木叶了，可这片土地上没有木叶。中忍考试只能在有木叶的世界里参加。\n\n" + objective;

        return $"{Main.rand.Next(greetings)}\n\n{objective}";
    }

    public const string RecommendationLine =
        "哟，辛苦了。……好了，回村子吧。啊对了——中忍考试，我把你的名字报上去了。嘛……要是怕了，现在后悔也还来得及。";

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
                Vector2 beside = closest.Bottom + new Vector2(-closest.direction * 48f, 0f);
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
        Main.NewText("卡卡西：" + RecommendationLine, new Color(200, 210, 230));
        // He opens the conversation himself once he stands beside the player: the line, with the ride home on a button
        // (user, 2026-09-30). Opening it the same tick he was moved broke the chat drawing.
        pendingTalkTicks = 300;
    }

    private static int pendingTalkTicks;

    // Client side, each tick (KakashiSpawnSystem): open the pending talk when he is close enough to be talked to.
    internal static void UpdatePendingTalk()
    {
        if (pendingTalkTicks <= 0 || Main.dedServ)
            return;
        pendingTalkTicks--;
        Player local = Main.LocalPlayer;
        int type = ModContent.NPCType<Kakashi>();
        foreach (NPC npc in Main.ActiveNPCs)
            if (npc.type == type && npc.Distance(local.Center) < 12 * 16)
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
        button = "指点";
        button2 = OffersRideHome(Main.LocalPlayer) ? "回村" : "练习替身术";
    }

    // Right after Wave Country, far from the Leaf, he takes the player home in one Body Flicker (the bridge is a long
    // walk from the village; user, 2026-09-30). Once, until the written test is passed.
    private static bool OffersRideHome(Player player) =>
        KonohaWorld.Site is KonohaSite site && player.GetModPlayer<ChuninExamPlayer>().Stage == ExamStage.Written &&
        System.Math.Abs(player.Center.X / 16f - site.CenterX) > KonohaDesign.HalfWidth + 60;

    private void RideHome()
    {
        if (KonohaWorld.Site is not KonohaSite site)
            return;
        Player player = Main.LocalPlayer;
        player.SetTalkNPC(-1);
        Main.npcChatText = "";
        Vector2 gate = new(site.CenterX * 16f + 8f, site.GroundY * 16f);
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
        Main.NewText("卡卡西：……瞬身之术。到了——欢迎回来。", new Color(200, 210, 230));
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
        else
            StartPractice();
    }

    private static string[] Tips()
    {
        string key = ShinobiKeybinds.SubstitutionKeyName();
        return new[]
        {
            $"替身术：在挨打的前一刻按【{key}】，留下一截木头，人已经在别处了。耗 20 查克拉，之后要等 4 秒才能再用。按早了就白费——想练的话，点“练习替身术”。",
            "查克拉会自己慢慢恢复，刚用完术的几秒会慢一些。打中敌人也能回一点，不管你用刀、弓、魔法还是召唤物。",
            "蘑菇配太阳花，在工作台能做兵粮丸，一口回 40 查克拉。应急用的，别想着连着吃。",
            "地下洞穴里有发蓝光的查克拉结晶，就像生命水晶那样，用了能让查克拉上限变高。下矿的时候留意一下。",
            "雾隐的人在离村子远的地方活动，海边尤其多。拿到他们的标记，就能追到再不斩。",
            "水牢术困住的人，从里面是破不开的。要是哪天我被关进去了——从外面打。",
            "我说过的这些，忍者手册里都记着，随时翻。",
        };
    }

    private void StartPractice()
    {
        Player player = Main.LocalPlayer;
        player.SetTalkNPC(-1);
        Main.npcChatText = "";
        CombatText.NewText(NPC.getRect(), Color.White, "看好了——");
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
