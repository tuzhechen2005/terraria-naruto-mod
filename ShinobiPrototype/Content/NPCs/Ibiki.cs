using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items;

namespace ShinobiPrototype.Content.NPCs;

// Morino Ibiki, proctor of the written test (specs/M2_中忍考试篇.spec.md 3.1): he stands in an Academy classroom and
// the test starts by talking to him, with Kakashi's recommendation in hand (user, 2026-09-30: using the scroll on
// its own felt strange). A story NPC like the Third Hokage: no house, cannot be hurt. Art: ibiki-npc-v1, the twelve
// NpcSheet frames plus arms folded and a pointing announcement (shown while someone talks to him).
public sealed class Ibiki : ModNPC
{
    private const int FoldedFrame = NpcSheet.FrameCount;
    private const int PointFrame = NpcSheet.FrameCount + 1;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = NpcSheet.FrameCount + 2;
        NPCID.Sets.NoTownNPCHappiness[Type] = true;
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers { Hide = true });
    }

    public override void SetDefaults()
    {
        NPC.width = 18;
        NPC.height = 40;
        NPC.aiStyle = -1;
        NPC.friendly = true;
        NPC.dontTakeDamage = true;
        NPC.lifeMax = 250;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
    }

    public override bool CanChat() => true;

    public override bool CheckActive() => false;

    public override void AI()
    {
        if (KonohaWorld.IbikiFeet is Vector2 feet)
        {
            NPC.velocity = Vector2.Zero;
            NPC.Bottom = feet;
        }
        Player nearest = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)];
        NPC.direction = NPC.spriteDirection = nearest.Center.X >= NPC.Center.X ? 1 : -1;
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.spriteDirection = NPC.direction;
        NPC.frame.Y = (Main.LocalPlayer.talkNPC == NPC.whoAmI ? PointFrame : FoldedFrame) * frameHeight;
    }

    public override string GetChat()
    {
        ChuninExamPlayer exam = Main.LocalPlayer.GetModPlayer<ChuninExamPlayer>();
        return exam.Stage switch
        {
            ExamStage.Written when !exam.CanSitWritten => "放弃过一次的人，今天别再来了。明天天亮，想清楚了再说。",
            ExamStage.Written when !Main.LocalPlayer.HasItem(ModContent.ItemType<ExamAdmissionScroll>()) =>
                "推荐书呢？没有上忍的推荐，谁也进不了这间考场。去找你的带队上忍。",
            ExamStage.Written => "我是第一试的主考官，森乃伊比喜。\n\n规矩只有一条：作弊被发现两次以上，当场出局。……准备好了就坐下。",
            ExamStage.Recommend or ExamStage.Locked or ExamStage.NoVillage => "……这里是中忍考试的考场。还没轮到你。",
            _ => "第一试已经过了吧。死亡森林可比这间教室危险得多——别死了。",
        };
    }

    public override void SetChatButtons(ref string button, ref string button2)
    {
        ChuninExamPlayer exam = Main.LocalPlayer.GetModPlayer<ChuninExamPlayer>();
        if (exam.Stage == ExamStage.Written && exam.CanSitWritten &&
            Main.LocalPlayer.HasItem(ModContent.ItemType<ExamAdmissionScroll>()))
            button = "开始笔试";
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
        if (!firstButton)
            return;
        Main.LocalPlayer.SetTalkNPC(-1);
        Main.npcChatText = "";
        CombatText.NewText(NPC.getRect(), new Color(220, 220, 220), "——开始！", true);
        WrittenExamSystem.Open();
    }
}
