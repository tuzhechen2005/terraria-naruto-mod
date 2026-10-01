using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items;

namespace ShinobiPrototype.Content.NPCs;

// Tazuna before the win: stands in his hut by the unfinished bridge, cannot be hurt (like the Dungeon's Old Man)
// and is not a town NPC. WaveBridgeWorld re-places him when missing and removes him once the bridge is finished.
// Shares the town Tazuna's sprite sheet.
public sealed class TazunaBridge : ModNPC
{
    public override string Texture => "ShinobiPrototype/Content/NPCs/Tazuna";

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = NpcSheet.FrameCount;
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
        NPC.npcSlots = 0f;
        NPC.scale = NpcSheet.ScaleFor(46);
    }

    public override bool CanChat() => true;

    public override bool CheckActive() => false;

    private static readonly string[] Lines =
    {
        "我是造桥的达兹纳。这座桥修通了，波之国才能活下去。",
        "卡多那家伙封锁了海路，船进不来，大家连饭都快吃不上了。",
        "雾隐的忍者常在夜里和下雨时出没，就在这片海边。",
        "桥断在那里好久了……工人们都不敢来了。",
        "这座桥全是石头砌的。一块一块，都是镇上的人扛上去的。",
        "起重机上还吊着一块石头呢——那天工人们一看见雾里的人影，扔下就跑了。",
        "我外孙伊那利说，这世上根本没有英雄。……我倒希望他是错的。",
        "（喝了一口酒）哈……别看我这样，我可是这个国家最好的造桥工。",
        "对面小岛上立着鸟居。以前渔民出海前，都会去拜一拜。",
    };

    private const string FirstMeeting =
        "哦？你就是卡卡西带来的忍者？我是造桥的达兹纳，这个国家最好的造桥工。" +
        "这座桥修通了，波之国才能活下去……路上小心点，海边这一带不太平。";

    private const string Confession =
        "……鬼之兄弟？连雾隐的中忍都来了……看来瞒不住了。\n" +
        "委托的时候我说了谎。这根本不是什么 C 级任务——海运大亨卡多想要我的命，他雇了忍者。\n" +
        "桥修通了，他对波之国的封锁就完了。可这个穷国家……付不起 A 级任务的钱。拜托了，忍者。";

    public override string GetChat()
    {
        Player player = Main.LocalPlayer;
        switch (player.GetModPlayer<StoryPlayer>().WaveStage)
        {
            case WaveStage.FindTazuna when !StoryWorld.MetTazuna:
                StoryWorld.RecordTazunaTalk(false);
                return FirstMeeting;
            case WaveStage.ReportToTazuna:
                StoryWorld.RecordTazunaTalk(true);
                Main.NewText("任务等级上调为 A 级。", 255, 190, 90);
                return Confession;
        }

        List<string> situational = new();
        if (!Main.dayTime)
            situational.Add("天黑了，雾更浓了。晚上最好别一个人上桥。");
        if (Main.raining)
            situational.Add("下雨天，雾隐的人最爱出来……你小心点。");
        switch (player.GetModPlayer<StoryPlayer>().WaveStage)
        {
            case WaveStage.GetStronger:
                situational.Add("卡卡西先生说，卡多雇来的是雾隐的鬼人……你可别逞强，先把本事练好。");
                break;
            case WaveStage.Lake:
                situational.Add("回镇上的路要经过湖边……那种地方起了雾，可就什么都看不见了。");
                break;
            case WaveStage.Bridge:
                situational.Add("你说再不斩被人带走了？……（脸色发白）那家伙要是还活着，这座桥就永远修不成了。");
                break;
        }
        if (player.GetModPlayer<MistEncounterPlayer>().SawPreview)
            situational.Add("你也看见了吧？雾里那两个人……就是卡多雇来的忍者。桥一天修不完，他们就守在那里。");
        if (player.CountItem(ModContent.ItemType<MistInsignia>()) >= 3)
            situational.Add("你身上那是雾隐的标记？……看来你是认真的。拜托了，忍者。");
        return situational.Count > 0 && Main.rand.NextBool()
            ? Main.rand.Next(situational)
            : Main.rand.Next(Lines);
    }

    public override void AI()
    {
        NPC.velocity.X = 0f;
        Player nearest = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)];
        NPC.direction = nearest.Center.X >= NPC.Center.X ? 1 : -1;
        NPC.spriteDirection = NPC.direction;
    }

    public override void FindFrame(int frameHeight) => NPC.frame.Y = NpcSheet.IdleFrame * frameHeight;
}
