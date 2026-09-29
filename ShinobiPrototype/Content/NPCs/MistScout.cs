using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Content.Items;

namespace ShinobiPrototype.Content.NPCs;

public sealed class MistScout : ModNPC
{
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 1;
    }

    public override void SetDefaults()
    {
        NPC.width = 26;
        NPC.height = 46;
        NPC.damage = 13;
        NPC.defense = 1;
        NPC.lifeMax = 70;
        NPC.knockBackResist = 0.45f;
        NPC.aiStyle = -1;
        NPC.value = 30f;
        NPC.color = new Color(100, 160, 190);
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        Player player = spawnInfo.Player;
        if (!player.ZoneOverworldHeight ||
            Math.Abs(player.Center.X - Main.spawnTileX * 16f) < 850f)
            return 0f;

        return 0.22f;
    }

    public override void AI()
    {
        NPC.TargetClosest();
        Player target = Main.player[NPC.target];
        if (!target.active || target.dead)
            return;

        float distance = target.Center.X - NPC.Center.X;
        NPC.direction = distance >= 0 ? 1 : -1;
        NPC.spriteDirection = NPC.direction;
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * 1.9f, 0.06f);

        if (NPC.collideX && NPC.velocity.Y == 0f)
            NPC.velocity.Y = -6f;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MistInsignia>()));
    }
}
