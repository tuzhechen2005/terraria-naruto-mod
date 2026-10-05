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
// (anko-hires-full-v1, hayate-hires-full-v1: high-res at 1x, the walking slots repeat the idle); without it they borrow Kakashi's sheet.
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
        NPC.scale = 1f;   // their own sheets and the borrowed Kakashi sheet are all drawn at their final size
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
            return Loc.Get("Anko.HeardOrochimaru");
        }
        return Chat();
    }

    private static string Chat() => Exam.Stage switch
    {
        ExamStage.ForestGate => Loc.Get("Anko.Rules"),
        ExamStage.ForestHunt => Loc.Get("Anko.Hurry"),
        < ExamStage.ForestGate => Loc.Get("Anko.NotYet"),
        _ => Loc.Get("Anko.Survived"),
    };

    public override void SetChatButtons(ref string button, ref string button2)
    {
        if (Exam.Stage == ExamStage.ForestGate)
            button = Loc.Get("Anko.ButtonScroll");
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
        if (!firstButton || Exam.Stage != ExamStage.ForestGate)
            return;
        Exam.IssueScroll();
        Main.npcChatText = Loc.Get("Anko.Issue", ChuninExamPlayer.ScrollName(Exam.Issued));
    }
}

// Gekkō Hayate in the tower hall: the prelims start when the candidate is ready.
public sealed class Hayate : ExamProctor
{
    protected override Vector2? Feet => ExamSiteWorld.HayateFeet;

    public override string GetChat() => Exam.Stage switch
    {
        ExamStage.Prelims => Loc.Get("Hayate.Prelims"),
        > ExamStage.Prelims => Loc.Get("Hayate.Passed"),
        _ => Loc.Get("Hayate.NotYet"),
    };

    public override void SetChatButtons(ref string button, ref string button2)
    {
        if (Exam.Stage == ExamStage.Prelims && !ExamBoutSystem.BoutUnderway)
            button = Loc.Get("Hayate.ButtonStart");
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
        if (!firstButton || Exam.Stage != ExamStage.Prelims)
            return;
        ExamBoutSystem.RequestPrelims(Main.LocalPlayer);
        NpcChatCloser.CloseNextTick();
    }
}
