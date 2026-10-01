using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Common.Systems;

namespace ShinobiPrototype.Content.NPCs;

// Sarutobi Hiruzen, the Third Hokage: a story NPC who stands in his office in the Hokage tower (no house, cannot be
// hurt) and hears the vow (立志, specs/流派系统.spec.md section 6). Art: hiruzen-npc-v2 (scripts/build_npc_sheet.py), the
// twelve NpcSheet frames plus two of his own: a puff on the pipe, and a raised hand while someone talks to him.
public sealed class Hiruzen : ModNPC
{
    private const int PuffFrame = NpcSheet.FrameCount;
    private const int TalkFrame = NpcSheet.FrameCount + 1;
    private const int PuffEvery = 60 * 7;
    private const int PuffTicks = 50;

    // The fee was named in this conversation: the next vow request is the confirmation.
    private static bool changePending;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = NpcSheet.FrameCount + 2;
        NPCID.Sets.NoTownNPCHappiness[Type] = true;
        NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
    }

    public override void SetDefaults()
    {
        NPC.width = 18;
        NPC.height = 40;
        NPC.aiStyle = -1;
        NPC.friendly = true;
        NPC.dontTakeDamage = true;
        NPC.lifeMax = 250;
        NPC.defense = 15;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.scale = NpcSheet.ScaleFor(46);
    }

    public override bool CanChat() => true;

    public override bool CheckActive() => false;

    public override void AI()
    {
        if (KonohaWorld.HokageFeet is Vector2 feet)
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
        bool talking = Main.LocalPlayer.talkNPC == NPC.whoAmI;
        int frame = talking ? TalkFrame
            : ++NPC.frameCounter % PuffEvery < PuffTicks ? PuffFrame
            : NpcSheet.IdleFrame;
        if (frame == PuffFrame && NPC.frameCounter % PuffEvery == PuffTicks / 2 && Main.netMode != NetmodeID.Server)
            Dust.NewDustPerfect(NPC.Top + new Vector2(NPC.direction * 14f, 14f), DustID.Smoke, new Vector2(0f, -0.6f), 120,
                default, 1.1f);
        NPC.frame.Y = frame * frameHeight;
    }

    public override string GetChat()
    {
        changePending = false;
        Player player = Main.LocalPlayer;
        StyleSchool vow = player.GetModPlayer<StyleCorePlayer>().Vow;
        string[] greetings =
        {
            "（吐出一口烟）……来了啊。",
            "村子里的每一个人，都是我的家人。你也一样。",
            "火之意志，会一直传下去的。",
        };
        if (vow == StyleSchool.None && player.GetModPlayer<StyleCorePlayer>().VowCalled)
            return "……你来了。听卡卡西说，你已经摸到了一条路的门槛。\n\n" +
                   "忍者要走哪一条路，得自己决定。想好了，就戴着那个流派的核心，点“立志”。" +
                   "仙术要等木叶崩溃之后才能取得——想走那条路的话，也可以等。";
        string status = vow == StyleSchool.None
            ? "你还没有立志。取得流派核心之后，戴着它来找我。"
            : $"你的忍道：{VowRules.SchoolName(vow)}。";
        return $"{Main.rand.Next(greetings)}\n\n{status}";
    }

    public override void SetChatButtons(ref string button, ref string button2)
    {
        button = "立志";
        button2 = "关于流派";
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
        Main.npcChatText = firstButton ? Vow(Main.LocalPlayer) : AboutSchools();
    }

    private static string Vow(Player player)
    {
        StyleCorePlayer styles = player.GetModPlayer<StyleCorePlayer>();
        StyleSchool worn = styles.School;
        long coins = Utils.CoinsCount(out _, player.inventory);
        VowOutcome outcome = VowRules.Evaluate(styles.Vow, worn, changePending, coins);
        changePending = false;
        switch (outcome)
        {
            case VowOutcome.NoCore:
                return "立志，要有能立的东西。流派核心的第一阶，都在中忍考试到木叶崩溃之间：" +
                       "写轮眼在死亡森林，白眼在宁次那里，八门在我爱罗那一战。\n\n" +
                       "仙术要等木叶崩溃之后才能取得——想走那条路的话，可以等。\n\n" +
                       "取得了核心，就戴着它来找我。";
            case VowOutcome.AlreadyVowed:
                return $"你早已立下了志向：{VowRules.SchoolName(worn)}。……把它走到底吧。";
            case VowOutcome.Vow:
                styles.Vow = worn;
                return $"……好。从今天起，{VowRules.SchoolName(worn)}就是你的忍道。\n\n" +
                       $"属于这条路的修行——“{VowRules.ChapterName(worn)}”——以后会找上你的。（本命章节开发中）\n\n" +
                       "仙术要等木叶崩溃之后才能取得。将来若改了主意，也可以再来找我。";
            case VowOutcome.NeedsFee:
                changePending = true;
                return $"想从{VowRules.SchoolName(styles.Vow)}改投{VowRules.SchoolName(worn)}？\n\n" +
                       $"可以。但要交 {VowRules.ChangeFee / 10000} 金币——就当是给村子的修缮费。" +
                       "之前那条路上得到的东西会留在你身上，只是不再起作用。\n\n想清楚了，再点一次“立志”。";
            case VowOutcome.CannotPay:
                return $"改投要 {VowRules.ChangeFee / 10000} 金币。钱不够的话，先去攒一攒吧。";
            default:
                player.BuyItem(VowRules.ChangeFee);
                StyleSchool old = styles.Vow;
                styles.Vow = worn;
                return $"……好。放下{VowRules.SchoolName(old)}，走{VowRules.SchoolName(worn)}这条路。\n\n" +
                       $"“{VowRules.ChapterName(worn)}”的修行，以后会找上你的。";
        }
    }

    private static string AboutSchools() =>
        "流派是跨职业的修行之路，靠“流派核心”承载。核心谁都能戴，一次只能戴一个，按【流派奥义】键施展。\n\n" +
        "立志，是认定其中一条作为自己的忍道。认定之后，会有只属于这条路的修行等着你：\n" +
        "· 写轮眼——咒印　· 八门——凯的修行\n· 白眼——日向宗家与分家　· 仙术——妙木山\n\n" +
        "仙术要等木叶崩溃之后才能取得。改投也可以，但要付出代价。";
}
