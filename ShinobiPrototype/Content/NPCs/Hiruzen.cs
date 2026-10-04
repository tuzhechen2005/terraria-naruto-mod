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
        // A short old man: a touch smaller than the others (user, 2026-10-01).
        NPC.scale = NpcSheet.ScaleFor(46) * 0.92f;
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
        if (vow == StyleSchool.None && player.GetModPlayer<StyleCorePlayer>().VowCalled)
            return Loc.Get("Hiruzen.Called");
        string status = vow == StyleSchool.None ? Loc.Get("Hiruzen.NoVow") : Loc.Get("Hiruzen.YourPath", School(vow));
        return Loc.Pick("Hiruzen.Greeting", 3) + "\n\n" + status;
    }

    private static string School(StyleSchool school) => Loc.Get(VowRules.SchoolKey(school));

    private static string Chapter(StyleSchool school) => Loc.Get(VowRules.ChapterKey(school));

    public override void SetChatButtons(ref string button, ref string button2)
    {
        button = Loc.Get("Hiruzen.ButtonVow");
        button2 = Loc.Get("Hiruzen.ButtonAbout");
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
                return Loc.Get("Hiruzen.NoCore");
            case VowOutcome.AlreadyVowed:
                return Loc.Get("Hiruzen.AlreadyVowed", School(worn));
            case VowOutcome.Vow:
                styles.Vow = worn;
                return Loc.Get("Hiruzen.Vowed", School(worn), Chapter(worn));
            case VowOutcome.NeedsFee:
                changePending = true;
                return Loc.Get("Hiruzen.NeedsFee", School(styles.Vow), School(worn), VowRules.ChangeFee / 10000);
            case VowOutcome.CannotPay:
                return Loc.Get("Hiruzen.CannotPay", VowRules.ChangeFee / 10000);
            default:
                player.BuyItem(VowRules.ChangeFee);
                StyleSchool old = styles.Vow;
                styles.Vow = worn;
                return Loc.Get("Hiruzen.Changed", School(old), School(worn), Chapter(worn));
        }
    }

    private static string AboutSchools() => Loc.Get("Hiruzen.About");
}
