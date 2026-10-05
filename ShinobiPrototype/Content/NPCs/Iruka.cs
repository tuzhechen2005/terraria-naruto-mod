using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ShinobiPrototype.Common;
using ShinobiPrototype.Common.Players;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items;
using ShinobiPrototype.Content.Items.Tier1;

namespace ShinobiPrototype.Content.NPCs;

// Iruka at the mission desk (specs/空档衔接与火影小兵.spec.md; user 2026-10-04): a town NPC who moves into the Leaf from
// the start. "Mission" takes a D-rank mission, reports one or hands in what was gathered (MissionPlayer); "Exchange"
// opens a shop priced in mission tokens (MissionTokenCurrency). Art: iruka-npc-v3 (scripts/build_npc_sheet.py).
[AutoloadHead]
public sealed class Iruka : ModNPC
{
    private const string ShopName = "Exchange";

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = NpcSheet.FrameCount;
        // At home in the Leaf; fond of Kakashi's genin (a former pupil's teacher), wary of the Goblin Tinkerer's prices.
        NPC.Happiness
            .SetBiomeAffection<KonohaBiome>(Terraria.GameContent.Personalities.AffectionLevel.Love)
            .SetBiomeAffection<Terraria.GameContent.Personalities.ForestBiome>(Terraria.GameContent.Personalities.AffectionLevel.Like)
            .SetNPCAffection<Kakashi>(Terraria.GameContent.Personalities.AffectionLevel.Like)
            .SetNPCAffection<ToolShopkeeper>(Terraria.GameContent.Personalities.AffectionLevel.Like)
            .SetNPCAffection(NPCID.GoblinTinkerer, Terraria.GameContent.Personalities.AffectionLevel.Dislike);
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
        MissionPlayer mission = Main.LocalPlayer.GetModPlayer<MissionPlayer>();
        if (mission.Kind != MissionKind.None && Main.rand.NextBool())
            return mission.Describe();
        var lines = new List<string>();
        for (int i = 1; i <= 5; i++)
            lines.Add(Loc.Get($"Iruka.Line{i}"));
        if (StoryWorld.WaveComplete)
            lines.Add(Loc.Get("Iruka.AfterWave"));
        return Main.rand.Next(lines);
    }

    public override void SetChatButtons(ref string button, ref string button2)
    {
        MissionPlayer mission = Main.LocalPlayer.GetModPlayer<MissionPlayer>();
        button = Loc.Get(mission.Kind == MissionKind.None ? "Iruka.TakeMission" : "Iruka.ReportMission");
        button2 = Loc.Get("Iruka.Exchange");
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
        if (firstButton)
            Main.npcChatText = Main.LocalPlayer.GetModPlayer<MissionPlayer>().AtDesk();
        else
            shopName = ShopName;
    }

    // Prices in mission tokens; a mission pays one to three.
    public override void AddShops()
    {
        var shop = new NPCShop(Type, ShopName);
        AddToken(shop, ModContent.ItemType<ToolBlueprint>(), 2);
        AddToken(shop, ModContent.ItemType<ChakraPill>(), 1);
        AddToken(shop, ModContent.ItemType<ChakraCrystal>(), 15);
        foreach (int dye in new[] { ItemID.RedDye, ItemID.OrangeDye, ItemID.GreenDye, ItemID.BlueDye, ItemID.SilverDye })
            AddToken(shop, dye, 2);
        shop.Register();
    }

    private static void AddToken(NPCShop shop, int type, int tokens)
    {
        var item = new Item(type) { shopCustomPrice = tokens, shopSpecialCurrency = MissionTokenCurrency.Id };
        shop.Add(item);
    }

    public override void TownNPCAttackStrength(ref int damage, ref float knockback)
    {
        damage = 12;
        knockback = 3f;
    }

    public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
    {
        cooldown = 25;
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
