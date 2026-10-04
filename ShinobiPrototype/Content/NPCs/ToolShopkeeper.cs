using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items;
using ShinobiPrototype.Content.Items.NinjaTools;
using ShinobiPrototype.Content.Items.Weapons;

namespace ShinobiPrototype.Content.NPCs;

// The ninja tool shop (specs/装备与忍术系统.spec.md; user, 2026-10-03: Tenten's father): a town NPC who moves into the
// Leaf from the start. He sells the thrown ninja tools and soldier pills from the first day, senbon once Zabuza has
// fallen; once the Chūnin Exams are over Tenten comes to help with more ninja tools (with tier two). Art:
// tool-shop-npc-v1 (scripts/build_npc_sheet.py, 12-frame NpcSheet, same body height as Tazuna). His head texture is his
// own file: tModLoader refuses to load a head shared between two NPCs.
[AutoloadHead]
public sealed class ToolShopkeeper : ModNPC
{
    private const string ShopName = "Shop";

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = NpcSheet.FrameCount;
        // At home in the Leaf; likes Kakashi (a good customer), finds the Arms Dealer's guns crude.
        NPC.Happiness
            .SetBiomeAffection<KonohaBiome>(Terraria.GameContent.Personalities.AffectionLevel.Love)
            .SetBiomeAffection<Terraria.GameContent.Personalities.ForestBiome>(Terraria.GameContent.Personalities.AffectionLevel.Like)
            .SetNPCAffection<Kakashi>(Terraria.GameContent.Personalities.AffectionLevel.Like)
            .SetNPCAffection(NPCID.ArmsDealer, Terraria.GameContent.Personalities.AffectionLevel.Dislike);
        NPCID.Sets.DangerDetectRange[Type] = 500;
        NPCID.Sets.AttackType[Type] = 0;
        NPCID.Sets.AttackTime[Type] = 40;
        NPCID.Sets.AttackAverageChance[Type] = 20;
    }

    public override void SetDefaults()
    {
        NPC.townNPC = true;
        NPC.friendly = true;
        NPC.width = 18;
        NPC.height = 40;
        NPC.aiStyle = NPCAIStyleID.Passive;
        NPC.damage = 12;
        NPC.defense = 15;
        NPC.lifeMax = 250;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.5f;
        NPC.scale = NpcSheet.ScaleFor(46);
    }

    public override void FindFrame(int frameHeight) => NpcSheet.Animate(NPC, frameHeight);

    public override bool CanTownNPCSpawn(int numTownNPCs) => true;

    public override List<string> SetNPCNameList() => new() { this.GetLocalizedValue("GivenName") };

    public override string GetChat()
    {
        var lines = new List<string>
        {
            "欢迎光临！苦无、手里剑、起爆符，木叶最齐全的就是我这儿。",
            "忍具要拿在手里用熟了，出手时才藏得住杀气——潜伏投掷就是这么回事。",
            "起爆符别揣在怀里乱跑，贴错地方可不是闹着玩的。",
            "我家那丫头天天，整天嚷嚷着要当像纲手大人那样的忍者。",
            "手里剑碰墙会弹回来，角度算好了，能打到墙后面的人。",
        };
        if (StoryWorld.WaveComplete)
            lines.Add("听说你从波之国带回了雾隐的千本？我这儿也进了一批，比那个白用的差不了多少。");
        if (StoryWorld.DownedGaara)
            lines.Add("中忍考试辛苦了！天天说她在会场看到你了，回头让她来店里帮忙。");
        return Main.rand.Next(lines);
    }

    public override void SetChatButtons(ref string button, ref string button2) =>
        button = Language.GetTextValue("LegacyInterface.28");

    public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
        if (firstButton)
            shopName = ShopName;
    }

    public override void AddShops()
    {
        var afterWave = new Condition(Language.GetText("Mods.ShinobiPrototype.Conditions.WaveComplete"), () => StoryWorld.WaveComplete);
        new NPCShop(Type, ShopName)
            .Add<Kunai>()
            .Add<Shuriken>()
            .Add<PaperBomb>()
            .Add<ChakraPill>()
            .Add<Senbon>(afterWave)
            .Register();
    }

    public override void TownNPCAttackStrength(ref int damage, ref float knockback)
    {
        damage = 14;
        knockback = 3f;
    }

    public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
    {
        cooldown = 20;
        randExtraCooldown = 15;
    }

    public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
    {
        projType = ModContent.ProjectileType<Projectiles.KunaiThrown>();
        attackDelay = 6;
    }

    public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
    {
        multiplier = 11f;
        randomOffset = 0.5f;
    }
}
