using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Systems;
using ShinobiPrototype.Content.Items;
using ShinobiPrototype.Common;

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
        // Loves the sea, dislikes the desert; likes Kakashi; dealers remind him of Gato.
        NPC.Happiness
            .SetBiomeAffection<Terraria.GameContent.Personalities.OceanBiome>(Terraria.GameContent.Personalities.AffectionLevel.Love)
            .SetBiomeAffection<Terraria.GameContent.Personalities.DesertBiome>(Terraria.GameContent.Personalities.AffectionLevel.Dislike)
            .SetNPCAffection<Kakashi>(Terraria.GameContent.Personalities.AffectionLevel.Like)
            .SetNPCAffection(NPCID.Merchant, Terraria.GameContent.Personalities.AffectionLevel.Dislike);
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
        NPC.scale = NpcSheet.ScaleFor(46);
    }

    public override void FindFrame(int frameHeight) => NpcSheet.Animate(NPC, frameHeight);

    public override bool CanTownNPCSpawn(int numTownNPCs) => StoryWorld.WaveComplete;

    public override List<string> SetNPCNameList() => new() { this.GetLocalizedValue("GivenName") };

    public override string GetChat() => Loc.Pick("Tazuna.Line", 9);

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
