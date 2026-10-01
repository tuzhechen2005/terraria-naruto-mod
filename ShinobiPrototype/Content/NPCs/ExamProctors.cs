using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Common.Systems;

namespace ShinobiPrototype.Content.NPCs;

// The proctors of the second test and the prelims (specs/M2_中忍考试篇.spec.md 3.2–3.3; user, 2026-10-01): story NPCs
// like Ibiki, standing at their post (no house, cannot be hurt). Art: the twelve NpcSheet frames plus two of their own
// (anko-npc-v1, hayate-npc-v1, drawn at their final size); without it they borrow Kakashi's sheet.
public abstract class ExamProctor : ModNPC
{
    private const int PoseFrame = NpcSheet.FrameCount;
    private const int TalkFrame = NpcSheet.FrameCount + 1;

    private bool OwnArt => ModContent.HasAsset(base.Texture);

    public override string Texture => ModContent.HasAsset(base.Texture) ? base.Texture : "ShinobiPrototype/Content/NPCs/Kakashi";

    protected abstract Vector2? Feet { get; }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = OwnArt ? NpcSheet.FrameCount + 2 : NpcSheet.FrameCount;
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
        // Their own art is drawn at its final size (64-pixel frames, shown at 1x); Kakashi's stand-in sheet is scaled.
        NPC.scale = OwnArt ? 1f : NpcSheet.ScaleFor(NpcSheet.KakashiBody);
    }

    public override bool CanChat() => true;

    public override bool CheckActive() => false;

    public override void AI()
    {
        if (Feet is Vector2 feet)
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
        int frame = NpcSheet.IdleFrame;
        if (OwnArt)
            frame = Main.LocalPlayer.talkNPC == NPC.whoAmI ? TalkFrame
                : ++NPC.frameCounter % (60 * 8) < 60 ? PoseFrame
                : NpcSheet.IdleFrame;
        NPC.frame.Y = frame * frameHeight;
    }

    protected static ChuninExamPlayer Exam => Main.LocalPlayer.GetModPlayer<ChuninExamPlayer>();
}

// Mitarashi Anko at the gate of Training Ground 44: the rules, and one of the two scrolls.
public sealed class Anko : ExamProctor
{
    protected override Vector2? Feet => ExamSiteWorld.AnkoFeet;

    public override string GetChat()
    {
        // The first time after the player met Orochimaru in the forest (Anko bears his curse mark).
        if (Exam.OrochimaruDone && !Exam.AnkoHeardOrochimaru)
        {
            Exam.AnkoHeardOrochimaru = true;
            return "……你说什么？长头发，蛇一样的眼睛？\n\n" +
                   "（她的手不自觉地按住了脖子后面）……那家伙的事，交给我。你只管往塔走——那不是你能应付的对手。";
        }
        return Chat();
    }

    private static string Chat() => Exam.Stage switch
    {
        ExamStage.ForestGate =>
            "哟，又来一个送死的。我是第二试的监考官，御手洗红豆。\n\n" +
            "规矩简单：一人一卷，天之卷或地之卷。从别人手里抢到另一卷，两卷一起带进森林最深处的中央塔。" +
            "不限时——不过在这片林子里待得越久，就越容易变成虫子的饭。",
        ExamStage.ForestHunt =>
            "还在这儿磨蹭？中央塔在林子最深处。……别让我进去给你收尸啊。",
        < ExamStage.ForestGate =>
            "第二试的考场。笔试都没过的小鬼，别往里凑。",
        _ => "居然活着出来了？……呵，有点意思。",
    };

    public override void SetChatButtons(ref string button, ref string button2)
    {
        if (Exam.Stage == ExamStage.ForestGate)
            button = "领取卷轴";
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
        if (!firstButton || Exam.Stage != ExamStage.ForestGate)
            return;
        Exam.IssueScroll();
        string mine = ChuninExamPlayer.ScrollName(Exam.Issued);
        Main.npcChatText = $"（甩过来一个卷轴）{mine}，拿好。\n\n……路上可别偷看里面写了什么哦？会死人的。";
    }
}

// Gekkō Hayate in the tower hall: the prelims start when the candidate is ready.
public sealed class Hayate : ExamProctor
{
    protected override Vector2? Feet => ExamSiteWorld.HayateFeet;

    public override string GetChat() => Exam.Stage switch
    {
        ExamStage.Prelims =>
            "（咳）……我是预选赛的主考官，月光疾风。\n\n" +
            "通过第二试的人太多了，所以要在这里先比一场。一对一，打到一方倒下为止。……准备好了，就跟我说。",
        > ExamStage.Prelims => "（咳）……恭喜。正式赛在一个月后，好好准备。",
        _ => "（咳）……这里是第二试的终点。带齐天、地两卷再来。",
    };

    public override void SetChatButtons(ref string button, ref string button2)
    {
        if (Exam.Stage == ExamStage.Prelims && !ExamBoutSystem.BoutUnderway)
            button = "开始预选赛";
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
        if (!firstButton || Exam.Stage != ExamStage.Prelims)
            return;
        ExamBoutSystem.RequestPrelims(Main.LocalPlayer);
        NpcChatCloser.CloseNextTick();
    }
}
