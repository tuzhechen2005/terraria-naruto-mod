using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items;

namespace ShinobiPrototype.Content.NPCs;

// Tazuna after the win: an ordinary town NPC who first moves into his finished hut by the bridge.
// Art: tazuna-npc-v1 (scripts/build_npc_sheet.py), 12-frame NpcSheet layout.
[AutoloadHead]
public sealed class Tazuna : ModNPC
{
    private const string ShopName = "Shop";

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = NpcSheet.FrameCount;
        NPCID.Sets.DangerDetectRange[Type] = 500;
        NPCID.Sets.AttackType[Type] = 0;
        NPCID.Sets.AttackTime[Type] = 60;
        NPCID.Sets.AttackAverageChance[Type] = 30;
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
    }

    public override void FindFrame(int frameHeight) => NpcSheet.Animate(NPC, frameHeight);

    public override bool CanTownNPCSpawn(int numTownNPCs) => StoryWorld.WaveComplete;

    public override List<string> SetNPCNameList() => new() { this.GetLocalizedValue("GivenName") };

    public override string GetChat() => Main.rand.Next(new[]
    {
        "桥修好了！多亏了你们。要买点什么？",
        "船又能进港了，市场也热闹起来了。",
        "那座桥啊，大家都叫它“鸣人大桥”。",
        "再不斩和那个孩子……说到底，他们也是被这个世道逼的吧。",
        "伊那利现在天天跑到桥上去，说长大了要当英雄。",
        "卡多一倒，他的手下全跑光了。这个国家总算能喘口气了。",
        "要木材？我这儿多的是，造桥剩下的。",
        "听说木叶要办中忍考试？你也要去吧，加油啊。",
    });

    public override void SetChatButtons(ref string button, ref string button2) =>
        button = Language.GetTextValue("LegacyInterface.28");

    public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
        if (firstButton)
            shopName = ShopName;
    }

    public override void AddShops()
    {
        new NPCShop(Type, ShopName)
            .Add<ChakraPill>()
            .Add(ItemID.Wood)
            .Add(ItemID.Rope)
            .Add(ItemID.WoodPlatform)
            .Register();
    }

    public override void TownNPCAttackStrength(ref int damage, ref float knockback)
    {
        damage = 12;
        knockback = 4f;
    }

    public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
    {
        cooldown = 30;
        randExtraCooldown = 20;
    }

    public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
    {
        projType = ProjectileID.ThrowingKnife;
        attackDelay = 8;
    }

    public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
    {
        multiplier = 9f;
        randomOffset = 1f;
    }
}
