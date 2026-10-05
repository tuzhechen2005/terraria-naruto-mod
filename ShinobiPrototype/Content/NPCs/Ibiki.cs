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
// its own felt strange). A story NPC like the Third Hokage: no house, cannot be hurt. Art: ibiki-hires-full-v1 (high-res at 1x, 80-pixel frames; the walking slots repeat the folded arms), the twelve
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
        // Drawn at its final size (ibiki-npc-v4, 64-pixel frames), shown at 1x: no blocky upscaling (user, 2026-10-01).
        NPC.scale = 1f;
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
            ExamStage.Written when !exam.CanSitWritten => Loc.Get("Ibiki.GaveUp"),
            ExamStage.Written when !Main.LocalPlayer.HasItem(ModContent.ItemType<ExamAdmissionScroll>()) =>
                Loc.Get("Ibiki.NoRecommendation"),
            ExamStage.Written => Loc.Get("Ibiki.Welcome"),
            ExamStage.Recommend or ExamStage.Locked or ExamStage.NoVillage => Loc.Get("Ibiki.NotYet"),
            _ => Loc.Get("Ibiki.Passed"),
        };
    }

    public override void SetChatButtons(ref string button, ref string button2)
    {
        ChuninExamPlayer exam = Main.LocalPlayer.GetModPlayer<ChuninExamPlayer>();
        if (exam.Stage == ExamStage.Written && exam.CanSitWritten &&
            Main.LocalPlayer.HasItem(ModContent.ItemType<ExamAdmissionScroll>()))
            button = Loc.Get("Ibiki.ButtonStart");
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
        if (!firstButton)
            return;
        NpcChatCloser.CloseNextTick();
        CombatText.NewText(NPC.getRect(), new Color(220, 220, 220), Loc.Get("Ibiki.Begin"), true);
        WrittenExamSystem.Open();
    }
}
