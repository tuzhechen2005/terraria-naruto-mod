using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShinobiPrototype.Content.NPCs;

// Tazuna before the win: stands in his hut by the unfinished bridge, cannot be hurt (like the Dungeon's Old Man)
// and is not a town NPC. WaveBridgeWorld re-places him when missing and removes him once the bridge is finished.
// Borrows the Merchant's sprite until his own art exists.
public sealed class TazunaBridge : ModNPC
{
    public override string Texture => $"Terraria/Images/NPC_{NPCID.Merchant}";

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Merchant];
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

    public override string GetChat() => Main.rand.Next(new[]
    {
        "我是造桥的达兹纳。这座桥修通了，波之国才能活下去。",
        "卡多那家伙封锁了海路，船进不来，大家连饭都快吃不上了。",
        "雾隐的忍者常在夜里和下雨时出没，就在这片海边。",
        "桥断在那里好久了……工人们都不敢来了。",
    });

    public override void AI()
    {
        NPC.velocity.X = 0f;
        Player nearest = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)];
        NPC.direction = nearest.Center.X >= NPC.Center.X ? 1 : -1;
        NPC.spriteDirection = NPC.direction;
    }

    public override void FindFrame(int frameHeight) => NPC.frame.Y = 0;
}
