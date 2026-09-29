using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
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
        "委托的时候我说了谎，这根本不是什么 C 级任务。……抱歉，可我付不起更多的钱了。",
        "对面小岛上立着鸟居。以前渔民出海前，都会去拜一拜。",
    };

    public override string GetChat()
    {
        List<string> situational = new();
        if (!Main.dayTime)
            situational.Add("天黑了，雾更浓了。晚上最好别一个人上桥。");
        if (Main.raining)
            situational.Add("下雨天，雾隐的人最爱出来……你小心点。");
        if (Main.LocalPlayer.GetModPlayer<MistEncounterPlayer>().SawPreview)
            situational.Add("你也看见了吧？雾里那两个人……就是卡多雇来的忍者。桥一天修不完，他们就守在那里。");
        if (Main.LocalPlayer.CountItem(ModContent.ItemType<MistInsignia>()) >= 3)
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
